using NUnit.Framework;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SelectionChangedEventArgsAliasTests
{
    [Test]
    public void SelectionChangedEventArgsJsonTreeSelectedAliasesForwardToCurrentValues()
    {
        var previousNode = new JsonTreeNode("previous", "1", JsonTreeNodeKind.Value);
        var currentNode = new JsonTreeNode("current", "2", JsonTreeNodeKind.Value);
        var args = new JsonTreeSelectionChangedEventArgs(1, 2, previousNode, currentNode);

        Assert.That(args.SelectedIndex, Is.EqualTo(args.CurrentIndex));
        Assert.That(args.SelectedNode, Is.SameAs(args.CurrentNode));
    }

    [Test]
    public void SelectionChangedEventArgsKeyValueListSelectedAliasesForwardToCurrentValues()
    {
        var previousItem = new KeyValueListEntry("a", "1");
        var currentItem = new KeyValueListEntry("b", "2");
        var args = new KeyValueListSelectionChangedEventArgs(3, 4, previousItem, currentItem);

        Assert.That(args.SelectedIndex, Is.EqualTo(args.CurrentIndex));
        Assert.That(args.SelectedItem, Is.SameAs(args.CurrentItem));
    }

    [Test]
    public void SelectionChangedEventArgsPropertyGridSelectedAliasesForwardToCurrentValues()
    {
        var previousProperty = new PropertyGridProperty("Alpha", "1");
        var currentProperty = new PropertyGridProperty("Beta", "2");
        var args = new PropertyGridSelectionChangedEventArgs(5, 6, previousProperty, currentProperty);

        Assert.That(args.SelectedIndex, Is.EqualTo(args.CurrentIndex));
        Assert.That(args.SelectedProperty, Is.SameAs(args.CurrentProperty));
    }

    [Test]
    public void SelectionChangedEventArgsValidationSelectedAliasesForwardToCurrentValues()
    {
        var previousIssue = new ValidationIssue("previous", ValidationSeverity.Warning, "FieldA");
        var currentIssue = new ValidationIssue("current", ValidationSeverity.Error, "FieldB");
        var args = new ValidationSelectionChangedEventArgs(7, 8, previousIssue, currentIssue);

        Assert.That(args.SelectedIndex, Is.EqualTo(args.CurrentIndex));
        Assert.That(args.SelectedIssue, Is.EqualTo(args.CurrentIssue));
    }

    [Test]
    public void SelectionChangedEventArgsGroupedListSelectedAliasesForwardToCurrentValues()
    {
        var args = new GroupedListSelectionChangedEventArgs<string, string>(
            previousRowIndex: 1,
            currentRowIndex: 2,
            previousGroupIndex: 0,
            currentGroupIndex: 1,
            previousItemIndex: 2,
            currentItemIndex: 3,
            previousItem: "A",
            currentItem: "B");

        Assert.That(args.SelectedRowIndex, Is.EqualTo(args.CurrentRowIndex));
        Assert.That(args.SelectedGroupIndex, Is.EqualTo(args.CurrentGroupIndex));
        Assert.That(args.SelectedItemIndex, Is.EqualTo(args.CurrentItemIndex));
        Assert.That(args.SelectedItem, Is.EqualTo(args.CurrentItem));
    }

    [Test]
    public void SelectionChangedEventArgsFileExplorerSelectedAliasesForwardToCurrentValues()
    {
        var previousItem = new FileExplorerItem("old.txt", isDirectory: false, path: "/tmp/old.txt");
        var currentItem = new FileExplorerItem("new.txt", isDirectory: false, path: "/tmp/new.txt");
        var args = new FileExplorerSelectionChangedEventArgs(
            previousPath: previousItem.Path,
            currentPath: currentItem.Path,
            previousItem: previousItem,
            currentItem: currentItem);

        Assert.That(args.SelectedPath, Is.EqualTo(args.CurrentPath));
        Assert.That(args.SelectedItem, Is.SameAs(args.CurrentItem));
    }

    [Test]
    public void SelectionChangedEventArgsStepperSelectedAliasesForwardToCurrentValues()
    {
        var previousStep = new StepperStep("intro", "Intro");
        var currentStep = new StepperStep("done", "Done");
        var args = new StepperCurrentStepChangedEventArgs(0, 1, previousStep, currentStep);

        Assert.That(args.SelectedIndex, Is.EqualTo(args.CurrentIndex));
        Assert.That(args.SelectedStep, Is.SameAs(args.CurrentStep));
    }

    [Test]
    public void SelectionChangedEventArgsWizardSelectedAliasesForwardToCurrentValues()
    {
        var previousStep = new WizardStep("intro", "Intro");
        var currentStep = new WizardStep("done", "Done");
        var args = new WizardStepChangedEventArgs(0, 1, previousStep, currentStep);

        Assert.That(args.SelectedIndex, Is.EqualTo(args.CurrentIndex));
        Assert.That(args.SelectedStep, Is.SameAs(args.CurrentStep));
    }
}
