namespace TeaSharp.Controls;

/// <summary>
/// Represents one five-number summary series rendered by <see cref="BoxPlot" />.
/// </summary>
public sealed class BoxPlotSeries
{
    /// <summary>
    /// Initializes a box-plot series from five-number summary values.
    /// </summary>
    /// <param name="name">Series display name.</param>
    /// <param name="minimum">Minimum value (left whisker).</param>
    /// <param name="firstQuartile">First quartile value.</param>
    /// <param name="median">Median value.</param>
    /// <param name="thirdQuartile">Third quartile value.</param>
    /// <param name="maximum">Maximum value (right whisker).</param>
    public BoxPlotSeries(
        string name,
        double minimum,
        double firstQuartile,
        double median,
        double thirdQuartile,
        double maximum)
    {
        Name = name ?? string.Empty;
        Minimum = minimum;
        FirstQuartile = firstQuartile;
        Median = median;
        ThirdQuartile = thirdQuartile;
        Maximum = maximum;
    }

    /// <summary>
    /// Gets or sets series display name.
    /// </summary>
    public string Name
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets minimum value (left whisker).
    /// </summary>
    public double Minimum { get; set; }

    /// <summary>
    /// Gets or sets first quartile value.
    /// </summary>
    public double FirstQuartile { get; set; }

    /// <summary>
    /// Gets or sets median value.
    /// </summary>
    public double Median { get; set; }

    /// <summary>
    /// Gets or sets third quartile value.
    /// </summary>
    public double ThirdQuartile { get; set; }

    /// <summary>
    /// Gets or sets maximum value (right whisker).
    /// </summary>
    public double Maximum { get; set; }

    /// <summary>
    /// Gets or sets whether row rendering should be muted.
    /// </summary>
    public bool IsMuted { get; set; }
}
