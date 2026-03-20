namespace TeaSharp.Controls;

/// <summary>
/// Provides selection details when a <see cref="KanbanBoard"/> selection changes.
/// </summary>
public sealed class KanbanSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new selection payload.
    /// </summary>
    /// <param name="previousLaneIndex">Previously selected lane index.</param>
    /// <param name="previousCardIndex">Previously selected card index within the lane.</param>
    /// <param name="selectedLaneIndex">Currently selected lane index.</param>
    /// <param name="selectedCardIndex">Currently selected card index within the lane.</param>
    /// <param name="previousLane">Previously selected lane.</param>
    /// <param name="previousCard">Previously selected card.</param>
    /// <param name="selectedLane">Currently selected lane.</param>
    /// <param name="selectedCard">Currently selected card.</param>
    public KanbanSelectionChangedEventArgs(
        int previousLaneIndex,
        int previousCardIndex,
        int selectedLaneIndex,
        int selectedCardIndex,
        KanbanLane? previousLane,
        KanbanCard? previousCard,
        KanbanLane? selectedLane,
        KanbanCard? selectedCard)
    {
        PreviousLaneIndex = previousLaneIndex;
        PreviousCardIndex = previousCardIndex;
        SelectedLaneIndex = selectedLaneIndex;
        SelectedCardIndex = selectedCardIndex;
        PreviousLane = previousLane;
        PreviousCard = previousCard;
        SelectedLane = selectedLane;
        SelectedCard = selectedCard;
    }

    /// <summary>
    /// Gets previously selected lane index.
    /// </summary>
    public int PreviousLaneIndex { get; }

    /// <summary>
    /// Gets previously selected card index.
    /// </summary>
    public int PreviousCardIndex { get; }

    /// <summary>
    /// Gets currently selected lane index.
    /// </summary>
    public int SelectedLaneIndex { get; }

    /// <summary>
    /// Gets currently selected card index.
    /// </summary>
    public int SelectedCardIndex { get; }

    /// <summary>
    /// Gets previously selected lane.
    /// </summary>
    public KanbanLane? PreviousLane { get; }

    /// <summary>
    /// Gets previously selected card.
    /// </summary>
    public KanbanCard? PreviousCard { get; }

    /// <summary>
    /// Gets currently selected lane.
    /// </summary>
    public KanbanLane? SelectedLane { get; }

    /// <summary>
    /// Gets currently selected card.
    /// </summary>
    public KanbanCard? SelectedCard { get; }
}
