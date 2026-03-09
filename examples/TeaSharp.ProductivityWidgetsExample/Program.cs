using System.Text;
using TeaSharp;
using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using ModelView = TeaSharp.Core.Abstractions.View;

var terminal = new ConsoleTerminalAdapter();
var program = Tea.NewProgram(new ProductivityModel(), new ProgramOptions
{
    Terminal = terminal,
    UseConsoleKeyEvents = false,
});

try
{
    await program.RunAsync();
    return 0;
}
catch (TeaProgramInterruptedException)
{
    return 130;
}

internal enum ProductivityFocus
{
    Menu = 0,
    Number = 1,
    Date = 2,
    Time = 3,
    Markdown = 4,
}

internal sealed class ProductivityModel : IModel
{
    private readonly MenuBarComponent _menu = new()
    {
        Focused = true,
    };

    private readonly ContextMenuComponent _context = new()
    {
        Title = "Actions",
    };

    private readonly NumberInputComponent _number = new()
    {
        Title = "Estimate (hours)",
        Min = 0,
        Max = 100,
        Step = 0.5,
        Precision = 1,
    };

    private readonly DatePickerComponent _date = new()
    {
        Title = "Due Date",
    };

    private readonly TimePickerComponent _time = new()
    {
        Title = "Reminder Time",
        MinuteStep = 5,
        SecondStep = 15,
    };

    private readonly MarkdownViewerComponent _markdown = new()
    {
        Title = "Project Notes",
        ShowLineNumbers = true,
    };

    private readonly StatusBarComponent _status = new(new StatusBarOptions(
        Theme: new UiTheme(StatusFill: '·')));
    private ProductivityFocus _focus = ProductivityFocus.Menu;
    private int _width = 120;
    private int _height = 36;
    private string _lastEvent = "ready";

    public ProductivityModel()
    {
        _menu.SetItems(
        [
            new MenuBarItem("new", "New", 'n'),
            new MenuBarItem("save", "Save", 's'),
            new MenuBarItem("export", "Export", 'e'),
            new MenuBarItem("help", "Help", 'h'),
        ]);

        _context.SetItems(
        [
            new ContextMenuItem("insert.todo", "Insert TODO", [WidgetVisualState.Warning]),
            new ContextMenuItem("insert.done", "Insert DONE", [WidgetVisualState.Success]),
            new ContextMenuItem("insert.error", "Insert ISSUE", [WidgetVisualState.Error]),
        ]);

        _number.SetValue(4.0);
        _date.SetDate(new DateOnly(2026, 3, 8));
        _time.SetValue(new TimeOnly(9, 0, 0));
        _markdown.SetMarkdown(
            BuildScrollableMarkdown(
                "Sprint Plan",
                "Finalize v1.0",
                "Polish docs",
                "Validate examples",
                "```bash",
                "dotnet run --project examples/TeaSharp.ProductivityWidgetsExample/TeaSharp.ProductivityWidgetsExample.csproj",
                "```"));

        SetFocus(ProductivityFocus.Menu);
    }

    public Command? Init() => null;

    public Command? Update(IMessage message)
    {
        if (message is WindowSizeMsg ws)
        {
            _width = ws.Width;
            _height = ws.Height;
            _lastEvent = $"resize:{_width}x{_height}";
            return null;
        }

        if (message is MouseMsg mouse)
        {
            var layout = GetInteractionRects();
            var mouseChanged = false;
            if (_context.Visible)
            {
                var beforeContext = _context.LastExecutedItemId;
                mouseChanged |= _context.UpdateMouse(mouse, layout.ContentRect);
                if (!_context.Visible)
                {
                    _context.Focused = false;
                }

                if (!string.Equals(beforeContext, _context.LastExecutedItemId, StringComparison.Ordinal))
                {
                    ApplyContextAction(_context.LastExecutedItemId);
                    mouseChanged = true;
                }

                if (mouseChanged)
                {
                    _lastEvent = $"mouse:{mouse.EventType.ToString().ToLowerInvariant()}";
                }

                return null;
            }

            var mouseBeforeMenuActivation = _menu.ActivationVersion;
            if (layout.MenuRect.Contains(mouse.X, mouse.Y)
                || (mouse is MouseWheelMsg && _focus == ProductivityFocus.Menu))
            {
                mouseChanged |= _menu.UpdateMouse(mouse, layout.MenuRect);
                if (mouse is MouseClickMsg { Button: MouseButton.Left })
                {
                    SetFocus(ProductivityFocus.Menu);
                    mouseChanged = true;
                }
            }
            else if (layout.DateRect.Contains(mouse.X, mouse.Y)
                || (mouse is MouseWheelMsg && _focus == ProductivityFocus.Date))
            {
                mouseChanged |= _date.UpdateMouse(mouse, layout.DateRect);
                if (mouse is MouseClickMsg { Button: MouseButton.Left })
                {
                    SetFocus(ProductivityFocus.Date);
                    mouseChanged = true;
                }
            }
            else if (layout.TimeRect.Contains(mouse.X, mouse.Y)
                || (mouse is MouseWheelMsg && _focus == ProductivityFocus.Time))
            {
                mouseChanged |= _time.UpdateMouse(mouse, layout.TimeRect);
                if (mouse is MouseClickMsg { Button: MouseButton.Left })
                {
                    SetFocus(ProductivityFocus.Time);
                    mouseChanged = true;
                }
            }

            if (mouseBeforeMenuActivation != _menu.ActivationVersion)
            {
                ApplyMenuAction(_menu.LastActivatedItemId);
                mouseChanged = true;
            }

            if (mouseChanged)
            {
                _lastEvent = $"mouse:{mouse.EventType.ToString().ToLowerInvariant()}";
            }

            return null;
        }

        if (message is not KeyPressMsg key)
        {
            return null;
        }

        if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c'))
            || key.IsCharacter('q', KeyModifiers.None))
        {
            return Tea.Cmd.Quit;
        }

        if (_context.Visible)
        {
            _context.Focused = true;
            var before = _context.LastExecutedItemId;
            if (_context.Update(key))
            {
                if (!_context.Visible)
                {
                    _context.Focused = false;
                }

                _lastEvent = $"context:{key.Keystroke()}";
                if (!string.Equals(before, _context.LastExecutedItemId, StringComparison.Ordinal))
                {
                    ApplyContextAction(_context.LastExecutedItemId);
                }
            }

            return null;
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.None))
        {
            CycleFocus();
            _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
            return null;
        }

        if (key.IsCharacter('m', KeyModifiers.None))
        {
            _context.OpenAt(Math.Max(0, (_width / 2) - 12), Math.Max(2, (_height / 2) - 3));
            _context.Focused = true;
            _lastEvent = "context:open";
            return null;
        }

        var beforeMenuActivation = _menu.ActivationVersion;
        var changed = RouteFocusedInput(key);
        var menuActionHandled = false;
        if (beforeMenuActivation != _menu.ActivationVersion)
        {
            ApplyMenuAction(_menu.LastActivatedItemId);
            changed = true;
            menuActionHandled = true;
        }

        if (changed && !menuActionHandled)
        {
            _lastEvent = key.Keystroke();
        }

        return null;
    }

    public ModelView View()
    {
        var width = Math.Max(80, _width);
        var height = Math.Max(24, _height);

        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var frame = new Rect(0, 0, width, height);
        canvas.DrawBox(frame, "TeaSharp Productivity Widgets", BorderStyle.Rounded);
        var body = frame.Inset(1, 1);

        _menu.Render(canvas, new Rect(body.X, body.Y, body.Width, 1));

        var content = new Rect(body.X, body.Y + 1, body.Width, body.Height - 2);
        var (left, right) = Layout.SplitVertical(content, Math.Max(36, content.Width / 3), minFirst: 28, minSecond: 30);
        var (topLeft, bottomLeft) = Layout.SplitHorizontal(left, Math.Max(8, left.Height / 3), minFirst: 7, minSecond: 10);
        var (dateRect, timeRect) = Layout.SplitHorizontal(bottomLeft, Math.Max(10, bottomLeft.Height / 2), minFirst: 9, minSecond: 7);

        _number.Render(canvas, topLeft);
        _date.Render(canvas, dateRect);
        _time.Render(canvas, timeRect);
        _markdown.Render(canvas, right);

        _context.Render(canvas, content);

        _status.LeftText = $"focus={_focus.ToString().ToLowerInvariant()} value={_number.Value:0.0} due={_date.SelectedDate:yyyy-MM-dd} time={_time.Value:HH:mm:ss}";
        _status.RightText = $"event={_lastEvent}";
        _status.Render(canvas, new Rect(0, height - 1, width, 1));

        return ModelView.From(canvas.Render()) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            MouseMode = MouseMode.AllMotion,
            ForegroundColor = "#CDD6F4",
            BackgroundColor = "#1E1E2E",
            CursorColor = "#F5C2E7",
            WindowTitle = "TeaSharp Productivity Widgets Example",
        };
    }

    private bool RouteFocusedInput(KeyPressMsg key)
    {
        return _focus switch
        {
            ProductivityFocus.Menu => _menu.Update(key),
            ProductivityFocus.Number => _number.Update(key),
            ProductivityFocus.Date => _date.Update(key),
            ProductivityFocus.Time => _time.Update(key),
            ProductivityFocus.Markdown => _markdown.Update(key),
            _ => false,
        };
    }

    private void CycleFocus()
    {
        SetFocus(_focus switch
        {
            ProductivityFocus.Menu => ProductivityFocus.Number,
            ProductivityFocus.Number => ProductivityFocus.Date,
            ProductivityFocus.Date => ProductivityFocus.Time,
            ProductivityFocus.Time => ProductivityFocus.Markdown,
            _ => ProductivityFocus.Menu,
        });
    }

    private void SetFocus(ProductivityFocus focus)
    {
        _focus = focus;
        _menu.Focused = focus == ProductivityFocus.Menu;
        _number.Focused = focus == ProductivityFocus.Number;
        _date.Focused = focus == ProductivityFocus.Date;
        _time.Focused = focus == ProductivityFocus.Time;
        _markdown.Focused = focus == ProductivityFocus.Markdown;
        if (!_context.Visible)
        {
            _context.Focused = false;
        }
    }

    private void ApplyMenuAction(string? menuId)
    {
        if (string.IsNullOrWhiteSpace(menuId))
        {
            return;
        }

        switch (menuId)
        {
            case "new":
                _markdown.SetMarkdown(
                    BuildScrollableMarkdown(
                        "New Note",
                        "- item 1",
                        "- item 2",
                        "- item 3"));
                break;
            case "save":
                _markdown.SetMarkdown(
                    BuildScrollableMarkdown(
                        "Saved",
                        "Content snapshot captured.",
                        "Sync complete."));
                break;
            case "export":
                _markdown.SetMarkdown(
                    BuildScrollableMarkdown(
                        "Export",
                        "`notes.md` emitted.",
                        "Artifacts: notes.md, status.json"));
                break;
            case "help":
                _markdown.SetMarkdown(
                    BuildScrollableMarkdown(
                        "Help",
                        "tab: cycle focus",
                        "m: open context menu",
                        "q: quit",
                        "arrows/hjkl: adjust active control"));
                break;
        }

        _lastEvent = $"menu:{menuId}";
    }

    private void ApplyContextAction(string? actionId)
    {
        if (string.IsNullOrWhiteSpace(actionId))
        {
            return;
        }

        switch (actionId)
        {
            case "insert.todo":
                _markdown.SetMarkdown(
                    BuildScrollableMarkdown(
                        "TODO",
                        "- [ ] follow up",
                        "- [ ] verify regression",
                        "- [ ] update release notes"));
                break;
            case "insert.done":
                _markdown.SetMarkdown(
                    BuildScrollableMarkdown(
                        "DONE",
                        "- [x] task completed",
                        "- [x] status reviewed"));
                break;
            case "insert.error":
                _markdown.SetMarkdown(
                    BuildScrollableMarkdown(
                        "ISSUE",
                        "- blocker detected",
                        "- owner: unassigned"));
                break;
        }

        _lastEvent = $"context:{actionId}";
    }

    private static string BuildScrollableMarkdown(string title, params string[] lines)
    {
        var builder = new StringBuilder();
        builder.Append("# ").Append(title).Append('\n');
        foreach (var line in lines)
        {
            builder.Append(line).Append('\n');
        }

        builder.AppendLine();
        builder.AppendLine("## Activity Log");
        for (var i = 1; i <= 40; i++)
        {
            builder.Append("- log ").Append(i.ToString("00")).Append(": sample entry").Append('\n');
        }

        return builder.ToString();
    }

    private InteractionRects GetInteractionRects()
    {
        var width = Math.Max(80, _width);
        var height = Math.Max(24, _height);
        var frame = new Rect(0, 0, width, height);
        var body = frame.Inset(1, 1);
        var menuRect = new Rect(body.X, body.Y, body.Width, 1);
        var content = new Rect(body.X, body.Y + 1, body.Width, body.Height - 2);
        var (left, _) = Layout.SplitVertical(content, Math.Max(36, content.Width / 3), minFirst: 28, minSecond: 30);
        var (_, bottomLeft) = Layout.SplitHorizontal(left, Math.Max(8, left.Height / 3), minFirst: 7, minSecond: 10);
        var (dateRect, timeRect) = Layout.SplitHorizontal(bottomLeft, Math.Max(10, bottomLeft.Height / 2), minFirst: 9, minSecond: 7);
        return new InteractionRects(menuRect, content, dateRect, timeRect);
    }

    private readonly record struct InteractionRects(
        Rect MenuRect,
        Rect ContentRect,
        Rect DateRect,
        Rect TimeRect);
}
