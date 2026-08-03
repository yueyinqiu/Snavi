using Snavi.CheatModeling;
using Snavi.Interacting;

namespace Snavi;

public sealed record Command(DirectoryInfo? Directory, CheatFile Cheat) : IPickable
{
    public string Value => RenderedCommandTemplate.FromCommand(Cheat.Command, null, Cheat.ExtraArguments).String;

    public string Description => Cheat.Description;
}