namespace Snavi.ArgumentSuggester.Raw;

public sealed record ArgumentSuggesterOutput(IReadOnlyList<ArgumentSuggesterOutput.Suggestion> Suggestions)
{
    public sealed record Suggestion(string Value, string Description);
}