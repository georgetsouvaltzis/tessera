namespace TeaSharp.Controls;

/// <summary>
/// Represents one semantically-styled inline segment rendered by <see cref="RichTextView"/>.
/// </summary>
public readonly record struct RichTextSegment
{
    /// <summary>
    /// Initializes a new segment.
    /// </summary>
    /// <param name="text">The segment text.</param>
    /// <param name="styleKind">The semantic style category for this segment.</param>
    public RichTextSegment(string? text, RichTextStyleKind styleKind = RichTextStyleKind.Plain)
    {
        Text = text ?? string.Empty;
        StyleKind = styleKind;
    }

    /// <summary>
    /// Gets the segment text.
    /// </summary>
    public string Text { get; init; }

    /// <summary>
    /// Gets the semantic style category for this segment.
    /// </summary>
    public RichTextStyleKind StyleKind { get; init; }

    /// <summary>
    /// Creates a plain body segment.
    /// </summary>
    /// <param name="text">The segment text.</param>
    /// <returns>A plain-text segment.</returns>
    public static RichTextSegment Plain(string? text) => new(text, RichTextStyleKind.Plain);

    /// <summary>
    /// Creates a heading segment and prepends markdown-style heading markers.
    /// </summary>
    /// <param name="text">The heading text.</param>
    /// <param name="level">Heading level from 1 through 6.</param>
    /// <returns>A heading segment.</returns>
    public static RichTextSegment Heading(string? text, int level = 1)
    {
        var normalizedLevel = Math.Clamp(level, 1, 6);
        return new($"{new string('#', normalizedLevel)} {(text ?? string.Empty)}", RichTextStyleKind.Heading);
    }

    /// <summary>
    /// Creates a list-marker segment and appends a trailing space.
    /// </summary>
    /// <param name="marker">The marker text (for example <c>-</c> or <c>1.</c>).</param>
    /// <returns>A list-marker segment.</returns>
    public static RichTextSegment ListMarker(string? marker = "-")
    {
        var value = string.IsNullOrWhiteSpace(marker) ? "-" : marker.Trim();
        return new($"{value} ", RichTextStyleKind.ListMarker);
    }

    /// <summary>
    /// Creates a quote-marker segment and appends a trailing space.
    /// </summary>
    /// <param name="marker">The marker text.</param>
    /// <returns>A quote-marker segment.</returns>
    public static RichTextSegment QuoteMarker(string? marker = ">")
    {
        var value = string.IsNullOrWhiteSpace(marker) ? ">" : marker.Trim();
        return new($"{value} ", RichTextStyleKind.QuoteMarker);
    }

    /// <summary>
    /// Creates an emphasized inline segment.
    /// </summary>
    /// <param name="text">The segment text.</param>
    /// <returns>An emphasized segment.</returns>
    public static RichTextSegment Emphasis(string? text) => new(text, RichTextStyleKind.Emphasis);

    /// <summary>
    /// Creates a strong-emphasis inline segment.
    /// </summary>
    /// <param name="text">The segment text.</param>
    /// <returns>A strong-emphasis segment.</returns>
    public static RichTextSegment Strong(string? text) => new(text, RichTextStyleKind.Strong);

    /// <summary>
    /// Creates an inline-code segment.
    /// </summary>
    /// <param name="text">The segment text.</param>
    /// <returns>An inline-code segment.</returns>
    public static RichTextSegment InlineCode(string? text) => new(text, RichTextStyleKind.InlineCode);
}
