namespace TeaSharp.Controls;

/// <summary>
/// Represents one row in a <see cref="PropertyGrid"/>.
/// </summary>
public sealed class PropertyGridProperty
{
    /// <summary>
    /// Initializes an empty property row.
    /// </summary>
    public PropertyGridProperty()
    {
    }

    /// <summary>
    /// Initializes a property row.
    /// </summary>
    /// <param name="name">The property name (left column).</param>
    /// <param name="value">The property value (right column).</param>
    /// <param name="category">Optional category name used for grouping.</param>
    public PropertyGridProperty(string name, string value, string? category = null)
    {
        Name = name;
        Value = value;
        Category = category;
    }

    /// <summary>
    /// Gets or sets the property name shown in the key column.
    /// </summary>
    public string Name
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the property value shown in the value column.
    /// </summary>
    public string Value
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the optional category used for grouping.
    /// </summary>
    public string? Category
    {
        get;
        set => field = string.IsNullOrWhiteSpace(value) ? null : value;
    }
}
