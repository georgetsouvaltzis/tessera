namespace Tessera.Controls;

/// <summary>
/// Provides previous/current values when <see cref="Wizard.SelectionChanged"/> fires.
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
    /// Gets selected index after the change.
    /// Compatibility alias for <see cref="SelectedIndex" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets selected index after the change.
    /// Canonical property for selection access.
    /// </summary>
    public int SelectedIndex => CurrentIndex;

    /// <summary>
    /// Gets previously selected step.
    /// </summary>
    public WizardStep? PreviousStep { get; }

    /// <summary>
    /// Gets selected step after the change.
    /// Compatibility alias for <see cref="SelectedStep" />.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public WizardStep? CurrentStep { get; }

    /// <summary>
    /// Gets selected step after the change.
    /// Canonical property for selection access.
    /// </summary>
    public WizardStep? SelectedStep => CurrentStep;
}
