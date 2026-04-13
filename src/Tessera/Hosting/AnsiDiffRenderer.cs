using System.ComponentModel;
using Tessera.Internal;

namespace Tessera.Hosting;

/// <summary>
///     Wraps the built-in ANSI diff renderer for advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class AnsiDiffRenderer : IProgramRenderer
{
    private readonly Core.Rendering.AnsiDiffRenderer _inner;

    /// <summary>
    ///     Initializes a hosting renderer wrapper over the built-in ANSI diff renderer.
    /// </summary>
    public AnsiDiffRenderer(
        TerminalCapabilityProfile? capabilities = null,
        AnsiRendererOptions? options = null)
    {
        _inner = new Core.Rendering.AnsiDiffRenderer(capabilities?.ToCore(), options?.ToCore());
    }

    /// <summary>
    ///     Initializes the renderer against the supplied terminal output stream.
    /// </summary>
    /// <param name="output">The output stream to write ANSI sequences to.</param>
    /// <param name="cancellationToken">Cancels initialization.</param>
    /// <returns>A task that completes when initialization finishes.</returns>
    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken)
    {
        return _inner.InitializeAsync(output, cancellationToken);
    }

    /// <summary>
    ///     Updates the renderer with the latest terminal dimensions.
    /// </summary>
    /// <param name="width">The width in columns.</param>
    /// <param name="height">The height in rows.</param>
    public void Resize(int width, int height)
    {
        _inner.Resize(width, height);
    }

    /// <summary>
    ///     Applies the latest detected terminal capability profile.
    /// </summary>
    /// <param name="capabilities">The terminal capability profile.</param>
    public void UpdateCapabilities(TerminalCapabilityProfile capabilities)
    {
        _inner.UpdateCapabilities(capabilities.ToCore());
    }

    /// <summary>
    ///     Renders the next composed frame.
    /// </summary>
    /// <param name="output">The composed render output.</param>
    public void Render(RenderOutput output)
    {
        _inner.Render(output.ToCore());
    }

    /// <summary>
    ///     Writes raw terminal content without diffing.
    /// </summary>
    /// <param name="content">The terminal content to write directly.</param>
    /// <param name="cancellationToken">Cancels the write.</param>
    /// <returns>A task that completes when the write finishes.</returns>
    public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken)
    {
        return _inner.WriteRawAsync(content, cancellationToken);
    }

    /// <summary>
    ///     Flushes pending terminal output.
    /// </summary>
    /// <param name="cancellationToken">Cancels the flush.</param>
    /// <returns>A task that completes when flushing finishes.</returns>
    public ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        return _inner.FlushAsync(cancellationToken);
    }

    /// <summary>
    ///     Resets renderer-owned terminal state.
    /// </summary>
    /// <param name="cancellationToken">Cancels the reset.</param>
    /// <returns>A task that completes when reset finishes.</returns>
    public ValueTask ResetAsync(CancellationToken cancellationToken)
    {
        return _inner.ResetAsync(cancellationToken);
    }

    /// <summary>
    ///     Disposes the renderer.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        return _inner.DisposeAsync();
    }
}
