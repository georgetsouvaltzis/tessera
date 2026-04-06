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
    /// Gets or sets text rendered before <see cref="Text"/> inside the button label.
    /// Set to an empty string to remove the default leading bracket chrome.
    /// </summary>
    public string LabelPrefix
    {
        get;
        set => field = value ?? string.Empty;
    } = "[";

    /// <summary>
    /// Gets or sets text rendered after <see cref="Text"/> inside the button label.
    /// Set to an empty string to remove the default trailing bracket chrome.
    /// </summary>
    public string LabelSuffix
    {
        get;
        set => field = value ?? string.Empty;
    } = "]";

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
    /// Background-like facets are ignored so the button body remains a single surface.
    /// </summary>
    public TeaStyle LabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the label style when the button is focused.
    /// Background-like facets are ignored so focus remains shell-led instead of creating an inner chip.
    /// </summary>
    public TeaStyle FocusedLabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the label style when the button is disabled.
    /// Background-like facets are ignored so the button body remains a single surface.
    /// </summary>
    public TeaStyle DisabledLabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the label style when the button is pressed.
    /// Background-like facets are ignored so pressed state stays surface-led.
    /// </summary>
    public TeaStyle PressedLabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty.WithInverse().WithBold();

    /// <summary>
    /// Gets or sets the style applied to the button body across the padded content area.
    /// </summary>
    public TeaStyle SurfaceStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into <see cref="SurfaceStyle"/> while the button is focused.
    /// </summary>
    public TeaStyle FocusedSurfaceStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into <see cref="SurfaceStyle"/> while the button is pressed.
    /// </summary>
    public TeaStyle PressedSurfaceStyle
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

    /// <summary>
    /// Gets or sets how rounded surface-styled buttons compose their border shell and body fill.
    /// </summary>
    /// <remarks>
    /// <see cref="ButtonRoundedSurfaceMode.UnifiedShell"/> keeps rounded pills as a single filled surface.
    /// <see cref="ButtonRoundedSurfaceMode.InsetBody"/> renders a distinct rounded outline with a separately filled inner body.
    /// </remarks>
    public ButtonRoundedSurfaceMode RoundedSurfaceMode
    {
        get;
        set;
    } = ButtonRoundedSurfaceMode.UnifiedShell;

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

        var shellBorder = ResolveShellBorderStyle();
        var box = ResolveInteractiveRect(bounds, shellBorder);
        if (box.IsEmpty)
        {
            return false;
        }

        var inside = box.Contains(pointer.X, pointer.Y);
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

        var shellBorder = ResolveShellBorderStyle();
        var surfaceStyle = ResolveSurfaceStyle();
        var borderStyleText = ResolveBorderStyleText();
        var content = ShouldRenderFilledRoundedShell(shellBorder, surfaceStyle, RoundedSurfaceMode)
            ? DrawFilledRoundedShell(canvas, clipped, borderStyleText, surfaceStyle)
            : DrawDefaultShell(canvas, clipped, shellBorder, borderStyleText, surfaceStyle);
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var label = $"{LabelPrefix}{Text}{LabelSuffix}";
        var plainLabel = Text;
        if (IsDisabled)
        {
            label += " (disabled)";
            plainLabel += " (disabled)";
        }

        label = ResolveRenderedLabel(label, plainLabel, content.Width, shellBorder, surfaceStyle);
        var renderedLabel = ApplyLabelStyle(label, surfaceStyle);
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
        var labelWidth = ControlTextLayout.MeasureDisplayWidth($"{LabelPrefix}{Text}{LabelSuffix}");
        var height = Padding.Vertical + (string.IsNullOrWhiteSpace(Description) ? 1 : 2);
        if (IsDisabled)
        {
            labelWidth += ControlTextLayout.MeasureDisplayWidth(" (disabled)");
        }

        var descriptionWidth = string.IsNullOrWhiteSpace(Description)
            ? 0
            : ControlTextLayout.MeasureDisplayWidth(Description);
        var width = Math.Max(labelWidth, descriptionWidth) + Padding.Horizontal;

        var shellBorder = ResolveShellBorderStyle();

        if (shellBorder != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        if (shellBorder != BorderStyle.None && HasSurfaceChrome() && Padding.Horizontal == 0)
        {
            // Borderless chip-style buttons should still reserve symmetric interior breathing room
            // so centered labels do not depend on example-level width guessing.
            width = Math.Max(width, labelWidth + 4);
        }

        if (shellBorder == BorderStyle.Rounded && HasSurfaceChrome() && RoundedSurfaceMode == ButtonRoundedSurfaceMode.InsetBody)
        {
            // A distinct bordered shell plus inset body needs enough height to show
            // top/bottom fill rows around the centered label instead of collapsing to a 3-row chip.
            height = Math.Max(height, 5);
        }

        if (width > labelWidth && ((width - labelWidth) & 1) != 0)
        {
            width++;
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

    private string ApplyLabelStyle(string label, TeaStyle surfaceStyle)
    {
        var style = surfaceStyle.Merge(SanitizeLabelStyle(LabelStyle));
        if (IsFocused)
        {
            style = style.Merge(SanitizeLabelStyle(FocusedLabelStyle));
        }

        if (IsDisabled)
        {
            style = style.Merge(SanitizeLabelStyle(DisabledLabelStyle));
        }

        if (_pressed)
        {
            style = style.Merge(SanitizeLabelStyle(PressedLabelStyle));
        }

        if (style.IsEmpty)
        {
            return label;
        }

        return style.Render(label);
    }

    private Rect DrawDefaultShell(
        Canvas canvas,
        Rect clipped,
        BorderStyle shellBorder,
        TeaStyle borderStyleText,
        TeaStyle surfaceStyle)
    {
        var box = FrameLayout.ResolveInnerRect(clipped, shellBorder);
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            null,
            shellBorder,
            Padding,
            borderStyleText);
        if (!box.IsEmpty)
        {
            FillSurface(canvas, box, surfaceStyle);
        }

        return content;
    }

    private Rect DrawFilledRoundedShell(Canvas canvas, Rect clipped, TeaStyle borderStyleText, TeaStyle surfaceStyle)
    {
        if (clipped.Width < 2 || clipped.Height < 3)
        {
            FillSurface(canvas, clipped, surfaceStyle);
            return clipped.Inset(Padding);
        }

        var fillRect = new Rect(clipped.X + 1, clipped.Y + 1, clipped.Width - 2, clipped.Height - 2);
        if (!fillRect.IsEmpty)
        {
            FillSurface(canvas, fillRect, surfaceStyle);
        }

        var shellStyle = borderStyleText.Merge(surfaceStyle);
        WriteBorderGlyph(canvas, clipped.X, clipped.Y, '▛', shellStyle);
        WriteBorderGlyph(canvas, clipped.Right - 1, clipped.Y, '▜', shellStyle);
        WriteBorderGlyph(canvas, clipped.X, clipped.Bottom - 1, '▙', shellStyle);
        WriteBorderGlyph(canvas, clipped.Right - 1, clipped.Bottom - 1, '▟', shellStyle);

        for (var x = clipped.X + 1; x < clipped.Right - 1; x++)
        {
            WriteBorderGlyph(canvas, x, clipped.Y, '▀', shellStyle);
            WriteBorderGlyph(canvas, x, clipped.Bottom - 1, '▄', shellStyle);
        }

        for (var y = clipped.Y + 1; y < clipped.Bottom - 1; y++)
        {
            WriteBorderGlyph(canvas, clipped.X, y, '▌', shellStyle);
            WriteBorderGlyph(canvas, clipped.Right - 1, y, '▐', shellStyle);
        }

        return fillRect.Inset(Padding);
    }

    private void FillSurface(Canvas canvas, Rect box, TeaStyle surfaceStyle)
    {
        if (surfaceStyle.IsEmpty)
        {
            return;
        }

        if (canvas.TextMode == CanvasTextMode.GraphemeAware)
        {
            var fill = surfaceStyle.Render(" ");
            for (var y = box.Y; y < box.Bottom; y++)
            {
                for (var x = box.X; x < box.Right; x++)
                {
                    canvas.WriteText(x, y, fill, 1);
                }
            }
            return;
        }

        canvas.FillRect(box, ' ');
    }

    private Rect ResolveInteractiveRect(Rect bounds, BorderStyle shellBorder)
    {
        if (ShouldRenderFilledRoundedShell(shellBorder, ResolveSurfaceStyle(), RoundedSurfaceMode))
        {
            return bounds.Width < 2
                ? bounds
                : new Rect(bounds.X + 1, bounds.Y, Math.Max(0, bounds.Width - 2), bounds.Height);
        }

        return FrameLayout.ResolveInnerRect(bounds, shellBorder);
    }

    private static void WriteCenteredLabel(Canvas canvas, Rect content, int y, string plainLabel, string renderedLabel)
    {
        if (y < content.Y || y > content.Bottom)
        {
            return;
        }

        var displayWidth = ControlTextLayout.MeasureDisplayWidth(plainLabel);
        if (displayWidth <= 0)
        {
            return;
        }

        var x = content.X;
        if (displayWidth < content.Width)
        {
            var offset = (content.Width - displayWidth) / 2;
            x += offset;
        }

        canvas.WriteText(x, y, renderedLabel, displayWidth);
    }

    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        return style;
    }

    private TeaStyle ResolveSurfaceStyle()
    {
        var style = SurfaceStyle;
        if (IsFocused)
        {
            style = style.Merge(FocusedSurfaceStyle);
        }

        if (_pressed)
        {
            style = style.Merge(PressedSurfaceStyle);
        }

        return style;
    }

    private static TeaStyle SanitizeLabelStyle(TeaStyle style)
    {
        if (style.IsEmpty)
        {
            return style;
        }

        return style with
        {
            Background = null,
            Inverse = null,
            Framed = null,
            Encircled = null,
            Conceal = null,
        };
    }

    private static string ResolveRenderedLabel(
        string chromeLabel,
        string plainLabel,
        int contentWidth,
        BorderStyle shellBorder,
        TeaStyle surfaceStyle)
    {
        if (chromeLabel == plainLabel || contentWidth <= 0)
        {
            return chromeLabel;
        }

        if (shellBorder != BorderStyle.Rounded || surfaceStyle.IsEmpty)
        {
            return chromeLabel;
        }

        return ControlTextLayout.MeasureDisplayWidth(chromeLabel) > contentWidth
            && ControlTextLayout.MeasureDisplayWidth(plainLabel) <= contentWidth
                ? plainLabel
                : chromeLabel;
    }

    private bool HasSurfaceChrome()
    {
        return !SurfaceStyle.IsEmpty || !FocusedSurfaceStyle.IsEmpty || !PressedSurfaceStyle.IsEmpty;
    }

    private static bool ShouldRenderFilledRoundedShell(
        BorderStyle shellBorder,
        TeaStyle surfaceStyle,
        ButtonRoundedSurfaceMode roundedSurfaceMode)
    {
        return shellBorder == BorderStyle.Rounded
            && !surfaceStyle.IsEmpty
            && roundedSurfaceMode == ButtonRoundedSurfaceMode.UnifiedShell;
    }

    private static void WriteBorderGlyph(Canvas canvas, int x, int y, char glyph, TeaStyle borderStyleText)
    {
        var token = borderStyleText.IsEmpty
            ? glyph.ToString()
            : borderStyleText.Render(glyph.ToString());
        canvas.WriteText(x, y, token, 1);
    }

    private BorderStyle ResolveShellBorderStyle()
    {
        return Border != BorderStyle.None || !HasSurfaceChrome()
            ? Border
            : BorderStyle.Rounded;
    }
}
