namespace Tessera.Layout;

/// <summary>
///     Describes how a slot should size itself along its primary axis.
/// </summary>
public readonly record struct LayoutLength(LayoutLengthKind Kind, int Value = 0)
{
    /// <summary>
    ///     Converts an integer into a fixed-size layout length.
    /// </summary>
    /// <param name="value">The exact size to reserve.</param>
    public static implicit operator LayoutLength(int value)
    {
        return Fixed(value);
    }

    /// <summary>
    ///     Creates an auto-sized length.
    /// </summary>
    public static LayoutLength Auto()
    {
        return new LayoutLength(LayoutLengthKind.Auto);
    }

    /// <summary>
    ///     Creates a fixed-size length.
    /// </summary>
    /// <param name="value">The exact size to reserve.</param>
    public static LayoutLength Fixed(int value)
    {
        return new LayoutLength(LayoutLengthKind.Fixed, Math.Max(0, value));
    }

    /// <summary>
    ///     Creates a fill length that shares remaining space evenly.
    /// </summary>
    public static LayoutLength Fill()
    {
        return new LayoutLength(LayoutLengthKind.Fill, 1);
    }

    /// <summary>
    ///     Creates a weighted length that claims a proportional share of the remaining space.
    /// </summary>
    /// <param name="weight">The slot weight.</param>
    public static LayoutLength Weighted(int weight)
    {
        return new LayoutLength(LayoutLengthKind.Weighted, Math.Max(1, weight));
    }
}
