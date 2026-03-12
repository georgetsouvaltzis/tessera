using TeaSharp.Components.Primitives;

namespace TeaSharp;

public sealed record ScreenContext
{
    public int Width { get; init; }

    public int Height { get; init; }

    public bool HasFocus { get; init; } = true;

    public Rect Bounds => new(0, 0, Width, Height);

    public Canvas CreateCanvas(CanvasTextMode textMode = CanvasTextMode.Fast)
    {
        return new Canvas(Math.Max(1, Width), Math.Max(1, Height), textMode);
    }
}
