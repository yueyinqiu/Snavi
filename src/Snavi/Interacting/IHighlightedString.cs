namespace Snavi.Interacting;

interface IHighlightedString
{
    string String { get; }
    Range Highlight { get; }
}