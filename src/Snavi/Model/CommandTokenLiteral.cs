using System.Text.Json.Serialization;

namespace Snavi.Model;

public sealed record CommandTokenLiteral(string Value) : CommandToken(nameof(CommandTokenLiteral));
