using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents one named series in a <see cref="LinePlot"/>.
/// </summary>
public sealed class LineSeries
{
    private readonly List<double> _samples = [];

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
    }

    /// <summary>
    /// Appends one sample value.
    /// </summary>
    /// <param name="sample">Sample value.</param>
    public void Append(double sample)
    {
        _samples.Add(sample);
    }

    /// <summary>
    /// Clears all sample values.
    /// </summary>
    public void Clear()
    {
        _samples.Clear();
    }
}
