using System.Diagnostics;
using System.Text.Json;
using Snavi.Model;

namespace Snavi.Interaction;

public sealed record ProviderResult(string Display, string Value, string? Preview = null);

public static class ProviderRunner
{
    public static IReadOnlyList<ProviderResult> Run(
        Provider provider, string cheatDir, IReadOnlyDictionary<string, string> resolved, Action<string> warn)
    {
        var path = Path.IsPathRooted(provider.Path)
            ? provider.Path
            : Path.Combine(cheatDir, provider.Path);
        if (!File.Exists(path))
        {
            warn($"provider 文件不存在: {path}");
            return [];
        }

        var inputFile = Path.Combine(Path.GetTempPath(), $"snavi-{Guid.NewGuid():N}.in.json");
        var outputFile = Path.Combine(Path.GetTempPath(), $"snavi-{Guid.NewGuid():N}.out.json");
        try
        {
            File.WriteAllText(inputFile, JsonSerializer.Serialize(resolved));
            File.WriteAllText(outputFile, string.Empty);

            var psi = BuildProcess(provider.Type, path, inputFile, outputFile);
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            using var process = Process.Start(psi);
            if (process is null)
            {
                warn($"无法启动 provider: {path}");
                return [];
            }
            var stderr = process.StandardError.ReadToEnd();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                warn($"provider 执行失败 (exit {process.ExitCode}): {stderr.Trim()}");
                return [];
            }

            return Parse(File.ReadAllText(outputFile));
        }
        catch (Exception ex)
        {
            warn($"provider 出错: {ex.Message}");
            return [];
        }
        finally
        {
            TryDelete(inputFile);
            TryDelete(outputFile);
        }
    }

    private static ProcessStartInfo BuildProcess(string type, string path, string inputFile, string outputFile)
    {
        var psi = new ProcessStartInfo();
        switch (type)
        {
            case "csharp":
                psi.FileName = "dotnet";
                psi.ArgumentList.Add("run");
                psi.ArgumentList.Add(path);
                psi.ArgumentList.Add("-p:SuppressTrimAnalysisWarnings=true");
                psi.ArgumentList.Add("-p:SuppressAotAnalysisWarnings=true");
                psi.ArgumentList.Add("--");
                psi.ArgumentList.Add(inputFile);
                psi.ArgumentList.Add(outputFile);
                break;
            case "sh":
                psi.FileName = "sh";
                psi.ArgumentList.Add(path);
                psi.ArgumentList.Add(inputFile);
                psi.ArgumentList.Add(outputFile);
                break;
            default:
                psi.FileName = path;
                psi.ArgumentList.Add(inputFile);
                psi.ArgumentList.Add(outputFile);
                break;
        }
        return psi;
    }

    private static IReadOnlyList<ProviderResult> Parse(string content)
    {
        using var doc = JsonDocument.Parse(content);
        var list = new List<ProviderResult>();
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.String:
                    var s = element.GetString() ?? string.Empty;
                    list.Add(new ProviderResult(s, s));
                    break;
                case JsonValueKind.Object:
                    var value = element.TryGetProperty("value", out var v) ? v.GetString() : null;
                    var display = element.TryGetProperty("display", out var d) ? d.GetString() : null;
                    var preview = element.TryGetProperty("preview", out var p) ? p.GetString() : null;
                    list.Add(new ProviderResult(display ?? value ?? string.Empty, value ?? string.Empty, preview));
                    break;
            }
        }
        return list;
    }

    private static void TryDelete(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch
        {
        }
    }
}
