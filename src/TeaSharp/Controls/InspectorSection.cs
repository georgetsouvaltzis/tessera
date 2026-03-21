namespace TeaSharp.Controls;

/// <summary>
/// Represents one collapsible section in an <see cref="InspectorPanel"/>.
/// </summary>
public sealed class InspectorSection
{
    /// <summary>
    /// Initializes a new section.
    /// </summary>
    /// <param name="title">Section title.</param>
    /// <param name="isExpanded"><see langword="true"/> to start expanded; otherwise <see langword="false"/>.</param>
    public InspectorSection(string? title, bool isExpanded = true)
    {
        Title = title ?? string.Empty;
        IsExpanded = isExpanded;
    }

    /// <summary>
    /// Gets or sets section title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets whether the section is expanded.
    /// </summary>
    public bool IsExpanded { get; set; } = true;

    /// <summary>
    /// Gets key/value fields rendered in this section.
    /// </summary>
    public IList<InspectorField> Fields { get; } = new List<InspectorField>();

    /// <summary>
    /// Gets detail rows rendered after fields.
    /// </summary>
    public IList<string> Details { get; } = new List<string>();

    /// <summary>
    /// Adds one key/value field.
    /// </summary>
    /// <param name="key">Field key text.</param>
    /// <param name="value">Field value text.</param>
    public void AddField(string? key, string? value)
    {
        Fields.Add(new InspectorField(key, value));
    }

    /// <summary>
    /// Adds one detail row.
    /// </summary>
    /// <param name="line">Detail text.</param>
    public void AddDetail(string? line)
    {
        Details.Add(line ?? string.Empty);
    }
}

/// <summary>
/// Represents one key/value row in <see cref="InspectorSection"/>.
/// </summary>
public readonly record struct InspectorField
{
    /// <summary>
    /// Initializes a new field row.
    /// </summary>
    /// <param name="key">Field key text.</param>
    /// <param name="value">Field value text.</param>
    public InspectorField(string? key, string? value)
    {
        Key = key ?? string.Empty;
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Gets field key text.
    /// </summary>
    public string Key { get; init; }

    /// <summary>
    /// Gets field value text.
    /// </summary>
    public string Value { get; init; }
}
