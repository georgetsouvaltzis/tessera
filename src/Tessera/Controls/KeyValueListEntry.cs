namespace Tessera.Controls;

/// <summary>
/// Represents one key/value row in a <see cref="KeyValueList"/>.
/// </summary>
public sealed class KeyValueListEntry
{
    /// <summary>
    /// Initializes an empty key/value row.
    /// </summary>
    public KeyValueListEntry()
    {
    }

    /// <summary>
    /// Initializes a key/value row.
    /// </summary>
    /// <param name="key">Key text shown in the left column.</param>
    /// <param name="value">Value text shown in the right column.</param>
    public KeyValueListEntry(string key, string value)
    {
        Key = key ?? string.Empty;
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the key text.
    /// </summary>
    public string Key
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the value text.
    /// </summary>
    public string Value
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;
}
