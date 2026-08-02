using System.Text.Json.Serialization;

namespace Snavi.Model;

public sealed record CommandTokenVariable(string Name, ArgumentProvider? Provider) : CommandToken(nameof(CommandTokenVariable));
