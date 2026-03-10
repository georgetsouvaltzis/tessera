using TeaSharp.Components;

namespace TeaSharp.Tests;

internal static class ApiErgonomicsTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("ApiErgonomics_TextInputOptions_ConfigureComponentWithoutNestedInputAccess", TextInputOptions_ConfigureComponentWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_TextAreaOptions_ConfigureComponentWithoutNestedInputAccess", TextAreaOptions_ConfigureComponentWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_ListComponent_ExposesSelectionWithoutModelAccess", ListComponent_ExposesSelectionWithoutModelAccess);
        yield return new TestCase("ApiErgonomics_ComboboxComponent_ExposesFilterWithoutNestedInputAccess", ComboboxComponent_ExposesFilterWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_TableOptions_ExposePageSizeWithoutInnerAccess", TableOptions_ExposePageSizeWithoutInnerAccess);
        yield return new TestCase("ApiErgonomics_TableComponent_ExposesSortStateWithoutInnerAccess", TableComponent_ExposesSortStateWithoutInnerAccess);
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

    private static Task ListComponent_ExposesSelectionWithoutModelAccess()
    {
        var list = new ListComponent<string>(["one", "two", "three"], item => item)
        {
            Focused = true,
        };

        list.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Down));

        TestAssert.Equal("two", list.SelectedItem ?? string.Empty, "List should expose selection at component level.");
        TestAssert.Equal(1, list.SelectedIndex, "List should expose selected index at component level.");
        return Task.CompletedTask;
    }

    private static Task ComboboxComponent_ExposesFilterWithoutNestedInputAccess()
    {
        var combobox = new ComboboxComponent
        {
            Placeholder = "type here",
        };

        combobox.SetItems(["alpha", "beta"]);
        combobox.SetFilterText("be");

        TestAssert.Equal("type here", combobox.Placeholder, "Combobox should expose placeholder at component level.");
        TestAssert.Equal("be", combobox.FilterText, "Combobox should expose filter text at component level.");
        return Task.CompletedTask;
    }

    private static Task TableComponent_ExposesSortStateWithoutInnerAccess()
    {
        var table = new TableComponent(["Name", "Status"])
        {
            Focused = true,
        };
        table.SetRows(
        [
            ["api", "ok"],
            ["worker", "warn"],
        ]);

        table.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Character, "c"));
        table.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Character, "s"));

        TestAssert.Equal(1, table.SortColumn, "Table should expose sort column at component level.");
        TestAssert.True(table.SortDescending, "Table should expose sort direction at component level.");
        return Task.CompletedTask;
    }
}
