using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Commands;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Tests;

internal static class ProgramRuntimeTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Program_InitReturnsQuit_Exits", InitQuitCommand_ExitsProgram);
        yield return new TestCase("Program_SendQuit_StopsRunLoop", SendQuit_StopsProgram);
        yield return new TestCase("Program_SequenceCommands_ProcessInOrder", Sequence_ProcessesInOrder);
        yield return new TestCase("Program_BatchCommands_ProcessAll", Batch_ProcessesAllCommands);
        yield return new TestCase("Program_FilterBlocksFirstQuit_AllowsSecond", Filter_CanBlockQuitMessage);
        yield return new TestCase("Program_CommandException_WithCatch_EmitsCommandErrorMsg", CommandException_WithCatch_EmitsCommandErrorMsg);
        yield return new TestCase("Program_CommandException_WithoutCatch_Propagates", CommandException_WithoutCatch_Propagates);
        yield return new TestCase("Program_EmitsTerminalCapabilitiesMessage", EmitsTerminalCapabilitiesMessage);
        yield return new TestCase("Program_CapabilityProbe_WritesModeQueries", CapabilityProbe_WritesModeQueries);
        yield return new TestCase("Program_CapabilityProbe_TimeoutDisablesModeReportsWhenNoResponses", CapabilityProbe_TimeoutDisablesModeReportsWhenNoResponses);
        yield return new TestCase("Program_CapabilityProbe_PartialResponseDisablesUnresolvedModes", CapabilityProbe_PartialResponseDisablesUnresolvedModes);
        yield return new TestCase("Program_CapabilityProbe_AllResponsesPreventTimeoutFallback", CapabilityProbe_AllResponsesPreventTimeoutFallback);
        yield return new TestCase("Program_ModeReport_RefinesTerminalCapabilities", ModeReport_RefinesTerminalCapabilities);
        yield return new TestCase("Program_ModeReport_UnsupportedDisablesCapability", ModeReport_UnsupportedDisablesCapability);
        yield return new TestCase("Program_ModeReport_PropagatesCapabilitiesToRenderer", ModeReport_PropagatesCapabilitiesToRenderer);
        yield return new TestCase("Program_ResizeLoop_EmitsWindowSizeChanges", ResizeLoop_EmitsWindowSizeChanges);
        yield return new TestCase("Program_ResizeSignalsDisabled_SkipsSignalRegistration", ResizeSignalsDisabled_SkipsSignalRegistration);
        yield return new TestCase("Program_ResizeSignalFactoryFailure_FallsBackToPolling", ResizeSignalFactoryFailure_FallsBackToPolling);
        yield return new TestCase("Program_ResizeSignal_EmitsWindowSizeChanges", ResizeSignal_EmitsWindowSizeChanges);
        yield return new TestCase("Program_QuitFromInput_CancelsBeforeTerminalDispose", QuitFromInput_CancelsBeforeTerminalDispose);
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
        var program = new TeaProgram(model, new ProgramOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            Filter = (_, msg) =>
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
        var program = new TeaProgram(model, new ProgramOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            CatchCommandExceptions = true,
        });

        // Act
        await program.RunAsync();

        // Assert
        TestAssert.True(model.CapturedError is InvalidOperationException, "CommandErrorMsg should capture command exception.");
        TestAssert.True(
            string.Equals(model.CapturedError?.Message, CommandFaultModel.FailureMessage, StringComparison.Ordinal),
            "Captured command exception message should match source failure.");
    }

    private static async Task CommandException_WithoutCatch_Propagates()
    {
        // Arrange
        var model = new CommandFaultModel();
        var program = new TeaProgram(model, new ProgramOptions
        {
            DisableRenderer = true,
            DisableInput = true,
            Terminal = new FakeTerminalAdapter(),
            CatchCommandExceptions = false,
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

    private static async Task ResizeLoop_EmitsWindowSizeChanges()
    {
        // Arrange
        var terminal = new ResizingFakeTerminal();
        var model = new ResizeTrackingModel();
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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

    private static async Task CapabilityProbe_WritesModeQueries()
    {
        // Arrange
        var terminal = new InteractiveProbeTerminalAdapter();
        var model = new TimedQuitModel(TimeSpan.FromMilliseconds(90));
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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
        var program = new TeaProgram(model, new ProgramOptions
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

    private static TeaProgram NewProgram(IModel model) =>
        new(model, new ProgramOptions
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
}
