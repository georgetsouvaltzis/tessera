namespace Tessera.Components.Primitives;

/// <summary>
///     Represents rect y.
/// </summary>
/// <param name="X">The x value.</param>
/// <param name="Y">The y value.</param>
/// <param name="Width">The width value.</param>
/// <param name="Height">The height value.</param>
public readonly record struct Rect(int X, int Y, int Width, int Height)
{
    /// <summary>
    ///     Represents right.
    /// </summary>
    public int Right => X + Width;

    /// <summary>
    ///     Represents bottom.
    /// </summary>
    public int Bottom => Y + Height;

    /// <summary>
    ///     Represents is empty.
    /// </summary>
    public bool IsEmpty => Width <= 0 || Height <= 0;

    /// <summary>
    ///     Executes contains.
    /// </summary>
    /// <param name="x">The x value.</param>
    /// <param name="y">The y value.</param>
    /// <returns><see langword="true" /> when contains succeeds.</returns>
    public bool Contains(int x, int y)
    {
        return !IsEmpty && x >= X && x < Right && y >= Y && y < Bottom;
    }

    /// <summary>
    ///     Executes inset.
    /// </summary>
    /// <param name="horizontal">The horizontal value.</param>
    /// <param name="vertical">The vertical value.</param>
    /// <returns>The result of inset.</returns>
    public Rect Inset(int horizontal, int vertical)
    {
        return new Rect(
            X + horizontal,
            Y + vertical,
            Width - horizontal * 2,
            Height - vertical * 2);
    }

    /// <summary>
    ///     Executes inset.
    /// </summary>
    /// <param name="thickness">The thickness value.</param>
    /// <returns>The result of inset.</returns>
    public Rect Inset(Thickness thickness)
    {
        return new Rect(
            X + thickness.Left,
            Y + thickness.Top,
            Width - thickness.Horizontal,
            Height - thickness.Vertical);
    }

    /// <summary>
    ///     Executes intersect.
    /// </summary>
    /// <param name="a">The a value.</param>
    /// <param name="b">The b value.</param>
    /// <returns>The result of intersect.</returns>
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
