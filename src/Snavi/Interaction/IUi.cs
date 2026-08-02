namespace Snavi.Interaction;

public sealed record PickerItem(string Display, string? Secondary = null, string? Value = null)
{
    public string ResolvedValue => Value ?? Display;
}

public interface IUi
{
    string? Pick(string title, IReadOnlyList<PickerItem> items);
    string? Complete(string title, string prompt, IReadOnlyList<PickerItem> suggestions);
    void Warn(string message);
    void Close();
}
