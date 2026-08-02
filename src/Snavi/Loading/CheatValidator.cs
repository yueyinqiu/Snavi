using Snavi.Model;

namespace Snavi.Loading;

public static class CheatValidator
{
    public static void Validate(CheatFile cheat, string? source = null)
    {
        var context = source is null ? string.Empty : $"{source}: ";
        if (string.IsNullOrWhiteSpace(cheat.Description))
            throw new InvalidDataException($"{context}cheat 缺少 description");
        if (cheat.Command.Count == 0)
            throw new InvalidDataException($"{context}command 不能为空");

        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var token in cheat.Command)
        {
            switch (token)
            {
                case Variable v:
                    if (string.IsNullOrWhiteSpace(v.Name))
                        throw new InvalidDataException($"{context}variable 缺少 name");
                    if (names.Contains(v.Name))
                        throw new InvalidDataException($"{context}重复的变量名: {v.Name}");
                    ValidateProvider(v.Provider, source, context);
                    names.Add(v.Name);
                    break;
            }
        }
    }

    private static void ValidateProvider(Provider? provider, string? source, string context)
    {
        if (provider is null)
            return;
        if (!Provider.KnownTypes.Contains(provider.Type))
            throw new InvalidDataException($"{context}未知的 provider 类型: {provider.Type}（可选: {string.Join(", ", Provider.KnownTypes)}）");
        if (string.IsNullOrWhiteSpace(provider.Path))
            throw new InvalidDataException($"{context}provider 缺少 path");
        var cheatDir = source is null ? null : Path.GetDirectoryName(source);
        var path = Path.IsPathRooted(provider.Path)
            ? provider.Path
            : cheatDir is null ? provider.Path : Path.Combine(cheatDir, provider.Path);
        if (!File.Exists(path))
            throw new InvalidDataException($"{context}provider 文件不存在: {path}");
    }
}
