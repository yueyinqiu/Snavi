using System.Text.Json.Serialization;

namespace Snavi.CheatModeling;

[JsonDerivedType(typeof(ArgumentProviderCsharp), nameof(ArgumentProviderCsharp))]
[JsonDerivedType(typeof(ArgumentProviderEmpty), nameof(ArgumentProviderEmpty))]
public abstract record ArgumentProvider;
