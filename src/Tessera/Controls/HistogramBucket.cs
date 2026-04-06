namespace Tessera.Controls;

/// <summary>
/// Represents one bucket entry in a <see cref="Histogram"/>.
/// </summary>
public readonly record struct HistogramBucket
{
    /// <summary>
    /// Initializes a new histogram bucket.
    /// </summary>
    /// <param name="label">The bucket label.</param>
    /// <param name="value">The bucket value.</param>
    public HistogramBucket(string? label, double value)
    {
        Label = label ?? string.Empty;
        Value = value;
    }

    /// <summary>
    /// Gets the bucket label.
    /// </summary>
    public string Label { get; init; }

    /// <summary>
    /// Gets the bucket value.
    /// </summary>
    public double Value { get; init; }
}
