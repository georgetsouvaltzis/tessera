namespace TeaSharp.Controls;

/// <summary>
/// Provides previous/current tag snapshots for <see cref="TagInput.TagsChanged" />.
/// </summary>
public sealed class TagInputTagsChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a tag collection change payload.
    /// </summary>
    /// <param name="previousTags">Tag values before the change.</param>
    /// <param name="tags">Tag values after the change.</param>
    public TagInputTagsChangedEventArgs(IReadOnlyList<string> previousTags, IReadOnlyList<string> tags)
    {
        PreviousTags = [.. previousTags];
        Tags = [.. tags];
    }

    /// <summary>
    /// Gets the tag values before the change.
    /// </summary>
    public IReadOnlyList<string> PreviousTags { get; }

    /// <summary>
    /// Gets the current tag values.
    /// </summary>
    public IReadOnlyList<string> Tags { get; }
}
