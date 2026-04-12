namespace Tessera.Controls;

/// <summary>
/// Represents one lane in a <see cref="KanbanBoard"/>.
/// </summary>
public sealed class KanbanLane
{
    private readonly List<KanbanCard> _cards = [];

    /// <summary>
    /// Initializes a new lane.
    /// </summary>
    /// <param name="title">Lane title text.</param>
    public KanbanLane(string title)
    {
        Title = title ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets an optional stable lane identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets lane title text.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets cards currently assigned to the lane.
    /// </summary>
    public IReadOnlyList<KanbanCard> Cards => _cards;

    /// <summary>
    /// Gets card count in the lane.
    /// </summary>
    public int Count => _cards.Count;

    /// <summary>
    /// Replaces cards in the lane.
    /// </summary>
    /// <param name="cards">Cards to assign.</param>
    public void SetCards(IEnumerable<KanbanCard> cards)
    {
        ArgumentNullException.ThrowIfNull(cards);
        _cards.Clear();
        foreach (var card in cards.Where(static card => card is not null))
        {
            _cards.Add(card);
        }
    }

    /// <summary>
    /// Adds one card to the lane.
    /// </summary>
    /// <param name="card">Card to add.</param>
    public void AddCard(KanbanCard card)
    {
        ArgumentNullException.ThrowIfNull(card);
        _cards.Add(card);
    }

    /// <summary>
    /// Removes one card at index.
    /// </summary>
    /// <param name="index">Card index to remove.</param>
    /// <returns><see langword="true"/> when removal succeeded.</returns>
    public bool RemoveCardAt(int index)
    {
        if ((uint)index >= (uint)_cards.Count)
        {
            return false;
        }

        _cards.RemoveAt(index);
        return true;
    }

    /// <summary>
    /// Clears all cards from the lane.
    /// </summary>
    public void ClearCards()
    {
        _cards.Clear();
    }
}
