using Tessera;
using Tessera.Core.Abstractions;
using Tessera.Core.Input;
using Tessera.Core.Messages;

namespace Tessera.TestFixtures;

public sealed class CounterFixtureModel
{
    private int _count;

    public Effect? Update(IMessage message)
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
            ? Tessera.Core.Commands.Effects.Quit
            : null;
    }

    public ScreenOutput Render()
    {
        return ScreenOutput.From($"Counter\n\nCount: {_count}\n\nUp/Down: change  q: quit") with
        {
            Terminal = new TerminalOutput
            {
                AltScreen = true,
                WindowTitle = "Tessera Counter Fixture",
            },
        };
    }
}

public sealed class CounterFixtureApp : TesseraApp
{
    private int _count;

    public override TesseraEffect? Update(Message message)
    {
        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.Is(Key.Up))
        {
            _count++;
            return null;
        }

        if (key.Is(Key.Down))
        {
            _count--;
            return null;
        }

        return key.IsCharacter('q')
            ? TesseraEffects.Quit
            : null;
    }

    public override Screen Build(ScreenContext context) =>
        Screen.From($"Counter\n\nCount: {_count}\n\nUp/Down: change  q: quit");
}
