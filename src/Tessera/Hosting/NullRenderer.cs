using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Wraps the no-op renderer for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class NullRenderer : IProgramRenderer
{
    private readonly global::Tessera.Core.Rendering.NullRenderer _inner = new();

    /// <inheritdoc />
    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken) =>
        _inner.InitializeAsync(output, cancellationToken);

    /// <inheritdoc />
    public void Resize(int width, int height) => _inner.Resize(width, height);

    /// <inheritdoc />
    public void UpdateCapabilities(TerminalCapabilityProfile capabilities) =>
        _inner.UpdateCapabilities(capabilities.ToCore());

    /// <inheritdoc />
    public void Render(RenderOutput output) =>
        _inner.Render(output.ToCore());

    /// <inheritdoc />
    public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken) =>
        _inner.WriteRawAsync(content, cancellationToken);

    /// <inheritdoc />
    public ValueTask FlushAsync(CancellationToken cancellationToken) =>
        _inner.FlushAsync(cancellationToken);

    /// <inheritdoc />
    public ValueTask ResetAsync(CancellationToken cancellationToken) =>
        _inner.ResetAsync(cancellationToken);

    /// <summary>
    /// Disposes the renderer.
    /// </summary>
    public ValueTask DisposeAsync() => _inner.DisposeAsync();
}
