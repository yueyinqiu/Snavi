namespace Snavi.Executing;

public sealed record ArgumentProviderInput(
    string OutputFilePath,
    string TemporaryDirectoryPath,
    IReadOnlyList<string> VariableValues
);