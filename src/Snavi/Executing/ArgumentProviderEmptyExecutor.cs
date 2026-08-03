using System.Runtime.CompilerServices;
using Snavi.CheatModeling;

namespace Snavi.Executing;

sealed class ArgumentProviderEmptyExecutor() : IArgumentProviderExecutor<ArgumentProviderEmpty>
{
    public async IAsyncEnumerable<ArgumentSuggestion> RunAsync(
        ArgumentProviderEmpty provider,
        IReadOnlyList<string> variables,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        yield break;
    }
}
