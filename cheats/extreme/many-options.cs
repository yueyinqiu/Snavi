using System.Text;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

var completions = new List<object>();
for (var i = 0; i < 500; i++)
{
    completions.Add(new
    {
        Value = $"image-{i:000}-{new string((char)('a' + i % 26), 40)}",
        Description = $"Description #{i:000}: a rather long description that keeps going and going and going {i}"
    });
}

File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = completions })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
