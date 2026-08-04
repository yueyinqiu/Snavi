using System.Text;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

var completions = new List<object>();
for (var i = 0; i < 2000; i++)
{
    completions.Add(new
    {
        Value = "the-exact-same-value",
        Description = "the exact same description"
    });
}

File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = completions })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
