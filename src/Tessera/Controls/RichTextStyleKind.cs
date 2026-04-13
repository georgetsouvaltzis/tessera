namespace Tessera.Controls;

/// <summary>
///     Represents semantic style categories for <see cref="RichTextSegment" /> values rendered by
///     <see cref="RichTextView" />.
/// </summary>
public enum RichTextStyleKind
{
    /// <summary>
    ///     Plain body text.
    /// </summary>
    Plain = 0,

    /// <summary>
    ///     Heading text.
    /// </summary>
    Heading = 1,

    /// <summary>
    ///     List marker text (for example <c>- </c> or <c>1. </c>).
    /// </summary>
    ListMarker = 2,

    /// <summary>
    ///     Quote marker text (for example <c>&gt; </c>).
    /// </summary>
    QuoteMarker = 3,

    /// <summary>
    ///     Inline emphasis text.
    /// </summary>
    Emphasis = 4,

    /// <summary>
    ///     Inline strong-emphasis text.
    /// </summary>
    Strong = 5,

    /// <summary>
    ///     Inline code text.
    /// </summary>
    InlineCode = 6
}
