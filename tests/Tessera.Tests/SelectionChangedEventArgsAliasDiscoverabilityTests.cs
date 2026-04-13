using NUnit.Framework;
using System.ComponentModel;
using System.Reflection;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SelectionChangedEventArgsAliasDiscoverabilityTests
{
    [Test]
    public void SelectionChangedEventArgsAliasCurrentPropertiesAreEditorBrowsableAdvanced()
    {
        AssertEditorBrowsableAdvanced(typeof(JsonTreeSelectionChangedEventArgs), "CurrentIndex");
        AssertEditorBrowsableAdvanced(typeof(JsonTreeSelectionChangedEventArgs), "CurrentNode");

        AssertEditorBrowsableAdvanced(typeof(KeyValueListSelectionChangedEventArgs), "CurrentIndex");
        AssertEditorBrowsableAdvanced(typeof(KeyValueListSelectionChangedEventArgs), "CurrentItem");

        AssertEditorBrowsableAdvanced(typeof(PropertyGridSelectionChangedEventArgs), "CurrentIndex");
        AssertEditorBrowsableAdvanced(typeof(PropertyGridSelectionChangedEventArgs), "CurrentProperty");

        AssertEditorBrowsableAdvanced(typeof(ValidationSelectionChangedEventArgs), "CurrentIndex");
        AssertEditorBrowsableAdvanced(typeof(ValidationSelectionChangedEventArgs), "CurrentIssue");

        var groupedType = typeof(GroupedListSelectionChangedEventArgs<string, string>);
        AssertEditorBrowsableAdvanced(groupedType, "CurrentRowIndex");
        AssertEditorBrowsableAdvanced(groupedType, "CurrentGroupIndex");
        AssertEditorBrowsableAdvanced(groupedType, "CurrentItemIndex");
        AssertEditorBrowsableAdvanced(groupedType, "CurrentItem");

        AssertEditorBrowsableAdvanced(typeof(FileExplorerSelectionChangedEventArgs), "CurrentPath");
        AssertEditorBrowsableAdvanced(typeof(FileExplorerSelectionChangedEventArgs), "CurrentItem");

        AssertEditorBrowsableAdvanced(typeof(StepperCurrentStepChangedEventArgs), "CurrentIndex");
        AssertEditorBrowsableAdvanced(typeof(StepperCurrentStepChangedEventArgs), "CurrentStep");

        AssertEditorBrowsableAdvanced(typeof(WizardStepChangedEventArgs), "CurrentIndex");
        AssertEditorBrowsableAdvanced(typeof(WizardStepChangedEventArgs), "CurrentStep");
    }

    [Test]
    public void SelectionChangedEventArgsAliasSelectedPropertiesRemainPrimaryInIntelliSense()
    {
        AssertEditorBrowsableMissing(typeof(JsonTreeSelectionChangedEventArgs), "SelectedIndex");
        AssertEditorBrowsableMissing(typeof(JsonTreeSelectionChangedEventArgs), "SelectedNode");

        AssertEditorBrowsableMissing(typeof(KeyValueListSelectionChangedEventArgs), "SelectedIndex");
        AssertEditorBrowsableMissing(typeof(KeyValueListSelectionChangedEventArgs), "SelectedItem");

        AssertEditorBrowsableMissing(typeof(PropertyGridSelectionChangedEventArgs), "SelectedIndex");
        AssertEditorBrowsableMissing(typeof(PropertyGridSelectionChangedEventArgs), "SelectedProperty");

        AssertEditorBrowsableMissing(typeof(ValidationSelectionChangedEventArgs), "SelectedIndex");
        AssertEditorBrowsableMissing(typeof(ValidationSelectionChangedEventArgs), "SelectedIssue");

        var groupedType = typeof(GroupedListSelectionChangedEventArgs<string, string>);
        AssertEditorBrowsableMissing(groupedType, "SelectedRowIndex");
        AssertEditorBrowsableMissing(groupedType, "SelectedGroupIndex");
        AssertEditorBrowsableMissing(groupedType, "SelectedItemIndex");
        AssertEditorBrowsableMissing(groupedType, "SelectedItem");

        AssertEditorBrowsableMissing(typeof(FileExplorerSelectionChangedEventArgs), "SelectedPath");
        AssertEditorBrowsableMissing(typeof(FileExplorerSelectionChangedEventArgs), "SelectedItem");

        AssertEditorBrowsableMissing(typeof(StepperCurrentStepChangedEventArgs), "SelectedIndex");
        AssertEditorBrowsableMissing(typeof(StepperCurrentStepChangedEventArgs), "SelectedStep");

        AssertEditorBrowsableMissing(typeof(WizardStepChangedEventArgs), "SelectedIndex");
        AssertEditorBrowsableMissing(typeof(WizardStepChangedEventArgs), "SelectedStep");
    }

    private static void AssertEditorBrowsableAdvanced(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"{type.Name}.{propertyName} should exist.");

        var attribute = property!.GetCustomAttribute<EditorBrowsableAttribute>();
        Assert.That(attribute, Is.Not.Null, $"{type.Name}.{propertyName} should define EditorBrowsable.");
        Assert.That(attribute!.State, Is.EqualTo(EditorBrowsableState.Advanced),
            $"{type.Name}.{propertyName} should be Advanced.");
    }

    private static void AssertEditorBrowsableMissing(Type type, string propertyName)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
        Assert.That(property, Is.Not.Null, $"{type.Name}.{propertyName} should exist.");

        var attribute = property!.GetCustomAttribute<EditorBrowsableAttribute>();
        Assert.That(attribute, Is.Null,
            $"{type.Name}.{propertyName} should remain discoverable without EditorBrowsable.");
    }
}
