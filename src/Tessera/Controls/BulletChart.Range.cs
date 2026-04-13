namespace Tessera.Controls;

/// <summary>
///     Classifies a bullet-chart range segment.
/// </summary>
public enum BulletRangeKind
{
    /// <summary>
    ///     Neutral range segment.
    /// </summary>
    Normal = 0,

    /// <summary>
    ///     Warning range segment.
    /// </summary>
    Warning = 1,

    /// <summary>
    ///     Critical range segment.
    /// </summary>
    Critical = 2
}

/// <summary>
///     Represents one range segment in a <see cref="BulletChart" />.
/// </summary>
/// <param name="Start">Range start value.</param>
/// <param name="End">Range end value.</param>
/// <param name="Kind">Range semantic kind.</param>
/// <param name="Label">Optional range label.</param>
public readonly record struct BulletRange(
    double Start,
    double End,
    BulletRangeKind Kind = BulletRangeKind.Normal,
    string? Label = null);
