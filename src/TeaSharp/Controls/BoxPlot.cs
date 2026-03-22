using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a selectable multi-series box plot control.
/// </summary>
public sealed class BoxPlot : Control
{
    private readonly List<BoxPlotSeries> _series = [];
    private int _selectedSeries = -1;
    private int _hoveredSeries = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    /// <summary>
    /// Occurs when selected series changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<BoxPlotSeries>>? SelectionChanged;

    /// <summary>
    /// Gets or sets plot title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Box Plot";

    /// <summary>
    /// Gets or sets marker appended to title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker" /> is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets text rendered when no series are configured.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no series)";

    /// <summary>
    /// Gets or sets marker shown for selected rows.
    /// </summary>
    public string SelectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = ">";

    /// <summary>
    /// Gets or sets marker shown for non-selected rows.
    /// </summary>
    public string UnselectedMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = " ";

    /// <summary>
    /// Gets or sets whisker glyph.
    /// </summary>
    public char WhiskerGlyph { get; set; } = '─';

    /// <summary>
    /// Gets or sets whisker cap glyph.
    /// </summary>
    public char WhiskerCapGlyph { get; set; } = '┼';

    /// <summary>
    /// Gets or sets quartile box glyph.
    /// </summary>
    public char QuartileGlyph { get; set; } = '═';

    /// <summary>
    /// Gets or sets median glyph.
    /// </summary>
    public char MedianGlyph { get; set; } = '│';

    /// <summary>
    /// Gets or sets frame border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner frame padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets title style while not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style while focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border glyph style while not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border glyph style while focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets base style for rendered series rows.
    /// </summary>
    public TeaStyle SeriesStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rows.
    /// </summary>
    public TeaStyle HoveredSeriesStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TeaStyle SelectedSeriesStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows while focused.
    /// </summary>
    public TeaStyle FocusedSelectedSeriesStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into quartile box glyphs.
    /// </summary>
    public TeaStyle QuartileStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into median glyphs.
    /// </summary>
    public TeaStyle MedianStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into whisker glyphs.
    /// </summary>
    public TeaStyle WhiskerStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into muted rows.
    /// </summary>
    public TeaStyle MutedSeriesStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged while control is disabled.
    /// </summary>
    public TeaStyle DisabledSeriesStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for empty-state text.
    /// </summary>
    public TeaStyle EmptyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets current plot series.
    /// </summary>
    public IReadOnlyList<BoxPlotSeries> Series => _series;

    /// <summary>
    /// Gets selected series index, or <c>-1</c> when empty.
    /// </summary>
    public int SelectedSeriesIndex => _selectedSeries;

    /// <summary>
    /// Gets selected series, if any.
    /// </summary>
    public BoxPlotSeries? SelectedSeries => _selectedSeries >= 0 && _selectedSeries < _series.Count
        ? _series[_selectedSeries]
        : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces current box-plot series.
    /// </summary>
    /// <param name="series">Series to render.</param>
    public void SetSeries(IEnumerable<BoxPlotSeries> series)
    {
        ArgumentNullException.ThrowIfNull(series);
        var previousIndex = _selectedSeries;
        var previousSeries = SelectedSeries;

        _series.Clear();
        foreach (var entry in series)
        {
            if (entry is not null)
            {
                _series.Add(Clone(entry));
            }
        }

        NormalizeSelectionState();
        RaiseSelectionChangedIfNeeded(previousIndex, previousSeries);
    }

    /// <summary>
    /// Sets selected series index using bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true" /> when selection changed.</returns>
    public bool SetSelectedSeries(int index)
    {
        if (_series.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _series.Count - 1);
        if (clamped == _selectedSeries)
        {
            return false;
        }

        var previousIndex = _selectedSeries;
        var previousSeries = SelectedSeries;
        _selectedSeries = clamped;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChanged(previousIndex, previousSeries, _selectedSeries, SelectedSeries);
        return true;
    }

    /// <summary>
    /// Compatibility wrapper for selecting by index.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true" /> when selection changed.</returns>
    public bool Select(int index)
    {
        return SetSelectedSeries(index);
    }

    /// <summary>
    /// Clears all series.
    /// </summary>
    public void Clear()
    {
        var previousIndex = _selectedSeries;
        var previousSeries = SelectedSeries;
        _series.Clear();
        _selectedSeries = -1;
        _hoveredSeries = -1;
        _scrollOffset = 0;
        RaiseSelectionChangedIfNeeded(previousIndex, previousSeries);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _series.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        var page = Math.Max(1, _lastViewportRows);
        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedSeries(_selectedSeries + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedSeries(_selectedSeries - 1);
        if (key.Is(Key.Home)) return SetSelectedSeries(0);
        if (key.Is(Key.End)) return SetSelectedSeries(_series.Count - 1);
        if (key.Is(Key.PageDown)) return SetSelectedSeries(_selectedSeries + page);
        if (key.Is(Key.PageUp)) return SetSelectedSeries(_selectedSeries - page);
        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHoveredSeries(-1);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _series.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown) return SetSelectedSeries(_selectedSeries + 1) || changed;
            if (pointer.Button == PointerButton.WheelUp) return SetSelectedSeries(_selectedSeries - 1) || changed;
        }

        if (!inside)
        {
            return changed;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(_lastViewportRows);
        var hovered = _scrollOffset + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _series.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredSeries(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            changed |= SetHoveredSeries(hovered);
            changed |= SetSelectedSeries(hovered);
            return changed;
        }

        return changed;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        if (_series.Count == 0)
        {
            var emptyStyle = IsDisabled ? EmptyStyle.Merge(DisabledSeriesStyle) : EmptyStyle;
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, emptyStyle), content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(_lastViewportRows);

        var visibleRows = Math.Min(content.Height, _series.Count - _scrollOffset);
        var labelWidth = ResolveLabelWidth(content.Width);
        var plotX = content.X + labelWidth + (labelWidth > 0 ? 1 : 0);
        var plotWidth = Math.Max(0, content.Right - plotX);
        ResolveRange(out var min, out var max);

        for (var row = 0; row < visibleRows; row++)
        {
            var seriesIndex = _scrollOffset + row;
            var y = content.Y + row;
            var series = _series[seriesIndex];
            var rowStyle = ResolveRowStyle(seriesIndex);
            WriteLabel(canvas, content.X, y, labelWidth, BuildLabelPrefix(seriesIndex, series), rowStyle);
            if (plotWidth > 0)
            {
                DrawSeries(canvas, series, y, plotX, plotWidth, min, max, rowStyle);
            }
        }
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 6);
        for (var index = 0; index < _series.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(_series[index].Name) + 18);
        }

        var height = Math.Max(4, Math.Min(12, _series.Count + 2));
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void DrawSeries(Canvas canvas, BoxPlotSeries series, int y, int plotX, int plotWidth, double min, double max, TeaStyle rowStyle)
    {
        var normalized = NormalizeSeries(series);
        var whiskerLeft = MapToPlot(normalized.Minimum, min, max, plotX, plotWidth);
        var boxLeft = MapToPlot(normalized.FirstQuartile, min, max, plotX, plotWidth);
        var median = MapToPlot(normalized.Median, min, max, plotX, plotWidth);
        var boxRight = MapToPlot(normalized.ThirdQuartile, min, max, plotX, plotWidth);
        var whiskerRight = MapToPlot(normalized.Maximum, min, max, plotX, plotWidth);

        WriteSpan(canvas, y, whiskerLeft, whiskerRight, WhiskerGlyph, rowStyle.Merge(WhiskerStyle));
        WriteSpan(canvas, y, boxLeft, boxRight, QuartileGlyph, rowStyle.Merge(QuartileStyle));
        WriteGlyph(canvas, whiskerLeft, y, WhiskerCapGlyph, rowStyle.Merge(WhiskerStyle));
        WriteGlyph(canvas, whiskerRight, y, WhiskerCapGlyph, rowStyle.Merge(WhiskerStyle));
        WriteGlyph(canvas, median, y, MedianGlyph, rowStyle.Merge(MedianStyle));
    }

    private TeaStyle ResolveRowStyle(int index)
    {
        var style = SeriesStyle;
        if (_series[index].IsMuted)
        {
            style = style.Merge(MutedSeriesStyle);
        }

        if (index == _hoveredSeries)
        {
            style = style.Merge(HoveredSeriesStyle);
        }

        if (index == _selectedSeries)
        {
            style = style.Merge(SelectedSeriesStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedSeriesStyle);
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledSeriesStyle);
        }

        return style;
    }

    private string BuildLabelPrefix(int index, BoxPlotSeries series)
    {
        var marker = index == _selectedSeries ? SelectedMarker : UnselectedMarker;
        return string.Concat(marker, " ", series.Name);
    }

    private int ResolveLabelWidth(int contentWidth)
    {
        var max = 0;
        for (var index = 0; index < _series.Count; index++)
        {
            var width = ControlTextLayout.MeasureDisplayWidth(BuildLabelPrefix(index, _series[index]));
            if (width > max)
            {
                max = width;
            }
        }

        return Math.Clamp(max, 0, Math.Max(0, contentWidth - 8));
    }

    private void ResolveRange(out double min, out double max)
    {
        min = double.PositiveInfinity;
        max = double.NegativeInfinity;
        for (var index = 0; index < _series.Count; index++)
        {
            var normalized = NormalizeSeries(_series[index]);
            if (normalized.Minimum < min) min = normalized.Minimum;
            if (normalized.Maximum > max) max = normalized.Maximum;
        }

        if (min == double.PositiveInfinity)
        {
            min = 0;
            max = 1;
        }
        else if (Math.Abs(max - min) < double.Epsilon)
        {
            max = min + 1;
        }
    }

    private static (double Minimum, double FirstQuartile, double Median, double ThirdQuartile, double Maximum) NormalizeSeries(BoxPlotSeries series)
    {
        var values = new[] { series.Minimum, series.FirstQuartile, series.Median, series.ThirdQuartile, series.Maximum };
        Array.Sort(values);
        return (values[0], values[1], values[2], values[3], values[4]);
    }

    private static int MapToPlot(double value, double min, double max, int plotX, int plotWidth)
    {
        if (plotWidth <= 1)
        {
            return plotX;
        }

        var normalized = Math.Clamp((value - min) / (max - min), 0d, 1d);
        return plotX + Math.Clamp((int)Math.Round(normalized * (plotWidth - 1), MidpointRounding.AwayFromZero), 0, plotWidth - 1);
    }

    private static void WriteLabel(Canvas canvas, int x, int y, int width, string text, TeaStyle style)
    {
        if (width <= 0)
        {
            return;
        }

        canvas.WriteText(x, y, ApplyStyle(text, style), width);
    }

    private static void WriteSpan(Canvas canvas, int y, int start, int end, char glyph, TeaStyle style)
    {
        var left = Math.Min(start, end);
        var right = Math.Max(start, end);
        for (var x = left; x <= right; x++)
        {
            WriteGlyph(canvas, x, y, glyph, style);
        }
    }

    private static void WriteGlyph(Canvas canvas, int x, int y, char glyph, TeaStyle style)
    {
        if (style.IsEmpty)
        {
            canvas.Set(x, y, glyph);
            return;
        }

        canvas.WriteText(x, y, style.Render(glyph.ToString()), 1);
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledSeriesStyle) : style;
    }

    private string RenderTitle()
    {
        var text = MeasureTitle();
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledSeriesStyle);
        }

        return ApplyStyle(text, style);
    }

    private string MeasureTitle()
    {
        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? string.Concat(Title, " ", FocusMarker)
            : Title;
    }

    private bool SetHoveredSeries(int index)
    {
        if (_hoveredSeries == index)
        {
            return false;
        }

        _hoveredSeries = index;
        return true;
    }

    private void NormalizeSelectionState()
    {
        if (_series.Count == 0)
        {
            _selectedSeries = -1;
            _hoveredSeries = -1;
            _scrollOffset = 0;
            return;
        }

        _selectedSeries = Math.Clamp(_selectedSeries < 0 ? 0 : _selectedSeries, 0, _series.Count - 1);
        _hoveredSeries = Math.Clamp(_hoveredSeries, -1, _series.Count - 1);
        EnsureSelectionVisible(_lastViewportRows);
    }

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_series.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (_selectedSeries < 0)
        {
            _selectedSeries = 0;
        }

        if (_selectedSeries < _scrollOffset)
        {
            _scrollOffset = _selectedSeries;
        }
        else if (_selectedSeries >= _scrollOffset + viewportRows)
        {
            _scrollOffset = _selectedSeries - viewportRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _series.Count - viewportRows));
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, BoxPlotSeries? previousSeries)
    {
        if (previousIndex == _selectedSeries && IsSameSeries(previousSeries, SelectedSeries))
        {
            return;
        }

        RaiseSelectionChanged(previousIndex, previousSeries, _selectedSeries, SelectedSeries);
    }

    private void RaiseSelectionChanged(int previousIndex, BoxPlotSeries? previousSeries, int selectedIndex, BoxPlotSeries? selectedSeries)
    {
        SelectionChanged?.Invoke(
            this,
            new ListSelectionChangedEventArgs<BoxPlotSeries>(previousIndex, selectedIndex, previousSeries, selectedSeries));
    }

    private static bool IsSameSeries(BoxPlotSeries? left, BoxPlotSeries? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return string.Equals(left.Name, right.Name, StringComparison.Ordinal)
            && left.Minimum.Equals(right.Minimum)
            && left.FirstQuartile.Equals(right.FirstQuartile)
            && left.Median.Equals(right.Median)
            && left.ThirdQuartile.Equals(right.ThirdQuartile)
            && left.Maximum.Equals(right.Maximum);
    }

    private static BoxPlotSeries Clone(BoxPlotSeries series)
    {
        return new BoxPlotSeries(series.Name, series.Minimum, series.FirstQuartile, series.Median, series.ThirdQuartile, series.Maximum)
        {
            IsMuted = series.IsMuted,
        };
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
