using Snavi.Interaction;

var cheatsDir = "cheats";
for (var i = 0; i < args.Length - 1; i++)
{
    if (args[i] is "--cheats" or "-c")
        cheatsDir = args[i + 1];
}

if (args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("用法: snavi [--cheats <目录>]");
    return;
}

try
{
    var library = new CheatLibrary(cheatsDir);
    var ui = new FzfUi();
    var app = new SnaviApp(library, ui);
    string? result;
    try
    {
        result = app.Run();
    }
    finally
    {
        ui.Close();
    }
    if (result is not null)
        Console.WriteLine(result);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"错误: {ex.Message}");
    Environment.ExitCode = 1;
}
