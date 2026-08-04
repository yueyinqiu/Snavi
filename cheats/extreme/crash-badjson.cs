using System.Text;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

File.WriteAllText(input.OutputFilePath, "this is definitely not valid json {{{");

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
