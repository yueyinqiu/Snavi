namespace Snavi.CheatModeling;

public sealed record ArgumentProviderCsharp(
    string DotnetPath, string ScriptPath) : ArgumentProvider(nameof(ArgumentProviderCsharp));
