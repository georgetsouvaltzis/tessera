namespace Tessera.Controls;

/// <summary>
///     Represents one logical data-entry field rendered by <see cref="Form" />.
/// </summary>
public sealed class FormField
{
    /// <summary>
    ///     Initializes a form field row.
    /// </summary>
    /// <param name="name">Stable field identifier.</param>
    /// <param name="label">Field label text.</param>
    /// <param name="value">Field value text.</param>
    /// <param name="helperText">Optional helper/validation text.</param>
    /// <param name="isRequired">Whether this field is required.</param>
    /// <param name="isDisabled">Whether this field is disabled and non-selectable.</param>
    public FormField(
        string name,
        string label,
        string value = "",
        string? helperText = null,
        bool isRequired = false,
        bool isDisabled = false)
    {
        Name = name;
        Label = label;
        Value = value;
        HelperText = helperText ?? string.Empty;
        IsRequired = isRequired;
        IsDisabled = isDisabled;
    }

    /// <summary>
    ///     Gets stable field identifier.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Gets field label text.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets field value text.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets optional helper text.
    /// </summary>
    public string HelperText { get; }

    /// <summary>
    ///     Gets whether this field is required.
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    ///     Gets whether this field is disabled and non-selectable.
    /// </summary>
    public bool IsDisabled { get; }
}
