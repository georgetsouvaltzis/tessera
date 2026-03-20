using TeaSharp.Core.Abstractions;
using System.Runtime.InteropServices;
using System.Threading.Channels;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Application;

internal static class TeaResizeMonitor
{
    public static (Task? Loop, IDisposable? SignalRegistration) Start(
        ITerminalAdapter? terminal,
        TeaRuntimeLoopOptions options,
        TerminalSize initialSize,
        Action<IMessage> send,
        CancellationToken token)
    {
        if (terminal is null || !terminal.IsOutputInteractive)
        {
            return (null, null);
        }

        var signalTicks = Channel.CreateBounded<bool>(new BoundedChannelOptions(1)
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest,
        });

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
                }
            }
        }, token);

        return (loop, registration);
    }

    private static async Task WaitForSignalOrPollIntervalAsync(ChannelReader<bool> signalTicks, TimeSpan interval, CancellationToken token)
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
        }
    }

    private static IDisposable? TryRegisterResizeSignal(ITerminalAdapter terminal, TeaRuntimeLoopOptions options, Action onResize)
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
                }
            });
        }
        catch
        {
            return null;
        }
    }
}
