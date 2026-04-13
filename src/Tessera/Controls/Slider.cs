using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Components.Styling;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a bounded slider control.
/// </summary>
public sealed class Slider : Control
{
    private readonly WidgetStatePalette _statePalette = WidgetStatePalette.CreateDefault();
    private bool _dragging;
    private bool _hovered;

    /// <summary>
    ///     Represents title.
    /// </summary>
    public string Title { get; set; } = "Slider";

    /// <summary>
    ///     Represents focus marker.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Represents show focus marker.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Represents title style.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents value label style.
    /// </summary>
    public TesseraStyle ValueLabelStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents fill style.
    /// </summary>
    public TesseraStyle FillStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents track style.
    /// </summary>
    public TesseraStyle TrackStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents disabled style.
    /// </summary>
    public TesseraStyle DisabledStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents min.
    /// </summary>
    public double Min
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents max.
    /// </summary>
    public double Max
    {
        get;
        set;
    } = 100.0;

    /// <summary>
    ///     Represents step.
    /// </summary>
    public double Step
    {
        get;
        set;
    } = 1.0;

    /// <summary>
    ///     Represents value.
    /// </summary>
    public double Value
    {
        get;
        private set;
    }

    /// <summary>
    ///     Represents border.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    ///     Represents padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsDisabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    ///     Executes set value.
    /// </summary>
    /// <param name="value)">The value value.</param>
    public void SetValue(double value)
    {
        Value = Clamp(value);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('-'))
        {
            var previous = Value;
            Value = Clamp(Value - Step);
            return !AreClose(previous, Value);
        }

        if (key.Is(Key.Right) || key.IsCharacter('+'))
        {
            var previous = Value;
            Value = Clamp(Value + Step);
            return !AreClose(previous, Value);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var changed = false;
        if (pointer is { Kind: PointerEventKind.Release, Button: PointerButton.Left })
        {
            var wasDragging = _dragging;
            _dragging = false;
            changed |= SetHovered(content.Contains(pointer.X, pointer.Y));
            return changed || wasDragging;
        }

        if (pointer is { Kind: PointerEventKind.Motion, Button: PointerButton.Left } && _dragging)
        {
            changed |= SetHovered(content.Contains(pointer.X, pointer.Y));
            changed |= SetValueFromPointer(pointer.X, content);
            return changed;
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHovered(false);
            }

            return changed;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            changed |= SetHovered(true);
            return changed;
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            var before = Value;
            if (pointer.Button == PointerButton.WheelUp)
            {
                Value = Clamp(Value + Step);
            }
            else if (pointer.Button == PointerButton.WheelDown)
            {
                Value = Clamp(Value - Step);
            }

            return changed || !AreClose(before, Value);
        }

        if (pointer is { Kind: PointerEventKind.Press, Button: PointerButton.Left } &&
            IsPointerOnBarRow(content, pointer.Y))
        {
            _dragging = true;
            changed |= SetHovered(true);
            changed |= SetValueFromPointer(pointer.X, content);
            return changed;
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

        var states = new List<WidgetVisualState>(4);
        if (IsFocused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (IsDisabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (IsReadOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (_hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        var label = $"{Value:0.##} / {Max:0.##}";
        var rendered = _statePalette.Render(label, states);
        var valueStyle = ResolveValueLabelStyle();
        if (!valueStyle.IsEmpty)
        {
            rendered = valueStyle.Render(rendered);
        }

        canvas.WriteText(content.X, content.Y, rendered, content.Width);
        if (content.Height > 1)
        {
            DrawStyledProgressBar(canvas, new Rect(content.X, content.Y + 1, content.Width, 1), Normalize());
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 8);
        var height = Border == BorderStyle.None ? 2 + Padding.Vertical : 4 + Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private double Normalize()
    {
        var range = Max - Min;
        if (range <= 0.0)
        {
            return 0.0;
        }

        return Math.Clamp((Value - Min) / range, 0.0, 1.0);
    }

    private double Clamp(double value)
    {
        if (Max <= Min)
        {
            return Min;
        }

        return Math.Clamp(value, Min, Max);
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.000001;
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return FrameLayout.ResolveContentRect(bounds, Border, Padding);
    }

    private static bool IsPointerOnBarRow(Rect content, int y)
    {
        var barY = content.Height > 1
            ? content.Y + 1
            : content.Y;
        return y == barY;
    }

    private bool SetValueFromPointer(int x, Rect content)
    {
        if (Max <= Min)
        {
            return false;
        }

        var barX = content.X + 1;
        var barWidth = Math.Max(1, content.Width - 2);
        var clampedX = Math.Clamp(x, barX, barX + barWidth - 1);
        var normalized = barWidth == 1
            ? 1.0
            : (double)(clampedX - barX) / Math.Max(1, barWidth - 1);
        var before = Value;
        Value = Clamp(Min + (Max - Min) * normalized);
        return !AreClose(before, Value);
    }

    private bool SetHovered(bool hovered)
    {
        if (_hovered == hovered)
        {
            return false;
        }

        _hovered = hovered;
        return true;
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

    private TesseraStyle ResolveValueLabelStyle()
    {
        if (IsDisabled && !DisabledStyle.IsEmpty)
        {
            return ValueLabelStyle.IsEmpty
                ? DisabledStyle
                : ValueLabelStyle.Merge(DisabledStyle);
        }

        return ValueLabelStyle;
    }

    private void DrawStyledProgressBar(Canvas canvas, Rect rect, double value)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1 || clipped.Width < 3)
        {
            return;
        }

        var fraction = Math.Clamp(value, 0.0, 1.0);
        var innerWidth = clipped.Width - 2;
        var filled = (int)Math.Round(innerWidth * fraction, MidpointRounding.AwayFromZero);
        filled = Math.Clamp(filled, 0, innerWidth);

        var trackStyle = ResolveTrackStyle();
        var fillStyle = ResolveFillStyle();
        WriteStyledChar(canvas, clipped.X, clipped.Y, '[', trackStyle);
        WriteStyledChar(canvas, clipped.Right - 1, clipped.Y, ']', trackStyle);
        for (var index = 0; index < innerWidth; index++)
        {
            var style = index < filled ? fillStyle : trackStyle;
            WriteStyledChar(canvas, clipped.X + 1 + index, clipped.Y, index < filled ? '█' : '░', style);
        }
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled || IsReadOnly)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private TesseraStyle ResolveTrackStyle()
    {
        if (IsDisabled && !DisabledStyle.IsEmpty)
        {
            return TrackStyle.IsEmpty
                ? DisabledStyle
                : TrackStyle.Merge(DisabledStyle);
        }

        return TrackStyle;
    }

    private TesseraStyle ResolveFillStyle()
    {
        if (IsDisabled && !DisabledStyle.IsEmpty)
        {
            return FillStyle.IsEmpty
                ? DisabledStyle
                : FillStyle.Merge(DisabledStyle);
        }

        return FillStyle;
    }

    private static void WriteStyledChar(Canvas canvas, int x, int y, char value, TesseraStyle style)
    {
        var text = value.ToString();
        if (!style.IsEmpty)
        {
            text = style.Render(text);
        }

        canvas.WriteText(x, y, text, 1);
    }
}
