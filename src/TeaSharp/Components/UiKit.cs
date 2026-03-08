using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public enum ViewportClass
{
    Xs = 0,
    Sm = 1,
    Md = 2,
    Lg = 3,
    Xl = 4,
}

public enum ToastSeverity
{
    Info = 0,
    Success = 1,
    Warning = 2,
    Error = 3,
}

public readonly record struct TimelineEntry(string Time, string Text);

public readonly record struct TreeNode(string Label, int Depth, bool Selected = false);

public readonly record struct ToastMessage(string Text, int TtlTicks = 80, ToastSeverity Severity = ToastSeverity.Info);

public readonly record struct AccordionSection(string Title, IReadOnlyList<string> Lines, bool Expanded = false);

public readonly record struct UiTheme(
    char StatusFill = ' ',
    char SkeletonEvenFill = '░',
    char SkeletonOddFill = '▒',
    char ModalBackdropFill = '·');

public static class Layout
{
    public static ViewportClass Classify(int width)
    {
        return width switch
        {
            < 80 => ViewportClass.Xs,
            < 110 => ViewportClass.Sm,
            < 150 => ViewportClass.Md,
            < 190 => ViewportClass.Lg,
            _ => ViewportClass.Xl,
        };
    }

    public static (Rect First, Rect Second) SplitVertical(Rect rect, int firstWidth, int minFirst = 8, int minSecond = 8)
    {
        var clippedWidth = Math.Max(0, rect.Width);
        if (clippedWidth == 0)
        {
            return (new Rect(rect.X, rect.Y, 0, rect.Height), new Rect(rect.X, rect.Y, 0, rect.Height));
        }

        var safeMinFirst = Math.Clamp(minFirst, 0, clippedWidth);
        var maxSecond = Math.Max(0, clippedWidth - safeMinFirst);
        var safeMinSecond = Math.Clamp(minSecond, 0, maxSecond);
        var safeFirst = Math.Clamp(firstWidth, safeMinFirst, clippedWidth - safeMinSecond);
        var first = new Rect(rect.X, rect.Y, safeFirst, rect.Height);
        var second = new Rect(rect.X + safeFirst, rect.Y, Math.Max(0, rect.Width - safeFirst), rect.Height);
        return (first, second);
    }

    public static (Rect First, Rect Second) SplitHorizontal(Rect rect, int firstHeight, int minFirst = 4, int minSecond = 4)
    {
        var clippedHeight = Math.Max(0, rect.Height);
        if (clippedHeight == 0)
        {
            return (new Rect(rect.X, rect.Y, rect.Width, 0), new Rect(rect.X, rect.Y, rect.Width, 0));
        }

        var safeMinFirst = Math.Clamp(minFirst, 0, clippedHeight);
        var maxSecond = Math.Max(0, clippedHeight - safeMinFirst);
        var safeMinSecond = Math.Clamp(minSecond, 0, maxSecond);
        var safeFirst = Math.Clamp(firstHeight, safeMinFirst, clippedHeight - safeMinSecond);
        var first = new Rect(rect.X, rect.Y, rect.Width, safeFirst);
        var second = new Rect(rect.X, rect.Y + safeFirst, rect.Width, Math.Max(0, rect.Height - safeFirst));
        return (first, second);
    }

    public static Rect[] Grid(Rect rect, int rows, int columns)
    {
        var safeRows = Math.Max(1, rows);
        var safeCols = Math.Max(1, columns);
        var result = new Rect[safeRows * safeCols];
        var totalWidth = Math.Max(0, rect.Width);
        var totalHeight = Math.Max(0, rect.Height);

        var baseWidth = totalWidth / safeCols;
        var widthRemainder = totalWidth % safeCols;
        var baseHeight = totalHeight / safeRows;
        var heightRemainder = totalHeight % safeRows;

        var y = rect.Y;
        for (var row = 0; row < safeRows; row++)
        {
            var h = baseHeight + (row < heightRemainder ? 1 : 0);
            var x = rect.X;
            for (var col = 0; col < safeCols; col++)
            {
                var w = baseWidth + (col < widthRemainder ? 1 : 0);
                result[(row * safeCols) + col] = new Rect(x, y, w, h);
                x += w;
            }

            y += h;
        }

        return result;
    }
}

public static class UiWidgets
{
    public static void DrawBreadcrumb(Canvas canvas, Rect rect, IReadOnlyList<string> segments, string separator = " / ")
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var text = string.Join(separator, segments);
        canvas.WriteText(clipped.X, clipped.Y, text, clipped.Width);
    }

    public static void DrawStatusBar(Canvas canvas, Rect rect, string leftText, string rightText)
    {
        DrawStatusBar(canvas, rect, leftText, rightText, new UiTheme());
    }

    public static void DrawStatusBar(Canvas canvas, Rect rect, string leftText, string rightText, UiTheme theme)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var row = new string(theme.StatusFill, Math.Max(0, clipped.Width)).ToCharArray();
        CopyToRow(row, 0, leftText);
        var rightStart = Math.Max(0, clipped.Width - rightText.Length);
        CopyToRow(row, rightStart, rightText);
        canvas.WriteText(clipped.X, clipped.Y, new string(row), clipped.Width);
    }

    public static void DrawTimeline(Canvas canvas, Rect rect, IReadOnlyList<TimelineEntry> entries, string title = "Timeline")
    {
        canvas.DrawBox(rect, title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, entries.Count);
        for (var i = 0; i < rows; i++)
        {
            var entry = entries[i];
            var marker = i == rows - 1 ? "└" : "├";
            var line = $"{marker} {entry.Time} {entry.Text}";
            canvas.WriteText(content.X, content.Y + i, line, content.Width);
        }
    }

    public static void DrawTree(Canvas canvas, Rect rect, IReadOnlyList<TreeNode> nodes, string title = "Tree")
    {
        canvas.DrawBox(rect, title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, nodes.Count);
        for (var i = 0; i < rows; i++)
        {
            var node = nodes[i];
            var indent = new string(' ', Math.Max(0, node.Depth * 2));
            var prefix = node.Selected ? "› " : "  ";
            canvas.WriteText(content.X, content.Y + i, prefix + indent + node.Label, content.Width);
        }
    }

    public static void DrawCalendar(Canvas canvas, Rect rect, DateTime date, string title = "Calendar")
    {
        canvas.DrawBox(rect, title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty || content.Height < 3)
        {
            return;
        }

        var first = new DateTime(date.Year, date.Month, 1);
        var startOffset = ((int)first.DayOfWeek + 6) % 7;
        var days = DateTime.DaysInMonth(date.Year, date.Month);

        canvas.WriteText(content.X, content.Y, first.ToString("MMMM yyyy"), content.Width);
        canvas.WriteText(content.X, content.Y + 1, "Mo Tu We Th Fr Sa Su", content.Width);

        var day = 1;
        var row = 2;
        while (day <= days && row < content.Height)
        {
            var line = new char[Math.Min(content.Width, 20)];
            Array.Fill(line, ' ');
            for (var col = 0; col < 7 && day <= days; col++)
            {
                if (row == 2 && col < startOffset)
                {
                    continue;
                }

                var index = col * 3;
                if (index + 1 >= line.Length)
                {
                    break;
                }

                var text = day.ToString().PadLeft(2);
                line[index] = text[0];
                line[index + 1] = text[1];
                day++;
            }

            canvas.WriteText(content.X, content.Y + row, new string(line), content.Width);
            row++;
        }
    }

    public static void DrawSkeleton(Canvas canvas, Rect rect, string title = "Loading")
    {
        DrawSkeleton(canvas, rect, title, new UiTheme());
    }

    public static void DrawSkeleton(Canvas canvas, Rect rect, string title, UiTheme theme)
    {
        canvas.DrawBox(rect, title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        for (var row = 0; row < content.Height; row++)
        {
            var ch = row % 2 == 0 ? theme.SkeletonEvenFill : theme.SkeletonOddFill;
            canvas.DrawHorizontalLine(content.X, content.Y + row, content.Width, ch);
        }
    }

    private static void CopyToRow(char[] row, int start, string text)
    {
        if (row.Length == 0 || string.IsNullOrEmpty(text) || start >= row.Length)
        {
            return;
        }

        var index = Math.Max(0, start);
        for (var i = 0; i < text.Length && index < row.Length; i++, index++)
        {
            row[index] = text[i];
        }
    }
}

public sealed class TabsComponent : IStatefulComponent
{
    private readonly List<string> _tabs = [];

    public TabsComponent(IEnumerable<string> tabs)
    {
        _tabs.AddRange(tabs);
    }

    public int SelectedIndex { get; private set; }

    public IReadOnlyList<string> Tabs => _tabs;

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
            SelectedIndex = (SelectedIndex + 1) % _tabs.Count;
            return true;
        }

        if (PreviousTabKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + _tabs.Count - 1) % _tabs.Count;
            return true;
        }

        if (EnableNumericShortcuts
            && key.TryGetDigit(out var oneBased)
            && oneBased >= 1
            && oneBased <= _tabs.Count)
        {
            SelectedIndex = oneBased - 1;
            return true;
        }

        return false;
    }

    public void Select(int index)
    {
        if (_tabs.Count == 0)
        {
            SelectedIndex = 0;
            return;
        }

        SelectedIndex = Math.Clamp(index, 0, _tabs.Count - 1);
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
            canvas.WriteText(x, clipped.Y, label, clipped.Right - x);
            x += label.Length + 1;
        }
    }
}

public sealed class AccordionComponent : IStatefulComponent
{
    private readonly List<AccordionSection> _sections = [];

    public int SelectedIndex { get; private set; }

    public string Title { get; set; } = "Accordion";

    public KeyBinding NextSectionKey { get; set; } = new("down", "next section", "down");

    public KeyBinding PreviousSectionKey { get; set; } = new("up", "previous section", "up");

    public KeyBinding ToggleSectionKey { get; set; } = new("enter/space", "toggle section", "enter", "space");

    public void SetSections(IEnumerable<AccordionSection> sections)
    {
        _sections.Clear();
        _sections.AddRange(sections);
        if (SelectedIndex >= _sections.Count)
        {
            SelectedIndex = Math.Max(0, _sections.Count - 1);
        }
    }

    public bool Update(IMessage message)
    {
        if (_sections.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextSectionKey.Matches(key))
        {
            SelectedIndex = Math.Min(_sections.Count - 1, SelectedIndex + 1);
            return true;
        }

        if (PreviousSectionKey.Matches(key))
        {
            SelectedIndex = Math.Max(0, SelectedIndex - 1);
            return true;
        }

        if (ToggleSectionKey.Matches(key))
        {
            var section = _sections[SelectedIndex];
            _sections[SelectedIndex] = section with { Expanded = !section.Expanded };
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, Title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty || _sections.Count == 0)
        {
            return;
        }

        var row = 0;
        for (var i = 0; i < _sections.Count && row < content.Height; i++)
        {
            var section = _sections[i];
            var selected = i == SelectedIndex ? "›" : " ";
            var marker = section.Expanded ? "▾" : "▸";
            canvas.WriteText(content.X, content.Y + row, $"{selected} {marker} {section.Title}", content.Width);
            row++;

            if (section.Expanded)
            {
                for (var j = 0; j < section.Lines.Count && row < content.Height; j++)
                {
                    canvas.WriteText(content.X + 2, content.Y + row, section.Lines[j], Math.Max(0, content.Width - 2));
                    row++;
                }
            }
        }
    }
}

public sealed class ToastCenterComponent : IStatefulComponent
{
    private readonly List<ActiveToast> _toasts = [];

    public int MaxToasts { get; set; } = 3;

    public bool Update(IMessage message)
    {
        if (message is not TickMsg)
        {
            return false;
        }

        var changed = false;
        for (var i = _toasts.Count - 1; i >= 0; i--)
        {
            var toast = _toasts[i];
            toast.RemainingTicks--;
            if (toast.RemainingTicks <= 0)
            {
                _toasts.RemoveAt(i);
                changed = true;
                continue;
            }

            _toasts[i] = toast;
        }

        return changed;
    }

    public void Push(ToastMessage toast)
    {
        _toasts.Add(new ActiveToast(toast.Text, toast.TtlTicks, toast.Severity));
        while (_toasts.Count > Math.Max(1, MaxToasts))
        {
            _toasts.RemoveAt(0);
        }
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || _toasts.Count == 0)
        {
            return;
        }

        var visible = Math.Min(_toasts.Count, clipped.Height / 3);
        for (var i = 0; i < visible; i++)
        {
            var toast = _toasts[_toasts.Count - visible + i];
            var rowTop = clipped.Y + (i * 3);
            var boxRect = new Rect(clipped.X, rowTop, clipped.Width, 3);
            var label = toast.Severity switch
            {
                ToastSeverity.Success => "OK",
                ToastSeverity.Warning => "WARN",
                ToastSeverity.Error => "ERR",
                _ => "INFO",
            };
            canvas.DrawBox(boxRect, label, BorderStyle.Rounded);
            var body = boxRect.Inset(1, 1);
            if (!body.IsEmpty)
            {
                canvas.WriteText(body.X, body.Y, toast.Text, body.Width);
            }
        }
    }

    private struct ActiveToast(string text, int remainingTicks, ToastSeverity severity)
    {
        public string Text = text;
        public int RemainingTicks = remainingTicks;
        public ToastSeverity Severity = severity;
    }
}

public sealed class SortableTableComponent : IStatefulComponent
{
    private readonly List<IReadOnlyList<string>> _rows = [];

    public SortableTableComponent(IReadOnlyList<string> headers)
    {
        Headers = headers;
    }

    public IReadOnlyList<string> Headers { get; }

    public int SortColumn { get; private set; }

    public bool SortDescending { get; private set; }

    public int PageSize { get; set; } = 8;

    public int PageIndex { get; private set; }

    public string Title { get; set; } = "Table";

    public bool EnableVirtualization { get; set; }

    public int VirtualStartIndex { get; private set; }

    public int VirtualWindowSize { get; private set; } = 32;

    public KeyBinding NextPageKey { get; set; } = new("]", "next page", "]");

    public KeyBinding PreviousPageKey { get; set; } = new("[", "previous page", "[");

    public KeyBinding ToggleSortDirectionKey { get; set; } = new("s", "toggle sort direction", "s");

    public KeyBinding NextSortColumnKey { get; set; } = new("c", "next sort column", "c");

    public KeyBinding VirtualForwardKey { get; set; } = new("v", "virtual forward", "v");

    public KeyBinding VirtualBackwardKey { get; set; } = new("shift+v", "virtual backward", "V");

    public void SetRows(IEnumerable<IReadOnlyList<string>> rows)
    {
        _rows.Clear();
        _rows.AddRange(rows);
        NormalizePage();
    }

    public bool Update(IMessage message)
    {
        if (Headers.Count == 0 || _rows.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextPageKey.Matches(key))
        {
            PageIndex++;
            NormalizePage();
            return true;
        }

        if (PreviousPageKey.Matches(key))
        {
            PageIndex = Math.Max(0, PageIndex - 1);
            return true;
        }

        if (ToggleSortDirectionKey.Matches(key))
        {
            SortDescending = !SortDescending;
            return true;
        }

        if (NextSortColumnKey.Matches(key))
        {
            SortColumn = (SortColumn + 1) % Headers.Count;
            return true;
        }

        if (EnableVirtualization && VirtualForwardKey.Matches(key))
        {
            VirtualStartIndex = Math.Max(0, VirtualStartIndex + Math.Max(1, VirtualWindowSize / 2));
            return true;
        }

        if (EnableVirtualization && VirtualBackwardKey.Matches(key))
        {
            VirtualStartIndex = Math.Max(0, VirtualStartIndex - Math.Max(1, VirtualWindowSize / 2));
            return true;
        }

        return false;
    }

    public void SetVirtualWindow(int startIndex, int windowSize)
    {
        VirtualStartIndex = Math.Max(0, startIndex);
        VirtualWindowSize = Math.Max(1, windowSize);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var sorted = _rows
            .OrderBy(row => ValueAt(row, SortColumn), StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (SortDescending)
        {
            sorted.Reverse();
        }

        var safePageSize = Math.Max(1, PageSize);
        var pageCount = Math.Max(1, (sorted.Count + safePageSize - 1) / safePageSize);
        var page = Math.Clamp(PageIndex, 0, pageCount - 1);
        var offset = page * safePageSize;
        var visibleRows = sorted.Skip(offset).Take(safePageSize).ToList();
        if (EnableVirtualization)
        {
            var virtualOffset = Math.Clamp(VirtualStartIndex, 0, Math.Max(0, sorted.Count - 1));
            var safeWindow = Math.Max(1, VirtualWindowSize);
            visibleRows = sorted.Skip(virtualOffset).Take(safeWindow).ToList();
        }

        Widgets.DrawTable(
            canvas,
            rect,
            Headers,
            visibleRows,
            selectedRow: -1,
            title: EnableVirtualization
                ? $"{Title} v{VirtualStartIndex + 1}+{Math.Max(1, VirtualWindowSize)} sort:{Headers[Math.Min(SortColumn, Headers.Count - 1)]} {(SortDescending ? "desc" : "asc")}"
                : $"{Title} p{page + 1}/{pageCount} sort:{Headers[Math.Min(SortColumn, Headers.Count - 1)]} {(SortDescending ? "desc" : "asc")}");
    }

    private void NormalizePage()
    {
        var safePageSize = Math.Max(1, PageSize);
        var pageCount = Math.Max(1, (_rows.Count + safePageSize - 1) / safePageSize);
        PageIndex = Math.Clamp(PageIndex, 0, pageCount - 1);
    }

    private static string ValueAt(IReadOnlyList<string> row, int column)
    {
        if (column < 0 || column >= row.Count)
        {
            return string.Empty;
        }

        return row[column];
    }
}

public sealed class CheckboxListComponent : IStatefulComponent
{
    private readonly List<(string Label, bool Checked)> _items = [];

    public int SelectedIndex { get; private set; }

    public string Title { get; set; } = "Checklist";

    public KeyBinding NextItemKey { get; set; } = new("down", "next item", "down");

    public KeyBinding PreviousItemKey { get; set; } = new("up", "previous item", "up");

    public KeyBinding ToggleItemKey { get; set; } = new("enter/space", "toggle item", "enter", "space");

    public void SetItems(IEnumerable<(string Label, bool Checked)> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = Math.Max(0, _items.Count - 1);
        }
    }

    public IReadOnlyList<(string Label, bool Checked)> Items => _items;

    public bool Update(IMessage message)
    {
        if (_items.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            SelectedIndex = Math.Min(_items.Count - 1, SelectedIndex + 1);
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            SelectedIndex = Math.Max(0, SelectedIndex - 1);
            return true;
        }

        if (ToggleItemKey.Matches(key))
        {
            var item = _items[SelectedIndex];
            _items[SelectedIndex] = (item.Label, !item.Checked);
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, Title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        for (var row = 0; row < rows; row++)
        {
            var item = _items[row];
            var selected = row == SelectedIndex ? "›" : " ";
            var marker = item.Checked ? "[x]" : "[ ]";
            canvas.WriteText(content.X, content.Y + row, $"{selected} {marker} {item.Label}", content.Width);
        }
    }
}

public sealed class RadioGroupComponent : IStatefulComponent
{
    private readonly List<string> _items = [];

    public int SelectedIndex { get; private set; }

    public string Title { get; set; } = "Radio";

    public KeyBinding NextItemKey { get; set; } = new("down/right", "next item", "down", "right");

    public KeyBinding PreviousItemKey { get; set; } = new("up/left", "previous item", "up", "left");

    public void SetItems(IEnumerable<string> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = Math.Max(0, _items.Count - 1);
        }
    }

    public bool Update(IMessage message)
    {
        if (_items.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + 1) % _items.Count;
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + _items.Count - 1) % _items.Count;
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, Title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(content.Height, _items.Count);
        for (var row = 0; row < rows; row++)
        {
            var marker = row == SelectedIndex ? "(•)" : "( )";
            canvas.WriteText(content.X, content.Y + row, $"{marker} {_items[row]}", content.Width);
        }
    }
}

public sealed class SelectComponent : IStatefulComponent
{
    private readonly List<string> _items = [];

    public int SelectedIndex { get; private set; }

    public string Title { get; set; } = "Select";

    public KeyBinding NextItemKey { get; set; } = new("down/right", "next item", "down", "right");

    public KeyBinding PreviousItemKey { get; set; } = new("up/left", "previous item", "up", "left");

    public void SetItems(IEnumerable<string> items)
    {
        _items.Clear();
        _items.AddRange(items);
        if (SelectedIndex >= _items.Count)
        {
            SelectedIndex = Math.Max(0, _items.Count - 1);
        }
    }

    public bool Update(IMessage message)
    {
        if (_items.Count == 0 || message is not KeyPressMsg key)
        {
            return false;
        }

        if (NextItemKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + 1) % _items.Count;
            return true;
        }

        if (PreviousItemKey.Matches(key))
        {
            SelectedIndex = (SelectedIndex + _items.Count - 1) % _items.Count;
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, Title);
        var content = rect.Inset(1, 1);
        if (content.IsEmpty || _items.Count == 0)
        {
            return;
        }

        var selected = _items[SelectedIndex];
        canvas.WriteText(content.X, content.Y, $"< {selected} >", content.Width);
    }
}

public sealed class ModalComponent : ICanvasComponent
{
    public string Title { get; set; } = "Modal";

    public bool Visible { get; set; }

    public BorderStyle BorderStyle { get; set; } = BorderStyle.Rounded;

    public IReadOnlyList<string> Lines { get; set; } = ["(empty)"];

    public UiTheme Theme { get; set; } = new();

    public void Render(Canvas canvas, Rect rect)
    {
        if (!Visible)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        for (var y = clipped.Y; y < clipped.Bottom; y++)
        {
            for (var x = clipped.X; x < clipped.Right; x++)
            {
                if ((x + y) % 2 == 0 && canvas.Get(x, y) == ' ')
                {
                    canvas.Set(x, y, Theme.ModalBackdropFill);
                }
            }
        }

        if (clipped.Width < 4 || clipped.Height < 4)
        {
            return;
        }

        var modalWidth = Math.Clamp(clipped.Width * 3 / 5, 4, Math.Max(4, clipped.Width - 2));
        var modalHeight = Math.Clamp(clipped.Height / 2, 4, Math.Max(4, clipped.Height - 2));
        var modalX = clipped.X + (clipped.Width - modalWidth) / 2;
        var modalY = clipped.Y + (clipped.Height - modalHeight) / 2;
        var modal = new Rect(modalX, modalY, modalWidth, modalHeight);

        canvas.DrawBox(modal, Title, BorderStyle);
        var body = modal.Inset(1, 1);
        if (body.IsEmpty)
        {
            return;
        }

        var rows = Math.Min(body.Height, Lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(body.X, body.Y + row, Lines[row], body.Width);
        }
    }
}
