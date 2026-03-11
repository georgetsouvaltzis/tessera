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
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Tests;

internal static class KeyBindingTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("KeyBinding_PlusChord_MatchesCharacter", PlusChord_MatchesCharacter);
        yield return new TestCase("KeyBinding_PlusAlias_MatchesCharacter", PlusAlias_MatchesCharacter);
    }

    private static Task PlusChord_MatchesCharacter()
    {
        var binding = new KeyBinding("+", "plus", "+");

        var matches = binding.Matches(new KeyPressMsg(KeyCode.Character, "+"));

        TestAssert.True(matches, "Binding should match literal plus character.");
        return Task.CompletedTask;
    }

    private static Task PlusAlias_MatchesCharacter()
    {
        var binding = new KeyBinding("plus", "plus", "plus");

        var matches = binding.Matches(new KeyPressMsg(KeyCode.Character, "+"));

        TestAssert.True(matches, "Binding should match plus alias.");
        return Task.CompletedTask;
    }
}
