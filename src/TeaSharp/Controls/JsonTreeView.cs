using System.Text.Json;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a navigable JSON inspection tree.
/// </summary>
public sealed partial class JsonTreeView : Control
{
    private readonly List<JsonTreeNode> _roots = [];
    private readonly List<(JsonTreeNode Node, int Depth, int? ParentIndex)> _visible = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    /// <summary>
    /// Occurs when selected node changes.
    /// </summary>
    public event EventHandler<JsonTreeSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "JSON";

    /// <summary>
    /// Gets or sets marker shown in title when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker" /> is shown while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets title style when not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style when not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style when focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for container rows.
    /// </summary>
    public TeaStyle ContainerStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for scalar value rows.
    /// </summary>
    public TeaStyle ValueStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rows.
    /// </summary>
    public TeaStyle HoveredRowStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows while focused.
    /// </summary>
    public TeaStyle FocusedSelectedRowStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rows while disabled.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style for muted/empty text.
    /// </summary>
    public TeaStyle MutedStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets marker for expanded container nodes.
    /// </summary>
    public string ExpandedMarker { get; set; } = "▼";

    /// <summary>
    /// Gets or sets marker for collapsed container nodes.
    /// </summary>
    public string CollapsedMarker { get; set; } = "▶";

    /// <summary>
    /// Gets or sets marker for scalar value nodes.
    /// </summary>
    public string ValueMarker { get; set; } = "•";

    /// <summary>
    /// Gets or sets text rendered when there are no nodes.
    /// </summary>
    public string EmptyText { get; set; } = "(empty)";

    /// <summary>
    /// Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets configured root nodes.
    /// </summary>
    public IReadOnlyList<JsonTreeNode> Roots => _roots;

    /// <summary>
    /// Gets selected visible index, or <c>-1</c> when empty.
    /// </summary>
    public int SelectedIndex => _visible.Count == 0 ? -1 : _selectedIndex;

    /// <summary>
    /// Gets selected node, or <see langword="null"/> when no selection.
    /// </summary>
    public JsonTreeNode? SelectedNode => _selectedIndex >= 0 && _selectedIndex < _visible.Count
        ? _visible[_selectedIndex].Node
        : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces root nodes.
    /// </summary>
    /// <param name="roots">Root nodes.</param>
    public void SetRoots(IEnumerable<JsonTreeNode> roots)
    {
        ArgumentNullException.ThrowIfNull(roots);
        _roots.Clear();
        foreach (var node in roots)
        {
            if (node is not null)
            {
                _roots.Add(Clone(node));
            }
        }

        RefreshVisible();
    }

    /// <summary>
    /// Parses JSON and replaces tree roots.
    /// </summary>
    /// <param name="json">JSON payload.</param>
    public void SetJson(string json)
    {
        var parsed = ParseJson(json);
        SetRoots(parsed);
    }

    /// <summary>
    /// Attempts to parse JSON and replace tree roots.
    /// </summary>
    /// <param name="json">JSON payload.</param>
    /// <param name="error">Parsing error text, when parsing fails.</param>
    /// <returns><see langword="true"/> on success; otherwise <see langword="false"/>.</returns>
    public bool TrySetJson(string json, out string? error)
    {
        try
        {
            SetJson(json);
            error = null;
            return true;
        }
        catch (JsonException exception)
        {
            error = exception.Message;
            return false;
        }
    }

    /// <summary>
    /// Sets selected row using bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed; otherwise <see langword="false"/>.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_visible.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _visible.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousNode = SelectedNode;
        _selectedIndex = clamped;
        SelectionChanged?.Invoke(
            this,
            new JsonTreeSelectionChangedEventArgs(previousIndex, _selectedIndex, previousNode, SelectedNode));
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _visible.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j')) return SetSelectedIndex(_selectedIndex + 1);
        if (key.Is(Key.Up) || key.IsCharacter('k')) return SetSelectedIndex(_selectedIndex - 1);
        if (key.Is(Key.Home)) return SetSelectedIndex(0);
        if (key.Is(Key.End)) return SetSelectedIndex(_visible.Count - 1);
        if (key.Is(Key.Right) || key.IsCharacter('l')) return ExpandOrMoveChild();
        if (key.Is(Key.Left) || key.IsCharacter('h')) return CollapseOrMoveParent();
        if (key.Is(Key.Enter) || key.IsCharacter(' ')) return ToggleSelectedExpansion();
        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed |= SetHoveredIndex(-1);
        }

        if (pointer.Kind == PointerEventKind.Wheel && _visible.Count > 0)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1) || changed;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1) || changed;
            }
        }

        if (!inside)
        {
            return changed;
        }

        var hovered = ComputeWindowStart(content.Height) + (pointer.Y - content.Y);
        if (hovered < 0 || hovered >= _visible.Count)
        {
            hovered = -1;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            changed |= SetHoveredIndex(hovered);
            changed |= SetSelectedIndex(hovered);
            return changed;
        }

        return changed;
    }

    private bool ToggleSelectedExpansion()
    {
        var node = SelectedNode;
        if (node is null || !node.IsContainer || node.Children.Count == 0)
        {
            return false;
        }

        node.Expanded = !node.Expanded;
        RefreshVisible();
        return true;
    }

    private bool ExpandOrMoveChild()
    {
        var entry = _visible[_selectedIndex];
        if (!entry.Node.IsContainer || entry.Node.Children.Count == 0)
        {
            return false;
        }

        if (!entry.Node.Expanded)
        {
            entry.Node.Expanded = true;
            RefreshVisible();
            return true;
        }

        if (_selectedIndex + 1 < _visible.Count && _visible[_selectedIndex + 1].Depth > entry.Depth)
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        return false;
    }

    private bool CollapseOrMoveParent()
    {
        var entry = _visible[_selectedIndex];
        if (entry.Node.IsContainer && entry.Node.Expanded && entry.Node.Children.Count > 0)
        {
            entry.Node.Expanded = false;
            RefreshVisible();
            return true;
        }

        if (entry.ParentIndex is int parent)
        {
            return SetSelectedIndex(parent);
        }

        return false;
    }

    private void RefreshVisible()
    {
        var previousNode = SelectedNode;
        _visible.Clear();
        foreach (var root in _roots)
        {
            AppendVisible(root, depth: 0, parentIndex: null);
        }

        if (_visible.Count == 0)
        {
            _selectedIndex = 0;
            _hoveredIndex = -1;
            if (previousNode is not null)
            {
                SelectionChanged?.Invoke(this, new JsonTreeSelectionChangedEventArgs(0, -1, previousNode, null));
            }

            return;
        }

        var previousIndex = _selectedIndex;
        _selectedIndex = Math.Clamp(_selectedIndex, 0, _visible.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _visible.Count - 1);
        if (previousIndex != _selectedIndex || !ReferenceEquals(previousNode, SelectedNode))
        {
            SelectionChanged?.Invoke(
                this,
                new JsonTreeSelectionChangedEventArgs(previousIndex, _selectedIndex, previousNode, SelectedNode));
        }
    }

    private void AppendVisible(JsonTreeNode node, int depth, int? parentIndex)
    {
        var visibleIndex = _visible.Count;
        _visible.Add((node, depth, parentIndex));
        if (!node.IsContainer || !node.Expanded || node.Children.Count == 0)
        {
            return;
        }

        for (var index = 0; index < node.Children.Count; index++)
        {
            AppendVisible(node.Children[index], depth + 1, visibleIndex);
        }
    }

    private int ComputeWindowStart(int contentHeight)
    {
        return Math.Clamp(_selectedIndex - (contentHeight / 2), 0, Math.Max(0, _visible.Count - contentHeight));
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

    private static JsonTreeNode Clone(JsonTreeNode node)
    {
        var copy = new JsonTreeNode(node.Key, node.DisplayValue, node.Kind)
        {
            Expanded = node.Expanded,
        };
        for (var index = 0; index < node.Children.Count; index++)
        {
            copy.Children.Add(Clone(node.Children[index]));
        }

        return copy;
    }
}
