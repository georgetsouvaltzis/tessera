using Tessera.Components.Primitives.Internal;
namespace Tessera.Components.Primitives;

public readonly record struct Rect(int X, int Y, int Width, int Height)
{
    public int Right => X + Width;

    public int Bottom => Y + Height;

    public bool IsEmpty => Width <= 0 || Height <= 0;

    public bool Contains(int x, int y)
    {
        return !IsEmpty && x >= X && x < Right && y >= Y && y < Bottom;
    }

    public Rect Inset(int horizontal, int vertical)
    {
        return new Rect(
            X + horizontal,
            Y + vertical,
            Width - (horizontal * 2),
            Height - (vertical * 2));
    }

    public Rect Inset(Thickness thickness)
    {
        return new Rect(
            X + thickness.Left,
            Y + thickness.Top,
            Width - thickness.Horizontal,
            Height - thickness.Vertical);
    }

    public static Rect Intersect(Rect a, Rect b)
    {
        var x = Math.Max(a.X, b.X);
        var y = Math.Max(a.Y, b.Y);
        var right = Math.Min(a.Right, b.Right);
        var bottom = Math.Min(a.Bottom, b.Bottom);
        var width = right - x;
        var height = bottom - y;
        if (width <= 0 || height <= 0)
        {
            return new Rect(0, 0, 0, 0);
        }

        return new Rect(x, y, width, height);
    }
}
