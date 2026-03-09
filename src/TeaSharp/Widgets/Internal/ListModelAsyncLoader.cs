namespace TeaSharp.Widgets.Internal;

internal static class ListModelAsyncLoader
{
    public static async ValueTask AppendItemsAsync<T>(List<T> target, IAsyncEnumerable<T> items, CancellationToken cancellationToken)
    {
        await foreach (var item in items.ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();
            target.Add(item);
        }
    }

    public static async ValueTask<List<T>> MaterializeAsync<T>(IAsyncEnumerable<T> items, CancellationToken cancellationToken)
    {
        var result = new List<T>();
        await AppendItemsAsync(result, items, cancellationToken).ConfigureAwait(false);
        return result;
    }
}
