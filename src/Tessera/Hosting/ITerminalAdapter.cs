using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
/// Represents the terminal adapter seam used by advanced Tessera hosting scenarios.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface ITerminalAdapter : IAsyncDisposable
{
    Stream Input { get; }

    Stream Output { get; }

    bool IsInputInteractive { get; }

    bool IsOutputInteractive { get; }

    ValueTask PrepareAsync(CancellationToken cancellationToken);

    ValueTask RestoreAsync(CancellationToken cancellationToken);

    ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken);
}
