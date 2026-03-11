using TeaSharp.Components.Advanced.Internal;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Advanced;

public sealed partial class NotificationCenterComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<NotificationEntry> _entries = [];
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    public string Title { get; set; } = "Notifications";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public int MaxEntries { get; set; } = 128;

    public bool ShowTimestamp { get; set; } = true;

    public bool AutoSelectNewest { get; set; } = true;

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding MarkReadKey { get; set; } = new("enter/space", "mark read", "enter", "space");

    public KeyBinding DismissKey { get; set; } = new("d", "dismiss", "d");

    public KeyBinding ClearAllKey { get; set; } = new("c", "clear all", "c");

    public WidgetStatePalette EntryStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public IReadOnlyList<NotificationEntry> Entries => _entries;

    public void Push(string message, NotificationSeverity severity = NotificationSeverity.Info, string? id = null)
    {
        var entry = new NotificationEntry(
            id ?? Guid.NewGuid().ToString("n"),
            message,
            severity,
            DateTimeOffset.UtcNow,
            IsRead: false);
        _entries.Add(entry);
        if (_entries.Count > MaxEntries)
        {
            _entries.RemoveAt(0);
        }

        if (AutoSelectNewest)
        {
            _selectedIndex = Math.Max(0, _entries.Count - 1);
        }
        else
        {
            _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _entries.Count - 1));
        }
    }

    public void Clear()
    {
        _entries.Clear();
        _selectedIndex = 0;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (ClearAllKey.Matches(key))
        {
            if (_entries.Count == 0)
            {
                return false;
            }

            Clear();
            return true;
        }

        if (_entries.Count == 0)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            var previous = _selectedIndex;
            _selectedIndex = Math.Min(_entries.Count - 1, _selectedIndex + 1);
            return _selectedIndex != previous;
        }

        if (PreviousItemKey.Matches(key))
        {
            var previous = _selectedIndex;
            _selectedIndex = Math.Max(0, _selectedIndex - 1);
            return _selectedIndex != previous;
        }

        if (DismissKey.Matches(key))
        {
            _entries.RemoveAt(_selectedIndex);
            _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _entries.Count - 1));
            return true;
        }

        if (MarkReadKey.Matches(key))
        {
            var current = _entries[_selectedIndex];
            if (current.IsRead)
            {
                return false;
            }

            _entries[_selectedIndex] = current with { IsRead = true };
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || ReadOnly)
        {
            return false;
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return false;
        }

        var inside = content.Contains(message.X, message.Y);
        var changed = false;
        if (!inside)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= SetHoveredIndex(-1);
            }

            if (message is not MouseWheelMsg)
            {
                return changed;
            }
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && _entries.Count > 0)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                var previous = _selectedIndex;
                _selectedIndex = Math.Min(_entries.Count - 1, _selectedIndex + 1);
                changed |= _selectedIndex != previous;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                var previous = _selectedIndex;
                _selectedIndex = Math.Max(0, _selectedIndex - 1);
                changed |= _selectedIndex != previous;
            }
        }

        if (!inside || _entries.Count == 0)
        {
            return changed;
        }

        var start = ComputeWindowStart(content.Height);
        var hovered = start + (message.Y - content.Y);
        if (hovered < 0 || hovered >= _entries.Count)
        {
            hovered = -1;
        }

        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredIndex(hovered);
            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= SetHoveredIndex(hovered);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && hovered >= 0)
            {
                if (_selectedIndex != hovered)
                {
                    _selectedIndex = hovered;
                    changed = true;
                }
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = ResolveRenderContentRect(canvas, clipped);
        if (content.IsEmpty)
        {
            return;
        }

        RenderEntries(canvas, content);
    }

    private int ComputeWindowStart(int contentHeight) =>
        Math.Clamp(_selectedIndex - (contentHeight / 2), 0, Math.Max(0, _entries.Count - contentHeight));

    private Rect ResolveContentRect(Rect bounds) =>
        ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }
}
