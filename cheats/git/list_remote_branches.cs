using System.Diagnostics;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var vars = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(args[0])) ?? new();
if (!vars.TryGetValue("远程", out var remote))
    return;

var psi = new ProcessStartInfo("git")
{
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
};
psi.ArgumentList.Add("ls-remote");
psi.ArgumentList.Add("--heads");
psi.ArgumentList.Add(remote);
using var git = Process.Start(psi)!;
var stdout = git.StandardOutput.ReadToEnd();
git.WaitForExit();
var branches = stdout
    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(l => l.Split('\t').Last().Replace("refs/heads/", ""))
    .ToList();
File.WriteAllText(args[1], JsonSerializer.Serialize(branches));
