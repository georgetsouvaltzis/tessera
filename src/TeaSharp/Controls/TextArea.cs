using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a multi-line text editor.
/// </summary>
public sealed class TextArea : Control
{
    private readonly ViewportModel _viewport = new();
    private readonly TextInputModel _input = new() { Multiline = true };

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Text Area";

    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    public TeaStyle TitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle FocusedTitleStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle ValueTextStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle DisabledValueTextStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TeaStyle BorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TeaStyle.Empty;

    public string Value => _input.Value;

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    public Thickness Padding
    {
        get;
        set;
    }

    public bool ShowLineNumbers
    {
        get => _viewport.ShowLineNumbers;
        set => _viewport.ShowLineNumbers = value;
    }

    public bool Wrap
    {
        get => _viewport.Wrap;
        set => _viewport.SetWrap(value);
    }

    public override bool IsFocused
    {
        get;
        set;
    }

    public void SetValue(string value)
    {
        _input.SetValue(value ?? string.Empty);
        SyncViewport();
    }

    public void Clear()
    {
        _input.Clear();
        SyncViewport();
    }

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
        var lines = _input.Value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n');
        var width = 0;
        for (var index = 0; index < lines.Length; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(lines[index]));
        }

        if (ShowLineNumbers)
        {
            width += 4;
        }

        width += Padding.Horizontal;
        var height = Math.Max(1, lines.Length) + Padding.Vertical;
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
        _viewport.SetLines(_input.Value.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n'));
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

    private TeaStyle ResolveDisabledLineStyle()
    {
        if (DisabledValueTextStyle.IsEmpty)
        {
            return ValueTextStyle;
        }

        return ValueTextStyle.IsEmpty
            ? DisabledValueTextStyle
            : ValueTextStyle.Merge(DisabledValueTextStyle);
    }

    private TeaStyle ResolveBorderStyleText()
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
