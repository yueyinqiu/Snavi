using System.Text.Json;
using Snavi.Model;

namespace Snavi.Loading;

public static class CheatSerializer
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static CheatFile Load(string path)
    {
        var json = File.ReadAllText(path);
        var cheat = JsonSerializer.Deserialize<CheatFile>(json, Options)
            ?? throw new InvalidDataException($"{path}: 无法解析 cheat 文件");
        CheatValidator.Validate(cheat, path);
        return cheat;
    }
}
