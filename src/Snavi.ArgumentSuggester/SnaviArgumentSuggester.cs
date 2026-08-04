using System.Text.Json;
using Snavi.ArgumentSuggester.Raw;

namespace Snavi.ArgumentSuggester;

public abstract class SnaviArgumentSuggester
{
    public abstract IAsyncEnumerable<(string Value, string Description)> SuggestAsync(
        IReadOnlyList<string> givenArguments,
        DirectoryInfo currentDirectory,
        DirectoryInfo temporaryDirectory,
        CancellationToken cancellationToken
    );

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        using var inputStream = Console.OpenStandardInput();
        var input = await JsonSerializer.DeserializeAsync(
            inputStream,
            ArgumentSuggesterInputSerializerContext.Default.ArgumentSuggesterInput,
            cancellationToken
        )!;
        var output = await this.SuggestAsync(
            input!.GivenArguments,
            new DirectoryInfo(input.TemporaryDirectoryPath),
            new DirectoryInfo(input.TemporaryDirectoryPath),
            cancellationToken
        ).Select(x => new ArgumentSuggesterOutput.Suggestion(x.Value, x.Description)).ToArrayAsync();

        using var outputStream = new FileStream(input.OutputFilePath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(
            outputStream,
            new ArgumentProviderOutput(output),
            ArgumentSuggesterOutputSerializerContext.Default.ArgumentProviderOutput,
            cancellationToken
        );
    }
}