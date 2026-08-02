using System.Diagnostics;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

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
File.WriteAllText(args[1], JsonSerializer.Serialize(images));
