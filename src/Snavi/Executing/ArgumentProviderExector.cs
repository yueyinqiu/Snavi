using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Snavi.CheatModeling;

namespace Snavi.Executing;

sealed class ArgumentProviderExector(DirectoryInfo? directory) : IArgumentProviderExecutor<ArgumentProvider>
{
    public IAsyncEnumerable<ArgumentSuggestion> RunAsync(
        ArgumentProvider provider,
        IReadOnlyList<string> variables,
        CancellationToken cancellationToken
    )
    {
        return provider switch
        {
            ArgumentProviderCsharp csharp => new ArgumentProviderCsharpExecutor(directory).RunAsync(
                csharp, variables, cancellationToken
            ),
            ArgumentProviderEmpty empty => new ArgumentProviderEmptyExecutor().RunAsync(
                empty, variables, cancellationToken
            ),
            _ => throw new Exception($"Unknown argument provider type '{provider.GetType().Name}'.")
        };
    }
}
