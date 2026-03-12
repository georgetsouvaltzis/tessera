using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.TestFixtures;

public sealed class CounterFixtureModel : IScreen
{
    private int _count;

    public Effect? Init() => null;

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
            ? TeaSharp.Core.Commands.Effects.Quit
            : null;
    }

    public TeaSharp.Core.Abstractions.ScreenOutput Render()
    {
        return TeaSharp.Core.Abstractions.ScreenOutput.From($"Counter\n\nCount: {_count}\n\nUp/Down: change  q: quit") with
        {
            Terminal = new TerminalOutput
            {
                AltScreen = true,
                WindowTitle = "TeaSharp Counter Fixture",
            },
        };
    }
}
