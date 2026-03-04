using TeaSharp;
using TeaSharp.Components;
using TeaSharp.Styles;
using TeaSharp.Widgets;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using TWidgets = TeaSharp.Components.Widgets;
using ModelView = TeaSharp.Core.Abstractions.View;

var terminal = new TeaSharp.Core.Terminal.ConsoleTerminalAdapter();
var capabilities = TerminalCapabilityDetector.Detect();
var model = new CounterModel(terminal);
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
}
catch (TeaProgramInterruptedException)
{
    // graceful interrupt path
}

internal sealed class CounterModel : IModel
{
    private static readonly TeaStyle HeaderStyle = TeaStyle.Empty
        .WithBold()
        .WithForeground(AnsiColor.BrightWhite);

    private static readonly TeaStyle AccentStyle = TeaStyle.Empty
        .WithBold()
        .WithForeground(AnsiColor.BrightCyan);

    private static readonly TeaStyle MutedStyle = TeaStyle.Empty
        .WithForeground(AnsiColor.Indexed(245));

    private static readonly TeaStyle WarningStyle = TeaStyle.Empty
        .WithBold()
        .WithForeground(AnsiColor.Indexed(214));

    private readonly TeaSharp.Core.Terminal.ConsoleTerminalAdapter _terminal;
    private readonly string _resizeBackend;
    private readonly List<int> _sparkline = [];
    private readonly ViewportModel _logViewport = new();
    private readonly TextInputModel _commandInput = new()
    {
        Placeholder = "type command (help, inc, dec, stress on/off/toggle, filter <term>, clear)",
        MaxLength = 256,
    };
    private readonly List<ActionItem> _allActions =
    [
        new ActionItem("Increment count", "enter"),
        new ActionItem("Decrement count", "enter"),
        new ActionItem("Toggle stress", "enter"),
        new ActionItem("Reset count", "enter"),
        new ActionItem("Clear log", "enter"),
        new ActionItem("Switch to protocol", "enter"),
    ];

    private readonly ListModel<ActionItem> _actionList;
    private readonly ViewportKeyMap _viewportKeys = ViewportKeyMap.Default;
    private readonly TextInputKeyMap _inputKeys = TextInputKeyMap.Default;
    private readonly ListKeyMap _listKeys = ListKeyMap.Default;
    private readonly KeyBinding[] _globalHelp =
    [
        new KeyBinding("tab", "cycle focus"),
        new KeyBinding("?", "toggle help"),
        new KeyBinding("1/2", "switch page"),
        new KeyBinding("q", "quit"),
    ];
    private readonly List<string> _eventLog = [];

    private TerminalCapabilityProfile _capabilities = TerminalCapabilityProfile.AllSupported;
    private readonly Dictionary<int, ModeReportState> _modeReports = [];

    private int _count;
    private int _width = 80;
    private int _height = 24;
    private bool _focused = true;
    private string _lastEvent = "none";
    private string _lastPaste = "(none)";
    private string _typedText = string.Empty;
    private bool _stressMode;
    private int _tickCount;
    private bool _workspaceMode = true;
    private WorkspaceFocus _focus = WorkspaceFocus.Actions;
    private bool _showFullHelp;

    public CounterModel(TeaSharp.Core.Terminal.ConsoleTerminalAdapter terminal)
    {
        _terminal = terminal;
        _resizeBackend = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()
            ? "signal+poll"
            : "poll";

        _actionList = new ListModel<ActionItem>(_allActions, item => item.Name)
        {
            PageSize = 8,
        };

        _logViewport.Resize(48, 12);
        _logViewport.SetWrap(false);
        AppendLog("TeaSharp workspace initialized.");
        AppendLog("Use tab to move focus between actions, log, and command input.");
        AppendLog("Use ? to toggle full help and 1/2 to switch protocol/workspace.");
    }

    public Command? Init() => NextTickCommand(_stressMode);

    public UpdateResult Update(IMessage message)
    {
        if (message is KeyPressMsg key)
        {
            if (key.Text == "q"
                || ((key.Text == "c" || key.Text == "\u0003") && key.Modifiers.HasFlag(KeyModifiers.Ctrl)))
            {
                return new UpdateResult(this, Tea.Cmd.Quit);
            }

            if (key.Text == "1")
            {
                _workspaceMode = false;
                _lastEvent = "view: protocol";
                return new UpdateResult(this, null);
            }

            if (key.Text == "2")
            {
                _workspaceMode = true;
                _lastEvent = "view: workspace";
                return new UpdateResult(this, null);
            }

            if (key.Text == "?" && key.Modifiers == KeyModifiers.None && _workspaceMode)
            {
                _showFullHelp = !_showFullHelp;
                _lastEvent = $"help: {(_showFullHelp ? "full" : "compact")}";
                return new UpdateResult(this, null);
            }

            if (key.Code == KeyCode.Tab && _workspaceMode)
            {
                CycleFocus();
                _lastEvent = $"focus: {_focus.ToString().ToLowerInvariant()}";
                return new UpdateResult(this, null);
            }

            if (key.Text == "s" && key.Modifiers == KeyModifiers.None)
            {
                _stressMode = !_stressMode;
                _lastEvent = $"stress: {(_stressMode ? "on" : "off")}";
                AppendLog(_lastEvent);
                return new UpdateResult(this, NextTickCommand(_stressMode));
            }

            if (_workspaceMode)
            {
                return HandleWorkspaceKey(key);
            }

            return HandleProtocolKey(key);
        }

        if (message is PasteMsg paste)
        {
            _lastPaste = SanitizePreview(paste.Content);
            if (_workspaceMode && _focus == WorkspaceFocus.Command)
            {
                _commandInput.Update(paste, _inputKeys);
                _lastEvent = $"paste: {paste.Content.Length} chars into command";
            }
            else
            {
                _typedText += paste.Content;
                _lastEvent = $"paste: {paste.Content.Length} chars";
            }

            return new UpdateResult(this, null);
        }

        if (message is FocusInMsg)
        {
            _focused = true;
            _lastEvent = "focus: in";
            return new UpdateResult(this, null);
        }

        if (message is FocusOutMsg)
        {
            _focused = false;
            _lastEvent = "focus: out";
            return new UpdateResult(this, null);
        }

        if (message is WindowSizeMsg ws)
        {
            _width = ws.Width;
            _height = ws.Height;
            ApplyWidgetSizing();
            _lastEvent = $"resize: {_width}x{_height}";
            return new UpdateResult(this, null);
        }

        if (message is MouseMsg mouse)
        {
            var category = message switch
            {
                MouseClickMsg => "click",
                MouseReleaseMsg => "release",
                MouseMotionMsg => "motion",
                MouseWheelMsg => "wheel",
                _ => mouse.EventType.ToString().ToLowerInvariant(),
            };

            if (_workspaceMode && (_focus == WorkspaceFocus.Log || mouse is MouseWheelMsg))
            {
                _logViewport.Update(mouse, _viewportKeys);
            }

            _lastEvent = $"mouse: {category} {mouse.Button.ToString().ToLowerInvariant()} @ {mouse.X},{mouse.Y} mod={mouse.Modifiers}";
            return new UpdateResult(this, null);
        }

        if (message is ModeReportMsg modeReport)
        {
            _modeReports[modeReport.Mode] = modeReport.State;
            _lastEvent = $"mode-report: ?{modeReport.Mode}={modeReport.State.ToString().ToLowerInvariant()}";
            return new UpdateResult(this, null);
        }

        if (message is TerminalCapabilitiesMsg capabilities)
        {
            _capabilities = capabilities.Profile;
            _lastEvent = $"capabilities: {_capabilities.Source}";
            AppendLog(_lastEvent);
            return new UpdateResult(this, null);
        }

        if (message is DashboardTickMsg)
        {
            _tickCount++;
            AppendSparkSample();
            if (_stressMode && _tickCount % 80 == 0)
            {
                AppendLog($"pulse {_tickCount}: count={_count} focus={(_focused ? "in" : "out")}");
            }

            return new UpdateResult(this, NextTickCommand(_stressMode));
        }

        if (message is UnknownInputMsg unknown)
        {
            _lastEvent = $"unknown: {unknown.Raw}";
            return new UpdateResult(this, null);
        }

        return new UpdateResult(this, null);
    }

    public ModelView View()
    {
        int? cursorX = null;
        int? cursorY = null;

        var content = _workspaceMode
            ? BuildWorkspaceView(out cursorX, out cursorY)
            : BuildProbeView();

        return ModelView.From(content) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            EnableSynchronizedUpdates = true,
            MouseMode = MouseMode.AllMotion,
            CursorX = cursorX,
            CursorY = cursorY,
            WindowTitle = _workspaceMode
                ? "TeaSharp Widget Workspace"
                : "TeaSharp Protocol Probe",
        };
    }

    private UpdateResult HandleProtocolKey(KeyPressMsg key)
    {
        if (key.Code == KeyCode.Up)
        {
            _count++;
        }
        else if (key.Code == KeyCode.Down)
        {
            _count--;
        }
        else if (key.Modifiers == KeyModifiers.None && key.Code == KeyCode.Character && !string.IsNullOrEmpty(key.Text))
        {
            _typedText += key.Text;
        }
        else if (key.Code == KeyCode.Backspace && _typedText.Length > 0)
        {
            _typedText = _typedText[..^1];
        }
        else if (key.Code == KeyCode.Enter)
        {
            _typedText += "\n";
        }

        _lastEvent = $"key: {key.Keystroke()}";
        return new UpdateResult(this, null);
    }

    private UpdateResult HandleWorkspaceKey(KeyPressMsg key)
    {
        if (_focus == WorkspaceFocus.Actions)
        {
            if (key.Code == KeyCode.Enter)
            {
                ExecuteSelectedAction();
                _lastEvent = $"action: {_actionList.SelectedItem?.Name ?? "none"}";
                return new UpdateResult(this, null);
            }

            _actionList.Update(key, _listKeys);
            _lastEvent = $"key: {key.Keystroke()}";
            return new UpdateResult(this, null);
        }

        if (_focus == WorkspaceFocus.Log)
        {
            _logViewport.Update(key, _viewportKeys);
            _lastEvent = $"key: {key.Keystroke()}";
            return new UpdateResult(this, null);
        }

        var result = _commandInput.Update(key, _inputKeys);
        if (result.Submitted)
        {
            var command = _commandInput.Value.Trim();
            _commandInput.Clear();
            ExecuteCommand(command);
            return new UpdateResult(this, null);
        }

        _lastEvent = $"key: {key.Keystroke()}";
        return new UpdateResult(this, null);
    }

    private void ExecuteSelectedAction()
    {
        var selected = _actionList.SelectedItem;
        if (selected is null)
        {
            return;
        }

        switch (selected.Name)
        {
            case "Increment count":
                _count++;
                AppendLog("action: increment count");
                break;
            case "Decrement count":
                _count--;
                AppendLog("action: decrement count");
                break;
            case "Toggle stress":
                _stressMode = !_stressMode;
                AppendLog($"action: stress {(_stressMode ? "on" : "off")}");
                break;
            case "Reset count":
                _count = 0;
                AppendLog("action: reset count");
                break;
            case "Clear log":
                _eventLog.Clear();
                AppendLog("action: clear log");
                break;
            case "Switch to protocol":
                _workspaceMode = false;
                AppendLog("action: switch to protocol");
                break;
        }
    }

    private void ExecuteCommand(string command)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            AppendLog("cmd: (empty)");
            return;
        }

        AppendLog($"cmd> {command}");

        if (string.Equals(command, "help", StringComparison.OrdinalIgnoreCase))
        {
            AppendLog("commands: help | inc | dec | stress on/off/toggle | filter <term> | clear");
            return;
        }

        if (string.Equals(command, "inc", StringComparison.OrdinalIgnoreCase))
        {
            _count++;
            AppendLog($"count={_count}");
            return;
        }

        if (string.Equals(command, "dec", StringComparison.OrdinalIgnoreCase))
        {
            _count--;
            AppendLog($"count={_count}");
            return;
        }

        if (command.StartsWith("stress ", StringComparison.OrdinalIgnoreCase))
        {
            var arg = command[7..].Trim();
            if (string.Equals(arg, "on", StringComparison.OrdinalIgnoreCase))
            {
                _stressMode = true;
                AppendLog("stress=on");
            }
            else if (string.Equals(arg, "off", StringComparison.OrdinalIgnoreCase))
            {
                _stressMode = false;
                AppendLog("stress=off");
            }
            else
            {
                _stressMode = !_stressMode;
                AppendLog($"stress toggled => {(_stressMode ? "on" : "off")}");
            }

            return;
        }

        if (string.Equals(command, "clear", StringComparison.OrdinalIgnoreCase))
        {
            _eventLog.Clear();
            _logViewport.Clear();
            AppendLog("log cleared");
            return;
        }

        if (command.StartsWith("filter", StringComparison.OrdinalIgnoreCase))
        {
            var term = command.Length > 6
                ? command[6..].Trim()
                : string.Empty;
            _actionList.SetFilter(term);
            AppendLog(term.Length == 0
                ? "action filter reset"
                : $"action filter='{term}'");
            return;
        }

        AppendLog($"unknown command: {command}");
    }

    private void AppendLog(string line)
    {
        var entry = $"[{DateTimeOffset.Now:HH:mm:ss}] {line}";
        _eventLog.Add(entry);
        _logViewport.AppendLine(entry);
        if (_eventLog.Count > 240)
        {
            var removeCount = _eventLog.Count - 240;
            _eventLog.RemoveRange(0, removeCount);
            _logViewport.SetContent(string.Join('\n', _eventLog));
        }

        _logViewport.ScrollToBottom();
    }

    private string BuildProbeView()
    {
        static string Label(string text) => HeaderStyle.WithForeground(AnsiColor.BrightCyan).Render(text);

        return
            $"{HeaderStyle.Render("TeaSharp Protocol Probe")}\n\n" +
            $"{Label("Count:")} {_count}\n" +
            $"{Label("Focus:")} {(_focused ? "in" : "out")}\n" +
            $"{Label("Size:")} {_width}x{_height}\n" +
            $"{Label("Raw mode active:")} {(_terminal.IsRawModeActive ? "yes" : "no")}\n" +
            $"{Label("Raw mode probe:")} {SummarizeProbe(_terminal.RawModeDiagnostics)}\n" +
            $"{Label("Raw mode error:")} {SummarizeProbe(_terminal.RawModeError)}\n" +
            $"{Label("Input backend:")} {(_terminal.IsRawModeActive ? "vt-bytes" : "console-keys-fallback")}\n" +
            $"{Label("Capabilities source:")} {_capabilities.Source}\n" +
            $"{Label("Capabilities:")} focus={ToYesNo(_capabilities.FocusReporting)} mouse={ToYesNo(_capabilities.MouseReporting)} paste={ToYesNo(_capabilities.BracketedPaste)} sync={ToYesNo(_capabilities.SynchronizedUpdates)} decrpm={ToYesNo(_capabilities.ModeReports)}\n" +
            $"{Label("Mode reports (DECRPM current-state):")}\n" +
            $"{FormatModeReports()}\n" +
            $"{Label("Resize backend:")} {_resizeBackend}\n" +
            $"{Label("Stress mode:")} {(_stressMode ? "on" : "off")} (ticks: {_tickCount})\n" +
            $"{Label("Last event:")} {_lastEvent}\n" +
            $"{Label("Last paste:")} {_lastPaste}\n" +
            $"{Label("Typed length:")} {_typedText.Length}\n" +
            $"{Label("Typed text:")} {SanitizePreview(_typedText)}\n\n" +
            $"{MutedStyle.Render("Try live:")}\n" +
            "- press 2 to open widget workspace\n" +
            "- up/down to change count\n" +
            "- move/click mouse in terminal window\n" +
            "- press s to toggle render stress mode\n" +
            "- type text; backspace and enter work\n" +
            "- paste multi-line text\n" +
            "- switch terminal focus away/back\n" +
            "- resize terminal window\n" +
            "- q or ctrl+c to quit\n";
    }

    private string BuildWorkspaceView(out int? cursorX, out int? cursorY)
    {
        cursorX = null;
        cursorY = null;

        if (_width < 52 || _height < 18)
        {
            var compact =
                "TeaSharp Widget Workspace\n\n" +
                "Terminal too small for widget workspace.\n" +
                "Resize to at least 52x18.\n\n" +
                "Press 1 for protocol view or q to quit.";
            return WarningStyle.Render(compact);
        }

        ApplyWidgetSizing();

        var canvas = new Canvas(_width, _height);
        const int headerHeight = 3;
        const int footerHeight = 5;
        var bodyTop = headerHeight;
        var bodyHeight = _height - headerHeight - footerHeight;
        var leftWidth = Math.Max(30, _width / 3);
        var rightWidth = _width - leftWidth;

        var headerMode = _workspaceMode ? "workspace" : "protocol";
        TWidgets.DrawPanel(
            canvas,
            new Rect(0, 0, _width, headerHeight),
            "TeaSharp Workspace",
            [
                $"count={_count} focus={(_focused ? "in" : "out")} size={_width}x{_height} mode={headerMode} source={_capabilities.Source}",
            ]);

        var leftRect = new Rect(0, bodyTop, leftWidth, bodyHeight);
        var rightRect = new Rect(leftWidth, bodyTop, rightWidth, bodyHeight);

        var visibleRows = _actionList.VisibleRows();
        var tableRows = new List<IReadOnlyList<string>>(visibleRows.Count);
        var selectedPageRow = -1;
        for (var i = 0; i < visibleRows.Count; i++)
        {
            var row = visibleRows[i];
            if (row.Selected)
            {
                selectedPageRow = i;
            }

            tableRows.Add(
            [
                row.Item.Name,
                row.Item.Shortcut,
                ActionState(row.Item),
            ]);
        }

        TWidgets.DrawTable(
            canvas,
            leftRect,
            ["Action", "Key", "State"],
            tableRows,
            selectedPageRow,
            _focus == WorkspaceFocus.Actions ? "Actions *" : "Actions");

        Rect throughputRect;
        Rect logRect;
        if (rightRect.Height >= 10)
        {
            throughputRect = new Rect(rightRect.X, rightRect.Y, rightRect.Width, 3);
            logRect = new Rect(rightRect.X, rightRect.Y + 3, rightRect.Width, rightRect.Height - 3);
            TWidgets.DrawPanel(canvas, throughputRect, "Throughput", [string.Empty]);
            TWidgets.DrawSparkline(
                canvas,
                new Rect(throughputRect.X + 2, throughputRect.Y + 1, Math.Max(8, throughputRect.Width - 4), 1),
                _sparkline,
                minValue: 0,
                maxValue: 100);
        }
        else
        {
            throughputRect = new Rect(0, 0, 0, 0);
            logRect = rightRect;
        }

        _logViewport.Resize(Math.Max(8, logRect.Width - 2), Math.Max(3, logRect.Height - 2));
        var logLines = _logViewport.RenderLines();
        TWidgets.DrawPanel(
            canvas,
            logRect,
            _focus == WorkspaceFocus.Log ? "Log *" : "Log",
            [.. logLines]);

        var footerRect = new Rect(0, _height - footerHeight, _width, footerHeight);
        var inputFrame = _commandInput.BuildFrame(Math.Max(10, _width - 6));
        var inputLine = $"> {inputFrame.Text}";

        var activeBindings = _focus switch
        {
            WorkspaceFocus.Actions => _listKeys.HelpBindings,
            WorkspaceFocus.Log => _viewportKeys.HelpBindings,
            WorkspaceFocus.Command => _inputKeys.HelpBindings,
            _ => _inputKeys.HelpBindings,
        };

        var bindingSet = _showFullHelp
            ? activeBindings.Concat(_globalHelp)
            : activeBindings.Take(4).Concat(_globalHelp);
        var helpText = HelpView.RenderCompact(bindingSet, Math.Max(10, _width - 4));

        var footerLines = new List<string>
        {
            inputLine,
            $"focus={_focus.ToString().ToLowerInvariant()} filter='{_actionList.Filter}' stress={ToYesNo(_stressMode)}",
        };
        footerLines.AddRange(helpText.Split('\n'));

        TWidgets.DrawPanel(
            canvas,
            footerRect,
            _focus == WorkspaceFocus.Command ? "Command *" : "Command",
            footerLines);

        if (_focus == WorkspaceFocus.Command)
        {
            cursorX = Math.Clamp(footerRect.X + 3 + inputFrame.CursorColumn, footerRect.X + 1, footerRect.Right - 2);
            cursorY = Math.Clamp(footerRect.Y + 1, footerRect.Y + 1, footerRect.Bottom - 2);
        }

        var rendered = canvas.Render();
        return ApplyWorkspaceStyles(rendered, footerRect.Y + 1, inputFrame.PlaceholderVisible);
    }

    private string ApplyWorkspaceStyles(string frame, int inputRow, bool placeholderVisible)
    {
        var rows = frame.Split('\n');
        if (rows.Length == 0)
        {
            return frame;
        }

        rows[0] = HeaderStyle.Render(rows[0]);

        for (var i = 0; i < rows.Length; i++)
        {
            if (rows[i].Contains("› ", StringComparison.Ordinal))
            {
                rows[i] = AccentStyle.Render(rows[i]);
                continue;
            }

            if (placeholderVisible && i == inputRow)
            {
                rows[i] = MutedStyle.Render(rows[i]);
                continue;
            }

            if (rows[i].Contains("raw mode: no", StringComparison.Ordinal))
            {
                rows[i] = WarningStyle.Render(rows[i]);
            }
        }

        return string.Join('\n', rows);
    }

    private string ActionState(ActionItem action)
    {
        return action.Name switch
        {
            "Toggle stress" => _stressMode ? "on" : "off",
            "Reset count" => $"{_count}",
            "Switch to protocol" => "ready",
            _ => "ready",
        };
    }

    private void ApplyWidgetSizing()
    {
        var safeWidth = Math.Max(52, _width);
        var safeHeight = Math.Max(18, _height);

        const int headerHeight = 3;
        const int footerHeight = 5;
        var bodyHeight = safeHeight - headerHeight - footerHeight;
        var leftWidth = Math.Max(30, safeWidth / 3);
        var rightWidth = safeWidth - leftWidth;

        _actionList.PageSize = Math.Max(1, bodyHeight - 3);
        _logViewport.Resize(Math.Max(12, rightWidth - 2), Math.Max(4, bodyHeight - 2));
    }

    private void CycleFocus()
    {
        _focus = _focus switch
        {
            WorkspaceFocus.Actions => WorkspaceFocus.Log,
            WorkspaceFocus.Log => WorkspaceFocus.Command,
            _ => WorkspaceFocus.Actions,
        };
    }

    private Command NextTickCommand(bool stressMode)
    {
        var delay = stressMode ? TimeSpan.FromMilliseconds(45) : TimeSpan.FromMilliseconds(150);
        return Tea.Cmd.Tick(delay, _ => new DashboardTickMsg());
    }

    private void AppendSparkSample()
    {
        var normalizedCount = Math.Clamp(_count + 20, 0, 40);
        var wave = 50 + (int)Math.Round(45 * Math.Sin(_tickCount / 8.0), MidpointRounding.AwayFromZero);
        var sample = (wave + ((normalizedCount * 100) / 40)) / 2;
        _sparkline.Add(Math.Clamp(sample, 0, 100));
        var maxSamples = Math.Max(12, _width - 8);
        if (_sparkline.Count > maxSamples)
        {
            _sparkline.RemoveAt(0);
        }
    }

    private static string ToYesNo(bool value) => value ? "yes" : "no";

    private static string SummarizeProbe(string probe)
    {
        if (string.IsNullOrWhiteSpace(probe))
        {
            return "n/a";
        }

        var compact = probe
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Trim();

        return compact.Length <= 160
            ? compact
            : compact[..160] + "...";
    }

    private static string SanitizePreview(string content)
    {
        var sanitized = content
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);

        return sanitized.Length <= 96
            ? sanitized
            : sanitized[..96] + "...";
    }

    private string FormatModeReports()
    {
        return
            $"  ?1004={FormatModeState(1004)}\n" +
            $"  ?1006={FormatModeState(1006)}\n" +
            $"  ?2004={FormatModeState(2004)}\n" +
            $"  ?2026={FormatModeState(2026)}";
    }

    private string FormatModeState(int mode)
    {
        return _modeReports.TryGetValue(mode, out var state)
            ? state.ToString().ToLowerInvariant()
            : "pending";
    }
}

internal sealed record DashboardTickMsg : IMessage;

internal sealed record ActionItem(string Name, string Shortcut);

internal enum WorkspaceFocus
{
    Actions = 0,
    Log = 1,
    Command = 2,
}
