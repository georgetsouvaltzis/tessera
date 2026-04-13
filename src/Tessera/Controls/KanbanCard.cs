namespace Tessera.Controls;

/// <summary>
///     Represents one card inside a <see cref="KanbanLane" />.
/// </summary>
public sealed class KanbanCard
{
    /// <summary>
    ///     Initializes a new card.
    /// </summary>
    /// <param name="title">Card title text.</param>
    /// <param name="description">Optional secondary card text.</param>
    public KanbanCard(string title, string? description = null)
    {
        Title = title;
        Description = description ?? string.Empty;
    }

    /// <summary>
    ///     Gets or sets an optional stable card identifier.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the card title text.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    ///     Gets or sets optional secondary card text.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets whether the card is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    ///     Gets or sets whether the card should render in error state.
    /// </summary>
    public bool HasError { get; set; }
}
