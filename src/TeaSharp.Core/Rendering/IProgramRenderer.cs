using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Rendering;

public interface IProgramRenderer : IAsyncDisposable
{
    ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken);
    void Resize(int width, int height);
    void Render(View view);
    ValueTask FlushAsync(CancellationToken cancellationToken);
    ValueTask ResetAsync(CancellationToken cancellationToken);
}
