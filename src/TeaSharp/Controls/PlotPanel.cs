using System.ComponentModel;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a bordered container that arranges multiple plot controls in a grid.
/// </summary>
public sealed class PlotPanel : Control
{
    private readonly List<Control> _plots = [];

    /// <summary>
    /// Gets or sets the panel title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Plots";

    /// <summary>
    /// Gets or sets the marker appended to the title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets style used for the title when the control is not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for the title when the control is focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style used for empty-state text.
    /// </summary>
    public TeaStyle EmptyTextStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rendered output while <see cref="Control.IsDisabled"/> is <see langword="true"/>.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the frame border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding applied around the plot grid.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets text shown when no plots are configured.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no plots)";

    /// <summary>
    /// Gets or sets advanced layout options.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public PlotPanelOptions? Options { get; set; }

    /// <summary>
    /// Gets the configured plot controls.
    /// </summary>
    public IReadOnlyList<Control> Plots => _plots;

    /// <summary>
    /// Replaces the plot collection.
    /// </summary>
    /// <param name="plots">Plot controls to render.</param>
    public void SetPlots(IEnumerable<Control> plots)
    {
        ArgumentNullException.ThrowIfNull(plots);

        _plots.Clear();
        foreach (var plot in plots)
        {
            if (plot is not null)
            {
                _plots.Add(plot);
            }
        }
    }

    /// <summary>
    /// Adds one plot control to the panel.
    /// </summary>
    /// <param name="plot">The plot control to add.</param>
    public void AddPlot(Control plot)
    {
        ArgumentNullException.ThrowIfNull(plot);
        _plots.Add(plot);
    }

    /// <summary>
    /// Removes all plot controls.
    /// </summary>
    public void ClearPlots()
    {
        _plots.Clear();
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled)
        {
            return false;
        }

        for (var i = 0; i < _plots.Count; i++)
        {
            if (_plots[i].Handle(message))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || !TryResolveCellRects(bounds, out var cells))
        {
            return Handle(message);
        }

        if (message is PointerInput pointer)
        {
            for (var i = 0; i < _plots.Count; i++)
            {
                if (!cells[i].Contains(pointer.X, pointer.Y))
                {
                    continue;
                }

                if (_plots[i].Handle(message, cells[i]))
                {
                    return true;
                }
            }

            return Handle(message);
        }

        for (var i = 0; i < _plots.Count; i++)
        {
            if (_plots[i].Handle(message, cells[i]))
            {
                return true;
            }
        }

        return Handle(message);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = Border == BorderStyle.None
            ? clipped.Inset(Padding)
            : FrameLayout.DrawFrameAndResolveContent(
                canvas,
                clipped,
                RenderTitle(),
                Border,
                Padding,
                ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        if (_plots.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveStyled(EmptyTextStyle)), content.Width);
            return;
        }

        if (!TryResolveCellRects(content, out var cells))
        {
            return;
        }

        for (var i = 0; i < _plots.Count; i++)
        {
            _plots[i].Render(canvas, cells[i]);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var options = NormalizeOptions(Options ?? new PlotPanelOptions());
        var columns = Math.Clamp(options.Columns, 1, Math.Max(1, _plots.Count));
        var rows = _plots.Count == 0
            ? 1
            : (_plots.Count + columns - 1) / columns;
        var spacing = Math.Max(0, options.Spacing);

        var cellWidth = 20;
        var cellHeight = 7;
        var unconstrained = new Rect(0, 0, 120, 40);
        for (var i = 0; i < _plots.Count; i++)
        {
            var measurement = _plots[i].Measure(unconstrained);
            cellWidth = Math.Max(cellWidth, measurement.Width);
            cellHeight = Math.Max(cellHeight, measurement.Height);
        }

        var width = (columns * cellWidth) + (Math.Max(0, columns - 1) * spacing);
        var height = (rows * cellHeight) + (Math.Max(0, rows - 1) * spacing);
        width += Padding.Horizontal;
        height += Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool TryResolveCellRects(Rect bounds, out Rect[] result)
    {
        if (bounds.IsEmpty || _plots.Count == 0)
        {
            result = [];
            return false;
        }

        var content = Border == BorderStyle.None
            ? bounds.Inset(Padding)
            : FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            result = [];
            return false;
        }

        var options = NormalizeOptions(Options ?? new PlotPanelOptions());
        var columns = Math.Clamp(options.Columns, 1, Math.Max(1, _plots.Count));
        var rows = (_plots.Count + columns - 1) / columns;
        var spacing = Math.Max(0, options.Spacing);

        var totalHorizontalSpacing = Math.Max(0, columns - 1) * spacing;
        var totalVerticalSpacing = Math.Max(0, rows - 1) * spacing;
        var innerWidth = content.Width - totalHorizontalSpacing;
        var innerHeight = content.Height - totalVerticalSpacing;
        if (innerWidth <= 0 || innerHeight <= 0)
        {
            result = [];
            return false;
        }

        var baseCellWidth = innerWidth / columns;
        var baseCellHeight = innerHeight / rows;
        if (baseCellWidth <= 0 || baseCellHeight <= 0)
        {
            result = [];
            return false;
        }

        result = new Rect[_plots.Count];
        var widthRemainder = innerWidth % columns;
        var heightRemainder = innerHeight % rows;
        var y = content.Y;
        var itemIndex = 0;

        for (var row = 0; row < rows && itemIndex < _plots.Count; row++)
        {
            var rowHeight = baseCellHeight + (row < heightRemainder ? 1 : 0);
            var x = content.X;
            for (var column = 0; column < columns && itemIndex < _plots.Count; column++)
            {
                var columnWidth = baseCellWidth + (column < widthRemainder ? 1 : 0);
                result[itemIndex] = new Rect(x, y, columnWidth, rowHeight);
                itemIndex++;
                x += columnWidth + spacing;
            }

            y += rowHeight + spacing;
        }

        return true;
    }

    private static PlotPanelOptions NormalizeOptions(PlotPanelOptions options)
    {
        return options with
        {
            Columns = Math.Max(1, options.Columns),
            Spacing = Math.Max(0, options.Spacing),
        };
    }

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return ResolveStyled(style);
    }

    private TeaStyle ResolveStyled(TeaStyle style)
    {
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private string RenderTitle()
    {
        var text = FormatTitleText();
        var style = IsFocused ? ResolveStyled(FocusedTitleStyle) : ResolveStyled(TitleStyle);
        return ApplyStyle(text, style);
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string FormatTitleForMeasure()
    {
        if (!ShowFocusMarker || string.IsNullOrWhiteSpace(FocusMarker))
        {
            return Title;
        }

        return string.IsNullOrEmpty(Title) ? string.Empty : $"{Title} {FocusMarker}";
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        if (style.IsEmpty || string.IsNullOrEmpty(text))
        {
            return text;
        }

        return style.Render(text);
    }
}
