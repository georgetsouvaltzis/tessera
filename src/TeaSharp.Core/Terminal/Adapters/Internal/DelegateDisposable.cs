namespace TeaSharp.Core.Terminal;

internal sealed class DelegateDisposable(Action dispose) : IDisposable
{
    public void Dispose()
    {
        dispose();
    }
}
