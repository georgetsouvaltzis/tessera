using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
///     Represents the terminal adapter seam used by advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface ITerminalAdapter : IAsyncDisposable
{
    /// <summary>
    ///     Gets the terminal input stream.
    /// </summary>
    Stream Input { get; }

    /// <summary>
    ///     Gets the terminal output stream.
    /// </summary>
    Stream Output { get; }

    /// <summary>
    ///     Gets whether the input stream is interactive.
    /// </summary>
    bool IsInputInteractive { get; }

    /// <summary>
    ///     Gets whether the output stream is interactive.
    /// </summary>
    bool IsOutputInteractive { get; }

    /// <summary>
    ///     Prepares the terminal for Tessera runtime execution.
    /// </summary>
    /// <param name="cancellationToken">Cancels preparation.</param>
    ValueTask PrepareAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Restores the terminal to its original state.
    /// </summary>
    /// <param name="cancellationToken">Cancels restoration.</param>
    ValueTask RestoreAsync(CancellationToken cancellationToken);

    /// <summary>
    ///     Reads the current terminal size.
    /// </summary>
    /// <param name="cancellationToken">Cancels the size query.</param>
    /// <returns>The current terminal size.</returns>
    ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken);
}
