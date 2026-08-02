namespace Snavi.Interaction;

public interface IUi
{
    int? Pick(string prompt, IReadOnlyList<string> options);
    string? Prompt(string message);
}
