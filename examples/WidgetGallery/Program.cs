using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;
using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.ScreenOutput;

var model = new WidgetGalleryModel();
var options = new TeaProgramOptions
{
    UseConsoleKeyEvents = false,
};
var program = Tea.CreateProgram(model, options);
try
{
    await program.RunAsync();
    return 0;
}
catch (TeaProgramInterruptedException)
{
    return 130;
}

internal sealed record GalleryTickMsg(DateTimeOffset At) : IMessage;

internal sealed class WidgetGalleryModel : InteractiveScreenModel
{
    private static readonly ScreenRegionKey TabsRegionId = new("gallery.tabs");
    private static readonly ScreenRegionKey ButtonRegionId = new("gallery.button");
    private static readonly ScreenRegionKey ProgressRegionId = new("gallery.progress");
    private static readonly ScreenRegionKey TextInputRegionId = new("gallery.textInput");
    private static readonly ScreenRegionKey TextAreaRegionId = new("gallery.textArea");
    private static readonly ScreenRegionKey ListRegionId = new("gallery.list");
    private static readonly ScreenRegionKey TableRegionId = new("gallery.table");
    private static readonly ScreenRegionKey LogViewerRegionId = new("gallery.logs");
    private static readonly ScreenRegionKey DialogRegionId = new("gallery.dialog");
    private static readonly ScreenRegionKey LayoutRegionId = new("gallery.layout");
    private static readonly ScreenRegionKey BasicsLabelRegionId = new("gallery.basics.label");
    private static readonly ScreenRegionKey BasicsInfoRegionId = new("gallery.basics.info");
    private static readonly ScreenRegionKey OverlayPanelRegionId = new("gallery.overlay.panel");

    private readonly TabsComponent _tabs = new(new TabsOptions(["Basics", "Inputs", "Data", "Overlay", "Layout"]));
    private readonly TextBlockComponent _label = new(new TextBlockOptions(
        Title: "Label",
        Text: "Widget Gallery\n\nRead-only text.\nTitles, captions, help, and status lines."));
    private readonly ButtonComponent _button = new(new ButtonOptions(
        Label: "Deploy",
        Description: "click, enter, or space",
        Border: BorderStyle.SingleLine));
    private readonly TextInputComponent _textInput = new(new TextInputOptions(
        Title: "Text Input",
        Placeholder: "type and press enter",
        ClearOnSubmit: true));
    private readonly TextAreaComponent _textArea = new(new TextAreaOptions(
        Title: "Text Area",
        ShowLineNumbers: true,
        Wrap: true));
    private readonly ListComponent<string> _list = new(new ListOptions<string>(
        [
            "alpha", "beta", "gamma", "delta", "epsilon", "zeta", "eta", "theta", "iota", "kappa", "lambda", "mu"
        ],
        item => item,
        Title: "List"));
    private readonly TableComponent _table = new(new TableOptions(
        ["Service", "Status", "P95"],
        Title: "Table",
        PageSize: 4));
    private readonly ProgressBarComponent _progress = new(new ProgressBarOptions(
        Title: "Progress Bar",
        Step: 0.08));
    private readonly StatusBarComponent _status = new(new StatusBarOptions(
        Theme: new UiTheme(StatusFill: '·')));
    private readonly LogViewerComponent _logs = new()
    {
        Title = "Log Viewer",
    };
    private readonly DialogComponent _dialog = new(new DialogOptions(
        Title: "Confirm",
        BodyLines:
        [
            "Publish widget package?",
            "Enter/Space = accept",
            "Esc = cancel"
        ]));
    private readonly DialogWorkflow _dialogWorkflow;
    private readonly LayoutContainerComponent _layoutDemo = new(new LayoutContainerOptions(
        Mode: LayoutFlow.Grid,
        GridRows: 2,
        GridColumns: 2));

    private readonly TextBlockComponent _layoutCellA = new(new TextBlockOptions(Title: "Stack A", Text: "Vertical\nHorizontal\nGrid"));
    private readonly TextBlockComponent _layoutCellB = new(new TextBlockOptions(Title: "Stack B", Text: "Nested\nregions"));
    private readonly TextBlockComponent _layoutCellC = new(new TextBlockOptions(Title: "Stack C", Text: "Responsive\nby rect math"));
    private readonly TextBlockComponent _layoutCellD = new(new TextBlockOptions(Title: "Stack D", Text: "Children\ncomposed"));
    private int _width = 120;
    private int _height = 36;
    private int _tick;
    private string _lastEvent = "ready";
    private string? _pendingActionEvent;

    public WidgetGalleryModel()
    {
        ConfigureInputRouter();
        _dialogWorkflow = CreateDialogWorkflow(_dialog, DialogRegionId, CreateFocusChain(TabsRegionId));

        _table.SetRows(
        [
            ["api", "ok", "21ms"],
            ["worker", "ok", "18ms"],
            ["scheduler", "warn", "63ms"],
            ["gateway", "ok", "25ms"],
            ["events", "ok", "34ms"],
            ["billing", "degraded", "92ms"],
            ["search", "ok", "30ms"],
            ["cache", "ok", "15ms"],
        ]);

        _layoutDemo.Add(_layoutCellA);
        _layoutDemo.Add(_layoutCellB);
        _layoutDemo.Add(_layoutCellC);
        _layoutDemo.Add(_layoutCellD);

        _button.Pressed += (_, _) =>
        {
            _logs.Append("button:press");
            SetPendingActionEvent("button:press");
        };

        _textInput.Submitted += (_, args) => _logs.Append($"input:{args.Value}");

        _dialog.Accepted += (_, _) => HandleDialogDecision(DialogResult.Accepted);
        _dialog.Dismissed += (_, _) => HandleDialogDecision(DialogResult.Dismissed);

        _logs.Append("gallery booted");
        _logs.Append("tab to cycle focus");
        _logs.Append("1-5 switch tabs");
        _logs.Append("q quits");
    }

    public override Effect? Init() => NextTick();

    public override Effect? Update(IMessage message)
    {
        if (message is GalleryTickMsg tick)
        {
            _tick++;
            var value = ((_tick % 100) / 100.0);
            _progress.SetValue(value);
            if (_tick % 6 == 0)
            {
                _logs.Append($"{tick.At:HH:mm:ss} pulse={_tick:0000}");
            }

            _lastEvent = $"tick:{_tick}";
            return NextTick();
        }

        if (message is WindowSizeMsg ws)
        {
            _width = ws.Width;
            _height = ws.Height;
            _lastEvent = $"resize:{_width}x{_height}";
            return null;
        }

        if (message is MouseMsg mouse)
        {
            if (HandleMouse(mouse))
            {
                return null;
            }

            return null;
        }

        if (message is KeyPressMsg key)
        {
            return RouteKey(key);
        }

        return null;
    }

    public override ModelView Render()
    {
        if (_width < 60 || _height < 18)
        {
            return ModelView.From("Widget Gallery\n\nTerminal too small.\nExpand to at least 60x18.");
        }

        var canvas = new Canvas(_width, _height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        RenderScreen(canvas);

        var statusRect = new Rect(0, _height - 1, _width, 1);
        _status.LeftText = $"tab={_tabs.SelectedIndex + 1}:{_tabs.Tabs[_tabs.SelectedIndex]} focus={FocusLabel()}";
        _status.RightText = $"event={_lastEvent}";
        _status.Render(canvas, statusRect);

        return ModelView.From(canvas.Render()) with
        {
            Terminal = new TerminalOutput
            {
                AltScreen = true,
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                MouseMode = MouseMode.AllMotion,
                ForegroundColor = "#CDD6F4",
                BackgroundColor = "#1E1E2E",
                CursorColor = "#F5C2E7",
                WindowTitle = "Widget Gallery",
            },
        };
    }

    protected override Rect GetBodyRect()
    {
        return new Rect(0, 0, _width, _height - 1);
    }

    protected override ScreenRegionKey? PreferredFocusRegionKey
    {
        get
        {
            if (_dialog.IsVisible)
            {
                return DialogRegionId;
            }

            return _tabs.SelectedIndex switch
            {
                0 => FocusedRegionKey is { } key && (key == ButtonRegionId || key == ProgressRegionId) ? key : TabsRegionId,
                1 => FocusedRegionKey is { } key && (key == TextInputRegionId || key == TextAreaRegionId) ? key : TabsRegionId,
                2 => FocusedRegionKey is { } key && (key == ListRegionId || key == TableRegionId || key == LogViewerRegionId) ? key : TabsRegionId,
                3 => TabsRegionId,
                _ => TabsRegionId,
            };
        }
    }

    protected override bool CanBuildScreen => _width >= 60 && _height >= 18;

    protected override void ComposeScreen(Rect bodyRect)
    {
        var frame = Frame(bodyRect, headerHeight: 1);
        Screen.AddComponent(TabsRegionId, frame.Header, _tabs);

        switch (_tabs.SelectedIndex)
        {
            case 0:
                RegisterBasicsRegions(frame.Body);
                break;
            case 1:
                RegisterInputRegions(frame.Body);
                break;
            case 2:
                RegisterDataRegions(frame.Body);
                break;
            case 3:
                RegisterOverlayRegions(frame.Body);
                break;
            default:
                Screen.AddComponent(LayoutRegionId, frame.Body, _layoutDemo, focusable: false);
                break;
        }

        _dialogWorkflow.Compose(bodyRect);
    }

    private void RegisterBasicsRegions(Rect rect)
    {
        var (top, bottom) = Layout.SplitHorizontal(rect, Math.Max(8, rect.Height / 2));
        var (left, right) = Layout.SplitVertical(top, Math.Max(36, top.Width / 2));
        Screen.AddComponent(BasicsLabelRegionId, left, _label, focusable: false);
        Screen.AddComponent(ButtonRegionId, new Rect(right.X, right.Y, right.Width, 3), _button);
        Screen.AddComponent(ProgressRegionId, new Rect(right.X, right.Y + 4, right.Width, 4), _progress);
        Screen.AddRegion(
            BasicsInfoRegionId,
            bottom,
            (canvas, bounds) =>
            {
                var info = new TextBlockComponent
                {
                    Title = "Status",
                    Text =
                        $"button presses: {_button.PressCount}\n" +
                        $"input submits: {_textInput.SubmitCount}\n" +
                        "keys: tab focus, click/enter/space button, left/right progress, 1-5 tabs",
                };
                info.Render(canvas, bounds);
            });
    }

    private void RegisterInputRegions(Rect rect)
    {
        var shell = Form(rect, actionsHeight: 5);
        Screen.AddRegion(TextInputRegionId, shell.Actions, _textInput.Render, UpdateTextInputRegion, focusable: true);
        Screen.AddRegion(TextAreaRegionId, shell.Body, _textArea.Render, UpdateTextAreaRegion, focusable: true);
    }

    private void RegisterDataRegions(Rect rect)
    {
        var shell = Dashboard(rect, sidebarWidth: Math.Max(28, rect.Width / 3));
        Screen.AddComponent(ListRegionId, shell.Sidebar, _list);

        var (tableRect, logsRect) = Layout.SplitHorizontal(shell.Main, Math.Max(10, shell.Main.Height / 2));
        Screen.AddComponent(TableRegionId, tableRect, _table);
        Screen.AddComponent(LogViewerRegionId, logsRect, _logs);
    }

    private void RegisterOverlayRegions(Rect rect)
    {
        Screen.AddRegion(
            OverlayPanelRegionId,
            rect,
            (canvas, bounds) =>
            {
                var panel = new TextBlockComponent
                {
                    Title = "Modal / Dialog",
                    Text =
                        "Press d to toggle dialog.\n" +
                        "Enter/Space accepts. Esc dismisses.\n" +
                        $"last dialog result: {_dialog.LastResult}",
                };
                panel.Render(canvas, bounds);
            });
    }

    private void ConfigureInputRouter()
    {
        InputRouter
            .AddScope("gallery.system", InputScopeKind.System, static () => true, HandleSystemKey)
            .AddScope("gallery.modal", InputScopeKind.Modal, () => _dialog.IsVisible, HandleDialogKey, InputScopeBehavior.CaptureWhileActive)
            .AddScope(
                "gallery.focused",
                InputScopeKind.FocusedRegion,
                () => !_dialog.IsVisible && FocusedRegionKey is not null,
                HandleFocusedRegionKey,
                blocksGlobalShortcuts: ShouldBlockGlobalShortcuts)
            .AddScope("gallery.global", InputScopeKind.Global, static () => true, HandleGlobalKey);
    }

    private InputRouteResult HandleSystemKey(KeyPressMsg key)
    {
        if (key.Modifiers.HasFlag(KeyModifiers.Ctrl)
            && (key.IsCharacter('c') || key.IsCharacter('\u0003', ignoreCase: false)))
        {
            return InputRouteResult.FromEffect(Tea.Effects.Quit);
        }

        return InputRouteResult.NotHandled;
    }

    private InputRouteResult HandleDialogKey(KeyPressMsg key)
    {
        return HandleScreenKey(key);
    }

    private InputRouteResult HandleFocusedRegionKey(KeyPressMsg key)
    {
        return HandleScreenKey(key);
    }

    private InputRouteResult HandleGlobalKey(KeyPressMsg key)
    {
        if (key.IsCharacter('q', KeyModifiers.None))
        {
            return InputRouteResult.FromEffect(Tea.Effects.Quit);
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.None))
        {
            FocusNext();
            _lastEvent = $"focus:{FocusLabel()}";
            return InputRouteResult.HandledWithoutEffect;
        }

        if (key.IsCharacter('d', KeyModifiers.None) && _tabs.SelectedIndex == 3)
        {
            if (_dialog.IsVisible)
            {
                _dialogWorkflow.Hide();
            }
            else
            {
                _dialogWorkflow.Show();
            }

            _lastEvent = _dialog.IsVisible ? "dialog:open" : "dialog:close";
            _logs.Append(_lastEvent);
            return InputRouteResult.HandledWithoutEffect;
        }

        return InputRouteResult.NotHandled;
    }

    private bool ShouldBlockGlobalShortcuts(KeyPressMsg key)
    {
        return FocusedRegionKey is { } focusedKey
            && (focusedKey == TextInputRegionId || focusedKey == TextAreaRegionId)
            && key.Modifiers == KeyModifiers.None
            && key.Code == KeyCode.Character;
    }

    private InputRouteResult HandleScreenKey(KeyPressMsg key)
    {
        var changed = Screen.Update(NormalizeInputKey(key));
        if (!changed)
        {
            return InputRouteResult.NotHandled;
        }

        if (TryConsumePendingActionEvent(out var actionEvent))
        {
            _lastEvent = actionEvent;
            return InputRouteResult.HandledWithoutEffect;
        }

        _lastEvent = FocusedRegionKey == TabsRegionId
            ? $"tab:{_tabs.SelectedIndex + 1}"
            : key.Keystroke();
        return InputRouteResult.HandledWithoutEffect;
    }

    private static KeyPressMsg NormalizeInputKey(KeyPressMsg key)
    {
        if (key.Code == KeyCode.Character
            && (key.IsCharacter('\r', ignoreCase: false)
                || key.IsCharacter('\n', ignoreCase: false)
                || string.IsNullOrEmpty(key.Text)))
        {
            return new KeyPressMsg(KeyCode.Enter, string.Empty, key.Modifiers, key.IsRepeat);
        }

        return key;
    }

    private static bool IsEnterIntent(KeyPressMsg key)
    {
        return key.Is(KeyCode.Enter, key.Modifiers)
            || key.IsCharacter('\r', ignoreCase: false)
            || key.IsCharacter('\n', ignoreCase: false)
            || (key.Modifiers.HasFlag(KeyModifiers.Ctrl)
                && (key.IsCharacter('m')
                    || key.IsCharacter('j')))
            || (key.Code == KeyCode.Unknown && string.IsNullOrEmpty(key.Text));
    }

    private bool HandleMouse(MouseMsg mouse)
    {
        if (_width < 60 || _height < 18)
        {
            return false;
        }

        var changed = RouteMouse(mouse);
        if (!changed)
        {
            return false;
        }

        if (TryConsumePendingActionEvent(out var actionEvent))
        {
            _lastEvent = actionEvent;
            return true;
        }

        _lastEvent = FocusedRegionKey switch
        {
            var key when key == TabsRegionId => $"mouse:tab:{_tabs.SelectedIndex + 1}",
            var key when key == ButtonRegionId => "button:hover",
            var key when key == ListRegionId => "mouse:list",
            var key when key == TableRegionId => "mouse:table",
            var key when key == DialogRegionId => $"dialog:{_dialog.LastResult}",
            _ => $"mouse:{FocusLabel()}",
        };
        return true;
    }

    private string FocusLabel()
    {
        return FocusedRegionKey switch
        {
            var key when key == TabsRegionId => "tabs",
            var key when key == ButtonRegionId => "button",
            var key when key == ProgressRegionId => "progress",
            var key when key == TextInputRegionId => "text-input",
            var key when key == TextAreaRegionId => "text-area",
            var key when key == ListRegionId => "list",
            var key when key == TableRegionId => "table",
            var key when key == LogViewerRegionId => "logs",
            var key when key == DialogRegionId => "dialog",
            _ => "none",
        };
    }

    private bool UpdateTextInputRegion(IMessage message)
    {
        var normalized = message is KeyPressMsg key && IsEnterIntent(key)
            ? new KeyPressMsg(KeyCode.Enter)
            : message;
        return _textInput.Update(normalized);
    }

    private bool UpdateTextAreaRegion(IMessage message)
    {
        var normalized = message is KeyPressMsg key && IsEnterIntent(key)
            ? new KeyPressMsg(KeyCode.Enter)
            : message;
        return _textArea.Update(normalized);
    }

    private static Effect NextTick() => Tea.Effects.Every(TimeSpan.FromMilliseconds(250), at => new GalleryTickMsg(at));

    private void HandleDialogDecision(DialogResult result)
    {
        var resultText = result.ToString().ToLowerInvariant();
        _logs.Append($"dialog:{resultText}");
        SetPendingActionEvent($"dialog:{resultText}");
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
}
