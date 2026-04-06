namespace Tessera.Controls;

/// <summary>
/// Defines the visual mode used by a <see cref="DiffView"/>.
/// </summary>
public enum DiffViewMode
{
    /// <summary>
    /// Renders one combined line per diff entry.
    /// </summary>
    Inline = 0,

    /// <summary>
    /// Renders old/new columns side by side.
    /// </summary>
    SideBySide = 1,
}

/// <summary>
/// Identifies the line-level change kind in a <see cref="DiffLineEntry"/>.
/// </summary>
public enum DiffLineKind
{
    /// <summary>
    /// The line exists in both old and new content.
    /// </summary>
    Unchanged = 0,

    /// <summary>
    /// The line exists only in new content.
    /// </summary>
    Added = 1,

    /// <summary>
    /// The line exists only in old content.
    /// </summary>
    Removed = 2,
}

/// <summary>
/// Represents one line-level diff entry.
/// </summary>
public sealed record DiffLineEntry(int OldLineNumber, int NewLineNumber, DiffLineKind Kind, string OldText, string NewText);
