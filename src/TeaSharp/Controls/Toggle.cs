using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a binary on/off control.
/// </summary>
public sealed class Toggle : Control
{
    private readonly WidgetStatePalette _statePalette = WidgetStatePalette.CreateDefault();
    private bool _hovered;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Toggle";

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

    public TeaStyle ValueStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle OnValueStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle OffValueStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle DisabledValueStyle
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

    public string OnText
    {
        get;
        set => field = value ?? string.Empty;
    } = "ON";

    public string OffText
    {
        get;
        set => field = value ?? string.Empty;
    } = "OFF";

    public bool Value
    {
        get;
        private set;
    }

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

    public override bool IsFocused
    {
        get;
        set;
    }

    public override bool IsDisabled
    {
        get;
        set;
    }

    public override bool IsReadOnly
    {
        get;
        set;
    }

    public void SetValue(bool value) => Value = value;

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
            changed |= SetHovered(true);
            Value = !Value;
            return true;
        }

        return Handle(message);
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

    private TeaStyle ResolveValueStyle()
    {
        var style = Value
            ? (OnValueStyle.IsEmpty ? ValueStyle : OnValueStyle)
            : (OffValueStyle.IsEmpty ? ValueStyle : OffValueStyle);

        if (IsDisabled && !DisabledValueStyle.IsEmpty)
        {
            style = style.IsEmpty
                ? DisabledValueStyle
                : style.Merge(DisabledValueStyle);
        }

        return style;
    }

    private TeaStyle ResolveBorderStyleText()
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
