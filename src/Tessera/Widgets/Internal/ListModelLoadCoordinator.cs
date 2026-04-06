namespace Tessera.Widgets.Internal;

internal sealed class ListModelLoadCoordinator
{
    private readonly object _gate = new();
    private int _version;
    private CancellationTokenSource? _activeLoadCts;

    public (int Version, CancellationToken Token, Action Dispose) Begin(CancellationToken cancellationToken)
    {
        CancellationTokenSource linkedCts;
        int version;
        lock (_gate)
        {
            _activeLoadCts?.Cancel();
            _activeLoadCts?.Dispose();
            _activeLoadCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts = _activeLoadCts;
            version = ++_version;
        }

        return (version, linkedCts.Token, () =>
        {
            lock (_gate)
            {
                if (ReferenceEquals(_activeLoadCts, linkedCts))
                {
                    _activeLoadCts.Dispose();
                    _activeLoadCts = null;
                }
                else
                {
                    linkedCts.Dispose();
                }
            }
        });
    }

    public bool IsCurrent(int version)
    {
        lock (_gate)
        {
            return version == _version;
        }
    }
}
