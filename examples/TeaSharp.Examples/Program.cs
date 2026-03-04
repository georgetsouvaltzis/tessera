using TeaSharp;
using TeaSharp.Components;
using TeaSharp.Styles;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
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
        .WithForeground(AnsiColor.BrightCyan);

    private static readonly TeaStyle AccentStyle = TeaStyle.Empty
        .WithBold()
        .WithForeground(AnsiColor.BrightGreen);

    private static readonly TeaStyle MutedStyle = TeaStyle.Empty
        .WithForeground(AnsiColor.Indexed(245));

    private static readonly TeaStyle WarningStyle = TeaStyle.Empty
        .WithBold()
        .WithForeground(AnsiColor.Indexed(214));

    private readonly TeaSharp.Core.Terminal.ConsoleTerminalAdapter _terminal;
    private TerminalCapabilityProfile _capabilities = TerminalCapabilityProfile.AllSupported;
    private readonly string _resizeBackend;
    private readonly List<int> _sparkline = [];
    private readonly string[] _actions =
    [
        "Inspect capabilities",
        "Toggle stress mode",
        "Switch page",
        "Quit",
    ];

    private int _count;
    private int _width = 80;
    private int _height = 24;
    private bool _focused = true;
    private string _lastEvent = "none";
    private string _lastPaste = "(none)";
    private string _typedText = string.Empty;
    private bool _stressMode;
    private int _tickCount;
    private int _selectedAction;
    private bool _dashboardMode = true;
    private readonly Dictionary<int, ModeReportState> _modeReports = [];

    public CounterModel(
        TeaSharp.Core.Terminal.ConsoleTerminalAdapter terminal)
    {
        _terminal = terminal;
        _resizeBackend = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux()
            ? "signal+poll"
            : "poll";
    }

    public Command? Init() => NextTickCommand(_stressMode);

    public UpdateResult Update(IMessage message)
    {
        if (message is KeyPressMsg key)
        {
            if (key.Code == KeyCode.Up)
            {
                _count++;
            }
            else if (key.Code == KeyCode.Down)
            {
                _count--;
            }
            else if (key.Code == KeyCode.Tab)
            {
                _selectedAction = (_selectedAction + 1) % _actions.Length;
            }
            else if (key.Code == KeyCode.Enter && _dashboardMode)
            {
                return ExecuteSelectedAction();
            }
            else if (key.Text == "q"
                     || ((key.Text == "c" || key.Text == "\u0003") && key.Modifiers.HasFlag(KeyModifiers.Ctrl)))
            {
                return new UpdateResult(this, Tea.Cmd.Quit);
            }
            else if (key.Text == "1")
            {
                _dashboardMode = false;
            }
            else if (key.Text == "2")
            {
                _dashboardMode = true;
            }
            else if (key.Text == "s" && key.Modifiers == KeyModifiers.None)
            {
                _stressMode = !_stressMode;
                _lastEvent = $"stress: {(_stressMode ? "on" : "off")}";
                return new UpdateResult(this, NextTickCommand(_stressMode));
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

        if (message is PasteMsg paste)
        {
            _lastPaste = SanitizePastePreview(paste.Content);
            _typedText += paste.Content;
            _lastEvent = $"paste: {paste.Content.Length} chars";
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
            return new UpdateResult(this, null);
        }

        if (message is DashboardTickMsg)
        {
            _tickCount++;
            AppendSparkSample();
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
        var content = _dashboardMode
            ? BuildDashboardView()
            : BuildProbeView();

        return ModelView.From(content) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            EnableSynchronizedUpdates = true,
            MouseMode = MouseMode.AllMotion,
            WindowTitle = _dashboardMode
                ? "TeaSharp Component Dashboard"
                : "TeaSharp Protocol Probe",
        };
    }

    private string BuildProbeView()
    {
        static string Label(string text) => HeaderStyle.Render(text);
        static string Hint(string text) => MutedStyle.Render(text);

        return
            $"{HeaderStyle.WithForeground(AnsiColor.BrightWhite).Render("TeaSharp Protocol Probe")}\n\n" +
            $"{Label("Count:")} {_count}\n" +
            $"{Label("Focus:")} {(_focused ? "in" : "out")}\n" +
            $"{Label("Size:")} {_width}x{_height}\n" +
            $"{Label("Raw mode active:")} {(_terminal.IsRawModeActive ? "yes" : "no")}\n" +
            $"{Label("Raw mode probe:")} {SummarizeProbe(_terminal.RawModeDiagnostics)}\n" +
            $"{Label("Raw mode error:")} {SummarizeProbe(_terminal.RawModeError)}\n" +
            $"{Label("Input backend:")} {(_terminal.IsRawModeActive ? "vt-bytes" : "console-keys-fallback")}\n" +
            $"{Label("Capabilities source:")} {_capabilities.Source}\n" +
            $"{Label("Capabilities:")} focus={ToYesNo(_capabilities.FocusReporting)} mouse={ToYesNo(_capabilities.MouseReporting)} paste={ToYesNo(_capabilities.BracketedPaste)} sync={ToYesNo(_capabilities.SynchronizedUpdates)} decrpm={ToYesNo(_capabilities.ModeReports)}\n" +
            $"{Label("Focus events:")} {(_terminal.IsRawModeActive ? "expected (if terminal supports ?1004)" : "not available in fallback mode")}\n" +
            $"{Label("Mouse events:")} {(_terminal.IsRawModeActive ? "expected (if terminal supports ?1006)" : "not available in fallback mode")}\n" +
            $"{Label("Synchronized updates:")} requested (?2026)\n" +
            $"{Label("Mode reports (DECRPM current-state):")}\n" +
            $"{FormatModeReports()}\n" +
            $"{Label("Resize backend:")} {_resizeBackend}\n" +
            $"{Label("Stress mode:")} {(_stressMode ? "on" : "off")} (ticks: {_tickCount})\n" +
            $"{Label("Last event:")} {_lastEvent}\n" +
            $"{Label("Last paste:")} {_lastPaste}\n" +
            $"{Label("Typed length:")} {_typedText.Length}\n" +
            $"{Label("Typed text:")} {SanitizePastePreview(_typedText)}\n\n" +
            $"{Hint("Try live:")}\n" +
            "- press 2 to open dashboard view\n" +
            "- up/down to change count\n" +
            "- move/click mouse in terminal window\n" +
            "- press s to toggle render stress mode\n" +
            "- type text; backspace and enter work\n" +
            "- try alt+letter or ctrl+shift+letter to verify enhanced key decode\n" +
            "- paste multi-line text (cmd+v/ctrl+v/right-click)\n" +
            "- hold a key or paste large text; events should stay responsive\n" +
            "- switch terminal focus away/back\n" +
            "- resize terminal window\n" +
            "- q or ctrl+c to quit\n";
    }

    private string BuildDashboardView()
    {
        if (_width < 40 || _height < 16)
        {
            var compactView =
                "TeaSharp Component Dashboard\n\n" +
                "Terminal too small for dashboard components.\n" +
                "Resize to at least 40x16.\n\n" +
                "Press 1 for protocol view or q to quit.";

            return WarningStyle.Render(compactView);
        }

        var canvas = new Canvas(_width, _height);
        var headerHeight = 3;
        var footerHeight = 4;
        var bodyTop = headerHeight;
        var bodyHeight = _height - headerHeight - footerHeight;
        var leftWidth = Math.Max(24, _width / 2);
        var rightWidth = _width - leftWidth;

        Widgets.DrawPanel(
            canvas,
            new Rect(0, 0, _width, headerHeight),
            "TeaSharp Dashboard",
            [
                $"count={_count} focus={(_focused ? "in" : "out")} size={_width}x{_height}  mode={(_dashboardMode ? "dashboard" : "probe")}  source={_capabilities.Source}",
            ]);

        Widgets.DrawPanel(
            canvas,
            new Rect(0, bodyTop, leftWidth, bodyHeight),
            "System",
            [
                $"raw mode: {ToYesNo(_terminal.IsRawModeActive)}",
                $"backend: {(_terminal.IsRawModeActive ? "vt-bytes" : "console-fallback")}",
                $"focus support: {ToYesNo(_capabilities.FocusReporting)}",
                $"mouse support: {ToYesNo(_capabilities.MouseReporting)}",
                $"paste support: {ToYesNo(_capabilities.BracketedPaste)}",
                $"sync support: {ToYesNo(_capabilities.SynchronizedUpdates)}",
                $"mode reports: {ToYesNo(_capabilities.ModeReports)}",
                $"stress mode: {ToYesNo(_stressMode)} ({_tickCount} ticks)",
                $"last event: {Truncate(_lastEvent, Math.Max(6, leftWidth - 4))}",
            ]);

        var gauge = Math.Clamp((_count + 20) / 40.0, 0.0, 1.0);
        Widgets.DrawProgressBar(
            canvas,
            new Rect(2, bodyTop + 10, Math.Max(10, leftWidth - 4), 2),
            gauge,
            $"count gauge: {_count}");

        var cardHeight = Math.Min(7, Math.Max(5, bodyHeight / 3));
        var sparkHeight = 3;
        var tableTop = bodyTop + cardHeight + sparkHeight;
        var tableHeight = bodyTop + bodyHeight - tableTop;
        if (tableHeight < 4)
        {
            tableTop = bodyTop + cardHeight + 2;
            tableHeight = bodyTop + bodyHeight - tableTop;
        }

        Widgets.DrawCard(
            canvas,
            new Rect(leftWidth, bodyTop, rightWidth, cardHeight),
            "Components",
            [
                "styles, sparkline, cards, table",
                "tab cycles action, enter executes",
                "1 protocol view, 2 dashboard view",
            ]);

        Widgets.DrawPanel(
            canvas,
            new Rect(leftWidth, bodyTop + cardHeight, rightWidth, sparkHeight),
            "Throughput",
            [string.Empty]);

        Widgets.DrawSparkline(
            canvas,
            new Rect(leftWidth + 2, bodyTop + cardHeight + 1, Math.Max(8, rightWidth - 4), 1),
            _sparkline,
            minValue: 0,
            maxValue: 100);

        if (tableHeight >= 4)
        {
            Widgets.DrawTable(
                canvas,
                new Rect(leftWidth, tableTop, rightWidth, tableHeight),
                ["Action", "Key", "State"],
                BuildActionRows(),
                selectedRow: _selectedAction,
                title: "Actions");
        }
        else
        {
            Widgets.DrawList(
                canvas,
                new Rect(leftWidth + 2, bodyTop + cardHeight + sparkHeight, Math.Max(12, rightWidth - 4), Math.Max(3, bodyHeight - cardHeight - sparkHeight - 2)),
                _actions,
                _selectedAction);
        }

        Widgets.DrawPanel(
            canvas,
            new Rect(0, _height - footerHeight, _width, footerHeight),
            "Live Event",
            [
                $"last: {Truncate(_lastEvent, Math.Max(8, _width - 8))}",
                $"paste: {Truncate(_lastPaste, Math.Max(8, _width - 8))}",
            ]);

        return ApplyDashboardStyles(canvas.Render());
    }

    private UpdateResult ExecuteSelectedAction()
    {
        switch (_selectedAction)
        {
            case 0:
                _lastEvent = $"capabilities: {_capabilities.Source}";
                return new UpdateResult(this, null);
            case 1:
                _stressMode = !_stressMode;
                _lastEvent = $"stress: {(_stressMode ? "on" : "off")}";
                return new UpdateResult(this, NextTickCommand(_stressMode));
            case 2:
                _dashboardMode = !_dashboardMode;
                _lastEvent = $"view: {(_dashboardMode ? "dashboard" : "probe")}";
                return new UpdateResult(this, null);
            case 3:
                return new UpdateResult(this, Tea.Cmd.Quit);
            default:
                return new UpdateResult(this, null);
        }
    }

    private Command NextTickCommand(bool stressMode)
    {
        var delay = stressMode ? TimeSpan.FromMilliseconds(35) : TimeSpan.FromMilliseconds(160);
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

    private IReadOnlyList<IReadOnlyList<string>> BuildActionRows()
    {
        return
        [
            new[] { "Inspect capabilities", "tab/enter", _selectedAction == 0 ? "selected" : "ready" },
            new[] { "Toggle stress mode", "s", _stressMode ? "on" : "off" },
            new[] { "Switch page", "1/2", _dashboardMode ? "dashboard" : "probe" },
            new[] { "Quit", "q/ctrl+c", "ready" },
        ];
    }

    private static string ApplyDashboardStyles(string frame)
    {
        var rows = frame.Split('\n');
        if (rows.Length == 0)
        {
            return frame;
        }

        rows[0] = HeaderStyle.WithForeground(AnsiColor.BrightWhite).Render(rows[0]);
        rows[^1] = MutedStyle.Render(rows[^1]);

        for (var i = 0; i < rows.Length; i++)
        {
            if (rows[i].Contains("› ", StringComparison.Ordinal))
            {
                rows[i] = AccentStyle.Render(rows[i]);
            }

            if (rows[i].Contains("raw mode: no", StringComparison.Ordinal))
            {
                rows[i] = WarningStyle.Render(rows[i]);
            }
        }

        return string.Join('\n', rows);
    }

    private static string SanitizePastePreview(string content)
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

    private static string ToYesNo(bool value)
    {
        return value ? "yes" : "no";
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value) || maxLength <= 0)
        {
            return string.Empty;
        }

        return value.Length <= maxLength
            ? value
            : value[..maxLength] + "...";
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
