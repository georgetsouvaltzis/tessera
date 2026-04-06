using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
/// Wraps the built-in ANSI diff renderer for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class AnsiDiffRenderer : IProgramRenderer
{
    private readonly global::Tessera.Core.Rendering.AnsiDiffRenderer _inner;

    /// <summary>
    /// Initializes a hosting renderer wrapper over the built-in ANSI diff renderer.
    /// </summary>
    public AnsiDiffRenderer(
        TerminalCapabilityProfile? capabilities = null,
        AnsiRendererOptions? options = null)
    {
        _inner = new global::Tessera.Core.Rendering.AnsiDiffRenderer(capabilities?.ToCore(), options?.ToCore());
    }

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
