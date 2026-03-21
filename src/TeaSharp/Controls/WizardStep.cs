namespace TeaSharp.Controls;

/// <summary>
/// Represents one step in a <see cref="Wizard"/> flow.
/// </summary>
public sealed class WizardStep
{
    /// <summary>
    /// Initializes a wizard step.
    /// </summary>
    /// <param name="id">Stable step identifier.</param>
    /// <param name="title">Primary step title text.</param>
    /// <param name="description">Optional secondary description text.</param>
    /// <param name="isCompleted">Whether the step is completed.</param>
    /// <param name="isDisabled">Whether the step is disabled.</param>
    public WizardStep(
        string id,
        string title,
        string? description = null,
        bool isCompleted = false,
        bool isDisabled = false)
    {
        Id = id ?? string.Empty;
        Title = title ?? string.Empty;
        Description = description ?? string.Empty;
        IsCompleted = isCompleted;
        IsDisabled = isDisabled;
    }

    /// <summary>
    /// Gets or sets stable step identifier.
    /// </summary>
    public string Id
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets step title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets optional step description text.
    /// </summary>
    public string Description
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets whether the step is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    /// Gets or sets whether the step is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }
}
