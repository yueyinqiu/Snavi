namespace Snavi.Interaction;

public sealed record PickerItem(string Display, string? Preview);

public interface IUi
{
    int? Pick(string prompt, IReadOnlyList<PickerItem> items);
    string? Prompt(string message);
    void Warn(string message);
    void Close();
}
