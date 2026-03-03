using TeaSharp.Core.Abstractions;

namespace TeaSharp.Core.Rendering;

public sealed class NullRenderer : IProgramRenderer
{
    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public void Resize(int width, int height)
    {
        _ = width;
        _ = height;
    }

    public void Render(View view)
    {
        _ = view;
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask ResetAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
