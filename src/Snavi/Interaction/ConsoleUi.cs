namespace Snavi.Interaction;

public sealed class ConsoleUi : IUi
{
    public int? Pick(string prompt, IReadOnlyList<string> options)
    {
        if (options.Count == 0)
        {
            Console.WriteLine($"{prompt}: (无可用选项)");
            return null;
        }
        Console.WriteLine(prompt);
        for (var i = 0; i < options.Count; i++)
            Console.WriteLine($"  [{i + 1}] {options[i]}");
        Console.Write("选择 (回车取消): ");
        var input = Console.ReadLine();
        if (int.TryParse(input, out var n) && n >= 1 && n <= options.Count)
            return n - 1;
        return null;
    }

    public string? Prompt(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }
}
