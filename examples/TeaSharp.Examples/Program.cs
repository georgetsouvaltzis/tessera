using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.View;

var model = new CounterModel();
var options = new ProgramOptions();
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
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() =>
        ModelView.From($"TeaSharp Counter\n\nCount: {_count}\n\nup/k increment\ndown/j decrement\nq or ctrl+c quit\n") with
        {
            AltScreen = true,
        };
}
