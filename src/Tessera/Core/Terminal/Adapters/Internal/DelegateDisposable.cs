namespace Tessera.Core.Terminal.Adapters.Internal;

internal sealed class DelegateDisposable(Action dispose) : IDisposable
{
    public void Dispose()
    {
        dispose();
    }
}
