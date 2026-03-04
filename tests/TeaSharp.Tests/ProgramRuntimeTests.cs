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
        yield return new TestCase("Program_EmitsTerminalCapabilitiesMessage", EmitsTerminalCapabilitiesMessage);
        yield return new TestCase("Program_ResizeLoop_EmitsWindowSizeChanges", ResizeLoop_EmitsWindowSizeChanges);
        yield return new TestCase("Program_ResizeSignal_EmitsWindowSizeChanges", ResizeSignal_EmitsWindowSizeChanges);
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
        terminal.SetSize(101, 41);
        raiseSignal?.Invoke();
        await runTask;

        // Assert
        TestAssert.True(model.Seen.Count >= 2, $"Expected at least 2 size events but got {model.Seen.Count}.");
        TestAssert.True(
            model.Seen[0] == (80, 24) && model.Seen[1] == (101, 41),
            $"Unexpected resize sequence: {string.Join(", ", model.Seen.Select(size => $"{size.W}x{size.H}"))}");
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
