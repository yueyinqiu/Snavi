using System.Text;
using System.Text.Json;

AppContext.SetSwitch("System.Text.Json.JsonSerializer.IsReflectionEnabledByDefault", true);

var input = JsonSerializer.Deserialize<Input>(Console.In.ReadToEnd())!;

var completions = new[]
{
    new { Value = "it's got a single quote", Description = "desc 'with' quote" },
    new { Value = "dollar $ (parens) `backtick` *glob? [brackets] {braces} ;semi &amp &pipe|", Description = "shell metacharacters" },
    new { Value = "中文 日本語 한국어 עברית", Description = "CJK and RTL" },
    new { Value = "emoji 😀🚀🔥🎉", Description = "emoji description 🎈" },
    new { Value = "has  multiple   spaces   inside", Description = "spacing" },
    new { Value = "backslash \\ forward / colon : equals = plus + at @ percent %", Description = "safe chars mixed with unsafe" },
    new { Value = "double \"quoted\" value", Description = "double quotes" },
    new { Value = "snowman ☃ unicode snow flake ❄", Description = "more unicode" }
};

File.WriteAllText(
    input.OutputFilePath,
    JsonSerializer.Serialize(new { Completions = completions })
);

record Input(string OutputFilePath, string TemporaryDirectoryPath, IReadOnlyList<string> VariableValues);
