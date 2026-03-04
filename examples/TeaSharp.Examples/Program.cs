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
        Placeholder = "type command (help, inc, dec, stress on/off/toggle, filter <term>, clear, protocol, dashboard, showcase)",
        MaxLength = 256,
    };
    private readonly List<ActionItem> _allActions =
    [
        new ActionItem("Increment count", "enter"),
        new ActionItem("Decrement count", "enter"),
        new ActionItem("Toggle stress", "enter"),
        new ActionItem("Reset count", "enter"),
        new ActionItem("Clear log", "enter"),
        new ActionItem("Switch to dashboard", "enter"),
        new ActionItem("Switch to showcase", "enter"),
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
        new KeyBinding("1/2/3", "switch page"),
        new KeyBinding("q", "quit"),
    ];
    private readonly List<string> _eventLog = [];
    private readonly LineChartComponent _throughputChart = new(capacity: 240)
    {
        Title = "Throughput",
        MinValue = 0,
        MaxValue = 100,
    };
    private readonly BarChartComponent _statusChart = new()
    {
        Title = "Status Mix",
        MaxValue = 100,
    };
    private readonly GaugeComponent _countGauge = new()
    {
        Title = "Count Gauge",
        MinValue = -10,
        MaxValue = 10,
    };
    private readonly StatsCardComponent _capabilityCard = new()
    {
        Title = "Capabilities",
    };
    private readonly MiniLogComponent _miniLog = new(capacity: 180)
    {
        Title = "Live Event",
    };
    private readonly UnicodeShowcaseComponent _unicodeShowcase = new();
    private readonly ComponentComposer _workspaceComposer = new();

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
    private AppPage _page = AppPage.Dashboard;
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
        RefreshStatusBars();
        AppendLog("TeaSharp workspace initialized.");
        AppendLog("Use tab to move focus between actions, log, and command input.");
        AppendLog("Use ? to toggle full help and 1/2/3 to switch protocol/dashboard/showcase.");
    }

    public Command? Init() => NextTickCommand(_stressMode);

    private bool IsWorkspacePage => _page is AppPage.Dashboard or AppPage.Showcase;

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
                _page = AppPage.Protocol;
                _lastEvent = "view: protocol";
                return new UpdateResult(this, null);
            }

            if (key.Text == "2")
            {
                _page = AppPage.Dashboard;
                _lastEvent = "view: dashboard";
                return new UpdateResult(this, null);
            }

            if (key.Text == "3")
            {
                _page = AppPage.Showcase;
                _lastEvent = "view: showcase";
                return new UpdateResult(this, null);
            }

            if (key.Text == "?" && key.Modifiers == KeyModifiers.None && IsWorkspacePage)
            {
                _showFullHelp = !_showFullHelp;
                _lastEvent = $"help: {(_showFullHelp ? "full" : "compact")}";
                return new UpdateResult(this, null);
            }

            if (key.Code == KeyCode.Tab && IsWorkspacePage)
            {
                CycleFocus();
                _lastEvent = $"focus: {_focus.ToString().ToLowerInvariant()}";
                return new UpdateResult(this, null);
            }

            if (key.Text == "s" && key.Modifiers == KeyModifiers.None)
            {
                _stressMode = !_stressMode;
                RefreshStatusBars();
                _lastEvent = $"stress: {(_stressMode ? "on" : "off")}";
                AppendLog(_lastEvent);
                return new UpdateResult(this, NextTickCommand(_stressMode));
            }

            if (IsWorkspacePage)
            {
                return HandleWorkspaceKey(key);
            }

            return HandleProtocolKey(key);
        }

        if (message is PasteMsg paste)
        {
            _lastPaste = SanitizePreview(paste.Content);
            if (IsWorkspacePage && _focus == WorkspaceFocus.Command)
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
            RefreshStatusBars();
            _lastEvent = "focus: in";
            return new UpdateResult(this, null);
        }

        if (message is FocusOutMsg)
        {
            _focused = false;
            RefreshStatusBars();
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

            if (IsWorkspacePage && (_focus == WorkspaceFocus.Log || mouse is MouseWheelMsg))
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
            RefreshStatusBars();
            _lastEvent = $"capabilities: {_capabilities.Source}";
            AppendLog(_lastEvent);
            return new UpdateResult(this, null);
        }

        if (message is DashboardTickMsg)
        {
            _tickCount++;
            RefreshStatusBars();
            AppendSparkSample();
            _throughputChart.Append(_sparkline[^1]);
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

        var content = _page switch
        {
            AppPage.Protocol => BuildProbeView(),
            AppPage.Dashboard => BuildWorkspaceView(out cursorX, out cursorY),
            AppPage.Showcase => BuildShowcaseView(out cursorX, out cursorY),
            _ => BuildProbeView(),
        };

        return ModelView.From(content) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            EnableSynchronizedUpdates = true,
            MouseMode = MouseMode.AllMotion,
            CursorX = cursorX,
            CursorY = cursorY,
            WindowTitle = _page switch
            {
                AppPage.Protocol => "TeaSharp Protocol Probe",
                AppPage.Dashboard => "TeaSharp Dashboard",
                AppPage.Showcase => "TeaSharp Capability Showcase",
                _ => "TeaSharp",
            },
        };
    }

    private UpdateResult HandleProtocolKey(KeyPressMsg key)
    {
        if (key.Code == KeyCode.Up)
        {
            _count++;
            RefreshStatusBars();
        }
        else if (key.Code == KeyCode.Down)
        {
            _count--;
            RefreshStatusBars();
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
                RefreshStatusBars();
                AppendLog("action: increment count");
                break;
            case "Decrement count":
                _count--;
                RefreshStatusBars();
                AppendLog("action: decrement count");
                break;
            case "Toggle stress":
                _stressMode = !_stressMode;
                RefreshStatusBars();
                AppendLog($"action: stress {(_stressMode ? "on" : "off")}");
                break;
            case "Reset count":
                _count = 0;
                RefreshStatusBars();
                AppendLog("action: reset count");
                break;
            case "Clear log":
                _eventLog.Clear();
                _miniLog.Clear();
                _logViewport.Clear();
                RefreshStatusBars();
                AppendLog("action: clear log");
                break;
            case "Switch to dashboard":
                _page = AppPage.Dashboard;
                AppendLog("action: switch to dashboard");
                break;
            case "Switch to showcase":
                _page = AppPage.Showcase;
                AppendLog("action: switch to showcase");
                break;
            case "Switch to protocol":
                _page = AppPage.Protocol;
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
            AppendLog("commands: help | inc | dec | stress on/off/toggle | filter <term> | clear | protocol | dashboard | showcase");
            return;
        }

        if (string.Equals(command, "inc", StringComparison.OrdinalIgnoreCase))
        {
            _count++;
            RefreshStatusBars();
            AppendLog($"count={_count}");
            return;
        }

        if (string.Equals(command, "dec", StringComparison.OrdinalIgnoreCase))
        {
            _count--;
            RefreshStatusBars();
            AppendLog($"count={_count}");
            return;
        }

        if (command.StartsWith("stress ", StringComparison.OrdinalIgnoreCase))
        {
            var arg = command[7..].Trim();
            if (string.Equals(arg, "on", StringComparison.OrdinalIgnoreCase))
            {
                _stressMode = true;
                RefreshStatusBars();
                AppendLog("stress=on");
            }
            else if (string.Equals(arg, "off", StringComparison.OrdinalIgnoreCase))
            {
                _stressMode = false;
                RefreshStatusBars();
                AppendLog("stress=off");
            }
            else
            {
                _stressMode = !_stressMode;
                RefreshStatusBars();
                AppendLog($"stress toggled => {(_stressMode ? "on" : "off")}");
            }

            return;
        }

        if (string.Equals(command, "clear", StringComparison.OrdinalIgnoreCase))
        {
            _eventLog.Clear();
            _miniLog.Clear();
            _logViewport.Clear();
            RefreshStatusBars();
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

        if (string.Equals(command, "protocol", StringComparison.OrdinalIgnoreCase))
        {
            _page = AppPage.Protocol;
            AppendLog("view=protocol");
            return;
        }

        if (string.Equals(command, "dashboard", StringComparison.OrdinalIgnoreCase))
        {
            _page = AppPage.Dashboard;
            AppendLog("view=dashboard");
            return;
        }

        if (string.Equals(command, "showcase", StringComparison.OrdinalIgnoreCase))
        {
            _page = AppPage.Showcase;
            AppendLog("view=showcase");
            return;
        }

        AppendLog($"unknown command: {command}");
    }

    private void AppendLog(string line)
    {
        var entry = $"[{DateTimeOffset.Now:HH:mm:ss}] {line}";
        _eventLog.Add(entry);
        _miniLog.Append(entry);
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
            "- press 2 for dashboard, 3 for showcase\n" +
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
                "TeaSharp Dashboard\n\n" +
                "Terminal too small for dashboard mode.\n" +
                "Resize to at least 52x18.\n\n" +
                "Press 1 protocol, 2 dashboard, 3 showcase.";
            return WarningStyle.Render(compact);
        }

        ApplyWidgetSizing();

        var canvas = new Canvas(_width, _height);
        const int headerHeight = 3;
        const int footerHeight = 5;
        var bodyTop = headerHeight;
        var bodyHeight = _height - headerHeight - footerHeight;
        var leftWidth = Math.Max(34, (_width * 44) / 100);
        var rightWidth = _width - leftWidth;

        var headerMode = "dashboard";
        TWidgets.DrawPanel(
            canvas,
            new Rect(0, 0, _width, headerHeight),
            "TeaSharp Dashboard",
            [
                $"count={_count} focus={(_focused ? "in" : "out")} size={_width}x{_height} mode={headerMode} source={_capabilities.Source}",
            ]);

        var leftRect = new Rect(0, bodyTop, leftWidth, bodyHeight);
        var rightRect = new Rect(leftWidth, bodyTop, rightWidth, bodyHeight);
        var systemHeight = Math.Clamp(bodyHeight / 2, 8, 12);
        if (bodyHeight - systemHeight < 6)
        {
            systemHeight = Math.Max(7, bodyHeight - 6);
        }

        var systemRect = new Rect(leftRect.X, leftRect.Y, leftRect.Width, systemHeight);
        var actionsRect = new Rect(leftRect.X, leftRect.Y + systemHeight, leftRect.Width, leftRect.Height - systemHeight);

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

        _workspaceComposer.Clear();
        var statusChartRect = DrawSystemPanel(canvas, systemRect);
        if (!statusChartRect.IsEmpty && statusChartRect.Height >= 3)
        {
            _workspaceComposer.Add(_statusChart, statusChartRect);
        }

        if (actionsRect.Height >= 4)
        {
            TWidgets.DrawTable(
                canvas,
                actionsRect,
                ["Action", "Key", "State"],
                tableRows,
                selectedPageRow,
                _focus == WorkspaceFocus.Actions ? "Actions *" : "Actions");
        }
        else
        {
            TWidgets.DrawPanel(
                canvas,
                actionsRect,
                _focus == WorkspaceFocus.Actions ? "Actions *" : "Actions",
                ["expand terminal for actions table"]);
        }

        var infoHeight = Math.Clamp(rightRect.Height / 3, 7, 10);
        var throughputHeight = Math.Clamp(rightRect.Height / 4, 5, 8);
        if (infoHeight + throughputHeight + 6 > rightRect.Height)
        {
            throughputHeight = Math.Max(4, rightRect.Height - infoHeight - 6);
        }

        if (infoHeight + throughputHeight + 4 > rightRect.Height)
        {
            infoHeight = Math.Max(5, rightRect.Height - throughputHeight - 4);
        }

        var infoRect = new Rect(rightRect.X, rightRect.Y, rightRect.Width, infoHeight);
        var throughputRect = new Rect(rightRect.X, infoRect.Bottom, rightRect.Width, throughputHeight);
        var logRect = new Rect(rightRect.X, throughputRect.Bottom, rightRect.Width, rightRect.Bottom - throughputRect.Bottom);

        if (!infoRect.IsEmpty && infoRect.Height >= 4)
        {
            var capabilityHeight = Math.Max(4, infoRect.Height - 3);
            var capabilityRect = new Rect(infoRect.X, infoRect.Y, infoRect.Width, capabilityHeight);
            _workspaceComposer.Add(_capabilityCard, capabilityRect);

            var gaugeRect = new Rect(infoRect.X, capabilityRect.Bottom, infoRect.Width, infoRect.Bottom - capabilityRect.Bottom);
            if (!gaugeRect.IsEmpty && gaugeRect.Height >= 3)
            {
                _workspaceComposer.Add(_countGauge, gaugeRect);
            }
        }

        if (!throughputRect.IsEmpty && throughputRect.Height >= 4)
        {
            _workspaceComposer.Add(_throughputChart, throughputRect);
        }

        Rect viewportLogRect;
        if (logRect.Height >= 9)
        {
            const int miniLogHeight = 4;
            var miniLogRect = new Rect(logRect.X, logRect.Y, logRect.Width, miniLogHeight);
            _workspaceComposer.Add(_miniLog, miniLogRect);
            viewportLogRect = new Rect(logRect.X, miniLogRect.Bottom, logRect.Width, logRect.Bottom - miniLogRect.Bottom);
        }
        else
        {
            viewportLogRect = logRect;
        }

        _workspaceComposer.Render(canvas);

        _logViewport.Resize(Math.Max(8, viewportLogRect.Width - 2), Math.Max(3, viewportLogRect.Height - 2));
        var logLines = _logViewport.RenderLines();
        TWidgets.DrawPanel(
            canvas,
            viewportLogRect,
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
            $"focus={_focus.ToString().ToLowerInvariant()} filter='{_actionList.Filter}' stress={ToYesNo(_stressMode)} page=dashboard",
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

    private string BuildShowcaseView(out int? cursorX, out int? cursorY)
    {
        cursorX = null;
        cursorY = null;

        if (_width < 68 || _height < 20)
        {
            var compact =
                "TeaSharp Capability Showcase\n\n" +
                "Terminal too small for showcase mode.\n" +
                "Resize to at least 68x20.\n\n" +
                "Press 1 protocol, 2 dashboard, 3 showcase.";
            return WarningStyle.Render(compact);
        }

        var canvas = new Canvas(_width, _height, CanvasTextMode.GraphemeAware);
        const int headerHeight = 3;
        const int footerHeight = 6;
        var bodyTop = headerHeight;
        var bodyHeight = _height - headerHeight - footerHeight;
        var leftWidth = Math.Max(34, (_width * 36) / 100);
        var rightWidth = _width - leftWidth;

        TWidgets.DrawPanel(
            canvas,
            new Rect(0, 0, _width, headerHeight),
            "TeaSharp Capability Showcase",
            [
                $"count={_count} focus={(_focused ? "in" : "out")} size={_width}x{_height} source={_capabilities.Source}",
            ]);

        var leftRect = new Rect(0, bodyTop, leftWidth, bodyHeight);
        var rightRect = new Rect(leftWidth, bodyTop, rightWidth, bodyHeight);

        var actionsHeight = Math.Max(7, bodyHeight / 2);
        var actionsRect = new Rect(leftRect.X, leftRect.Y, leftRect.Width, actionsHeight);
        var leftLowerRect = new Rect(leftRect.X, actionsRect.Bottom, leftRect.Width, leftRect.Bottom - actionsRect.Bottom);

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

        if (actionsRect.Height >= 4)
        {
            TWidgets.DrawTable(
                canvas,
                actionsRect,
                ["Action", "Key", "State"],
                tableRows,
                selectedPageRow,
                _focus == WorkspaceFocus.Actions ? "Actions *" : "Actions");
        }

        var capabilityHeight = Math.Max(4, leftLowerRect.Height - 3);
        var capabilityRect = new Rect(leftLowerRect.X, leftLowerRect.Y, leftLowerRect.Width, capabilityHeight);
        var gaugeRect = new Rect(leftLowerRect.X, capabilityRect.Bottom, leftLowerRect.Width, leftLowerRect.Bottom - capabilityRect.Bottom);

        var unicodeHeight = Math.Clamp(bodyHeight / 4, 5, 8);
        var throughputHeight = Math.Clamp(bodyHeight / 4, 5, 8);
        var statusHeight = Math.Clamp(bodyHeight / 5, 4, 6);
        if (unicodeHeight + throughputHeight + statusHeight + 6 > bodyHeight)
        {
            statusHeight = Math.Max(3, bodyHeight - unicodeHeight - throughputHeight - 6);
        }

        var unicodeRect = new Rect(rightRect.X, rightRect.Y, rightRect.Width, unicodeHeight);
        var throughputRect = new Rect(rightRect.X, unicodeRect.Bottom, rightRect.Width, throughputHeight);
        var statusRect = new Rect(rightRect.X, throughputRect.Bottom, rightRect.Width, statusHeight);
        var logRect = new Rect(rightRect.X, statusRect.Bottom, rightRect.Width, rightRect.Bottom - statusRect.Bottom);

        _workspaceComposer.Clear();
        if (!capabilityRect.IsEmpty && capabilityRect.Height >= 4)
        {
            _workspaceComposer.Add(_capabilityCard, capabilityRect);
        }

        if (!gaugeRect.IsEmpty && gaugeRect.Height >= 3)
        {
            _workspaceComposer.Add(_countGauge, gaugeRect);
        }

        _unicodeShowcase.CapabilitySource = _capabilities.Source;
        _unicodeShowcase.Focus = _focused;
        _unicodeShowcase.LastPaste = _lastPaste;
        _unicodeShowcase.TypedPreview = SanitizePreview(_typedText);
        _unicodeShowcase.Count = _count;
        if (!unicodeRect.IsEmpty && unicodeRect.Height >= 4)
        {
            _workspaceComposer.Add(_unicodeShowcase, unicodeRect);
        }

        if (!throughputRect.IsEmpty && throughputRect.Height >= 4)
        {
            _workspaceComposer.Add(_throughputChart, throughputRect);
        }

        if (!statusRect.IsEmpty && statusRect.Height >= 3)
        {
            _workspaceComposer.Add(_statusChart, statusRect);
        }

        Rect viewportLogRect;
        if (logRect.Height >= 9)
        {
            var miniLogRect = new Rect(logRect.X, logRect.Y, logRect.Width, 4);
            _workspaceComposer.Add(_miniLog, miniLogRect);
            viewportLogRect = new Rect(logRect.X, miniLogRect.Bottom, logRect.Width, logRect.Bottom - miniLogRect.Bottom);
        }
        else
        {
            viewportLogRect = logRect;
        }

        _workspaceComposer.Render(canvas);

        _logViewport.Resize(Math.Max(10, viewportLogRect.Width - 2), Math.Max(3, viewportLogRect.Height - 2));
        var logLines = _logViewport.RenderLines();
        TWidgets.DrawPanel(
            canvas,
            viewportLogRect,
            _focus == WorkspaceFocus.Log ? "Log *" : "Log",
            [.. logLines]);

        var footerRect = new Rect(0, _height - footerHeight, _width, footerHeight);
        var inputFrame = _commandInput.BuildFrame(Math.Max(12, _width - 6));
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
            : activeBindings.Take(5).Concat(_globalHelp);
        var helpText = HelpView.RenderCompact(bindingSet, Math.Max(10, _width - 4));

        var footerLines = new List<string>
        {
            inputLine,
            $"focus={_focus.ToString().ToLowerInvariant()} filter='{_actionList.Filter}' stress={ToYesNo(_stressMode)} page=showcase",
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

    private Rect DrawSystemPanel(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return new Rect(0, 0, 0, 0);
        }

        canvas.DrawBox(clipped, "System");
        var content = clipped.Inset(1, 1);
        if (content.IsEmpty)
        {
            return new Rect(0, 0, 0, 0);
        }

        var systemLines = new[]
        {
            $"raw mode: {ToYesNo(_terminal.IsRawModeActive)}",
            $"backend: {(_terminal.IsRawModeActive ? "vt-bytes" : "console-keys-fallback")}",
            $"focus support: {ToYesNo(_capabilities.FocusReporting)}",
            $"mouse support: {ToYesNo(_capabilities.MouseReporting)}",
            $"paste support: {ToYesNo(_capabilities.BracketedPaste)}",
            $"stress mode: {ToYesNo(_stressMode)} ({_tickCount} ticks)",
            $"last event: {_lastEvent}",
        };

        var infoRows = Math.Min(systemLines.Length, Math.Max(1, content.Height - 4));
        for (var i = 0; i < infoRows; i++)
        {
            canvas.WriteText(content.X, content.Y + i, systemLines[i], content.Width);
        }

        var statusTop = Math.Min(content.Bottom, content.Y + infoRows);
        var statusHeight = content.Bottom - statusTop;
        if (statusHeight < 3)
        {
            return new Rect(0, 0, 0, 0);
        }

        return new Rect(content.X, statusTop, content.Width, statusHeight);
    }

    private string ActionState(ActionItem action)
    {
        return action.Name switch
        {
            "Toggle stress" => _stressMode ? "on" : "off",
            "Reset count" => $"{_count}",
            "Switch to dashboard" => _page == AppPage.Dashboard ? "active" : "ready",
            "Switch to showcase" => _page == AppPage.Showcase ? "active" : "ready",
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
        var leftWidth = Math.Max(34, (safeWidth * 44) / 100);
        var rightWidth = safeWidth - leftWidth;

        var systemHeight = Math.Clamp(bodyHeight / 2, 8, 12);
        if (bodyHeight - systemHeight < 6)
        {
            systemHeight = Math.Max(7, bodyHeight - 6);
        }

        var actionHeight = Math.Max(4, bodyHeight - systemHeight);
        _actionList.PageSize = Math.Max(1, actionHeight - 3);

        var infoHeight = Math.Clamp(bodyHeight / 3, 7, 10);
        var throughputHeight = Math.Clamp(bodyHeight / 4, 5, 8);
        if (infoHeight + throughputHeight + 6 > bodyHeight)
        {
            throughputHeight = Math.Max(4, bodyHeight - infoHeight - 6);
        }

        if (infoHeight + throughputHeight + 4 > bodyHeight)
        {
            infoHeight = Math.Max(5, bodyHeight - throughputHeight - 4);
        }

        var logHeight = Math.Max(4, bodyHeight - infoHeight - throughputHeight);
        var viewportHeight = logHeight >= 9
            ? Math.Max(4, logHeight - 4)
            : logHeight;
        _logViewport.Resize(Math.Max(12, rightWidth - 2), Math.Max(3, viewportHeight - 2));
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

    private void RefreshStatusBars()
    {
        var normalizedCount = Math.Clamp(_count + 50, 0, 100);
        var normalizedTicks = Math.Clamp(_tickCount % 100, 0, 100);
        var absBound = Math.Max(10, Math.Abs(_count));

        _statusChart.SetBars(
        [
            new BarDatum("raw", _terminal.IsRawModeActive ? 100 : 0),
            new BarDatum("focus", _focused ? 100 : 0),
            new BarDatum("mouse", _capabilities.MouseReporting ? 100 : 0),
            new BarDatum("paste", _capabilities.BracketedPaste ? 100 : 0),
            new BarDatum("stress", _stressMode ? 100 : 0),
            new BarDatum("count", normalizedCount),
            new BarDatum("pulse", normalizedTicks),
        ]);

        _countGauge.MinValue = -absBound;
        _countGauge.MaxValue = absBound;
        _countGauge.Value = _count;
        _countGauge.Label = $"count {_count} range ±{absBound}";

        _capabilityCard.SetItems(
        [
            new StatsCardItem("raw", ToYesNo(_terminal.IsRawModeActive)),
            new StatsCardItem("backend", _terminal.IsRawModeActive ? "vt-bytes" : "console"),
            new StatsCardItem("focus", ToYesNo(_capabilities.FocusReporting)),
            new StatsCardItem("mouse", ToYesNo(_capabilities.MouseReporting)),
            new StatsCardItem("paste", ToYesNo(_capabilities.BracketedPaste)),
            new StatsCardItem("sync", ToYesNo(_capabilities.SynchronizedUpdates)),
            new StatsCardItem("stress", ToYesNo(_stressMode)),
            new StatsCardItem("source", _capabilities.Source),
        ]);
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

internal enum AppPage
{
    Protocol = 0,
    Dashboard = 1,
    Showcase = 2,
}

internal enum WorkspaceFocus
{
    Actions = 0,
    Log = 1,
    Command = 2,
}
