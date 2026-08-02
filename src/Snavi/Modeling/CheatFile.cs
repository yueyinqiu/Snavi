namespace Snavi.CheatModeling;

public sealed record CheatFile(string Description, IReadOnlyList<CommandToken> Command, bool ExtraArguments = false);
