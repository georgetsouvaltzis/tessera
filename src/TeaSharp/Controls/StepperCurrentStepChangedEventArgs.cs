namespace TeaSharp.Controls;

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
    /// Gets the current step index.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets the previous step.
    /// </summary>
    public StepperStep? PreviousStep { get; }

    /// <summary>
    /// Gets the current step.
    /// </summary>
    public StepperStep? CurrentStep { get; }
}
