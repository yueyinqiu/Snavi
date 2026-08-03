using CliWrap;
using CliWrap.Buffered;

namespace Snavi.Interacting;

sealed class UserInterfaceFzf : IUserInterface
{
    public async Task<T?> PickAsync<T>(
        IHighlightedString title,
        string prompt,
        IAsyncEnumerable<T> suggestions,
        CancellationToken cancellationToken
    ) where T : IPickable
    {
        var suggestionDictionary = await suggestions.Index().ToDictionaryAsync();

        var delimiter = Guid.NewGuid().ToString("N");

        var command = Cli.Wrap("fzf");
        command = command.WithValidation(CommandResultValidation.None);
        command = command.WithArguments([
            "--header", title.String,

            "--accept-nth", "1",
            "--with-nth", "{2} {3}",
            "--delimiter", delimiter
        ]);
        command = command.WithStandardInputPipe(PipeSource.FromString(
            string.Join(
                Environment.NewLine,
                suggestionDictionary.Select(kv => $"{kv.Key}{delimiter}{kv.Value.Value}{delimiter}{kv.Value.Description}")
            )
        ));
        var output = await command.ExecuteBufferedAsync(cancellationToken);
        
        _ = suggestionDictionary.TryGetValue(output, out var selection);
        return selection;
    }

    public async Task<string?> InputAsync(
        IHighlightedString title,
        string prompt,
        IAsyncEnumerable<IPickable> suggestions,
        CancellationToken cancellationToken
    )
    {
        var delimiter = Guid.NewGuid().ToString("N");

        var command = Cli.Wrap("fzf");
        command = command.WithValidation(CommandResultValidation.None);
        command = command.WithArguments([
            "--print-query",
            "--header", title.String,

            "--with-nth", "2",
            "--delimiter", delimiter
        ]);
        command = command.WithStandardInputPipe(PipeSource.FromString(
            string.Join(
                Environment.NewLine,
                suggestions.Select(s => $"{s.Value}{delimiter}{s.Description}")
            )
        ));
        var output = await command.ExecuteBufferedAsync(cancellationToken);
        return output.StandardOutput.Split(Environment.NewLine)[0];
    }
}
