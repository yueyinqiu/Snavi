using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using Snavi.Executing;
using Snavi.Interacting;

namespace Snavi;

[Command("Run")]
public partial class RunCommand : ICommand
{
    [CommandOption("cheats", 'c')]
    public required IReadOnlyList<FileInfo> Cheats { get; set; }

    [CommandOption("dotnet")]
    public required string Dotnet { get; set; } = "dotnet";

    [CommandOption("fzf")]
    public required string Fzf { get; set; } = "fzf";

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var result = await new SnaviApp(Cheats, new UserInterfaceFzf(Fzf), new ArgumentProviderExector(Dotnet)).RunAsync(console.RegisterCancellationHandler());
        console.WriteLine(result);
    }
}