using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.UiKit;
using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.ScreenOutput;

var program = Tea.CreateProgram(new ComboBoxDemoModel(), new TeaProgramOptions
{
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

internal sealed class ComboBoxDemoModel : IScreen
{
    private readonly ComboboxComponent _combobox = new(new ComboboxOptions(
        Items:
        [
            "us-east-1",
            "us-east-2",
            "us-west-1",
            "us-west-2",
            "eu-central-1",
            "eu-west-1",
            "eu-west-2",
            "ap-southeast-1",
            "ap-southeast-2",
            "ap-northeast-1",
            "sa-east-1",
        ],
        Title: "Region",
        Placeholder: "type to filter regions",
        IsFocused: true,
        MaxVisibleItems: 7));

    private int _width = 90;
    private int _height = 28;
    private string _lastEvent = "ready";

    public ComboBoxDemoModel()
    {
    }

    public Effect? Init() => null;

    public Effect? Update(IMessage message)
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
            var mousePrevious = _combobox.SelectedItem;
            if (_combobox.UpdateMouse(mouse, GetComboboxRect()))
            {
                if (!string.Equals(mousePrevious, _combobox.SelectedItem, StringComparison.Ordinal))
                {
                    _lastEvent = $"selected:{_combobox.SelectedItem}";
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
            var changed = _combobox.Update(message);
            if (changed)
            {
                _lastEvent = "input:update";
            }

            return null;
        }

        if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c'))
            || key.IsCharacter('q', KeyModifiers.None))
        {
            return Tea.Effects.Quit;
        }

        var previous = _combobox.SelectedItem;
        if (_combobox.Update(key))
        {
            if (!string.Equals(previous, _combobox.SelectedItem, StringComparison.Ordinal))
            {
                _lastEvent = $"selected:{_combobox.SelectedItem}";
            }
            else
            {
                _lastEvent = $"filter:{_combobox.FilterText}";
            }
        }

        return null;
    }

    public ModelView Render()
    {
        var width = Math.Max(56, _width);
        var height = Math.Max(18, _height);

        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var frame = new Rect(0, 0, width, height);
        canvas.DrawBox(frame, "TeaSharp Combobox Example", BorderStyle.Rounded);

        var body = frame.Inset(2, 2);
        canvas.WriteText(body.X, body.Y, "Controls: type filter, enter select, up/down navigate, esc close, mouse click+wheel, q quit", body.Width);

        var comboRect = GetComboboxRect();
        _combobox.Render(canvas, comboRect);

        canvas.WriteText(body.X, body.Bottom - 4, $"Filter: {_combobox.FilterText}", body.Width);
        canvas.WriteText(body.X, body.Bottom - 3, $"Selected: {_combobox.SelectedItem}", body.Width);
        canvas.WriteText(body.X, body.Bottom - 2, $"Open: {(_combobox.IsOpen ? "yes" : "no")}", body.Width);

        var status = new StatusBarComponent
        {
            LeftText = "widget=combobox mode=demo",
            RightText = $"event={_lastEvent}",
            Theme = new UiTheme(StatusFill: '·'),
        };
        status.Render(canvas, new Rect(0, height - 1, width, 1));

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
                WindowTitle = "TeaSharp Combobox Example",
            },
        };
    }

    private Rect GetComboboxRect()
    {
        var width = Math.Max(56, _width);
        var height = Math.Max(18, _height);
        var frame = new Rect(0, 0, width, height);
        var body = frame.Inset(2, 2);
        return new Rect(body.X, body.Y + 2, body.Width, Math.Min(12, body.Height - 7));
    }
}
