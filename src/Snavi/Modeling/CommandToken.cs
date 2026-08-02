using System.Text.Json.Serialization;

namespace Snavi.CheatModeling;

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(Type))]
[JsonDerivedType(typeof(CommandTokenLiteral), nameof(CommandTokenLiteral))]
[JsonDerivedType(typeof(CommandTokenVariable), nameof(CommandTokenVariable))]
public abstract record CommandToken(string Type);
