using TeaSharp.Components.Advanced;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

internal static class ApiErgonomicsTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("ApiErgonomics_Thickness_UsesStandardSpacingVocabulary", Thickness_UsesStandardSpacingVocabulary);
        yield return new TestCase("ApiErgonomics_ScreenFrameLayout_ReducesScreenRectBookkeeping", ScreenFrameLayout_ReducesScreenRectBookkeeping);
        yield return new TestCase("ApiErgonomics_RootTextInput_ConfiguresWithoutNestedInputAccess", RootTextInput_ConfiguresWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_RootTextArea_ConfiguresWithoutNestedInputAccess", RootTextArea_ConfiguresWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_RootListView_ExposesSelectionWithoutModelAccess", RootListView_ExposesSelectionWithoutModelAccess);
        yield return new TestCase("ApiErgonomics_RootChoice_ConfiguresWithoutPostConstructionMutation", RootChoice_ConfiguresWithoutPostConstructionMutation);
        yield return new TestCase("ApiErgonomics_RootComboBox_ExposesFilterWithoutNestedInputAccess", RootComboBox_ExposesFilterWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_Table_ExposePageSizeWithoutInnerAccess", Table_ExposePageSizeWithoutInnerAccess);
        yield return new TestCase("ApiErgonomics_Table_ExposesSortStateWithoutInnerAccess", Table_ExposesSortStateWithoutInnerAccess);
        yield return new TestCase("ApiErgonomics_ActionEvents_EnableEventDrivenIntegration", ActionEvents_EnableEventDrivenIntegration);
        yield return new TestCase("ApiErgonomics_ConsumeMethods_ExposeOneShotInteractionResults", ConsumeMethods_ExposeOneShotInteractionResults);
        yield return new TestCase("ApiErgonomics_RootTextInput_ConfiguresWithoutCatalog", RootTextInput_ConfiguresWithoutCatalog);
        yield return new TestCase("ApiErgonomics_RootMenuBar_ConfiguresWithoutCatalog", RootMenuBar_ConfiguresWithoutCatalog);
        yield return new TestCase("ApiErgonomics_RootDialog_ConfiguresFrameWithoutLegacyBorderStyleName", RootDialog_ConfiguresFrameWithoutLegacyBorderStyleName);
        yield return new TestCase("ApiErgonomics_RootModal_ConfiguresWithoutCatalog", RootModal_ConfiguresWithoutCatalog);
        yield return new TestCase("ApiErgonomics_RootModal_ConfiguresFrameWithoutLegacyBorderStyleName", RootModal_ConfiguresFrameWithoutLegacyBorderStyleName);
    }

    private static Task Thickness_UsesStandardSpacingVocabulary()
    {
        var spacing = Thickness.Symmetric(horizontal: 2, vertical: 1);

        TestAssert.Equal(2, spacing.Left, "Thickness should expose left spacing.");
        TestAssert.Equal(2, spacing.Right, "Thickness should expose right spacing.");
        TestAssert.Equal(1, spacing.Top, "Thickness should expose top spacing.");
        TestAssert.Equal(1, spacing.Bottom, "Thickness should expose bottom spacing.");
        TestAssert.Equal(4, spacing.Horizontal, "Thickness should expose aggregate horizontal spacing.");
        TestAssert.Equal(2, spacing.Vertical, "Thickness should expose aggregate vertical spacing.");
        return Task.CompletedTask;
    }

    private static Task ScreenFrameLayout_ReducesScreenRectBookkeeping()
    {
        var screen = new ScreenComposer();
        var frame = screen.Frame(new Rect(0, 0, 100, 30), headerHeight: 1, footerHeight: 2);
        var (left, right) = frame.SplitBodyColumns(28);

        TestAssert.Equal(new Rect(0, 0, 100, 1), frame.Header, "Screen frame should expose header bounds directly.");
        TestAssert.Equal(new Rect(0, 1, 100, 27), frame.Body, "Screen frame should expose body bounds directly.");
        TestAssert.Equal(new Rect(0, 28, 100, 2), frame.Footer, "Screen frame should expose footer bounds directly.");
        TestAssert.Equal(28, left.Width, "Screen frame body split should preserve requested left width.");
        TestAssert.Equal(72, right.Width, "Screen frame body split should preserve remaining width.");
        return Task.CompletedTask;
    }

    private static Task RootTextInput_ConfiguresWithoutNestedInputAccess()
    {
        var input = new TextInput
        {
            Title = "Command",
            Placeholder = "type here",
            MaxLength = 32,
            ClearOnSubmit = true,
            MaskInput = true,
            MaskCharacter = '#',
        };
        input.SetValue("deploy");

        TestAssert.Equal("Command", input.Title, "Text input options should set title.");
        TestAssert.Equal("type here", input.Placeholder, "Text input options should set placeholder.");
        TestAssert.Equal("deploy", input.Value, "Text input options should set initial value.");
        TestAssert.Equal(32, input.MaxLength, "Text input options should set max length.");
        TestAssert.True(input.ClearOnSubmit, "Text input options should set clear on submit.");
        TestAssert.True(input.MaskInput, "Text input options should set masking.");
        TestAssert.Equal('#', input.MaskCharacter, "Text input options should set mask character.");
        return Task.CompletedTask;
    }

    private static Task RootTextArea_ConfiguresWithoutNestedInputAccess()
    {
        var area = new TextArea
        {
            Title = "Notes",
            ShowLineNumbers = true,
            Wrap = true,
        };
        area.SetValue("a\nb");

        TestAssert.Equal("Notes", area.Title, "Text area options should set title.");
        TestAssert.Equal("a\nb", area.Value, "Text area options should set initial value.");
        TestAssert.True(area.ShowLineNumbers, "Text area options should set line number mode.");
        TestAssert.True(area.Wrap, "Text area options should set wrapping.");
        return Task.CompletedTask;
    }

    private static Task Table_ExposePageSizeWithoutInnerAccess()
    {
        var table = new Table("Name", "Status")
        {
            Title = "Deployments",
            PageSize = 6,
        };

        TestAssert.Equal("Deployments", table.Title, "Table options should set title.");
        TestAssert.Equal(6, table.PageSize, "Table options should set page size.");
        return Task.CompletedTask;
    }

    private static Task RootListView_ExposesSelectionWithoutModelAccess()
    {
        var list = new ListView<string>(item => item)
        {
            IsFocused = true,
        };
        list.SetItems(["one", "two", "three"]);

        list.Handle(new KeyPressed(Key.Down));

        TestAssert.Equal("two", list.SelectedItem ?? string.Empty, "ListView should expose selection at root level.");
        TestAssert.Equal(1, list.SelectedIndex, "ListView should expose selected index at root level.");
        return Task.CompletedTask;
    }

    private static Task RootChoice_ConfiguresWithoutPostConstructionMutation()
    {
        var choice = new Choice
        {
            Title = "Environment",
            IsFocused = true,
            Border = BorderStyle.None,
            MaxVisibleItems = 4,
        };
        choice.SetItems(["Development", "Production"]);

        TestAssert.Equal("Environment", choice.Title, "Choice configuration should set title.");
        TestAssert.True(choice.IsFocused, "Choice configuration should set focus.");
        TestAssert.True(choice.Border == BorderStyle.None, "Choice configuration should set border style.");
        TestAssert.Equal(4, choice.MaxVisibleItems, "Choice configuration should set max visible items.");
        TestAssert.Equal("Development", choice.SelectedItem, "Choice should preload items through root configuration.");
        return Task.CompletedTask;
    }

    private static Task RootComboBox_ExposesFilterWithoutNestedInputAccess()
    {
        var combobox = new ComboBox
        {
            Placeholder = "type here",
        };

        combobox.SetItems(["alpha", "beta"]);
        combobox.SetFilterText("be");

        TestAssert.Equal("type here", combobox.Placeholder, "ComboBox should expose placeholder at root level.");
        TestAssert.Equal("be", combobox.FilterText, "ComboBox should expose filter text at root level.");
        return Task.CompletedTask;
    }

    private static Task Table_ExposesSortStateWithoutInnerAccess()
    {
        var table = new Table("Name", "Status")
        {
            IsFocused = true,
        };
        table.SetRows(
        [
            ["api", "ok"],
            ["worker", "warn"],
        ]);

        table.Handle(new KeyPressed(Key.Character, "c"));
        table.Handle(new KeyPressed(Key.Character, "s"));

        TestAssert.Equal(1, table.SortColumn, "Table should expose sort column at component level.");
        TestAssert.True(table.SortDescending, "Table should expose sort direction at component level.");
        return Task.CompletedTask;
    }

    private static Task ConsumeMethods_ExposeOneShotInteractionResults()
    {
        var button = new Button
        {
            IsFocused = true,
        };
        button.Handle(new KeyPressed(Key.Enter));

        var input = new TextInput
        {
            IsFocused = true,
        };
        input.Handle(new KeyPressed(Key.Character, "x"));
        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(button.TryConsumeActivation(), "Button should expose one-shot activation consumption instead of requiring poll-style flags.");
        TestAssert.True(input.TryConsumeSubmission(out var submitted), "Text input should expose one-shot submit consumption.");
        TestAssert.Equal("x", submitted, "Consumed submit should preserve submitted text.");
        return Task.CompletedTask;
    }

    private static Task ActionEvents_EnableEventDrivenIntegration()
    {
        var button = new Button
        {
            IsFocused = true,
        };
        var input = new TextInput
        {
            IsFocused = true,
        };
        var buttonPressed = 0;
        string? submitted = null;
        button.Activated += (_, _) => buttonPressed++;
        input.Submitted += (_, args) => submitted = args.Value;

        button.Handle(new KeyPressed(Key.Enter));
        input.Handle(new KeyPressed(Key.Character, "x"));
        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal(1, buttonPressed, "Button should expose an event-driven activation hook.");
        TestAssert.Equal("x", submitted ?? string.Empty, "Text input should expose submitted text through an event payload.");
        return Task.CompletedTask;
    }

    private static Task RootTextInput_ConfiguresWithoutCatalog()
    {
        var input = new TextInput
        {
            Title = "Search",
        };
        input.SetValue("tea");

        TestAssert.Equal("Search", input.Title, "Root text input should configure directly without a category catalog.");
        TestAssert.Equal("tea", input.Value, "Root text input should expose state without catalog indirection.");
        return Task.CompletedTask;
    }

    private static Task RootMenuBar_ConfiguresWithoutCatalog()
    {
        var menu = new MenuBar
        {
            IsFocused = true,
        };
        menu.SetItems(
        [
            new MenuItem("file", "File", 'f'),
            new MenuItem("help", "Help", 'h'),
        ]);

        TestAssert.True(menu.IsFocused, "Root menu bar should configure directly without a category catalog.");
        TestAssert.Equal(2, menu.Items.Count, "Root menu bar should accept items directly.");
        return Task.CompletedTask;
    }

    private static Task RootDialog_ConfiguresFrameWithoutLegacyBorderStyleName()
    {
        var dialog = new Dialog
        {
            Title = "Confirm delete",
            IsVisible = true,
            Border = BorderStyle.Heavy,
            Padding = Thickness.All(1),
            BodyLines = ["Delete item?"],
        };

        TestAssert.True(dialog.IsVisible, "Dialog options should set visibility.");
        TestAssert.True(dialog.Border == BorderStyle.Heavy, "Dialog options should set border style through Border.");
        TestAssert.Equal(1, dialog.Padding.Left, "Dialog options should set padding through Thickness.");
        TestAssert.Equal("Delete item?", dialog.BodyLines[0], "Dialog options should preserve content lines.");
        return Task.CompletedTask;
    }

    private static Task RootModal_ConfiguresWithoutCatalog()
    {
        var modal = new Modal
        {
            Title = "Confirm",
            IsVisible = true,
        };
        modal.SetBodyLines(["ready"]);

        TestAssert.True(modal.IsVisible, "Root modal should configure directly without a category catalog.");
        TestAssert.Equal("Confirm", modal.Title, "Root modal should preserve configured title.");
        TestAssert.Equal(1, modal.BodyLines.Count, "Root modal should preserve configured body content.");
        return Task.CompletedTask;
    }

    private static Task RootModal_ConfiguresFrameWithoutLegacyBorderStyleName()
    {
        var modal = new Modal
        {
            Title = "Confirm",
            IsVisible = true,
            Border = BorderStyle.Ascii,
            Padding = Thickness.Symmetric(horizontal: 2, vertical: 1),
            BodyLines = ["ready"],
        };

        TestAssert.True(modal.IsVisible, "Modal options should set visibility.");
        TestAssert.True(modal.Border == BorderStyle.Ascii, "Modal options should set border style through Border.");
        TestAssert.Equal(2, modal.Padding.Left, "Modal options should set left padding.");
        TestAssert.Equal(1, modal.Padding.Top, "Modal options should set top padding.");
        return Task.CompletedTask;
    }
}
