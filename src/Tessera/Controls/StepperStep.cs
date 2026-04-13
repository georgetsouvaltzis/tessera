namespace Tessera.Controls;

/// <summary>
///     Represents one step in a <see cref="Stepper" />.
/// </summary>
public sealed class StepperStep
{
    /// <summary>
    ///     Initializes a step.
    /// </summary>
    /// <param name="id">Stable step identifier.</param>
    /// <param name="label">Display label.</param>
    /// <param name="isCompleted"><see langword="true" /> when the step is completed.</param>
    /// <param name="isDisabled"><see langword="true" /> when the step cannot be selected.</param>
    public StepperStep(string id, string label, bool isCompleted = false, bool isDisabled = false)
    {
        Id = id;
        Label = label;
        IsCompleted = isCompleted;
        IsDisabled = isDisabled;
    }

    /// <summary>
    ///     Gets the stable step identifier.
    /// </summary>
    public string Id { get; }

    /// <summary>
    ///     Gets or sets display label.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the step is completed.
    /// </summary>
    public bool IsCompleted { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the step is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }
}
