using System.Diagnostics;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

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
File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = branches.Select(b => new { Value = b, Description = b }) })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
