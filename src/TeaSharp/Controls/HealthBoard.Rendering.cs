using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

public sealed partial class HealthBoard
{
    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped, title, Border, Padding, ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        if (_services.Count == 0)
        {
            var emptyStyle = IsDisabled ? EmptyStyle.Merge(DisabledServiceStyle) : EmptyStyle;
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, emptyStyle), content.Width);
            return;
        }

        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(_lastViewportRows);
        var visible = Math.Min(content.Height, _services.Count - _scrollOffset);
        for (var row = 0; row < visible; row++)
        {
            var index = _scrollOffset + row;
            var line = FormatLine(_services[index], index);
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, ResolveRowStyle(index)), content.Width);
        }
    }

    private TeaStyle ResolveRowStyle(int index)
    {
        var service = _services[index];
        var style = ServiceStyle.Merge(ResolveSeverityStyle(service.Severity));
        if (service.IsAcknowledged)
        {
            style = style.Merge(AcknowledgedServiceStyle);
        }

        if (service.IsMuted)
        {
            style = style.Merge(MutedServiceStyle);
        }

        if (index == _hoveredIndex)
        {
            style = style.Merge(HoveredServiceStyle);
        }

        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedServiceStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedServiceStyle);
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledServiceStyle);
        }

        return style;
    }

    private TeaStyle ResolveSeverityStyle(HealthServiceSeverity severity)
    {
        return severity switch
        {
            HealthServiceSeverity.Degraded => DegradedServiceStyle,
            HealthServiceSeverity.Outage => OutageServiceStyle,
            _ => HealthyServiceStyle,
        };
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledServiceStyle) : style;
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? string.Concat(Title, " ", FocusMarker)
            : Title;
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledServiceStyle);
        }

        return ApplyStyle(title, style);
    }

    private string FormatLine(HealthService service, int index)
    {
        var marker = ResolveRowMarker(index);
        var severityGlyph = ResolveSeverityGlyph(service.Severity);
        var summary = string.IsNullOrWhiteSpace(service.Summary)
            ? string.Empty
            : string.Concat(" - ", service.Summary);
        var acknowledged = service.IsAcknowledged && _glyphs.AcknowledgedGlyph.Length > 0
            ? string.Concat(" [", _glyphs.AcknowledgedGlyph, "]")
            : string.Empty;
        return string.Concat(marker, _glyphs.MarkerSeparator, severityGlyph, _glyphs.MarkerSeparator, service.Name, summary, acknowledged);
    }

    private string ResolveRowMarker(int index)
    {
        if (index == _selectedIndex)
        {
            return _glyphs.SelectedRowMarker;
        }

        if (index == _hoveredIndex)
        {
            return _glyphs.HoveredRowMarker;
        }

        return _glyphs.NormalRowMarker;
    }

    private string ResolveSeverityGlyph(HealthServiceSeverity severity)
    {
        return severity switch
        {
            HealthServiceSeverity.Degraded => _glyphs.DegradedGlyph,
            HealthServiceSeverity.Outage => _glyphs.OutageGlyph,
            _ => _glyphs.HealthyGlyph,
        };
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
