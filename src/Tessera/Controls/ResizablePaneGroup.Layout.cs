using Tessera.Components.Primitives;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class ResizablePaneGroup
{
    private bool TryResolveLayout(Rect content, out PaneLayoutInfo layout)
    {
        layout = default;
        if (content.IsEmpty || _panes.Count == 0)
        {
            return false;
        }

        var dividerCount = ShowDividers ? _panes.Count - 1 : 0;
        var dividerTotal = dividerCount * DividerThickness;
        if (content.Width <= dividerTotal)
        {
            return false;
        }

        var availableWidth = content.Width - dividerTotal;
        var paneRects = new Rect[_panes.Count];
        var dividerRects = dividerCount == 0 ? Array.Empty<Rect>() : new Rect[dividerCount];
        var x = content.X;
        var consumed = 0;

        for (var paneIndex = 0; paneIndex < _panes.Count; paneIndex++)
        {
            var paneWidth = paneIndex == _panes.Count - 1
                ? availableWidth - consumed
                : Math.Max(1, (int)Math.Round(_splitRatios[paneIndex] * availableWidth) - consumed);
            paneRects[paneIndex] = new Rect(x, content.Y, paneWidth, content.Height);
            x += paneWidth;
            consumed += paneWidth;

            if (ShowDividers && paneIndex < _panes.Count - 1)
            {
                dividerRects[paneIndex] = new Rect(x, content.Y, DividerThickness, content.Height);
                x += DividerThickness;
            }
        }

        layout = new PaneLayoutInfo(content, paneRects, dividerRects);
        return true;
    }

    private void DrawDividers(Canvas canvas, PaneLayoutInfo layout)
    {
        if (!ShowDividers || layout.Dividers.Length == 0)
        {
            return;
        }

        var glyph = DividerGlyph == '\0' ? '│' : DividerGlyph;
        var style = IsFocused ? DividerStyleText.Merge(FocusedDividerStyleText) : DividerStyleText;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyleText);
        }

        var text = ApplyStyle(glyph.ToString(), style);
        for (var dividerIndex = 0; dividerIndex < layout.Dividers.Length; dividerIndex++)
        {
            var divider = layout.Dividers[dividerIndex];
            for (var row = 0; row < divider.Height; row++)
            {
                canvas.WriteText(divider.X, divider.Y + row, text, divider.Width);
            }
        }
    }

    private string RenderTitle()
    {
        var title = Title;
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            title = $"{title} {FocusMarker}";
        }

        var style = IsFocused ? FocusedTitleStyleText : TitleStyleText;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyleText);
        }

        return ApplyStyle(title, style);
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStyleText);
        }

        return style;
    }

    private TesseraStyle ResolvePaneStyle(int paneIndex)
    {
        var style = PaneStyleText;
        if (paneIndex == _selectedPaneIndex)
        {
            style = style.Merge(SelectedPaneStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyleText);
        }

        return style;
    }

    private static string ResolvePaneText(PaneSpec pane)
    {
        return string.IsNullOrWhiteSpace(pane.Title) ? pane.Id : pane.Title;
    }

    private static int HitTestPane(PaneLayoutInfo layout, int x, int y)
    {
        for (var index = 0; index < layout.Panes.Length; index++)
        {
            if (layout.Panes[index].Contains(x, y))
            {
                return index;
            }
        }

        return -1;
    }

    private static int HitTestDivider(PaneLayoutInfo layout, int x, int y)
    {
        for (var index = 0; index < layout.Dividers.Length; index++)
        {
            if (layout.Dividers[index].Contains(x, y))
            {
                return index;
            }
        }

        return -1;
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty ? text : style.Render(text);
    }

    private readonly record struct PaneLayoutInfo(Rect Content, Rect[] Panes, Rect[] Dividers);
}
