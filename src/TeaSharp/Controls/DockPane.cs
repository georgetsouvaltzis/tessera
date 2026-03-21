namespace TeaSharp.Controls;

/// <summary>
/// Identifies docking position for a <see cref="DockPane" />.
/// </summary>
public enum DockPanePosition
{
    /// <summary>
    /// Docks pane to top edge.
    /// </summary>
    Top = 0,

    /// <summary>
    /// Docks pane to bottom edge.
    /// </summary>
    Bottom = 1,

    /// <summary>
    /// Docks pane to left edge.
    /// </summary>
    Left = 2,

    /// <summary>
    /// Docks pane to right edge.
    /// </summary>
    Right = 3,

    /// <summary>
    /// Uses remaining center area.
    /// </summary>
    Center = 4,
}

/// <summary>
/// Represents one pane inside <see cref="DockWorkspace" />.
/// </summary>
public sealed class DockPane
{
    /// <summary>
    /// Initializes a dock pane.
    /// </summary>
    /// <param name="id">Stable pane identifier.</param>
    /// <param name="title">Pane title.</param>
    /// <param name="position">Docking position.</param>
    /// <param name="size">Preferred size in rows/columns for edge panes.</param>
    public DockPane(string id, string title, DockPanePosition position = DockPanePosition.Center, int size = 8)
    {
        Id = id ?? string.Empty;
        Title = title ?? string.Empty;
        Position = position;
        Size = Math.Max(1, size);
    }

    /// <summary>
    /// Gets stable pane identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets or sets pane title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets pane docking position.
    /// </summary>
    public DockPanePosition Position { get; set; } = DockPanePosition.Center;

    /// <summary>
    /// Gets or sets preferred size for edge-docked panes.
    /// </summary>
    public int Size { get; set; } = 8;

    /// <summary>
    /// Gets or sets pane body lines when <see cref="Content" /> is not provided.
    /// </summary>
    public IReadOnlyList<string> Lines { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Gets or sets optional child control rendered inside pane body.
    /// </summary>
    public Control? Content { get; set; }

    /// <summary>
    /// Gets or sets whether pane renders muted.
    /// </summary>
    public bool IsMuted { get; set; }

    /// <summary>
    /// Gets or sets whether pane is disabled/non-selectable.
    /// </summary>
    public bool IsDisabled { get; set; }
}
