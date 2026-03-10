using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class TreeViewComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly List<TreeItemNode> _roots = [];
    private readonly List<(TreeItemNode Node, int Depth, int? ParentVisibleIndex)> _visible = [];
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    public string Title { get; set; } = "Tree";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ExpandKey { get; set; } = new("right/l", "expand", "right", "l");

    public KeyBinding CollapseKey { get; set; } = new("left/h", "collapse", "left", "h");

    public KeyBinding ToggleExpandKey { get; set; } = new("enter/space", "toggle", "enter", "space");

    public WidgetStatePalette NodeStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    public string? SelectedNodeId => _selectedIndex >= 0 && _selectedIndex < _visible.Count
        ? _visible[_selectedIndex].Node.Id
        : null;

    public void SetRoots(IEnumerable<TreeItemNode> roots)
    {
        _roots.Clear();
        _roots.AddRange(roots);
        RefreshVisible();
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly || message is not KeyPressMsg key)
        {
            return false;
        }

        if (_visible.Count == 0)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            var previous = _selectedIndex;
            _selectedIndex = Math.Min(_visible.Count - 1, _selectedIndex + 1);
            return _selectedIndex != previous;
        }

        if (PreviousItemKey.Matches(key))
        {
            var previous = _selectedIndex;
            _selectedIndex = Math.Max(0, _selectedIndex - 1);
            return _selectedIndex != previous;
        }

        if (ExpandKey.Matches(key))
        {
            return ExpandOrMoveIntoChild();
        }

        if (CollapseKey.Matches(key))
        {
            return CollapseOrMoveToParent();
        }

        if (ToggleExpandKey.Matches(key))
        {
            var node = _visible[_selectedIndex].Node;
            if (node.Children.Count == 0)
            {
                return false;
            }

            node.Expanded = !node.Expanded;
            RefreshVisible();
            return true;
        }

        return false;
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled || ReadOnly || _visible.Count == 0)
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

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                var previous = _selectedIndex;
                _selectedIndex = Math.Min(_visible.Count - 1, _selectedIndex + 1);
                changed |= _selectedIndex != previous;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                var previous = _selectedIndex;
                _selectedIndex = Math.Max(0, _selectedIndex - 1);
                changed |= _selectedIndex != previous;
            }
        }

        if (!inside)
        {
            return changed;
        }

        var start = ComputeWindowStart(content.Height);
        var hovered = start + (message.Y - content.Y);
        if (hovered < 0 || hovered >= _visible.Count)
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

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
            content = clipped.Inset(1, 1);
        }
        else
        {
            content = clipped;
        }

        if (content.IsEmpty)
        {
            return;
        }

        if (_visible.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, NodeStatePalette.Render("(empty)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_visible.Count, start + content.Height);
        var row = 0;
        for (var i = start; i < end; i++, row++)
        {
            var (node, depth, _) = _visible[i];
            var indent = new string(' ', Math.Max(0, depth) * 2);
            var marker = node.Children.Count == 0
                ? "•"
                : node.Expanded ? "▾" : "▸";
            var cursor = i == _selectedIndex ? ">" : " ";

            var states = new List<WidgetVisualState>(6);
            if (Focused)
            {
                states.Add(WidgetVisualState.Focused);
            }

            if (Disabled)
            {
                states.Add(WidgetVisualState.Disabled);
            }

            if (ReadOnly)
            {
                states.Add(WidgetVisualState.ReadOnly);
            }

            if (i == _selectedIndex)
            {
                states.Add(WidgetVisualState.Cursor);
                states.Add(WidgetVisualState.Selected);
            }

            if (i == _hoveredIndex)
            {
                states.Add(WidgetVisualState.Hovered);
            }

            states.AddRange(node.States);
            canvas.WriteText(content.X, content.Y + row, NodeStatePalette.Render($"{cursor} {indent}{marker} {node.Label}", states), content.Width);
        }
    }

    private bool ExpandOrMoveIntoChild()
    {
        var node = _visible[_selectedIndex].Node;
        if (node.Children.Count == 0)
        {
            return false;
        }

        if (!node.Expanded)
        {
            node.Expanded = true;
            RefreshVisible();
            return true;
        }

        if (_selectedIndex + 1 < _visible.Count && _visible[_selectedIndex + 1].Depth > _visible[_selectedIndex].Depth)
        {
            _selectedIndex++;
            return true;
        }

        return false;
    }

    private bool CollapseOrMoveToParent()
    {
        var entry = _visible[_selectedIndex];
        var node = entry.Node;
        if (node.Expanded && node.Children.Count > 0)
        {
            node.Expanded = false;
            RefreshVisible();
            return true;
        }

        if (entry.ParentVisibleIndex is int parent)
        {
            _selectedIndex = parent;
            return true;
        }

        return false;
    }

    private void RefreshVisible()
    {
        _visible.Clear();
        for (var i = 0; i < _roots.Count; i++)
        {
            AppendVisible(_roots[i], depth: 0, parentVisibleIndex: null);
        }

        if (_visible.Count == 0)
        {
            _selectedIndex = 0;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, _visible.Count - 1);
    }

    private void AppendVisible(TreeItemNode node, int depth, int? parentVisibleIndex)
    {
        var visibleIndex = _visible.Count;
        _visible.Add((node, depth, parentVisibleIndex));
        if (!node.Expanded || node.Children.Count == 0)
        {
            return;
        }

        for (var i = 0; i < node.Children.Count; i++)
        {
            AppendVisible(node.Children[i], depth + 1, visibleIndex);
        }
    }

    private int ComputeWindowStart(int contentHeight)
    {
        return Math.Clamp(_selectedIndex - (contentHeight / 2), 0, Math.Max(0, _visible.Count - contentHeight));
    }

    private Rect ResolveContentRect(Rect bounds)
    {
        return ShowBorder
            ? bounds.Inset(1, 1)
            : bounds;
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
}
