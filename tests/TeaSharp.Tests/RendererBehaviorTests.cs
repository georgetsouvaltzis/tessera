using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Rendering;

namespace TeaSharp.Tests;

internal static class RendererBehaviorTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Renderer_MouseModeCellMotion_EmitsEnableSequences", MouseModeCellMotion_EmitsEnableSequences);
        yield return new TestCase("Renderer_MouseModeAllMotion_EmitsEnableSequences", MouseModeAllMotion_EmitsEnableSequences);
        yield return new TestCase("Renderer_Reset_DisablesMouseModes", Reset_DisablesMouseModes);
        yield return new TestCase("Renderer_CellDiff_UpdatesOnlyChangedCellRun", CellDiff_UpdatesOnlyChangedCellRun);
        yield return new TestCase("Renderer_CellDiff_ClearsShortenedLineTail", CellDiff_ClearsShortenedLineTail);
        yield return new TestCase("Renderer_Resize_ClipsToWidth", Resize_ClipsToWidth);
        yield return new TestCase("Renderer_Resize_DropsWideRuneAtBoundary", Resize_DropsWideRuneAtBoundary);
        yield return new TestCase("Renderer_CellDiff_CombiningGrapheme_PatchesSingleColumn", CellDiff_CombiningGrapheme_PatchesSingleColumn);
    }

    private static async Task MouseModeCellMotion_EmitsEnableSequences()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(View.From("mouse") with { MouseMode = MouseMode.CellMotion });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[?1000h");
        AssertContains(rendered, "\u001b[?1002h");
        AssertContains(rendered, "\u001b[?1003l");
        AssertContains(rendered, "\u001b[?1006h");
    }

    private static async Task MouseModeAllMotion_EmitsEnableSequences()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(View.From("mouse") with { MouseMode = MouseMode.AllMotion });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[?1000h");
        AssertContains(rendered, "\u001b[?1002l");
        AssertContains(rendered, "\u001b[?1003h");
        AssertContains(rendered, "\u001b[?1006h");
    }

    private static async Task Reset_DisablesMouseModes()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        renderer.Render(View.From("mouse") with { MouseMode = MouseMode.AllMotion });
        await renderer.FlushAsync(CancellationToken.None);

        // Act
        await renderer.ResetAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[?1000l");
        AssertContains(rendered, "\u001b[?1002l");
        AssertContains(rendered, "\u001b[?1003l");
        AssertContains(rendered, "\u001b[?1006l");
    }

    private static async Task CellDiff_UpdatesOnlyChangedCellRun()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Render(View.From("abc"));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(View.From("axc"));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;2H");
        AssertContains(patch, "x");
        AssertDoesNotContain(patch, "\u001b[1;1Haxc");
    }

    private static async Task CellDiff_ClearsShortenedLineTail()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Render(View.From("hello"));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(View.From("he"));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;3H");
        AssertContains(patch, "   ");
    }

    private static async Task Resize_ClipsToWidth()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Resize(width: 3, height: 5);

        // Act
        renderer.Render(View.From("abcdef"));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "abc");
        AssertDoesNotContain(rendered, "abcdef");
    }

    private static async Task Resize_DropsWideRuneAtBoundary()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Resize(width: 3, height: 5);

        // Act
        renderer.Render(View.From("ab好"));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "ab");
        AssertDoesNotContain(rendered, "好");
    }

    private static async Task CellDiff_CombiningGrapheme_PatchesSingleColumn()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Render(View.From("Cafe\u0301"));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(View.From("Cafe\u0300"));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;4H");
        AssertContains(patch, "e\u0300");
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

    private static void AssertDoesNotContain(string actual, string fragment)
    {
        if (actual.Contains(fragment, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Expected output to exclude '{Escape(fragment)}'.");
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
