using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Represents the renderer seam used by advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IProgramRenderer : IAsyncDisposable
{
    ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken);

    void Resize(int width, int height);

    void UpdateCapabilities(TerminalCapabilityProfile capabilities);

    void Render(RenderOutput output);

    ValueTask WriteRawAsync(string content, CancellationToken cancellationToken);

    ValueTask FlushAsync(CancellationToken cancellationToken);

    ValueTask ResetAsync(CancellationToken cancellationToken);
}
