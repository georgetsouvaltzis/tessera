namespace Tessera.Controls;

/// <summary>
/// Defines label wrappers and shortcut delimiters used by <see cref="MenuBar"/>.
/// </summary>
public readonly record struct MenuBarGlyphSet
{
    /// <summary>
    /// Gets the default glyph set used by menu bars.
    /// </summary>
    public static MenuBarGlyphSet Default => new();

    /// <summary>
    /// Initializes a new glyph set with built-in menu bar wrappers.
    /// </summary>
    public MenuBarGlyphSet()
    {
        SelectedPrefix = "[";
        SelectedSuffix = "]";
        UnselectedPrefix = " ";
        UnselectedSuffix = " ";
        HoveredPrefix = ">";
        HoveredSuffix = "<";
        ShortcutOpen = "(";
        ShortcutClose = ")";
    }

    /// <summary>
    /// Initializes a new glyph set.
    /// </summary>
    /// <param name="selectedPrefix">Prefix used around selected menu labels.</param>
    /// <param name="selectedSuffix">Suffix used around selected menu labels.</param>
    /// <param name="unselectedPrefix">Prefix used around unselected menu labels.</param>
    /// <param name="unselectedSuffix">Suffix used around unselected menu labels.</param>
    /// <param name="hoveredPrefix">Prefix used around hovered unselected menu labels.</param>
    /// <param name="hoveredSuffix">Suffix used around hovered unselected menu labels.</param>
    /// <param name="shortcutOpen">Opening delimiter used for menu shortcuts.</param>
    /// <param name="shortcutClose">Closing delimiter used for menu shortcuts.</param>
    public MenuBarGlyphSet(
        string selectedPrefix,
        string selectedSuffix,
        string unselectedPrefix,
        string unselectedSuffix,
        string hoveredPrefix,
        string hoveredSuffix,
        string shortcutOpen,
        string shortcutClose)
    {
        SelectedPrefix = selectedPrefix ?? string.Empty;
        SelectedSuffix = selectedSuffix ?? string.Empty;
        UnselectedPrefix = unselectedPrefix ?? string.Empty;
        UnselectedSuffix = unselectedSuffix ?? string.Empty;
        HoveredPrefix = hoveredPrefix ?? string.Empty;
        HoveredSuffix = hoveredSuffix ?? string.Empty;
        ShortcutOpen = shortcutOpen ?? string.Empty;
        ShortcutClose = shortcutClose ?? string.Empty;
    }

    /// <summary>
    /// Gets the prefix used around selected menu labels.
    /// </summary>
    public string SelectedPrefix { get; init; }

    /// <summary>
    /// Gets the suffix used around selected menu labels.
    /// </summary>
    public string SelectedSuffix { get; init; }

    /// <summary>
    /// Gets the prefix used around unselected menu labels.
    /// </summary>
    public string UnselectedPrefix { get; init; }

    /// <summary>
    /// Gets the suffix used around unselected menu labels.
    /// </summary>
    public string UnselectedSuffix { get; init; }

    /// <summary>
    /// Gets the prefix used around hovered unselected menu labels.
    /// </summary>
    public string HoveredPrefix { get; init; }

    /// <summary>
    /// Gets the suffix used around hovered unselected menu labels.
    /// </summary>
    public string HoveredSuffix { get; init; }

    /// <summary>
    /// Gets the opening delimiter used for menu shortcuts.
    /// </summary>
    public string ShortcutOpen { get; init; }

    /// <summary>
    /// Gets the closing delimiter used for menu shortcuts.
    /// </summary>
    public string ShortcutClose { get; init; }
}
