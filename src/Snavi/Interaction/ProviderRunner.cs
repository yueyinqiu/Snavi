using System.Diagnostics;
using Snavi.Model;

namespace Snavi.Interaction;

public sealed record ProviderResult(string Display, string Value);

public static class ProviderRunner
{
    public static IReadOnlyList<ProviderResult> Run(Provider provider, IReadOnlyDictionary<string, string> resolved)
    {
        var psi = new ProcessStartInfo { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
        var args = provider.Command
            .Select(token => token switch
            {
                Literal l => l.Value,
                Ref r => resolved.TryGetValue(r.Name, out var v) ? v : string.Empty,
                _ => throw new InvalidOperationException("provider 中出现了非法 token"),
            })
            .ToList();
        if (args.Count == 0)
            throw new InvalidOperationException("provider 的 command 为空");
        psi.FileName = args[0];
        for (var i = 1; i < args.Count; i++)
            psi.ArgumentList.Add(args[i]);

        using var process = Process.Start(psi)
            ?? throw new InvalidOperationException($"无法启动 provider: {psi.FileName}");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"provider 执行失败 (exit {process.ExitCode}): {stderr.Trim()}");

        return stdout
            .Split('\n')
            .Select(l => l.TrimEnd('\r'))
            .Where(l => l.Length > 0)
            .Select(ParseLine)
            .ToList();
    }

    private static ProviderResult ParseLine(string line)
    {
        var idx = line.LastIndexOf('\t');
        if (idx < 0)
            return new ProviderResult(line, line);
        return new ProviderResult(line[..idx], line[(idx + 1)..]);
    }
}
