using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.Text;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Tests;

internal static class RendererBehaviorTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Renderer_MouseModeCellMotion_EmitsEnableSequences", MouseModeCellMotion_EmitsEnableSequences);
        yield return new TestCase("Renderer_MouseModeAllMotion_EmitsEnableSequences", MouseModeAllMotion_EmitsEnableSequences);
        yield return new TestCase("Renderer_Reset_DisablesMouseModes", Reset_DisablesMouseModes);
        yield return new TestCase("Renderer_UnsupportedCapabilities_SuppressModeSequences", UnsupportedCapabilities_SuppressModeSequences);
        yield return new TestCase("Renderer_ModeReportQueries_EmittedForEnabledFeatures", ModeReportQueries_EmittedForEnabledFeatures);
        yield return new TestCase("Renderer_ModeReportQueries_EmittedOncePerMode", ModeReportQueries_EmittedOncePerMode);
        yield return new TestCase("Renderer_ModeReportQueries_ConfigurableDisable_SkipsModeQueries", ModeReportQueries_ConfigurableDisable_SkipsModeQueries);
        yield return new TestCase("Renderer_ModeReportQueries_RepeatWhenOncePerModeDisabled", ModeReportQueries_RepeatWhenOncePerModeDisabled);
        yield return new TestCase("Renderer_ModeReportsDisabled_SkipsModeQueries", ModeReportsDisabled_SkipsModeQueries);
        yield return new TestCase("Renderer_SynchronizedUpdates_WrapFrameOutput", SynchronizedUpdates_WrapFrameOutput);
        yield return new TestCase("Renderer_SynchronizedUpdates_Disabled_DoesNotWrapFrameOutput", SynchronizedUpdates_Disabled_DoesNotWrapFrameOutput);
        yield return new TestCase("Renderer_CursorStyle_EmitsDecscusrWhenCursorVisible", CursorStyle_EmitsDecscusrWhenCursorVisible);
        yield return new TestCase("Renderer_CursorStyle_Unchanged_DoesNotRepeatSequence", CursorStyle_Unchanged_DoesNotRepeatSequence);
        yield return new TestCase("Renderer_Reset_RestoresDefaultCursorStyle", Reset_RestoresDefaultCursorStyle);
        yield return new TestCase("Renderer_TerminalColors_EmitsOscColorSequences", TerminalColors_EmitsOscColorSequences);
        yield return new TestCase("Renderer_Progress_EmitsOscProgressSequences", Progress_EmitsOscProgressSequences);
        yield return new TestCase("Renderer_KeyboardEnhancements_EmitsKittySequence", KeyboardEnhancements_EmitsKittySequence);
        yield return new TestCase("Renderer_KeyboardEnhancements_CanDisableKittyBaseFlag", KeyboardEnhancements_CanDisableKittyBaseFlag);
        yield return new TestCase("Renderer_CellDiff_UpdatesOnlyChangedCellRun", CellDiff_UpdatesOnlyChangedCellRun);
        yield return new TestCase("Renderer_CellDiff_ClearsShortenedLineTail", CellDiff_ClearsShortenedLineTail);
        yield return new TestCase("Renderer_Resize_ClipsToWidth", Resize_ClipsToWidth);
        yield return new TestCase("Renderer_Resize_HeightClip_KeepsBottomRows", Resize_HeightClip_KeepsBottomRows);
        yield return new TestCase("Renderer_Resize_WrapsLongLines", Resize_WrapsLongLines);
        yield return new TestCase("Renderer_Resize_WrapsWideRuneAtBoundary", Resize_WrapsWideRuneAtBoundary);
        yield return new TestCase("Renderer_CellDiff_CombiningGrapheme_PatchesSingleColumn", CellDiff_CombiningGrapheme_PatchesSingleColumn);
        yield return new TestCase("Renderer_CellBuffer_ClearsWideContinuation_WhenReplacingWithNarrowCells", CellBuffer_ClearsWideContinuation_WhenReplacingWithNarrowCells);
    }

    private static async Task MouseModeCellMotion_EmitsEnableSequences()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("mouse") with
        {
            Terminal = new TerminalOutput
            {
                MouseMode = MouseMode.CellMotion,
            },
        });
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
        renderer.Render(ScreenOutput.From("mouse") with
        {
            Terminal = new TerminalOutput
            {
                MouseMode = MouseMode.AllMotion,
            },
        });
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

        renderer.Render(ScreenOutput.From("mouse") with
        {
            Terminal = new TerminalOutput
            {
                MouseMode = MouseMode.AllMotion,
            },
        });
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

    private static async Task UnsupportedCapabilities_SuppressModeSequences()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer(new TerminalCapabilityProfile(
            FocusReporting: false,
            MouseReporting: false,
            BracketedPaste: false,
            SynchronizedUpdates: false,
            ModeReports: false,
            Source: "test"));
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("probe") with
        {
            Terminal = new TerminalOutput
            {
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                EnableSynchronizedUpdates = true,
                MouseMode = MouseMode.AllMotion,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertDoesNotContain(rendered, "\u001b[?2004h");
        AssertDoesNotContain(rendered, "\u001b[?1004h");
        AssertDoesNotContain(rendered, "\u001b[?1000h");
        AssertDoesNotContain(rendered, "\u001b[?1006h");
        AssertDoesNotContain(rendered, "\u001b[?2026h");
        AssertDoesNotContain(rendered, "\u001b[?2004$p");
        AssertDoesNotContain(rendered, "\u001b[?1004$p");
        AssertDoesNotContain(rendered, "\u001b[?1006$p");
        AssertDoesNotContain(rendered, "\u001b[?2026$p");
    }

    private static async Task ModeReportQueries_EmittedForEnabledFeatures()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("probe") with
        {
            Terminal = new TerminalOutput
            {
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                EnableSynchronizedUpdates = true,
                MouseMode = MouseMode.AllMotion,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[?2004$p");
        AssertContains(rendered, "\u001b[?1004$p");
        AssertContains(rendered, "\u001b[?2026$p");
        AssertContains(rendered, "\u001b[?1006$p");
    }

    private static async Task ModeReportQueries_EmittedOncePerMode()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("first") with
        {
            Terminal = new TerminalOutput
            {
                EnableFocusReporting = true,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        renderer.Render(ScreenOutput.From("second") with
        {
            Terminal = new TerminalOutput
            {
                EnableFocusReporting = true,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertCount(rendered, "\u001b[?1004$p", 1);
    }

    private static async Task ModeReportQueries_ConfigurableDisable_SkipsModeQueries()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer(
            options: new AnsiRendererOptions
            {
                QueryModeReports = false,
            });
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("probe") with
        {
            Terminal = new TerminalOutput
            {
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                EnableSynchronizedUpdates = true,
                MouseMode = MouseMode.AllMotion,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[?2004h");
        AssertContains(rendered, "\u001b[?1004h");
        AssertContains(rendered, "\u001b[?1006h");
        AssertContains(rendered, "\u001b[?2026h");
        AssertDoesNotContain(rendered, "$p");
    }

    private static async Task ModeReportQueries_RepeatWhenOncePerModeDisabled()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer(
            options: new AnsiRendererOptions
            {
                QueryModeReportsOncePerMode = false,
            });
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("sync-a") with
        {
            Terminal = new TerminalOutput
            {
                EnableSynchronizedUpdates = true,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        renderer.Render(ScreenOutput.From("sync-b") with
        {
            Terminal = new TerminalOutput
            {
                EnableSynchronizedUpdates = true,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertCount(rendered, "\u001b[?2026$p", 2);
    }

    private static async Task ModeReportsDisabled_SkipsModeQueries()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer(
            new TerminalCapabilityProfile(ModeReports: false, Source: "test"));
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("probe") with
        {
            Terminal = new TerminalOutput
            {
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                EnableSynchronizedUpdates = true,
                MouseMode = MouseMode.AllMotion,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[?2004h");
        AssertContains(rendered, "\u001b[?1004h");
        AssertContains(rendered, "\u001b[?1006h");
        AssertContains(rendered, "\u001b[?2026h");
        AssertDoesNotContain(rendered, "$p");
    }

    private static async Task SynchronizedUpdates_WrapFrameOutput()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("sync") with
        {
            Terminal = new TerminalOutput
            {
                EnableSynchronizedUpdates = true,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[?2026h");
        AssertContains(rendered, "\u001b[?2026l");
        AssertBefore(rendered, "\u001b[?2026h", "\u001b[?2026l");
    }

    private static async Task SynchronizedUpdates_Disabled_DoesNotWrapFrameOutput()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("nosync") with
        {
            Terminal = new TerminalOutput
            {
                EnableSynchronizedUpdates = false,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertDoesNotContain(rendered, "\u001b[?2026h");
        AssertDoesNotContain(rendered, "\u001b[?2026l");
    }

    private static async Task CursorStyle_EmitsDecscusrWhenCursorVisible()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("cursor") with
        {
            Frame = ScreenFrame.From("cursor") with
            {
                CursorX = 2,
                CursorY = 1,
                CursorStyle = CursorStyle.SteadyBar,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[6 q");
        AssertContains(rendered, "\u001b[?25h");
        AssertContains(rendered, "\u001b[2;3H");
    }

    private static async Task CursorStyle_Unchanged_DoesNotRepeatSequence()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        var view = ScreenOutput.From("cursor") with
        {
            Frame = ScreenFrame.From("cursor") with
            {
                CursorX = 0,
                CursorY = 0,
                CursorStyle = CursorStyle.BlinkingUnderline,
            },
        };
        renderer.Render(view);
        await renderer.FlushAsync(CancellationToken.None);
        renderer.Render(view.WithContent("cursor2"));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertCount(rendered, "\u001b[3 q", 1);
    }

    private static async Task Reset_RestoresDefaultCursorStyle()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Render(ScreenOutput.From("cursor") with
        {
            Frame = ScreenFrame.From("cursor") with
            {
                CursorX = 0,
                CursorY = 0,
                CursorStyle = CursorStyle.SteadyUnderline,
            },
        });
        await renderer.FlushAsync(CancellationToken.None);

        // Act
        await renderer.ResetAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[4 q");
        AssertContains(rendered, "\u001b[0 q");
    }

    private static async Task TerminalColors_EmitsOscColorSequences()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("colors") with
        {
            Terminal = new TerminalOutput
            {
                ForegroundColor = "#112233",
                BackgroundColor = "rgb:44/55/66",
                CursorColor = "#abcdef",
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        await renderer.ResetAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b]10;#112233\u001b\\");
        AssertContains(rendered, "\u001b]11;#445566\u001b\\");
        AssertContains(rendered, "\u001b]12;#ABCDEF\u001b\\");
        AssertContains(rendered, "\u001b]110;\u001b\\");
        AssertContains(rendered, "\u001b]111;\u001b\\");
        AssertContains(rendered, "\u001b]112;\u001b\\");
    }

    private static async Task Progress_EmitsOscProgressSequences()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("progress") with
        {
            Terminal = new TerminalOutput
            {
                Progress = new TerminalProgress(TerminalProgressState.Warning, 61),
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        renderer.Render(ScreenOutput.From("progress") with
        {
            Terminal = new TerminalOutput
            {
                Progress = new TerminalProgress(TerminalProgressState.Indeterminate, 0),
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        await renderer.ResetAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b]9;4;4;61\u001b\\");
        AssertContains(rendered, "\u001b]9;4;3\u001b\\");
        AssertContains(rendered, "\u001b]9;4;0\u001b\\");
    }

    private static async Task KeyboardEnhancements_EmitsKittySequence()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("keys") with
        {
            Terminal = new TerminalOutput
            {
                KeyboardEnhancements = new KeyboardEnhancementOptions { ReportEventTypes = true },
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        await renderer.ResetAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[>3u");
        AssertContains(rendered, "\u001b[>0u");
    }

    private static async Task KeyboardEnhancements_CanDisableKittyBaseFlag()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer(
            options: new AnsiRendererOptions
            {
                IncludeKittyKeyboardBaseFlag = false,
            });
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);

        // Act
        renderer.Render(ScreenOutput.From("keys") with
        {
            Terminal = new TerminalOutput
            {
                KeyboardEnhancements = new KeyboardEnhancementOptions { ReportEventTypes = true },
            },
        });
        await renderer.FlushAsync(CancellationToken.None);
        await renderer.ResetAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[>2u");
        AssertContains(rendered, "\u001b[>0u");
        AssertDoesNotContain(rendered, "\u001b[>3u");
    }

    private static async Task CellDiff_UpdatesOnlyChangedCellRun()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Render(ScreenOutput.From("abc"));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From("axc"));
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
        renderer.Render(ScreenOutput.From("hello"));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From("he"));
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
        renderer.Render(ScreenOutput.From("abcdef"));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "abc");
        AssertDoesNotContain(rendered, "abcdef");
    }

    private static async Task Resize_HeightClip_KeepsBottomRows()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Resize(width: 4, height: 2);

        // Act
        renderer.Render(ScreenOutput.From("row1\nrow2\nrow3"));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "row2");
        AssertContains(rendered, "row3");
        AssertDoesNotContain(rendered, "row1");
    }

    private static async Task Resize_WrapsLongLines()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Resize(width: 4, height: 5);

        // Act
        renderer.Render(ScreenOutput.From("abcdefgh"));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "\u001b[1;1H");
        AssertContains(rendered, "abcd");
        AssertContains(rendered, "\u001b[2;1H");
        AssertContains(rendered, "efgh");
    }

    private static async Task Resize_WrapsWideRuneAtBoundary()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Resize(width: 3, height: 5);

        // Act
        renderer.Render(ScreenOutput.From("ab好"));
        await renderer.FlushAsync(CancellationToken.None);
        var rendered = ReadUtf8(output);

        // Assert
        AssertContains(rendered, "ab");
        AssertContains(rendered, "\u001b[2;1H");
        AssertContains(rendered, "好");
    }

    private static async Task CellDiff_CombiningGrapheme_PatchesSingleColumn()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Render(ScreenOutput.From("Cafe\u0301"));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From("Cafe\u0300"));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;4H");
        AssertContains(patch, "e\u0300");
    }

    private static async Task CellBuffer_ClearsWideContinuation_WhenReplacingWithNarrowCells()
    {
        // Arrange
        await using var renderer = new AnsiDiffRenderer();
        await using var output = new MemoryStream();
        await renderer.InitializeAsync(output, CancellationToken.None);
        renderer.Resize(width: 3, height: 2);
        renderer.Render(ScreenOutput.From("好x"));
        await renderer.FlushAsync(CancellationToken.None);
        var marker = output.Length;

        // Act
        renderer.Render(ScreenOutput.From("ab"));
        await renderer.FlushAsync(CancellationToken.None);
        var patch = ReadUtf8(output, marker);

        // Assert
        AssertContains(patch, "\u001b[1;1H");
        AssertContains(patch, "ab ");
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

    private static void AssertBefore(string actual, string first, string second)
    {
        var firstIndex = actual.IndexOf(first, StringComparison.Ordinal);
        var secondIndex = actual.IndexOf(second, StringComparison.Ordinal);
        if (firstIndex < 0 || secondIndex < 0 || firstIndex >= secondIndex)
        {
            throw new InvalidOperationException(
                $"Expected '{Escape(first)}' before '{Escape(second)}'.");
        }
    }

    private static void AssertCount(string actual, string fragment, int expected)
    {
        var count = 0;
        var index = 0;
        while (true)
        {
            index = actual.IndexOf(fragment, index, StringComparison.Ordinal);
            if (index < 0)
            {
                break;
            }

            count++;
            index += fragment.Length;
        }

        if (count != expected)
        {
            throw new InvalidOperationException(
                $"Expected '{Escape(fragment)}' count={expected} but got {count}.");
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
