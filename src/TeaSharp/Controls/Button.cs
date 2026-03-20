using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Represents an activatable push button.
/// </summary>
/// <remarks>
/// Use <see cref="Activated"/> as the normal interaction hook. The advanced polling surface remains available
/// only for transitional interop.
/// </remarks>
public sealed class Button : Control
{
    private bool _hovered;
    private bool _pressed;
    private int _activationCount;
    private bool _pendingActivation;

    /// <summary>
    /// Occurs when the button is activated by input.
    /// </summary>
    public event EventHandler? Activated;

    /// <summary>
    /// Gets or sets the button label.
    /// </summary>
    public string Text
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the optional secondary description shown with the button.
    /// </summary>
    public string? Description
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the button border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the inner padding applied to the button body.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the base style applied to the button label.
    /// </summary>
    public TeaStyle LabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the label style when the button is focused.
    /// </summary>
    public TeaStyle FocusedLabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the label style when the button is disabled.
    /// </summary>
    public TeaStyle DisabledLabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the label style when the button is pressed.
    /// </summary>
    public TeaStyle PressedLabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty.WithInverse().WithBold();

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

    /// <summary>
    /// Gets how many activations have been observed.
    /// </summary>
    public int ActivationCount => _activationCount;

    /// <summary>
    /// Gets a value indicating whether the button is currently pressed.
    /// </summary>
    public bool IsPressed => _pressed;

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

    /// <summary>
    /// Attempts to consume a pending activation.
    /// </summary>
    /// <returns><see langword="true"/> when an activation was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeActivation()
    {
        if (!_pendingActivation)
        {
            return false;
        }

        _pendingActivation = false;
        return true;
    }

    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (!key.Is(Key.Enter) && !key.IsCharacter(' '))
        {
            return false;
        }

        Activate(pressed: false);
        return true;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press or PointerEventKind.Release)
            {
                changed |= SetHovered(false);
                changed |= SetPressed(false);
            }

            return changed || Handle(message);
        }

        switch (pointer.Kind)
        {
            case PointerEventKind.Motion:
                return SetHovered(true);
            case PointerEventKind.Press when pointer.Button == PointerButton.Left:
                SetHovered(true);
                Activate(pressed: true);
                return true;
            case PointerEventKind.Release when pointer.Button == PointerButton.Left:
                return SetPressed(false);
            default:
                return changed || Handle(message);
        }
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            null,
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var label = $"[{Text}]";
        if (IsDisabled)
        {
            label += " (disabled)";
        }

        var renderedLabel = ApplyLabelStyle(label);
        var rowCount = string.IsNullOrWhiteSpace(Description) || content.Height < 2 ? 1 : 2;
        var top = content.Y + Math.Max(0, (content.Height - rowCount) / 2);
        WriteCenteredLabel(canvas, content, top, label, renderedLabel);
        if (rowCount > 1)
        {
            ControlTextLayout.WriteCentered(canvas, content, top + 1, Description!);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth($"[{Text}]") + Padding.Horizontal;
        var height = Padding.Vertical + (string.IsNullOrWhiteSpace(Description) ? 1 : 2);
        if (IsDisabled)
        {
            width += ControlTextLayout.MeasureDisplayWidth(" (disabled)");
        }

        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void Activate(bool pressed)
    {
        _activationCount++;
        _pendingActivation = true;
        Activated?.Invoke(this, EventArgs.Empty);
        SetPressed(pressed);
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

    private bool SetPressed(bool pressed)
    {
        if (_pressed == pressed)
        {
            return false;
        }

        _pressed = pressed;
        return true;
    }

    private string ApplyLabelStyle(string label)
    {
        var style = LabelStyle;
        if (IsFocused)
        {
            style = style.Merge(FocusedLabelStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledLabelStyle);
        }

        if (_pressed)
        {
            style = style.Merge(PressedLabelStyle);
        }

        if (style.IsEmpty)
        {
            return label;
        }

        return style.Render(label);
    }

    private static void WriteCenteredLabel(Canvas canvas, Rect content, int y, string plainLabel, string renderedLabel)
    {
        if (y < content.Y || y > content.Bottom)
        {
            return;
        }

        var displayWidth = ControlTextLayout.MeasureDisplayWidth(plainLabel);
        var x = content.X;
        var width = content.Width;
        if (displayWidth < content.Width)
        {
            var offset = (content.Width - displayWidth) / 2;
            x += offset;
            width -= offset;
        }

        canvas.WriteText(x, y, renderedLabel, width);
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
            style = style.Merge(DisabledLabelStyle);
        }

        return style;
    }
}
