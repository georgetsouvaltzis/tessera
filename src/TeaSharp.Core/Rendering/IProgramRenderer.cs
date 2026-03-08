using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Rendering;

public interface IProgramRenderer : IAsyncDisposable
{
    ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken);
    void Resize(int width, int height);
    void UpdateCapabilities(TerminalCapabilityProfile capabilities);
    void Render(View view);
    ValueTask WriteRawAsync(string content, CancellationToken cancellationToken);
    ValueTask FlushAsync(CancellationToken cancellationToken);
    ValueTask ResetAsync(CancellationToken cancellationToken);
}
