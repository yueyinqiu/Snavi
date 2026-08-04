using System.Text;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

throw new Exception("boom: deliberate crash inside the completion script");

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
