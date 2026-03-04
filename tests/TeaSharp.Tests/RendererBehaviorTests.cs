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

    private static string ReadUtf8(MemoryStream output)
    {
        return Encoding.UTF8.GetString(output.ToArray());
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
