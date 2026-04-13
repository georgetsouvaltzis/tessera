using System.Text;
using Tessera.Core.Abstractions;
using Tessera.Core.Rendering;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class StyleRenderingTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Style_Merge_ComposesAttributes", Style_Merge_ComposesAttributes);
        yield return new TestCase("Style_FontWeight_MapsToBoldAndDimFlags", Style_FontWeight_MapsToBoldAndDimFlags);
        yield return new TestCase("Style_FontWeight_MergeOverridesWeightFlags",
            Style_FontWeight_MergeOverridesWeightFlags);
        yield return new TestCase("Style_Render_BlinkAndStrikethrough_EmitsSgr",
            Style_Render_BlinkAndStrikethrough_EmitsSgr);
        yield return new TestCase("Style_Render_ConcealAndOverline_EmitsSgr", Style_Render_ConcealAndOverline_EmitsSgr);
        yield return new TestCase("Style_Render_DoubleUnderlineAndFrame_EmitsSgr",
            Style_Render_DoubleUnderlineAndFrame_EmitsSgr);
        yield return new TestCase("Style_ToEscapeSequence_CachesPerStyleValue",
            Style_ToEscapeSequence_CachesPerStyleValue);
        yield return new TestCase("Style_Render_DisabledFlags_PreserveResetSemantics",
            Style_Render_DisabledFlags_PreserveResetSemantics);
        yield return new TestCase("Style_Render_EmptyStyle_Passthrough", Style_Render_EmptyStyle_Passthrough);
        yield return new TestCase("Renderer_StyledContent_EmitsSgrSequences", Renderer_StyledContent_EmitsSgrSequences);
        yield return new TestCase("Renderer_StyleOnlyChange_TriggersDiffPatch",
            Renderer_StyleOnlyChange_TriggersDiffPatch);
        yield return new TestCase("Renderer_BlinkStrikethroughChange_TriggersDiffPatch",
            Renderer_BlinkStrikethroughChange_TriggersDiffPatch);
        yield return new TestCase("Renderer_ConcealOverlineChange_TriggersDiffPatch",
            Renderer_ConcealOverlineChange_TriggersDiffPatch);
        yield return new TestCase("Renderer_DoubleUnderlineFrameChange_TriggersDiffPatch",
            Renderer_DoubleUnderlineFrameChange_TriggersDiffPatch);
    }

    private static Task Style_Merge_ComposesAttributes()
    {
        // Arrange
        var baseStyle = TesseraStyle.Empty
            .WithBold()
            .WithForeground(AnsiColor.Indexed(33));
        var overlay = TesseraStyle.Empty
            .WithUnderline()
            .WithBackground(AnsiColor.Rgb(10, 20, 30));

        // Act
        var merged = baseStyle.Merge(overlay);
        var rendered = merged.Render("X");

        // Assert
        TestAssert.Equal(
            "\e[1;4;38;5;33;48;2;10;20;30mX\e[0m",
            rendered,
            "Merged style should emit composed SGR sequence.");
        return Task.CompletedTask;
    }

    private static Task Style_FontWeight_MapsToBoldAndDimFlags()
    {
        var normal = TesseraStyle.Empty.WithFontWeight(TesseraFontWeight.Normal);
        var bold = TesseraStyle.Empty.WithFontWeight(TesseraFontWeight.Bold);
        var dim = TesseraStyle.Empty.WithFontWeight(TesseraFontWeight.Dim);

        TestAssert.True(normal.Bold is false && normal.Dim is false,
            "Normal font weight should disable bold/dim emphasis flags.");
        TestAssert.True(bold.Bold is true && bold.Dim is false,
            "Bold font weight should enable bold and disable dim emphasis flags.");
        TestAssert.True(dim.Bold is false && dim.Dim is true,
            "Dim font weight should disable bold and enable dim emphasis flags.");
        return Task.CompletedTask;
    }

    private static Task Style_FontWeight_MergeOverridesWeightFlags()
    {
        var baseStyle = TesseraStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen);
        var overlay = TesseraStyle.Empty.WithFontWeight(TesseraFontWeight.Dim);

        var merged = baseStyle.Merge(overlay);
        var rendered = merged.Render("X");

        TestAssert.Equal(
            "\e[22;2;38;5;10mX\e[0m",
            rendered,
            "Font weight overlay should override prior bold emphasis using SGR weight flags.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_EmptyStyle_Passthrough()
    {
        // Arrange
        const string text = "plain";

        // Act
        var rendered = TesseraStyle.Empty.Render(text);

        // Assert
        TestAssert.Equal(text, rendered, "Empty style should not wrap text.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_BlinkAndStrikethrough_EmitsSgr()
    {
        // Arrange
        var style = TesseraStyle.Empty
            .WithBlink()
            .WithStrikethrough()
            .WithForeground(AnsiColor.BrightYellow);

        // Act
        var rendered = style.Render("warn");

        // Assert
        TestAssert.Equal(
            "\e[5;9;38;5;11mwarn\e[0m",
            rendered,
            "Blink + strikethrough should be encoded in SGR output.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_ConcealAndOverline_EmitsSgr()
    {
        // Arrange
        var style = TesseraStyle.Empty
            .WithConceal()
            .WithOverline()
            .WithForeground(AnsiColor.BrightCyan);

        // Act
        var rendered = style.Render("masked");

        // Assert
        TestAssert.Equal(
            "\e[8;53;38;5;14mmasked\e[0m",
            rendered,
            "Conceal + overline should be encoded in SGR output.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_DoubleUnderlineAndFrame_EmitsSgr()
    {
        // Arrange
        var style = TesseraStyle.Empty
            .WithDoubleUnderline()
            .WithFramed()
            .WithForeground(AnsiColor.BrightMagenta);

        // Act
        var rendered = style.Render("boxed");

        // Assert
        TestAssert.Equal(
            "\e[21;51;38;5;13mboxed\e[0m",
            rendered,
            "Double underline + frame should be encoded in SGR output.");
        return Task.CompletedTask;
    }

    private static Task Style_ToEscapeSequence_CachesPerStyleValue()
    {
        var firstStyle = TesseraStyle.Empty
            .WithBold()
            .WithForeground(AnsiColor.BrightGreen);
        var secondStyle = TesseraStyle.Empty
            .WithUnderline()
            .WithForeground(AnsiColor.BrightGreen);

        var firstA = firstStyle.ToEscapeSequence();
        var firstB = firstStyle.ToEscapeSequence();
        var second = secondStyle.ToEscapeSequence();

        TestAssert.True(
            ReferenceEquals(firstA, firstB),
            "ToEscapeSequence should reuse cached escape string for the same style value.");
        TestAssert.True(
            !ReferenceEquals(firstA, second),
            "Distinct style values should not share cached escape string instances.");
        TestAssert.Equal("\e[1;38;5;10m", firstA, "Cached style escape output should remain correct.");
        TestAssert.Equal("\e[4;38;5;10m", second, "Distinct style escape output should remain correct.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_DisabledFlags_PreserveResetSemantics()
    {
        var style = TesseraStyle.Empty
            .WithBold(false)
            .WithUnderline(false)
            .WithInverse(false)
            .WithForeground(AnsiColor.BrightRed);

        var rendered = style.Render("x");
        TestAssert.Equal(
            "\e[22;24;27;38;5;9mx\e[0m",
            rendered,
            "Render should preserve exact reset/open semantics for explicitly disabled flags.");
        return Task.CompletedTask;
    }

    private static async Task Renderer_StyledContent_EmitsSgrSequences()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var style = TesseraStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen);

        // Act
        renderer.Render(ScreenOutput.From(style.Render("ok")));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\e[1;38;5;10m");
        AssertContains(rendered, "ok");
        AssertContains(rendered, "\e[0m");
    }

    private static async Task Renderer_BlinkStrikethroughChange_TriggersDiffPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var plain = TesseraStyle.Empty.WithForeground(AnsiColor.BrightYellow);
        var emphasized = TesseraStyle.Empty.WithBlink().WithStrikethrough().WithForeground(AnsiColor.BrightYellow);
        renderer.Render(ScreenOutput.From(plain.Render("!")));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From(emphasized.Render("!")));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\e[1;1H");
        AssertContains(patch, "\e[5;9;38;5;11m");
        AssertContains(patch, "!");
        AssertContains(patch, "\e[0m");
    }

    private static async Task Renderer_ConcealOverlineChange_TriggersDiffPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var plain = TesseraStyle.Empty.WithForeground(AnsiColor.BrightCyan);
        var emphasized = TesseraStyle.Empty.WithConceal().WithOverline().WithForeground(AnsiColor.BrightCyan);
        renderer.Render(ScreenOutput.From(plain.Render("x")));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From(emphasized.Render("x")));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\e[1;1H");
        AssertContains(patch, "\e[8;53;38;5;14m");
        AssertContains(patch, "x");
        AssertContains(patch, "\e[0m");
    }

    private static async Task Renderer_DoubleUnderlineFrameChange_TriggersDiffPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var plain = TesseraStyle.Empty.WithForeground(AnsiColor.BrightMagenta);
        var emphasized = TesseraStyle.Empty.WithDoubleUnderline().WithFramed().WithForeground(AnsiColor.BrightMagenta);
        renderer.Render(ScreenOutput.From(plain.Render("b")));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From(emphasized.Render("b")));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\e[1;1H");
        AssertContains(patch, "\e[21;51;38;5;13m");
        AssertContains(patch, "b");
        AssertContains(patch, "\e[0m");
    }

    private static async Task Renderer_StyleOnlyChange_TriggersDiffPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var red = TesseraStyle.Empty.WithForeground(AnsiColor.BrightRed);
        var green = TesseraStyle.Empty.WithForeground(AnsiColor.BrightGreen);
        renderer.Render(ScreenOutput.From(red.Render("A")));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From(green.Render("A")));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\e[1;1H");
        AssertContains(patch, "\e[38;5;10m");
        AssertContains(patch, "A");
        AssertContains(patch, "\e[0m");
    }

    private static string ReadUtf8(MemoryStream output)
    {
        return Encoding.UTF8.GetString(output.ToArray());
    }

    private static string ReadUtf8(MemoryStream output, long offset)
    {
        var bytes = output.ToArray();
        if (offset >= bytes.Length)
        {
            return string.Empty;
        }

        return Encoding.UTF8.GetString(bytes.AsSpan((int)offset));
    }

    private static void AssertContains(string actual, string expectedFragment)
    {
        if (!actual.Contains(expectedFragment, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Expected output to contain '{Escape(expectedFragment)}'.");
        }
    }

    private static string Escape(string text)
    {
        return text
            .Replace("\e", "\\e", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }
}
