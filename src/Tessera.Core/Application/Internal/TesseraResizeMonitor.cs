using System.Runtime.InteropServices;
using System.Threading.Channels;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;
using Tessera.Core.Terminal;

namespace Tessera.Core.Application.Internal;

internal static class TesseraResizeMonitor
{
    private static readonly BoundedChannelOptions ResizeSignalChannelOptions = new(1)
    {
        SingleReader = true,
        SingleWriter = false,
        FullMode = BoundedChannelFullMode.DropOldest,
        AllowSynchronousContinuations = true
    };

    public static (Task? Loop, IDisposable? SignalRegistration) Start(
        ITerminalAdapter? terminal,
        TesseraRuntimeLoopOptions options,
        TerminalSize initialSize,
        Action<IMessage> send,
        CancellationToken token)
    {
        if (terminal is null || !terminal.IsOutputInteractive)
        {
            return (null, null);
        }

        var signalTicks = Channel.CreateBounded<bool>(ResizeSignalChannelOptions);

        var registration = TryRegisterResizeSignal(terminal, options, () => signalTicks.Writer.TryWrite(true));
        var loop = Task.Run(async () =>
        {
            var last = initialSize;
            var minInterval = options.MinResizePollInterval <= TimeSpan.Zero
                ? TimeSpan.FromMilliseconds(1)
                : options.MinResizePollInterval;
            var interval = options.ResizePollInterval < minInterval
                ? minInterval
                : options.ResizePollInterval;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    await WaitForSignalOrPollIntervalAsync(signalTicks.Reader, interval, token).ConfigureAwait(false);

                    while (signalTicks.Reader.TryRead(out _))
                    {
                        // Drain coalesced resize ticks so only the freshest size is observed.
                    }

                    var current = await terminal.GetSizeAsync(token).ConfigureAwait(false);
                    if (current.Width != last.Width || current.Height != last.Height)
                    {
                        last = current;
                        send(new WindowSizeMsg(current.Width, current.Height));
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    // Resize polling is best-effort; terminal backends can transiently fail.
                }
            }
        }, token);

        return (loop, registration);
    }

    private static async Task WaitForSignalOrPollIntervalAsync(ChannelReader<bool> signalTicks, TimeSpan interval,
        CancellationToken token)
    {
        var waitForSignal = signalTicks.WaitToReadAsync(token);
        if (waitForSignal.IsCompletedSuccessfully)
        {
            return;
        }

        try
        {
            await waitForSignal.AsTask().WaitAsync(interval, token).ConfigureAwait(false);
        }
        catch (TimeoutException)
        {
            // Poll interval elapsed without a signal; caller will re-check terminal size.
        }
    }

    private static IDisposable? TryRegisterResizeSignal(ITerminalAdapter terminal, TesseraRuntimeLoopOptions options,
        Action onResize)
    {
        if (!options.EnableResizeSignals)
        {
            return null;
        }

        if (options.ResizeSignalRegistrationFactory is not null)
        {
            try
            {
                return options.ResizeSignalRegistrationFactory(onResize);
            }
            catch
            {
                // Custom registrations are optional; fall back to polling when they fail.
                return null;
            }
        }

        if (terminal is ConsoleTerminalAdapter consoleTerminal)
        {
            var windowsRegistration = consoleTerminal.TryRegisterResizeSignal(onResize);
            if (windowsRegistration is not null)
            {
                return windowsRegistration;
            }
        }

        if (!(OperatingSystem.IsLinux() || OperatingSystem.IsMacOS()))
        {
            return null;
        }

        try
        {
            return PosixSignalRegistration.Create(PosixSignal.SIGWINCH, _ =>
            {
                try
                {
                    onResize();
                }
                catch
                {
                    // Signal callbacks are best-effort and must not crash the watcher.
                }
            });
        }
        catch
        {
            // SIGWINCH is not guaranteed to be available; polling remains as fallback.
            return null;
        }
    }
}
