using System.Runtime.CompilerServices;
using System.Text.Json;
using Snavi.CheatModeling;
using Snavi.UserInterfaces;

namespace Snavi.Interaction;

sealed class SnaviApp(IReadOnlyList<FileInfo> cheats, IUserInterface ui)
{
    private async IAsyncEnumerable<Command> EnumerateValidCheatsAsync(
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
        var cheat = await ui.PickCommandAsync(
            this.EnumerateValidCheatsAsync(cancellationToken),
            cancellationToken
        );

        var variables = new List<string>();
        foreach (var token in cheat.Cheat.Command)
        {
            if (token is CommandTokenVariable variable)
            {
                var header = RenderedCommandTemplate.FromCommand(cheat.Cheat.Command, variables);
                var value = await InputVariableAsync(variable, cheat.Directory, variables, header, cancellationToken);
                variables.Add(value);
            }
        }

        var result = RenderedCommandTemplate.FromCommand(cheat.Cheat.Command, variables);
        if (cheat.Cheat.ExtraArguments)
        {
            var extraArguments = await ui.CompleteArgumentAsync(
                result,
                "Extra Arguments: ",
                Array.Empty<ArgumentSuggestion>().ToAsyncEnumerable(),
                cancellationToken
            );
            return $"{result.String} {extraArguments}";
        }
        return result.String;
    }

    private async Task<string> InputVariableAsync(
        CommandTokenVariable variable,
        DirectoryInfo? directory,
        IReadOnlyList<string> variables,
        RenderedCommandTemplate header,
        CancellationToken cancellationToken
    )
    {
        var suggestions = ArgumentProviderExecutor.RunAsync(variable.Provider, directory, variables, cancellationToken);
        var result = await ui.CompleteArgumentAsync(header, $"{variable.Name}: ", suggestions, cancellationToken);
        return result.Value;
    }
}
