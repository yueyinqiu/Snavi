using System.Runtime.CompilerServices;
using Snavi.Core;
using Snavi.Modeling;

namespace Snavi.Executing;

sealed class ArgumentProviderEmptyExecutor : IArgumentProviderExecutor<ArgumentProviderEmpty>
{
    public async IAsyncEnumerable<ArgumentSuggestion> RunAsync(
        ArgumentProviderEmpty provider,
        DirectoryInfo? directory,
        IReadOnlyList<string> variables,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        yield break;
    }
}
