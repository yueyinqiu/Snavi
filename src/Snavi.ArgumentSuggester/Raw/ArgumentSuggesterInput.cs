namespace Snavi.ArgumentSuggester.Raw;

public sealed record ArgumentSuggesterInput(
    string OutputFile,
    string CurrentDirectory,
    string TemporaryDirectory,
    IReadOnlyList<string> GivenArguments
);