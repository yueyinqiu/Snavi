using System.Diagnostics;
using System.Text;

namespace Snavi.Interaction;

public sealed class FzfUi : IUi
{
    private const string Dim = "\u001b[38;5;245m";
    private const string Highlight = "\u001b[38;5;214m";
    private const string Reset = "\u001b[0m";

    public string? Pick(string title, IReadOnlyList<PickerItem> items)
    {
        if (items.Count == 0)
            return null;

        var lines = items.Select(FormatLine).ToList();
        var result = RunFzf(lines, ["--print-query", "--ansi", "--header", title, "--prompt", "> "]);
        if (result is null)
            return null;

        var display = result.Selection.Split('\t')[0];
        var item = items.FirstOrDefault(i => i.Display == display);
        return item?.ResolvedValue;
    }

    public string? Complete(string title, string prompt, IReadOnlyList<PickerItem> suggestions)
    {
        var lines = suggestions.Select(FormatLine).ToList();
        var result = RunFzf(lines, ["--print-query", "--ansi", "--header", title, "--prompt", prompt, "--bind", "tab:replace-query"]);
        if (result is null)
            return null;

        var value = ResolveValue(result, suggestions);
        return string.IsNullOrEmpty(value) ? null : value;
    }

    public void Warn(string message)
    {
        Console.Error.WriteLine(message);
    }

    public void Close()
    {
    }

    private static string ResolveValue(FzfResult result, IReadOnlyList<PickerItem> suggestions)
    {
        if (!string.IsNullOrEmpty(result.Selection))
        {
            var display = result.Selection.Split('\t')[0];
            if (display == result.Query)
            {
                var item = suggestions.FirstOrDefault(s => s.Display == display);
                if (item is not null)
                    return item.ResolvedValue;
            }
        }

        return result.Query;
    }

    private static string FormatLine(PickerItem item)
    {
        return item.Secondary is null
            ? item.Display
            : $"{item.Display}\t{Dim}{item.Secondary}{Reset}";
    }

    private static FzfResult? RunFzf(IReadOnlyList<string> lines, IReadOnlyList<string> args)
    {
        var psi = new ProcessStartInfo("fzf")
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        foreach (var arg in args)
            psi.ArgumentList.Add(arg);

        Process process;
        try
        {
            process = Process.Start(psi)!;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"无法启动 fzf: {ex.Message}");
            return null;
        }

        using (process)
        {
            foreach (var line in lines)
                process.StandardInput.WriteLine(line);
            process.StandardInput.Close();

            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode is not (0 or 1))
                return null;

            var parts = output.Split('\n');
            return new FzfResult(parts.Length > 0 ? parts[0] : "", parts.Length > 1 ? parts[1] : "");
        }
    }

    private sealed record FzfResult(string Query, string Selection);
}
