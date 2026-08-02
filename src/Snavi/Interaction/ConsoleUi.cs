namespace Snavi.Interaction;

public sealed class ConsoleUi : IUi
{
    public int? Pick(string prompt, IReadOnlyList<PickerItem> items)
    {
        if (items.Count == 0)
        {
            Console.WriteLine($"{prompt}: (无可用选项)");
            return null;
        }
        Console.WriteLine(prompt);
        for (var i = 0; i < items.Count; i++)
            Console.WriteLine($"  [{i + 1}] {items[i].Display}");
        Console.Write("选择 (回车取消): ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out var n) && n >= 1 && n <= items.Count)
            return n - 1;
        return null;
    }

    public string? Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    public void Warn(string message) => Console.Error.WriteLine($"警告: {message}");

    public void Close()
    {
    }
}
