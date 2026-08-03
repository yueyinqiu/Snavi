using System.Buffers;
using System.Text;
using Snavi.CheatModeling;
using Snavi.Interacting;

namespace Snavi;

public sealed record RenderedCommandTemplate(string String, Range Highlight) : IHighlightedString
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

    public static RenderedCommandTemplate FromCommand(
        IReadOnlyList<CommandToken> command,
        IReadOnlyList<string>? variables
    )
    {
        using var variableEnumerator = (variables ?? Enumerable.Empty<string>()).GetEnumerator();
        Range? highlight = variables is null ? 0..0 : null;

        var builder = new StringBuilder();
        foreach (var token in command)
        {
            builder.Append(' ');
            switch (token)
            {
                case CommandTokenVariable variable:
                    if (variableEnumerator.MoveNext())
                        builder.Append(Escape(variableEnumerator.Current));
                    else
                    {
                        var name = $"<{variable.Name}>";
                        builder.Append(name);
                        highlight ??= (builder.Length - 1 - name.Length)..(builder.Length - 1);
                    }
                    break;
                case CommandTokenLiteral literal:
                    builder.Append(Escape(literal.Value));
                    break;
            }
        }

        return new(builder.Remove(0, 1).ToString(), highlight ?? 0..0);
    }
}
