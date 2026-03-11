using System.Globalization;
using System.Text;
using TeaSharp;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.View;

var program = Tea.NewProgram(new ProductivityWidgetsModel(), new TeaProgramOptions
{
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

internal sealed class ProductivityWidgetsModel : InteractiveScreenModel
{
    private static readonly ScreenRegionKey MenuRegionId = new("productivity.menu");
    private static readonly ScreenRegionKey NumberRegionId = new("productivity.number");
    private static readonly ScreenRegionKey DateRegionId = new("productivity.date");
    private static readonly ScreenRegionKey TimeRegionId = new("productivity.time");
    private static readonly ScreenRegionKey MarkdownRegionId = new("productivity.markdown");
    private static readonly ScreenRegionKey ContextRegionId = new("productivity.context");

    private readonly MenuBarComponent _menu = new(new MenuBarOptions(Focused: true));
    private readonly ContextMenuComponent _context = new(new ContextMenuOptions(Title: "Actions"));
    private readonly NumberInputComponent _number = new(new NumberInputOptions(
        Title: "Estimate (hours)",
        Min: 0,
        Max: 100,
        Step: 0.5,
        Precision: 1));
    private readonly DatePickerComponent _date = new(new DatePickerOptions(Title: "Due Date"));
    private readonly TimePickerComponent _time = new(new TimePickerOptions(
        Title: "Reminder Time",
        MinuteStep: 5,
        SecondStep: 15));
    private readonly MarkdownViewerComponent _markdown = new(new MarkdownViewerOptions(
        Title: "Project Notes",
        ShowLineNumbers: true));
    private readonly StatusBarComponent _status = new(new StatusBarOptions(
        Theme: new UiTheme(StatusFill: '·')));
    private readonly ScreenFocusChain _focusChain;
    private ScreenFocusSnapshot _contextFocusSnapshot;
    private bool _pendingContextFocus;
    private int _width = 120;
    private int _height = 36;
    private string _lastEvent = "ready";
    private string? _pendingActionEvent;

    public ProductivityWidgetsModel()
    {
        _focusChain = CreateFocusChain(MenuRegionId, NumberRegionId, DateRegionId, TimeRegionId, MarkdownRegionId);
        ConfigureInputRouter();

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
                "dotnet run --project examples/ProductivityWidgets/ProductivityWidgets.csproj",
                "```"));

        _menu.ItemActivated += (_, args) =>
        {
            ApplyMenuAction(args.ItemId);
            SetPendingActionEvent($"menu:{args.ItemId}");
        };

        _context.ItemExecuted += (_, args) =>
        {
            ApplyContextAction(args.ItemId);
            SetPendingActionEvent($"context:{args.ItemId}");
        };
    }

    public override Command? Init() => null;

    public override Command? Update(IMessage message)
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
            var changed = RouteMouse(mouse);
            HandleContextLifecycle();
            if (changed)
            {
                _lastEvent = TryConsumePendingActionEvent(out var actionEvent)
                    ? actionEvent
                    : $"mouse:{FocusLabel()}";
            }

            return null;
        }

        if (message is KeyPressMsg key)
        {
            return RouteKey(key);
        }

        return null;
    }

    public override ModelView View()
    {
        if (_width < 80 || _height < 24)
        {
            return ModelView.From("Productivity Widgets\n\nTerminal too small.\nExpand to at least 80x24.");
        }

        var canvas = new Canvas(_width, _height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        RenderScreen(canvas);

        _status.LeftText = $"focus={FocusLabel()} value={_number.Value:0.0} due={_date.SelectedDate:yyyy-MM-dd} time={_time.Value:HH:mm:ss}";
        _status.RightText = $"event={_lastEvent}";
        _status.Render(canvas, new Rect(0, _height - 1, _width, 1));

        return ModelView.From(canvas.Render()) with
        {
            Terminal = new ViewTerminal
            {
                AltScreen = true,
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                MouseMode = MouseMode.AllMotion,
                ForegroundColor = "#CDD6F4",
                BackgroundColor = "#1E1E2E",
                CursorColor = "#F5C2E7",
                WindowTitle = "Productivity Widgets",
            },
        };
    }

    protected override Rect GetBodyRect() => new(0, 0, _width, _height - 1);

    protected override bool CanBuildScreen => _width >= 80 && _height >= 24;

    protected override ScreenRegionKey? PreferredFocusRegionKey =>
        _context.Visible
            ? ContextRegionId
            : FocusedRegionKey ?? MenuRegionId;

    protected override void ComposeScreen(Rect bodyRect)
    {
        var shell = Dashboard(bodyRect, sidebarWidth: Math.Max(36, bodyRect.Width / 3), headerHeight: 1);
        shell.AddHeader(MenuRegionId, _menu);

        var (numberRect, lowerLeftRect) = Layout.SplitHorizontal(shell.Sidebar, Math.Max(8, shell.Sidebar.Height / 3), minFirst: 7, minSecond: 10);
        var (dateRect, timeRect) = Layout.SplitHorizontal(lowerLeftRect, Math.Max(10, lowerLeftRect.Height / 2), minFirst: 9, minSecond: 7);

        Screen.AddComponent(NumberRegionId, numberRect, _number);
        Screen.AddComponent(DateRegionId, dateRect, _date);
        Screen.AddComponent(TimeRegionId, timeRect, _time);
        shell.AddMain(MarkdownRegionId, _markdown);

        if (_context.Visible)
        {
            Screen.AddPaletteComponent(ContextRegionId, shell.Frame.Body, _context);
            if (_pendingContextFocus)
            {
                Screen.SetFocus(ContextRegionId);
                _pendingContextFocus = false;
            }
        }
    }

    private void ConfigureInputRouter()
    {
        InputRouter
            .AddScope("productivity.system", InputScopeKind.System, static () => true, HandleSystemKey)
            .AddScope("productivity.palette", InputScopeKind.Palette, () => _context.Visible, HandleContextKey, InputScopeBehavior.CaptureWhileActive)
            .AddScope("productivity.focused", InputScopeKind.FocusedRegion, () => !_context.Visible && FocusedRegionKey is not null, HandleFocusedKey)
            .AddScope("productivity.global", InputScopeKind.Global, static () => true, HandleGlobalKey);
    }

    private InputRouteResult HandleSystemKey(KeyPressMsg key)
    {
        if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c'))
            || key.IsCharacter('q', KeyModifiers.None))
        {
            return InputRouteResult.FromCommand(Tea.Cmd.Quit);
        }

        return InputRouteResult.NotHandled;
    }

    private InputRouteResult HandleContextKey(KeyPressMsg key)
    {
        var changed = RouteFocusedMessage(key);
        HandleContextLifecycle();
        if (!changed)
        {
            return InputRouteResult.NotHandled;
        }

        _lastEvent = TryConsumePendingActionEvent(out var actionEvent)
            ? actionEvent
            : $"context:{key.Keystroke()}";
        return InputRouteResult.HandledWithoutCommand;
    }

    private InputRouteResult HandleFocusedKey(KeyPressMsg key)
    {
        var changed = RouteFocusedMessage(key);
        if (!changed)
        {
            return InputRouteResult.NotHandled;
        }

        _lastEvent = TryConsumePendingActionEvent(out var actionEvent)
            ? actionEvent
            : key.Keystroke();
        return InputRouteResult.HandledWithoutCommand;
    }

    private InputRouteResult HandleGlobalKey(KeyPressMsg key)
    {
        if (HandleTabNavigation(key, _focusChain))
        {
            _lastEvent = $"focus:{FocusLabel()}";
            return InputRouteResult.HandledWithoutCommand;
        }

        if (key.IsCharacter('m', KeyModifiers.None))
        {
            OpenContextMenu();
            return InputRouteResult.HandledWithoutCommand;
        }

        return InputRouteResult.NotHandled;
    }

    private void OpenContextMenu()
    {
        _contextFocusSnapshot = CaptureFocus();
        _context.OpenAt(Math.Max(0, (_width / 2) - 12), Math.Max(2, (_height / 2) - 3));
        _context.Focused = true;
        _pendingContextFocus = true;
        _lastEvent = "context:open";
    }

    private void HandleContextLifecycle()
    {
        if (_context.Visible)
        {
            return;
        }

        if (!_pendingContextFocus)
        {
            _context.Focused = false;
        }

        if (_contextFocusSnapshot.RegionKey is null)
        {
            return;
        }

        RestoreFocus(_contextFocusSnapshot, _focusChain);
        _contextFocusSnapshot = default;
    }

    private string FocusLabel()
    {
        return FocusedRegionKey switch
        {
            var key when key == MenuRegionId => "menu",
            var key when key == NumberRegionId => "number",
            var key when key == DateRegionId => "date",
            var key when key == TimeRegionId => "time",
            var key when key == MarkdownRegionId => "markdown",
            var key when key == ContextRegionId => "context",
            _ => "none",
        };
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
                _markdown.SetMarkdown(BuildScrollableMarkdown("New Note", "- item 1", "- item 2", "- item 3"));
                break;
            case "save":
                _markdown.SetMarkdown(BuildScrollableMarkdown("Saved", "Content snapshot captured.", "Sync complete."));
                break;
            case "export":
                _markdown.SetMarkdown(BuildScrollableMarkdown("Export", "`notes.md` emitted.", "Artifacts: notes.md, status.json"));
                break;
            case "help":
                _markdown.SetMarkdown(BuildScrollableMarkdown("Help", "tab: cycle focus", "m: open context menu", "q: quit", "arrows/hjkl: adjust active control"));
                break;
        }
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
                _markdown.SetMarkdown(BuildScrollableMarkdown("TODO", "- [ ] follow up", "- [ ] verify regression", "- [ ] update release notes"));
                break;
            case "insert.done":
                _markdown.SetMarkdown(BuildScrollableMarkdown("DONE", "- [x] task completed", "- [x] status reviewed"));
                break;
            case "insert.error":
                _markdown.SetMarkdown(BuildScrollableMarkdown("ISSUE", "- blocker detected", "- owner: unassigned"));
                break;
        }
    }

    private void SetPendingActionEvent(string value)
    {
        _pendingActionEvent = value;
        _lastEvent = value;
    }

    private bool TryConsumePendingActionEvent(out string value)
    {
        if (string.IsNullOrEmpty(_pendingActionEvent))
        {
            value = string.Empty;
            return false;
        }

        value = _pendingActionEvent;
        _pendingActionEvent = null;
        return true;
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
            builder.Append("- log ").Append(i.ToString("00", CultureInfo.InvariantCulture)).Append(": sample entry").Append('\n');
        }

        return builder.ToString();
    }
}
