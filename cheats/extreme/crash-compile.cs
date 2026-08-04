using System.Text;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

// this line deliberately fails to compile:
var this will not compile = ;

File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = Array.Empty<object>() })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
