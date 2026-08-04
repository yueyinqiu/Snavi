using System.Text.Json;
using Snavi.ArgumentSuggester.Raw;

namespace Snavi.ArgumentSuggester;

public abstract class SnaviArgumentSuggester
{
    public abstract IAsyncEnumerable<(string Value, string Description)> SuggestAsync(
        IReadOnlyList<string> variableValues,
        DirectoryInfo currentDirectory,
        DirectoryInfo temporaryDirectory,
        CancellationToken cancellationToken
    );

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        using var inputStream = Console.OpenStandardInput();
        var input = await JsonSerializer.DeserializeAsync(
            inputStream,
            ArgumentProviderInputSerializerContext.Default.ArgumentProviderInput,
            cancellationToken
        )!;
        var output = await this.SuggestAsync(
            input!.VariableValues,
            new DirectoryInfo(input.TemporaryDirectoryPath),
            new DirectoryInfo(input.TemporaryDirectoryPath),
            cancellationToken
        ).Select(x => new ArgumentProviderOutput.Completion(x.Value, x.Description)).ToArrayAsync();

        using var outputStream = new FileStream(input.OutputFilePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(
            outputStream,
            new ArgumentProviderOutput(output),
            ArgumentProviderOutputSerializerContext.Default.ArgumentProviderOutput,
            cancellationToken
        );
    }
}