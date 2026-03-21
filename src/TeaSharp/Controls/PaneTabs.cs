using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>Represents a tab strip used for pane/workspace switching.</summary>
public sealed class PaneTabs : Control
{
    private readonly List<PaneTabItem> _tabs = [];
    private int _selectedIndex;
    private int _hoveredIndex = -1;

    /// <summary>Occurs when selected tab changes.</summary>
    public event EventHandler<PaneTabSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>Gets or sets optional title rendered before tab labels.</summary>
    public string Title { get; set => field = value ?? string.Empty; } = string.Empty;
    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set => field = value ?? string.Empty; } = "*";
    /// <summary>Gets or sets whether the focus marker is shown while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets title style while unfocused.</summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets title style while focused.</summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets base tab style.</summary>
    public TeaStyle TabStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets selected-tab style.</summary>
    public TeaStyle SelectedTabStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets selected-tab style while focused.</summary>
    public TeaStyle FocusedSelectedTabStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets hovered-tab style.</summary>
    public TeaStyle HoveredTabStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets disabled-tab style.</summary>
    public TeaStyle DisabledTabStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets style applied to separators between tabs.</summary>
    public TeaStyle SeparatorStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets border style while unfocused.</summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets border style while focused.</summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets style merged while control is disabled.</summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets style used for empty-state text.</summary>
    public TeaStyle EmptyTextStyle { get; set; } = TeaStyle.Empty;

    /// <summary>Gets or sets border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }
    /// <summary>Gets or sets separator rendered between tabs.</summary>
    public string Separator { get; set; } = "│";
    /// <summary>Gets or sets selected-tab prefix marker.</summary>
    public string SelectedPrefix { get; set; } = "[";
    /// <summary>Gets or sets selected-tab suffix marker.</summary>
    public string SelectedSuffix { get; set; } = "]";
    /// <summary>Gets or sets text shown when no tabs are configured.</summary>
    public string EmptyText { get; set; } = "(no tabs)";

    /// <summary>Gets configured tabs.</summary>
    public IReadOnlyList<PaneTabItem> Tabs => _tabs;
    /// <summary>Gets selected index, or <c>-1</c> when no tabs exist.</summary>
    public int SelectedIndex => _tabs.Count == 0 ? -1 : _selectedIndex;
    /// <summary>Gets selected tab, if any.</summary>
    public PaneTabItem? SelectedItem => _tabs.Count == 0 ? null : _tabs[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Replaces tab items.</summary>
    /// <param name="tabs">Tabs in visual order.</param>
    public void SetTabs(IEnumerable<PaneTabItem> tabs)
    {
        ArgumentNullException.ThrowIfNull(tabs);
        _tabs.Clear();
        foreach (var tab in tabs)
        {
            if (tab is null) continue;
            _tabs.Add(new PaneTabItem(tab.Id, tab.Title, tab.IsDisabled) { IsDirty = tab.IsDirty });
        }

        if (_tabs.Count == 0)
        {
            _selectedIndex = 0;
            _hoveredIndex = -1;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, _tabs.Count - 1);
        if (_tabs[_selectedIndex].IsDisabled)
        {
            _selectedIndex = ResolveNextEnabled(_selectedIndex, +1) ?? ResolveNextEnabled(_selectedIndex, -1) ?? 0;
        }
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _tabs.Count - 1);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _tabs.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h')) return MoveSelection(-1);
        if (key.Is(Key.Right) || key.IsCharacter('l')) return MoveSelection(+1);
        if (key.Is(Key.Home))
        {
            var firstEnabled = ResolveNextEnabled(-1, +1);
            return firstEnabled.HasValue && SetSelectedIndex(firstEnabled.Value);
        }
        if (key.Is(Key.End))
        {
            var lastEnabled = ResolveNextEnabled(_tabs.Count, -1);
            return lastEnabled.HasValue && SetSelectedIndex(lastEnabled.Value);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || _tabs.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty) return Handle(message);

        var inRow = content.Contains(pointer.X, pointer.Y) && pointer.Y == content.Y;
        if (!inRow)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                return SetHoveredIndex(-1) || Handle(message);
            }

            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown) return MoveSelection(+1);
            if (pointer.Button == PointerButton.WheelUp) return MoveSelection(-1);
            return false;
        }

        var hit = HitTestTab(pointer.X, content);
        if (pointer.Kind == PointerEventKind.Motion) return SetHoveredIndex(hit);
        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hit >= 0 && !_tabs[hit].IsDisabled)
        {
            RequestFocus();
            return SetSelectedIndex(hit);
        }

        return false;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty) return;

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty) return;

        if (_tabs.Count == 0)
        {
            var emptyStyle = IsDisabled ? EmptyTextStyle.Merge(DisabledStyle) : EmptyTextStyle;
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, emptyStyle), content.Width);
            return;
        }

        var x = content.X;
        for (var index = 0; index < _tabs.Count && x < content.Right; index++)
        {
            var label = FormatLabel(index);
            canvas.WriteText(x, content.Y, ApplyStyle(label, ResolveTabStyle(index)), content.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(label);
            if (index < _tabs.Count - 1 && x < content.Right)
            {
                canvas.WriteText(x, content.Y, ApplyStyle(Separator, ResolveSeparatorStyle()), content.Right - x);
                x += ControlTextLayout.MeasureDisplayWidth(Separator);
            }
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth(MeasureTitle());
        if (width > 0 && _tabs.Count > 0) width += 1;
        for (var index = 0; index < _tabs.Count; index++)
        {
            width += ControlTextLayout.MeasureDisplayWidth(FormatLabel(index));
            if (index < _tabs.Count - 1) width += ControlTextLayout.MeasureDisplayWidth(Separator);
        }

        if (_tabs.Count == 0) width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(EmptyText));

        var height = 1;
        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(Math.Clamp(Math.Max(width, 8), 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool MoveSelection(int direction)
    {
        if (_tabs.Count == 0) return false;
        var next = ResolveNextEnabled(_selectedIndex, direction);
        return next.HasValue && SetSelectedIndex(next.Value);
    }

    private int? ResolveNextEnabled(int start, int direction)
    {
        if (_tabs.Count == 0) return null;
        var index = start;
        for (var i = 0; i < _tabs.Count; i++)
        {
            index += direction;
            if (index < 0) index = _tabs.Count - 1;
            else if (index >= _tabs.Count) index = 0;
            if (!_tabs[index].IsDisabled) return index;
        }

        return null;
    }

    /// <summary>Sets the selected tab index.</summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed; otherwise <see langword="false"/>.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (index < 0 || index >= _tabs.Count || _tabs[index].IsDisabled || index == _selectedIndex) return false;
        var previous = _selectedIndex;
        var previousItem = SelectedItem;
        _selectedIndex = index;
        SelectionChanged?.Invoke(this, new PaneTabSelectionChangedEventArgs(previous, _selectedIndex, previousItem, SelectedItem));
        return true;
    }

    private int HitTestTab(int x, Rect content)
    {
        var cursor = content.X;
        for (var index = 0; index < _tabs.Count && cursor < content.Right; index++)
        {
            var label = FormatLabel(index);
            var width = ControlTextLayout.MeasureDisplayWidth(label);
            if (x >= cursor && x < cursor + width) return index;
            cursor += width;
            if (index < _tabs.Count - 1) cursor += ControlTextLayout.MeasureDisplayWidth(Separator);
        }

        return -1;
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index) return false;
        _hoveredIndex = index;
        return true;
    }

    private string FormatLabel(int index)
    {
        var tab = _tabs[index];
        var title = tab.IsDirty ? $"{tab.Title}*" : tab.Title;
        return index == _selectedIndex ? $"{SelectedPrefix}{title}{SelectedSuffix}" : $" {title} ";
    }

    private TeaStyle ResolveTabStyle(int index)
    {
        var tab = _tabs[index];
        var style = TabStyle;
        if (index == _selectedIndex)
        {
            style = style.Merge(SelectedTabStyle);
            if (IsFocused) style = style.Merge(FocusedSelectedTabStyle);
        }
        else if (index == _hoveredIndex)
        {
            style = style.Merge(HoveredTabStyle);
        }

        if (tab.IsDisabled) style = style.Merge(DisabledTabStyle);
        if (IsDisabled) style = style.Merge(DisabledStyle);
        return style;
    }

    private TeaStyle ResolveSeparatorStyle()
    {
        var style = SeparatorStyle;
        if (IsDisabled) style = style.Merge(DisabledStyle);
        return style;
    }

    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        if (IsDisabled) style = style.Merge(DisabledStyle);
        return style;
    }

    private string RenderTitle()
    {
        var title = MeasureTitle();
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (IsDisabled) style = style.Merge(DisabledStyle);
        return ApplyStyle(title, style);
    }

    private string MeasureTitle()
    {
        if (string.IsNullOrEmpty(Title)) return string.Empty;
        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title;
    }

    private static string ApplyStyle(string value, TeaStyle style) => style.IsEmpty ? value : style.Render(value);
}
