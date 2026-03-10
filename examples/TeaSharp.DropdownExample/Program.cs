using TeaSharp;
using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using ModelView = TeaSharp.Core.Abstractions.View;

var terminal = new ConsoleTerminalAdapter();
var program = Tea.NewProgram(new DropdownDemoModel(), new ProgramOptions
{
    Terminal = terminal,
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

internal sealed class DropdownDemoModel : IModel
{
    private readonly DropdownComponent _dropdown = new()
    {
        Title = "Environment",
        Focused = true,
        MaxVisibleItems = 6,
    };

    private int _width = 90;
    private int _height = 28;
    private string _lastEvent = "ready";

    public DropdownDemoModel()
    {
        _dropdown.SetItems(
        [
            "Development",
            "Staging",
            "Production",
            "Canary",
            "Disaster-Recovery",
            "Benchmark",
            "QA",
            "Sandbox",
        ]);
    }

    public Command? Init() => null;

    public Command? Update(IMessage message)
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
            var mouseWasOpen = _dropdown.IsOpen;
            var mousePrevious = _dropdown.SelectedItem;
            if (_dropdown.UpdateMouse(mouse, GetDropdownRect()))
            {
                if (!string.Equals(mousePrevious, _dropdown.SelectedItem, StringComparison.Ordinal))
                {
                    _lastEvent = $"selected:{_dropdown.SelectedItem}";
                }
                else if (!mouseWasOpen && _dropdown.IsOpen)
                {
                    _lastEvent = "mouse:open";
                }
                else if (mouseWasOpen && !_dropdown.IsOpen)
                {
                    _lastEvent = "mouse:close";
                }
                else
                {
                    _lastEvent = $"mouse:{mouse.EventType.ToString().ToLowerInvariant()}";
                }
            }

            return null;
        }

        if (message is not KeyPressMsg key)
        {
            return null;
        }

        if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c'))
            || key.IsCharacter('q', KeyModifiers.None))
        {
            return Tea.Cmd.Quit;
        }

        var wasOpen = _dropdown.IsOpen;
        var previous = _dropdown.SelectedItem;
        if (_dropdown.Update(key))
        {
            if (!string.Equals(previous, _dropdown.SelectedItem, StringComparison.Ordinal))
            {
                _lastEvent = $"selected:{_dropdown.SelectedItem}";
            }
            else if (!wasOpen && _dropdown.IsOpen)
            {
                _lastEvent = "dropdown:open";
            }
            else if (wasOpen && !_dropdown.IsOpen)
            {
                _lastEvent = "dropdown:close";
            }
            else
            {
                _lastEvent = $"key:{key.Keystroke()}";
            }
        }

        return null;
    }

    public ModelView View()
    {
        var width = Math.Max(56, _width);
        var height = Math.Max(18, _height);

        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var frame = new Rect(0, 0, width, height);
        canvas.DrawBox(frame, "TeaSharp Dropdown Example", BorderStyle.Rounded);

        var body = frame.Inset(2, 2);
        canvas.WriteText(body.X, body.Y, "Controls: enter/space open+select, up/down navigate, esc close, mouse click+wheel, q quit", body.Width);

        var dropdownRect = GetDropdownRect();
        _dropdown.Render(canvas, dropdownRect);

        canvas.WriteText(body.X, body.Bottom - 3, $"Current: {_dropdown.SelectedItem}", body.Width);
        canvas.WriteText(body.X, body.Bottom - 2, $"Open: {(_dropdown.IsOpen ? "yes" : "no")}", body.Width);

        var status = new StatusBarComponent
        {
            LeftText = "widget=dropdown mode=demo",
            RightText = $"event={_lastEvent}",
            Theme = new UiTheme(StatusFill: '·'),
        };
        status.Render(canvas, new Rect(0, height - 1, width, 1));

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
                WindowTitle = "TeaSharp Dropdown Example",
            },
        };
    }

    private Rect GetDropdownRect()
    {
        var width = Math.Max(56, _width);
        var height = Math.Max(18, _height);
        var frame = new Rect(0, 0, width, height);
        var body = frame.Inset(2, 2);
        return new Rect(body.X, body.Y + 2, body.Width, Math.Min(10, body.Height - 6));
    }
}
