using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls;
using System.Globalization;
using TeaSharp.Core.Messages;
namespace TeaSharp.Tests;

internal static class PrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Controls_Label_RendersText", Label_RendersText);
        yield return new TestCase("Controls_Button_ActivatesWhenFocused", Button_ActivatesWhenFocused);
        yield return new TestCase("Controls_Button_MouseClickActivatesAndTracksState", Button_MouseClickActivatesAndTracksState);
        yield return new TestCase("Controls_Button_ActivatedEvent_FiresOnActivation", Button_ActivatedEvent_FiresOnActivation);
        yield return new TestCase("Controls_Button_TryConsumeActivation_IsSingleUse", Button_TryConsumeActivation_IsSingleUse);
        yield return new TestCase("Controls_Button_RendersBorderedState", Button_RendersBorderedState);
        yield return new TestCase("Controls_TextInput_SubmitsValue", TextInput_SubmitsValue);
        yield return new TestCase("Controls_TextInput_Events_ReportSubmitAndCancelValues", TextInput_Events_ReportSubmitAndCancelValues);
        yield return new TestCase("Controls_TextInput_TryConsumeSubmissionAndCancellation_AreSingleUse", TextInput_TryConsumeSubmissionAndCancellation_AreSingleUse);
        yield return new TestCase("Controls_TextInput_CancelSignalsAndCanClear", TextInput_CancelSignalsAndCanClear);
        yield return new TestCase("Controls_TextInput_HidesBorderWhenConfigured", TextInput_HidesBorderWhenConfigured);
        yield return new TestCase("Controls_TextArea_RendersMultilineContent", TextArea_RendersMultilineContent);
        yield return new TestCase("Controls_TextArea_EnterInsertsNewline", TextArea_EnterInsertsNewline);
        yield return new TestCase("Controls_Tabs_CycleAndSelectByNumber", Tabs_CycleAndSelectByNumber);
        yield return new TestCase("Controls_Tabs_ZeroShortcut_SelectsTenthTab", Tabs_ZeroShortcut_SelectsTenthTab);
        yield return new TestCase("Controls_Tabs_MouseClickSelectsTab", Tabs_MouseClickSelectsTab);
        yield return new TestCase("Controls_Tabs_SelectionChangedEvent_ReportsTab", Tabs_SelectionChangedEvent_ReportsTab);
        yield return new TestCase("Controls_ListView_NavigatesSelection", ListView_NavigatesSelection);
        yield return new TestCase("Controls_ListView_SelectionChangedEvent_ReportsTransition", ListView_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_ListView_MouseClickSelectsRow", ListView_MouseClickSelectsRow);
        yield return new TestCase("Controls_ListView_MouseClickOutsideLabel_DoesNotSelectRow", ListView_MouseClickOutsideLabel_DoesNotSelectRow);
        yield return new TestCase("Controls_ListView_MouseMotionShowsHoverMarker", ListView_MouseMotionShowsHoverMarker);
        yield return new TestCase("Controls_Choice_SelectsOpenMenuItem", Choice_SelectsOpenMenuItem);
        yield return new TestCase("Controls_Choice_SelectionChangedEvent_ReportsSelection", Choice_SelectionChangedEvent_ReportsSelection);
        yield return new TestCase("Controls_Choice_HidesBorderWhenConfigured", Choice_HidesBorderWhenConfigured);
        yield return new TestCase("Controls_Choice_MouseClickOpensAndSelects", Choice_MouseClickOpensAndSelects);
        yield return new TestCase("Controls_ComboBox_FiltersAndSelects", ComboBox_FiltersAndSelects);
        yield return new TestCase("Controls_ComboBox_SelectionChangedEvent_ReportsSelection", ComboBox_SelectionChangedEvent_ReportsSelection);
        yield return new TestCase("Controls_ComboBox_MouseWheelNavigatesAndSelects", ComboBox_MouseWheelNavigatesAndSelects);
        yield return new TestCase("Controls_MenuBar_ActivatesShortcut", MenuBar_ActivatesShortcut);
        yield return new TestCase("Controls_MenuBar_ItemActivatedEvent_ReportsItem", MenuBar_ItemActivatedEvent_ReportsItem);
        yield return new TestCase("Controls_MenuBar_TryConsumeActivation_IsSingleUse", MenuBar_TryConsumeActivation_IsSingleUse);
        yield return new TestCase("Controls_MenuBar_MouseClickActivatesItem", MenuBar_MouseClickActivatesItem);
        yield return new TestCase("Controls_MenuBar_MouseMotionSelectsHoveredItem", MenuBar_MouseMotionSelectsHoveredItem);
        yield return new TestCase("Controls_ContextMenu_ExecutesAndCloses", ContextMenu_ExecutesAndCloses);
        yield return new TestCase("Controls_ContextMenu_ItemExecutedEvent_ReportsItem", ContextMenu_ItemExecutedEvent_ReportsItem);
        yield return new TestCase("Controls_ContextMenu_TryConsumeExecution_IsSingleUse", ContextMenu_TryConsumeExecution_IsSingleUse);
        yield return new TestCase("Controls_ContextMenu_MouseClickExecutesAndCloses", ContextMenu_MouseClickExecutesAndCloses);
        yield return new TestCase("Controls_ContextMenu_MouseReleaseExecutesAndCloses", ContextMenu_MouseReleaseExecutesAndCloses);
        yield return new TestCase("Controls_CommandPalette_FiltersAndExecutes", CommandPalette_FiltersAndExecutes);
        yield return new TestCase("Controls_CommandPalette_ItemExecutedEvent_ReportsItem", CommandPalette_ItemExecutedEvent_ReportsItem);
        yield return new TestCase("Controls_CommandPalette_TryConsumeExecution_IsSingleUse", CommandPalette_TryConsumeExecution_IsSingleUse);
        yield return new TestCase("Controls_CommandPalette_MouseClickExecutesSelection", CommandPalette_MouseClickExecutesSelection);
        yield return new TestCase("Controls_CommandPalette_ExposesQueryAccessors", CommandPalette_ExposesQueryAccessors);
        yield return new TestCase("Controls_CommandPalette_Open_ClearsQueryWhenClosed", CommandPalette_Open_ClearsQueryWhenClosed);
        yield return new TestCase("Controls_CommandPalette_LettersRemainQueryable", CommandPalette_LettersRemainQueryable);
        yield return new TestCase("Controls_Table_ForwardsSortHotkeys", Table_ForwardsSortHotkeys);
        yield return new TestCase("Controls_ProgressBar_AdjustsValue", ProgressBar_AdjustsValue);
        yield return new TestCase("Controls_StatusBar_RendersLeftAndRightText", StatusBar_RendersLeftAndRightText);
        yield return new TestCase("Controls_LogView_AppendsAndFilters", LogView_AppendsAndFilters);
        yield return new TestCase("Controls_Dialog_AcceptsAndDismisses", Dialog_AcceptsAndDismisses);
        yield return new TestCase("Controls_Dialog_Events_FirePerDecision", Dialog_Events_FirePerDecision);
        yield return new TestCase("Controls_Dialog_TryConsumeResult_IsSingleUse", Dialog_TryConsumeResult_IsSingleUse);
    }

    private static Task Label_RendersText()
    {
        var label = new Label
        {
            Title = "L",
            Text = "hello\nworld",
        };
        var canvas = new Canvas(20, 6);

        label.Render(canvas, new Rect(0, 0, 20, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("hello", StringComparison.Ordinal), "Label should render first line.");
        TestAssert.True(output.Contains("world", StringComparison.Ordinal), "Label should render second line.");
        return Task.CompletedTask;
    }

    private static Task Button_ActivatesWhenFocused()
    {
        var button = new Button
        {
            Text = "Go",
            IsFocused = true,
        };

        var changed = button.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(changed, "IsFocused button should handle enter.");
        TestAssert.Equal(1, button.ActivationCount, "Button activation count should increment.");
        return Task.CompletedTask;
    }

    private static Task Button_MouseClickActivatesAndTracksState()
    {
        var button = new Button
        {
            Text = "Deploy",
            Border = BorderStyle.SingleLine,
        };
        var bounds = new Rect(0, 0, 18, 5);

        var hoverChanged = button.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 4, 2), bounds);
        var clickChanged = button.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 4, 2), bounds);
        var releaseChanged = button.Handle(new PointerInput(PointerEventKind.Release, PointerButton.Left, 4, 2), bounds);

        TestAssert.True(hoverChanged, "Mouse motion inside button should update hover state.");
        TestAssert.True(clickChanged, "Mouse click should activate the button.");
        TestAssert.True(releaseChanged, "Mouse release should clear the pressed state.");
        TestAssert.True(!button.IsPressed, "Button should clear pressed state on release.");
        TestAssert.True(button.ActivationCount == 1, "Mouse click should increment activation count.");
        TestAssert.True(button.TryConsumeActivation(), "Button should surface a one-shot activation after the click.");
        TestAssert.True(!button.TryConsumeActivation(), "Activation consumption should remain single-use after the click.");
        return Task.CompletedTask;
    }

    private static Task Button_TryConsumeActivation_IsSingleUse()
    {
        var button = new Button
        {
            IsFocused = true,
        };

        button.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(button.TryConsumeActivation(), "Button should expose one-shot activation consumption.");
        TestAssert.True(!button.TryConsumeActivation(), "Button should not report the same activation twice.");
        return Task.CompletedTask;
    }

    private static Task Button_ActivatedEvent_FiresOnActivation()
    {
        var button = new Button
        {
            IsFocused = true,
        };
        var pressCount = 0;
        button.Activated += (_, _) => pressCount++;

        button.Handle(new KeyPressed(Key.Enter));
        button.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal(2, pressCount, "Button should raise the activated event for each activation.");
        return Task.CompletedTask;
    }

    private static Task Button_RendersBorderedState()
    {
        var button = new Button
        {
            Text = "Start",
            Description = "click or press enter",
            Border = BorderStyle.SingleLine,
        };
        var canvas = new Canvas(24, 5);

        button.Render(canvas, new Rect(0, 0, 24, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains("[Start]", StringComparison.Ordinal), "Bordered button should render its label.");
        TestAssert.True(output.Contains("click or press enter", StringComparison.Ordinal), "Bordered button should render its description.");
        return Task.CompletedTask;
    }

    private static Task TextInput_SubmitsValue()
    {
        var input = new TextInput
        {
            IsFocused = true,
            ClearOnSubmit = true,
        };

        input.Handle(new KeyPressed(Key.Character, "a"));
        input.Handle(new KeyPressed(Key.Character, "b"));
        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal("ab", input.LastSubmittedValue, "Text input should capture submitted value.");
        TestAssert.Equal(1, input.SubmitCount, "Text input should count submissions.");
        TestAssert.Equal(string.Empty, input.Value, "Text input should clear after submit when configured.");
        return Task.CompletedTask;
    }

    private static Task TextInput_HidesBorderWhenConfigured()
    {
        var input = new TextInput
        {
            Border = BorderStyle.None,
        };
        input.SetValue("plain");
        var canvas = new Canvas(20, 2);

        input.Render(canvas, new Rect(0, 0, 20, 2));
        var output = canvas.Render();

        TestAssert.True(output.Contains("plain", StringComparison.Ordinal), "Text input should render content in borderless mode.");
        TestAssert.True(!output.Contains('┌'), "Text input should not draw border when disabled.");
        return Task.CompletedTask;
    }

    private static Task TextInput_CancelSignalsAndCanClear()
    {
        var input = new TextInput
        {
            IsFocused = true,
            ClearOnCancel = true,
        };

        input.Handle(new KeyPressed(Key.Character, "a"));
        input.Handle(new KeyPressed(Key.Character, "b"));
        var changed = input.Handle(new KeyPressed(Key.Escape));

        TestAssert.True(changed, "Text input escape should signal a handled cancel action.");
        TestAssert.Equal("ab", input.LastCancelledValue, "Text input should capture cancelled value.");
        TestAssert.Equal(1, input.CancelCount, "Text input should count cancel actions.");
        TestAssert.Equal(string.Empty, input.Value, "Text input should clear value on cancel when configured.");
        return Task.CompletedTask;
    }

    private static Task TextInput_TryConsumeSubmissionAndCancellation_AreSingleUse()
    {
        var input = new TextInput
        {
            IsFocused = true,
        };

        input.Handle(new KeyPressed(Key.Character, "a"));
        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(input.TryConsumeSubmission(out var submitted), "Text input should expose one-shot submit consumption.");
        TestAssert.Equal("a", submitted, "Consumed submit should preserve submitted value.");
        TestAssert.True(!input.TryConsumeSubmission(out _), "Submit consumption should be single-use per submit.");

        input.Handle(new KeyPressed(Key.Character, "b"));
        input.Handle(new KeyPressed(Key.Escape));

        TestAssert.True(input.TryConsumeCancellation(out var cancelled), "Text input should expose one-shot cancel consumption.");
        TestAssert.Equal("ab", cancelled, "Consumed cancel should preserve cancelled value.");
        TestAssert.True(!input.TryConsumeCancellation(out _), "Cancel consumption should be single-use per cancel.");
        return Task.CompletedTask;
    }

    private static Task TextInput_Events_ReportSubmitAndCancelValues()
    {
        var input = new TextInput
        {
            IsFocused = true,
        };
        string? submitted = null;
        string? cancelled = null;
        input.Submitted += (_, args) => submitted = args.Value;
        input.Cancelled += (_, args) => cancelled = args.Value;

        input.Handle(new KeyPressed(Key.Character, "a"));
        input.Handle(new KeyPressed(Key.Enter));
        input.Handle(new KeyPressed(Key.Character, "b"));
        input.Handle(new KeyPressed(Key.Escape));

        TestAssert.Equal("a", submitted ?? string.Empty, "Text input submit event should expose the submitted value.");
        TestAssert.Equal("ab", cancelled ?? string.Empty, "Text input cancel event should expose the cancelled value.");
        return Task.CompletedTask;
    }

    private static Task TextArea_RendersMultilineContent()
    {
        var area = new TextArea
        {
            ShowLineNumbers = true,
        };
        area.SetValue("a\nb\nc");
        var canvas = new Canvas(24, 8);

        area.Render(canvas, new Rect(0, 0, 24, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains('1'), "Text area should render line numbers when enabled.");
        TestAssert.True(output.Contains('a'), "Text area should render text content.");
        return Task.CompletedTask;
    }

    private static Task TextArea_EnterInsertsNewline()
    {
        var area = new TextArea
        {
            IsFocused = true,
        };

        area.Handle(new KeyPressed(Key.Character, "l"));
        area.Handle(new KeyPressed(Key.Character, "i"));
        area.Handle(new KeyPressed(Key.Character, "n"));
        area.Handle(new KeyPressed(Key.Character, "e"));
        area.Handle(new KeyPressed(Key.Character, "A"));
        area.Handle(new KeyPressed(Key.Enter));
        area.Handle(new KeyPressed(Key.Character, "l"));
        area.Handle(new KeyPressed(Key.Character, "i"));
        area.Handle(new KeyPressed(Key.Character, "n"));
        area.Handle(new KeyPressed(Key.Character, "e"));
        area.Handle(new KeyPressed(Key.Character, "B"));

        TestAssert.True(area.Value.Contains('\n'), "Text area Enter should insert newline.");
        TestAssert.True(area.Value.StartsWith("lineA\nlineB", StringComparison.Ordinal), "Text area should keep content on separate lines.");
        return Task.CompletedTask;
    }

    private static Task Tabs_CycleAndSelectByNumber()
    {
        var tabs = new Tabs("Overview", "Data", "Forms")
        {
            IsFocused = true,
        };

        tabs.Handle(new KeyPressed(Key.Right));
        tabs.Handle(new KeyPressed(Key.Character, "3"));

        TestAssert.Equal(2, tabs.SelectedIndex, "Tabs should select requested one-based index from numeric key.");
        return Task.CompletedTask;
    }

    private static Task Tabs_MouseClickSelectsTab()
    {
        var tabs = new Tabs("Overview", "Data", "Forms");

        var changed = tabs.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 15, 0), new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Mouse click inside tab row should update selected tab.");
        TestAssert.Equal(1, tabs.SelectedIndex, "Tab click should select the clicked tab.");
        return Task.CompletedTask;
    }

    private static Task Tabs_ZeroShortcut_SelectsTenthTab()
    {
        var tabs = new Tabs("1", "2", "3", "4", "5", "6", "7", "8", "9", "10")
        {
            IsFocused = true,
        };

        tabs.Handle(new KeyPressed(Key.Character, "0"));

        TestAssert.Equal(9, tabs.SelectedIndex, "Zero shortcut should map to the tenth tab for parity with legacy tab strips.");
        return Task.CompletedTask;
    }

    private static Task Tabs_SelectionChangedEvent_ReportsTab()
    {
        var tabs = new Tabs("Overview", "Data", "Forms")
        {
            IsFocused = true,
        };
        SelectionChangedEventArgs? args = null;
        tabs.SelectionChanged += (_, eventArgs) => args = eventArgs;

        tabs.Handle(new KeyPressed(Key.Right));

        TestAssert.True(args is not null, "Tabs should raise selection changed when the selected tab changes.");
        TestAssert.Equal(0, args!.PreviousIndex, "Tabs event should expose the previous index.");
        TestAssert.Equal(1, args.SelectedIndex, "Tabs event should expose the selected index.");
        TestAssert.Equal("Overview", args.PreviousItem, "Tabs event should expose the previous tab label.");
        TestAssert.Equal("Data", args.SelectedItem, "Tabs event should expose the selected tab label.");
        return Task.CompletedTask;
    }

    private static Task ListView_NavigatesSelection()
    {
        var list = new ListView<string>(x => x)
        {
            IsFocused = true,
        };
        list.SetItems(["one", "two", "three"]);

        list.Handle(new KeyPressed(Key.Down));
        var selected = list.SelectedItem;

        TestAssert.Equal("two", selected ?? string.Empty, "List down key should advance selection.");
        return Task.CompletedTask;
    }

    private static Task ListView_SelectionChangedEvent_ReportsTransition()
    {
        var list = new ListView<string>(x => x)
        {
            IsFocused = true,
        };
        list.SetItems(["one", "two", "three"]);
        ListSelectionChangedEventArgs<string>? args = null;
        list.SelectionChanged += (_, eventArgs) => args = eventArgs;

        list.Handle(new KeyPressed(Key.Down));

        TestAssert.True(args is not null, "List should raise selection changed when the selected row changes.");
        TestAssert.Equal(0, args!.PreviousIndex, "List event should expose the previous index.");
        TestAssert.Equal(1, args.SelectedIndex, "List event should expose the selected index.");
        TestAssert.Equal("one", args.PreviousItem ?? string.Empty, "List event should expose the previous item.");
        TestAssert.Equal("two", args.SelectedItem ?? string.Empty, "List event should expose the selected item.");
        return Task.CompletedTask;
    }

    private static Task ListView_MouseClickSelectsRow()
    {
        var list = new ListView<string>(x => x)
        {
            Border = BorderStyle.None,
        };
        list.SetItems(["one", "two", "three"]);

        var changed = list.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 1), new Rect(0, 0, 20, 3));

        TestAssert.True(changed, "List mouse click should report selection changes.");
        TestAssert.Equal("two", list.SelectedItem ?? string.Empty, "List mouse click should select clicked row.");
        return Task.CompletedTask;
    }

    private static Task ListView_MouseClickOutsideLabel_DoesNotSelectRow()
    {
        var list = new ListView<string>(x => x)
        {
            Border = BorderStyle.None,
        };
        list.SetItems(["one", "two", "three"]);

        list.Handle(new KeyPressed(Key.Down));
        var changed = list.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 18, 1), new Rect(0, 0, 20, 3));

        TestAssert.True(!changed, "List mouse click in trailing whitespace should not report a selection change.");
        TestAssert.Equal("two", list.SelectedItem ?? string.Empty, "List mouse click in trailing whitespace should preserve the current selection.");
        return Task.CompletedTask;
    }

    private static Task ListView_MouseMotionShowsHoverMarker()
    {
        var list = new ListView<string>(x => x)
        {
            Border = BorderStyle.None,
        };
        list.SetItems(["one", "two", "three"]);
        var changed = list.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 0, 1), new Rect(0, 0, 20, 3));
        var canvas = new Canvas(20, 3);

        list.Render(canvas, new Rect(0, 0, 20, 3));
        var output = canvas.Render();

        TestAssert.True(changed, "Mouse motion inside list should update hover state.");
        TestAssert.True(output.Contains("▸ two", StringComparison.Ordinal), "Hovered row should render the hover marker.");
        return Task.CompletedTask;
    }

    private static Task Choice_SelectsOpenMenuItem()
    {
        var dropdown = new Choice
        {
            IsFocused = true,
            Title = "D",
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);

        dropdown.Handle(new KeyPressed(Key.Enter));
        dropdown.Handle(new KeyPressed(Key.Down));
        dropdown.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(!dropdown.IsOpen, "Dropdown should close after selecting an item.");
        TestAssert.Equal("beta", dropdown.SelectedItem, "Dropdown should select highlighted item.");
        return Task.CompletedTask;
    }

    private static Task Choice_SelectionChangedEvent_ReportsSelection()
    {
        var dropdown = new Choice
        {
            IsFocused = true,
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);
        SelectionChangedEventArgs? args = null;
        dropdown.SelectionChanged += (_, eventArgs) => args = eventArgs;

        dropdown.Handle(new KeyPressed(Key.Enter));
        dropdown.Handle(new KeyPressed(Key.Down));
        dropdown.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(args is not null, "Dropdown should raise selection changed when the selected item changes.");
        TestAssert.Equal("alpha", args!.PreviousItem, "Dropdown event should expose the previous item.");
        TestAssert.Equal("beta", args.SelectedItem, "Dropdown event should expose the selected item.");
        return Task.CompletedTask;
    }

    private static Task Choice_HidesBorderWhenConfigured()
    {
        var dropdown = new Choice
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);
        var canvas = new Canvas(24, 5);

        dropdown.Render(canvas, new Rect(0, 0, 24, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains("v alpha", StringComparison.Ordinal), "Dropdown should render selected item in borderless mode.");
        TestAssert.True(!output.Contains('┌'), "Dropdown should not draw border when disabled.");
        return Task.CompletedTask;
    }

    private static Task Choice_MouseClickOpensAndSelects()
    {
        var dropdown = new Choice
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);
        var bounds = new Rect(0, 0, 24, 6);

        var openChanged = dropdown.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0), bounds);
        var selectChanged = dropdown.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 2), bounds);

        TestAssert.True(openChanged, "Field click should open dropdown when click activation is enabled.");
        TestAssert.True(selectChanged, "Option click should select highlighted dropdown row.");
        TestAssert.True(!dropdown.IsOpen, "Dropdown should close after click-select.");
        TestAssert.Equal("beta", dropdown.SelectedItem, "Dropdown click-select should pick the clicked option.");
        return Task.CompletedTask;
    }

    private static Task ComboBox_FiltersAndSelects()
    {
        var combobox = new ComboBox
        {
            IsFocused = true,
            Title = "C",
        };
        combobox.SetItems(["alpha", "beta", "gamma"]);

        combobox.Handle(new KeyPressed(Key.Character, "g"));
        combobox.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(!combobox.IsOpen, "Combobox should close after selection.");
        TestAssert.Equal("gamma", combobox.SelectedItem, "Combobox should select the filtered match.");
        TestAssert.Equal("gamma", combobox.FilterText, "Combobox filter text should sync to selected item.");
        return Task.CompletedTask;
    }

    private static Task ComboBox_SelectionChangedEvent_ReportsSelection()
    {
        var combobox = new ComboBox
        {
            IsFocused = true,
        };
        combobox.SetItems(["alpha", "beta", "gamma"]);
        SelectionChangedEventArgs? args = null;
        combobox.SelectionChanged += (_, eventArgs) => args = eventArgs;

        combobox.Handle(new KeyPressed(Key.Character, "g"));
        combobox.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(args is not null, "Combobox should raise selection changed when the selected item changes.");
        TestAssert.Equal("gamma", args!.SelectedItem, "Combobox event should expose the selected item.");
        return Task.CompletedTask;
    }

    private static Task ComboBox_MouseWheelNavigatesAndSelects()
    {
        var combobox = new ComboBox
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        combobox.SetItems(["alpha", "beta", "gamma"]);
        var bounds = new Rect(0, 0, 24, 6);

        var openChanged = combobox.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0), bounds);
        var wheelChanged = combobox.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelDown, 0, 2), bounds);
        var selectChanged = combobox.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 3), bounds);

        TestAssert.True(openChanged, "Field click should open combobox list.");
        TestAssert.True(wheelChanged, "Wheel input should move combobox highlight when list is open.");
        TestAssert.True(selectChanged, "Option click should select highlighted combobox row.");
        TestAssert.True(!combobox.IsOpen, "Combobox should close after click-select.");
        TestAssert.Equal("gamma", combobox.SelectedItem, "Combobox selection should reflect wheel-adjusted highlighted option.");
        return Task.CompletedTask;
    }

    private static Task MenuBar_ActivatesShortcut()
    {
        var menu = new MenuBar
        {
            IsFocused = true,
        };
        menu.SetItems(
        [
            new MenuItem("file", "File", 'f'),
            new MenuItem("edit", "Edit", 'e'),
            new MenuItem("help", "Help", 'h'),
        ]);

        menu.Handle(new KeyPressed(Key.Character, "e"));
        menu.Handle(new KeyPressed(Key.Character, "h"));
        menu.Handle(new KeyPressed(Key.Enter));
        menu.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal("help", menu.LastActivatedItemId ?? string.Empty, "Menu bar should prioritize shortcut activation over navigation aliases.");
        TestAssert.True(menu.TryConsumeActivation(out var itemId), "Menu bar should expose one-shot activation consumption.");
        TestAssert.Equal("help", itemId, "Menu bar should consume the activated item id.");
        return Task.CompletedTask;
    }

    private static Task MenuBar_MouseClickActivatesItem()
    {
        var menu = new MenuBar();
        menu.SetItems(
        [
            new MenuItem("file", "File", 'f'),
            new MenuItem("edit", "Edit", 'e'),
            new MenuItem("help", "Help", 'h'),
        ]);

        var changed = menu.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 12, 0), new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Menu mouse click should trigger selection and activation.");
        TestAssert.Equal("edit", menu.LastActivatedItemId ?? string.Empty, "Menu mouse click should activate clicked item.");
        return Task.CompletedTask;
    }

    private static Task MenuBar_MouseMotionSelectsHoveredItem()
    {
        var menu = new MenuBar();
        menu.SetItems(
        [
            new MenuItem("file", "File", 'f'),
            new MenuItem("edit", "Edit", 'e'),
            new MenuItem("help", "Help", 'h'),
        ]);

        var changed = menu.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 12, 0), new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Menu hover should move selection to the hovered item.");
        TestAssert.Equal(1, menu.SelectedIndex, "Menu hover should track the hovered item for parity with legacy menu bars.");
        return Task.CompletedTask;
    }

    private static Task MenuBar_TryConsumeActivation_IsSingleUse()
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

        menu.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(menu.TryConsumeActivation(out var itemId), "Menu bar should expose one-shot activation consumption.");
        TestAssert.Equal("file", itemId, "Menu bar should consume the activated item id.");
        TestAssert.True(!menu.TryConsumeActivation(out _), "Menu bar should not report the same activation twice.");
        return Task.CompletedTask;
    }

    private static Task MenuBar_ItemActivatedEvent_ReportsItem()
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
        string? activated = null;
        menu.ItemActivated += (_, args) => activated = args.ItemId;

        menu.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal("file", activated ?? string.Empty, "Menu bar activation event should expose the selected item id.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_ExecutesAndCloses()
    {
        var menu = new ContextMenu
        {
            IsFocused = true,
        };
        menu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"),
        ]);
        menu.OpenAt(4, 2);

        menu.Handle(new KeyPressed(Key.Down));
        menu.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal("paste", menu.LastExecutedItemId ?? string.Empty, "Context menu should execute selected action.");
        TestAssert.True(!menu.IsVisible, "Context menu should close after execute.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_MouseClickExecutesAndCloses()
    {
        var menu = new ContextMenu
        {
            Border = BorderStyle.None,
        };
        menu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"),
        ]);
        menu.OpenAt(0, 0);

        var changed = menu.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 1), new Rect(0, 0, 20, 6));

        TestAssert.True(changed, "Context menu click should execute row action.");
        TestAssert.Equal("paste", menu.LastExecutedItemId ?? string.Empty, "Context menu click should execute clicked item.");
        TestAssert.True(!menu.IsVisible, "Context menu should close after mouse execute.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_TryConsumeExecution_IsSingleUse()
    {
        var menu = new ContextMenu
        {
            IsFocused = true,
        };
        menu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"),
        ]);
        menu.OpenAt(4, 2);

        menu.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(menu.TryConsumeExecution(out var itemId), "Context menu should expose one-shot execution consumption.");
        TestAssert.Equal("copy", itemId, "Context menu should consume the executed item id.");
        TestAssert.True(!menu.TryConsumeExecution(out _), "Context menu should not report the same execution twice.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_ItemExecutedEvent_ReportsItem()
    {
        var menu = new ContextMenu
        {
            IsFocused = true,
        };
        menu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"),
        ]);
        string? executed = null;
        menu.ItemExecuted += (_, args) => executed = args.ItemId;
        menu.OpenAt(4, 2);

        menu.Handle(new KeyPressed(Key.Down));
        menu.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal("paste", executed ?? string.Empty, "Context menu execution event should expose the executed item id.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_MouseReleaseExecutesAndCloses()
    {
        var menu = new ContextMenu
        {
            Border = BorderStyle.None,
        };
        menu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"),
        ]);
        menu.OpenAt(0, 0);

        var changed = menu.Handle(new PointerInput(PointerEventKind.Release, PointerButton.None, 0, 1), new Rect(0, 0, 20, 6));

        TestAssert.True(changed, "Context menu mouse release should execute row action.");
        TestAssert.Equal("paste", menu.LastExecutedItemId ?? string.Empty, "Context menu release should execute hovered item.");
        TestAssert.True(!menu.IsVisible, "Context menu should close after mouse release execute.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_FiltersAndExecutes()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);

        palette.Handle(new KeyPressed(Key.Character, "p", ModifierKeys.Ctrl));
        palette.Handle(new KeyPressed(Key.Character, "r"));
        palette.Handle(new KeyPressed(Key.Character, "o"));
        palette.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal("rollback", palette.LastExecutedItemId ?? string.Empty, "Command palette should execute filtered item.");
        TestAssert.True(!palette.IsVisible, "Palette should close after execute.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_MouseClickExecutesSelection()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);
        palette.Open();

        var changed = palette.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 12, 5), new Rect(0, 0, 60, 20));

        TestAssert.True(changed, "Command palette click should execute selected command.");
        TestAssert.Equal("deploy", palette.LastExecutedItemId ?? string.Empty, "Palette click should execute clicked row.");
        TestAssert.True(!palette.IsVisible, "Palette should close after click execute.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_TryConsumeExecution_IsSingleUse()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);

        palette.Handle(new KeyPressed(Key.Character, "p", ModifierKeys.Ctrl));
        palette.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(palette.TryConsumeExecution(out var itemId), "Command palette should expose one-shot execution consumption.");
        TestAssert.Equal("deploy", itemId, "Command palette should consume the executed item id.");
        TestAssert.True(!palette.TryConsumeExecution(out _), "Command palette should not report the same execution twice.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_ItemExecutedEvent_ReportsItem()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        string? executed = null;
        palette.ItemExecuted += (_, args) => executed = args.ItemId;
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);

        palette.Handle(new KeyPressed(Key.Character, "p", ModifierKeys.Ctrl));
        palette.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal("deploy", executed ?? string.Empty, "Command palette execution event should expose the executed command id.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_ExposesQueryAccessors()
    {
        var palette = new CommandPalette();
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);

        palette.SetQueryText("roll");

        TestAssert.Equal("roll", palette.QueryText, "Command palette should expose the current query text without requiring a nested input model.");

        palette.ClearQuery();

        TestAssert.Equal(string.Empty, palette.QueryText, "Command palette should clear the query through the root API.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_Open_ClearsQueryWhenClosed()
    {
        var palette = new CommandPalette();
        palette.SetItems([new CommandPaletteItem("deploy", "Deploy")]);
        palette.SetQueryText("dep");

        palette.Open();

        TestAssert.Equal(string.Empty, palette.QueryText, "Opening the palette from a closed state should reset query text for a fresh interaction.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_LettersRemainQueryable()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("jobs", "Jobs"),
            new CommandPaletteItem("deploy", "Deploy"),
        ]);

        palette.Open();
        palette.Handle(new KeyPressed(Key.Character, "j"));

        TestAssert.Equal("j", palette.QueryText, "Letter keys should contribute to the query instead of being stolen for navigation.");
        return Task.CompletedTask;
    }

    private static Task Table_ForwardsSortHotkeys()
    {
        var table = new Table("A", "B")
        {
            IsFocused = true,
            Title = "T",
        };
        table.SetRows(
        [
            ["x", "2"],
            ["y", "1"],
        ]);

        table.Handle(new KeyPressed(Key.Character, "c"));
        table.Handle(new KeyPressed(Key.Character, "s"));
        TestAssert.Equal(1, table.SortColumn, "Table should change sort column from hotkey.");
        TestAssert.True(table.SortDescending, "Table should toggle sort direction from hotkey.");
        return Task.CompletedTask;
    }

    private static Task ProgressBar_AdjustsValue()
    {
        var progress = new ProgressBar
        {
            IsFocused = true,
            Step = 0.25,
        };

        progress.Handle(new KeyPressed(Key.Right));
        progress.Handle(new KeyPressed(Key.Right));
        progress.Handle(new KeyPressed(Key.Left));

        TestAssert.True(Math.Abs(progress.Value - 0.25) < 0.0001, "Progress should settle at 25% after two increments and one decrement.");
        return Task.CompletedTask;
    }

    private static Task StatusBar_RendersLeftAndRightText()
    {
        var status = new StatusBar
        {
            LeftText = "left",
            RightText = "right",
        };
        var canvas = new Canvas(24, 1);

        status.Render(canvas, new Rect(0, 0, 24, 1));
        var output = canvas.Render();

        TestAssert.True(output.StartsWith("left", StringComparison.Ordinal), "Status bar should render left text at start.");
        TestAssert.True(output.EndsWith("right", StringComparison.Ordinal), "Status bar should render right text at end.");
        return Task.CompletedTask;
    }

    private static Task LogView_AppendsAndFilters()
    {
        var logs = new LogView
        {
            IsFocused = true,
        };
        logs.Append("alpha");
        logs.Append("beta");
        logs.SetFilter("alp");
        var canvas = new Canvas(26, 8);

        logs.Render(canvas, new Rect(0, 0, 26, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("alpha", StringComparison.Ordinal), "Filtered log view should keep matching entries.");
        TestAssert.True(!output.Contains("beta", StringComparison.Ordinal), "Filtered log view should hide non-matching entries.");
        return Task.CompletedTask;
    }

    private static Task Dialog_AcceptsAndDismisses()
    {
        var dialog = new Dialog
        {
            IsVisible = true,
            IsFocused = true,
        };

        var accepted = dialog.Handle(new KeyPressed(Key.Enter));
        TestAssert.True(accepted, "Dialog should accept on enter.");
        TestAssert.True(dialog.LastResult == DialogResult.Accepted, "Dialog should record accepted result.");

        dialog.IsVisible = true;
        dialog.IsFocused = true;
        var dismissed = dialog.Handle(new KeyPressed(Key.Escape));
        TestAssert.True(dismissed, "Dialog should dismiss on escape.");
        TestAssert.True(dialog.LastResult == DialogResult.Dismissed, "Dialog should record dismissed result.");
        return Task.CompletedTask;
    }

    private static Task Dialog_TryConsumeResult_IsSingleUse()
    {
        var dialog = new Dialog
        {
            IsVisible = true,
            IsFocused = true,
        };

        dialog.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(dialog.TryConsumeResult(out var result), "Dialog should expose one-shot result consumption.");
        TestAssert.True(result == DialogResult.Accepted, "Dialog should consume accepted result.");
        TestAssert.True(!dialog.TryConsumeResult(out _), "Dialog result consumption should be single-use per decision.");
        return Task.CompletedTask;
    }

    private static Task Dialog_Events_FirePerDecision()
    {
        var dialog = new Dialog
        {
            IsVisible = true,
            IsFocused = true,
        };
        var accepted = 0;
        var dismissed = 0;
        dialog.Accepted += (_, _) => accepted++;
        dialog.Dismissed += (_, _) => dismissed++;

        dialog.Handle(new KeyPressed(Key.Enter));
        dialog.IsVisible = true;
        dialog.Handle(new KeyPressed(Key.Escape));

        TestAssert.Equal(1, accepted, "Dialog should raise accepted exactly once for an accept decision.");
        TestAssert.Equal(1, dismissed, "Dialog should raise dismissed exactly once for a dismiss decision.");
        return Task.CompletedTask;
    }

    private sealed class KeyProbeComponent : IStatefulComponent, IFocusableComponent
    {
        public bool IsFocused { get; set; }

        public int KeyEvents { get; private set; }

        public bool Update(TeaSharp.Core.Abstractions.IMessage message)
        {
            if (message is not KeyPressMsg)
            {
                return false;
            }

            KeyEvents++;
            return true;
        }

        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, KeyEvents.ToString(CultureInfo.InvariantCulture), rect.Width);
        }
    }

    private sealed class MouseProbeComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
    {
        public bool IsFocused { get; set; }

        public int MouseEvents { get; private set; }

        public bool Update(TeaSharp.Core.Abstractions.IMessage message) => false;

        public bool UpdateMouse(MouseMsg message, Rect bounds)
        {
            MouseEvents++;
            return true;
        }

        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, IsFocused ? "focused" : "idle", rect.Width);
        }
    }

    private static bool ContainsStrikethroughSgr(string value)
    {
        return value.Contains("\u001b[9m", StringComparison.Ordinal)
            || value.Contains("\u001b[2;9m", StringComparison.Ordinal)
            || value.Contains(";9m", StringComparison.Ordinal)
            || value.Contains(";9;", StringComparison.Ordinal)
            || value.Contains("[9;", StringComparison.Ordinal);
    }
}
