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
psi.ArgumentList.Add("remote");
using var git = Process.Start(psi)!;
var stdout = git.StandardOutput.ReadToEnd();
git.WaitForExit();
var remotes = stdout
    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(l => l.Trim())
    .ToList();
File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = remotes.Select(r => new { Value = r, Description = r }) })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
