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

        var cheatChoice = _ui.Pick("选择命令:", entries.Select(e => new PickerItem(e.Cheat.Description, ToTemplate(e.Cheat))).ToList());
        if (cheatChoice is null)
            return null;
        var entry = entries[cheatChoice.Value];
        var cheat = entry.Cheat;
        var cheatDir = Path.GetDirectoryName(entry.Path)!;

        var resolved = new Dictionary<string, string>();
        var tokens = new List<string>();
        foreach (var token in cheat.Command)
        {
            switch (token)
            {
                case Literal l:
                    tokens.Add(l.Value);
                    break;
                case Variable v:
                    var value = ResolveVariable(v, cheatDir, resolved);
                    if (value is null)
                        return null;
                    resolved[v.Name] = value;
                    tokens.Add(value);
                    break;
            }
        }

        var rendered = Renderer.Render(tokens);
        if (cheat.ExtraArgs)
        {
            var extra = _ui.Prompt($"{rendered}\n追加参数 (回车跳过): ");
            if (!string.IsNullOrWhiteSpace(extra))
                rendered = $"{rendered} {extra.Trim()}";
        }
        return rendered;
    }

    private string? ResolveVariable(Variable v, string cheatDir, IReadOnlyDictionary<string, string> resolved)
    {
        if (v.Provider is null)
            return _ui.Prompt($"{v.Name}: ");

        var results = ProviderRunner.Run(v.Provider, cheatDir, resolved, _ui.Warn);
        if (results.Count == 0)
        {
            _ui.Warn($"{v.Name} 的 provider 无可用选项，改为手动输入");
            return _ui.Prompt($"{v.Name}: ");
        }
        var choice = _ui.Pick(v.Name, results.Select(r => new PickerItem(r.Display, r.Preview)).ToList());
        if (choice is null)
            return null;
        return results[choice.Value].Value;
    }

    private static string ToTemplate(CheatFile cheat) => string.Join(' ', cheat.Command.Select(t => t switch
    {
        Literal l => l.Value,
        Variable v => $"{{{v.Name}}}",
        _ => string.Empty,
    }));
}
