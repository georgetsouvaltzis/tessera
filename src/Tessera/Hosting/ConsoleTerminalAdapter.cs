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

    /// <inheritdoc />
    public Stream Input => _inner.Input;

    /// <inheritdoc />
    public Stream Output => _inner.Output;

    /// <inheritdoc />
    public bool IsInputInteractive => _inner.IsInputInteractive;

    /// <inheritdoc />
    public bool IsOutputInteractive => _inner.IsOutputInteractive;

    /// <inheritdoc />
    public ValueTask PrepareAsync(CancellationToken cancellationToken) =>
        _inner.PrepareAsync(cancellationToken);

    /// <inheritdoc />
    public ValueTask RestoreAsync(CancellationToken cancellationToken) =>
        _inner.RestoreAsync(cancellationToken);

    /// <inheritdoc />
    public ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken) =>
        _inner.GetSizeAsync(cancellationToken).AsHosting();

    /// <summary>
    /// Disposes the terminal adapter.
    /// </summary>
    public ValueTask DisposeAsync() => _inner.DisposeAsync();
}
