namespace TeaSharp.Components.Primitives;

/// <summary>
/// Represents four-sided spacing using familiar UI vocabulary.
/// </summary>
public readonly record struct Thickness(int Left, int Top, int Right, int Bottom)
{
    /// <summary>
    /// Returns a zero-spacing instance.
    /// </summary>
    public static Thickness None => default;

    /// <summary>
    /// Creates uniform spacing on all four sides.
    /// </summary>
    public static Thickness All(int value) => new(value, value, value, value);

    /// <summary>
    /// Creates spacing with shared horizontal and vertical values.
    /// </summary>
    public static Thickness Symmetric(int horizontal = 0, int vertical = 0) => new(horizontal, vertical, horizontal, vertical);

    /// <summary>
    /// Gets the combined left and right spacing.
    /// </summary>
    public int Horizontal => Left + Right;

    /// <summary>
    /// Gets the combined top and bottom spacing.
    /// </summary>
    public int Vertical => Top + Bottom;
}
