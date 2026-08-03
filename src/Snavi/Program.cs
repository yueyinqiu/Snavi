using CliFx;
using CliFx.Binding;
using CliFx.Infrastructure;
using Snavi.Interacting;

namespace Snavi;

[Command("Run")]
public partial class RunCommand : ICommand
{
    [CommandOption("cheats", 'c')]
    public required IReadOnlyList<FileInfo> Cheats { get; set; }

    public async ValueTask ExecuteAsync(IConsole console)
    {
        var result = await new SnaviApp(Cheats, new UserInterfaceFzf()).RunAsync(console.RegisterCancellationHandler());
        console.WriteLine(result);
    }
}