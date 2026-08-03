using System.Diagnostics;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;
var remote = input.VariableValues.FirstOrDefault();
if (remote is null)
{
    File.WriteAllText(input.OutputFilePath, "{\"Completions\":[]}");
    return;
}

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
File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = branches.Select(b => new { Value = b, Description = b }) })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
