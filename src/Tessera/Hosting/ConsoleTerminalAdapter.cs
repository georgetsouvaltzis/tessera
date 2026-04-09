using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Wraps the built-in console terminal adapter for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class ConsoleTerminalAdapter : ITerminalAdapter
{
    private readonly global::Tessera.Core.Terminal.Adapters.ConsoleTerminalAdapter _inner = new();

    public Stream Input => _inner.Input;

    public Stream Output => _inner.Output;

    public bool IsInputInteractive => _inner.IsInputInteractive;

    public bool IsOutputInteractive => _inner.IsOutputInteractive;

    public ValueTask PrepareAsync(CancellationToken cancellationToken) =>
        _inner.PrepareAsync(cancellationToken);

    public ValueTask RestoreAsync(CancellationToken cancellationToken) =>
        _inner.RestoreAsync(cancellationToken);

    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken) =>
        _inner.GetSizeAsync(cancellationToken).AsHosting();

    public ValueTask DisposeAsync() => _inner.DisposeAsync();
}
