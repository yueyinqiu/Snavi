#:package Snavi.ArgumentSuggester@0.0.1

using Snavi.ArgumentSuggester;

await new Suggester().RunAsync();

class Suggester : SnaviArgumentSuggester
{
    public override IAsyncEnumerable<(string Value, string Description)> SuggestAsync(
        IReadOnlyList<string> givenArguments,
        DirectoryInfo currentDirectory,
        DirectoryInfo temporaryDirectory,
        CancellationToken cancellationToken
    )
    {
        return currentDirectory.EnumerateFiles()
            .Select(x => (x.FullName, ""))
            .ToAsyncEnumerable();
    }
}
