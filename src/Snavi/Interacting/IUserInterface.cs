namespace Snavi.UserInterfaces;

interface IUserInterface
{
    Task<T> PickCommandAsync<T>(IAsyncEnumerable<T> commands, CancellationToken cancellationToken) where T : IPickableCommand;
    Task<ArgumentSuggestion> CompleteArgumentAsync(IHighlightedString title, string prompt, IAsyncEnumerable<ArgumentSuggestion> suggestions, CancellationToken cancellationToken);
}
