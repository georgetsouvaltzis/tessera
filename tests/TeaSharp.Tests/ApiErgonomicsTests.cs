using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;

namespace TeaSharp.Tests;

internal static class ApiErgonomicsTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("ApiErgonomics_Thickness_UsesStandardSpacingVocabulary", Thickness_UsesStandardSpacingVocabulary);
        yield return new TestCase("ApiErgonomics_ScreenFrameLayout_ReducesScreenRectBookkeeping", ScreenFrameLayout_ReducesScreenRectBookkeeping);
        yield return new TestCase("ApiErgonomics_MasterDetailScreen_ReducesShellBookkeeping", MasterDetailScreen_ReducesShellBookkeeping);
        yield return new TestCase("ApiErgonomics_DashboardScreen_ReducesShellBookkeeping", DashboardScreen_ReducesShellBookkeeping);
        yield return new TestCase("ApiErgonomics_FormScreen_ReducesShellBookkeeping", FormScreen_ReducesShellBookkeeping);
        yield return new TestCase("ApiErgonomics_DialogWorkflow_ReducesOpenCloseBoilerplate", DialogWorkflow_ReducesOpenCloseBoilerplate);
        yield return new TestCase("ApiErgonomics_TextInputOptions_ConfigureComponentWithoutNestedInputAccess", TextInputOptions_ConfigureComponentWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_TextAreaOptions_ConfigureComponentWithoutNestedInputAccess", TextAreaOptions_ConfigureComponentWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_ListComponent_ExposesSelectionWithoutModelAccess", ListComponent_ExposesSelectionWithoutModelAccess);
        yield return new TestCase("ApiErgonomics_DropdownOptions_ConfigureComponentWithoutPostConstructionMutation", DropdownOptions_ConfigureComponentWithoutPostConstructionMutation);
        yield return new TestCase("ApiErgonomics_ComboboxOptions_ConfigureComponentWithoutPostConstructionMutation", ComboboxOptions_ConfigureComponentWithoutPostConstructionMutation);
        yield return new TestCase("ApiErgonomics_ComboboxComponent_ExposesFilterWithoutNestedInputAccess", ComboboxComponent_ExposesFilterWithoutNestedInputAccess);
        yield return new TestCase("ApiErgonomics_TableOptions_ExposePageSizeWithoutInnerAccess", TableOptions_ExposePageSizeWithoutInnerAccess);
        yield return new TestCase("ApiErgonomics_TableComponent_ExposesSortStateWithoutInnerAccess", TableComponent_ExposesSortStateWithoutInnerAccess);
        yield return new TestCase("ApiErgonomics_InteractionProfiles_AreClonedOnAssignment", InteractionProfiles_AreClonedOnAssignment);
        yield return new TestCase("ApiErgonomics_ActionEvents_EnableEventDrivenIntegration", ActionEvents_EnableEventDrivenIntegration);
        yield return new TestCase("ApiErgonomics_ConsumeMethods_ExposeOneShotInteractionResults", ConsumeMethods_ExposeOneShotInteractionResults);
        yield return new TestCase("ApiErgonomics_PrebuiltCatalog_CreatesConfiguredTextInput", PrebuiltCatalog_CreatesConfiguredTextInput);
        yield return new TestCase("ApiErgonomics_ProductivityCatalog_CreatesConfiguredMenuBar", ProductivityCatalog_CreatesConfiguredMenuBar);
        yield return new TestCase("ApiErgonomics_DialogOptions_ConfigureFrameWithoutLegacyBorderStyleName", DialogOptions_ConfigureFrameWithoutLegacyBorderStyleName);
        yield return new TestCase("ApiErgonomics_UiKitCatalog_CreatesConfiguredModal", UiKitCatalog_CreatesConfiguredModal);
        yield return new TestCase("ApiErgonomics_ModalOptions_ConfigureFrameWithoutLegacyBorderStyleName", ModalOptions_ConfigureFrameWithoutLegacyBorderStyleName);
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

    private static Task MasterDetailScreen_ReducesShellBookkeeping()
    {
        var screen = new ScreenComposer();
        var scaffold = screen.MasterDetail(new Rect(0, 0, 100, 30), masterWidth: 28, headerHeight: 1, footerHeight: 2);
        var master = new ButtonComponent();
        var detail = new ButtonComponent();

        screen.BeginFrame();
        scaffold.AddMaster("master", master);
        scaffold.AddDetail("detail", detail);
        screen.CompleteFrame();

        var focusChain = scaffold.CreateFocusChain();
        var changed = screen.FocusFirst(focusChain);

        TestAssert.Equal(new Rect(0, 1, 28, 27), scaffold.Master, "Master-detail scaffold should expose master bounds directly.");
        TestAssert.Equal(new Rect(28, 1, 72, 27), scaffold.Detail, "Master-detail scaffold should expose detail bounds directly.");
        TestAssert.True(changed, "Master-detail scaffold should build a reusable focus chain from added regions.");
        TestAssert.True(screen.FocusedRegionKey == new ScreenRegionKey("master"), "Scaffold focus chain should respect helper-add order.");
        return Task.CompletedTask;
    }

    private static Task DashboardScreen_ReducesShellBookkeeping()
    {
        var screen = new ScreenComposer();
        var scaffold = screen.Dashboard(new Rect(0, 0, 100, 30), sidebarWidth: 20, headerHeight: 1, footerHeight: 2);
        var sidebar = new ButtonComponent();
        var main = new ButtonComponent();

        screen.BeginFrame();
        scaffold.AddSidebar("sidebar", sidebar);
        scaffold.AddMain("main", main);
        screen.CompleteFrame();

        var focusChain = scaffold.CreateFocusChain();
        var changed = screen.FocusFirst(focusChain);

        TestAssert.Equal(new Rect(0, 1, 20, 27), scaffold.Sidebar, "Dashboard scaffold should expose sidebar bounds directly.");
        TestAssert.Equal(new Rect(20, 1, 80, 27), scaffold.Main, "Dashboard scaffold should expose main bounds directly.");
        TestAssert.True(changed, "Dashboard scaffold should build a reusable focus chain from added regions.");
        TestAssert.True(screen.FocusedRegionKey == new ScreenRegionKey("sidebar"), "Scaffold focus chain should respect helper-add order.");
        return Task.CompletedTask;
    }

    private static Task FormScreen_ReducesShellBookkeeping()
    {
        var screen = new ScreenComposer();
        var scaffold = screen.Form(new Rect(0, 0, 100, 30), actionsHeight: 2, headerHeight: 1, footerHeight: 2);
        var body = new ButtonComponent();
        var actions = new ButtonComponent();

        screen.BeginFrame();
        scaffold.AddBody("body", body);
        scaffold.AddActions("actions", actions);
        screen.CompleteFrame();

        var focusChain = scaffold.CreateFocusChain();
        var changed = screen.FocusFirst(focusChain);

        TestAssert.Equal(new Rect(0, 1, 100, 25), scaffold.Body, "Form scaffold should expose body bounds directly.");
        TestAssert.Equal(new Rect(0, 26, 100, 2), scaffold.Actions, "Form scaffold should expose action bounds directly.");
        TestAssert.True(changed, "Form scaffold should build a reusable focus chain from added regions.");
        TestAssert.True(screen.FocusedRegionKey == new ScreenRegionKey("body"), "Scaffold focus chain should respect helper-add order.");
        return Task.CompletedTask;
    }

    private static Task DialogWorkflow_ReducesOpenCloseBoilerplate()
    {
        var screen = new ScreenComposer();
        var dialog = new DialogComponent(new DialogOptions(Title: "Confirm"));
        var editor = new ButtonComponent();
        var editorKey = new ScreenRegionKey("editor");
        var dialogKey = new ScreenRegionKey("dialog");
        var workflow = screen.CreateDialogWorkflow(dialog, dialogKey, new ScreenFocusChain([editorKey]));

        screen.BeginFrame();
        screen.AddComponent(editorKey, new Rect(0, 0, 20, 6), editor);
        screen.CompleteFrame(editorKey);

        workflow.Show("Confirm delete", ["Delete item?"]);

        screen.BeginFrame();
        screen.AddComponent(editorKey, new Rect(0, 0, 20, 6), editor);
        workflow.Compose(new Rect(0, 0, 20, 6));
        screen.CompleteFrame();

        TestAssert.True(screen.FocusedRegionKey == dialogKey, "Dialog workflow should move focus to the dialog without manual focus bookkeeping.");

        var restored = workflow.Hide();

        TestAssert.True(restored, "Dialog workflow should restore prior focus when hidden programmatically.");
        TestAssert.True(screen.FocusedRegionKey == editorKey, "Dialog workflow should return focus to the previously active region.");
        return Task.CompletedTask;
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

    private static Task DropdownOptions_ConfigureComponentWithoutPostConstructionMutation()
    {
        var options = new DropdownOptions(
            Items: ["Development", "Production"],
            Title: "Environment",
            Focused: true,
            Border: BorderStyle.None,
            MaxVisibleItems: 4,
            InteractionProfile: WidgetInteractionProfile.KeyboardOnly);
        var dropdown = new DropdownComponent(options);

        TestAssert.Equal("Environment", dropdown.Title, "Dropdown options should set title.");
        TestAssert.True(dropdown.Focused, "Dropdown options should set focus.");
        TestAssert.True(dropdown.Border == BorderStyle.None, "Dropdown options should set border style.");
        TestAssert.Equal(4, dropdown.MaxVisibleItems, "Dropdown options should set max visible items.");
        TestAssert.Equal("Development", dropdown.SelectedItem, "Dropdown options should preload items.");
        TestAssert.True(!ReferenceEquals(options.InteractionProfile, dropdown.InteractionProfile), "Dropdown should clone interaction profile instead of sharing mutable state.");
        return Task.CompletedTask;
    }

    private static Task ComboboxOptions_ConfigureComponentWithoutPostConstructionMutation()
    {
        var options = new ComboboxOptions(
            Items: ["alpha", "beta"],
            Title: "Region",
            Placeholder: "type here",
            InitialFilter: "be",
            Focused: true,
            MaxVisibleItems: 5,
            InteractionProfile: WidgetInteractionProfile.KeyboardOnly);
        var combobox = new ComboboxComponent(options);

        TestAssert.Equal("Region", combobox.Title, "Combobox options should set title.");
        TestAssert.Equal("type here", combobox.Placeholder, "Combobox options should set placeholder.");
        TestAssert.Equal("be", combobox.FilterText, "Combobox options should set initial filter text.");
        TestAssert.True(combobox.Focused, "Combobox options should set focus.");
        TestAssert.Equal(5, combobox.MaxVisibleItems, "Combobox options should set max visible items.");
        TestAssert.True(!ReferenceEquals(options.InteractionProfile, combobox.InteractionProfile), "Combobox should clone interaction profile instead of sharing mutable state.");
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

    private static Task InteractionProfiles_AreClonedOnAssignment()
    {
        var shared = WidgetInteractionProfile.KeyboardOnly;
        var button = new ButtonComponent
        {
            InteractionProfile = shared,
        };
        var tabs = new TabsComponent(["Overview", "Logs"])
        {
            InteractionProfile = shared,
        };

        button.InteractionProfile.NavigateOnWheel = true;

        TestAssert.True(!shared.NavigateOnWheel, "Shared profile instances should not be mutated through component assignment.");
        TestAssert.True(!tabs.InteractionProfile.NavigateOnWheel, "Components should not share the same interaction profile instance.");
        TestAssert.True(button.InteractionProfile.NavigateOnWheel, "Component-local profile mutation should still work after cloning.");
        return Task.CompletedTask;
    }

    private static Task ConsumeMethods_ExposeOneShotInteractionResults()
    {
        var button = new ButtonComponent
        {
            Focused = true,
        };
        button.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Enter));

        var input = new TextInputComponent
        {
            Focused = true,
        };
        input.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Character, "x"));
        input.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Enter));

        TestAssert.True(button.TryConsumePress(), "Button should expose one-shot press consumption instead of requiring poll-style flags.");
        TestAssert.True(input.TryConsumeSubmit(out var submitted), "Text input should expose one-shot submit consumption.");
        TestAssert.Equal("x", submitted, "Consumed submit should preserve submitted text.");
        return Task.CompletedTask;
    }

    private static Task ActionEvents_EnableEventDrivenIntegration()
    {
        var button = new ButtonComponent
        {
            Focused = true,
        };
        var input = new TextInputComponent
        {
            Focused = true,
        };
        var buttonPressed = 0;
        string? submitted = null;
        button.Pressed += (_, _) => buttonPressed++;
        input.Submitted += (_, args) => submitted = args.Value;

        button.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Enter));
        input.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Character, "x"));
        input.Update(new TeaSharp.Core.Messages.KeyPressMsg(TeaSharp.Core.Messages.KeyCode.Enter));

        TestAssert.Equal(1, buttonPressed, "Button should expose an event-driven activation hook.");
        TestAssert.Equal("x", submitted ?? string.Empty, "Text input should expose submitted text through an event payload.");
        return Task.CompletedTask;
    }

    private static Task PrebuiltCatalog_CreatesConfiguredTextInput()
    {
        var input = TeaSharp.Components.Prebuilt.PrebuiltCatalog.TextInput(new TextInputOptions(
            Title: "Search",
            InitialValue: "tea"));

        TestAssert.Equal("Search", input.Title, "Prebuilt catalog should create configured text input instances.");
        TestAssert.Equal("tea", input.Value, "Prebuilt catalog should pass options through to the component.");
        return Task.CompletedTask;
    }

    private static Task ProductivityCatalog_CreatesConfiguredMenuBar()
    {
        var menu = TeaSharp.Components.Productivity.ProductivityCatalog.MenuBar(new MenuBarOptions(
            Items:
            [
                new MenuBarItem("file", "File", 'f'),
                new MenuBarItem("help", "Help", 'h'),
            ],
            Focused: true));

        TestAssert.True(menu.Focused, "Productivity catalog should create configured menu surfaces.");
        TestAssert.Equal(2, menu.Items.Count, "Productivity catalog should pass items through to the component.");
        return Task.CompletedTask;
    }

    private static Task DialogOptions_ConfigureFrameWithoutLegacyBorderStyleName()
    {
        var dialog = new DialogComponent(new DialogOptions(
            Title: "Confirm delete",
            Visible: true,
            Border: BorderStyle.Heavy,
            Padding: Thickness.All(1),
            Lines: ["Delete item?"]));

        TestAssert.True(dialog.Visible, "Dialog options should set visibility.");
        TestAssert.True(dialog.Border == BorderStyle.Heavy, "Dialog options should set border style through Border.");
        TestAssert.Equal(1, dialog.Padding.Left, "Dialog options should set padding through Thickness.");
        TestAssert.Equal("Delete item?", dialog.Lines[0], "Dialog options should preserve content lines.");
        return Task.CompletedTask;
    }

    private static Task UiKitCatalog_CreatesConfiguredModal()
    {
        var modal = TeaSharp.Components.UiKit.UiKitCatalog.Modal(new ModalOptions(
            Title: "Confirm",
            Visible: true,
            Lines: ["ready"]));

        TestAssert.True(modal.Visible, "UI-kit catalog should create configured modal surfaces.");
        TestAssert.Equal("Confirm", modal.Title, "UI-kit catalog should pass options through to the component.");
        TestAssert.Equal(1, modal.Lines.Count, "UI-kit catalog should preserve configured modal content.");
        return Task.CompletedTask;
    }

    private static Task ModalOptions_ConfigureFrameWithoutLegacyBorderStyleName()
    {
        var modal = new ModalComponent(new ModalOptions(
            Title: "Confirm",
            Visible: true,
            Border: BorderStyle.Ascii,
            Padding: Thickness.Symmetric(horizontal: 2, vertical: 1),
            Lines: ["ready"]));

        TestAssert.True(modal.Visible, "Modal options should set visibility.");
        TestAssert.True(modal.Border == BorderStyle.Ascii, "Modal options should set border style through Border.");
        TestAssert.Equal(2, modal.Padding.Left, "Modal options should set left padding.");
        TestAssert.Equal(1, modal.Padding.Top, "Modal options should set top padding.");
        return Task.CompletedTask;
    }
}
