using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public enum LayoutContainerMode
{
    Vertical = 0,
    Horizontal = 1,
    Grid = 2,
}

public enum DialogResult
{
    None = 0,
    Accepted = 1,
    Dismissed = 2,
}

public sealed class LabelComponent : ICanvasComponent
{
    public string Text { get; set; } = string.Empty;

    public string? Title { get; set; }

    public bool DrawBorder { get; set; } = true;

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        if (DrawBorder)
        {
            canvas.DrawBox(clipped, Title ?? "Label");
            var content = clipped.Inset(1, 1);
            if (content.IsEmpty)
            {
                return;
            }

            DrawLines(canvas, content);
            return;
        }

        DrawLines(canvas, clipped);
    }

    private void DrawLines(Canvas canvas, Rect rect)
    {
        var lines = Text
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

        var rows = Math.Min(rect.Height, lines.Length);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(rect.X, rect.Y + row, lines[row], rect.Width);
        }
    }
}

public sealed class ButtonComponent : IStatefulComponent
{
    private static readonly KeyBinding ActivateKey = new("enter/space", "activate", "enter", "space");

    public string Label { get; set; } = "Button";

    public string? Description { get; set; }

    public bool Focused { get; set; }

    public bool Enabled { get; set; } = true;

    public int PressCount { get; private set; }

    public bool WasPressed { get; private set; }

    public bool Update(IMessage message)
    {
        WasPressed = false;
        if (!Enabled || !Focused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (!ActivateKey.Matches(key))
        {
            return false;
        }

        PressCount++;
        WasPressed = true;
        return true;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var prefix = Focused ? "›" : " ";
        var state = Enabled ? string.Empty : " (disabled)";
        var text = $"{prefix} [{Label}]{state}";
        canvas.WriteText(clipped.X, clipped.Y, text, clipped.Width);
        if (!string.IsNullOrWhiteSpace(Description) && clipped.Height > 1)
        {
            canvas.WriteText(clipped.X, clipped.Y + 1, Description!, clipped.Width);
        }
    }
}

public sealed class TextInputComponent : IStatefulComponent
{
    public TextInputModel Input { get; } = new();

    public TextInputKeyMap KeyMap { get; set; } = TextInputKeyMap.Default;

    public string Title { get; set; } = "Text Input";

    public bool Focused { get; set; }

    public bool ClearOnSubmit { get; set; }

    public string LastSubmittedValue { get; private set; } = string.Empty;

    public int SubmitCount { get; private set; }

    public bool Update(IMessage message)
    {
        var result = Input.Update(message, KeyMap);
        if (!result.Submitted)
        {
            return result.Changed;
        }

        LastSubmittedValue = Input.Value;
        SubmitCount++;
        if (ClearOnSubmit)
        {
            Input.Clear();
        }

        return true;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var frame = Input.BuildFrame(content.Width);
        canvas.WriteText(content.X, content.Y, frame.Text, content.Width);
        if (content.Height > 1)
        {
            var submitted = SubmitCount == 0
                ? "submit: -"
                : $"submit: {SubmitCount}";
            canvas.WriteText(content.X, content.Y + 1, submitted, content.Width);
        }
    }
}

public sealed class TextAreaComponent : IStatefulComponent
{
    private readonly ViewportModel _viewport = new();

    public TextAreaComponent()
    {
        Input.Multiline = true;
    }

    public TextInputModel Input { get; } = new() { Multiline = true };

    public TextInputKeyMap InputKeyMap { get; set; } = TextInputKeyMap.Default;

    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public string Title { get; set; } = "Text Area";

    public bool Focused { get; set; }

    public bool ShowLineNumbers
    {
        get => _viewport.ShowLineNumbers;
        set => _viewport.ShowLineNumbers = value;
    }

    public bool Wrap
    {
        get => _viewport.Wrap;
        set => _viewport.SetWrap(value);
    }

    public bool Update(IMessage message)
    {
        var changed = false;
        var update = Input.Update(message, InputKeyMap);
        if (update.Changed)
        {
            SyncViewport();
            changed = true;
        }

        if (_viewport.Update(message, ViewportKeyMap))
        {
            changed = true;
        }

        _viewport.HighlightVisualLine = CursorLineIndex();
        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        _viewport.Resize(content.Width, content.Height);
        _viewport.HighlightVisualLine = CursorLineIndex();
        SyncViewport();

        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, lines[row], content.Width);
        }
    }

    private void SyncViewport()
    {
        _viewport.SetContent(Input.Value);
    }

    private int CursorLineIndex()
    {
        if (Input.Cursor <= 0)
        {
            return 0;
        }

        var cursor = Math.Min(Input.Cursor, Input.Value.Length);
        var lines = 0;
        for (var i = 0; i < cursor; i++)
        {
            if (Input.Value[i] == '\n')
            {
                lines++;
            }
        }

        return lines;
    }
}

public sealed class ListComponent<T> : IStatefulComponent
{
    public ListComponent(IEnumerable<T> items, Func<T, string> toText)
    {
        Model = new ListModel<T>(items, toText);
    }

    public ListModel<T> Model { get; }

    public string Title { get; set; } = "List";

    public bool Focused { get; set; }

    public ListKeyMap KeyMap { get; set; } = ListKeyMap.Default;

    public bool Update(IMessage message)
    {
        return Model.Update(message, KeyMap);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        Model.PageSize = Math.Max(1, content.Height);
        var rows = Model.VisibleRows();
        for (var row = 0; row < rows.Count && row < content.Height; row++)
        {
            var visible = rows[row];
            var marker = visible.Selected ? "›" : " ";
            var text = $"{marker} {Model.LabelFor(visible.Item)}";
            canvas.WriteText(content.X, content.Y + row, text, content.Width);
        }
    }
}

public sealed class TableComponent : IStatefulComponent
{
    public TableComponent(IReadOnlyList<string> headers)
    {
        Inner = new SortableTableComponent(headers);
    }

    public SortableTableComponent Inner { get; }

    public bool Focused { get; set; }

    public string Title
    {
        get => Inner.Title;
        set => Inner.Title = value;
    }

    public void SetRows(IEnumerable<IReadOnlyList<string>> rows)
    {
        Inner.SetRows(rows);
    }

    public bool Update(IMessage message)
    {
        return Inner.Update(message);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var original = Inner.Title;
        Inner.Title = Focused ? $"{original} *" : original.Replace(" *", string.Empty, StringComparison.Ordinal);
        Inner.Render(canvas, rect);
        Inner.Title = original;
    }
}

public sealed class ProgressBarComponent : IStatefulComponent
{
    public double Value { get; private set; }

    public string Title { get; set; } = "Progress";

    public bool Focused { get; set; }

    public double Step { get; set; } = 0.05;

    public KeyBinding DecreaseKey { get; set; } = new("left/-", "decrease", "left", "-");

    public KeyBinding IncreaseKey { get; set; } = new("right/+", "increase", "right", "+");

    public bool Update(IMessage message)
    {
        if (message is not KeyPressMsg key || !Focused)
        {
            return false;
        }

        if (DecreaseKey.Matches(key))
        {
            SetValue(Value - Step);
            return true;
        }

        if (IncreaseKey.Matches(key))
        {
            SetValue(Value + Step);
            return true;
        }

        return false;
    }

    public void SetValue(double value)
    {
        Value = Math.Clamp(value, 0.0, 1.0);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var percent = (int)Math.Round(Value * 100, MidpointRounding.AwayFromZero);
        Widgets.DrawProgressBar(canvas, new Rect(content.X, content.Y, content.Width, 1), Value, $"{percent}%");
    }
}

public sealed class StatusBarComponent : ICanvasComponent
{
    public string LeftText { get; set; } = string.Empty;

    public string RightText { get; set; } = string.Empty;

    public UiTheme Theme { get; set; } = new();

    public void Render(Canvas canvas, Rect rect)
    {
        UiWidgets.DrawStatusBar(canvas, rect, LeftText, RightText, Theme);
    }
}

public sealed class LogViewerComponent : IStatefulComponent
{
    private readonly ViewportModel _viewport = new();
    private readonly List<string> _entries = [];

    public LogViewerComponent()
    {
        _viewport.SetWrap(false);
    }

    public string Title { get; set; } = "Logs";

    public bool Focused { get; set; }

    public bool AutoScroll { get; set; } = true;

    public bool Paused { get; private set; }

    public string Filter { get; private set; } = string.Empty;

    public ViewportKeyMap ViewportKeyMap { get; set; } = ViewportKeyMap.Default;

    public KeyBinding TogglePauseKey { get; set; } = new("p", "toggle pause", "p");

    public KeyBinding ClearKey { get; set; } = new("c", "clear", "c");

    public int Count => _entries.Count;

    public void Append(string line)
    {
        if (Paused)
        {
            return;
        }

        _entries.Add(line);
        RefreshViewport();
        if (AutoScroll)
        {
            _viewport.ScrollToBottom();
        }
    }

    public void Clear()
    {
        _entries.Clear();
        RefreshViewport();
    }

    public void SetFilter(string filter)
    {
        Filter = filter ?? string.Empty;
        RefreshViewport();
    }

    public bool Update(IMessage message)
    {
        if (message is KeyPressMsg key)
        {
            if (TogglePauseKey.Matches(key))
            {
                Paused = !Paused;
                return true;
            }

            if (ClearKey.Matches(key))
            {
                Clear();
                return true;
            }
        }

        return _viewport.Update(message, ViewportKeyMap);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Focused
            ? $"{Title} *"
            : Title;
        if (Paused)
        {
            title += " [paused]";
        }

        canvas.DrawBox(clipped, title);
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        _viewport.Resize(content.Width, content.Height);
        var lines = _viewport.RenderLines();
        var rows = Math.Min(content.Height, lines.Count);
        for (var row = 0; row < rows; row++)
        {
            canvas.WriteText(content.X, content.Y + row, lines[row], content.Width);
        }
    }

    private void RefreshViewport()
    {
        var visible = string.IsNullOrWhiteSpace(Filter)
            ? _entries
            : _entries.Where(line => line.Contains(Filter, StringComparison.OrdinalIgnoreCase)).ToList();
        _viewport.SetContent(string.Join("\n", visible));
    }
}

public sealed class DialogComponent : IStatefulComponent
{
    public string Title { get; set; } = "Dialog";

    public IReadOnlyList<string> Lines { get; set; } = ["Confirm?"];

    public bool Visible { get; set; }

    public bool Focused { get; set; }

    public BorderStyle BorderStyle { get; set; } = BorderStyle.Rounded;

    public UiTheme Theme { get; set; } = new();

    public DialogResult LastResult { get; private set; }

    public KeyBinding AcceptKey { get; set; } = new("enter/space", "accept", "enter", "space");

    public KeyBinding DismissKey { get; set; } = new("esc", "dismiss", "escape");

    public bool Update(IMessage message)
    {
        if (!Visible || !Focused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (DismissKey.Matches(key))
        {
            Visible = false;
            LastResult = DialogResult.Dismissed;
            return true;
        }

        if (AcceptKey.Matches(key))
        {
            Visible = false;
            LastResult = DialogResult.Accepted;
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!Visible)
        {
            return;
        }

        var modal = new ModalComponent
        {
            Visible = true,
            Title = Title,
            Lines = Lines,
            BorderStyle = BorderStyle,
            Theme = Theme,
        };
        modal.Render(canvas, rect);
    }
}

public sealed class LayoutContainerComponent : ICanvasComponent
{
    private readonly List<(ICanvasComponent Component, int Weight)> _children = [];

    public LayoutContainerMode Mode { get; set; } = LayoutContainerMode.Vertical;

    public int GridRows { get; set; } = 1;

    public int GridColumns { get; set; } = 1;

    public IReadOnlyList<(ICanvasComponent Component, int Weight)> Children => _children;

    public void Clear()
    {
        _children.Clear();
    }

    public void Add(ICanvasComponent component, int weight = 1)
    {
        _children.Add((component, Math.Max(1, weight)));
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (_children.Count == 0)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        switch (Mode)
        {
            case LayoutContainerMode.Horizontal:
                RenderHorizontal(canvas, clipped);
                break;
            case LayoutContainerMode.Grid:
                RenderGrid(canvas, clipped);
                break;
            default:
                RenderVertical(canvas, clipped);
                break;
        }
    }

    private void RenderVertical(Canvas canvas, Rect rect)
    {
        var totalWeight = _children.Sum(entry => entry.Weight);
        var y = rect.Y;
        var consumed = 0;
        for (var i = 0; i < _children.Count; i++)
        {
            var remainingHeight = rect.Height - consumed;
            if (remainingHeight <= 0)
            {
                break;
            }

            var planned = i == _children.Count - 1
                ? remainingHeight
                : Math.Max(1, (rect.Height * _children[i].Weight) / Math.Max(1, totalWeight));
            var h = Math.Min(remainingHeight, planned);
            var childRect = new Rect(rect.X, y, rect.Width, h);
            _children[i].Component.Render(canvas, childRect);
            y += h;
            consumed += h;
        }
    }

    private void RenderHorizontal(Canvas canvas, Rect rect)
    {
        var totalWeight = _children.Sum(entry => entry.Weight);
        var x = rect.X;
        var consumed = 0;
        for (var i = 0; i < _children.Count; i++)
        {
            var remainingWidth = rect.Width - consumed;
            if (remainingWidth <= 0)
            {
                break;
            }

            var planned = i == _children.Count - 1
                ? remainingWidth
                : Math.Max(1, (rect.Width * _children[i].Weight) / Math.Max(1, totalWeight));
            var w = Math.Min(remainingWidth, planned);
            var childRect = new Rect(x, rect.Y, w, rect.Height);
            _children[i].Component.Render(canvas, childRect);
            x += w;
            consumed += w;
        }
    }

    private void RenderGrid(Canvas canvas, Rect rect)
    {
        var rows = Math.Max(1, GridRows);
        var columns = Math.Max(1, GridColumns);
        var cells = Layout.Grid(rect, rows, columns);
        var count = Math.Min(cells.Length, _children.Count);
        for (var i = 0; i < count; i++)
        {
            _children[i].Component.Render(canvas, cells[i]);
        }
    }
}
