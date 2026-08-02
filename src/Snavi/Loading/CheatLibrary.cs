using Snavi.Model;

namespace Snavi.Loading;

public sealed record CheatEntry(string Path, CheatFile Cheat);

public sealed class CheatLibrary
{
    private readonly List<CheatEntry> _cheats;

    public CheatLibrary(string directory)
    {
        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException($"cheat 目录不存在: {directory}");
        _cheats = Directory
            .EnumerateFiles(directory, "*.json", SearchOption.AllDirectories)
            .OrderBy(path => path, StringComparer.Ordinal)
            .Select(path => new CheatEntry(path, CheatSerializer.Load(path)))
            .ToList();
    }

    public IReadOnlyList<CheatEntry> Cheats => _cheats;
}
