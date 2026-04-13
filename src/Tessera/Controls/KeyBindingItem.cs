namespace Tessera.Controls;

/// <summary>
///     Represents one key binding row in <see cref="KeyBindingHelpDialog" />.
/// </summary>
public sealed class KeyBindingItem
{
    /// <summary>
    ///     Initializes a key-binding row.
    /// </summary>
    /// <param name="keys">Shortcut text (for example <c>Ctrl+P</c>).</param>
    /// <param name="description">Human-readable action text.</param>
    /// <param name="group">Optional group/category label.</param>
    /// <param name="isGlobal"><see langword="true" /> when binding is global.</param>
    public KeyBindingItem(string keys, string description, string? group = null, bool isGlobal = false)
    {
        Keys = keys;
        Description = description;
        Group = group ?? string.Empty;
        IsGlobal = isGlobal;
    }

    /// <summary>
    ///     Gets or sets shortcut text.
    /// </summary>
    public string Keys { get; set; }

    /// <summary>
    ///     Gets or sets action description text.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets optional group/category label.
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    ///     Gets or sets whether this binding is global.
    /// </summary>
    public bool IsGlobal { get; set; }
}
