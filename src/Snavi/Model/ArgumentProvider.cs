using System.Text.Json.Serialization;

namespace Snavi.Model;

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(Type))]
[JsonDerivedType(typeof(ArgumentProviderCsharp), nameof(ArgumentProviderCsharp))]
[JsonDerivedType(typeof(ArgumentProviderEmpty), nameof(ArgumentProviderEmpty))]
public abstract record ArgumentProvider(string Type);
