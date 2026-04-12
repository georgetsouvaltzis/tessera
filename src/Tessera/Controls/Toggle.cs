using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Components.Styling;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a binary on/off control.
/// </summary>
public sealed class Toggle : Control
{
    private readonly WidgetStatePalette _statePalette = WidgetStatePalette.CreateDefault();
    private bool _hovered;

    /// <summary>
    /// Represents title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Toggle";

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
    /// Represents value style.
    /// </summary>
    public TesseraStyle ValueStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents on value style.
    /// </summary>
    public TesseraStyle OnValueStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents off value style.
    /// </summary>
    public TesseraStyle OffValueStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Represents disabled value style.
    /// </summary>
    public TesseraStyle DisabledValueStyle
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
    /// Represents on text.
    /// </summary>
    public string OnText
    {
        get;
        set => field = value ?? string.Empty;
    } = "ON";

    /// <summary>
    /// Represents off text.
    /// </summary>
    public string OffText
    {
        get;
        set => field = value ?? string.Empty;
    } = "OFF";

    /// <summary>
    /// Represents value.
    /// </summary>
    public bool Value
    {
        get;
        private set;
    }

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
    /// Executes set value.
    /// </summary>
    /// <param name="value">The value value.</param>
    public void SetValue(bool value) => Value = value;

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            Value = !Value;
            return true;
        }

        if (key.Is(Key.Right))
        {
            var changed = !Value;
            Value = true;
            return changed;
        }

        if (key.Is(Key.Left))
        {
            var changed = Value;
            Value = false;
            return changed;
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
            return false;
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
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
            return SetHovered(true);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelUp)
            {
                var was = Value;
                Value = true;
                return !was || changed;
            }

            if (pointer.Button == PointerButton.WheelDown)
            {
                var was = Value;
                Value = false;
                return was || changed;
            }
        }

        if (pointer is { Kind: PointerEventKind.Press, Button: PointerButton.Left })
        {
            _ = SetHovered(true);
            Value = !Value;
            return true;
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

        if (content.IsEmpty || content.Height < 1)
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

        if (Value)
        {
            states.Add(WidgetVisualState.Checked);
            states.Add(WidgetVisualState.Success);
        }
        else
        {
            states.Add(WidgetVisualState.Unchecked);
        }

        var label = Value ? OnText : OffText;
        var rendered = _statePalette.Render($"<{label}>", states);
        var valueStyle = ResolveValueStyle();
        if (!valueStyle.IsEmpty)
        {
            rendered = valueStyle.Render(rendered);
        }

        canvas.WriteText(content.X, content.Y, rendered, content.Width);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(8, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + Math.Max(OnText.Length, OffText.Length) + 6);
        var height = Border == BorderStyle.None ? 1 + Padding.Vertical : 3 + Padding.Vertical;
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private Rect ResolveContentRect(Rect bounds) => FrameLayout.ResolveContentRect(bounds, Border, Padding);

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

    private TesseraStyle ResolveValueStyle()
    {
        TesseraStyle style;
        if (Value)
        {
            style = OnValueStyle.IsEmpty ? ValueStyle : OnValueStyle;
        }
        else
        {
            style = OffValueStyle.IsEmpty ? ValueStyle : OffValueStyle;
        }

        if (IsDisabled && !DisabledValueStyle.IsEmpty)
        {
            style = style.IsEmpty
                ? DisabledValueStyle
                : style.Merge(DisabledValueStyle);
        }

        return style;
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
            style = style.Merge(DisabledValueStyle);
        }

        return style;
    }
}
