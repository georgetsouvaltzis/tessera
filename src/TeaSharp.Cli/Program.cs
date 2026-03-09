using System.Text.Json;

var argsList = args.ToList();
if (argsList.Count == 0 || string.Equals(argsList[0], "wizard", StringComparison.OrdinalIgnoreCase))
{
    return await WizardRunner.RunAsync();
}

if (argsList[0] is "-h" or "--help" or "help")
{
    PrintUsage();
    return 0;
}

Console.Error.WriteLine($"Unknown command '{argsList[0]}'.");
PrintUsage();
return 1;

static void PrintUsage()
{
    Console.WriteLine("TeaSharp CLI");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project src/TeaSharp.Cli -- wizard");
    Console.WriteLine();
    Console.WriteLine("Interactive scaffold generator for TeaSharp apps.");
}

internal static class WizardRunner
{
    private static readonly ThemePreset[] Presets =
    [
        new("teasharp-default", "TeaSharp Default", "#9CDCFE", "#4FC1FF", "#8A8A8A", '.', '·'),
        new("catppuccin-latte", "Catppuccin Latte", "#1E66F5", "#8839EF", "#6C6F85", '·', '░'),
        new("catppuccin-frappe", "Catppuccin Frappe", "#8CAAEE", "#CA9EE6", "#A5ADCE", '·', '▒'),
        new("catppuccin-macchiato", "Catppuccin Macchiato", "#8AADF4", "#C6A0F6", "#A5ADCB", '·', '▓'),
        new("catppuccin-mocha", "Catppuccin Mocha", "#89B4FA", "#CBA6F7", "#A6ADC8", '·', '▓'),
        new("rosepine-main", "Rosé Pine Main", "#EBBCBA", "#C4A7E7", "#908CAA", '•', '░'),
        new("rosepine-moon", "Rosé Pine Moon", "#EA9A97", "#C4A7E7", "#908CAA", '•', '▒'),
        new("rosepine-dawn", "Rosé Pine Dawn", "#D7827E", "#907AA9", "#797593", '•', '░'),
    ];

    public static Task<int> RunAsync()
    {
        Console.WriteLine("TeaSharp Wizard");
        Console.WriteLine("Scaffold a new TeaSharp app with preset themes and configurable keybindings.");
        Console.WriteLine();

        var cwd = Directory.GetCurrentDirectory();
        var defaultName = "TeaSharp.Pomodoro";

        var appName = Prompt("App name", defaultName, ValidateIdentifier);
        var template = PromptChoice(
            "Template",
            ["pomodoro", "dashboard"],
            defaultIndex: 0);

        var themeLabels = Presets.Select(static x => x.DisplayName).ToArray();
        var themeIndex = PromptChoiceIndex("Theme preset", themeLabels, defaultIndex: 0);
        var preset = Presets[themeIndex];

        Console.WriteLine();
        Console.WriteLine("Key bindings (single key like ':' or 'b', or named key: esc/enter/space)");
        var commandKey = Prompt("Command mode key", ":", ValidateBindingToken);
        var toastKey = Prompt("Toast key", "t", ValidateBindingToken);
        var modalKey = Prompt("Modal key", "m", ValidateBindingToken);

        var outputDefault = Path.Combine(cwd, appName);
        var outputDir = Prompt("Output directory", outputDefault, static _ => true);
        outputDir = Path.GetFullPath(outputDir);

        if (Directory.Exists(outputDir) && Directory.EnumerateFileSystemEntries(outputDir).Any())
        {
            if (!PromptYesNo($"Directory '{outputDir}' is not empty. Overwrite scaffold files?", defaultYes: false))
            {
                Console.WriteLine("Aborted.");
                return Task.FromResult(1);
            }
        }

        Directory.CreateDirectory(outputDir);

        var repoRoot = FindRepoRoot(cwd);
        if (repoRoot is null)
        {
            Console.Error.WriteLine("Could not locate TeaSharp repo root (expected src/TeaSharp and src/TeaSharp.Core). Run wizard from inside the TeaSharp repository.");
            return Task.FromResult(1);
        }

        var projectName = SanitizeProjectName(appName);
        var csprojPath = Path.Combine(outputDir, $"{projectName}.csproj");
        var programPath = Path.Combine(outputDir, "Program.cs");
        var manifestPath = Path.Combine(outputDir, "teasharp.json");

        var teaSharpCsproj = Path.Combine(repoRoot, "src", "TeaSharp", "TeaSharp.csproj");
        var teaSharpCoreCsproj = Path.Combine(repoRoot, "src", "TeaSharp.Core", "TeaSharp.Core.csproj");
        var relTeaSharp = ResolveProjectReferencePath(outputDir, repoRoot, teaSharpCsproj);
        var relTeaSharpCore = ResolveProjectReferencePath(outputDir, repoRoot, teaSharpCoreCsproj);

        var csproj = BuildProjectFile(relTeaSharp, relTeaSharpCore);
        var source = template == "pomodoro"
            ? BuildPomodoroTemplate(projectName, preset, commandKey, toastKey, modalKey)
            : BuildDashboardTemplate(projectName, preset, commandKey, toastKey, modalKey);

        var manifest = new WizardManifest(
            AppName: appName,
            Template: template,
            ThemePreset: preset.Id,
            KeyBindings: new WizardKeyBindings(commandKey, toastKey, modalKey),
            GeneratedAtUtc: DateTimeOffset.UtcNow);

        File.WriteAllText(csprojPath, csproj);
        File.WriteAllText(programPath, source);
        File.WriteAllText(manifestPath, JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }) + Environment.NewLine);

        Console.WriteLine();
        Console.WriteLine("Scaffold created:");
        Console.WriteLine($"- {csprojPath}");
        Console.WriteLine($"- {programPath}");
        Console.WriteLine($"- {manifestPath}");
        Console.WriteLine();
        Console.WriteLine("Next:");
        Console.WriteLine($"  cd {QuoteIfNeeded(outputDir)}");
        Console.WriteLine($"  dotnet run --project {QuoteIfNeeded(Path.GetFileName(csprojPath))}");
        return Task.FromResult(0);
    }

    private static string BuildProjectFile(string relTeaSharp, string relTeaSharpCore)
    {
        return $$"""
<Project Sdk="Microsoft.NET.Sdk">

  <ItemGroup>
    <ProjectReference Include="{{relTeaSharp}}" />
    <ProjectReference Include="{{relTeaSharpCore}}" />
  </ItemGroup>

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
""";
    }

    private static string BuildPomodoroTemplate(string appName, ThemePreset theme, string commandKey, string toastKey, string modalKey)
    {
        var (titleR, titleG, titleB) = ParseHex(theme.TitleHex);
        var (accentR, accentG, accentB) = ParseHex(theme.AccentHex);
        var (mutedR, mutedG, mutedB) = ParseHex(theme.MutedHex);

        return $$"""
using TeaSharp;
using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using TeaSharp.Styles;
using ModelView = TeaSharp.Core.Abstractions.View;

var terminal = new ConsoleTerminalAdapter();
var options = new ProgramOptions
{
    UseConsoleKeyEvents = false,
    Terminal = terminal,
    TerminalCapabilities = TerminalCapabilityDetector.Detect(),
};

var program = Tea.NewProgram(new PomodoroModel(), options);

try
{
    await program.RunAsync();
    return 0;
}
catch (TeaProgramInterruptedException)
{
    return 130;
}

internal sealed record PomodoroTickMsg(DateTimeOffset At) : IMessage;

internal enum InputMode
{
    Navigate = 0,
    Command = 1,
}

internal sealed class PomodoroModel : IModel
{
    private const string CommandModeKey = "{{EscapeForString(commandKey)}}";
    private const string ToastKey = "{{EscapeForString(toastKey)}}";
    private const string ModalKey = "{{EscapeForString(modalKey)}}";

    private readonly TeaStyle _titleStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.Rgb({{titleR}}, {{titleG}}, {{titleB}}));
    private readonly TeaStyle _accentStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.Rgb({{accentR}}, {{accentG}}, {{accentB}}));
    private readonly TeaStyle _mutedStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb({{mutedR}}, {{mutedG}}, {{mutedB}}));

    private readonly ProgressBarComponent _progress = new() { Title = "Session Progress", Step = 0.05 };
    private readonly StatusBarComponent _status = new()
    {
        Theme = new UiTheme(StatusFill: '{{theme.StatusFill}}', ModalBackdropFill: '{{theme.ModalFill}}')
    };
    private readonly LogViewerComponent _logs = new() { Title = "Event Log", AutoScroll = true };
    private readonly TextInputComponent _commandInput = new()
    {
        Title = "Command",
        ClearOnSubmit = true,
    };
    private readonly DialogComponent _resetDialog = new()
    {
        Title = "Reset Session",
        Lines =
        [
            "Reset current timer phase?",
            "enter/space = confirm",
            "esc = cancel",
        ],
        Theme = new UiTheme(ModalBackdropFill: '{{theme.ModalFill}}'),
    };
    private readonly ToastCenterComponent _toasts = new() { MaxToasts = 2 };

    private int _width = 100;
    private int _height = 32;

    private int _focusMinutes = 25;
    private int _breakMinutes = 5;
    private int _remainingSeconds = 25 * 60;
    private bool _isBreak;
    private bool _running;
    private int _cycleCount;
    private int _tickCount;
    private InputMode _mode;
    private string _lastEvent = "ready";

    public PomodoroModel()
    {
        _commandInput.Input.Placeholder = "help | start | pause | reset | skip | focus <min> | break <min> | quit";
        _logs.Append("Pomodoro scaffold ready");
        _logs.Append($"Theme: {{theme.DisplayName}}");
        _logs.Append($"Command={{EscapeForString(commandKey)}}, toast={{EscapeForString(toastKey)}}, modal={{EscapeForString(modalKey)}}");
    }

    public Command? Init() => NextTick();

    public Command? Update(IMessage message)
    {
        switch (message)
        {
            case PomodoroTickMsg:
                _tickCount++;
                _toasts.Update(new TickMsg(DateTimeOffset.Now));
                if (_running && !_resetDialog.Visible)
                {
                    if (_remainingSeconds > 0)
                    {
                        _remainingSeconds--;
                    }

                    if (_remainingSeconds == 0)
                    {
                        AdvancePhase(logTransition: true);
                    }
                }

                return NextTick();

            case WindowSizeMsg ws:
                _width = ws.Width;
                _height = ws.Height;
                _lastEvent = $"resize:{_width}x{_height}";
                return null;

            case KeyPressMsg key:
                return HandleKey(key);

            default:
                return null;
        }
    }

    public ModelView View()
    {
        if (_width < 70 || _height < 16)
        {
            return ModelView.From("Pomodoro\n\nTerminal too small. Expand to at least 70x16.");
        }

        var canvas = new Canvas(_width, _height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var bodyRect = new Rect(0, 0, _width, _height - 1);
        var (left, right) = Layout.SplitVertical(bodyRect, Math.Max(34, bodyRect.Width / 2));
        var (sessionRect, commandRect) = Layout.SplitHorizontal(left, Math.Max(10, left.Height - 6), minFirst: 8, minSecond: 4);

        RenderSession(canvas, sessionRect);

        _commandInput.Focused = _mode == InputMode.Command;
        _commandInput.Render(canvas, commandRect);

        _logs.Focused = _mode == InputMode.Navigate;
        _logs.Render(canvas, right);

        var toastWidth = Math.Min(42, right.Width);
        var toastRect = new Rect(right.Right - toastWidth, right.Y, toastWidth, Math.Min(9, right.Height));
        _toasts.Render(canvas, toastRect);

        _resetDialog.Render(canvas, bodyRect);

        var statusLeft = $"{(_isBreak ? "break" : "focus")} {FormatClock(_remainingSeconds)} running={YesNo(_running)} mode={(_mode == InputMode.Command ? "cmd" : "nav")}";
        var statusRight = $"cmd={CommandModeKey} toast={ToastKey} modal={ModalKey} event={_lastEvent}";
        _status.LeftText = statusLeft;
        _status.RightText = statusRight;
        _status.Render(canvas, new Rect(0, _height - 1, _width, 1));

        return ModelView.From(canvas.Render()) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            MouseMode = MouseMode.AllMotion,
            WindowTitle = "{{EscapeForString(appName)}}",
        };
    }

    private Command? HandleKey(KeyPressMsg key)
    {
        if (key.Modifiers.HasFlag(KeyModifiers.Ctrl)
            && (key.IsCharacter('c') || key.IsCharacter('\u0003', ignoreCase: false)))
        {
            return Tea.Cmd.Quit;
        }

        if (_resetDialog.Visible)
        {
            _resetDialog.Focused = true;
            if (_resetDialog.Update(key))
            {
                if (_resetDialog.LastResult == DialogResult.Accepted)
                {
                    ResetCurrentPhase();
                    _logs.Append("session reset");
                }

                _lastEvent = $"dialog:{_resetDialog.LastResult.ToString().ToLowerInvariant()}";
            }

            return null;
        }

        if (_mode == InputMode.Command)
        {
            if (MatchesBinding(key, "esc"))
            {
                _mode = InputMode.Navigate;
                _lastEvent = "mode:nav";
                return null;
            }

            var changed = _commandInput.Update(key);
            if (!changed)
            {
                return null;
            }

            if (_commandInput.SubmitCount == 0)
            {
                return null;
            }

            var command = _commandInput.LastSubmittedValue.Trim();
            if (string.IsNullOrWhiteSpace(command))
            {
                return null;
            }

            var cmd = ExecuteCommand(command);
            return cmd;
        }

        if (MatchesBinding(key, CommandModeKey))
        {
            _mode = InputMode.Command;
            _lastEvent = "mode:cmd";
            return null;
        }

        if (MatchesBinding(key, ModalKey))
        {
            _resetDialog.Visible = !_resetDialog.Visible;
            _lastEvent = _resetDialog.Visible ? "dialog:open" : "dialog:close";
            return null;
        }

        if (MatchesBinding(key, ToastKey))
        {
            _toasts.Push(new ToastMessage($"{(_isBreak ? "break" : "focus")} {FormatClock(_remainingSeconds)}", 70, ToastSeverity.Info));
            _lastEvent = "toast";
            return null;
        }

        if (MatchesBinding(key, "space") || MatchesBinding(key, "enter"))
        {
            _running = !_running;
            _lastEvent = _running ? "running:on" : "running:off";
            _logs.Append(_lastEvent);
            return null;
        }

        if (MatchesBinding(key, "r"))
        {
            ResetCurrentPhase();
            _lastEvent = "reset";
            _logs.Append(_lastEvent);
            return null;
        }

        if (MatchesBinding(key, "n"))
        {
            AdvancePhase(logTransition: true);
            _lastEvent = "skip";
            return null;
        }

        if (MatchesBinding(key, "q"))
        {
            return Tea.Cmd.Quit;
        }

        return null;
    }

    private Command? ExecuteCommand(string command)
    {
        _lastEvent = $"cmd:{command}";
        _logs.Append(_lastEvent);

        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
        {
            return null;
        }

        switch (parts[0].ToLowerInvariant())
        {
            case "help":
                _logs.Append("commands: help | start | pause | reset | skip | focus <min> | break <min> | quit");
                break;
            case "start":
                _running = true;
                break;
            case "pause":
                _running = false;
                break;
            case "reset":
                ResetCurrentPhase();
                break;
            case "skip":
                AdvancePhase(logTransition: true);
                break;
            case "focus":
                if (parts.Length > 1 && int.TryParse(parts[1], out var focus) && focus > 0)
                {
                    _focusMinutes = focus;
                    if (!_isBreak)
                    {
                        _remainingSeconds = _focusMinutes * 60;
                    }
                }
                break;
            case "break":
                if (parts.Length > 1 && int.TryParse(parts[1], out var brk) && brk > 0)
                {
                    _breakMinutes = brk;
                    if (_isBreak)
                    {
                        _remainingSeconds = _breakMinutes * 60;
                    }
                }
                break;
            case "quit":
                return Tea.Cmd.Quit;
            default:
                _logs.Append($"unknown command: {command}");
                break;
        }

        return null;
    }

    private void RenderSession(Canvas canvas, Rect rect)
    {
        canvas.DrawBox(rect, _accentStyle.Render("Pomodoro"));
        var content = rect.Inset(1, 1);
        if (content.IsEmpty)
        {
            return;
        }

        var header = _titleStyle.Render("{{EscapeForString(appName)}}");
        canvas.WriteText(content.X, content.Y, header, content.Width);
        if (content.Height > 1)
        {
            canvas.WriteText(content.X, content.Y + 1, _mutedStyle.Render("Theme: {{EscapeForString(theme.DisplayName)}}"), content.Width);
        }

        var phaseLine = $"phase: {(_isBreak ? "break" : "focus")}";
        var cycleLine = $"cycles: {_cycleCount}";
        var clockLine = _accentStyle.Render($"time: {FormatClock(_remainingSeconds)}");
        if (content.Height > 2) canvas.WriteText(content.X, content.Y + 2, phaseLine, content.Width);
        if (content.Height > 3) canvas.WriteText(content.X, content.Y + 3, cycleLine, content.Width);
        if (content.Height > 4) canvas.WriteText(content.X, content.Y + 4, clockLine, content.Width);

        var total = Math.Max(1, (_isBreak ? _breakMinutes : _focusMinutes) * 60);
        var progressValue = 1.0 - ((double)_remainingSeconds / total);
        _progress.SetValue(progressValue);

        var progressRect = new Rect(content.X, Math.Max(content.Y + 6, content.Bottom - 3), content.Width, Math.Min(3, content.Height));
        _progress.Focused = false;
        _progress.Render(canvas, progressRect);
    }

    private void ResetCurrentPhase()
    {
        _remainingSeconds = (_isBreak ? _breakMinutes : _focusMinutes) * 60;
        _running = false;
    }

    private void AdvancePhase(bool logTransition)
    {
        if (!_isBreak)
        {
            _cycleCount++;
        }

        _isBreak = !_isBreak;
        _remainingSeconds = (_isBreak ? _breakMinutes : _focusMinutes) * 60;
        _running = false;
        _toasts.Push(new ToastMessage(_isBreak ? "Break started" : "Focus started", 70, ToastSeverity.Success));

        if (logTransition)
        {
            _logs.Append(_isBreak ? "phase -> break" : "phase -> focus");
        }
    }

    private static string FormatClock(int totalSeconds)
    {
        var safe = Math.Max(0, totalSeconds);
        var minutes = safe / 60;
        var seconds = safe % 60;
        return $"{minutes:00}:{seconds:00}";
    }

    private static bool MatchesBinding(KeyPressMsg key, string binding)
    {
        var normalized = (binding ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized.Length == 0)
        {
            return false;
        }

        if (key.Modifiers.HasFlag(KeyModifiers.Ctrl)
            || key.Modifiers.HasFlag(KeyModifiers.Alt)
            || key.Modifiers.HasFlag(KeyModifiers.Meta))
        {
            return false;
        }

        return normalized switch
        {
            "esc" or "escape" => key.Is(KeyCode.Escape, KeyModifiers.None),
            "enter" => key.Is(KeyCode.Enter, KeyModifiers.None),
            "space" => key.IsCharacter(' ', KeyModifiers.None, ignoreCase: false),
            _ when normalized.Length == 1 => key.IsCharacter(normalized[0], KeyModifiers.None),
            _ => false,
        };
    }

    private static string YesNo(bool value) => value ? "yes" : "no";

    private static Command NextTick() => Tea.Cmd.Every(TimeSpan.FromSeconds(1), at => new PomodoroTickMsg(at));
}
""";
    }

    private static string BuildDashboardTemplate(string appName, ThemePreset theme, string commandKey, string toastKey, string modalKey)
    {
        var (titleR, titleG, titleB) = ParseHex(theme.TitleHex);
        var (accentR, accentG, accentB) = ParseHex(theme.AccentHex);
        var (mutedR, mutedG, mutedB) = ParseHex(theme.MutedHex);

        return $$"""
using TeaSharp;
using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using TeaSharp.Styles;
using ModelView = TeaSharp.Core.Abstractions.View;

var terminal = new ConsoleTerminalAdapter();
var options = new ProgramOptions
{
    UseConsoleKeyEvents = false,
    Terminal = terminal,
    TerminalCapabilities = TerminalCapabilityDetector.Detect(),
};

var program = Tea.NewProgram(new DashboardModel(), options);

try
{
    await program.RunAsync();
    return 0;
}
catch (TeaProgramInterruptedException)
{
    return 130;
}

internal sealed record DashboardTickMsg(DateTimeOffset At) : IMessage;

internal sealed class DashboardModel : IModel
{
    private const string CommandModeKey = "{{EscapeForString(commandKey)}}";
    private const string ToastKey = "{{EscapeForString(toastKey)}}";
    private const string ModalKey = "{{EscapeForString(modalKey)}}";

    private readonly TeaStyle _titleStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.Rgb({{titleR}}, {{titleG}}, {{titleB}}));
    private readonly TeaStyle _accentStyle = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.Rgb({{accentR}}, {{accentG}}, {{accentB}}));
    private readonly TeaStyle _mutedStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb({{mutedR}}, {{mutedG}}, {{mutedB}}));

    private readonly TabsComponent _tabs = new(["Overview", "Services", "Events"]);
    private readonly StatusBarComponent _status = new()
    {
        Theme = new UiTheme(StatusFill: '{{theme.StatusFill}}', ModalBackdropFill: '{{theme.ModalFill}}')
    };
    private readonly TableComponent _table = new(["Service", "Status", "P95"]) { Title = "Service Table" };
    private readonly LogViewerComponent _logs = new() { Title = "Events" };
    private readonly ProgressBarComponent _capacity = new() { Title = "Cluster Load" };
    private readonly ToastCenterComponent _toasts = new();
    private readonly DialogComponent _modal = new()
    {
        Title = "Action",
        Lines = ["Confirm maintenance window?", "enter/space confirm", "esc cancel"],
        Theme = new UiTheme(ModalBackdropFill: '{{theme.ModalFill}}'),
    };

    private int _width = 100;
    private int _height = 32;
    private int _tick;
    private string _lastEvent = "ready";

    public DashboardModel()
    {
        _table.SetRows(
        [
            ["api", "ok", "24ms"],
            ["search", "ok", "31ms"],
            ["billing", "warn", "88ms"],
            ["queue", "ok", "17ms"],
            ["worker", "ok", "22ms"],
            ["notify", "degraded", "92ms"],
            ["metrics", "ok", "20ms"],
            ["auth", "ok", "29ms"],
        ]);
        _table.Inner.PageSize = 4;
        _logs.Append("dashboard scaffold ready");
        _logs.Append($"Theme: {{theme.DisplayName}}");
    }

    public Command? Init() => Tea.Cmd.Every(TimeSpan.FromMilliseconds(900), at => new DashboardTickMsg(at));

    public Command? Update(IMessage message)
    {
        switch (message)
        {
            case DashboardTickMsg tick:
                _tick++;
                _capacity.SetValue((_tick % 100) / 100.0);
                if (_tick % 3 == 0)
                {
                    _logs.Append($"{tick.At:HH:mm:ss} pulse={_tick:0000}");
                }

                _toasts.Update(new TickMsg(tick.At));
                return Tea.Cmd.Every(TimeSpan.FromMilliseconds(900), at => new DashboardTickMsg(at));

            case WindowSizeMsg ws:
                _width = ws.Width;
                _height = ws.Height;
                _lastEvent = $"resize:{_width}x{_height}";
                return null;

            case KeyPressMsg key:
                if (key.Modifiers.HasFlag(KeyModifiers.Ctrl)
                    && (key.IsCharacter('c') || key.IsCharacter('\u0003', ignoreCase: false)))
                {
                    return Tea.Cmd.Quit;
                }

                if (_modal.Visible)
                {
                    _modal.Focused = true;
                    if (_modal.Update(key))
                    {
                        _lastEvent = $"modal:{_modal.LastResult.ToString().ToLowerInvariant()}";
                    }

                    return null;
                }

                if (MatchesBinding(key, CommandModeKey))
                {
                    _tabs.Select((_tabs.SelectedIndex + 1) % _tabs.Tabs.Count);
                    _lastEvent = $"tab:{_tabs.SelectedIndex + 1}";
                    return null;
                }

                if (MatchesBinding(key, ToastKey))
                {
                    _toasts.Push(new ToastMessage($"capacity {(_tick % 100)}%", 70, ToastSeverity.Info));
                    _lastEvent = "toast";
                    return null;
                }

                if (MatchesBinding(key, ModalKey))
                {
                    _modal.Visible = !_modal.Visible;
                    _lastEvent = _modal.Visible ? "modal:open" : "modal:close";
                    return null;
                }

                if (MatchesBinding(key, "q"))
                {
                    return Tea.Cmd.Quit;
                }

                if (_tabs.Update(key))
                {
                    _lastEvent = $"tab:{_tabs.SelectedIndex + 1}";
                }

                if (_tabs.SelectedIndex == 1)
                {
                    _table.Update(key);
                }

                if (_tabs.SelectedIndex == 2)
                {
                    _logs.Update(key);
                }

                return null;

            default:
                return null;
        }
    }

    public ModelView View()
    {
        var canvas = new Canvas(Math.Max(40, _width), Math.Max(14, _height), CanvasTextMode.GraphemeAware);
        canvas.Clear();

        _tabs.Render(canvas, new Rect(0, 0, _width, 1));

        var body = new Rect(0, 1, _width, Math.Max(1, _height - 2));
        var (left, right) = Layout.SplitVertical(body, Math.Max(30, body.Width / 2));

        var heading = _titleStyle.Render("{{EscapeForString(appName)}}") + " " + _mutedStyle.Render("•") + " " + _accentStyle.Render("{{EscapeForString(theme.DisplayName)}}");
        canvas.WriteText(left.X, left.Y, heading, left.Width);

        _capacity.Render(canvas, new Rect(left.X, left.Y + 2, left.Width, 4));

        if (_tabs.SelectedIndex == 1)
        {
            _table.Render(canvas, right);
        }
        else
        {
            _logs.Render(canvas, right);
        }

        _toasts.Render(canvas, new Rect(Math.Max(0, _width - 40), 1, Math.Min(40, _width), 9));
        _modal.Render(canvas, body);

        _status.LeftText = $"tab={_tabs.SelectedIndex + 1} mode=nav";
        _status.RightText = $"cmd={CommandModeKey} toast={ToastKey} modal={ModalKey} event={_lastEvent}";
        _status.Render(canvas, new Rect(0, _height - 1, _width, 1));

        return ModelView.From(canvas.Render()) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            MouseMode = MouseMode.AllMotion,
            WindowTitle = "{{EscapeForString(appName)}}",
        };
    }

    private static bool MatchesBinding(KeyPressMsg key, string binding)
    {
        var normalized = (binding ?? string.Empty).Trim().ToLowerInvariant();
        if (normalized.Length == 0)
        {
            return false;
        }

        if (key.Modifiers.HasFlag(KeyModifiers.Ctrl)
            || key.Modifiers.HasFlag(KeyModifiers.Alt)
            || key.Modifiers.HasFlag(KeyModifiers.Meta))
        {
            return false;
        }

        return normalized switch
        {
            "esc" or "escape" => key.Is(KeyCode.Escape, KeyModifiers.None),
            "enter" => key.Is(KeyCode.Enter, KeyModifiers.None),
            "space" => key.IsCharacter(' ', KeyModifiers.None, ignoreCase: false),
            _ when normalized.Length == 1 => key.IsCharacter(normalized[0], KeyModifiers.None),
            _ => false,
        };
    }
}
""";
    }

    private static string? FindRepoRoot(string startDirectory)
    {
        var current = new DirectoryInfo(startDirectory);
        while (current is not null)
        {
            var hasCore = File.Exists(Path.Combine(current.FullName, "src", "TeaSharp.Core", "TeaSharp.Core.csproj"));
            var hasTeaSharp = File.Exists(Path.Combine(current.FullName, "src", "TeaSharp", "TeaSharp.csproj"));
            if (hasCore && hasTeaSharp)
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        return null;
    }

    private static string ResolveProjectReferencePath(string outputDir, string repoRoot, string projectPath)
    {
        var absolute = Path.GetFullPath(projectPath);
        var chosen = IsPathUnderRoot(outputDir, repoRoot)
            ? Path.GetRelativePath(outputDir, projectPath)
            : absolute;
        return chosen.Replace(Path.DirectorySeparatorChar, '/');
    }

    private static bool IsPathUnderRoot(string path, string root)
    {
        var fullPath = Path.GetFullPath(path)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var fullRoot = Path.GetFullPath(root)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

        if (OperatingSystem.IsWindows())
        {
            return fullPath.StartsWith(fullRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)
                || string.Equals(fullPath, fullRoot, StringComparison.OrdinalIgnoreCase);
        }

        return fullPath.StartsWith(fullRoot + Path.DirectorySeparatorChar, StringComparison.Ordinal)
            || string.Equals(fullPath, fullRoot, StringComparison.Ordinal);
    }

    private static string Prompt(string label, string defaultValue, Func<string, bool> validator)
    {
        while (true)
        {
            Console.Write($"{label} [{defaultValue}]: ");
            var input = Console.ReadLine();
            var value = string.IsNullOrWhiteSpace(input) ? defaultValue : input.Trim();
            if (validator(value))
            {
                return value;
            }

            Console.WriteLine("Invalid value. Try again.");
        }
    }

    private static int PromptChoiceIndex(string label, IReadOnlyList<string> options, int defaultIndex)
    {
        while (true)
        {
            Console.WriteLine(label + ":");
            for (var i = 0; i < options.Count; i++)
            {
                var marker = i == defaultIndex ? "*" : " ";
                Console.WriteLine($"  {i + 1}. [{marker}] {options[i]}");
            }

            Console.Write($"Select 1-{options.Count} [{defaultIndex + 1}]: ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                return defaultIndex;
            }

            if (int.TryParse(input, out var selected) && selected >= 1 && selected <= options.Count)
            {
                return selected - 1;
            }

            Console.WriteLine("Invalid choice. Try again.");
        }
    }

    private static string PromptChoice(string label, IReadOnlyList<string> options, int defaultIndex)
    {
        var index = PromptChoiceIndex(label, options, defaultIndex);
        return options[index];
    }

    private static bool PromptYesNo(string question, bool defaultYes)
    {
        var suffix = defaultYes ? "[Y/n]" : "[y/N]";
        Console.Write($"{question} {suffix}: ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input))
        {
            return defaultYes;
        }

        return input.Trim().StartsWith("y", StringComparison.OrdinalIgnoreCase);
    }

    private static bool ValidateIdentifier(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        foreach (var ch in value)
        {
            if (!(char.IsLetterOrDigit(ch) || ch is '.' or '_' or '-'))
            {
                return false;
            }
        }

        return true;
    }

    private static bool ValidateBindingToken(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();
        if (normalized is "esc" or "escape" or "enter" or "space")
        {
            return true;
        }

        return normalized.Length == 1;
    }

    private static string SanitizeProjectName(string name)
    {
        var chars = name.Where(static c => char.IsLetterOrDigit(c) || c == '_' || c == '.').ToArray();
        var value = new string(chars);
        return string.IsNullOrWhiteSpace(value) ? "TeaSharp.App" : value;
    }

    private static (byte R, byte G, byte B) ParseHex(string hex)
    {
        var value = hex.Trim();
        if (value.StartsWith('#'))
        {
            value = value[1..];
        }

        if (value.Length != 6)
        {
            throw new InvalidOperationException($"Invalid color hex '{hex}'.");
        }

        var r = Convert.ToByte(value[0..2], 16);
        var g = Convert.ToByte(value[2..4], 16);
        var b = Convert.ToByte(value[4..6], 16);
        return (r, g, b);
    }

    private static string EscapeForString(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);
    }

    private static string QuoteIfNeeded(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return value.Contains(' ', StringComparison.Ordinal)
            ? $"\"{value}\""
            : value;
    }
}

internal sealed record ThemePreset(
    string Id,
    string DisplayName,
    string TitleHex,
    string AccentHex,
    string MutedHex,
    char StatusFill,
    char ModalFill);

internal sealed record WizardManifest(
    string AppName,
    string Template,
    string ThemePreset,
    WizardKeyBindings KeyBindings,
    DateTimeOffset GeneratedAtUtc);

internal sealed record WizardKeyBindings(
    string CommandMode,
    string Toast,
    string Modal);
