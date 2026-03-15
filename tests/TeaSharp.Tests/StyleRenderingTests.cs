using TeaSharp.Components.Advanced;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Rendering;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class StyleRenderingTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Style_Merge_ComposesAttributes", Style_Merge_ComposesAttributes);
        yield return new TestCase("Style_Render_BlinkAndStrikethrough_EmitsSgr", Style_Render_BlinkAndStrikethrough_EmitsSgr);
        yield return new TestCase("Style_Render_ConcealAndOverline_EmitsSgr", Style_Render_ConcealAndOverline_EmitsSgr);
        yield return new TestCase("Style_Render_DoubleUnderlineAndFrame_EmitsSgr", Style_Render_DoubleUnderlineAndFrame_EmitsSgr);
        yield return new TestCase("Style_Render_EmptyStyle_Passthrough", Style_Render_EmptyStyle_Passthrough);
        yield return new TestCase("Renderer_StyledContent_EmitsSgrSequences", Renderer_StyledContent_EmitsSgrSequences);
        yield return new TestCase("Renderer_StyleOnlyChange_TriggersDiffPatch", Renderer_StyleOnlyChange_TriggersDiffPatch);
        yield return new TestCase("Renderer_BlinkStrikethroughChange_TriggersDiffPatch", Renderer_BlinkStrikethroughChange_TriggersDiffPatch);
        yield return new TestCase("Renderer_ConcealOverlineChange_TriggersDiffPatch", Renderer_ConcealOverlineChange_TriggersDiffPatch);
        yield return new TestCase("Renderer_DoubleUnderlineFrameChange_TriggersDiffPatch", Renderer_DoubleUnderlineFrameChange_TriggersDiffPatch);
    }

    private static Task Style_Merge_ComposesAttributes()
    {
        // Arrange
        var baseStyle = TeaStyle.Empty
            .WithBold()
            .WithForeground(AnsiColor.Indexed(33));
        var overlay = TeaStyle.Empty
            .WithUnderline()
            .WithBackground(AnsiColor.Rgb(10, 20, 30));

        // Act
        var merged = baseStyle.Merge(overlay);
        var rendered = merged.Render("X");

        // Assert
        TestAssert.Equal(
            "\u001b[1;4;38;5;33;48;2;10;20;30mX\u001b[0m",
            rendered,
            "Merged style should emit composed SGR sequence.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_EmptyStyle_Passthrough()
    {
        // Arrange
        const string text = "plain";

        // Act
        var rendered = TeaStyle.Empty.Render(text);

        // Assert
        TestAssert.Equal(text, rendered, "Empty style should not wrap text.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_BlinkAndStrikethrough_EmitsSgr()
    {
        // Arrange
        var style = TeaStyle.Empty
            .WithBlink()
            .WithStrikethrough()
            .WithForeground(AnsiColor.BrightYellow);

        // Act
        var rendered = style.Render("warn");

        // Assert
        TestAssert.Equal(
            "\u001b[5;9;38;5;11mwarn\u001b[0m",
            rendered,
            "Blink + strikethrough should be encoded in SGR output.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_ConcealAndOverline_EmitsSgr()
    {
        // Arrange
        var style = TeaStyle.Empty
            .WithConceal()
            .WithOverline()
            .WithForeground(AnsiColor.BrightCyan);

        // Act
        var rendered = style.Render("masked");

        // Assert
        TestAssert.Equal(
            "\u001b[8;53;38;5;14mmasked\u001b[0m",
            rendered,
            "Conceal + overline should be encoded in SGR output.");
        return Task.CompletedTask;
    }

    private static Task Style_Render_DoubleUnderlineAndFrame_EmitsSgr()
    {
        // Arrange
        var style = TeaStyle.Empty
            .WithDoubleUnderline()
            .WithFramed()
            .WithForeground(AnsiColor.BrightMagenta);

        // Act
        var rendered = style.Render("boxed");

        // Assert
        TestAssert.Equal(
            "\u001b[21;51;38;5;13mboxed\u001b[0m",
            rendered,
            "Double underline + frame should be encoded in SGR output.");
        return Task.CompletedTask;
    }

    private static async Task Renderer_StyledContent_EmitsSgrSequences()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var style = TeaStyle.Empty.WithBold().WithForeground(AnsiColor.BrightGreen);

        // Act
        renderer.Render(ScreenOutput.From(style.Render("ok")));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[1;38;5;10m");
        AssertContains(rendered, "ok");
        AssertContains(rendered, "\u001b[0m");
    }

    private static async Task Renderer_BlinkStrikethroughChange_TriggersDiffPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var plain = TeaStyle.Empty.WithForeground(AnsiColor.BrightYellow);
        var emphasized = TeaStyle.Empty.WithBlink().WithStrikethrough().WithForeground(AnsiColor.BrightYellow);
        renderer.Render(ScreenOutput.From(plain.Render("!")));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From(emphasized.Render("!")));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;1H");
        AssertContains(patch, "\u001b[5;9;38;5;11m");
        AssertContains(patch, "!");
        AssertContains(patch, "\u001b[0m");
    }

    private static async Task Renderer_ConcealOverlineChange_TriggersDiffPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var plain = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan);
        var emphasized = TeaStyle.Empty.WithConceal().WithOverline().WithForeground(AnsiColor.BrightCyan);
        renderer.Render(ScreenOutput.From(plain.Render("x")));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From(emphasized.Render("x")));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;1H");
        AssertContains(patch, "\u001b[8;53;38;5;14m");
        AssertContains(patch, "x");
        AssertContains(patch, "\u001b[0m");
    }

    private static async Task Renderer_DoubleUnderlineFrameChange_TriggersDiffPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var plain = TeaStyle.Empty.WithForeground(AnsiColor.BrightMagenta);
        var emphasized = TeaStyle.Empty.WithDoubleUnderline().WithFramed().WithForeground(AnsiColor.BrightMagenta);
        renderer.Render(ScreenOutput.From(plain.Render("b")));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From(emphasized.Render("b")));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;1H");
        AssertContains(patch, "\u001b[21;51;38;5;13m");
        AssertContains(patch, "b");
        AssertContains(patch, "\u001b[0m");
    }

    private static async Task Renderer_StyleOnlyChange_TriggersDiffPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        var red = TeaStyle.Empty.WithForeground(AnsiColor.BrightRed);
        var green = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen);
        renderer.Render(ScreenOutput.From(red.Render("A")));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From(green.Render("A")));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;1H");
        AssertContains(patch, "\u001b[38;5;10m");
        AssertContains(patch, "A");
        AssertContains(patch, "\u001b[0m");
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
            .Replace("\u001b", "\\u001b", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }
}
