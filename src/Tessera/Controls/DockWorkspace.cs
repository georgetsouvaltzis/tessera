using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>Represents an IDE-style docked pane workspace.</summary>
public sealed class DockWorkspace : Control
{
    private readonly List<DockPane> _panes = [];
    private int _hoveredIndex = -1;
    private int _selectedIndex;

    /// <summary>Gets or sets layout title.</summary>
    public string Title { get; set; } = "Workspace";

    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>Gets or sets whether focus marker is shown while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets title style while unfocused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets title style while focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets outer border style while unfocused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets outer border style while focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets pane title style.</summary>
    public TesseraStyle PaneTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets selected pane title style.</summary>
    public TesseraStyle SelectedPaneTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets focused selected pane title style.</summary>
    public TesseraStyle FocusedSelectedPaneTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets pane body style.</summary>
    public TesseraStyle PaneBodyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets selected pane body style.</summary>
    public TesseraStyle SelectedPaneBodyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets hovered pane style.</summary>
    public TesseraStyle HoveredPaneStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets muted pane style.</summary>
    public TesseraStyle MutedPaneStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets disabled pane style.</summary>
    public TesseraStyle DisabledPaneStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets pane border style while unselected.</summary>
    public TesseraStyle PaneBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets pane border style while selected and focused.</summary>
    public TesseraStyle FocusedPaneBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into all rendering while disabled.</summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets empty-state style.</summary>
    public TesseraStyle EmptyTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets outer border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets pane border style.</summary>
    public BorderStyle PaneBorder { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets outer content padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets pane content padding.</summary>
    public Thickness PanePadding { get; set; }

    /// <summary>Gets or sets text rendered when there are no panes.</summary>
    public string EmptyText { get; set; } = "(no panes)";

    /// <summary>Gets or sets text rendered for empty pane body.</summary>
    public string PaneEmptyText { get; set; } = "(empty pane)";

    /// <summary>Gets or sets marker prefixed to selected pane title.</summary>
    public string SelectedPaneMarker { get; set; } = ">";

    /// <summary>Gets configured panes.</summary>
    public IReadOnlyList<DockPane> Panes => _panes;

    /// <summary>Gets selected pane index, or <c>-1</c> when no panes exist.</summary>
    public int SelectedIndex => _panes.Count == 0 ? -1 : _selectedIndex;

    /// <summary>Gets selected pane, if any.</summary>
    public DockPane? SelectedPane => _panes.Count == 0 ? null : _panes[_selectedIndex];

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Occurs when selected pane changes.</summary>
    public event EventHandler<ListSelectionChangedEventArgs<DockPane>>? SelectionChanged;

    /// <summary>Replaces panes in docking order.</summary>
    /// <param name="panes">Panes to render.</param>
    public void SetPanes(IEnumerable<DockPane> panes)
    {
        ArgumentNullException.ThrowIfNull(panes);
        _panes.Clear();
        foreach (var pane in panes)
        {
            _panes.Add(ClonePane(pane));
        }

        if (_panes.Count == 0)
        {
            _selectedIndex = 0;
            _hoveredIndex = -1;
            return;
        }

        _selectedIndex = Math.Clamp(_selectedIndex, 0, _panes.Count - 1);
        if (_panes[_selectedIndex].IsDisabled)
        {
            _selectedIndex = ResolveNextEnabled(_selectedIndex, +1) ?? ResolveNextEnabled(_selectedIndex, -1) ?? 0;
        }

        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _panes.Count - 1);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _panes.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.Is(Key.Up) || key.IsCharacter('h') || key.IsCharacter('k'))
        {
            return MoveSelection(-1);
        }

        if (key.Is(Key.Right) || key.Is(Key.Down) || key.IsCharacter('l') || key.IsCharacter('j'))
        {
            return MoveSelection(+1);
        }

        if (key.Is(Key.Home))
        {
            var first = ResolveNextEnabled(-1, +1);
            return first.HasValue && SetSelectedIndex(first.Value);
        }

        if (key.Is(Key.End))
        {
            var last = ResolveNextEnabled(_panes.Count, -1);
            return last.HasValue && SetSelectedIndex(last.Value);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || _panes.Count == 0 || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var paneRects = ResolvePaneRects(content);
        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return MoveSelection(+1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return MoveSelection(-1);
            }

            return false;
        }

        var hit = HitTestPane(pointer.X, pointer.Y, paneRects);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hit);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hit >= 0 &&
            !_panes[hit].IsDisabled)
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
            ResolveOuterBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }

        if (_panes.Count == 0)
        {
            var style = IsDisabled ? EmptyTextStyle.Merge(DisabledStyle) : EmptyTextStyle;
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, style), content.Width);
            return;
        }

        var paneRects = ResolvePaneRects(content);
        for (var index = 0; index < _panes.Count; index++)
        {
            var paneRect = paneRects[index];
            if (paneRect.IsEmpty)
            {
                continue;
            }

            var pane = _panes[index];
            var paneContent = FrameLayout.DrawFrameAndResolveContent(
                canvas,
                paneRect,
                RenderPaneTitle(pane, index == _selectedIndex),
                PaneBorder,
                PanePadding,
                ResolvePaneBorderStyle(index, pane));
            if (paneContent.IsEmpty)
            {
                continue;
            }

            if (pane.Content is not null)
            {
                pane.Content.Render(canvas, paneContent);
                continue;
            }

            RenderPaneLines(canvas, pane, paneContent, index == _selectedIndex, index == _hoveredIndex);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(MeasureTitle()) + 6);
        var height = 10;
        for (var index = 0; index < _panes.Count; index++)
        {
            var pane = _panes[index];
            if (pane.Position is DockPanePosition.Left or DockPanePosition.Right)
            {
                width += Math.Max(4, pane.Size);
            }

            if (pane.Position is DockPanePosition.Top or DockPanePosition.Bottom)
            {
                height += Math.Max(3, pane.Size);
            }
        }

        if (Border != BorderStyle.None)
        {
            width += 2 + Padding.Horizontal;
            height += 2 + Padding.Vertical;
        }

        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private Rect[] ResolvePaneRects(Rect content)
    {
        var rects = new Rect[_panes.Count];
        var remaining = content;
        AssignEdge(DockPanePosition.Top, ref remaining, rects, false);
        AssignEdge(DockPanePosition.Bottom, ref remaining, rects, true);
        AssignEdge(DockPanePosition.Left, ref remaining, rects, false);
        AssignEdge(DockPanePosition.Right, ref remaining, rects, true);
        AssignCenters(remaining, rects);
        return rects;
    }

    private void AssignEdge(DockPanePosition position, ref Rect remaining, Rect[] rects, bool fromEnd)
    {
        for (var index = 0; index < _panes.Count && !remaining.IsEmpty; index++)
        {
            if (_panes[index].Position != position)
            {
                continue;
            }

            var size = position is DockPanePosition.Top or DockPanePosition.Bottom
                ? Math.Clamp(_panes[index].Size, 1, remaining.Height)
                : Math.Clamp(_panes[index].Size, 1, remaining.Width);

            if (position == DockPanePosition.Top)
            {
                rects[index] = new Rect(remaining.X, remaining.Y, remaining.Width, size);
                remaining = new Rect(remaining.X, remaining.Y + size, remaining.Width, remaining.Height - size);
                continue;
            }

            if (position == DockPanePosition.Bottom)
            {
                var y = fromEnd ? remaining.Bottom - size : remaining.Y;
                rects[index] = new Rect(remaining.X, y, remaining.Width, size);
                remaining = new Rect(remaining.X, remaining.Y, remaining.Width, remaining.Height - size);
                continue;
            }

            if (position == DockPanePosition.Left)
            {
                rects[index] = new Rect(remaining.X, remaining.Y, size, remaining.Height);
                remaining = new Rect(remaining.X + size, remaining.Y, remaining.Width - size, remaining.Height);
                continue;
            }

            var x = fromEnd ? remaining.Right - size : remaining.X;
            rects[index] = new Rect(x, remaining.Y, size, remaining.Height);
            remaining = new Rect(remaining.X, remaining.Y, remaining.Width - size, remaining.Height);
        }
    }

    private void AssignCenters(Rect remaining, Rect[] rects)
    {
        var centerIndexes = new List<int>();
        for (var index = 0; index < _panes.Count; index++)
        {
            if (_panes[index].Position == DockPanePosition.Center)
            {
                centerIndexes.Add(index);
            }
        }

        if (remaining.IsEmpty || centerIndexes.Count == 0)
        {
            return;
        }

        if (centerIndexes.Count == 1)
        {
            rects[centerIndexes[0]] = remaining;
            return;
        }

        var baseHeight = remaining.Height / centerIndexes.Count;
        var remainder = remaining.Height % centerIndexes.Count;
        var y = remaining.Y;
        for (var i = 0; i < centerIndexes.Count; i++)
        {
            var height = baseHeight + (i < remainder ? 1 : 0);
            rects[centerIndexes[i]] = new Rect(remaining.X, y, remaining.Width, height);
            y += height;
        }
    }

    private static int HitTestPane(int x, int y, Rect[] paneRects)
    {
        for (var index = 0; index < paneRects.Length; index++)
        {
            if (paneRects[index].Contains(x, y))
            {
                return index;
            }
        }

        return -1;
    }

    private bool MoveSelection(int direction)
    {
        var next = ResolveNextEnabled(_selectedIndex, direction);
        return next.HasValue && SetSelectedIndex(next.Value);
    }

    private int? ResolveNextEnabled(int start, int direction)
    {
        if (_panes.Count == 0)
        {
            return null;
        }

        var index = start;
        for (var i = 0; i < _panes.Count; i++)
        {
            index += direction;
            if (index < 0)
            {
                index = _panes.Count - 1;
            }
            else if (index >= _panes.Count)
            {
                index = 0;
            }

            if (!_panes[index].IsDisabled)
            {
                return index;
            }
        }

        return null;
    }

    private bool SetSelectedIndex(int index)
    {
        if (index < 0 || index >= _panes.Count || _panes[index].IsDisabled || index == _selectedIndex)
        {
            return false;
        }

        var previous = _selectedIndex;
        var previousPane = SelectedPane;
        _selectedIndex = index;
        SelectionChanged?.Invoke(this,
            new ListSelectionChangedEventArgs<DockPane>(previous, _selectedIndex, previousPane, SelectedPane));
        return true;
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

    private TesseraStyle ResolveOuterBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TesseraStyle ResolvePaneBorderStyle(int index, DockPane pane)
    {
        var style = PaneBorderStyleText;
        if (index == _selectedIndex && IsFocused)
        {
            style = style.Merge(FocusedPaneBorderStyleText);
        }

        if (pane.IsDisabled || IsDisabled)
        {
            style = style.Merge(DisabledPaneStyle).Merge(DisabledStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return ApplyStyle(MeasureTitle(), IsDisabled ? style.Merge(DisabledStyle) : style);
    }

    private string MeasureTitle()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private string RenderPaneTitle(DockPane pane, bool selected)
    {
        var title = selected && !string.IsNullOrWhiteSpace(SelectedPaneMarker)
            ? $"{SelectedPaneMarker} {pane.Title}"
            : pane.Title;
        var style = PaneTitleStyle;
        if (selected)
        {
            style = style.Merge(SelectedPaneTitleStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedSelectedPaneTitleStyle);
            }
        }

        if (pane.IsDisabled || IsDisabled)
        {
            style = style.Merge(DisabledPaneStyle).Merge(DisabledStyle);
        }
        else if (pane.IsMuted)
        {
            style = style.Merge(MutedPaneStyle);
        }

        return ApplyStyle(title, style);
    }

    private void RenderPaneLines(Canvas canvas, DockPane pane, Rect content, bool selected, bool hovered)
    {
        var style = PaneBodyStyle;
        if (selected)
        {
            style = style.Merge(SelectedPaneBodyStyle);
        }
        else if (hovered)
        {
            style = style.Merge(HoveredPaneStyle);
        }

        if (pane.IsMuted)
        {
            style = style.Merge(MutedPaneStyle);
        }

        if (pane.IsDisabled || IsDisabled)
        {
            style = style.Merge(DisabledPaneStyle).Merge(DisabledStyle);
        }

        if (pane.Lines.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(PaneEmptyText, style), content.Width);
            return;
        }

        var rows = Math.Min(content.Height, pane.Lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(pane.Lines[row], style),
                content.Width);
        }
    }

    private static DockPane ClonePane(DockPane pane)
    {
        return new DockPane(pane.Id, pane.Title, pane.Position, pane.Size)
        {
            Content = pane.Content,
            Lines = pane.Lines.ToArray(),
            IsMuted = pane.IsMuted,
            IsDisabled = pane.IsDisabled
        };
    }

    private static string ApplyStyle(string value, TesseraStyle style)
    {
        return style.IsEmpty ? value : style.Render(value);
    }
}
