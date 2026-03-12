using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit.Internal;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Components.UiKit;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class TabsComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<string> _tabs = [];
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private int _hoveredIndex = -1;

    public TabsComponent(IEnumerable<string> tabs)
    {
        _tabs.AddRange(tabs);
    }

    public TabsComponent(TabsOptions options)
        : this(options.Tabs)
    {
        IsFocused = options.IsFocused;
        EnableNumericShortcuts = options.EnableNumericShortcuts;
        NextTabKey = options.NextTabKey ?? new KeyBinding("right", "next tab", "right");
        PreviousTabKey = options.PreviousTabKey ?? new KeyBinding("left", "previous tab", "left");
        InteractionProfile = options.InteractionProfile ?? WidgetInteractionProfile.Default;
    }

    public int SelectedIndex { get; private set; }

    public IReadOnlyList<string> Tabs => _tabs;

    public bool IsFocused { get; set; }

    /// <summary>
    /// Raised when the selected tab changes.
    /// </summary>
    public event EventHandler<TabSelectionChangedEventArgs>? SelectionChanged;

    public WidgetStatePalette TabStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public KeyBinding NextTabKey { get; set; } = new("right", "next tab", "right");

    public KeyBinding PreviousTabKey { get; set; } = new("left", "previous tab", "left");

    public bool EnableNumericShortcuts { get; set; } = true;

    public bool Update(IMessage message)
    {
        if (_tabs.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextTabKey.Matches(key))
        {
            return SetSelectedIndex((SelectedIndex + 1) % _tabs.Count);
        }

        if (PreviousTabKey.Matches(key))
        {
            return SetSelectedIndex((SelectedIndex + _tabs.Count - 1) % _tabs.Count);
        }

        if (EnableNumericShortcuts
            && key.TryGetDigit(out var oneBased)
            && oneBased >= 1
            && oneBased <= _tabs.Count)
        {
            return SetSelectedIndex(oneBased - 1);
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (_tabs.Count == 0 || bounds.IsEmpty)
        {
            return false;
        }

        if (message is MouseWheelMsg wheel
            && InteractionProfile.NavigateOnWheel
            && bounds.Contains(wheel.X, wheel.Y))
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                return SetSelectedIndex((SelectedIndex + 1) % _tabs.Count);
            }

            if (wheel.Button == MouseButton.WheelUp)
            {
                return SetSelectedIndex((SelectedIndex + _tabs.Count - 1) % _tabs.Count);
            }
        }

        if (!bounds.Contains(message.X, message.Y) || message.Y != bounds.Y)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                return SetHoveredIndex(-1);
            }

            return false;
        }

        var changed = false;
        var hovered = HitTestTabIndex(message.X, bounds);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= SetHoveredIndex(hovered);
        }
        else if (message is MouseClickMsg && InteractionProfile.HoverOnClick)
        {
            changed |= SetHoveredIndex(hovered);
        }

        if (message is MouseClickMsg { Button: MouseButton.Left }
            && InteractionProfile.ActivateOnClick
            && hovered >= 0
            && hovered < _tabs.Count
            && hovered != SelectedIndex)
        {
            changed |= SetSelectedIndex(hovered);
        }

        return changed;
    }

    public void Select(int index)
    {
        if (_tabs.Count == 0)
        {
            SelectedIndex = 0;
            return;
        }

        SetSelectedIndex(Math.Clamp(index, 0, _tabs.Count - 1));
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1 || _tabs.Count == 0)
        {
            return;
        }

        var x = clipped.X;
        for (var i = 0; i < _tabs.Count && x < clipped.Right; i++)
        {
            var active = i == SelectedIndex;
            var label = active
                ? $"[{i + 1}:{_tabs[i]}]"
                : $" {i + 1}:{_tabs[i]} ";
            var states = ResolveTabStates(i, active);
            canvas.WriteText(x, clipped.Y, TabStatePalette.Render(label, states), clipped.Right - x);
            x += label.Length + 1;
        }
    }

    private List<WidgetVisualState> ResolveTabStates(int index, bool active)
    {
        var states = new List<WidgetVisualState>(4);
        if (IsFocused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (active)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (index == _hoveredIndex)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
    }

    private int HitTestTabIndex(int x, Rect bounds)
    {
        var cursor = bounds.X;
        for (var i = 0; i < _tabs.Count && cursor < bounds.Right; i++)
        {
            var label = i == SelectedIndex
                ? $"[{i + 1}:{_tabs[i]}]"
                : $" {i + 1}:{_tabs[i]} ";
            var end = cursor + label.Length;
            if (x >= cursor && x < end)
            {
                return i;
            }

            cursor = end + 1;
        }

        return -1;
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private bool SetSelectedIndex(int index)
    {
        if (_tabs.Count == 0)
        {
            SelectedIndex = 0;
            return false;
        }

        var clamped = Math.Clamp(index, 0, _tabs.Count - 1);
        if (clamped == SelectedIndex)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousTab = _tabs[previousIndex];
        SelectedIndex = clamped;
        SelectionChanged?.Invoke(this, new TabSelectionChangedEventArgs(previousIndex, SelectedIndex, previousTab, _tabs[SelectedIndex]));
        return true;
    }
}
