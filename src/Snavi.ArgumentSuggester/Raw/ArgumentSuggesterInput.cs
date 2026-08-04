namespace Snavi.ArgumentSuggester.Raw;

public sealed record ArgumentProviderInput(
    string OutputFilePath,
    string TemporaryDirectoryPath,
    IReadOnlyList<string> VariableValues
);