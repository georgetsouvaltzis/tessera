using System.ComponentModel;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Hosting;

/// <summary>
/// Wraps the built-in console terminal adapter for advanced TeaSharp hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class ConsoleTerminalAdapter : ITerminalAdapter
{
    private readonly global::TeaSharp.Core.Terminal.ConsoleTerminalAdapter _inner = new();

    public Stream Input => _inner.Input;

    public Stream Output => _inner.Output;

    public bool IsInputInteractive => _inner.IsInputInteractive;

    public bool IsOutputInteractive => _inner.IsOutputInteractive;

    public ValueTask PrepareAsync(CancellationToken cancellationToken) =>
        _inner.PrepareAsync(cancellationToken);

    public ValueTask RestoreAsync(CancellationToken cancellationToken) =>
        _inner.RestoreAsync(cancellationToken);

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken) =>
        _inner.GetSizeAsync(cancellationToken);

    public ValueTask DisposeAsync() => _inner.DisposeAsync();
}
