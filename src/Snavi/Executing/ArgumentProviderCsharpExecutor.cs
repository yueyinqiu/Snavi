using System.Runtime.CompilerServices;
using System.Text.Json;
using CliWrap;
using Snavi.Core;
using Snavi.Modeling;

namespace Snavi.Executing;

sealed class ArgumentProviderCsharpExecutor(string dotnet) : IArgumentProviderExecutor<ArgumentProviderCsharp>
{
    public async IAsyncEnumerable<ArgumentSuggestion> RunAsync(
        ArgumentProviderCsharp provider,
        DirectoryInfo? directory,
        IReadOnlyList<string> variables,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        if (directory is null)
        {
            yield return new ArgumentSuggestion("No suggestion provided as the cheat file is not located in any directory.", "");
            yield break;
        }

        var logs = Directory.CreateTempSubdirectory();

        var outputs = logs.CreateSubdirectory("outputs");
        var completions = Path.Combine(outputs.FullName, "completions.json");

        var command = Cli.Wrap(dotnet);
        command = command.WithArguments([
            "run", Path.GetFullPath(provider.ScriptPath, directory.FullName)
        ]);
        command = command.WithStandardOutputPipe(PipeTarget.ToFile(Path.Combine(outputs.FullName, "stdout.txt")));
        command = command.WithStandardErrorPipe(PipeTarget.ToFile(Path.Combine(outputs.FullName, "stderr.txt")));
        command = command.WithStandardInputPipe(PipeSource.FromString(
            JsonSerializer.Serialize(new ArgumentProviderInput(
                completions,
                logs.CreateSubdirectory("temp").FullName,
                variables
            ), ArgumentProviderInputSerializerContext.Default.ArgumentProviderInput)
        ));
        command = command.WithWorkingDirectory(directory.FullName);
        command = command.WithValidation(CommandResultValidation.None);

        ArgumentProviderOutput? outputCompletions;
        try
        {
            await command.ExecuteAsync(cancellationToken);
            using var stream = File.OpenRead(completions);
            outputCompletions = await JsonSerializer.DeserializeAsync(
                stream,
                ArgumentProviderOutputSerializerContext.Default.ArgumentProviderOutput,
                cancellationToken
            ) ?? new ArgumentProviderOutput([]);
        }
        catch (Exception exception)
        {
            outputCompletions = null;
            await File.WriteAllTextAsync(
                Path.Combine(outputs.FullName, "snavi-exception.txt"),
                exception.ToString(),
                cancellationToken
            );
        }

        if (outputCompletions is null)
        {
            yield return new ArgumentSuggestion(
                $"No suggestion provided as an error occurred. For more information, see {logs.FullName}",
                ""
            );
            yield break;
        }

        try
        {
            logs.Delete(true);
        }
        catch
        {

        }

        foreach (var completion in outputCompletions.Completions)
        {
            yield return new ArgumentSuggestion(completion.Value, completion.Description);
        }
    }
}
