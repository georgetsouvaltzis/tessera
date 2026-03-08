using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Rendering;

namespace TeaSharp.Tests;

internal static class RendererSnapshotTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Renderer_Snapshot_FirstFrame_ModesAndTitle", Snapshot_FirstFrame_ModesAndTitle);
        yield return new TestCase("Renderer_Snapshot_SecondFrame_EmitsMinimalPatch", Snapshot_SecondFrame_EmitsMinimalPatch);
        yield return new TestCase("Renderer_Snapshot_Reset_EmitsTeardownModes", Snapshot_Reset_EmitsTeardownModes);
    }

    private static async Task Snapshot_FirstFrame_ModesAndTitle()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Resize(width: 6, height: 3);

        // Act
        renderer.Render(View.From("ab\ncd") with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            EnableSynchronizedUpdates = true,
            MouseMode = MouseMode.AllMotion,
            WindowTitle = "Snap",
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = NormalizeOutput(ReadUtf8(output));

        // Assert
        const string expected =
            "<ESC>[?1049h<ESC>[?2004h<ESC>[?2004$p<ESC>[?1004h<ESC>[?1004$p" +
            "<ESC>[?2026h<ESC>[?2026$p<ESC>[?1000h<ESC>[?1002l<ESC>[?1003h<ESC>[?1006h<ESC>[?1006$p" +
            "<ESC>]2;Snap<BEL><ESC>[>1u<ESC>[2J<ESC>[H<ESC>[1;1Hab<ESC>[2;1Hcd<ESC>[?25l<ESC>[?2026l";
        TestAssert.Equal(expected, rendered, "First frame snapshot should match deterministic control sequence order.");
    }

    private static async Task Snapshot_SecondFrame_EmitsMinimalPatch()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Resize(width: 6, height: 3);
        var baseView = View.From("ab\ncd") with
        {
            EnableSynchronizedUpdates = true,
        };
        renderer.Render(baseView);
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(baseView with { Content = "az\ncd" });
        await renderer.FlushAsync(CancellationToken.None);
        var patch = NormalizeOutput(ReadUtf8(output, marker));

        // Assert
        const string expected = "<ESC>[?2026h<ESC>[1;2Hz<ESC>[?25l<ESC>[?2026l";
        TestAssert.Equal(expected, patch, "Second frame should only emit changed cell run plus sync wrappers.");
    }

    private static async Task Snapshot_Reset_EmitsTeardownModes()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Render(View.From("snap") with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            MouseMode = MouseMode.AllMotion,
        });
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        await renderer.ResetAsync(CancellationToken.None);
        var reset = NormalizeOutput(ReadUtf8(output, marker));

        // Assert
        const string expected =
            "<ESC>[0m<ESC>[?25h<ESC>[>0u<ESC>[?2004l<ESC>[?1004l<ESC>[?1000l<ESC>[?1002l<ESC>[?1003l<ESC>[?1006l<ESC>[?1049l";
        TestAssert.Equal(expected, reset, "Reset snapshot should disable all enabled terminal modes.");
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

    private static string NormalizeOutput(string raw)
    {
        return raw
            .Replace("\u001b", "<ESC>", StringComparison.Ordinal)
            .Replace("\u0007", "<BEL>", StringComparison.Ordinal)
            .Replace("\r", "<CR>", StringComparison.Ordinal)
            .Replace("\n", "<LF>", StringComparison.Ordinal);
    }
}
