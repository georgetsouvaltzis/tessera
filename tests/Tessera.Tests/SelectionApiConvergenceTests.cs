using NUnit.Framework;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SelectionApiConvergenceTests
{
    [Test]
    public void SelectionApiConvergence_Stepper_SelectedAliasesAndEvents_RemainInSync()
    {
        var control = new Stepper();
        control.SetSteps(
        [
            new StepperStep("intro", "Intro"),
            new StepperStep("config", "Config"),
            new StepperStep("done", "Done"),
        ]);

        StepperCurrentStepChangedEventArgs? selectionChangedArgs = null;
        StepperCurrentStepChangedEventArgs? currentStepChangedArgs = null;
        control.SelectionChanged += (_, args) => selectionChangedArgs = args;
        control.CurrentStepChanged += (_, args) => currentStepChangedArgs = args;

        Assert.That(control.SetCurrentStep(2), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(control.CurrentIndex));
        Assert.That(control.SelectedStep, Is.SameAs(control.CurrentStep));

        Assert.That(selectionChangedArgs, Is.Not.Null);
        Assert.That(currentStepChangedArgs, Is.Not.Null);
        Assert.That(selectionChangedArgs!.SelectedIndex, Is.EqualTo(selectionChangedArgs.CurrentIndex));
        Assert.That(selectionChangedArgs.SelectedStep, Is.SameAs(selectionChangedArgs.CurrentStep));
        Assert.That(currentStepChangedArgs!.SelectedIndex, Is.EqualTo(currentStepChangedArgs.CurrentIndex));
        Assert.That(currentStepChangedArgs.SelectedStep, Is.SameAs(currentStepChangedArgs.CurrentStep));
    }

    [Test]
    public void SelectionApiConvergence_Wizard_SelectedAliasesAndEvents_RemainInSync()
    {
        var control = new Wizard();
        control.SetSteps(
        [
            new WizardStep("intro", "Intro"),
            new WizardStep("config", "Config"),
            new WizardStep("done", "Done"),
        ]);

        WizardStepChangedEventArgs? selectionChangedArgs = null;
        WizardStepChangedEventArgs? stepChangedArgs = null;
        control.SelectionChanged += (_, args) => selectionChangedArgs = args;
        control.StepChanged += (_, args) => stepChangedArgs = args;

        Assert.That(control.SelectStep(2), Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(control.CurrentIndex));
        Assert.That(control.SelectedStep, Is.SameAs(control.CurrentStep));

        Assert.That(selectionChangedArgs, Is.Not.Null);
        Assert.That(stepChangedArgs, Is.Not.Null);
        Assert.That(selectionChangedArgs!.SelectedIndex, Is.EqualTo(selectionChangedArgs.CurrentIndex));
        Assert.That(selectionChangedArgs.SelectedStep, Is.SameAs(selectionChangedArgs.CurrentStep));
        Assert.That(stepChangedArgs!.SelectedIndex, Is.EqualTo(stepChangedArgs.CurrentIndex));
        Assert.That(stepChangedArgs.SelectedStep, Is.SameAs(stepChangedArgs.CurrentStep));
    }
}
