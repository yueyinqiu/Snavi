namespace Snavi.ArgumentSuggester.Raw;

public sealed record ArgumentProviderOutput(IReadOnlyList<ArgumentProviderOutput.Completion> Completions)
{
    public sealed record Completion(string Value, string Description);
}