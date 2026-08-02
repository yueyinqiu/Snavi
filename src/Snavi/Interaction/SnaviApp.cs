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

    public void Run()
    {
        var entries = _library.Cheats;
        if (entries.Count == 0)
        {
            Console.WriteLine("没有找到任何 cheat");
            return;
        }

        // 1. 选大命令
        var choice = _ui.Pick("选择命令:", entries.Select(e => e.Cheat.Description).ToList());
        if (choice is null)
            return;
        var cheat = entries[choice.Value].Cheat;

        // 2. 顺序问变量
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
                    var value = ResolveVariable(v, resolved);
                    if (value is null)
                        return;
                    resolved[v.Name] = value;
                    tokens.Add(value);
                    break;
            }
        }

        // 3. 可选编辑
        var rendered = Renderer.Render(tokens);
        if (cheat.ExtraArgs)
        {
            var extra = _ui.Prompt($"{rendered}\n追加参数 (回车跳过): ");
            if (!string.IsNullOrWhiteSpace(extra))
                rendered = $"{rendered} {extra.Trim()}";
        }

        // 4. 渲染输出
        Console.WriteLine(rendered);
    }

    private string? ResolveVariable(Variable v, IReadOnlyDictionary<string, string> resolved)
    {
        if (v.Provider is null)
            return _ui.Prompt($"{v.Name}: ");

        var results = ProviderRunner.Run(v.Provider, resolved);
        var choice = _ui.Pick(v.Name, results.Select(r => r.Display).ToList());
        if (choice is null)
            return null;
        return results[choice.Value].Value;
    }
}
