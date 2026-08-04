using System.Runtime.CompilerServices;
using System.Text.Json;
using Snavi.Executing;
using Snavi.Helpers;
using Snavi.Modeling;

namespace Snavi.Core;

sealed class SnaviApp(
    IReadOnlyList<FileInfo> cheats,
    IUserInterface ui,
    IArgumentProviderExecutor<ArgumentProvider> executor
)
{
    private async IAsyncEnumerable<PickingCommand> EnumerateValidCheatsAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        bool notFound = true;
        foreach (var file in cheats)
        {
            CheatFile? cheatFile;
            try
            {
                using var stream = file.OpenRead();
                cheatFile = await JsonSerializer.DeserializeAsync(
                    stream,
                    CheatFileSerializerContext.Default.CheatFile,
                    cancellationToken
                );
            }
            catch (Exception)
            {
                cheatFile = null;
            }

            if (cheatFile is not null)
            {
                notFound = false;
                yield return new (file.Directory, cheatFile);
            }
        }

        if (notFound)
        {
            yield return new (null,
                new CheatFile(
                    "Snavi: No valid cheat file found. There will be nothing to suggest.",
                    [], false
                )
            );
        }
    }

    public async Task<string> RunAsync(CancellationToken cancellationToken)
    {
        var cheat = await ui.PickAsync(
            new RenderingCommand("Choose a Command", 0..0),
            "Command: ",
            this.EnumerateValidCheatsAsync(cancellationToken),
            cancellationToken
        );

        if (cheat is null)
            return "Cancelled.";

        var variables = new List<string>();
        foreach (var token in cheat.Cheat.Command)
        {
            if (token is CommandTokenVariable variable)
            {
                var header = RenderingCommand.FromCommand(cheat.Cheat.Command, variables, cheat.Cheat.ExtraArguments);
                var suggestions = executor.RunAsync(variable.Provider, cheat.Directory, variables, cancellationToken);
                var value = await ui.InputAsync(header, $"{variable.Name} = ", suggestions, cancellationToken);
                if (value is null)
                    return "Cancelled.";
                variables.Add(value);
            }
        }

        if (cheat.Cheat.ExtraArguments)
        {
            var value = await ui.InputAsync(
                RenderingCommand.FromCommand(cheat.Cheat.Command, variables, true),
                "Extra Arguments: ",
                Array.Empty<ArgumentSuggestion>().ToAsyncEnumerable(),
                cancellationToken
            );
            if (value is null)
                return "Cancelled.";
            variables.Add(value);
        }
        return RenderingCommand.FromCommand(cheat.Cheat.Command, variables, cheat.Cheat.ExtraArguments).String;
    }
}
