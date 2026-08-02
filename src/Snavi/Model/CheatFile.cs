namespace Snavi.Model;

public sealed record CheatFile(string Description, IReadOnlyList<CommandToken> Command, bool ExtraArguments = false);
