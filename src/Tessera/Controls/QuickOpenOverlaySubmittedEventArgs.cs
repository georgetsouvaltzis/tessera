namespace Tessera.Controls;

/// <summary>
///     Provides submission payload for <see cref="QuickOpenOverlay.Submitted" />.
/// </summary>
public sealed class QuickOpenOverlaySubmittedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a submission payload.
    /// </summary>
    /// <param name="item">The submitted item.</param>
    /// <param name="query">The query used at submission time.</param>
    public QuickOpenOverlaySubmittedEventArgs(QuickOpenItem item, string query)
    {
        Item = item;
        Query = query;
    }

    /// <summary>
    ///     Gets the submitted item.
    /// </summary>
    public QuickOpenItem Item { get; }

    /// <summary>
    ///     Gets the submitted item identifier.
    /// </summary>
    public string ItemId => Item.Id;

    /// <summary>
    ///     Gets the query value used at submission time.
    /// </summary>
    public string Query { get; }
}
