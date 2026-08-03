using System.Text;
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
            "--ansi",
            "--header", ToAnsiString(title),

            "--accept-nth", "1",
            "--with-nth", "{2}        {3}",
            "--delimiter", delimiter
        ]);
        command = command.WithStandardInputPipe(PipeSource.FromString(
            string.Join(
                Environment.NewLine,
                suggestionDictionary.Select(kv => $"{kv.Key}{delimiter}{kv.Value.Value}{delimiter}{kv.Value.Description}")
            )
        ));
        var output = await command.ExecuteBufferedAsync(cancellationToken);

        if (!int.TryParse(output.StandardOutput, out var index))
            index = -1;
        _ = suggestionDictionary.TryGetValue(index, out var selection);
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
            "--ansi",
            "--header", ToAnsiString(title),

            "--accept-nth", "1",
            "--with-nth", "{1}        {2}",
            "--delimiter", delimiter
        ]);
        var lines = await suggestions
            .Select(s => $"{s.Value}{delimiter}{s.Description}")
            .ToArrayAsync();
        command = command.WithStandardInputPipe(PipeSource.FromString(string.Join(Environment.NewLine, lines)));
        var output = await command.ExecuteBufferedAsync(cancellationToken);
        var outputLines = output.StandardOutput.Split(Environment.NewLine);
        var query = outputLines[0];
        if (query.Length == 0 && outputLines.Length > 1)
            return outputLines[1];
        return query;
    }

    private static string ToAnsiString(IHighlightedString highlightedString)
    {
        var (offset, length) = highlightedString.Highlight.GetOffsetAndLength(highlightedString.String.Length);
        var builder = new StringBuilder();
        builder.Append(highlightedString.String, 0, offset);
        builder.Append("\x1b[4m");
        builder.Append(highlightedString.String, offset, length);
        builder.Append("\x1b[0m");
        builder.Append(highlightedString.String, offset + length, highlightedString.String.Length - offset - length);
        return builder.ToString();
    }
}
