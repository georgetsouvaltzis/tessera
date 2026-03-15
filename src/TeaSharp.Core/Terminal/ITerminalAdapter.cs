using System.ComponentModel;

namespace TeaSharp.Core.Terminal;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface ITerminalAdapter : IAsyncDisposable
{
    Stream Input { get; }
    Stream Output { get; }
    bool IsInputInteractive { get; }
    bool IsOutputInteractive { get; }
    ValueTask PrepareAsync(CancellationToken cancellationToken);
    ValueTask RestoreAsync(CancellationToken cancellationToken);
    ValueTask<TerminalSize> GetSizeAsync(CancellationToken cancellationToken);
}
