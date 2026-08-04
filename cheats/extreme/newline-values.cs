using System.Text;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

var completions = new[]
{
    new { Value = "line1\nline2\nline3", Description = "multi-line value with \\n" },
    new { Value = "tab\there\tand\tthere", Description = "value with tabs" },
    new { Value = "a\r\nb\r\nc", Description = "CRLF line breaks" },
    new { Value = "single'quote double\"quote back\\slash", Description = "desc with 'quotes' and \\slashes" },
    new { Value = " leading and trailing spaces ", Description = " value with surrounding spaces " },
    new { Value = "", Description = "empty string value" },
    new { Value = "   ", Description = "whitespace-only value" }
};

File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = completions })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
