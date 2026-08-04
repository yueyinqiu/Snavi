using Snavi.Core;
using Snavi.Modeling;

namespace Snavi.Executing;

sealed class ArgumentProviderExector(string dotnet) : IArgumentProviderExecutor<ArgumentProvider>
{
    public IAsyncEnumerable<ArgumentSuggestion> RunAsync(
        ArgumentProvider provider,
        DirectoryInfo? directory,
        IReadOnlyList<string> variables,
        CancellationToken cancellationToken
    )
    {
        return provider switch
        {
            ArgumentProviderCsharp csharp => new ArgumentProviderCsharpExecutor(dotnet).RunAsync(
                csharp, directory, variables, cancellationToken
            ),
            ArgumentProviderEmpty empty => new ArgumentProviderEmptyExecutor().RunAsync(
                empty, directory, variables, cancellationToken
            ),
            _ => throw new Exception($"Unknown argument provider type '{provider.GetType().Name}'.")
        };
    }
}
