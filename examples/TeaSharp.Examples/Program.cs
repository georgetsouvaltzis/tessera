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
        Placeholder = "type command (help, inc, dec, stress on/off/toggle, filter <term>, clear, protocol/dashboard/showcase, toast <text>, modal on/off/toggle, tab <n>)",
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
        new KeyBinding(":", "enter command mode"),
        new KeyBinding("esc", "exit command mode"),
        new KeyBinding("?", "toggle help"),
        new KeyBinding("1/2/3", "switch page"),
        new KeyBinding("q", "quit"),
    ];
    private readonly KeyBinding[] _showcaseHelp =
    [
        new KeyBinding("p/P", "cycle pane"),
        new KeyBinding("left/right", "switch tab"),
        new KeyBinding("t", "toast"),
        new KeyBinding("m", "modal"),
        new KeyBinding("a/z/r/f", "pane actions"),
        new KeyBinding("c/v/[/]", "table controls"),
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
    private readonly TabsComponent _showcaseTabs = new(["Overview", "Data", "Forms"]);
    private readonly AccordionComponent _showcaseAccordion = new()
    {
        Title = "Playbook",
    };
    private readonly SortableTableComponent _showcaseTable = new(["Metric", "Value", "Trend"])
    {
        Title = "Metrics Table",
        PageSize = 5,
    };
    private readonly CheckboxListComponent _showcaseChecklist = new()
    {
        Title = "Checklist",
    };
    private readonly RadioGroupComponent _showcaseTheme = new()
    {
        Title = "Theme",
    };
    private readonly SelectComponent _showcaseDensity = new()
    {
        Title = "Density",
    };
    private readonly ToastCenterComponent _showcaseToasts = new()
    {
        MaxToasts = 2,
    };
    private readonly ModalComponent _showcaseModal = new()
    {
        Title = "Showcase Help",
        BorderStyle = BorderStyle.Heavy,
        Lines =
        [
            "Hotkeys",
            "t toast  m modal  a accordion  z checklist",
            "r theme  f density  c column  v sort",
            "[/ ] page table  left/right tab  p/P pane",
        ],
    };
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
    private WorkspaceFocus _focusBeforeCommand = WorkspaceFocus.Actions;
    private bool _showFullHelp;
    private ShowcasePane _showcasePane = ShowcasePane.OverviewUnicode;
    private WorkspaceInputMode _workspaceInputMode = WorkspaceInputMode.Navigate;
    private DateTimeOffset _lastEscapePress = DateTimeOffset.MinValue;
    private string _showcaseLastEvent = "none";
    private int _showcaseTickSnapshot;
    private int _showcaseCountSnapshot;
    private int _showcaseWidthSnapshot = 80;
    private int _showcaseHeightSnapshot = 24;
    private string _showcaseSourceSnapshot = "unknown";

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
        _throughputChart.Options = new LineChartOptions(
            ShowAxes: true,
            Legend: "ops/s",
            XLabel: "t",
            YLabel: "y");
        _statusChart.Options = new BarChartOptions(
            ShowScale: true,
            Legend: "mix");
        _showcaseAccordion.SetSections(
        [
            new AccordionSection("Input", ["keyboard, mouse, focus, paste", "enhanced VT decode"], Expanded: true),
            new AccordionSection("Render", ["frame buffer diff", "grapheme-aware canvas", "component composer"]),
            new AccordionSection("Runtime", ["commands + ticks", "capability detection", "cross-platform terminal adapter"]),
        ]);
        _showcaseTable.SetRows(
        [
            ["throughput", "86", "up"],
            ["latency", "24ms", "flat"],
            ["errors", "0.1%", "down"],
            ["sessions", "312", "up"],
            ["queue", "18", "flat"],
            ["cpu", "33%", "up"],
            ["memory", "58%", "flat"],
            ["drop-rate", "0.02%", "down"],
        ]);
        _showcaseChecklist.SetItems(
        [
            ("raw mode", true),
            ("focus events", true),
            ("mouse events", true),
            ("paste capture", true),
            ("decrpm reports", true),
        ]);
        _showcaseTheme.SetItems(["classic", "ocean", "amber"]);
        _showcaseDensity.SetItems(["compact", "cozy", "comfortable"]);
        RefreshStatusBars();
        CaptureShowcaseSnapshot();
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
            if (IsEscapeKey(key))
            {
                _lastEscapePress = DateTimeOffset.UtcNow;
            }

            if (IsPlainChar(key, "q")
                || ((key.Text == "c" || key.Text == "\u0003") && key.Modifiers.HasFlag(KeyModifiers.Ctrl)))
            {
                return new UpdateResult(this, Tea.Cmd.Quit);
            }

            if (IsWorkspacePage && IsCommandModeEnterKey(key))
            {
                EnterCommandMode();
                return new UpdateResult(this, null);
            }

            if (IsWorkspacePage && IsEscapeKey(key))
            {
                if (_workspaceInputMode == WorkspaceInputMode.Command)
                {
                    ExitCommandMode();
                }

                return new UpdateResult(this, null);
            }

            if (IsPlainChar(key, "1") && !WasRecentEscape())
            {
                SwitchPage(AppPage.Protocol);
                _lastEvent = "view: protocol";
                return new UpdateResult(this, null);
            }

            if (IsPlainChar(key, "2") && !WasRecentEscape())
            {
                SwitchPage(AppPage.Dashboard);
                _lastEvent = "view: dashboard";
                return new UpdateResult(this, null);
            }

            if (IsPlainChar(key, "3") && !WasRecentEscape())
            {
                SwitchPage(AppPage.Showcase);
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

            if (key.Code == KeyCode.Character
                && string.Equals(key.Text, "s", StringComparison.OrdinalIgnoreCase)
                && key.Modifiers.HasFlag(KeyModifiers.Ctrl))
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
            if (IsWorkspacePage
                && _focus == WorkspaceFocus.Command
                && _workspaceInputMode == WorkspaceInputMode.Command)
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
            _showcaseToasts.Update(new TickMsg(DateTimeOffset.Now));
            RefreshShowcaseData();
            if (_page == AppPage.Showcase && _focus == WorkspaceFocus.Showcase)
            {
                CaptureShowcaseSnapshot();
            }
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

    private void RefreshShowcaseData()
    {
        var throughput = _sparkline.Count == 0 ? 0 : _sparkline[^1];
        _showcaseTable.SetRows(
        [
            ["throughput", throughput.ToString(System.Globalization.CultureInfo.InvariantCulture), throughput > 55 ? "up" : "flat"],
            ["latency", $"{Math.Max(1, 160 - throughput)}ms", throughput > 70 ? "down" : "flat"],
            ["errors", $"{Math.Max(0.0, (100 - throughput) / 800.0):0.00}%", throughput > 80 ? "down" : "up"],
            ["sessions", (120 + (_tickCount % 260)).ToString(System.Globalization.CultureInfo.InvariantCulture), _focused ? "up" : "flat"],
            ["queue", Math.Max(0, 90 - throughput).ToString(System.Globalization.CultureInfo.InvariantCulture), throughput > 60 ? "down" : "up"],
            ["cpu", $"{Math.Clamp((throughput * 7) / 10, 0, 100)}%", _stressMode ? "up" : "flat"],
            ["memory", $"{Math.Clamp(30 + ((_tickCount * 3) % 55), 0, 100)}%", "flat"],
            ["drop-rate", $"{Math.Max(0.0, (55 - throughput) / 1200.0):0.000}%", throughput > 65 ? "down" : "flat"],
        ]);
    }

    private void SwitchPage(AppPage page)
    {
        _page = page;
        _focus = page switch
        {
            AppPage.Protocol => WorkspaceFocus.Actions,
            _ => WorkspaceFocus.Actions,
        };
        _focusBeforeCommand = _focus;
        _workspaceInputMode = WorkspaceInputMode.Navigate;

        if (page == AppPage.Showcase)
        {
            EnsureShowcasePaneInRange();
            CaptureShowcaseSnapshot();
        }
    }

    private void CaptureShowcaseSnapshot(string? showcaseEvent = null)
    {
        if (!string.IsNullOrWhiteSpace(showcaseEvent))
        {
            _showcaseLastEvent = showcaseEvent.Trim();
        }

        _showcaseTickSnapshot = _tickCount;
        _showcaseCountSnapshot = _count;
        _showcaseWidthSnapshot = _width;
        _showcaseHeightSnapshot = _height;
        _showcaseSourceSnapshot = _capabilities.Source;
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
        if (_page == AppPage.Showcase
            && _focus == WorkspaceFocus.Showcase
            && HandleShowcaseNavigationKey(key))
        {
            return new UpdateResult(this, null);
        }

        if (_page == AppPage.Showcase
            && _focus == WorkspaceFocus.Showcase
            && HandleShowcaseHotKey(key))
        {
            return new UpdateResult(this, null);
        }

        if (_focus == WorkspaceFocus.Showcase)
        {
            _lastEvent = $"key: {key.Keystroke()}";
            return new UpdateResult(this, null);
        }

        if (_focus == WorkspaceFocus.Command
            && _workspaceInputMode != WorkspaceInputMode.Command)
        {
            _lastEvent = "command mode locked (press : to enter)";
            return new UpdateResult(this, null);
        }

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

    private bool HandleShowcaseNavigationKey(KeyPressMsg key)
    {
        if (IsPlainChar(key, "p"))
        {
            MoveShowcasePane(1);
            _lastEvent = $"showcase: pane={ShowcasePaneLabel()}";
            CaptureShowcaseSnapshot(_lastEvent);
            AppendLog(_lastEvent);
            return true;
        }

        if (IsPlainShiftChar(key, "p"))
        {
            MoveShowcasePane(-1);
            _lastEvent = $"showcase: pane={ShowcasePaneLabel()}";
            CaptureShowcaseSnapshot(_lastEvent);
            AppendLog(_lastEvent);
            return true;
        }

        if (_showcaseTabs.Update(key))
        {
            EnsureShowcasePaneInRange();
            _lastEvent = $"showcase: tab={_showcaseTabs.SelectedIndex + 1}";
            CaptureShowcaseSnapshot(_lastEvent);
            AppendLog(_lastEvent);
            return true;
        }

        return false;
    }

    private string ShowcaseModeLabel()
    {
        return _workspaceInputMode == WorkspaceInputMode.Command
            ? "cmd"
            : "nav";
    }

    private string InputModeLabel()
    {
        return _workspaceInputMode == WorkspaceInputMode.Command
            ? "CMD"
            : "NAV";
    }

    private static bool IsPlainChar(KeyPressMsg key, string text)
    {
        return key.Code == KeyCode.Character
            && key.Modifiers == KeyModifiers.None
            && string.Equals(key.Text, text, StringComparison.Ordinal);
    }

    private static bool IsPlainShiftChar(KeyPressMsg key, string lower)
    {
        return key.Code == KeyCode.Character
            && (key.Modifiers == KeyModifiers.Shift
                || (key.Modifiers == KeyModifiers.None
                    && key.Text.Length == 1
                    && char.IsUpper(key.Text[0])))
            && string.Equals(key.Text, lower, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCommandModeEnterKey(KeyPressMsg key)
    {
        return key.Code == KeyCode.Character
            && (key.Text == ":"
                || (key.Text == ";" && key.Modifiers.HasFlag(KeyModifiers.Shift)))
            && !key.Modifiers.HasFlag(KeyModifiers.Ctrl)
            && !key.Modifiers.HasFlag(KeyModifiers.Alt)
            && !key.Modifiers.HasFlag(KeyModifiers.Meta);
    }

    private static bool IsEscapeKey(KeyPressMsg key)
    {
        return key.Code == KeyCode.Escape
            || (key.Code == KeyCode.Character
                && string.Equals(key.Text, "\u001b", StringComparison.Ordinal));
    }

    private void EnterCommandMode()
    {
        if (_focus != WorkspaceFocus.Command)
        {
            _focusBeforeCommand = _focus;
        }

        _workspaceInputMode = WorkspaceInputMode.Command;
        _focus = WorkspaceFocus.Command;
        _lastEvent = "mode: cmd-input";
        AppendLog(_lastEvent);
        if (_page == AppPage.Showcase)
        {
            CaptureShowcaseSnapshot(_lastEvent);
        }
    }

    private void ExitCommandMode()
    {
        if (_workspaceInputMode != WorkspaceInputMode.Command)
        {
            return;
        }

        _workspaceInputMode = WorkspaceInputMode.Navigate;
        if (_focus == WorkspaceFocus.Command)
        {
            _focus = _focusBeforeCommand switch
            {
                WorkspaceFocus.Command => WorkspaceFocus.Actions,
                _ => _focusBeforeCommand,
            };
        }

        _lastEvent = $"mode: {ShowcaseModeLabel()}";
        AppendLog(_lastEvent);
        if (_page == AppPage.Showcase)
        {
            CaptureShowcaseSnapshot(_lastEvent);
        }
    }

    private bool WasRecentEscape()
    {
        return (DateTimeOffset.UtcNow - _lastEscapePress) <= TimeSpan.FromMilliseconds(220);
    }

    private bool HandleShowcaseHotKey(KeyPressMsg key)
    {
        var changed = false;
        var action = string.Empty;

        if (key.Text == "t" && key.Modifiers == KeyModifiers.None)
        {
            _showcaseToasts.Push(new ToastMessage($"tick={_tickCount} count={_count}", TtlTicks: 70, Severity: ToastSeverity.Info));
            changed = true;
            action = "toast";
        }
        else if (key.Text == "m" && key.Modifiers == KeyModifiers.None)
        {
            _showcaseModal.Visible = !_showcaseModal.Visible;
            changed = true;
            action = _showcaseModal.Visible ? "modal=open" : "modal=close";
        }
        changed = HandleFocusedShowcasePaneKey(key, ref action) || changed;

        if (!changed)
        {
            return false;
        }

        _lastEvent = $"showcase: {(string.IsNullOrWhiteSpace(action) ? key.Keystroke() : action)}";
        CaptureShowcaseSnapshot(_lastEvent);
        AppendLog(_lastEvent);
        return true;
    }

    private bool HandleFocusedShowcasePaneKey(KeyPressMsg key, ref string action)
    {
        if (_showcaseTabs.SelectedIndex == 1 && _showcasePane == ShowcasePane.DataTable)
        {
            if (key.Text == "c" && key.Modifiers == KeyModifiers.None)
            {
                var changed = _showcaseTable.Update(key);
                if (changed)
                {
                    action = "table=column";
                }

                return changed;
            }

            if ((key.Text == "v" || key.Text == "[" || key.Text == "]") && key.Modifiers == KeyModifiers.None)
            {
                var mapped = key.Text == "v"
                    ? new KeyPressMsg(KeyCode.Character, "s")
                    : key;
                var changed = _showcaseTable.Update(mapped);
                if (changed)
                {
                    action = key.Text == "v"
                        ? "table=sort"
                        : "table=page";
                }

                return changed;
            }
        }

        if (_showcaseTabs.SelectedIndex == 2)
        {
            if (_showcasePane == ShowcasePane.FormsPlaybook)
            {
                if (key.Text == "a" && key.Modifiers == KeyModifiers.None)
                {
                    var toggled = _showcaseAccordion.Update(new KeyPressMsg(KeyCode.Enter));
                    if (toggled)
                    {
                        action = "accordion=toggle";
                    }

                    return toggled;
                }

                if (key.Code == KeyCode.Up || key.Code == KeyCode.Down || key.Code == KeyCode.Enter || key.Text == " ")
                {
                    var changed = _showcaseAccordion.Update(key);
                    if (changed)
                    {
                        action = "accordion=update";
                    }

                    return changed;
                }
            }

            if (_showcasePane == ShowcasePane.FormsChecklist)
            {
                if (key.Text == "z" && key.Modifiers == KeyModifiers.None)
                {
                    var toggled = _showcaseChecklist.Update(new KeyPressMsg(KeyCode.Enter));
                    if (toggled)
                    {
                        action = "check=toggle";
                    }

                    return toggled;
                }

                if (key.Code == KeyCode.Up || key.Code == KeyCode.Down || key.Code == KeyCode.Enter || key.Text == " ")
                {
                    var changed = _showcaseChecklist.Update(key);
                    if (changed)
                    {
                        action = "check=update";
                    }

                    return changed;
                }
            }

            if (_showcasePane == ShowcasePane.FormsTheme)
            {
                if (key.Text == "r" && key.Modifiers == KeyModifiers.None)
                {
                    var changed = _showcaseTheme.Update(new KeyPressMsg(KeyCode.Right));
                    if (changed)
                    {
                        action = "theme=next";
                    }

                    return changed;
                }

                if (key.Code is KeyCode.Up or KeyCode.Down or KeyCode.Left or KeyCode.Right)
                {
                    var changed = _showcaseTheme.Update(key);
                    if (changed)
                    {
                        action = "theme=update";
                    }

                    return changed;
                }
            }

            if (_showcasePane == ShowcasePane.FormsDensity)
            {
                if (key.Text == "f" && key.Modifiers == KeyModifiers.None)
                {
                    var changed = _showcaseDensity.Update(new KeyPressMsg(KeyCode.Right));
                    if (changed)
                    {
                        action = "density=next";
                    }

                    return changed;
                }

                if (key.Code is KeyCode.Up or KeyCode.Down or KeyCode.Left or KeyCode.Right)
                {
                    var changed = _showcaseDensity.Update(key);
                    if (changed)
                    {
                        action = "density=update";
                    }

                    return changed;
                }
            }
        }

        return false;
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
                SwitchPage(AppPage.Dashboard);
                AppendLog("action: switch to dashboard");
                break;
            case "Switch to showcase":
                SwitchPage(AppPage.Showcase);
                AppendLog("action: switch to showcase");
                break;
            case "Switch to protocol":
                SwitchPage(AppPage.Protocol);
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
            AppendLog("commands: help | inc | dec | stress on/off/toggle | filter <term> | clear | protocol | dashboard | showcase | toast <text> | modal on/off/toggle | tab <n>");
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
            SwitchPage(AppPage.Protocol);
            AppendLog("view=protocol");
            return;
        }

        if (string.Equals(command, "dashboard", StringComparison.OrdinalIgnoreCase))
        {
            SwitchPage(AppPage.Dashboard);
            AppendLog("view=dashboard");
            return;
        }

        if (string.Equals(command, "showcase", StringComparison.OrdinalIgnoreCase))
        {
            SwitchPage(AppPage.Showcase);
            AppendLog("view=showcase");
            return;
        }

        if (command.StartsWith("toast ", StringComparison.OrdinalIgnoreCase))
        {
            var payload = command[6..].Trim();
            if (payload.Length == 0)
            {
                payload = "hello from command";
            }

            _showcaseToasts.Push(new ToastMessage(payload, TtlTicks: 90, Severity: ToastSeverity.Success));
            CaptureShowcaseSnapshot("showcase: toast");
            AppendLog("toast queued");
            return;
        }

        if (command.StartsWith("modal ", StringComparison.OrdinalIgnoreCase))
        {
            var arg = command[6..].Trim();
            if (string.Equals(arg, "on", StringComparison.OrdinalIgnoreCase))
            {
                _showcaseModal.Visible = true;
            }
            else if (string.Equals(arg, "off", StringComparison.OrdinalIgnoreCase))
            {
                _showcaseModal.Visible = false;
            }
            else
            {
                _showcaseModal.Visible = !_showcaseModal.Visible;
            }

            CaptureShowcaseSnapshot($"showcase: modal={(_showcaseModal.Visible ? "on" : "off")}");
            AppendLog($"modal={(_showcaseModal.Visible ? "on" : "off")}");
            return;
        }

        if (command.StartsWith("tab ", StringComparison.OrdinalIgnoreCase))
        {
            var arg = command[4..].Trim();
            if (int.TryParse(arg, out var index))
            {
                _showcaseTabs.Select(index - 1);
                EnsureShowcasePaneInRange();
                CaptureShowcaseSnapshot($"showcase: tab={_showcaseTabs.SelectedIndex + 1}");
                AppendLog($"tab={_showcaseTabs.SelectedIndex + 1}");
                return;
            }
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
            "- press ctrl+s to toggle render stress mode\n" +
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
                $"count={_count} focus={(_focused ? "in" : "out")} size={_width}x{_height} mode={headerMode} input={InputModeLabel()} source={_capabilities.Source}",
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
            $"focus={_focus.ToString().ToLowerInvariant()} filter='{_actionList.Filter}' stress={ToYesNo(_stressMode)} page=dashboard input={InputModeLabel()}",
        };
        footerLines.AddRange(helpText.Split('\n'));

        TWidgets.DrawPanel(
            canvas,
            footerRect,
            _focus == WorkspaceFocus.Command ? $"Command * [{InputModeLabel()}]" : $"Command [{InputModeLabel()}]",
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

        if (_width < 76 || _height < 22)
        {
            var compact =
                "TeaSharp Capability Showcase\n\n" +
                "Terminal too small for showcase mode.\n" +
                "Resize to at least 76x22.\n\n" +
                "Press 1 protocol, 2 dashboard, 3 showcase.";
            return WarningStyle.Render(compact);
        }

        var canvas = new Canvas(_width, _height, CanvasTextMode.GraphemeAware);
        const int headerHeight = 3;
        const int footerHeight = 6;
        UiWidgets.DrawStatusBar(
            canvas,
            new Rect(0, 0, _width, 1),
            "TeaSharp Capability Showcase",
            $"tab={_showcaseTabs.SelectedIndex + 1} focus={_focus.ToString().ToLowerInvariant()} pane={ShowcasePaneLabel()} mode={ShowcaseModeLabel()} input={InputModeLabel()}");
        UiWidgets.DrawBreadcrumb(
            canvas,
            new Rect(0, 1, _width, 1),
            ["TeaSharp", "Showcase", _showcaseTabs.Tabs[_showcaseTabs.SelectedIndex]]);
        canvas.DrawHorizontalLine(0, 2, _width, '─');

        var bodyRect = new Rect(0, headerHeight, _width, _height - headerHeight - footerHeight);
        var (leftRect, rightRect) = Layout.SplitVertical(bodyRect, Math.Max(34, (bodyRect.Width * 40) / 100), minFirst: 28, minSecond: 34);
        var (leftTopRect, leftBottomRect) = Layout.SplitHorizontal(leftRect, Math.Max(12, (leftRect.Height * 62) / 100), minFirst: 10, minSecond: 7);
        var (actionsRect, leftStatusRect) = Layout.SplitHorizontal(leftTopRect, Math.Max(8, leftTopRect.Height - 9), minFirst: 6, minSecond: 5);

        var selectedActionLine = RenderActionsTable(canvas, actionsRect, _focus == WorkspaceFocus.Actions ? "Actions *" : "Actions");
        RenderShowcaseLeftStatus(canvas, leftStatusRect);

        _logViewport.Resize(Math.Max(12, leftBottomRect.Width - 2), Math.Max(3, leftBottomRect.Height - 2));
        var logLines = _logViewport.RenderLines();
        TWidgets.DrawPanel(
            canvas,
            leftBottomRect,
            _focus == WorkspaceFocus.Log ? "Log *" : "Log",
            [.. logLines]);

        _showcaseTabs.Render(canvas, new Rect(rightRect.X, rightRect.Y, rightRect.Width, 1));
        var rightBody = new Rect(rightRect.X, rightRect.Y + 1, rightRect.Width, Math.Max(0, rightRect.Height - 1));
        switch (_showcaseTabs.SelectedIndex)
        {
            case 0:
                RenderShowcaseOverview(canvas, rightBody, _focus == WorkspaceFocus.Showcase);
                break;
            case 1:
                RenderShowcaseData(canvas, rightBody, _focus == WorkspaceFocus.Showcase);
                break;
            default:
                RenderShowcaseForms(canvas, rightBody, _focus == WorkspaceFocus.Showcase);
                break;
        }

        var toastWidth = Math.Min(42, rightBody.Width);
        var toastRect = new Rect(rightBody.Right - toastWidth, rightBody.Y, toastWidth, Math.Min(9, rightBody.Height));
        _showcaseToasts.Render(canvas, toastRect);
        _showcaseModal.Render(canvas, bodyRect);

        if (_focus == WorkspaceFocus.Actions)
        {
            DrawFocusChrome(canvas, actionsRect);
            if (selectedActionLine.HasValue)
            {
                DrawSelectedRowMarkers(canvas, actionsRect, selectedActionLine.Value);
            }
        }
        else if (_focus == WorkspaceFocus.Log)
        {
            DrawFocusChrome(canvas, leftBottomRect);
        }
        else if (_focus == WorkspaceFocus.Showcase)
        {
            DrawFocusChrome(canvas, rightRect);
            var paneRect = ActiveShowcasePaneRect(rightBody);
            DrawFocusChrome(canvas, paneRect);
        }

        var footerRect = new Rect(0, _height - footerHeight, _width, footerHeight);
        var inputFrame = _commandInput.BuildFrame(Math.Max(12, _width - 6));
        var inputLine = $"> {inputFrame.Text}";

        var activeBindings = _focus switch
        {
            WorkspaceFocus.Actions => _listKeys.HelpBindings,
            WorkspaceFocus.Log => _viewportKeys.HelpBindings,
            WorkspaceFocus.Showcase => _showcaseHelp,
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
            $"focus={_focus.ToString().ToLowerInvariant()} filter='{_actionList.Filter}' stress={ToYesNo(_stressMode)} page=showcase class={Layout.Classify(_width).ToString().ToLowerInvariant()} mode={ShowcaseModeLabel()} input={InputModeLabel()}",
        };
        footerLines.AddRange(helpText.Split('\n'));

        TWidgets.DrawPanel(
            canvas,
            footerRect,
            _focus == WorkspaceFocus.Command ? $"Command * [{InputModeLabel()}]" : $"Command [{InputModeLabel()}]",
            footerLines);

        if (_focus == WorkspaceFocus.Command)
        {
            DrawFocusChrome(canvas, footerRect);
        }

        if (_focus == WorkspaceFocus.Command)
        {
            cursorX = Math.Clamp(footerRect.X + 3 + inputFrame.CursorColumn, footerRect.X + 1, footerRect.Right - 2);
            cursorY = Math.Clamp(footerRect.Y + 1, footerRect.Y + 1, footerRect.Bottom - 2);
        }

        var rendered = canvas.Render();
        return ApplyWorkspaceStyles(rendered, footerRect.Y + 1, inputFrame.PlaceholderVisible);
    }

    private int? RenderActionsTable(Canvas canvas, Rect rect, string title)
    {
        if (rect.IsEmpty || rect.Height < 4)
        {
            return null;
        }

        var visibleRows = _actionList.VisibleRows();
        var rows = new List<IReadOnlyList<string>>(visibleRows.Count);
        var selected = -1;
        for (var i = 0; i < visibleRows.Count; i++)
        {
            var row = visibleRows[i];
            if (row.Selected)
            {
                selected = i;
            }

            rows.Add([row.Item.Name, row.Item.Shortcut, ActionState(row.Item)]);
        }

        TWidgets.DrawTable(canvas, rect, ["Action", "Key", "State"], rows, selected, title);
        if (selected < 0)
        {
            return null;
        }

        var selectedY = rect.Y + 3 + selected;
        if (selectedY >= rect.Bottom - 1)
        {
            return null;
        }

        return selectedY;
    }

    private void RenderShowcaseLeftStatus(Canvas canvas, Rect rect)
    {
        if (rect.IsEmpty)
        {
            return;
        }

        var split = Math.Max(4, rect.Height - 3);
        var (capRect, gaugeRect) = Layout.SplitHorizontal(rect, split, minFirst: 4, minSecond: 3);
        _capabilityCard.Render(canvas, capRect);
        _countGauge.Render(canvas, gaugeRect);
    }

    private void RenderShowcaseOverview(Canvas canvas, Rect rect, bool showcaseFocused)
    {
        if (rect.IsEmpty || rect.Height < 10)
        {
            return;
        }

        var (top, bottom) = Layout.SplitHorizontal(rect, Math.Max(7, rect.Height / 2), minFirst: 6, minSecond: 4);
        var (unicodeRect, timelineRect) = Layout.SplitVertical(top, Math.Max(20, top.Width / 2), minFirst: 18, minSecond: 16);
        var (calendarRect, treeRect) = Layout.SplitVertical(bottom, Math.Max(20, bottom.Width / 2), minFirst: 18, minSecond: 16);

        _unicodeShowcase.CapabilitySource = _showcaseSourceSnapshot;
        _unicodeShowcase.Focus = _focused;
        _unicodeShowcase.LastPaste = _lastPaste;
        _unicodeShowcase.TypedPreview = SanitizePreview(_typedText);
        _unicodeShowcase.Count = _showcaseCountSnapshot;
        _unicodeShowcase.Title = IsFocusedShowcasePane(showcaseFocused, ShowcasePane.OverviewUnicode)
            ? "Unicode + Runtime *"
            : "Unicode + Runtime";
        _unicodeShowcase.Render(canvas, unicodeRect);

        TimelineEntry[] timeline =
        [
            new($"{_showcaseTickSnapshot:0000}", $"event {_showcaseLastEvent}"),
            new($"{_showcaseCountSnapshot:+#;-#;0}", $"count now {_showcaseCountSnapshot}"),
            new($"{_showcaseWidthSnapshot}x{_showcaseHeightSnapshot}", $"viewport {Layout.Classify(_showcaseWidthSnapshot)}"),
            new("caps", $"src {_showcaseSourceSnapshot}"),
        ];
        UiWidgets.DrawTimeline(
            canvas,
            timelineRect,
            timeline,
            IsFocusedShowcasePane(showcaseFocused, ShowcasePane.OverviewTimeline) ? "Timeline *" : "Timeline");

        UiWidgets.DrawCalendar(
            canvas,
            calendarRect,
            DateTime.Now,
            IsFocusedShowcasePane(showcaseFocused, ShowcasePane.OverviewCalendar) ? "Calendar *" : "Calendar");
        TreeNode[] nodes =
        [
            new("Core", 0),
            new("Runtime", 1),
            new("Input Decoder", 2),
            new("Renderer", 2),
            new("Widgets", 1),
            new("UiKit", 2, Selected: true),
        ];
        UiWidgets.DrawTree(
            canvas,
            treeRect,
            nodes,
            IsFocusedShowcasePane(showcaseFocused, ShowcasePane.OverviewArchitecture) ? "Architecture *" : "Architecture");
    }

    private void RenderShowcaseData(Canvas canvas, Rect rect, bool showcaseFocused)
    {
        if (rect.IsEmpty || rect.Height < 10)
        {
            return;
        }

        var (top, bottom) = Layout.SplitHorizontal(rect, Math.Max(7, (rect.Height * 42) / 100), minFirst: 6, minSecond: 4);
        var (lineRect, barRect) = Layout.SplitVertical(top, Math.Max(22, (top.Width * 62) / 100), minFirst: 20, minSecond: 14);
        var (tableRect, skeletonRect) = Layout.SplitVertical(bottom, Math.Max(24, (bottom.Width * 68) / 100), minFirst: 22, minSecond: 10);

        _throughputChart.Title = IsFocusedShowcasePane(showcaseFocused, ShowcasePane.DataLineChart) ? "Throughput *" : "Throughput";
        _statusChart.Title = IsFocusedShowcasePane(showcaseFocused, ShowcasePane.DataBarChart) ? "Status Mix *" : "Status Mix";
        _showcaseTable.Title = IsFocusedShowcasePane(showcaseFocused, ShowcasePane.DataTable) ? "Metrics Table *" : "Metrics Table";
        _throughputChart.Render(canvas, lineRect);
        _statusChart.Render(canvas, barRect);
        _showcaseTable.PageSize = Math.Max(1, tableRect.Height - 3);
        _showcaseTable.Render(canvas, tableRect);
        UiWidgets.DrawSkeleton(
            canvas,
            skeletonRect,
            IsFocusedShowcasePane(showcaseFocused, ShowcasePane.DataSkeleton) ? "Frame Buffer *" : "Frame Buffer");
    }

    private void RenderShowcaseForms(Canvas canvas, Rect rect, bool showcaseFocused)
    {
        if (rect.IsEmpty || rect.Height < 10)
        {
            return;
        }

        var cells = Layout.Grid(rect, 2, 2);
        _showcaseAccordion.Title = IsFocusedShowcasePane(showcaseFocused, ShowcasePane.FormsPlaybook) ? "Playbook *" : "Playbook";
        _showcaseChecklist.Title = IsFocusedShowcasePane(showcaseFocused, ShowcasePane.FormsChecklist) ? "Checklist *" : "Checklist";
        _showcaseAccordion.Render(canvas, cells[0]);
        _showcaseChecklist.Render(canvas, cells[1]);

        var (radioRect, selectRect) = Layout.SplitHorizontal(cells[2], Math.Max(4, cells[2].Height - 4), minFirst: 4, minSecond: 3);
        _showcaseTheme.Title = IsFocusedShowcasePane(showcaseFocused, ShowcasePane.FormsTheme) ? "Theme *" : "Theme";
        _showcaseDensity.Title = IsFocusedShowcasePane(showcaseFocused, ShowcasePane.FormsDensity) ? "Density *" : "Density";
        _showcaseTheme.Render(canvas, radioRect);
        _showcaseDensity.Render(canvas, selectRect);

        var summaryLines = new List<string>
        {
            $"theme: {_showcaseTheme.SelectedIndex + 1}",
            $"density: {_showcaseDensity.SelectedIndex + 1}",
            $"table sort: {(_showcaseTable.SortDescending ? "desc" : "asc")}",
            "hotkeys: t,m,a,z,r,f,c,v,[,],left,right,p/P",
        };
        TWidgets.DrawCard(
            canvas,
            cells[3],
            IsFocusedShowcasePane(showcaseFocused, ShowcasePane.FormsSummary) ? "Summary *" : "Summary",
            summaryLines);
    }

    private Rect ActiveShowcasePaneRect(Rect rightBody)
    {
        if (rightBody.IsEmpty || rightBody.Height < 3)
        {
            return new Rect(0, 0, 0, 0);
        }

        return _showcaseTabs.SelectedIndex switch
        {
            0 => ActiveOverviewPaneRect(rightBody),
            1 => ActiveDataPaneRect(rightBody),
            _ => ActiveFormsPaneRect(rightBody),
        };
    }

    private Rect ActiveOverviewPaneRect(Rect rect)
    {
        var (top, bottom) = Layout.SplitHorizontal(rect, Math.Max(7, rect.Height / 2), minFirst: 6, minSecond: 4);
        var (unicodeRect, timelineRect) = Layout.SplitVertical(top, Math.Max(20, top.Width / 2), minFirst: 18, minSecond: 16);
        var (calendarRect, treeRect) = Layout.SplitVertical(bottom, Math.Max(20, bottom.Width / 2), minFirst: 18, minSecond: 16);
        return _showcasePane switch
        {
            ShowcasePane.OverviewUnicode => unicodeRect,
            ShowcasePane.OverviewTimeline => timelineRect,
            ShowcasePane.OverviewCalendar => calendarRect,
            _ => treeRect,
        };
    }

    private Rect ActiveDataPaneRect(Rect rect)
    {
        var (top, bottom) = Layout.SplitHorizontal(rect, Math.Max(7, (rect.Height * 42) / 100), minFirst: 6, minSecond: 4);
        var (lineRect, barRect) = Layout.SplitVertical(top, Math.Max(22, (top.Width * 62) / 100), minFirst: 20, minSecond: 14);
        var (tableRect, skeletonRect) = Layout.SplitVertical(bottom, Math.Max(24, (bottom.Width * 68) / 100), minFirst: 22, minSecond: 10);
        return _showcasePane switch
        {
            ShowcasePane.DataLineChart => lineRect,
            ShowcasePane.DataBarChart => barRect,
            ShowcasePane.DataTable => tableRect,
            _ => skeletonRect,
        };
    }

    private Rect ActiveFormsPaneRect(Rect rect)
    {
        var cells = Layout.Grid(rect, 2, 2);
        var (radioRect, selectRect) = Layout.SplitHorizontal(cells[2], Math.Max(4, cells[2].Height - 4), minFirst: 4, minSecond: 3);
        return _showcasePane switch
        {
            ShowcasePane.FormsPlaybook => cells[0],
            ShowcasePane.FormsChecklist => cells[1],
            ShowcasePane.FormsTheme => radioRect,
            ShowcasePane.FormsDensity => selectRect,
            _ => cells[3],
        };
    }

    private static void DrawFocusChrome(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Width < 4 || clipped.Height < 3)
        {
            return;
        }

        var innerLeft = clipped.X + 1;
        var innerRight = clipped.Right - 2;
        for (var y = clipped.Y + 1; y < clipped.Bottom - 1; y++)
        {
            canvas.Set(innerLeft, y, '▌');
            canvas.Set(innerRight, y, '▐');
        }

        canvas.Set(clipped.X + 2, clipped.Y, '◆');
    }

    private static void DrawSelectedRowMarkers(Canvas canvas, Rect rect, int rowY)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || rowY <= clipped.Y || rowY >= clipped.Bottom - 1)
        {
            return;
        }

        if (clipped.Width >= 4)
        {
            canvas.Set(clipped.X + 1, rowY, '▶');
            canvas.Set(clipped.Right - 2, rowY, '◀');
        }
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
            rows[i] = rows[i].Replace("› ", AccentStyle.Render("› "), StringComparison.Ordinal);

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
        _focus = _page == AppPage.Showcase
            ? _focus switch
            {
                WorkspaceFocus.Actions => WorkspaceFocus.Showcase,
                WorkspaceFocus.Showcase => WorkspaceFocus.Log,
                WorkspaceFocus.Log => WorkspaceFocus.Command,
                _ => WorkspaceFocus.Actions,
            }
            : _focus switch
            {
                WorkspaceFocus.Actions => WorkspaceFocus.Log,
                WorkspaceFocus.Log => WorkspaceFocus.Command,
                _ => WorkspaceFocus.Actions,
            };
    }

    private bool IsFocusedShowcasePane(bool showcaseFocused, ShowcasePane pane)
    {
        return showcaseFocused && _showcasePane == pane;
    }

    private int ShowcasePaneCount()
    {
        return _showcaseTabs.SelectedIndex switch
        {
            0 => 4,
            1 => 4,
            _ => 5,
        };
    }

    private void EnsureShowcasePaneInRange()
    {
        var max = ShowcasePaneCount() - 1;
        var current = Math.Clamp((int)_showcasePane, 0, max);
        _showcasePane = (ShowcasePane)current;
    }

    private void MoveShowcasePane(int delta)
    {
        var count = ShowcasePaneCount();
        if (count <= 0)
        {
            return;
        }

        var index = ((int)_showcasePane + delta) % count;
        if (index < 0)
        {
            index += count;
        }

        _showcasePane = (ShowcasePane)index;
    }

    private string ShowcasePaneLabel()
    {
        return _showcaseTabs.SelectedIndex switch
        {
            0 => _showcasePane switch
            {
                ShowcasePane.OverviewUnicode => "unicode",
                ShowcasePane.OverviewTimeline => "timeline",
                ShowcasePane.OverviewCalendar => "calendar",
                _ => "architecture",
            },
            1 => _showcasePane switch
            {
                ShowcasePane.DataLineChart => "line-chart",
                ShowcasePane.DataBarChart => "bar-chart",
                ShowcasePane.DataTable => "table",
                _ => "skeleton",
            },
            _ => _showcasePane switch
            {
                ShowcasePane.FormsPlaybook => "playbook",
                ShowcasePane.FormsChecklist => "checklist",
                ShowcasePane.FormsTheme => "theme",
                ShowcasePane.FormsDensity => "density",
                _ => "summary",
            },
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
    Showcase = 2,
    Command = 3,
}

internal enum WorkspaceInputMode
{
    Navigate = 0,
    Command = 1,
}

internal enum ShowcasePane
{
    OverviewUnicode = 0,
    OverviewTimeline = 1,
    OverviewCalendar = 2,
    OverviewArchitecture = 3,
    DataLineChart = 0,
    DataBarChart = 1,
    DataTable = 2,
    DataSkeleton = 3,
    FormsPlaybook = 0,
    FormsChecklist = 1,
    FormsTheme = 2,
    FormsDensity = 3,
    FormsSummary = 4,
}
