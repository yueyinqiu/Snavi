using Snavi.Executing;
using Snavi.Modeling;

namespace Snavi.Core;

interface IArgumentProviderExecutor<T> where T : ArgumentProvider
{
    IAsyncEnumerable<ArgumentSuggestion> RunAsync(
        T provider,
        DirectoryInfo? directory,
        IReadOnlyList<string> variables,
        CancellationToken cancellationToken
    );
}
