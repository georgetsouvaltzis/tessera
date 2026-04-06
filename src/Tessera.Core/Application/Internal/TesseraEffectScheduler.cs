using System.Threading.Channels;
using System.Runtime.ExceptionServices;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;

namespace Tessera.Core.Application;

internal sealed class TesseraEffectScheduler : IDisposable
{
    private readonly TesseraRuntimeLoopOptions _options;
    private readonly Action<IMessage> _send;
    private readonly object _effectTaskLock = new();
    private readonly HashSet<Task> _effectTasks = [];
    private readonly SemaphoreSlim? _effectConcurrencyGate;
    private ExceptionDispatchInfo? _unhandledEffectException;

    public TesseraEffectScheduler(TesseraRuntimeLoopOptions options, Action<IMessage> send)
    {
        _options = options;
        _send = send;
        var maxConcurrentEffects = Math.Max(0, options.MaxConcurrentEffects);
        _effectConcurrencyGate = maxConcurrentEffects == 0
            ? null
            : new SemaphoreSlim(maxConcurrentEffects, maxConcurrentEffects);
    }

    public async Task RunLoopAsync(ChannelReader<Effect> effects, CancellationToken token)
    {
        while (await effects.WaitToReadAsync(token).ConfigureAwait(false))
        {
            while (effects.TryRead(out var effect))
            {
                if (_effectConcurrencyGate is not null)
                {
                    try
                    {
                        await _effectConcurrencyGate.WaitAsync(token).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                StartTrackedEffect(effect, token);
            }
        }

        await WaitForTrackedEffectsAsync().ConfigureAwait(false);
    }

    public ExceptionDispatchInfo? ConsumeUnhandledException()
    {
        return Interlocked.Exchange(ref _unhandledEffectException, null);
    }

    public Task RunSequenceAsync(IReadOnlyList<Effect> effects, CancellationToken token)
    {
        return ExecuteSequenceAsync(effects, token);
    }

    public async Task WaitForTrackedEffectsAsync()
    {
        while (true)
        {
            Task[] snapshot;
            lock (_effectTaskLock)
            {
                if (_effectTasks.Count == 0)
                {
                    return;
                }

                snapshot = [.. _effectTasks];
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
        _effectConcurrencyGate?.Dispose();
        lock (_effectTaskLock)
        {
            _effectTasks.Clear();
        }
    }

    private async Task ExecuteEffectAsync(Effect effect, CancellationToken token)
    {
        try
        {
            var message = await effect(token).ConfigureAwait(false);
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
            if (_options.CatchEffectExceptions)
            {
                if (_options.MapEffectException is { } recover)
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
                        _send(new EffectErrorMsg(recoveryException));
                        return;
                    }
                }

                _send(new EffectErrorMsg(ex));
                return;
            }

            _ = Interlocked.CompareExchange(
                ref _unhandledEffectException,
                ExceptionDispatchInfo.Capture(ex),
                comparand: null);
            _send(new InterruptMsg());
        }
    }

    private async Task ExecuteSequenceAsync(IReadOnlyList<Effect> effects, CancellationToken token)
    {
        foreach (var effect in effects)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            await ExecuteEffectAsync(effect, token).ConfigureAwait(false);
        }
    }

    private void StartTrackedEffect(Effect effect, CancellationToken token)
    {
        var task = Task.Run(async () =>
        {
            try
            {
                await ExecuteEffectAsync(effect, token).ConfigureAwait(false);
            }
            finally
            {
                _effectConcurrencyGate?.Release();
            }
        }, CancellationToken.None);

        lock (_effectTaskLock)
        {
            _effectTasks.Add(task);
        }

        _ = task.ContinueWith(
            static (completed, state) =>
            {
                if (state is not TesseraEffectScheduler scheduler)
                {
                    return;
                }

                lock (scheduler._effectTaskLock)
                {
                    scheduler._effectTasks.Remove(completed);
                }
            },
            this,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
    }
}
