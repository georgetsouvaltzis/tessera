namespace TeaSharp.Controls;

/// <summary>
/// Provides previous/current values when <see cref="Wizard.StepChanged"/> fires.
/// </summary>
public sealed class WizardStepChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes step-change payload.
    /// </summary>
    /// <param name="previousIndex">Previously selected index.</param>
    /// <param name="currentIndex">Current selected index.</param>
    /// <param name="previousStep">Previously selected step.</param>
    /// <param name="currentStep">Current selected step.</param>
    public WizardStepChangedEventArgs(
        int previousIndex,
        int currentIndex,
        WizardStep? previousStep,
        WizardStep? currentStep)
    {
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousStep = previousStep;
        CurrentStep = currentStep;
    }

    /// <summary>
    /// Gets previously selected index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets current selected index.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets previously selected step.
    /// </summary>
    public WizardStep? PreviousStep { get; }

    /// <summary>
    /// Gets current selected step.
    /// </summary>
    public WizardStep? CurrentStep { get; }
}
