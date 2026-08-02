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
                    ValidateProvider(v.Provider, names, context);
                    names.Add(v.Name);
                    break;
                case Ref:
                    throw new InvalidDataException($"{context}ref 只能在 provider 中使用");
            }
        }
    }

    private static void ValidateProvider(Provider? provider, IReadOnlySet<string> names, string context)
    {
        if (provider is null)
            return;
        if (provider.Command.Count == 0)
            throw new InvalidDataException($"{context}provider 的 command 不能为空");
        foreach (var token in provider.Command)
        {
            switch (token)
            {
                case Variable:
                    throw new InvalidDataException($"{context}provider 中不能包含 variable");
                case Ref r when !names.Contains(r.Name):
                    throw new InvalidDataException($"{context}引用了不存在或尚未定义的变量: {r.Name}");
            }
        }
    }
}
