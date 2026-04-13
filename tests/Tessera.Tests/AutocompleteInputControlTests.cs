using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class AutocompleteInputControlTests
{
    [Test]
    public void AutocompleteInputApiSetSuggestionsAndSelectedSuggestionIndexWork()
    {
        var control = CreateControl();
        control.SetSuggestions(["alpha", "beta", "gamma"]);
        control.Text = "a";

        var changed = control.SetSelectedSuggestionIndex(1);

        Assert.That(control.Suggestions.Count, Is.EqualTo(3));
        Assert.That(changed, Is.True);
        Assert.That(control.SelectedSuggestionIndex, Is.EqualTo(1));
        Assert.That(control.SelectedSuggestion, Is.EqualTo("gamma"));
    }

    [Test]
    public void AutocompleteInputKeyboardCommitRaisesSuggestionCommitted()
    {
        var control = CreateControl();
        control.SetSuggestions(["alpha", "beta", "bravo"]);
        control.IsFocused = true;

        control.Handle(new KeyPressed(Key.Character, "b"));
        control.Handle(new KeyPressed(Key.Character, "r"));

        AutocompleteInputSuggestionCommittedEventArgs? args = null;
        control.SuggestionCommitted += (_, eventArgs) => args = eventArgs;

        var handled = control.Handle(new KeyPressed(Key.Enter));

        Assert.That(handled, Is.True);
        Assert.That(args, Is.Not.Null);
        Assert.That(TestAssert.NotNull(args).Text, Is.EqualTo("bravo"));
        Assert.That(args.SuggestionIndex, Is.EqualTo(2));
        Assert.That(args.PreviousText, Is.EqualTo("br"));
        Assert.That(control.Text, Is.EqualTo("bravo"));
    }

    [Test]
    public void AutocompleteInputPointerClickSelectsAndCommitsSuggestion()
    {
        var control = CreateControl();
        control.Border = BorderStyle.None;
        control.SetSuggestions(["home", "help", "health"]);
        control.Text = "he";

        AutocompleteInputSuggestionCommittedEventArgs? args = null;
        control.SuggestionCommitted += (_, eventArgs) => args = eventArgs;

        var bounds = new Rect(0, 0, 30, 6);
        var handled = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2), bounds);

        Assert.That(handled, Is.True);
        Assert.That(control.Text, Is.EqualTo("health"));
        Assert.That(args, Is.Not.Null);
        Assert.That(TestAssert.NotNull(args).SuggestionIndex, Is.EqualTo(2));
    }

    [Test]
    public void AutocompleteInputStyleAndGlyphHooksRenderExpectedAnsi()
    {
        var control = CreateControl();
        control.Title = "AC";
        control.FocusMarker = "!";
        control.Border = BorderStyle.SingleLine;
        control.IsFocused = true;
        control.Glyphs = new AutocompleteInputGlyphSet("~", "!", "|");
        control.FocusedTitleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30));
        control.FocusedBorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(40, 50, 60));
        control.InputTextStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(70, 80, 90));
        control.PopupStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(100, 110, 120));
        control.SelectedSuggestionStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(130, 140, 150));
        control.FocusedSelectedSuggestionStyle = TesseraStyle.Empty.WithBold();
        control.CommitMarkerStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(160, 170, 180));
        control.SetSuggestions(["alpha", "beta", "bravo"]);
        control.Text = "b";
        control.SetSelectedSuggestionIndex(1);

        var output = Render(control, 40, 8);

        Assert.That(output.Contains("AC !", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('!'), Is.True);
        Assert.That(output.Contains("~|bravo", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;10;20;30", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;40;50;60", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;70;80;90", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;130;140;150", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("38;2;160;170;180", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void AutocompleteInputDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = CreateControl();
        control.Border = BorderStyle.None;
        control.SetSuggestions(["alpha", "beta"]);
        control.Text = "b";

        var first = Render(control, 24, 6);
        var second = Render(control, 24, 6);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
        Assert.That(first.Contains('>'), Is.True);
        Assert.That(first.Contains('↵'), Is.True);
    }

    private static AutocompleteInput CreateControl()
    {
        return new AutocompleteInput { Placeholder = "Search..." };
    }

    private static string Render(AutocompleteInput control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
