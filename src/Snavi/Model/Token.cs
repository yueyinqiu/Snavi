using System.Text.Json.Serialization;

namespace Snavi.Model;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Literal), "literal")]
[JsonDerivedType(typeof(Variable), "variable")]
public abstract record Token;

public sealed record Literal(string Value) : Token;

public sealed record Variable(string Name, Provider? Provider) : Token;
