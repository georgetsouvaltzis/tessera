using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Rendering;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface IProgramRenderer : IAsyncDisposable
{
    ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken);
    void Resize(int width, int height);
    void UpdateCapabilities(TerminalCapabilityProfile capabilities);
    void Render(ScreenOutput output);
    ValueTask WriteRawAsync(string content, CancellationToken cancellationToken);
    ValueTask FlushAsync(CancellationToken cancellationToken);
    ValueTask ResetAsync(CancellationToken cancellationToken);
}
