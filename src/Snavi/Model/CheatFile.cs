namespace Snavi.Model;

public sealed record CheatFile(string Description, List<Token> Command, bool ExtraArgs = false);
