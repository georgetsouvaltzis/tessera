using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a bounded progress indicator.
/// </summary>
public sealed class ProgressBar : Control
{
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Progress";

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

    public TeaStyle FillStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle TrackStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle LabelStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public TeaStyle DisabledStyle
    {
        get;
        set;
    } = TeaStyle.Empty;

    public double Value { get; private set; }

    public double Step
    {
        get;
        set;
    } = 0.05;

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

    public void SetValue(double value) => Value = Math.Clamp(value, 0.0, 1.0);

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('-'))
        {
            SetValue(Value - Step);
            return true;
        }

        if (key.Is(Key.Right) || key.IsCharacter('+'))
        {
            SetValue(Value + Step);
            return true;
        }

        return false;
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
            Padding);
        if (content.IsEmpty)
        {
            return;
        }

        DrawStyledProgressBar(canvas, new Rect(content.X, content.Y, content.Width, 1), Value);
        if (content.Height > 1)
        {
            var percent = (int)Math.Round(Value * 100, MidpointRounding.AwayFromZero);
            var text = $"{percent}%";
            var labelStyle = ResolveLabelStyle();
            if (!labelStyle.IsEmpty)
            {
                text = labelStyle.Render(text);
            }

            canvas.WriteText(content.X, content.Y + 1, text, content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4) + Padding.Horizontal;
        var height = Padding.Vertical + 2;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
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

    private TeaStyle ResolveTrackStyle()
    {
        if (IsDisabled && !DisabledStyle.IsEmpty)
        {
            return TrackStyle.IsEmpty
                ? DisabledStyle
                : TrackStyle.Merge(DisabledStyle);
        }

        return TrackStyle;
    }

    private TeaStyle ResolveFillStyle()
    {
        if (IsDisabled && !DisabledStyle.IsEmpty)
        {
            return FillStyle.IsEmpty
                ? DisabledStyle
                : FillStyle.Merge(DisabledStyle);
        }

        return FillStyle;
    }

    private TeaStyle ResolveLabelStyle()
    {
        if (IsDisabled && !DisabledStyle.IsEmpty)
        {
            return LabelStyle.IsEmpty
                ? DisabledStyle
                : LabelStyle.Merge(DisabledStyle);
        }

        return LabelStyle;
    }

    private static void WriteStyledChar(Canvas canvas, int x, int y, char value, TeaStyle style)
    {
        var text = value.ToString();
        if (!style.IsEmpty)
        {
            text = style.Render(text);
        }

        canvas.WriteText(x, y, text, 1);
    }
}
