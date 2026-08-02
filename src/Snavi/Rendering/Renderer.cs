namespace Snavi.Rendering;

public static class Renderer
{
    public static string Render(IEnumerable<string> tokens) => string.Join(' ', tokens);
}
