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

        var signalTicks = Channel.CreateUnbounded<bool>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
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
                    var pollDelay = Task.Delay(interval, token);
                    var signalWait = signalTicks.Reader.WaitToReadAsync(token).AsTask();
                    await Task.WhenAny(pollDelay, signalWait).ConfigureAwait(false);

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
