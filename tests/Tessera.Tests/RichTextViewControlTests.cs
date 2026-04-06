using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class RichTextViewControlTests
{
    [Test]
    public void RichTextView_RendersHeadingsListMarkersQuotesAndInlineEmphasis()
    {
        var control = new RichTextView
        {
            Border = BorderStyle.None,
        };
        control.SetLines(
        [
            [RichTextSegment.Heading("Quick Start", 2)],
            [RichTextSegment.ListMarker("-"), RichTextSegment.Plain("Install "), RichTextSegment.Emphasis("Tessera"), RichTextSegment.Plain(" package")],
            [RichTextSegment.QuoteMarker(">"), RichTextSegment.Plain("Use "), RichTextSegment.Strong("Tab"), RichTextSegment.Plain(" to move focus.")],
        ]);

        var output = Render(control, width: 64, height: 6);

        TestAssert.True(output.Contains("## Quick Start", StringComparison.Ordinal), "Heading should render with heading marker text.");
        TestAssert.True(output.Contains("- Install Tessera package", StringComparison.Ordinal), "List marker and inline emphasis content should render in-order.");
        TestAssert.True(output.Contains("> Use Tab to move focus.", StringComparison.Ordinal), "Quote marker and strong inline content should render in-order.");
    }

    [Test]
    public void RichTextView_StyleHooks_EmitAnsiForSemanticKinds()
    {
        var control = new RichTextView
        {
            Border = BorderStyle.None,
            TextStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
            HeadingStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(11, 12, 13)),
            ListMarkerStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(21, 22, 23)),
            QuoteMarkerStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
            EmphasisStyle = TesseraStyle.Empty.WithItalic(),
            StrongStyle = TesseraStyle.Empty.WithUnderline(),
            InlineCodeStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(41, 42, 43)),
        };
        control.SetLines(
        [
            [RichTextSegment.Heading("Title", 1)],
            [RichTextSegment.ListMarker("*"), RichTextSegment.Plain("plain "), RichTextSegment.Emphasis("em"), RichTextSegment.Plain(" "), RichTextSegment.Strong("strong"), RichTextSegment.Plain(" "), RichTextSegment.InlineCode("code")],
            [RichTextSegment.QuoteMarker(">"), RichTextSegment.Plain("quote")],
        ]);

        var output = Render(control, width: 80, height: 8, textMode: CanvasTextMode.GraphemeAware);
        var headingExpected = control.TextStyle.Merge(control.HeadingStyle).Render("# Title");
        var listMarkerExpected = control.TextStyle.Merge(control.ListMarkerStyle).Render("* ");
        var quoteMarkerExpected = control.TextStyle.Merge(control.QuoteMarkerStyle).Render("> ");
        var plainExpected = control.TextStyle.Render("plain ");
        var emphasisExpected = control.TextStyle.Merge(control.EmphasisStyle).Render("em");
        var strongExpected = control.TextStyle.Merge(control.StrongStyle).Render("strong");
        var codeExpected = control.TextStyle.Merge(control.InlineCodeStyle).Render("code");

        TestAssert.True(output.Contains(headingExpected, StringComparison.Ordinal), "HeadingStyle should apply to heading segments.");
        TestAssert.True(output.Contains(listMarkerExpected, StringComparison.Ordinal), "ListMarkerStyle should apply to list marker segments.");
        TestAssert.True(output.Contains(quoteMarkerExpected, StringComparison.Ordinal), "QuoteMarkerStyle should apply to quote marker segments.");
        TestAssert.True(output.Contains(plainExpected, StringComparison.Ordinal), "TextStyle should apply to plain segments.");
        TestAssert.True(output.Contains(emphasisExpected, StringComparison.Ordinal), "EmphasisStyle should apply to inline-emphasis segments.");
        TestAssert.True(output.Contains(strongExpected, StringComparison.Ordinal), "StrongStyle should apply to strong inline segments.");
        TestAssert.True(output.Contains(codeExpected, StringComparison.Ordinal), "InlineCodeStyle should apply to inline-code segments.");
    }

    [Test]
    public void RichTextView_DefaultRender_IsDeterministicAndMonochrome()
    {
        var control = new RichTextView
        {
            Border = BorderStyle.None,
        };
        control.SetLines(
        [
            [RichTextSegment.Heading("Docs", 1)],
            [RichTextSegment.ListMarker("-"), RichTextSegment.Plain("first item")],
            [RichTextSegment.QuoteMarker(">"), RichTextSegment.Plain("note")],
        ]);
        var bounds = new Rect(0, 0, 40, 6);
        var firstCanvas = new Canvas(40, 6);
        var secondCanvas = new Canvas(40, 6);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        TestAssert.Equal(first, second, "RichTextView should render deterministically for identical state.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default RichTextView output should be monochrome.");
    }

    [Test]
    public void RichTextView_WrapAndKeyboardScrolling_AreDeterministic()
    {
        var control = new RichTextView
        {
            Border = BorderStyle.None,
            Wrap = true,
            IsFocused = true,
        };
        control.SetLines(
        [
            [RichTextSegment.Plain("line 0000 long content for wrap")],
            [RichTextSegment.Plain("line 0001 long content for wrap")],
            [RichTextSegment.Plain("line 0002 long content for wrap")],
            [RichTextSegment.Plain("line 0003 long content for wrap")],
            [RichTextSegment.Plain("line 0004 long content for wrap")],
            [RichTextSegment.Plain("line 0005 long content for wrap")],
        ]);

        _ = Render(control, width: 14, height: 3);

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var endHandled = control.Handle(new KeyPressed(Key.End));
        var upHandled = control.Handle(new KeyPressed(Key.Up));
        var homeHandled = control.Handle(new KeyPressed(Key.Home));

        TestAssert.True(downHandled, "Down should be handled while focused.");
        TestAssert.True(endHandled, "End should be handled while focused.");
        TestAssert.True(upHandled, "Up should be handled while focused.");
        TestAssert.True(homeHandled, "Home should be handled while focused.");
        TestAssert.Equal(0, control.ScrollOffset, "Home should reset scroll offset.");
    }

    private static string Render(RichTextView control, int width, int height, CanvasTextMode textMode = CanvasTextMode.Fast)
    {
        var canvas = new Canvas(width, height, textMode);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
