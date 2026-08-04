using System.Text.Json.Serialization;

namespace Snavi.Modeling;

[JsonDerivedType(typeof(ArgumentProviderCsharp), nameof(ArgumentProviderCsharp))]
[JsonDerivedType(typeof(ArgumentProviderEmpty), nameof(ArgumentProviderEmpty))]
public abstract record ArgumentProvider;
