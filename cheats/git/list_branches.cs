using System.Diagnostics;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var psi = new ProcessStartInfo("git")
{
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
};
psi.ArgumentList.Add("for-each-ref");
psi.ArgumentList.Add("--format=%(refname:short)");
psi.ArgumentList.Add("refs/heads");
using var git = Process.Start(psi)!;
var stdout = git.StandardOutput.ReadToEnd();
git.WaitForExit();
var branches = stdout
    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(l => l.Trim())
    .ToList();
File.WriteAllText(args[1], JsonSerializer.Serialize(branches));
