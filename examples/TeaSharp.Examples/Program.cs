using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.View;

var terminal = new TeaSharp.Core.Terminal.ConsoleTerminalAdapter();
var model = new CounterModel(terminal);
var options = new ProgramOptions
{
    UseConsoleKeyEvents = false,
    Terminal = terminal,
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
    private readonly TeaSharp.Core.Terminal.ConsoleTerminalAdapter _terminal;

    private int _count;
    private int _width = 80;
    private int _height = 24;
    private bool _focused = true;
    private string _lastEvent = "none";
    private string _lastPaste = "(none)";
    private string _typedText = string.Empty;
    private bool _stressMode;
    private int _pulseCount;

    public CounterModel(TeaSharp.Core.Terminal.ConsoleTerminalAdapter terminal)
    {
        _terminal = terminal;
    }

    public Command? Init() => null;

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
            else if (key.Text == "q"
                     || ((key.Text == "c" || key.Text == "\u0003") && key.Modifiers.HasFlag(KeyModifiers.Ctrl)))
            {
                return new UpdateResult(this, Tea.Cmd.Quit);
            }
            else if (key.Text == "s" && key.Modifiers == KeyModifiers.None)
            {
                _stressMode = !_stressMode;
                _lastEvent = $"stress: {(_stressMode ? "on" : "off")}";
                return new UpdateResult(this, _stressMode ? NextPulseCommand() : null);
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

        if (message is RenderPulseMsg)
        {
            if (!_stressMode)
            {
                return new UpdateResult(this, null);
            }

            _pulseCount++;
            return new UpdateResult(this, NextPulseCommand());
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
        var content =
            "TeaSharp Protocol Probe\n\n" +
            $"Count: {_count}\n" +
            $"Focus: {(_focused ? "in" : "out")}\n" +
            $"Size: {_width}x{_height}\n" +
            $"Raw mode active: {(_terminal.IsRawModeActive ? "yes" : "no")}\n" +
            $"Raw mode probe: {SummarizeProbe(_terminal.RawModeDiagnostics)}\n" +
            $"Raw mode error: {SummarizeProbe(_terminal.RawModeError)}\n" +
            $"Input backend: {(_terminal.IsRawModeActive ? "vt-bytes" : "console-keys-fallback")}\n" +
            $"Focus events: {(_terminal.IsRawModeActive ? "expected (if terminal supports ?1004)" : "not available in fallback mode")}\n" +
            $"Mouse events: {(_terminal.IsRawModeActive ? "expected (if terminal supports ?1006)" : "not available in fallback mode")}\n" +
            $"Stress mode: {(_stressMode ? "on" : "off")} (pulses: {_pulseCount})\n" +
            $"Last event: {_lastEvent}\n" +
            $"Last paste: {_lastPaste}\n" +
            $"Typed text: {SanitizePastePreview(_typedText)}\n\n" +
            "Try live:\n" +
            "- up/down to change count\n" +
            "- move/click mouse in terminal window\n" +
            "- press s to toggle render stress mode\n" +
            "- type text; backspace and enter work\n" +
            "- paste multi-line text (cmd+v/ctrl+v/right-click)\n" +
            "- switch terminal focus away/back\n" +
            "- resize terminal window\n" +
            "- q or ctrl+c to quit\n";

        return ModelView.From(content) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            MouseMode = MouseMode.AllMotion,
            WindowTitle = "TeaSharp Protocol Probe",
        };
    }

    private static Command NextPulseCommand()
    {
        return Tea.Cmd.Tick(TimeSpan.FromMilliseconds(35), _ => new RenderPulseMsg());
    }

    private static string SanitizePastePreview(string content)
    {
        var sanitized = content
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("\t", "\\t", StringComparison.Ordinal);

        return sanitized.Length <= 72
            ? sanitized
            : sanitized[..72] + "...";
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
}

internal sealed record RenderPulseMsg : IMessage;
