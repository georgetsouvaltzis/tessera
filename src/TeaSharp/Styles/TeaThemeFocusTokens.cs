namespace TeaSharp.Styles;

/// <summary>
/// Defines semantic focus styles for a <see cref="TeaTheme"/>.
/// </summary>
public sealed class TeaThemeFocusTokens
{
    /// <summary>
    /// Gets focus ring style for focused elements.
    /// </summary>
    public TeaStyle Ring { get; init; } = TeaStyle.Empty;

    /// <summary>
    /// Gets focus style for focused titles.
    /// </summary>
    public TeaStyle Title { get; init; } = TeaStyle.Empty;

    /// <summary>
    /// Gets focus style for focused borders.
    /// </summary>
    public TeaStyle Border { get; init; } = TeaStyle.Empty;

    /// <summary>
    /// Gets marker text appended by focus-capable overlays.
    /// </summary>
    /// <remarks>
    /// Empty means "unspecified" and allows control defaults to remain in effect.
    /// </remarks>
    public string Marker
    {
        get;
        init => field = value ?? string.Empty;
    } = string.Empty;
}
