using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
///     Represents the renderer seam used by advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IProgramRenderer : IAsyncDisposable
{
    /// <summary>
    ///     Initializes the renderer against the terminal output stream.
    /// </summary>
    /// <param name="output">The output stream to write to.</param>
    /// <param name="cancellationToken">Cancels initialization.</param>
    ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken);

    /// <summary>
    ///     Updates the renderer with the latest terminal size.
    /// </summary>
    /// <param name="width">The width in columns.</param>
    /// <param name="height">The height in rows.</param>
    void Resize(int width, int height);

    /// <summary>
    ///     Applies the latest terminal capability profile.
    /// </summary>
    /// <param name="capabilities">The terminal capability profile.</param>
    void UpdateCapabilities(TerminalCapabilityProfile capabilities);

    /// <summary>
    ///     Renders the next frame output.
    /// </summary>
    /// <param name="output">The composed render output.</param>
    void Render(RenderOutput output);

    /// <summary>
    ///     Writes raw terminal content without diffing.
    /// </summary>
    /// <param name="content">The raw terminal content.</param>
    /// <param name="cancellationToken">Cancels the write.</param>
    ValueTask WriteRawAsync(string content, CancellationToken cancellationToken);

    /// <summary>
    ///     Flushes pending terminal output.
    /// </summary>
    /// <param name="cancellationToken">Cancels the flush.</param>
    ValueTask FlushAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Resets renderer-owned terminal state.
    /// </summary>
    /// <param name="cancellationToken">Cancels the reset.</param>
    ValueTask ResetAsync(CancellationToken cancellationToken);
}
