using System.Globalization;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class CommandOutput
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        if (_lines.Count == 0)
        {
            var style = IsDisabled ? EmptyStyle.Merge(DisabledStyle) : EmptyStyle;
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, style), content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(_lastViewportRows);
        var visible = Math.Min(content.Height, _lines.Count - _scrollOffset);
        for (var row = 0; row < visible; row++)
        {
            var lineIndex = _scrollOffset + row;
            var style = ResolveLineStyle(lineIndex);
            var text = FormatLine(_lines[lineIndex]);
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(text, style), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 6);
        for (var index = 0; index < _lines.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatLine(_lines[index])) + 2);
        }

        var height = Math.Max(4, Math.Min(12, _lines.Count + 2));
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private TesseraStyle ResolveLineStyle(int lineIndex)
    {
        var style = ResolveChannelStyle(_lines[lineIndex].Channel);
        if (lineIndex == _hoveredIndex)
        {
            style = style.Merge(HoveredLineStyle);
        }

        if (lineIndex == SelectedIndex)
        {
            style = style.Merge(SelectedLineStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedLineStyle);
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private TesseraStyle ResolveChannelStyle(CommandOutputChannel channel)
    {
        return channel switch
        {
            CommandOutputChannel.StdErr => StdErrStyle,
            CommandOutputChannel.System => SystemStyle,
            _ => StdOutStyle
        };
    }

    private string FormatLine(CommandOutputLine line)
    {
        var channel = line.Channel switch
        {
            CommandOutputChannel.StdErr => "ERR",
            CommandOutputChannel.System => "SYS",
            _ => "OUT"
        };
        var payload = $"{channel} {line.Text}";
        if (!ShowTimestamp)
        {
            return payload;
        }

        var stamp = line.Timestamp.ToString(TimestampFormat, CultureInfo.InvariantCulture);
        if (!TimestampStyle.IsEmpty)
        {
            stamp = TimestampStyle.Render(stamp);
        }

        return $"{stamp} {payload}";
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string MeasureTitle()
    {
        return ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
