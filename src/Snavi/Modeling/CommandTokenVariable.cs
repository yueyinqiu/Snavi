using System.Text.Json.Serialization;

namespace Snavi.CheatModeling;

public sealed record CommandTokenVariable(string Name, ArgumentProvider Provider) : CommandToken(nameof(CommandTokenVariable));
