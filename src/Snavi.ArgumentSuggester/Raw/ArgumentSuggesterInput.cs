namespace Snavi.ArgumentSuggester.Raw;

public sealed record ArgumentSuggesterInput(
    string OutputFilePath,
    string TemporaryDirectoryPath,
    IReadOnlyList<string> GivenArguments
);