using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents one named series in a <see cref="LinePlot"/>.
/// </summary>
public sealed class LineSeries
{
    private readonly List<double> _samples = [];
    private int? _capacity;

    /// <summary>
    /// Initializes an empty line series.
    /// </summary>
    /// <param name="name">Series display name.</param>
    public LineSeries(string name = "")
    {
        Name = name ?? string.Empty;
    }

    /// <summary>
    /// Initializes a line series with initial samples.
    /// </summary>
    /// <param name="name">Series display name.</param>
    /// <param name="samples">Initial sample values.</param>
    public LineSeries(string name, IEnumerable<double> samples)
        : this(name)
    {
        SetSamples(samples);
    }

    /// <summary>
    /// Gets or sets the series display name.
    /// </summary>
    public string Name
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the style used for this series line and legend text.
    /// </summary>
    public TeaStyle Style
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the glyph used for plotted points.
    /// </summary>
    public char PointGlyph
    {
        get;
        set;
    } = '●';

    /// <summary>
    /// Gets or sets the per-series scaling mode used by <see cref="LinePlot"/>.
    /// </summary>
    public LineSeriesScaleMode ScaleMode { get; set; } = LineSeriesScaleMode.Shared;

    /// <summary>
    /// Gets or sets an optional retained sample capacity.
    /// </summary>
    /// <remarks>
    /// When set, older samples are trimmed automatically after <see cref="SetSamples"/> and <see cref="Append"/>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is less than 1.</exception>
    public int? Capacity
    {
        get => _capacity;
        set
        {
            if (value is <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Capacity must be greater than zero.");
            }

            _capacity = value;
            TrimToCapacity();
        }
    }

    /// <summary>
    /// Gets the retained sample values.
    /// </summary>
    public IReadOnlyList<double> Samples => _samples;

    /// <summary>
    /// Replaces the current sample values.
    /// </summary>
    /// <param name="samples">Values in display order.</param>
    public void SetSamples(IEnumerable<double> samples)
    {
        ArgumentNullException.ThrowIfNull(samples);

        _samples.Clear();
        foreach (var sample in samples)
        {
            _samples.Add(sample);
        }

        TrimToCapacity();
    }

    /// <summary>
    /// Appends one sample value.
    /// </summary>
    /// <param name="sample">Sample value.</param>
    public void Append(double sample)
    {
        _samples.Add(sample);
        TrimToCapacity();
    }

    /// <summary>
    /// Trims retained samples to the last <paramref name="count"/> values.
    /// </summary>
    /// <param name="count">The number of trailing samples to keep.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="count"/> is negative.</exception>
    public void TrimToLast(int count)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count), "Trim count must be non-negative.");
        }

        if (count == 0)
        {
            _samples.Clear();
            return;
        }

        if (_samples.Count > count)
        {
            _samples.RemoveRange(0, _samples.Count - count);
        }
    }

    /// <summary>
    /// Clears all sample values.
    /// </summary>
    public void Clear()
    {
        _samples.Clear();
    }

    private void TrimToCapacity()
    {
        if (_capacity.HasValue && _samples.Count > _capacity.Value)
        {
            _samples.RemoveRange(0, _samples.Count - _capacity.Value);
        }
    }
}

/// <summary>
/// Defines how a <see cref="LineSeries"/> is scaled when rendered inside a <see cref="LinePlot"/>.
/// </summary>
public enum LineSeriesScaleMode
{
    /// <summary>
    /// Uses the shared visible Y-range across all shared-scale series.
    /// </summary>
    Shared = 0,

    /// <summary>
    /// Uses an independent normalized Y-range based on the series' own visible samples.
    /// </summary>
    Normalized = 1,
}
