using TeaSharp.Components.Advanced.Internal;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Advanced;

internal sealed partial class NotificationCenterComponent
{
    private Rect ResolveRenderContentRect(Canvas canvas, Rect clipped)
    {
        return FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : IsFocused ? $"{Title} *" : Title,
            Border,
            Padding);
    }

    private void RenderEntries(Canvas canvas, Rect content)
    {
        if (_entries.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, EntryStatePalette.Render("(empty)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_entries.Count, start + content.Height);
        var row = 0;
        for (var i = start; i < end; i++, row++)
        {
            var entry = _entries[i];
            var cursor = i == _selectedIndex ? ">" : " ";
            var readMark = entry.IsRead ? " " : "•";
            var timestamp = ShowTimestamp ? $"{entry.CreatedAt:HH:mm:ss} " : string.Empty;
            var line = $"{cursor}{readMark} {timestamp}{entry.Message}";
            var states = ResolveEntryStates(entry, i == _selectedIndex, i == _hoveredIndex);
            canvas.WriteText(content.X, content.Y + row, EntryStatePalette.Render(line, states), content.Width);
        }
    }

    private List<WidgetVisualState> ResolveEntryStates(NotificationEntry entry, bool selected, bool hovered)
    {
        var states = new List<WidgetVisualState>(7);
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

        if (selected)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (hovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (!entry.IsRead)
        {
            states.Add(WidgetVisualState.New);
        }

        states.Add(entry.Severity switch
        {
            NotificationSeverity.Success => WidgetVisualState.Success,
            NotificationSeverity.Warning => WidgetVisualState.Warning,
            NotificationSeverity.Error => WidgetVisualState.Error,
            _ => WidgetVisualState.Default,
        });
        return states;
    }
}
