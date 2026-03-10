using TeaSharp;
using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using ModelView = TeaSharp.Core.Abstractions.View;

var model = new WidgetGalleryModel();
var terminal = new ConsoleTerminalAdapter();
var capabilities = TerminalCapabilityDetector.Detect();
var options = new ProgramOptions
{
    UseConsoleKeyEvents = false,
    Terminal = terminal,
    TerminalCapabilities = capabilities,
};
var program = Tea.NewProgram(model, options);
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

internal sealed class WidgetGalleryModel : IModel
{
    private const string TabsRegionId = "gallery.tabs";
    private const string ButtonRegionId = "gallery.button";
    private const string ProgressRegionId = "gallery.progress";
    private const string TextInputRegionId = "gallery.textInput";
    private const string TextAreaRegionId = "gallery.textArea";
    private const string ListRegionId = "gallery.list";
    private const string TableRegionId = "gallery.table";
    private const string LogViewerRegionId = "gallery.logs";
    private const string DialogRegionId = "gallery.dialog";

    private readonly ScreenComposer _screen = new();
    private readonly TabsComponent _tabs = new(new TabsOptions(["Basics", "Inputs", "Data", "Overlay", "Layout"]));
    private readonly LabelComponent _label = new(new LabelOptions(
        Title: "Label",
        Text: "TeaSharp Widget Gallery\n\nRead-only text.\nTitles, captions, help, and status lines."));
    private readonly ButtonComponent _button = new(new ButtonOptions(
        Label: "Deploy",
        Description: "click, enter, or space",
        ShowBorder: true));
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
        Lines:
        [
            "Publish widget package?",
            "Enter/Space = accept",
            "Esc = cancel"
        ]));
    private readonly LayoutContainerComponent _layoutDemo = new(new LayoutContainerOptions(
        Mode: LayoutContainerMode.Grid,
        GridRows: 2,
        GridColumns: 2));

    private readonly LabelComponent _layoutCellA = new(new LabelOptions(Title: "Stack A", Text: "Vertical\nHorizontal\nGrid"));
    private readonly LabelComponent _layoutCellB = new(new LabelOptions(Title: "Stack B", Text: "Nested\nregions"));
    private readonly LabelComponent _layoutCellC = new(new LabelOptions(Title: "Stack C", Text: "Responsive\nby rect math"));
    private readonly LabelComponent _layoutCellD = new(new LabelOptions(Title: "Stack D", Text: "Children\ncomposed"));
    private int _width = 120;
    private int _height = 36;
    private int _tick;
    private string _lastEvent = "ready";

    public WidgetGalleryModel()
    {
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

        _logs.Append("gallery booted");
        _logs.Append("tab to cycle focus");
        _logs.Append("1-5 switch tabs");
        _logs.Append("q quits");
    }

    public Command? Init() => NextTick();

    public Command? Update(IMessage message)
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
            EnsureScreen();
            if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && (key.IsCharacter('c') || key.IsCharacter('\u0003', ignoreCase: false)))
                || key.IsCharacter('q', KeyModifiers.None))
            {
                return Tea.Cmd.Quit;
            }

            if (_dialog.Visible)
            {
                var dialogChanged = HandleScreenKey(key);
                if (dialogChanged)
                {
                    _lastEvent = key.Keystroke();
                }

                return null;
            }

            if (key.Is(KeyCode.Tab, KeyModifiers.None))
            {
                _screen.FocusNext();
                _lastEvent = $"focus:{FocusLabel()}";
                return null;
            }

            if (key.IsCharacter('d', KeyModifiers.None) && _tabs.SelectedIndex == 3)
            {
                _dialog.Visible = !_dialog.Visible;
                if (_dialog.Visible)
                {
                    _screen.SetFocus(DialogRegionId);
                }
                else
                {
                    _screen.SetFocus(TabsRegionId);
                }

                _lastEvent = _dialog.Visible ? "dialog:open" : "dialog:close";
                _logs.Append(_lastEvent);
                return null;
            }

            if (_screen.FocusedRegionId == TabsRegionId)
            {
                if (HandleScreenKey(key))
                {
                    _lastEvent = $"tab:{_tabs.SelectedIndex + 1}";
                }

                return null;
            }

            var changed = HandleScreenKey(key);
            if (changed)
            {
                _lastEvent = key.Keystroke();
            }

            return null;
        }

        return null;
    }

    public ModelView View()
    {
        if (_width < 60 || _height < 18)
        {
            return ModelView.From("TeaSharp Widget Gallery\n\nTerminal too small.\nExpand to at least 60x18.");
        }

        var canvas = new Canvas(_width, _height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var bodyRect = new Rect(0, 1, _width, _height - 2);
        BuildScreen(bodyRect);
        _screen.Render(canvas);

        var statusRect = new Rect(0, _height - 1, _width, 1);
        _status.LeftText = $"tab={_tabs.SelectedIndex + 1}:{_tabs.Tabs[_tabs.SelectedIndex]} focus={FocusLabel()}";
        _status.RightText = $"event={_lastEvent}";
        _status.Render(canvas, statusRect);

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
                WindowTitle = "TeaSharp Widget Gallery",
            },
        };
    }

    private void BuildScreen(Rect bodyRect)
    {
        _screen.BeginFrame();
        _screen.AddComponent(TabsRegionId, new Rect(0, 0, _width, 1), _tabs);

        switch (_tabs.SelectedIndex)
        {
            case 0:
                RegisterBasicsRegions(bodyRect);
                break;
            case 1:
                RegisterInputRegions(bodyRect);
                break;
            case 2:
                RegisterDataRegions(bodyRect);
                break;
            case 3:
                RegisterOverlayRegions(bodyRect);
                break;
            default:
                _screen.AddComponent("gallery.layout", bodyRect, _layoutDemo, focusable: false);
                break;
        }

        if (_dialog.Visible)
        {
            _screen.AddComponent(DialogRegionId, bodyRect, _dialog, layer: 100);
        }

        _screen.CompleteFrame(PreferredFocusRegionId());
    }

    private void RegisterBasicsRegions(Rect rect)
    {
        var (top, bottom) = Layout.SplitHorizontal(rect, Math.Max(8, rect.Height / 2));
        var (left, right) = Layout.SplitVertical(top, Math.Max(36, top.Width / 2));
        _screen.AddComponent("gallery.basics.label", left, _label, focusable: false);
        _screen.AddComponent(ButtonRegionId, new Rect(right.X, right.Y, right.Width, 3), _button);
        _screen.AddComponent(ProgressRegionId, new Rect(right.X, right.Y + 4, right.Width, 4), _progress);
        _screen.AddRegion(
            "gallery.basics.info",
            bottom,
            (canvas, bounds) =>
            {
                var info = new LabelComponent
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
        var (inputRect, areaRect) = Layout.SplitHorizontal(rect, 5, minFirst: 5, minSecond: 8);
        _screen.AddRegion(TextInputRegionId, inputRect, _textInput.Render, UpdateTextInputRegion, focusable: true);
        _screen.AddRegion(TextAreaRegionId, areaRect, _textArea.Render, UpdateTextAreaRegion, focusable: true);
    }

    private void RegisterDataRegions(Rect rect)
    {
        var (left, right) = Layout.SplitVertical(rect, Math.Max(28, rect.Width / 3));
        _screen.AddComponent(ListRegionId, left, _list);

        var (tableRect, logsRect) = Layout.SplitHorizontal(right, Math.Max(10, right.Height / 2));
        _screen.AddComponent(TableRegionId, tableRect, _table);
        _screen.AddComponent(LogViewerRegionId, logsRect, _logs);
    }

    private void RegisterOverlayRegions(Rect rect)
    {
        _screen.AddRegion(
            "gallery.overlay.panel",
            rect,
            (canvas, bounds) =>
            {
                var panel = new LabelComponent
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

    private bool HandleScreenKey(KeyPressMsg key)
    {
        var previousSubmitCount = _textInput.SubmitCount;
        var previousDialogResult = _dialog.LastResult;
        var changed = _screen.Update(NormalizeInputKey(key));
        if (!changed)
        {
            return false;
        }

        if (_screen.FocusedRegionId == DialogRegionId && previousDialogResult != _dialog.LastResult)
        {
            _logs.Append($"dialog:{_dialog.LastResult}");
            return true;
        }

        if (_screen.FocusedRegionId == TextInputRegionId && _textInput.SubmitCount > previousSubmitCount)
        {
            _logs.Append($"input:{_textInput.LastSubmittedValue}");
        }

        return true;
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

        EnsureScreen();
        var changed = _screen.Update(mouse);
        if (!changed)
        {
            return false;
        }

        _lastEvent = _screen.FocusedRegionId switch
        {
            TabsRegionId => $"mouse:tab:{_tabs.SelectedIndex + 1}",
            ButtonRegionId => _button.WasPressed ? "button:press" : "button:hover",
            ListRegionId => "mouse:list",
            TableRegionId => "mouse:table",
            DialogRegionId => $"dialog:{_dialog.LastResult}",
            _ => $"mouse:{FocusLabel()}",
        };
        return true;
    }

    private string FocusLabel()
    {
        return _screen.FocusedRegionId switch
        {
            TabsRegionId => "tabs",
            ButtonRegionId => "button",
            ProgressRegionId => "progress",
            TextInputRegionId => "text-input",
            TextAreaRegionId => "text-area",
            ListRegionId => "list",
            TableRegionId => "table",
            LogViewerRegionId => "logs",
            DialogRegionId => "dialog",
            _ => "none",
        };
    }

    private string? PreferredFocusRegionId()
    {
        if (_dialog.Visible)
        {
            return DialogRegionId;
        }

        return _tabs.SelectedIndex switch
        {
            0 => _screen.FocusedRegionId is ButtonRegionId or ProgressRegionId ? _screen.FocusedRegionId : TabsRegionId,
            1 => _screen.FocusedRegionId is TextInputRegionId or TextAreaRegionId ? _screen.FocusedRegionId : TabsRegionId,
            2 => _screen.FocusedRegionId is ListRegionId or TableRegionId or LogViewerRegionId ? _screen.FocusedRegionId : TabsRegionId,
            3 => TabsRegionId,
            _ => TabsRegionId,
        };
    }

    private void EnsureScreen()
    {
        if (_screen.Regions.Count == 0 && _width >= 60 && _height >= 18)
        {
            BuildScreen(new Rect(0, 1, _width, _height - 2));
        }
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

    private static Command NextTick() => Tea.Cmd.Every(TimeSpan.FromMilliseconds(250), at => new GalleryTickMsg(at));
}
