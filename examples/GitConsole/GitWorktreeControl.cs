using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.GitConsole;

internal sealed class GitWorktreeControl : Control
{
    private readonly List<GitWorktreeSection> _sections = [];
    private readonly List<GitFileEntry> _visibleItems = [];
    private int _selectedIndex;
    private int _scrollOffset;
    private int _lastViewportRows = 8;

    public event EventHandler<GitWorktreeSelectionChangedEventArgs>? SelectionChanged;

    public string Title { get; set; } = "Worktree";
    public string FocusMarker { get; set; } = "◆";
    public bool ShowFocusMarker { get; set; } = true;
    public BorderStyle Border { get; set; } = BorderStyle.Rounded;
    public Thickness Padding { get; set; } = Thickness.All(1);
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;
    public TesseraStyle GroupStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle DefaultRowStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle SelectedRowStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle FocusedSelectedRowStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle SecondaryStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle StagedStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle ReviewStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle AddedStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle RemovedStyle { get; set; } = TesseraStyle.Empty;
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;

    public GitFileEntry? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _visibleItems.Count ? _visibleItems[_selectedIndex] : null;

    public void SetSections(IEnumerable<GitWorktreeSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);
        var previous = SelectedItem;

        _sections.Clear();
        _sections.AddRange(sections.Where(static section => section.Items.Count > 0));

        _visibleItems.Clear();
        foreach (var section in _sections)
        {
            _visibleItems.AddRange(section.Items);
        }

        _selectedIndex = previous is null ? 0 : Math.Max(0, _visibleItems.FindIndex(item => string.Equals(item.Id, previous.Id, StringComparison.Ordinal)));
        if (_visibleItems.Count == 0)
        {
            _selectedIndex = -1;
            _scrollOffset = 0;
        }
    }

    public bool SelectById(string id)
    {
        var index = _visibleItems.FindIndex(item => string.Equals(item.Id, id, StringComparison.Ordinal));
        return SetSelectedIndex(index);
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || _visibleItems.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_visibleItems.Count - 1);
        }

        if (key.Is(Key.PageDown))
        {
            return SetSelectedIndex(Math.Min(_visibleItems.Count - 1, _selectedIndex + Math.Max(1, _lastViewportRows - 2)));
        }

        if (key.Is(Key.PageUp))
        {
            return SetSelectedIndex(Math.Max(0, _selectedIndex - Math.Max(1, _lastViewportRows - 2)));
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = ResolveContentRect(bounds);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1);
            }
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && content.Contains(pointer.X, pointer.Y))
        {
            RequestFocus();
            var index = HitTestIndex(pointer.Y, content);
            return index >= 0 && SetSelectedIndex(index);
        }

        return Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, RenderTitle(), Border, ResolveBorderStyle());
        var content = clipped.Inset(1, 1).Inset(Padding);
        if (content.IsEmpty)
        {
            return;
        }

        if (_visibleItems.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, Render(EmptyStyle, "No files in this lane."), content.Width);
            return;
        }

        var rows = BuildVisualRows();
        _lastViewportRows = Math.Max(1, content.Height);
        EnsureSelectionVisible(rows);

        for (var row = 0; row < content.Height; row++)
        {
            var index = _scrollOffset + row;
            if (index >= rows.Count)
            {
                break;
            }

            canvas.WriteText(content.X, content.Y + row, rows[index].Text, content.Width);
        }
    }

    private bool SetSelectedIndex(int index)
    {
        if (_visibleItems.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _visibleItems.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previous = SelectedItem;
        _selectedIndex = clamped;
        SelectionChanged?.Invoke(this, new GitWorktreeSelectionChangedEventArgs(previous, SelectedItem));
        return true;
    }

    private Rect ResolveContentRect(Rect bounds) => bounds.Inset(1, 1).Inset(Padding);

    private int HitTestIndex(int pointerY, Rect content)
    {
        var rows = BuildVisualRows();
        var row = pointerY - content.Y;
        if (row < 0 || row >= content.Height)
        {
            return -1;
        }

        var target = _scrollOffset + row;
        if (target < 0 || target >= rows.Count)
        {
            return -1;
        }

        return rows[target].ItemIndex;
    }

    private void EnsureSelectionVisible(IReadOnlyList<GitWorktreeVisualRow> rows)
    {
        var selectedRowIndex = -1;
        for (var index = 0; index < rows.Count; index++)
        {
            if (rows[index].ItemIndex == _selectedIndex)
            {
                selectedRowIndex = index;
                break;
            }
        }
        if (selectedRowIndex < 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (selectedRowIndex < _scrollOffset)
        {
            _scrollOffset = selectedRowIndex;
            return;
        }

        var bottom = _scrollOffset + _lastViewportRows - 1;
        if (selectedRowIndex > bottom)
        {
            _scrollOffset = selectedRowIndex - _lastViewportRows + 1;
        }
    }

    private List<GitWorktreeVisualRow> BuildVisualRows()
    {
        var rows = new List<GitWorktreeVisualRow>();
        var itemIndex = 0;
        foreach (var section in _sections)
        {
            rows.Add(new GitWorktreeVisualRow(-1, Render(GroupStyle, $"{section.Title.ToUpperInvariant()} [{section.Items.Count:00}]")));
            foreach (var item in section.Items)
            {
                var selected = itemIndex == _selectedIndex;
                rows.Add(new GitWorktreeVisualRow(itemIndex, RenderItemRow(item, selected)));
                itemIndex++;
            }
        }

        return rows;
    }

    private string RenderItemRow(GitFileEntry item, bool selected)
    {
        var marker = selected ? "▶" : " ";
        var path = item.Path.Length > 22 ? $"…{item.Path[^21..]}" : item.Path.PadRight(22);
        var status = item.Kind switch
        {
            GitChangeKind.Added => Render(AddedStyle, "A"),
            GitChangeKind.Deleted => Render(RemovedStyle, "D"),
            GitChangeKind.Renamed => Render(SecondaryStyle, "R"),
            _ => Render(SecondaryStyle, "M"),
        };
        var stage = item.IsStaged ? Render(StagedStyle, "STAGED") : Render(SecondaryStyle, "WORKTREE");
        var review = item.IsReviewCritical ? $" {Render(ReviewStyle, "HOT")}" : string.Empty;
        var delta = $"{Render(AddedStyle, $"+{item.AddedLines:00}")} {Render(RemovedStyle, $"-{item.RemovedLines:00}")}";
        var style = ResolveRowStyle(selected);
        return Render(style, $"{marker} {status} {path} {delta} {stage}{review}");
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker ? $"{Title} {FocusMarker}" : Title;
        return Render(IsFocused ? FocusedTitleStyle : TitleStyle, title);
    }

    private TesseraStyle ResolveBorderStyle() => IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;

    private TesseraStyle ResolveRowStyle(bool selected)
    {
        if (!selected)
        {
            return DefaultRowStyle;
        }

        return IsFocused ? SelectedRowStyle.Merge(FocusedSelectedRowStyle) : SelectedRowStyle;
    }

    private static string Render(TesseraStyle style, string text) => style.IsEmpty ? text : style.Render(text);

    private sealed record GitWorktreeVisualRow(int ItemIndex, string Text);
}

internal sealed class GitWorktreeSelectionChangedEventArgs(GitFileEntry? previousItem, GitFileEntry? selectedItem) : EventArgs
{
    public GitFileEntry? PreviousItem { get; } = previousItem;
    public GitFileEntry? SelectedItem { get; } = selectedItem;
}
