using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TokenEditorControlTests
{
    [Test]
    public void ControlsTokenEditorTokensChangedRaisesForSetAddRemoveWithSnapshots()
    {
        var control = new TokenEditor();
        var events = new List<TokenEditorTokensChangedEventArgs>();
        control.TokensChanged += (_, args) => events.Add(args);

        control.SetTokens([new TokenItem("one"), new TokenItem("two", true)]);
        var added = control.AddToken("three");
        _ = control.SetSelectedTokenIndex(2);
        var removed = control.RemoveSelectedToken();

        Assert.That(added, Is.True);
        Assert.That(removed, Is.True);
        Assert.That(events, Has.Count.EqualTo(3));

        var initialTokens = new[] { "one", "two" };
        var updatedTokens = new[] { "one", "two", "three" };
        Assert.That(events[0].PreviousTokens, Is.Empty);
        Assert.That(events[0].Tokens.Select(static token => token.Value), Is.EqualTo(initialTokens));
        Assert.That(events[0].Tokens[1].IsDisabled, Is.True);

        Assert.That(events[1].PreviousTokens.Select(static token => token.Value), Is.EqualTo(initialTokens));
        Assert.That(events[1].Tokens.Select(static token => token.Value), Is.EqualTo(updatedTokens));

        Assert.That(events[2].PreviousTokens.Select(static token => token.Value), Is.EqualTo(updatedTokens));
        Assert.That(events[2].Tokens.Select(static token => token.Value), Is.EqualTo(initialTokens));
    }

    [Test]
    public void ControlsTokenEditorTokensChangedDoesNotRaiseForSelectionOnlyOrNoOpMutations()
    {
        var control = new TokenEditor();
        var events = 0;
        control.SetTokens([new TokenItem("one"), new TokenItem("two")]);
        control.TokensChanged += (_, _) => events++;

        var selected = control.SetSelectedTokenIndex(1);
        var duplicateWhitespace = control.AddToken("   ");
        control.SetTokens([new TokenItem("one"), new TokenItem("two")]);

        Assert.That(selected, Is.True);
        Assert.That(duplicateWhitespace, Is.False);
        Assert.That(events, Is.Zero);
    }

    [Test]
    public void ControlsTokenEditorSetTokensAddRemoveAndSelectionEventWork()
    {
        var control = new TokenEditor();
        TokenEditorSelectionChangedEventArgs? lastEvent = null;
        control.SelectionChanged += (_, args) => lastEvent = args;

        control.SetTokens([new TokenItem("one"), new TokenItem("two")]);
        var added = control.AddToken(" three ");
        var moved = control.SetSelectedTokenIndex(2);
        var removed = control.RemoveSelectedToken();

        Assert.That(added, Is.True);
        Assert.That(moved, Is.True);
        Assert.That(removed, Is.True);
        Assert.That(control.Tokens.Count, Is.EqualTo(2));
        Assert.That(control.Tokens[0].Value, Is.EqualTo("one"));
        Assert.That(control.Tokens[1].Value, Is.EqualTo("two"));
        Assert.That(control.SelectedTokenIndex, Is.EqualTo(1));
        Assert.That(lastEvent, Is.Not.Null);
        Assert.That(lastEvent!.PreviousIndex, Is.EqualTo(2));
        Assert.That(lastEvent.SelectedIndex, Is.EqualTo(1));
    }

    [Test]
    public void ControlsTokenEditorKeyboardInputNavigationAndDeleteWork()
    {
        var control = new TokenEditor { IsFocused = true, Border = BorderStyle.None };

        _ = control.Handle(new KeyPressed(Key.Character, "o"));
        _ = control.Handle(new KeyPressed(Key.Character, "p"));
        _ = control.Handle(new KeyPressed(Key.Character, "s"));
        _ = control.Handle(new KeyPressed(Key.Enter));
        _ = control.Handle(new KeyPressed(Key.Character, "u"));
        _ = control.Handle(new KeyPressed(Key.Character, "i"));
        _ = control.Handle(new KeyPressed(Key.Enter));
        _ = control.Handle(new KeyPressed(Key.Right));
        _ = control.Handle(new KeyPressed(Key.Delete));

        Assert.That(control.Tokens.Count, Is.EqualTo(1));
        Assert.That(control.Tokens[0].Value, Is.EqualTo("ops"));
        Assert.That(control.SelectedTokenIndex, Is.EqualTo(0));
    }

    [Test]
    public void ControlsTokenEditorPointerClickSelectsTokenAndRaisesEvent()
    {
        var control = new TokenEditor
        {
            Border = BorderStyle.None,
            Glyphs = new TokenEditorGlyphSet(
                ">",
                ".",
                "[",
                "]",
                string.Empty,
                " ")
        };
        control.SetTokens([new TokenItem("a"), new TokenItem("b")]);
        var bounds = new Rect(0, 0, 40, 4);
        TokenEditorSelectionChangedEventArgs? selectionChanged = null;
        control.SelectionChanged += (_, args) => selectionChanged = args;

        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 6, 0), bounds);
        var handled = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 6, 0), bounds);

        Assert.That(handled, Is.True);
        Assert.That(control.SelectedTokenIndex, Is.EqualTo(1));
        Assert.That(selectionChanged, Is.Not.Null);
        Assert.That(selectionChanged!.SelectedIndex, Is.EqualTo(1));
        Assert.That(selectionChanged.SelectedToken?.Value, Is.EqualTo("b"));
    }

    [Test]
    public void ControlsTokenEditorCustomGlyphsAndStateStylesRenderExpectedAnsi()
    {
        var control = new TokenEditor
        {
            IsFocused = true,
            Border = BorderStyle.None,
            Glyphs = new TokenEditorGlyphSet(
                "S",
                "U",
                "<",
                ">",
                ":",
                "|"),
            SelectedTokenStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(11, 22, 33)),
            FocusedSelectedTokenStyle = TesseraStyle.Empty.WithBold(),
            HoveredTokenStyle = TesseraStyle.Empty.WithUnderline(),
            DisabledTokenStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(44, 55, 66))
        };
        control.SetTokens([new TokenItem("enabled"), new TokenItem("disabled", true)]);
        _ = control.SetSelectedTokenIndex(0);
        var bounds = new Rect(0, 0, 80, 4);

        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 16, 0), bounds);
        var canvas = new Canvas(80, 4, CanvasTextMode.GraphemeAware);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(output.Contains("S:<enabled>", StringComparison.Ordinal),
            "Selected token should render custom marker/prefix/suffix.");
        TestAssert.True(output.Contains("U:<disabled>", StringComparison.Ordinal),
            "Unselected token should render custom marker/prefix/suffix.");
        TestAssert.True(output.Contains("48;2;11;22;33", StringComparison.Ordinal),
            "Selected token style should render.");
        TestAssert.True(output.Contains("38;2;44;55;66", StringComparison.Ordinal),
            "Disabled token style should render.");
        TestAssert.True(
            output.Contains(";4;", StringComparison.Ordinal)
            || output.Contains("[4m", StringComparison.Ordinal)
            || output.Contains("[4;", StringComparison.Ordinal),
            "Hovered token style should render.");
    }

    [Test]
    public void ControlsTokenEditorDefaultRenderIsDeterministicAndMonochrome()
    {
        var control = new TokenEditor { Border = BorderStyle.None };
        control.SetTokens([new TokenItem("one"), new TokenItem("two")]);
        var bounds = new Rect(0, 0, 40, 4);
        var firstCanvas = new Canvas(40, 4);
        var secondCanvas = new Canvas(40, 4);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        TestAssert.Equal(first, second, "Token editor render should be deterministic.");
        TestAssert.True(!first.Contains("\e[", StringComparison.Ordinal),
            "Default token editor output should remain monochrome.");
    }
}
