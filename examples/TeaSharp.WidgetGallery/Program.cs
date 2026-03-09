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

internal enum GalleryFocus
{
    Tabs = 0,
    Button = 1,
    Progress = 2,
    TextInput = 3,
    TextArea = 4,
    List = 5,
    Table = 6,
    LogViewer = 7,
    Dialog = 8,
}

internal sealed class WidgetGalleryModel : IModel
{
    private readonly TabsComponent _tabs = new(["Basics", "Inputs", "Data", "Overlay", "Layout"]);
    private readonly LabelComponent _label = new()
    {
        Title = "Label",
        Text = "TeaSharp Widget Gallery\n\nRead-only text.\nTitles, captions, help, and status lines."
    };
    private readonly ButtonComponent _button = new()
    {
        Label = "Deploy",
        Description = "enter/space to trigger"
    };
    private readonly TextInputComponent _textInput = new()
    {
        Title = "Text Input",
        ClearOnSubmit = true,
    };
    private readonly TextAreaComponent _textArea = new()
    {
        Title = "Text Area",
        ShowLineNumbers = true,
        Wrap = true,
    };
    private readonly ListComponent<string> _list = new(
    [
        "alpha", "beta", "gamma", "delta", "epsilon", "zeta", "eta", "theta", "iota", "kappa", "lambda", "mu"
    ],
    item => item)
    {
        Title = "List"
    };
    private readonly TableComponent _table = new(["Service", "Status", "P95"])
    {
        Title = "Table"
    };
    private readonly ProgressBarComponent _progress = new()
    {
        Title = "Progress Bar",
        Step = 0.08,
    };
    private readonly StatusBarComponent _status = new()
    {
        Theme = new UiTheme(StatusFill: '·')
    };
    private readonly LogViewerComponent _logs = new()
    {
        Title = "Log Viewer",
    };
    private readonly DialogComponent _dialog = new()
    {
        Title = "Confirm",
        Lines =
        [
            "Publish widget package?",
            "Enter/Space = accept",
            "Esc = cancel"
        ],
    };
    private readonly LayoutContainerComponent _layoutDemo = new()
    {
        Mode = LayoutContainerMode.Grid,
        GridRows = 2,
        GridColumns = 2,
    };

    private readonly LabelComponent _layoutCellA = new() { DrawBorder = true, Title = "Stack A", Text = "Vertical\nHorizontal\nGrid" };
    private readonly LabelComponent _layoutCellB = new() { DrawBorder = true, Title = "Stack B", Text = "Nested\nregions" };
    private readonly LabelComponent _layoutCellC = new() { DrawBorder = true, Title = "Stack C", Text = "Responsive\nby rect math" };
    private readonly LabelComponent _layoutCellD = new() { DrawBorder = true, Title = "Stack D", Text = "Children\ncomposed" };

    private GalleryFocus _focus = GalleryFocus.Tabs;
    private int _width = 120;
    private int _height = 36;
    private int _tick;
    private string _lastEvent = "ready";

    public WidgetGalleryModel()
    {
        _textInput.Input.Placeholder = "type and press enter";
        _textArea.Input.SetValue(string.Empty);
        _table.Inner.PageSize = 4;

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

    public UpdateResult Update(IMessage message)
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
            return new UpdateResult(this, NextTick());
        }

        if (message is WindowSizeMsg ws)
        {
            _width = ws.Width;
            _height = ws.Height;
            _lastEvent = $"resize:{_width}x{_height}";
            return new UpdateResult(this, null);
        }

        if (message is MouseMsg mouse)
        {
            if (HandleMouse(mouse))
            {
                return new UpdateResult(this, null);
            }

            return new UpdateResult(this, null);
        }

        if (message is KeyPressMsg key)
        {
            if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && (key.IsCharacter('c') || key.IsCharacter('\u0003', ignoreCase: false)))
                || key.IsCharacter('q', KeyModifiers.None))
            {
                return new UpdateResult(this, Tea.Cmd.Quit);
            }

            if (_dialog.Visible)
            {
                _focus = GalleryFocus.Dialog;
                var dialogChanged = RouteFocusedInput(key);
                if (dialogChanged)
                {
                    _lastEvent = key.Keystroke();
                }

                return new UpdateResult(this, null);
            }

            if (key.Is(KeyCode.Tab, KeyModifiers.None))
            {
                CycleFocus();
                _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
                return new UpdateResult(this, null);
            }

            if (_focus == GalleryFocus.Tabs && TryHandleTabShortcut(key))
            {
                _focus = GalleryFocus.Tabs;
                _lastEvent = $"tab:{_tabs.SelectedIndex + 1}";
                return new UpdateResult(this, null);
            }

            if (key.IsCharacter('d', KeyModifiers.None) && _tabs.SelectedIndex == 3)
            {
                _dialog.Visible = !_dialog.Visible;
                _focus = _dialog.Visible ? GalleryFocus.Dialog : GalleryFocus.Tabs;
                _lastEvent = _dialog.Visible ? "dialog:open" : "dialog:close";
                _logs.Append(_lastEvent);
                return new UpdateResult(this, null);
            }

            if (_focus == GalleryFocus.Tabs)
            {
                if (_tabs.Update(key))
                {
                    _lastEvent = $"tab:{_tabs.SelectedIndex + 1}";
                }

                return new UpdateResult(this, null);
            }

            var changed = RouteFocusedInput(key);
            if (changed)
            {
                _lastEvent = key.Keystroke();
            }

            return new UpdateResult(this, null);
        }

        return new UpdateResult(this, null);
    }

    public ModelView View()
    {
        if (_width < 60 || _height < 18)
        {
            return ModelView.From("TeaSharp Widget Gallery\n\nTerminal too small.\nExpand to at least 60x18.");
        }

        ApplyFocusFlags();

        var canvas = new Canvas(_width, _height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var tabsRect = new Rect(0, 0, _width, 1);
        _tabs.Render(canvas, tabsRect);

        var statusRect = new Rect(0, _height - 1, _width, 1);
        _status.LeftText = $"tab={_tabs.SelectedIndex + 1}:{_tabs.Tabs[_tabs.SelectedIndex]} focus={_focus.ToString().ToLowerInvariant()}";
        _status.RightText = $"event={_lastEvent}";
        _status.Render(canvas, statusRect);

        var bodyRect = new Rect(0, 1, _width, _height - 2);
        switch (_tabs.SelectedIndex)
        {
            case 0:
                RenderBasics(canvas, bodyRect);
                break;
            case 1:
                RenderInputs(canvas, bodyRect);
                break;
            case 2:
                RenderData(canvas, bodyRect);
                break;
            case 3:
                RenderOverlay(canvas, bodyRect);
                break;
            default:
                RenderLayout(canvas, bodyRect);
                break;
        }

        _dialog.Render(canvas, bodyRect);

        return ModelView.From(canvas.Render()) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            MouseMode = MouseMode.AllMotion,
            ForegroundColor = "#CDD6F4",
            BackgroundColor = "#1E1E2E",
            CursorColor = "#F5C2E7",
            WindowTitle = "TeaSharp Widget Gallery",
        };
    }

    private void RenderBasics(Canvas canvas, Rect rect)
    {
        var (top, bottom) = Layout.SplitHorizontal(rect, Math.Max(8, rect.Height / 2));
        var (left, right) = Layout.SplitVertical(top, Math.Max(36, top.Width / 2));
        _label.Render(canvas, left);

        var buttonRect = new Rect(right.X, right.Y, right.Width, 3);
        _button.Render(canvas, buttonRect);

        var progressRect = new Rect(right.X, right.Y + 4, right.Width, 4);
        _progress.Render(canvas, progressRect);

        var info = new LabelComponent
        {
            Title = "Status",
            Text =
                $"button presses: {_button.PressCount}\n" +
                $"input submits: {_textInput.SubmitCount}\n" +
                "keys: tab focus, enter/space button, left/right progress, 1-5 tabs",
        };
        info.Render(canvas, bottom);
    }

    private void RenderInputs(Canvas canvas, Rect rect)
    {
        var (inputRect, areaRect) = Layout.SplitHorizontal(rect, 5, minFirst: 5, minSecond: 8);
        _textInput.Render(canvas, inputRect);
        _textArea.Render(canvas, areaRect);
    }

    private void RenderData(Canvas canvas, Rect rect)
    {
        var (left, right) = Layout.SplitVertical(rect, Math.Max(28, rect.Width / 3));
        _list.Render(canvas, left);

        var (tableRect, logsRect) = Layout.SplitHorizontal(right, Math.Max(10, right.Height / 2));
        _table.Render(canvas, tableRect);
        _logs.Render(canvas, logsRect);
    }

    private void RenderOverlay(Canvas canvas, Rect rect)
    {
        var panel = new LabelComponent
        {
            Title = "Modal / Dialog",
            Text =
                "Press d to toggle dialog.\n" +
                "Enter/Space accepts. Esc dismisses.\n" +
                $"last dialog result: {_dialog.LastResult}",
        };
        panel.Render(canvas, rect);
    }

    private void RenderLayout(Canvas canvas, Rect rect)
    {
        _layoutDemo.Render(canvas, rect);
    }

    private bool RouteFocusedInput(KeyPressMsg key)
    {
        var normalized = NormalizeInputKey(key);
        if (_dialog.Visible && _focus == GalleryFocus.Dialog)
        {
            var changed = _dialog.Update(normalized);
            if (changed)
            {
                _logs.Append($"dialog:{_dialog.LastResult}");
                if (!_dialog.Visible)
                {
                    _focus = GalleryFocus.Tabs;
                }
            }

            return changed;
        }

        if (_focus == GalleryFocus.TextInput)
        {
            if (IsEnterIntent(normalized))
            {
                return _textInput.Update(new KeyPressMsg(KeyCode.Enter));
            }

            return _textInput.Update(normalized);
        }

        if (_focus == GalleryFocus.TextArea)
        {
            if (IsEnterIntent(normalized))
            {
                return _textArea.Update(new KeyPressMsg(KeyCode.Enter));
            }

            return _textArea.Update(normalized);
        }

        return _focus switch
        {
            GalleryFocus.Button => _button.Update(normalized),
            GalleryFocus.Progress => _progress.Update(normalized),
            GalleryFocus.List => _list.Update(normalized),
            GalleryFocus.Table => _table.Update(normalized),
            GalleryFocus.LogViewer => _logs.Update(normalized),
            _ => false,
        };
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

    private bool TryHandleTabShortcut(KeyPressMsg key)
    {
        if (!key.TryGetDigit(out var oneBased))
        {
            return false;
        }

        if (oneBased < 1 || oneBased > _tabs.Tabs.Count)
        {
            return false;
        }

        _tabs.Select(oneBased - 1);
        return true;
    }

    private bool HandleMouse(MouseMsg mouse)
    {
        if (_width < 60 || _height < 18)
        {
            return false;
        }

        var changed = false;
        var tabsRect = new Rect(0, 0, _width, 1);
        if (_tabs.UpdateMouse(mouse, tabsRect))
        {
            _focus = GalleryFocus.Tabs;
            _lastEvent = $"mouse:tab:{_tabs.SelectedIndex + 1}";
            changed = true;
        }

        if (_tabs.SelectedIndex != 2)
        {
            return changed;
        }

        var bodyRect = new Rect(0, 1, _width, _height - 2);
        var (left, right) = Layout.SplitVertical(bodyRect, Math.Max(28, bodyRect.Width / 3));
        var (tableRect, logsRect) = Layout.SplitHorizontal(right, Math.Max(10, right.Height / 2));

        if (mouse is MouseClickMsg { Button: MouseButton.Left })
        {
            if (left.Contains(mouse.X, mouse.Y))
            {
                _focus = GalleryFocus.List;
                changed = true;
            }
            else if (tableRect.Contains(mouse.X, mouse.Y))
            {
                _focus = GalleryFocus.Table;
                changed = true;
            }
            else if (logsRect.Contains(mouse.X, mouse.Y))
            {
                _focus = GalleryFocus.LogViewer;
                changed = true;
            }
        }

        if (left.Contains(mouse.X, mouse.Y) || (mouse is MouseWheelMsg && _focus == GalleryFocus.List))
        {
            if (_list.UpdateMouse(mouse, left))
            {
                _lastEvent = "mouse:list";
                changed = true;
            }
        }

        if (tableRect.Contains(mouse.X, mouse.Y) || (mouse is MouseWheelMsg && _focus == GalleryFocus.Table))
        {
            if (_table.UpdateMouse(mouse, tableRect))
            {
                _lastEvent = "mouse:table";
                changed = true;
            }
        }

        return changed;
    }

    private void ApplyFocusFlags()
    {
        _button.Focused = _focus == GalleryFocus.Button;
        _progress.Focused = _focus == GalleryFocus.Progress;
        _textInput.Focused = _focus == GalleryFocus.TextInput;
        _textArea.Focused = _focus == GalleryFocus.TextArea;
        _list.Focused = _focus == GalleryFocus.List;
        _table.Focused = _focus == GalleryFocus.Table;
        _logs.Focused = _focus == GalleryFocus.LogViewer;
        _dialog.Focused = _focus == GalleryFocus.Dialog;
    }

    private void CycleFocus()
    {
        var ring = FocusRingForTab();
        var index = Array.IndexOf(ring, _focus);
        if (index < 0)
        {
            _focus = ring[0];
            return;
        }

        _focus = ring[(index + 1) % ring.Length];
    }

    private GalleryFocus[] FocusRingForTab()
    {
        return _tabs.SelectedIndex switch
        {
            0 => [GalleryFocus.Tabs, GalleryFocus.Button, GalleryFocus.Progress],
            1 => [GalleryFocus.Tabs, GalleryFocus.TextInput, GalleryFocus.TextArea],
            2 => [GalleryFocus.Tabs, GalleryFocus.List, GalleryFocus.Table, GalleryFocus.LogViewer],
            3 => _dialog.Visible
                ? [GalleryFocus.Dialog]
                : [GalleryFocus.Tabs],
            _ => [GalleryFocus.Tabs],
        };
    }

    private static Command NextTick() => Tea.Cmd.Every(TimeSpan.FromMilliseconds(250), at => new GalleryTickMsg(at));
}
