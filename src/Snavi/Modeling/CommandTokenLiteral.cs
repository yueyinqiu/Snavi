using System.Text.Json.Serialization;

namespace Snavi.CheatModeling;

public sealed record CommandTokenLiteral(string Value) : CommandToken(nameof(CommandTokenLiteral));
