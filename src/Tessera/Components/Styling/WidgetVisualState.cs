using System.ComponentModel;

namespace Tessera.Components.Styling;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal enum WidgetVisualState
{
    Default = 0,
    Focused = 1,
    Hovered = 2,
    Active = 3,
    Selected = 4,
    Disabled = 5,
    ReadOnly = 6,
    Loading = 7,
    Success = 8,
    Warning = 9,
    Error = 10,
    Empty = 11,
    Editing = 12,
    Expanded = 13,
    Collapsed = 14,
    Checked = 15,
    Unchecked = 16,
    Indeterminate = 17,
    Dragging = 18,
    DropTarget = 19,
    Cursor = 20,
    Marked = 21,
    Completed = 22,
    FilteredOut = 23,
    New = 24,
    Stale = 25
}
