namespace Tessera.Controls;

/// <summary>
///     Represents one group in <see cref="GroupedListView{TGroup,TItem}" />.
/// </summary>
/// <typeparam name="TGroup">The group-key type.</typeparam>
/// <typeparam name="TItem">The item type.</typeparam>
public sealed class GroupedListViewGroup<TGroup, TItem>
{
    /// <summary>
    ///     Initializes a new grouped list section.
    /// </summary>
    /// <param name="group">The group key/value.</param>
    /// <param name="items">Items within the group.</param>
    public GroupedListViewGroup(TGroup group, IEnumerable<TItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        Group = group;
        Items = items is IReadOnlyList<TItem> readOnlyList
            ? readOnlyList
            : items.ToList();
    }

    /// <summary>
    ///     Gets or sets the group key/value.
    /// </summary>
    public TGroup Group { get; set; }

    /// <summary>
    ///     Gets or sets items within the group.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; set; }

    /// <summary>
    ///     Gets or sets whether item rows are collapsed.
    /// </summary>
    public bool IsCollapsed { get; set; }
}
