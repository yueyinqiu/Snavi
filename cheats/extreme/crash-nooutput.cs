using System.Text;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

// deliberately never write the completions file, exit 0

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
