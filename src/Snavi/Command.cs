using Snavi.CheatModeling;
using Snavi.UserInterfaces;

namespace Snavi;

public sealed record Command(DirectoryInfo? Directory, CheatFile Cheat) : IPickableCommand
{
    public string Template => RenderedCommandTemplate.FromCommand(Cheat.Command, null).String;

    public string Description => Cheat.Description;
}