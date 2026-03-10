using TeaSharp.Components;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class PrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Prebuilt_LabelComponent_RendersText", LabelComponent_RendersText);
        yield return new TestCase("Prebuilt_ButtonComponent_ActivatesWhenFocused", ButtonComponent_ActivatesWhenFocused);
        yield return new TestCase("Prebuilt_ButtonComponent_MouseClickActivatesAndTracksState", ButtonComponent_MouseClickActivatesAndTracksState);
        yield return new TestCase("Prebuilt_ButtonComponent_RendersBorderedState", ButtonComponent_RendersBorderedState);
        yield return new TestCase("Prebuilt_TextInputComponent_SubmitsValue", TextInputComponent_SubmitsValue);
        yield return new TestCase("Prebuilt_TextInputComponent_CancelSignalsAndCanClear", TextInputComponent_CancelSignalsAndCanClear);
        yield return new TestCase("Prebuilt_TextInputComponent_HidesBorderWhenConfigured", TextInputComponent_HidesBorderWhenConfigured);
        yield return new TestCase("Prebuilt_TextAreaComponent_RendersMultilineContent", TextAreaComponent_RendersMultilineContent);
        yield return new TestCase("Prebuilt_TextAreaComponent_EnterInsertsNewline", TextAreaComponent_EnterInsertsNewline);
        yield return new TestCase("Prebuilt_ListComponent_NavigatesSelection", ListComponent_NavigatesSelection);
        yield return new TestCase("Prebuilt_ListComponent_MouseClickSelectsRow", ListComponent_MouseClickSelectsRow);
        yield return new TestCase("Prebuilt_ListComponent_MouseMotionShowsHoverMarker", ListComponent_MouseMotionShowsHoverMarker);
        yield return new TestCase("Prebuilt_ListComponent_AppliesCustomItemStateStyles", ListComponent_AppliesCustomItemStateStyles);
        yield return new TestCase("Prebuilt_DropdownComponent_SelectsOpenMenuItem", DropdownComponent_SelectsOpenMenuItem);
        yield return new TestCase("Prebuilt_DropdownComponent_HidesBorderWhenConfigured", DropdownComponent_HidesBorderWhenConfigured);
        yield return new TestCase("Prebuilt_DropdownComponent_AppliesOptionStateStyles", DropdownComponent_AppliesOptionStateStyles);
        yield return new TestCase("Prebuilt_DropdownComponent_MouseClickOpensAndSelects", DropdownComponent_MouseClickOpensAndSelects);
        yield return new TestCase("Prebuilt_ComboboxComponent_FiltersAndSelects", ComboboxComponent_FiltersAndSelects);
        yield return new TestCase("Prebuilt_ComboboxComponent_MouseWheelNavigatesAndSelects", ComboboxComponent_MouseWheelNavigatesAndSelects);
        yield return new TestCase("Prebuilt_TableComponent_ForwardsSortHotkeys", TableComponent_ForwardsSortHotkeys);
        yield return new TestCase("Prebuilt_ProgressBarComponent_AdjustsValue", ProgressBarComponent_AdjustsValue);
        yield return new TestCase("Prebuilt_StatusBarComponent_RendersLeftAndRightText", StatusBarComponent_RendersLeftAndRightText);
        yield return new TestCase("Prebuilt_LogViewerComponent_AppendsAndFilters", LogViewerComponent_AppendsAndFilters);
        yield return new TestCase("Prebuilt_DialogComponent_AcceptsAndDismisses", DialogComponent_AcceptsAndDismisses);
        yield return new TestCase("Prebuilt_LayoutContainerComponent_RendersChildren", LayoutContainerComponent_RendersChildren);
        yield return new TestCase("Prebuilt_LayoutContainerComponent_KeyboardRoutesToFocusedChildOnly", LayoutContainerComponent_KeyboardRoutesToFocusedChildOnly);
        yield return new TestCase("Prebuilt_LayoutContainerComponent_MouseResizeAdjustsPrimarySize", LayoutContainerComponent_MouseResizeAdjustsPrimarySize);
        yield return new TestCase("Prebuilt_LayoutContainerComponent_MouseRoutesToTargetChild", LayoutContainerComponent_MouseRoutesToTargetChild);
    }

    private static Task LabelComponent_RendersText()
    {
        var label = new LabelComponent
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

    private static Task ButtonComponent_ActivatesWhenFocused()
    {
        var button = new ButtonComponent
        {
            Label = "Go",
            Focused = true,
        };

        var changed = button.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(changed, "Focused button should handle enter.");
        TestAssert.Equal(1, button.PressCount, "Button press count should increment.");
        return Task.CompletedTask;
    }

    private static Task ButtonComponent_MouseClickActivatesAndTracksState()
    {
        var button = new ButtonComponent(new ButtonOptions(
            Label: "Deploy",
            ShowBorder: true));
        var bounds = new Rect(0, 0, 18, 5);

        var hoverChanged = button.UpdateMouse(new MouseMotionMsg(MouseButton.None, 4, 2), bounds);
        var clickChanged = button.UpdateMouse(new MouseClickMsg(MouseButton.Left, 4, 2), bounds);
        var releaseChanged = button.UpdateMouse(new MouseReleaseMsg(MouseButton.Left, 4, 2), bounds);

        TestAssert.True(hoverChanged, "Mouse motion inside button should update hover state.");
        TestAssert.True(clickChanged, "Mouse click should activate the button.");
        TestAssert.True(releaseChanged, "Mouse release should clear the pressed state.");
        TestAssert.True(button.Hovered, "Button should remain hovered while pointer is inside.");
        TestAssert.True(!button.Pressed, "Button should clear pressed state on release.");
        TestAssert.True(button.PressCount == 1, "Mouse click should increment press count.");
        TestAssert.True(!button.WasPressed, "Release should clear the one-frame pressed signal.");
        return Task.CompletedTask;
    }

    private static Task ButtonComponent_RendersBorderedState()
    {
        var button = new ButtonComponent(new ButtonOptions(
            Label: "Start",
            Description: "click or press enter",
            ShowBorder: true));
        var canvas = new Canvas(24, 5);

        button.Render(canvas, new Rect(0, 0, 24, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains("[Start]", StringComparison.Ordinal), "Bordered button should render its label.");
        TestAssert.True(output.Contains("click or press enter", StringComparison.Ordinal), "Bordered button should render its description.");
        return Task.CompletedTask;
    }

    private static Task TextInputComponent_SubmitsValue()
    {
        var input = new TextInputComponent
        {
            ClearOnSubmit = true,
        };

        input.Update(new KeyPressMsg(KeyCode.Character, "a"));
        input.Update(new KeyPressMsg(KeyCode.Character, "b"));
        input.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("ab", input.LastSubmittedValue, "Text input should capture submitted value.");
        TestAssert.Equal(1, input.SubmitCount, "Text input should count submissions.");
        TestAssert.Equal(string.Empty, input.Value, "Text input should clear after submit when configured.");
        return Task.CompletedTask;
    }

    private static Task TextInputComponent_HidesBorderWhenConfigured()
    {
        var input = new TextInputComponent
        {
            ShowBorder = false,
        };
        input.SetValue("plain");
        var canvas = new Canvas(20, 2);

        input.Render(canvas, new Rect(0, 0, 20, 2));
        var output = canvas.Render();

        TestAssert.True(output.Contains("plain", StringComparison.Ordinal), "Text input should render content in borderless mode.");
        TestAssert.True(!output.Contains("┌", StringComparison.Ordinal), "Text input should not draw border when disabled.");
        return Task.CompletedTask;
    }

    private static Task TextInputComponent_CancelSignalsAndCanClear()
    {
        var input = new TextInputComponent
        {
            ClearOnCancel = true,
        };

        input.Update(new KeyPressMsg(KeyCode.Character, "a"));
        input.Update(new KeyPressMsg(KeyCode.Character, "b"));
        var changed = input.Update(new KeyPressMsg(KeyCode.Escape));

        TestAssert.True(changed, "Text input escape should signal a handled cancel action.");
        TestAssert.True(input.WasCancelled, "Text input should expose cancellation signal after escape.");
        TestAssert.Equal("ab", input.LastCancelledValue, "Text input should capture cancelled value.");
        TestAssert.Equal(1, input.CancelCount, "Text input should count cancel actions.");
        TestAssert.Equal(string.Empty, input.Value, "Text input should clear value on cancel when configured.");
        return Task.CompletedTask;
    }

    private static Task TextAreaComponent_RendersMultilineContent()
    {
        var area = new TextAreaComponent
        {
            ShowLineNumbers = true,
        };
        area.SetValue("a\nb\nc");
        var canvas = new Canvas(24, 8);

        area.Render(canvas, new Rect(0, 0, 24, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("1", StringComparison.Ordinal), "Text area should render line numbers when enabled.");
        TestAssert.True(output.Contains("a", StringComparison.Ordinal), "Text area should render text content.");
        return Task.CompletedTask;
    }

    private static Task TextAreaComponent_EnterInsertsNewline()
    {
        var area = new TextAreaComponent
        {
            Focused = true,
        };

        area.Update(new KeyPressMsg(KeyCode.Character, "l"));
        area.Update(new KeyPressMsg(KeyCode.Character, "i"));
        area.Update(new KeyPressMsg(KeyCode.Character, "n"));
        area.Update(new KeyPressMsg(KeyCode.Character, "e"));
        area.Update(new KeyPressMsg(KeyCode.Character, "A"));
        area.Update(new KeyPressMsg(KeyCode.Enter));
        area.Update(new KeyPressMsg(KeyCode.Character, "l"));
        area.Update(new KeyPressMsg(KeyCode.Character, "i"));
        area.Update(new KeyPressMsg(KeyCode.Character, "n"));
        area.Update(new KeyPressMsg(KeyCode.Character, "e"));
        area.Update(new KeyPressMsg(KeyCode.Character, "B"));

        TestAssert.True(area.Value.Contains('\n'), "Text area Enter should insert newline.");
        TestAssert.True(area.Value.StartsWith("lineA\nlineB", StringComparison.Ordinal), "Text area should keep content on separate lines.");
        return Task.CompletedTask;
    }

    private static Task ListComponent_NavigatesSelection()
    {
        var list = new ListComponent<string>(["one", "two", "three"], x => x)
        {
            Focused = true,
        };

        list.Update(new KeyPressMsg(KeyCode.Down));
        var selected = list.SelectedItem;

        TestAssert.Equal("two", selected ?? string.Empty, "List down key should advance selection.");
        return Task.CompletedTask;
    }

    private static Task ListComponent_AppliesCustomItemStateStyles()
    {
        var list = new ListComponent<string>(["todo", "done"], x => x)
        {
            Focused = true,
            ShowBorder = false,
            ItemStateResolver = item => string.Equals(item, "done", StringComparison.Ordinal)
                ? [WidgetVisualState.Completed]
                : [],
        };
        list.Update(new KeyPressMsg(KeyCode.Down));
        var canvas = new Canvas(28, 3);

        list.Render(canvas, new Rect(0, 0, 28, 3));
        var output = canvas.Render();

        TestAssert.True(output.Contains("[x] ", StringComparison.Ordinal), "List should render completed item prefix when state resolver marks it.");
        TestAssert.True(ContainsStrikethroughSgr(output), "Completed item should use strikethrough style.");
        return Task.CompletedTask;
    }

    private static Task ListComponent_MouseClickSelectsRow()
    {
        var list = new ListComponent<string>(["one", "two", "three"], x => x)
        {
            ShowBorder = false,
        };

        var changed = list.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 1), new Rect(0, 0, 20, 3));

        TestAssert.True(changed, "List mouse click should report selection changes.");
        TestAssert.Equal("two", list.SelectedItem ?? string.Empty, "List mouse click should select clicked row.");
        return Task.CompletedTask;
    }

    private static Task ListComponent_MouseMotionShowsHoverMarker()
    {
        var list = new ListComponent<string>(["one", "two", "three"], x => x)
        {
            ShowBorder = false,
        };
        var changed = list.UpdateMouse(new MouseMotionMsg(MouseButton.None, 0, 1), new Rect(0, 0, 20, 3));
        var canvas = new Canvas(20, 3);

        list.Render(canvas, new Rect(0, 0, 20, 3));
        var output = canvas.Render();

        TestAssert.True(changed, "Mouse motion inside list should update hover state.");
        TestAssert.True(output.Contains("▸ two", StringComparison.Ordinal), "Hovered row should render the hover marker.");
        return Task.CompletedTask;
    }

    private static Task DropdownComponent_SelectsOpenMenuItem()
    {
        var dropdown = new DropdownComponent
        {
            Focused = true,
            Title = "D",
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);

        dropdown.Update(new KeyPressMsg(KeyCode.Enter));
        dropdown.Update(new KeyPressMsg(KeyCode.Down));
        dropdown.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(!dropdown.IsOpen, "Dropdown should close after selecting an item.");
        TestAssert.Equal("beta", dropdown.SelectedItem, "Dropdown should select highlighted item.");
        return Task.CompletedTask;
    }

    private static Task DropdownComponent_HidesBorderWhenConfigured()
    {
        var dropdown = new DropdownComponent
        {
            Focused = true,
            ShowBorder = false,
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);
        var canvas = new Canvas(24, 5);

        dropdown.Render(canvas, new Rect(0, 0, 24, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains("v alpha", StringComparison.Ordinal), "Dropdown should render selected item in borderless mode.");
        TestAssert.True(!output.Contains("┌", StringComparison.Ordinal), "Dropdown should not draw border when disabled.");
        return Task.CompletedTask;
    }

    private static Task DropdownComponent_AppliesOptionStateStyles()
    {
        var dropdown = new DropdownComponent
        {
            Focused = true,
            ShowBorder = false,
            OptionStateResolver = (item, _) => string.Equals(item, "beta", StringComparison.Ordinal)
                ? [WidgetVisualState.Completed]
                : [],
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);
        dropdown.Update(new KeyPressMsg(KeyCode.Enter));
        dropdown.Update(new KeyPressMsg(KeyCode.Down));
        var canvas = new Canvas(30, 6);

        dropdown.Render(canvas, new Rect(0, 0, 30, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("[x] ", StringComparison.Ordinal), "Dropdown should render completed prefix for resolved state.");
        TestAssert.True(ContainsStrikethroughSgr(output), "Dropdown completed state should use strikethrough style.");
        return Task.CompletedTask;
    }

    private static Task DropdownComponent_MouseClickOpensAndSelects()
    {
        var dropdown = new DropdownComponent
        {
            Focused = true,
            ShowBorder = false,
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);
        var bounds = new Rect(0, 0, 24, 6);

        var openChanged = dropdown.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 0), bounds);
        var selectChanged = dropdown.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 2), bounds);

        TestAssert.True(openChanged, "Field click should open dropdown when click activation is enabled.");
        TestAssert.True(selectChanged, "Option click should select highlighted dropdown row.");
        TestAssert.True(!dropdown.IsOpen, "Dropdown should close after click-select.");
        TestAssert.Equal("beta", dropdown.SelectedItem, "Dropdown click-select should pick the clicked option.");
        return Task.CompletedTask;
    }

    private static Task ComboboxComponent_FiltersAndSelects()
    {
        var combobox = new ComboboxComponent
        {
            Focused = true,
            Title = "C",
        };
        combobox.SetItems(["alpha", "beta", "gamma"]);

        combobox.Update(new KeyPressMsg(KeyCode.Character, "g"));
        combobox.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(!combobox.IsOpen, "Combobox should close after selection.");
        TestAssert.Equal("gamma", combobox.SelectedItem, "Combobox should select the filtered match.");
        TestAssert.Equal("gamma", combobox.FilterText, "Combobox filter text should sync to selected item.");
        return Task.CompletedTask;
    }

    private static Task ComboboxComponent_MouseWheelNavigatesAndSelects()
    {
        var combobox = new ComboboxComponent
        {
            Focused = true,
            ShowBorder = false,
        };
        combobox.SetItems(["alpha", "beta", "gamma"]);
        var bounds = new Rect(0, 0, 24, 6);

        var openChanged = combobox.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 0), bounds);
        var wheelChanged = combobox.UpdateMouse(new MouseWheelMsg(MouseButton.WheelDown, 0, 2), bounds);
        var selectChanged = combobox.UpdateMouse(new MouseClickMsg(MouseButton.Left, 0, 3), bounds);

        TestAssert.True(openChanged, "Field click should open combobox list.");
        TestAssert.True(wheelChanged, "Wheel input should move combobox highlight when list is open.");
        TestAssert.True(selectChanged, "Option click should select highlighted combobox row.");
        TestAssert.True(!combobox.IsOpen, "Combobox should close after click-select.");
        TestAssert.Equal("gamma", combobox.SelectedItem, "Combobox selection should reflect wheel-adjusted highlighted option.");
        return Task.CompletedTask;
    }

    private static Task TableComponent_ForwardsSortHotkeys()
    {
        var table = new TableComponent(["A", "B"])
        {
            Focused = true,
            Title = "T",
        };
        table.SetRows(
        [
            ["x", "2"],
            ["y", "1"],
        ]);

        table.Update(new KeyPressMsg(KeyCode.Character, "c"));
        table.Update(new KeyPressMsg(KeyCode.Character, "s"));
        TestAssert.Equal(1, table.SortColumn, "Table should change sort column from hotkey.");
        TestAssert.True(table.SortDescending, "Table should toggle sort direction from hotkey.");
        return Task.CompletedTask;
    }

    private static Task ProgressBarComponent_AdjustsValue()
    {
        var progress = new ProgressBarComponent
        {
            Focused = true,
            Step = 0.25,
        };

        progress.Update(new KeyPressMsg(KeyCode.Right));
        progress.Update(new KeyPressMsg(KeyCode.Right));
        progress.Update(new KeyPressMsg(KeyCode.Left));

        TestAssert.True(Math.Abs(progress.Value - 0.25) < 0.0001, "Progress should settle at 25% after two increments and one decrement.");
        return Task.CompletedTask;
    }

    private static Task StatusBarComponent_RendersLeftAndRightText()
    {
        var status = new StatusBarComponent
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

    private static Task LogViewerComponent_AppendsAndFilters()
    {
        var logs = new LogViewerComponent
        {
            Focused = true,
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

    private static Task DialogComponent_AcceptsAndDismisses()
    {
        var dialog = new DialogComponent
        {
            Visible = true,
            Focused = true,
        };

        var accepted = dialog.Update(new KeyPressMsg(KeyCode.Enter));
        TestAssert.True(accepted, "Dialog should accept on enter.");
        TestAssert.True(dialog.LastResult == DialogResult.Accepted, "Dialog should record accepted result.");

        dialog.Visible = true;
        dialog.Focused = true;
        var dismissed = dialog.Update(new KeyPressMsg(KeyCode.Escape));
        TestAssert.True(dismissed, "Dialog should dismiss on escape.");
        TestAssert.True(dialog.LastResult == DialogResult.Dismissed, "Dialog should record dismissed result.");
        return Task.CompletedTask;
    }

    private static Task LayoutContainerComponent_RendersChildren()
    {
        var layout = new LayoutContainerComponent
        {
            Mode = LayoutContainerMode.Grid,
            GridRows = 1,
            GridColumns = 2,
        };
        layout.Add(new LabelComponent { DrawBorder = false, Text = "left" });
        layout.Add(new LabelComponent { DrawBorder = false, Text = "right" });

        var canvas = new Canvas(20, 3);
        layout.Render(canvas, new Rect(0, 0, 20, 3));
        var output = canvas.Render();

        TestAssert.True(output.Contains("left", StringComparison.Ordinal), "Layout container should render first child.");
        TestAssert.True(output.Contains("right", StringComparison.Ordinal), "Layout container should render second child.");
        return Task.CompletedTask;
    }

    private static Task LayoutContainerComponent_MouseResizeAdjustsPrimarySize()
    {
        var layout = new LayoutContainerComponent
        {
            Mode = LayoutContainerMode.Horizontal,
            MinPrimarySize = 4,
            MinSecondarySize = 4,
        };
        layout.Add(new LabelComponent { DrawBorder = false, Text = "left" });
        layout.Add(new LabelComponent { DrawBorder = false, Text = "right" });
        layout.SetPrimarySize(12);

        var bounds = new Rect(0, 0, 30, 6);
        layout.UpdateMouse(new MouseClickMsg(MouseButton.Left, 12, 1), bounds);
        var changed = layout.UpdateMouse(new MouseMotionMsg(MouseButton.Left, 20, 1), bounds);
        layout.UpdateMouse(new MouseReleaseMsg(MouseButton.Left, 20, 1), bounds);

        TestAssert.True(changed, "Dragging split should update primary size.");
        TestAssert.Equal(20, layout.PrimarySize ?? -1, "Drag motion should move split to pointer position.");
        return Task.CompletedTask;
    }

    private static Task LayoutContainerComponent_KeyboardRoutesToFocusedChildOnly()
    {
        var first = new KeyProbeComponent { Focused = true };
        var second = new KeyProbeComponent();
        var layout = new LayoutContainerComponent
        {
            Mode = LayoutContainerMode.Horizontal,
        };
        layout.Add(first);
        layout.Add(second);

        var changed = layout.Update(new KeyPressMsg(KeyCode.Character, "x"));

        TestAssert.True(changed, "Focused child should handle keyboard input.");
        TestAssert.Equal(1, first.KeyEvents, "Focused child should receive keyboard input.");
        TestAssert.Equal(0, second.KeyEvents, "Non-focused child should not receive keyboard input.");
        return Task.CompletedTask;
    }

    private static Task LayoutContainerComponent_MouseRoutesToTargetChild()
    {
        var first = new MouseProbeComponent();
        var second = new MouseProbeComponent();
        var layout = new LayoutContainerComponent
        {
            Mode = LayoutContainerMode.Horizontal,
        };
        layout.Add(first);
        layout.Add(second);

        var changed = layout.UpdateMouse(new MouseClickMsg(MouseButton.Left, 16, 1), new Rect(0, 0, 20, 4));

        TestAssert.True(changed, "Mouse click should be routed and focus updated.");
        TestAssert.Equal(0, first.MouseEvents, "First child should not receive click outside its bounds.");
        TestAssert.Equal(1, second.MouseEvents, "Second child should receive routed mouse click.");
        TestAssert.True(!first.Focused, "First child focus should be cleared.");
        TestAssert.True(second.Focused, "Target child should become focused.");
        return Task.CompletedTask;
    }

    private sealed class KeyProbeComponent : IStatefulComponent, IFocusableComponent
    {
        public bool Focused { get; set; }

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
            canvas.WriteText(rect.X, rect.Y, KeyEvents.ToString(), rect.Width);
        }
    }

    private sealed class MouseProbeComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
    {
        public bool Focused { get; set; }

        public int MouseEvents { get; private set; }

        public bool Update(TeaSharp.Core.Abstractions.IMessage message) => false;

        public bool UpdateMouse(MouseMsg message, Rect bounds)
        {
            MouseEvents++;
            return true;
        }

        public void Render(Canvas canvas, Rect rect)
        {
            canvas.WriteText(rect.X, rect.Y, Focused ? "focused" : "idle", rect.Width);
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
