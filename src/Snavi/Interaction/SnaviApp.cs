using System.Text;
using Snavi.Loading;
using Snavi.Model;
using Snavi.Rendering;

namespace Snavi.Interaction;

public sealed class SnaviApp
{
    private readonly CheatLibrary _library;
    private readonly IUi _ui;

    public SnaviApp(CheatLibrary library, IUi ui)
    {
        _library = library;
        _ui = ui;
    }

    public string? Run()
    {
        var entries = _library.Cheats;
        if (entries.Count == 0)
        {
            Console.WriteLine("没有找到任何 cheat");
            return null;
        }

        var cheatChoice = _ui.Pick("选择命令", entries.Select(e => new PickerItem(ToTemplate(e.Cheat), e.Cheat.Description)).ToList());
        if (cheatChoice is null)
            return null;
        var entry = entries.First(e => ToTemplate(e.Cheat) == cheatChoice);
        var cheat = entry.Cheat;
        var cheatDir = Path.GetDirectoryName(entry.Path)!;

        var resolved = new Dictionary<string, string>();
        var tokens = new List<string>();
        for (var i = 0; i < cheat.Command.Count; i++)
        {
            switch (cheat.Command[i])
            {
                case Literal l:
                    tokens.Add(l.Value);
                    break;
                case Variable v:
                    var header = BuildHeader(cheat.Command, i, resolved);
                    var value = ResolveVariable(v, cheatDir, resolved, header);
                    if (value is null)
                        return null;
                    resolved[v.Name] = value;
                    tokens.Add(value);
                    break;
            }
        }

        var rendered = Renderer.Render(tokens);
        if (cheat.ExtraArguments)
        {
            var extra = _ui.Complete(rendered, "追加参数: ", []);
            if (extra is not null && extra.Length > 0)
                rendered = $"{rendered} {extra}";
        }
        return rendered;
    }

    private string? ResolveVariable(Variable v, string cheatDir, IReadOnlyDictionary<string, string> resolved, string header)
    {
        if (v.Provider is null)
            return _ui.Complete(header, $"{v.Name}: ", []);

        var results = ProviderRunner.Run(v.Provider, cheatDir, resolved, _ui.Warn);
        var suggestions = results.Select(r => new PickerItem(r.Display, r.Preview)).ToList();
        if (suggestions.Count == 0)
            _ui.Warn($"{v.Name} 的 provider 无可用选项，直接输入");
        return _ui.Complete(header, $"{v.Name}: ", suggestions);
    }

    private static string BuildHeader(IReadOnlyList<CommandToken> command, int currentIndex, IReadOnlyDictionary<string, string> resolved)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < command.Count; i++)
        {
            if (i > 0)
                sb.Append(' ');
            switch (command[i])
            {
                case Literal l:
                    sb.Append(l.Value);
                    break;
                case Variable v:
                    if (resolved.TryGetValue(v.Name, out var value))
                        sb.Append(value);
                    else if (i == currentIndex)
                        sb.Append($"\u001b[38;5;214m{{{v.Name}}}\u001b[0m");
                    else
                        sb.Append($"{{{v.Name}}}");
                    break;
            }
        }
        return sb.ToString();
    }

    private static string ToTemplate(CheatFile cheat) => string.Join(' ', cheat.Command.Select(t => t switch
    {
        Literal l => l.Value,
        Variable v => $"{{{v.Name}}}",
        _ => string.Empty,
    }));
}
