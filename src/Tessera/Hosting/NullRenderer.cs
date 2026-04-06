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

    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken) =>
        _inner.InitializeAsync(output, cancellationToken);

    public void Resize(int width, int height) => _inner.Resize(width, height);

    public void UpdateCapabilities(TerminalCapabilityProfile capabilities) =>
        _inner.UpdateCapabilities(capabilities.ToCore());

    public void Render(RenderOutput output) =>
        _inner.Render(output.ToCore());

    public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken) =>
        _inner.WriteRawAsync(content, cancellationToken);

    public ValueTask FlushAsync(CancellationToken cancellationToken) =>
        _inner.FlushAsync(cancellationToken);

    public ValueTask ResetAsync(CancellationToken cancellationToken) =>
        _inner.ResetAsync(cancellationToken);

    public ValueTask DisposeAsync() => _inner.DisposeAsync();
}
