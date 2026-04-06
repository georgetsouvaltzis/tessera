namespace Tessera.Controls;

/// <summary>
/// Provides details when the current step changes.
/// </summary>
public sealed class StepperCurrentStepChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new step transition payload.
    /// </summary>
    /// <param name="previousIndex">Previous current step index.</param>
    /// <param name="currentIndex">Current step index.</param>
    /// <param name="previousStep">Previous step reference.</param>
    /// <param name="currentStep">Current step reference.</param>
    public StepperCurrentStepChangedEventArgs(
        int previousIndex,
        int currentIndex,
        StepperStep? previousStep,
        StepperStep? currentStep)
    {
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousStep = previousStep;
        CurrentStep = currentStep;
    }

    /// <summary>
    /// Gets the previous step index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets the selected step index after the change.
    /// Compatibility alias for <see cref="SelectedIndex" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets the selected step index after the change.
    /// Canonical property for selection access.
    /// </summary>
    public int SelectedIndex => CurrentIndex;

    /// <summary>
    /// Gets the previous step.
    /// </summary>
    public StepperStep? PreviousStep { get; }

    /// <summary>
    /// Gets the selected step after the change.
    /// Compatibility alias for <see cref="SelectedStep" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public StepperStep? CurrentStep { get; }

    /// <summary>
    /// Gets the selected step after the change.
    /// Canonical property for selection access.
    /// </summary>
    public StepperStep? SelectedStep => CurrentStep;
}
