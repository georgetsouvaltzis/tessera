using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.View;

var model = new CounterModel();
var options = new ProgramOptions
{
    UseConsoleKeyEvents = false,
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
    private int _count;
    private int _width = 80;
    private int _height = 24;
    private bool _focused = true;
    private string _lastEvent = "none";
    private string _lastPaste = "(none)";

    public Command? Init() => null;

    public UpdateResult Update(IMessage message)
    {
        if (message is KeyPressMsg key)
        {
            if (key.Code == KeyCode.Up || key.Text == "k")
            {
                _count++;
            }
            else if (key.Code == KeyCode.Down || key.Text == "j")
            {
                _count--;
            }
            else if (key.Text == "q" || (key.Text == "c" && key.Modifiers.HasFlag(KeyModifiers.Ctrl)))
            {
                return new UpdateResult(this, Tea.Cmd.Quit);
            }

            _lastEvent = $"key: {key.Keystroke()}";
            return new UpdateResult(this, null);
        }

        if (message is PasteMsg paste)
        {
            _lastPaste = paste.Content
                .Replace("\n", "\\n", StringComparison.Ordinal)
                .Replace("\t", "\\t", StringComparison.Ordinal);
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
            $"Last event: {_lastEvent}\n" +
            $"Last paste: {_lastPaste}\n\n" +
            "Try live:\n" +
            "- up/down or k/j to change count\n" +
            "- paste multi-line text\n" +
            "- switch terminal focus away/back\n" +
            "- resize terminal window\n" +
            "- q or ctrl+c to quit\n";

        return ModelView.From(content) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            WindowTitle = "TeaSharp Protocol Probe",
        };
    }
}
