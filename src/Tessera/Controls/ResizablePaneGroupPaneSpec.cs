namespace Tessera.Controls;

/// <summary>
/// Describes a pane used by <see cref="ResizablePaneGroup" />.
/// </summary>
public sealed record PaneSpec
{
    /// <summary>
    /// Initializes a pane descriptor.
    /// </summary>
    /// <param name="id">Stable pane identifier.</param>
    /// <param name="content">Optional content control rendered inside the pane.</param>
    /// <param name="title">Optional pane label used when <paramref name="content" /> is <see langword="null" />.</param>
    /// <param name="minSize">Minimum pane width in cells.</param>
    /// <exception cref="ArgumentException"><paramref name="id" /> is empty or whitespace.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="minSize" /> is less than one.</exception>
    public PaneSpec(string id, Control? content = null, string? title = null, int minSize = 6)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ArgumentException("Pane id must be non-empty.", nameof(id));
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(minSize, 1);

        Id = id;
        Content = content;
        Title = title ?? string.Empty;
        MinSize = minSize;
    }

    /// <summary>
    /// Gets pane identifier.
    /// </summary>
    public string Id { get; init; }

    /// <summary>
    /// Gets pane content control.
    /// </summary>
    public Control? Content { get; init; }

    /// <summary>
    /// Gets optional pane title fallback.
    /// </summary>
    public string Title { get; init; }

    /// <summary>
    /// Gets minimum pane width in cells.
    /// </summary>
    public int MinSize { get; init; }
}
