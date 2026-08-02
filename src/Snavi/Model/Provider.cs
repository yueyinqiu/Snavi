namespace Snavi.Model;

public sealed record Provider(string Type, string Path)
{
    public static readonly IReadOnlyList<string> KnownTypes = ["csharp", "sh", "exec"];
}
