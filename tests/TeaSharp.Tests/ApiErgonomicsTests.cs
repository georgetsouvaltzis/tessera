using TeaSharp.Components;

namespace TeaSharp.Tests;

internal static class ApiErgonomicsTests
{
    private enum FocusTarget
    {
        Button = 0,
        Input = 1,
        Progress = 2,
    }

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("ApiErgonomics_TextInputOptions_ConfigureComponentWithoutNestedInputAccess", TextInputOptions_ConfigureComponentWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_TextAreaOptions_ConfigureComponentWithoutNestedInputAccess", TextAreaOptions_ConfigureComponentWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_TableOptions_ExposePageSizeWithoutInnerAccess", TableOptions_ExposePageSizeWithoutInnerAccess);
        yield return new TestCase("ApiErgonomics_FocusGroup_AppliesSingleFocusedComponent", FocusGroup_AppliesSingleFocusedComponent);
    }

    private static Task TextInputOptions_ConfigureComponentWithoutNestedInputAccess()
    {
        var input = new TextInputComponent(new TextInputOptions(
            Title: "Command",
            Placeholder: "type here",
            InitialValue: "deploy",
            MaxLength: 32,
            ClearOnSubmit: true,
            MaskInput: true,
            MaskCharacter: '#'));

        TestAssert.Equal("Command", input.Title, "Text input options should set title.");
        TestAssert.Equal("type here", input.Placeholder, "Text input options should set placeholder.");
        TestAssert.Equal("deploy", input.Value, "Text input options should set initial value.");
        TestAssert.Equal(32, input.MaxLength, "Text input options should set max length.");
        TestAssert.True(input.ClearOnSubmit, "Text input options should set clear on submit.");
        TestAssert.True(input.MaskInput, "Text input options should set masking.");
        TestAssert.Equal('#', input.MaskCharacter, "Text input options should set mask character.");
        return Task.CompletedTask;
    }

    private static Task TextAreaOptions_ConfigureComponentWithoutNestedInputAccess()
    {
        var area = new TextAreaComponent(new TextAreaOptions(
            Title: "Notes",
            InitialValue: "a\nb",
            ShowLineNumbers: true,
            Wrap: true));

        TestAssert.Equal("Notes", area.Title, "Text area options should set title.");
        TestAssert.Equal("a\nb", area.Value, "Text area options should set initial value.");
        TestAssert.True(area.ShowLineNumbers, "Text area options should set line number mode.");
        TestAssert.True(area.Wrap, "Text area options should set wrapping.");
        return Task.CompletedTask;
    }

    private static Task TableOptions_ExposePageSizeWithoutInnerAccess()
    {
        var table = new TableComponent(new TableOptions(
            ["Name", "Status"],
            Title: "Deployments",
            PageSize: 6));

        TestAssert.Equal("Deployments", table.Title, "Table options should set title.");
        TestAssert.Equal(6, table.PageSize, "Table options should set page size.");
        return Task.CompletedTask;
    }

    private static Task FocusGroup_AppliesSingleFocusedComponent()
    {
        var button = new ButtonComponent();
        var input = new TextInputComponent();
        var progress = new ProgressBarComponent();
        var group = new FocusGroup<FocusTarget>()
            .Register(FocusTarget.Button, button)
            .Register(FocusTarget.Input, input)
            .Register(FocusTarget.Progress, progress);

        var changed = group.Apply(FocusTarget.Input);

        TestAssert.True(changed, "Focus group should report focus changes.");
        TestAssert.True(!button.Focused, "Unselected component should not be focused.");
        TestAssert.True(input.Focused, "Selected component should be focused.");
        TestAssert.True(!progress.Focused, "Other components should be unfocused.");
        return Task.CompletedTask;
    }
}
