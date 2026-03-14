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
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Commands;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Tests;

internal static class RuntimeLoopTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Runtime_InitReturnsQuit_Exits", InitQuitCommand_ExitsProgram);
        yield return new TestCase("Runtime_SendQuit_StopsRunLoop", SendQuit_StopsProgram);
        yield return new TestCase("Runtime_SequenceCommands_ProcessInOrder", Sequence_ProcessesInOrder);
        yield return new TestCase("Runtime_BatchCommands_ProcessAll", Batch_ProcessesAllCommands);
        yield return new TestCase("Runtime_FilterBlocksFirstQuit_AllowsSecond", Filter_CanBlockQuitMessage);
        yield return new TestCase("Runtime_CommandException_WithCatch_EmitsCommandErrorMsg", CommandException_WithCatch_EmitsCommandErrorMsg);
        yield return new TestCase("Runtime_CommandException_WithRecovery_EmitsRecoveredMessage", CommandException_WithRecovery_EmitsRecoveredMessage);
        yield return new TestCase("Runtime_CommandException_RecoveryFailure_EmitsCommandErrorMsg", CommandException_RecoveryFailure_EmitsCommandErrorMsg);
        yield return new TestCase("Runtime_CommandException_WithoutCatch_Propagates", CommandException_WithoutCatch_Propagates);
        yield return new TestCase("Runtime_AdaptiveFramePacing_BatchesBurstRenders", AdaptiveFramePacing_BatchesBurstRenders);
        yield return new TestCase("Runtime_RawOutputMsg_WritesDirectlyToRenderer", RawOutputMsg_WritesDirectlyToRenderer);
        yield return new TestCase("Runtime_MouseOnViewInterceptor_EnqueuesCommand", MouseOnViewInterceptor_EnqueuesCommand);
        yield return new TestCase("Runtime_EmitsTerminalCapabilitiesMessage", EmitsTerminalCapabilitiesMessage);
        yield return new TestCase("Runtime_EmitsColorProfileMessage", EmitsColorProfileMessage);
        yield return new TestCase("Runtime_TerminalCapabilityDetectorDelegate_OverridesDetection", TerminalCapabilityDetectorDelegate_OverridesDetection);
        yield return new TestCase("Runtime_ColorProfileDetectorDelegate_OverridesDetection", ColorProfileDetectorDelegate_OverridesDetection);
        yield return new TestCase("Runtime_CapabilityProbe_CustomModeList_WritesOnlyConfiguredQueries", CapabilityProbe_CustomModeList_WritesOnlyConfiguredQueries);
        yield return new TestCase("Runtime_EventDecoderOverride_IsUsedForInputLoop", EventDecoderOverride_IsUsedForInputLoop);
        yield return new TestCase("Runtime_MaxConcurrentEffects_OneSerializesExecution", MaxConcurrentEffects_OneSerializesExecution);
        yield return new TestCase("Runtime_AnsiRendererOptions_DisableModeQueries", AnsiRendererOptions_DisableModeQueries);
        yield return new TestCase("Runtime_CapabilityProbe_WritesModeQueries", CapabilityProbe_WritesModeQueries);
        yield return new TestCase("Runtime_CapabilityProbe_TimeoutDisablesModeReportsWhenNoResponses", CapabilityProbe_TimeoutDisablesModeReportsWhenNoResponses);
        yield return new TestCase("Runtime_CapabilityProbe_PartialResponseDisablesUnresolvedModes", CapabilityProbe_PartialResponseDisablesUnresolvedModes);
        yield return new TestCase("Runtime_CapabilityProbe_LegacyMouseResponsePreservesMouseCapability", CapabilityProbe_LegacyMouseResponsePreservesMouseCapability);
        yield return new TestCase("Runtime_CapabilityProbe_AllResponsesPreventTimeoutFallback", CapabilityProbe_AllResponsesPreventTimeoutFallback);
        yield return new TestCase("Runtime_ModeReport_RefinesTerminalCapabilities", ModeReport_RefinesTerminalCapabilities);
        yield return new TestCase("Runtime_ModeReport_LegacyMouseSetEnablesCapability", ModeReport_LegacyMouseSetEnablesCapability);
        yield return new TestCase("Runtime_ModeReport_UnsupportedDisablesCapability", ModeReport_UnsupportedDisablesCapability);
        yield return new TestCase("Runtime_ModeReport_PropagatesCapabilitiesToRenderer", ModeReport_PropagatesCapabilitiesToRenderer);
        yield return new TestCase("Runtime_ResizeLoop_EmitsWindowSizeChanges", ResizeLoop_EmitsWindowSizeChanges);
        yield return new TestCase("Runtime_ResizeSignalsDisabled_SkipsSignalRegistration", ResizeSignalsDisabled_SkipsSignalRegistration);
        yield return new TestCase("Runtime_ResizeSignalFactoryFailure_FallsBackToPolling", ResizeSignalFactoryFailure_FallsBackToPolling);
        yield return new TestCase("Runtime_ResizeSignal_EmitsWindowSizeChanges", ResizeSignal_EmitsWindowSizeChanges);
        yield return new TestCase("Runtime_QuitFromInput_CancelsBeforeTerminalDispose", QuitFromInput_CancelsBeforeTerminalDispose);
    }

    private static async Task InitQuitCommand_ExitsProgram()
    {
        // Arrange
        var model = new InitQuitModel();
        var program = NewProgram(model);

        // Act
        var final = await program.RunAsync();

        // Assert
        TestAssert.ReferenceSame(model, final, "Program should return the same model instance.");
    }

    private static async Task SendQuit_StopsProgram()
    {
        // Arrange
        var model = new IdleModel();
        var program = NewProgram(model);

        // Act
        var runTask = program.RunAsync();
        await Task.Delay(20);
        program.Send(new QuitMsg());
        await runTask;

        // Assert
        TestAssert.True(runTask.IsCompletedSuccessfully, "Program should complete after receiving QuitMsg.");
    }

    private static async Task Sequence_ProcessesInOrder()
    {
        // Arrange
        var model = new SequenceModel();
        var program = NewProgram(model);

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.Equal(2, model.Values.Count, "Sequence should emit exactly two values");
        TestAssert.Equal(1, model.Values[0], "First sequence value should be 1");
        TestAssert.Equal(2, model.Values[1], "Second sequence value should be 2");
    }

    private static async Task Batch_ProcessesAllCommands()
    {
        // Arrange
        var model = new BatchModel();
        var program = NewProgram(model);

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.Equal(2, model.Count, "Batch should process both commands");
    }

    private static async Task Filter_CanBlockQuitMessage()
    {
        // Arrange
        var model = new IdleModel();
        var blocked = true;
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            MessageFilter = msg =>
            {
                if (blocked && msg is QuitMsg)
                {
                    blocked = false;
                    return null;
                }

                return msg;
            },
        });

        // Act
        var runTask = program.RunAsync();
        await Task.Delay(20);
        program.Send(new QuitMsg());
        await Task.Delay(20);

        // Assert
        TestAssert.True(!runTask.IsCompleted, "Program should still run after first blocked QuitMsg.");

        // Act
        program.Send(new QuitMsg());
        await runTask;

        // Assert
        TestAssert.True(runTask.IsCompletedSuccessfully, "Program should stop after second QuitMsg.");
    }

    private static async Task CommandException_WithCatch_EmitsCommandErrorMsg()
    {
        // Arrange
        var model = new CommandErrorCaptureModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            CatchEffectExceptions = true,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(model.CapturedError is InvalidOperationException, "EffectErrorMsg should capture command exception.");
        TestAssert.True(
            string.Equals(model.CapturedError?.Message, CommandFaultModel.FailureMessage, StringComparison.Ordinal),
            "Captured command exception message should match source failure.");
    }

    private static async Task CommandException_WithoutCatch_Propagates()
    {
        // Arrange
        var model = new CommandFaultModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            CatchEffectExceptions = false,
        });

        // Act / Assert
        try
        {
            await program.RunAsync();
            throw new InvalidOperationException("Expected command exception to propagate.");
        }
        catch (InvalidOperationException ex)
        {
            TestAssert.True(
                string.Equals(ex.Message, CommandFaultModel.FailureMessage, StringComparison.Ordinal),
                $"Unexpected propagated exception message: {ex.Message}");
        }
    }

    private static async Task CommandException_WithRecovery_EmitsRecoveredMessage()
    {
        // Arrange
        var model = new CommandRecoveryModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            CatchEffectExceptions = true,
            MapEffectException = _ => new NumberMsg(42),
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(model.RecoveredValue == 42, "Recovery hook should transform command exception into a replacement message.");
    }

    private static async Task CommandException_RecoveryFailure_EmitsCommandErrorMsg()
    {
        // Arrange
        var model = new CommandErrorCaptureModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            CatchEffectExceptions = true,
            MapEffectException = _ => throw new InvalidOperationException("recovery-failure"),
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(model.CapturedError is InvalidOperationException, "Recovery hook failures should be surfaced as command errors.");
        TestAssert.True(
            string.Equals(model.CapturedError?.Message, "recovery-failure", StringComparison.Ordinal),
            "Recovery hook exception message should be preserved.");
    }

    private static async Task AdaptiveFramePacing_BatchesBurstRenders()
    {
        // Arrange
        var nonAdaptiveModel = new BurstUpdateModel(targetCount: 8);
        await using var nonAdaptiveRenderer = new RenderCountingRendererSpy();
        var nonAdaptiveProgram = new TestRuntimeDriver(nonAdaptiveModel, new TeaRuntimeLoopOptions
        {
            DisableInput = true,
            Renderer = nonAdaptiveRenderer,
            Terminal = new FakeTerminalAdapter(),
            MaxFps = 120,
            AdaptiveFramePacing = false,
        });

        var adaptiveModel = new BurstUpdateModel(targetCount: 8);
        await using var adaptiveRenderer = new RenderCountingRendererSpy();
        var adaptiveProgram = new TestRuntimeDriver(adaptiveModel, new TeaRuntimeLoopOptions
        {
            DisableInput = true,
            Renderer = adaptiveRenderer,
            Terminal = new FakeTerminalAdapter(),
            MaxFps = 120,
            AdaptiveFramePacing = true,
        });

        // Act
        await nonAdaptiveProgram.RunAsync();
        await adaptiveProgram.RunAsync();

        // Assert
        TestAssert.True(
            adaptiveRenderer.FlushCalls <= nonAdaptiveRenderer.FlushCalls,
            $"Adaptive pacing should not exceed non-adaptive flush count (nonAdaptive={nonAdaptiveRenderer.FlushCalls}, adaptive={adaptiveRenderer.FlushCalls}).");
    }

    private static async Task RawOutputMsg_WritesDirectlyToRenderer()
    {
        // Arrange
        var model = new RawOutputInitModel();
        await using var renderer = new RenderCountingRendererSpy();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableInput = true,
            Renderer = renderer,
            Terminal = new FakeTerminalAdapter(),
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(
            renderer.RawWrites.Count == 1 && renderer.RawWrites[0] == "raw-sequence",
            $"Expected a single raw renderer write, got [{string.Join(", ", renderer.RawWrites)}].");
    }

    private static async Task MouseOnViewInterceptor_EnqueuesCommand()
    {
        // Arrange
        var model = new MouseInterceptModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
        });

        // Act
        var runTask = program.RunAsync();
        await Task.Delay(20);
        program.Send(new MouseClickMsg(MouseButton.Left, 1, 1));
        await runTask;

        // Assert
        TestAssert.Equal(1, model.Intercepted, "OnMouse callback should enqueue and execute command.");
    }

    private static async Task ResizeLoop_EmitsWindowSizeChanges()
    {
        // Arrange
        var terminal = new ResizingFakeTerminal();
        var model = new ResizeTrackingModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = terminal,
            ResizePollInterval = TimeSpan.FromMilliseconds(10),
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(model.Seen.Count >= 2, $"Expected at least 2 size events but got {model.Seen.Count}.");
        TestAssert.True(
            model.Seen[0] == (80, 24) && model.Seen[1] == (100, 40),
            $"Unexpected resize sequence: {string.Join(", ", model.Seen.Select(size => $"{size.W}x{size.H}"))}");
    }

    private static async Task EmitsTerminalCapabilitiesMessage()
    {
        // Arrange
        var expected = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: false,
            ModeReports: true,
            Source: "test-override");
        var model = new CapabilityTrackingModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            TerminalCapabilities = expected,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(model.Seen is not null, "Program should emit TerminalCapabilitiesMsg.");
        TestAssert.Equal(expected, model.Seen!, "Program should emit configured terminal capabilities.");
    }

    private static async Task EmitsColorProfileMessage()
    {
        // Arrange
        var model = new ColorProfileTrackingModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            ColorProfile = TerminalColorProfile.Ansi256,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(
            model.Seen == TerminalColorProfile.Ansi256,
            $"Program should emit configured color profile Ansi256, got {model.Seen}.");
    }

    private static async Task TerminalCapabilityDetectorDelegate_OverridesDetection()
    {
        // Arrange
        var expected = new TerminalCapabilityProfile(
            FocusReporting: false,
            MouseReporting: true,
            BracketedPaste: false,
            SynchronizedUpdates: false,
            ModeReports: false,
            Source: "capability-detector-delegate");
        var model = new CapabilityTrackingModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            TerminalCapabilityDetector = () => expected,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.Equal(expected, model.Seen!, "Terminal capability detector delegate should override default detection.");
    }

    private static async Task ColorProfileDetectorDelegate_OverridesDetection()
    {
        // Arrange
        var model = new ColorProfileTrackingModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            ColorProfileDetector = () => TerminalColorProfile.TrueColor,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(
            model.Seen == TerminalColorProfile.TrueColor,
            $"Color profile detector delegate should override default detection, got {model.Seen}.");
    }

    private static async Task CapabilityProbe_CustomModeList_WritesOnlyConfiguredQueries()
    {
        // Arrange
        var terminal = new InteractiveProbeTerminalAdapter();
        var model = new TimedQuitModel(TimeSpan.FromMilliseconds(90));
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = false,
            Terminal = terminal,
            EnableCapabilityProbe = true,
            CapabilityProbeModes = [2026],
            TerminalCapabilities = new TerminalCapabilityProfile(
                FocusReporting: true,
                MouseReporting: true,
                BracketedPaste: true,
                SynchronizedUpdates: true,
                ModeReports: true,
                Source: "custom-probe-modes"),
            CapabilityProbeTimeout = TimeSpan.FromMilliseconds(700),
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        var output = terminal.OutputText;
        TestAssert.True(output.Contains("\u001b[?2026$p", StringComparison.Ordinal), "Probe should query configured mode 2026.");
        TestAssert.True(!output.Contains("\u001b[?1004$p", StringComparison.Ordinal), "Probe should not query mode 1004 when excluded.");
        TestAssert.True(!output.Contains("\u001b[?1006$p", StringComparison.Ordinal), "Probe should not query mode 1006 when excluded.");
        TestAssert.True(!output.Contains("\u001b[?2004$p", StringComparison.Ordinal), "Probe should not query mode 2004 when excluded.");
    }

    private static async Task EventDecoderOverride_IsUsedForInputLoop()
    {
        // Arrange
        var terminal = new InteractiveInputTerminalAdapter("x");
        var decoder = new QuitOnFirstByteDecoder();
        var program = new TestRuntimeDriver(new IdleModel(), new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = false,
            Terminal = terminal,
            EventDecoder = decoder,
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        TestAssert.True(decoder.Calls > 0, "Injected event decoder should be invoked by input loop.");
    }

    private static async Task MaxConcurrentEffects_OneSerializesExecution()
    {
        // Arrange
        var model = new ConcurrencyTrackingModel(commandCount: 6, delay: TimeSpan.FromMilliseconds(25));
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            MaxConcurrentEffects = 1,
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        TestAssert.Equal(1, model.MaxActiveCommands, "MaxConcurrentEffects=1 should serialize command execution.");
    }

    private static async Task AnsiRendererOptions_DisableModeQueries()
    {
        // Arrange
        var terminal = new InteractiveProbeTerminalAdapter();
        var model = new TimedQuitProbeViewModel(TimeSpan.FromMilliseconds(60));
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableInput = true,
            Terminal = terminal,
            EnableCapabilityProbe = false,
            TerminalCapabilities = new TerminalCapabilityProfile(
                FocusReporting: true,
                MouseReporting: true,
                BracketedPaste: true,
                SynchronizedUpdates: true,
                ModeReports: true,
                Source: "ansi-renderer-options-test"),
            AnsiRendererOptions = new AnsiRendererOptions
            {
                QueryModeReports = false,
            },
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        var output = terminal.OutputText;
        TestAssert.True(output.Contains("\u001b[?2004h", StringComparison.Ordinal), "Runtime should still enable bracketed paste.");
        TestAssert.True(output.Contains("\u001b[?1004h", StringComparison.Ordinal), "Runtime should still enable focus reporting.");
        TestAssert.True(output.Contains("\u001b[?1006h", StringComparison.Ordinal), "Runtime should still enable mouse reporting.");
        TestAssert.True(output.Contains("\u001b[?2026h", StringComparison.Ordinal), "Runtime should still enable synchronized updates.");
        TestAssert.True(!output.Contains("$p", StringComparison.Ordinal), "Renderer mode-report queries should be disabled by options.");
    }

    private static async Task CapabilityProbe_WritesModeQueries()
    {
        // Arrange
        var terminal = new InteractiveProbeTerminalAdapter();
        var model = new TimedQuitModel(TimeSpan.FromMilliseconds(90));
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = false,
            Terminal = terminal,
            TerminalCapabilities = new TerminalCapabilityProfile(
                FocusReporting: true,
                MouseReporting: true,
                BracketedPaste: true,
                SynchronizedUpdates: true,
                ModeReports: true,
                Source: "probe-test"),
            EnableCapabilityProbe = true,
            CapabilityProbeTimeout = TimeSpan.FromMilliseconds(800),
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        var output = terminal.OutputText;
        TestAssert.True(output.Contains("\u001b[?1000$p", StringComparison.Ordinal), "Startup capability probe should query mode 1000.");
        TestAssert.True(output.Contains("\u001b[?1002$p", StringComparison.Ordinal), "Startup capability probe should query mode 1002.");
        TestAssert.True(output.Contains("\u001b[?1003$p", StringComparison.Ordinal), "Startup capability probe should query mode 1003.");
        TestAssert.True(output.Contains("\u001b[?1004$p", StringComparison.Ordinal), "Startup capability probe should query mode 1004.");
        TestAssert.True(output.Contains("\u001b[?1006$p", StringComparison.Ordinal), "Startup capability probe should query mode 1006.");
        TestAssert.True(output.Contains("\u001b[?2004$p", StringComparison.Ordinal), "Startup capability probe should query mode 2004.");
        TestAssert.True(output.Contains("\u001b[?2026$p", StringComparison.Ordinal), "Startup capability probe should query mode 2026.");
    }

    private static async Task CapabilityProbe_TimeoutDisablesModeReportsWhenNoResponses()
    {
        // Arrange
        var terminal = new InteractiveProbeTerminalAdapter();
        var model = new CapabilityProbeTimeoutModel(TimeSpan.FromMilliseconds(260));
        var initial = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            Source: "probe-timeout-test");
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = false,
            Terminal = terminal,
            TerminalCapabilities = initial,
            EnableCapabilityProbe = true,
            CapabilityProbeTimeout = TimeSpan.FromMilliseconds(35),
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        TestAssert.True(model.Seen.Count >= 2, "Probe timeout path should emit a refined capability profile.");
        var refined = model.Seen[^1];
        TestAssert.True(!refined.ModeReports, "No probe responses should disable mode reports for runtime gating.");
        TestAssert.True(
            refined.Source.Contains("+probe-timeout", StringComparison.Ordinal),
            "Timeout-refined capabilities should annotate source with probe-timeout.");
    }

    private static async Task CapabilityProbe_PartialResponseDisablesUnresolvedModes()
    {
        // Arrange
        var terminal = new InteractiveProbeTerminalAdapter();
        var model = new CapabilityProbeResponseModel(
            TimeSpan.FromMilliseconds(140),
            [new ModeReportMsg(2026, ModeReportState.Reset)]);
        var initial = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            Source: "probe-response-test");
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = false,
            Terminal = terminal,
            TerminalCapabilities = initial,
            EnableCapabilityProbe = true,
            CapabilityProbeTimeout = TimeSpan.FromMilliseconds(30),
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        TestAssert.True(model.Seen.Count >= 3, "Partial probe responses should emit both mode-report and probe-timeout refinements.");
        var final = model.Seen[^1];
        TestAssert.True(final.ModeReports, "Any probe response should keep mode reports enabled.");
        TestAssert.True(
            final.Source.Contains("+probe-partial-timeout", StringComparison.Ordinal),
            "Partial probe timeout should annotate source.");
        TestAssert.True(!final.FocusReporting, "Unresolved focus probe should downgrade focus reporting.");
        TestAssert.True(!final.MouseReporting, "Unresolved mouse probe should downgrade mouse reporting.");
        TestAssert.True(!final.BracketedPaste, "Unresolved paste probe should downgrade bracketed paste support.");
        TestAssert.True(final.SynchronizedUpdates, "Mode report reset should retain synchronized update support while reporting current reset state.");
    }

    private static async Task CapabilityProbe_LegacyMouseResponsePreservesMouseCapability()
    {
        // Arrange
        var terminal = new InteractiveProbeTerminalAdapter();
        var model = new CapabilityProbeResponseModel(
            TimeSpan.FromMilliseconds(180),
            [
                new ModeReportMsg(1000, ModeReportState.Set),
                new ModeReportMsg(2026, ModeReportState.Reset),
            ]);
        var initial = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            Source: "probe-legacy-mouse-test");
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = false,
            Terminal = terminal,
            TerminalCapabilities = initial,
            EnableCapabilityProbe = true,
            CapabilityProbeTimeout = TimeSpan.FromMilliseconds(30),
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        TestAssert.True(model.Seen.Count >= 3, "Legacy mouse + sync probe responses should emit refined capability updates.");
        var final = model.Seen[^1];
        TestAssert.True(
            final.Source.Contains("+probe-partial-timeout", StringComparison.Ordinal),
            "Partial timeout should still annotate unresolved representative modes.");
        TestAssert.True(final.MouseReporting, "Legacy mouse mode support should preserve mouse capability when 1006 stays unresolved.");
        TestAssert.True(!final.FocusReporting, "Unresolved focus probe should downgrade focus reporting.");
        TestAssert.True(!final.BracketedPaste, "Unresolved paste probe should downgrade bracketed paste support.");
    }

    private static async Task CapabilityProbe_AllResponsesPreventTimeoutFallback()
    {
        // Arrange
        var terminal = new InteractiveProbeTerminalAdapter();
        var model = new CapabilityProbeResponseModel(
            TimeSpan.FromMilliseconds(220),
            [
                new ModeReportMsg(1004, ModeReportState.Set),
                new ModeReportMsg(1006, ModeReportState.Set),
                new ModeReportMsg(2004, ModeReportState.Set),
                new ModeReportMsg(2026, ModeReportState.Reset),
            ]);
        var initial = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            Source: "probe-all-responses-test");
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = false,
            Terminal = terminal,
            TerminalCapabilities = initial,
            EnableCapabilityProbe = true,
            CapabilityProbeTimeout = TimeSpan.FromMilliseconds(120),
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        TestAssert.True(model.Seen.Count >= 2, "All probe responses should still refine capability profile.");
        var final = model.Seen[^1];
        TestAssert.True(final.ModeReports, "All probe responses should keep mode reports enabled.");
        TestAssert.True(final.FocusReporting, "Probe set response should keep focus reporting enabled.");
        TestAssert.True(final.MouseReporting, "Probe set response should keep mouse reporting enabled.");
        TestAssert.True(final.BracketedPaste, "Probe set response should keep bracketed paste enabled.");
        TestAssert.True(final.SynchronizedUpdates, "Mode report reset should retain synchronized update support.");
        TestAssert.True(
            !final.Source.Contains("+probe-timeout", StringComparison.Ordinal)
            && !final.Source.Contains("+probe-partial-timeout", StringComparison.Ordinal),
            "Full probe responses should avoid timeout fallback annotations.");
    }

    private static async Task ModeReport_RefinesTerminalCapabilities()
    {
        // Arrange
        var initial = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            Source: "test-initial");
        var model = new CapabilityRefinementModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            TerminalCapabilities = initial,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.Equal(2, model.Seen.Count, "Program should emit initial and refined capability messages.");
        TestAssert.True(model.Seen[0].SynchronizedUpdates, "Initial capabilities should keep synchronized updates enabled.");
        TestAssert.True(model.Seen[1].SynchronizedUpdates, "Mode report reset should retain synchronized update support.");
        TestAssert.True(
            model.Seen[1].Source.Contains("+mode-report", StringComparison.Ordinal)
            && model.Seen[1].Source.Contains("+mode-report-reset", StringComparison.Ordinal),
            "Refined capabilities should annotate source with mode-report reset state.");
    }

    private static async Task ModeReport_LegacyMouseSetEnablesCapability()
    {
        // Arrange
        var initial = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: false,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            Source: "legacy-mode-report-test");
        var model = new CapabilityProbeResponseModel(
            TimeSpan.FromMilliseconds(120),
            [new ModeReportMsg(1000, ModeReportState.Set)]);
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            TerminalCapabilities = initial,
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(2));

        // Assert
        TestAssert.True(model.Seen.Count >= 2, "Legacy mouse mode report should emit refined capabilities.");
        var refined = model.Seen[^1];
        TestAssert.True(refined.MouseReporting, "Legacy mouse mode report set-state should enable mouse capability.");
        TestAssert.True(
            refined.Source.Contains("+mode-report", StringComparison.Ordinal)
            && refined.Source.Contains("+mode-report-mouse-legacy", StringComparison.Ordinal),
            "Legacy mouse refinement should annotate source.");
    }

    private static async Task ModeReport_UnsupportedDisablesCapability()
    {
        // Arrange
        var initial = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            Source: "test-unsupported");
        var model = new UnsupportedModeReportRefinementModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            TerminalCapabilities = initial,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.Equal(2, model.Seen.Count, "Program should emit initial and refined capability messages.");
        var refined = model.Seen[1];
        TestAssert.True(!refined.MouseReporting, "Unsupported mode-report state should downgrade mouse reporting.");
        TestAssert.True(
            refined.Source.Contains("+mode-report", StringComparison.Ordinal)
            && refined.Source.Contains("+mode-report-unsupported", StringComparison.Ordinal),
            "Unsupported mode-report refinement should annotate source.");
    }

    private static async Task ModeReport_PropagatesCapabilitiesToRenderer()
    {
        // Arrange
        var initial = new TerminalCapabilityProfile(
            FocusReporting: true,
            MouseReporting: true,
            BracketedPaste: true,
            SynchronizedUpdates: true,
            ModeReports: true,
            Source: "test-renderer-propagation");
        var model = new UnsupportedModeReportRefinementModel();
        await using var renderer = new CapabilityAwareRendererSpy();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableInput = true,
            Renderer = renderer,
            Terminal = new FakeTerminalAdapter(),
            TerminalCapabilities = initial,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(renderer.Updates.Count >= 2, "Renderer should receive initial and refined capability updates.");
        TestAssert.True(!renderer.Updates[^1].MouseReporting, "Renderer should receive refined unsupported mouse capability state.");
    }

    private static async Task ResizeSignal_EmitsWindowSizeChanges()
    {
        // Arrange
        var terminal = new SignalDrivenFakeTerminal(new TeaSharp.Core.Terminal.TerminalSize(80, 24));
        var model = new ResizeTrackingModel();
        Action? raiseSignal = null;
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = terminal,
            ResizePollInterval = TimeSpan.FromSeconds(2),
            ResizeSignalRegistrationFactory = onResize =>
            {
                raiseSignal = onResize;
                return new DelegateDisposable(() => { });
            },
        });

        // Act
        var runTask = program.RunAsync();
        await WaitUntilAsync(() => raiseSignal is not null, TimeSpan.FromSeconds(1), "Resize signal registration was not initialized.");
        raiseSignal?.Invoke();
        raiseSignal?.Invoke();
        await Task.Delay(30);
        TestAssert.Equal(1, model.Seen.Count, "Resize signals without size change should not emit duplicate WindowSizeMsg events.");
        terminal.SetSize(101, 41);
        raiseSignal?.Invoke();
        await runTask;

        // Assert
        TestAssert.True(model.Seen.Count >= 2, $"Expected at least 2 size events but got {model.Seen.Count}.");
        TestAssert.True(
            model.Seen[0] == (80, 24) && model.Seen[1] == (101, 41),
            $"Unexpected resize sequence: {string.Join(", ", model.Seen.Select(size => $"{size.W}x{size.H}"))}");
    }

    private static async Task ResizeSignalsDisabled_SkipsSignalRegistration()
    {
        // Arrange
        var terminal = new ResizingFakeTerminal();
        var model = new ResizeTrackingModel();
        var registrationCalls = 0;
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = terminal,
            EnableResizeSignals = false,
            ResizePollInterval = TimeSpan.FromMilliseconds(10),
            ResizeSignalRegistrationFactory = _ =>
            {
                registrationCalls++;
                return new DelegateDisposable(() => { });
            },
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.Equal(0, registrationCalls, "Signal registration should be skipped when resize signals are disabled.");
        TestAssert.True(model.Seen.Count >= 2, "Polling fallback should still emit resize updates.");
    }

    private static async Task ResizeSignalFactoryFailure_FallsBackToPolling()
    {
        // Arrange
        var terminal = new ResizingFakeTerminal();
        var model = new ResizeTrackingModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = terminal,
            EnableResizeSignals = true,
            ResizePollInterval = TimeSpan.FromMilliseconds(10),
            ResizeSignalRegistrationFactory = _ => throw new InvalidOperationException("boom"),
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(model.Seen.Count >= 2, "Resize polling should continue if signal registration throws.");
    }

    private static async Task QuitFromInput_CancelsBeforeTerminalDispose()
    {
        // Arrange
        var terminal = new DisposeOrderingTerminalAdapter();
        var model = new QuitOnQModel();
        var program = new TestRuntimeDriver(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            Terminal = terminal,
            EscapeTimeout = TimeSpan.FromMilliseconds(10),
        });

        // Act
        await program.RunAsync().WaitAsync(TimeSpan.FromSeconds(1));

        // Assert
        TestAssert.True(terminal.DisposeObservedCancellation, "Program should cancel input processing before terminal dispose.");
    }

    private static TestRuntimeDriver NewProgram(IScreen model) =>
        new(model, new TeaRuntimeLoopOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
        });

    private static async Task WaitUntilAsync(Func<bool> condition, TimeSpan timeout, string failureMessage)
    {
        var deadline = DateTimeOffset.UtcNow + timeout;
        while (!condition())
        {
            if (DateTimeOffset.UtcNow >= deadline)
            {
                throw new InvalidOperationException(failureMessage);
            }

            await Task.Delay(10);
        }
    }

    private sealed class TestRuntimeDriver
    {
        private readonly IScreen _screen;
        private readonly TeaRuntimeLoop _runtime;

        public TestRuntimeDriver(IScreen screen, TeaRuntimeLoopOptions? options = null)
        {
            _screen = screen ?? throw new ArgumentNullException(nameof(screen));
            _runtime = new TeaRuntimeLoop(screen.Init, screen.Update, screen.Render, options);
        }

        public void Send(IMessage message)
        {
            _runtime.Send(message);
        }

        public async Task<IScreen> RunAsync(CancellationToken cancellationToken = default)
        {
            await _runtime.RunAsync(cancellationToken).ConfigureAwait(false);
            return _screen;
        }
    }
}
