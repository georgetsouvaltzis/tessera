using System.ComponentModel;
using Tessera.Core.Abstractions;
using Tessera.Core.Terminal.Capabilities;

namespace Tessera.Core.Rendering;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class NullRenderer : IProgramRenderer
{
    public ValueTask InitializeAsync(Stream output, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

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

    public ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask ResetAsync(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
