using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.TestFixtures;

public sealed class CounterFixtureModel : IModel
{
    private int _count;

    public Command? Init() => null;

    public Command? Update(IMessage message)
    {
        if (message is not KeyPressMsg key)
        {
            return null;
        }

        if (key.Is(KeyCode.Up, KeyModifiers.None))
        {
            _count++;
            return null;
        }

        if (key.Is(KeyCode.Down, KeyModifiers.None))
        {
            _count--;
            return null;
        }

        return key.IsCharacter('q', KeyModifiers.None)
            ? Tea.Cmd.Quit
            : null;
    }

    public TeaSharp.Core.Abstractions.View View()
    {
        return TeaSharp.Core.Abstractions.View.From($"Counter\n\nCount: {_count}\n\nUp/Down: change  q: quit") with
        {
            Terminal = new ViewTerminal
            {
                AltScreen = true,
                WindowTitle = "TeaSharp Counter Fixture",
            },
        };
    }
}
