using System.Buffers;
using System.Text;
using Snavi.Modeling;
using Snavi.Core;

namespace Snavi.Helpers;

public sealed record RenderingCommand(string String, Range Highlight) : IHighlightedString
{
    private static readonly SearchValues<char> safeCharacters = SearchValues.Create(
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_@%+=:,./-"
    );
    
    private static string Escape(string token)
    {
        if (token == "")
            return "''";
        if (token.AsSpan().ContainsAnyExcept(safeCharacters))
            return $"'{token.Replace("'", "'\"'\"'")}'";
        return token;
    }

    public static RenderingCommand FromCommand(
        IEnumerable<CommandToken> command,
        IEnumerable<string>? variables,
        bool extraArguments
    )
    {
        if (extraArguments)
            command = command.Append(new CommandTokenVariable("args", new ArgumentProviderEmpty()));

        using var variableEnumerator = (variables ?? []).GetEnumerator();
        Range? highlight = variables is null ? 0..0 : null;
        
        var builder = new StringBuilder();
        foreach (var token in command)
        {
            switch (token)
            {
                case CommandTokenVariable variable:
                    if (variableEnumerator.MoveNext())
                        builder.Append(Escape(variableEnumerator.Current));
                    else
                    {
                        var name = $"<{variable.Name}>";
                        builder.Append(name);
                        highlight ??= (builder.Length - name.Length)..builder.Length;
                    }
                    break;
                case CommandTokenLiteral literal:
                    builder.Append(Escape(literal.Value));
                    break;
            }
            builder.Append(' ');
        }

        if (builder.Length == 0)
            return new("", highlight ?? 0..0);

        return new(builder.Remove(builder.Length - 1, 1).ToString(), highlight ?? 0..0);
    }
}
