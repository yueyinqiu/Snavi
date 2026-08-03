namespace Snavi.Interacting;

interface IUserInterface
{
    Task<T?> PickAsync<T>(
        IHighlightedString title,
        IAsyncEnumerable<T> suggestions,
        CancellationToken cancellationToken
    ) where T : IPickable;

    Task<string?> InputAsync(
        IHighlightedString title,
        IAsyncEnumerable<IPickable> suggestions,
        CancellationToken cancellationToken
    );
}
