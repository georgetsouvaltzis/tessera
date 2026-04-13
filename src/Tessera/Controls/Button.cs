using System.ComponentModel;
using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents an activatable push button.
/// </summary>
/// <remarks>
///     Buttons are rectangular action surfaces. Surface styling owns the whole button body, while label styles
///     remain text-only semantics layered on top of that body.
/// </remarks>
public sealed class Button : Control
{
    private bool _hovered;
    private bool _pendingActivation;

    /// <summary>
    ///     Gets or sets the button label.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets text rendered before <see cref="Text" /> inside the button label.
    /// </summary>
    public string LabelPrefix { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets text rendered after <see cref="Text" /> inside the button label.
    /// </summary>
    public string LabelSuffix { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the optional secondary description shown with the button.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     Gets or sets the inner padding applied to the button body.
    /// </summary>
    public Thickness Padding { get; set; } = Thickness.Symmetric(1);

    /// <summary>
    ///     Gets or sets the base style applied to the button label.
    ///     Background-like facets are ignored so the button body remains a single surface.
    /// </summary>
    public TesseraStyle LabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into the label style when the button is focused.
    ///     Background-like facets are ignored so focus remains label-led instead of creating an inner chip.
    /// </summary>
    public TesseraStyle FocusedLabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into the label style when the button is disabled.
    ///     Background-like facets are ignored so the button body remains a single surface.
    /// </summary>
    public TesseraStyle DisabledLabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into the label style when the button is pressed.
    ///     Background-like facets are ignored so pressed state stays surface-led.
    /// </summary>
    public TesseraStyle PressedLabelStyle { get; set; } = TesseraStyle.Empty.WithInverse().WithBold();

    /// <summary>
    ///     Gets or sets the style applied to the button body across the padded content area.
    /// </summary>
    public TesseraStyle SurfaceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into <see cref="SurfaceStyle" /> while the button is focused.
    /// </summary>
    public TesseraStyle FocusedSurfaceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style merged into <see cref="SurfaceStyle" /> while the button is pressed.
    /// </summary>
    public TesseraStyle PressedSurfaceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets how many activations have been observed.
    /// </summary>
    public int ActivationCount { get; private set; }

    /// <summary>
    ///     Gets a value indicating whether the button is currently pressed.
    /// </summary>
    public bool IsPressed { get; private set; }

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <summary>
    ///     Occurs when the button is activated by input.
    /// </summary>
    public event EventHandler? Activated;

    /// <summary>
    ///     Attempts to consume a pending activation.
    /// </summary>
    /// <returns><see langword="true" /> when an activation was consumed; otherwise, <see langword="false" />.</returns>
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

    /// <inheritdoc />
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

        Activate(false);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        if (!bounds.Contains(pointer.X, pointer.Y))
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press or PointerEventKind.Release)
            {
                var hoverChanged = SetHovered(false);
                var pressedChanged = SetPressed(false);
                return hoverChanged || pressedChanged || Handle(message);
            }

            return Handle(message);
        }

        switch (pointer.Kind)
        {
            case PointerEventKind.Motion:
                return SetHovered(true);
            case PointerEventKind.Press when pointer.Button == PointerButton.Left:
                SetHovered(true);
                Activate(true);
                return true;
            case PointerEventKind.Release when pointer.Button == PointerButton.Left:
                return SetPressed(false);
            default:
                return Handle(message);
        }
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var surfaceStyle = ResolveSurfaceStyle();
        FillSurface(canvas, clipped, surfaceStyle);

        var content = ResolveContentRect(clipped, Padding);
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var label = $"{LabelPrefix}{Text}{LabelSuffix}";
        if (IsDisabled)
        {
            label += " (disabled)";
        }

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
        if (IsDisabled)
        {
            labelWidth += ControlTextLayout.MeasureDisplayWidth(" (disabled)");
        }

        var descriptionWidth = string.IsNullOrWhiteSpace(Description)
            ? 0
            : ControlTextLayout.MeasureDisplayWidth(Description);
        var contentWidth = Math.Max(labelWidth, descriptionWidth);
        var width = contentWidth + Padding.Horizontal;
        var height = Padding.Vertical + (string.IsNullOrWhiteSpace(Description) ? 1 : 2);

        if (width > contentWidth && ((width - contentWidth) & 1) != 0)
        {
            width++;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void Activate(bool pressed)
    {
        ActivationCount++;
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
        if (IsPressed == pressed)
        {
            return false;
        }

        IsPressed = pressed;
        return true;
    }

    private string ApplyLabelStyle(string label, TesseraStyle surfaceStyle)
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

        if (IsPressed)
        {
            style = style.Merge(SanitizeLabelStyle(PressedLabelStyle));
        }

        return style.IsEmpty ? label : style.Render(label);
    }

    private TesseraStyle ResolveSurfaceStyle()
    {
        var style = SurfaceStyle;
        if (IsFocused)
        {
            style = style.Merge(FocusedSurfaceStyle);
        }

        if (IsPressed)
        {
            style = style.Merge(PressedSurfaceStyle);
        }

        return style;
    }

    private static TesseraStyle SanitizeLabelStyle(TesseraStyle style)
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
            Conceal = null
        };
    }

    private static void FillSurface(Canvas canvas, Rect box, TesseraStyle surfaceStyle)
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

    private static Rect ResolveContentRect(Rect box, Thickness padding)
    {
        if (box.IsEmpty)
        {
            return box;
        }

        var left = ClampInset(padding.Left, box.Width);
        var right = ClampInset(padding.Right, box.Width - left);
        var top = ClampInset(padding.Top, box.Height);
        var bottom = ClampInset(padding.Bottom, box.Height - top);

        return new Rect(
            box.X + left,
            box.Y + top,
            Math.Max(1, box.Width - left - right),
            Math.Max(1, box.Height - top - bottom));
    }

    private static int ClampInset(int requested, int available)
    {
        if (requested <= 0 || available <= 1)
        {
            return 0;
        }

        return Math.Min(requested, available - 1);
    }

    private static void WriteCenteredLabel(Canvas canvas, Rect content, int y, string plainLabel, string renderedLabel)
    {
        if (y < content.Y || y >= content.Bottom)
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
            x += (content.Width - displayWidth) / 2;
        }

        canvas.WriteText(x, y, renderedLabel, displayWidth);
    }
}
