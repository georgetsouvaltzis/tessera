using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Commands;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using TeaSharp.Tests;
using ModelView = TeaSharp.Core.Abstractions.View;

var failures = new List<string>();

await RunTest("InitQuitCommand_ExitsProgram", InitQuitCommand_ExitsProgram, failures);
await RunTest("SendQuit_StopsProgram", SendQuit_StopsProgram, failures);
await RunTest("Sequence_ProcessesInOrder", Sequence_ProcessesInOrder, failures);
await RunTest("Batch_ProcessesAllCommands", Batch_ProcessesAllCommands, failures);
await RunTest("Filter_CanBlockQuitMessage", Filter_CanBlockQuitMessage, failures);
await RunTest("ResizeLoop_EmitsWindowSizeChanges", ResizeLoop_EmitsWindowSizeChanges, failures);
await RunTest("EventDecoder_GoldenSequences", EventDecoder_GoldenSequences, failures);
await RunTest("TerminalReader_AggregatesBracketedPaste", TerminalReader_AggregatesBracketedPaste, failures);

if (failures.Count > 0)
{
    Console.Error.WriteLine("TeaSharp tests failed:");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"- {failure}");
    }

    return 1;
}

Console.WriteLine("TeaSharp tests passed.");
return 0;

static async Task RunTest(string name, Func<Task> test, List<string> failures)
{
    try
    {
        await test();
        Console.WriteLine($"[PASS] {name}");
    }
    catch (Exception ex)
    {
        failures.Add($"{name}: {ex.Message}");
        Console.WriteLine($"[FAIL] {name}");
    }
}

static async Task InitQuitCommand_ExitsProgram()
{
    var model = new InitQuitModel();
    var program = NewProgram(model);

    var final = await program.RunAsync();
    AssertReferenceSame(model, final);
}

static async Task SendQuit_StopsProgram()
{
    var model = new IdleModel();
    var program = NewProgram(model);

    var runTask = program.RunAsync();
    await Task.Delay(20);
    program.Send(new QuitMsg());

    await runTask;
}

static async Task Sequence_ProcessesInOrder()
{
    var model = new SequenceModel();
    var program = NewProgram(model);

    await program.RunAsync();

    AssertEqual(2, model.Values.Count, "Sequence values count");
    AssertEqual(1, model.Values[0], "Sequence first value");
    AssertEqual(2, model.Values[1], "Sequence second value");
}

static async Task Batch_ProcessesAllCommands()
{
    var model = new BatchModel();
    var program = NewProgram(model);

    await program.RunAsync();

    AssertEqual(2, model.Count, "Batch processed count");
}

static async Task Filter_CanBlockQuitMessage()
{
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

    var runTask = program.RunAsync();
    await Task.Delay(20);
    program.Send(new QuitMsg());
    await Task.Delay(20);

    if (runTask.IsCompleted)
    {
        throw new InvalidOperationException("Program should still run after first blocked quit.");
    }

    program.Send(new QuitMsg());
    await runTask;
}

static Task EventDecoder_GoldenSequences() => EventDecoderGoldenTests.RunAsync();

static Task TerminalReader_AggregatesBracketedPaste() => TerminalReaderBehaviorTests.RunAsync();

static async Task ResizeLoop_EmitsWindowSizeChanges()
{
    var terminal = new ResizingFakeTerminal();
    var model = new ResizeTrackingModel();
    var program = new TeaProgram(model, new ProgramOptions
    {
        DisableRenderer = true,
        DisableInput = true,
        Terminal = terminal,
        ResizePollInterval = TimeSpan.FromMilliseconds(10),
    });

    await program.RunAsync();

    if (model.Seen.Count < 2)
    {
        throw new InvalidOperationException($"Expected at least 2 size events but got {model.Seen.Count}.");
    }

    if (model.Seen[0] != (80, 24) || model.Seen[1] != (100, 40))
    {
        throw new InvalidOperationException($"Unexpected resize sequence: {string.Join(", ", model.Seen.Select(s => $"{s.W}x{s.H}"))}");
    }
}

static TeaProgram NewProgram(IModel model) =>
    new(model, new ProgramOptions
    {
        DisableRenderer = true,
        DisableInput = true,
        Terminal = new FakeTerminalAdapter(),
    });

static void AssertReferenceSame(object expected, object actual)
{
    if (!ReferenceEquals(expected, actual))
    {
        throw new InvalidOperationException("Reference equality assertion failed.");
    }
}

static void AssertEqual<T>(T expected, T actual, string title)
    where T : IEquatable<T>
{
    if (!expected.Equals(actual))
    {
        throw new InvalidOperationException($"{title} assertion failed. Expected={expected}, Actual={actual}");
    }
}

sealed class InitQuitModel : IModel
{
    public Command? Init() => Commands.Quit;

    public UpdateResult Update(IMessage message) => new(this, null);

    public ModelView View() => ModelView.From("quit");
}

sealed class IdleModel : IModel
{
    public Command? Init() => null;

    public UpdateResult Update(IMessage message) => new(this, null);

    public ModelView View() => ModelView.From("idle");
}

sealed class SequenceModel : IModel
{
    public List<int> Values { get; } = [];

    public Command? Init() => Commands.Sequence(
        Commands.FromMessage(new NumberMsg(1)),
        Commands.FromMessage(new NumberMsg(2)),
        Commands.Quit);

    public UpdateResult Update(IMessage message)
    {
        if (message is NumberMsg number)
        {
            Values.Add(number.Value);
        }

        return new(this, null);
    }

    public ModelView View() => ModelView.From("sequence");
}

sealed class BatchModel : IModel
{
    public int Count { get; private set; }

    public Command? Init() => Commands.Batch(
        Commands.FromMessage(new NumberMsg(1)),
        Commands.FromMessage(new NumberMsg(2)));

    public UpdateResult Update(IMessage message)
    {
        if (message is NumberMsg)
        {
            Count++;
            if (Count == 2)
            {
                return new(this, Commands.Quit);
            }
        }

        return new(this, null);
    }

    public ModelView View() => ModelView.From("batch");
}

sealed record NumberMsg(int Value) : IMessage;

sealed class FakeTerminalAdapter : ITerminalAdapter
{
    public Stream Input { get; } = new MemoryStream();

    public Stream Output { get; } = new MemoryStream();

    public bool IsInputInteractive => false;

    public bool IsOutputInteractive => false;

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.FromResult(new TerminalSize(80, 24));
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

sealed class ResizingFakeTerminal : ITerminalAdapter
{
    private int _callCount;

    public Stream Input { get; } = new MemoryStream();

    public Stream Output { get; } = new MemoryStream();

    public bool IsInputInteractive => false;

    public bool IsOutputInteractive => true;

    public ValueTask PrepareAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask RestoreAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken)
    {
        _ = cancellationToken;
        var call = Interlocked.Increment(ref _callCount);
        return call <= 1
            ? ValueTask.FromResult(new TerminalSize(80, 24))
            : ValueTask.FromResult(new TerminalSize(100, 40));
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

sealed class ResizeTrackingModel : IModel
{
    public List<(int W, int H)> Seen { get; } = [];

    public Command? Init() => null;

    public UpdateResult Update(IMessage message)
    {
        if (message is WindowSizeMsg ws)
        {
            Seen.Add((ws.Width, ws.Height));
            if (Seen.Count >= 2)
            {
                return new UpdateResult(this, Commands.Quit);
            }
        }

        return new UpdateResult(this, null);
    }

    public ModelView View() => ModelView.From("resize");
}
