using System.Diagnostics;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

var psi = new ProcessStartInfo("docker")
{
    UseShellExecute = false,
    RedirectStandardOutput = true,
    RedirectStandardError = true,
};
psi.ArgumentList.Add("images");
psi.ArgumentList.Add("--format");
psi.ArgumentList.Add("{{.Repository}}:{{.Tag}}");
using var docker = Process.Start(psi)!;
var stdout = docker.StandardOutput.ReadToEnd();
docker.WaitForExit();
var images = stdout
    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
    .Select(l => l.Trim())
    .ToList();
File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = images.Select(i => new { Value = i, Description = i }) })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
