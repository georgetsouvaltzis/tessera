using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;
using Tessera.Widgets;

namespace Tessera.Controls;

/// <summary>
/// Represents a multi-line text editor.
/// </summary>
public sealed class TextArea : Control
{
    private readonly ViewportModel _viewport = new();
    private readonly TextInputModel _input = new() { Multiline = true };
    private string _lastViewportValue = string.Empty;

    /// <summary>
    /// Represents title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Text Area";

    /// <summary>
    /// Represents focus marker.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Represents show focus marker.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Represents title style.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents value text style.
    /// </summary>
    public TesseraStyle ValueTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents disabled value text style.
    /// </summary>
    public TesseraStyle DisabledValueTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents value.
    /// </summary>
    public string Value => _input.Value;

    /// <summary>
    /// Represents border.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Represents padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Represents show line numbers.
    /// </summary>
    public bool ShowLineNumbers
    {
        get => _viewport.ShowLineNumbers;
        set => _viewport.ShowLineNumbers = value;
    }

    /// <summary>
    /// Represents wrap.
    /// </summary>
    public bool Wrap
    {
        get => _viewport.Wrap;
        set => _viewport.SetWrap(value);
    }

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <summary>
    /// Executes set value.
    /// </summary>
    /// <param name="value">The value value.</param>
    public void SetValue(string value)
    {
        _input.SetValue(value ?? string.Empty);
        SyncViewport();
    }

    /// <summary>
    /// Executes clear.
    /// </summary>
    public void Clear()
    {
        _input.Clear();
        SyncViewport();
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused)
        {
            return false;
        }

        var changed = false;
        var update = _input.Update(message);
        if (update.Changed)
        {
            SyncViewport();
            changed = true;
        }

        if (_viewport.Update(message))
        {
            changed = true;
        }

        _viewport.HighlightVisualLine = CursorLineIndex();
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

        var title = Border == BorderStyle.None ? null : FormatTitle();
        if (!string.IsNullOrEmpty(title))
        {
            var titleStyle = IsFocused ? FocusedTitleStyle : TitleStyle;
            if (!titleStyle.IsEmpty)
            {
                title = titleStyle.Render(title);
            }
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        _viewport.Resize(content.Width, content.Height);
        _viewport.HighlightVisualLine = CursorLineIndex();
        SyncViewport();

        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        var lineStyle = IsDisabled
            ? ResolveDisabledLineStyle()
            : ValueTextStyle;
        for (var row = 0; row < rows; row++)
        {
            var line = lines[row];
            if (!lineStyle.IsEmpty)
            {
                line = lineStyle.Render(line);
            }

            canvas.WriteText(content.X, content.Y + row, line, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var lines = ViewportLineFormatter.NormalizeContentLines(_input.Value);
        var width = 0;
        for (var index = 0; index < lines.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(lines[index]));
        }

        if (ShowLineNumbers)
        {
            width += 4;
        }

        width += Padding.Horizontal;
        var height = Math.Max(1, lines.Count) + Padding.Vertical;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void SyncViewport()
    {
        var value = _input.Value;
        if (string.Equals(value, _lastViewportValue, StringComparison.Ordinal))
        {
            return;
        }

        _viewport.SetLines(ViewportLineFormatter.NormalizeContentLines(value));
        _lastViewportValue = value;
    }

    private int CursorLineIndex()
    {
        if (_input.Cursor <= 0)
        {
            return 0;
        }

        var cursor = Math.Min(_input.Cursor, _input.Value.Length);
        var lines = 0;
        for (var index = 0; index < cursor; index++)
        {
            if (_input.Value[index] == '\n')
            {
                lines++;
            }
        }

        return lines;
    }

    private string FormatTitle()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string FormatTitleForMeasure()
    {
        if (ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private TesseraStyle ResolveDisabledLineStyle()
    {
        if (DisabledValueTextStyle.IsEmpty)
        {
            return ValueTextStyle;
        }

        return ValueTextStyle.IsEmpty
            ? DisabledValueTextStyle
            : ValueTextStyle.Merge(DisabledValueTextStyle);
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledValueTextStyle);
        }

        return style;
    }
}
