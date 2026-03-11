using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Core.Rendering;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class NullRenderer : IProgramRenderer
{
    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public void Resize(int width, int height)
    {
        _ = width;
        _ = height;
    }

    public void UpdateCapabilities(TerminalCapabilityProfile capabilities)
    {
        _ = capabilities;
    }

    public void Render(ScreenOutput output)
    {
        _ = output;
    }

    public ValueTask WriteRawAsync(string content, CancellationToken cancellationToken)
    {
        _ = content;
        _ = cancellationToken;
        return ValueTask.CompletedTask;
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask ResetAsync(CancellationToken cancellationToken) => ValueTask.CompletedTask;

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
