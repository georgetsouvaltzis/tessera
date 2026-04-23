using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace Tessera.Core.Terminal.Adapters.Internal;

internal static partial class WindowsConsoleSession
{
    private const int StdInputHandle = -10;
    private const int StdOutputHandle = -11;
    private const uint EnableProcessedInput = 0x0001;
    private const uint EnableLineInput = 0x0002;
    private const uint EnableEchoInput = 0x0004;
    private const uint EnableVirtualTerminalProcessing = 0x0004;
    private const uint DisableNewlineAutoReturn = 0x0008;
    private const uint EnableWindowInput = 0x0008;
    private const uint EnableVirtualTerminalInput = 0x0200;
    private const ushort WindowBufferSizeEventType = 0x0004;
    private const uint WaitObject0 = 0x00000000;
    private const uint WaitTimeout = 0x00000102;
    private const uint WaitFailed = 0xFFFFFFFF;

    public static bool TryEnableVirtualTerminalModes(ref uint originalInputMode, ref uint originalOutputMode)
    {
        var inputHandle = GetStdHandle(StdInputHandle);
        var outputHandle = GetStdHandle(StdOutputHandle);
        var vtInputEnabled = false;

        if (IsInvalidHandle(inputHandle) || IsInvalidHandle(outputHandle))
        {
            return false;
        }

        if (GetConsoleMode(inputHandle, out var imode))
        {
            originalInputMode = imode;
            var nextInputMode = imode | EnableVirtualTerminalInput;
            nextInputMode &= ~(EnableLineInput | EnableEchoInput | EnableProcessedInput);
            vtInputEnabled = SetConsoleMode(inputHandle, nextInputMode);
        }

        if (GetConsoleMode(outputHandle, out var omode))
        {
            originalOutputMode = omode;
            _ = SetConsoleMode(outputHandle, omode | EnableVirtualTerminalProcessing | DisableNewlineAutoReturn);
        }

        return vtInputEnabled;
    }

    public static void TryRestoreModes(uint originalInputMode, uint originalOutputMode)
    {
        var inputHandle = GetStdHandle(StdInputHandle);
        var outputHandle = GetStdHandle(StdOutputHandle);

        if (!IsInvalidHandle(inputHandle) && originalInputMode != 0)
        {
            _ = SetConsoleMode(inputHandle, originalInputMode);
        }

        if (!IsInvalidHandle(outputHandle) && originalOutputMode != 0)
        {
            _ = SetConsoleMode(outputHandle, originalOutputMode);
        }
    }

    public static IDisposable? TryRegisterResizeSignal(bool isInputInteractive, Action onResize)
    {
        if (!OperatingSystem.IsWindows() || !isInputInteractive)
        {
            return null;
        }

        var inputHandle = GetStdHandle(StdInputHandle);
        if (IsInvalidHandle(inputHandle))
        {
            return null;
        }

        if (GetConsoleMode(inputHandle, out var mode))
        {
            _ = SetConsoleMode(inputHandle, mode | EnableWindowInput);
        }

        var cts = new CancellationTokenSource();
        var watcher = Task.Run(() => WatchResizeSignals(inputHandle, onResize, cts.Token), CancellationToken.None);
        return new DelegateDisposable(() =>
        {
            cts.Cancel();
            try
            {
                _ = watcher.Wait(180);
            }
            catch
            {
                // Watcher completion is best-effort during disposal.
            }

            cts.Dispose();
        });
    }

    private static void WatchResizeSignals(IntPtr inputHandle, Action onResize, CancellationToken cancellationToken)
    {
        var records = new InputRecord[16];
        Coord? lastReportedSize = null;

        while (!cancellationToken.IsCancellationRequested)
        {
            var wait = WaitForSingleObject(inputHandle, 120);
            if (wait == WaitFailed)
            {
                break;
            }

            if (wait == WaitTimeout || wait != WaitObject0)
            {
                continue;
            }

            if (!GetNumberOfConsoleInputEvents(inputHandle, out var eventCount) || eventCount == 0)
            {
                continue;
            }

            if (!PeekConsoleInput(inputHandle, records, (uint)records.Length, out var read) || read == 0)
            {
                continue;
            }

            for (var i = 0; i < read; i++)
            {
                if (records[i].EventType != WindowBufferSizeEventType)
                {
                    continue;
                }

                var size = records[i].WindowBufferSizeEvent.Size;
                if (size.X <= 0 || size.Y <= 0)
                {
                    continue;
                }

                if (lastReportedSize is { } previous && previous.X == size.X && previous.Y == size.Y)
                {
                    continue;
                }

                lastReportedSize = size;
                try
                {
                    onResize();
                }
                catch
                {
                    // Resize callbacks must not tear down the watcher.
                }

                break;
            }
        }
    }

    private static bool IsInvalidHandle(IntPtr handle)
    {
        return handle == IntPtr.Zero || handle == new IntPtr(-1);
    }

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial IntPtr GetStdHandle(int nStdHandle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, out uint lpcNumberOfEvents);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool PeekConsoleInput(
        IntPtr hConsoleInput,
        [MarshalUsing(CountElementName = nameof(nLength))]
        [Out]
        InputRecord[] lpBuffer,
        uint nLength,
        out uint lpNumberOfEventsRead);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    private static partial uint WaitForSingleObject(IntPtr hHandle, uint dwMilliseconds);

    [StructLayout(LayoutKind.Sequential)]
    private struct Coord
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct WindowBufferSizeRecord
    {
        public Coord Size;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputRecord
    {
        [FieldOffset(0)] public ushort EventType;

        [FieldOffset(4)] public WindowBufferSizeRecord WindowBufferSizeEvent;
    }
}
