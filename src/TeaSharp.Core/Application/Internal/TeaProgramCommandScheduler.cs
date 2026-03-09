using System.Threading.Channels;
using System.Runtime.ExceptionServices;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Application;

internal sealed class TeaProgramCommandScheduler : IDisposable
{
    private readonly ProgramOptions _options;
    private readonly Action<IMessage> _send;
    private readonly object _commandTaskLock = new();
    private readonly HashSet<Task> _commandTasks = [];
    private readonly SemaphoreSlim? _commandConcurrencyGate;
    private ExceptionDispatchInfo? _unhandledCommandException;

    public TeaProgramCommandScheduler(ProgramOptions options, Action<IMessage> send)
    {
        _options = options;
        _send = send;
        var maxConcurrentCommands = Math.Max(0, options.MaxConcurrentCommands);
        _commandConcurrencyGate = maxConcurrentCommands == 0
            ? null
            : new SemaphoreSlim(maxConcurrentCommands, maxConcurrentCommands);
    }

    public async Task RunLoopAsync(ChannelReader<Command> commands, CancellationToken token)
    {
        while (await commands.WaitToReadAsync(token).ConfigureAwait(false))
        {
            while (commands.TryRead(out var command))
            {
                if (_commandConcurrencyGate is not null)
                {
                    try
                    {
                        await _commandConcurrencyGate.WaitAsync(token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                StartTrackedCommand(command, token);
            }
        }

        await WaitForTrackedCommandsAsync().ConfigureAwait(false);
    }

    public ExceptionDispatchInfo? ConsumeUnhandledException()
    {
        return Interlocked.Exchange(ref _unhandledCommandException, null);
    }

    public Task RunSequenceAsync(IReadOnlyList<Command> commands, CancellationToken token)
    {
        return ExecuteSequenceAsync(commands, token);
    }

    public async Task WaitForTrackedCommandsAsync()
    {
        while (true)
        {
            Task[] snapshot;
            lock (_commandTaskLock)
            {
                if (_commandTasks.Count == 0)
                {
                    return;
                }

                snapshot = [.. _commandTasks];
            }

            try
            {
                await Task.WhenAll(snapshot).ConfigureAwait(false);
            }
            catch
            {
            }
        }
    }

    public void Dispose()
    {
        _commandConcurrencyGate?.Dispose();
        lock (_commandTaskLock)
        {
            _commandTasks.Clear();
        }
    }

    private async Task ExecuteCommandAsync(Command command, CancellationToken token)
    {
        try
        {
            var message = await command(token).ConfigureAwait(false);
            if (message is not null)
            {
                _send(message);
            }
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            if (_options.CatchCommandExceptions)
            {
                if (_options.RecoverCommandException is { } recover)
                {
                    try
                    {
                        var recoveryMessage = recover(ex);
                        if (recoveryMessage is not null)
                        {
                            _send(recoveryMessage);
                            return;
                        }
                    }
                    catch (Exception recoveryException)
                    {
                        _send(new CommandErrorMsg(recoveryException));
                        return;
                    }
                }

                _send(new CommandErrorMsg(ex));
                return;
            }

            _ = Interlocked.CompareExchange(
                ref _unhandledCommandException,
                ExceptionDispatchInfo.Capture(ex),
                comparand: null);
            _send(new InterruptMsg());
        }
    }

    private async Task ExecuteSequenceAsync(IReadOnlyList<Command> commands, CancellationToken token)
    {
        foreach (var command in commands)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            await ExecuteCommandAsync(command, token).ConfigureAwait(false);
        }
    }

    private void StartTrackedCommand(Command command, CancellationToken token)
    {
        var task = Task.Run(async () =>
        {
            try
            {
                await ExecuteCommandAsync(command, token).ConfigureAwait(false);
            }
            finally
            {
                _commandConcurrencyGate?.Release();
            }
        }, CancellationToken.None);

        lock (_commandTaskLock)
        {
            _commandTasks.Add(task);
        }

        _ = task.ContinueWith(
            static (completed, state) =>
            {
                if (state is not TeaProgramCommandScheduler scheduler)
                {
                    return;
                }

                lock (scheduler._commandTaskLock)
                {
                    scheduler._commandTasks.Remove(completed);
                }
            },
            this,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }
}
