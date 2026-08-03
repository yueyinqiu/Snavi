using System.Text.Json.Serialization;

namespace Snavi.CheatModeling;

[JsonDerivedType(typeof(CommandTokenLiteral), nameof(CommandTokenLiteral))]
[JsonDerivedType(typeof(CommandTokenVariable), nameof(CommandTokenVariable))]
public abstract record CommandToken;
