using Snavi.CheatModeling;

namespace Snavi.Executing;

interface IArgumentProviderExecutor<T> where T : ArgumentProvider
{
    IAsyncEnumerable<ArgumentSuggestion> RunAsync(
        T provider,
        DirectoryInfo? directory,
        IReadOnlyList<string> variables,
        CancellationToken cancellationToken
    );
}
