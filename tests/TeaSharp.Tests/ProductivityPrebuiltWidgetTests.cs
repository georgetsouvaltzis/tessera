using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class ProductivityPrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Controls_NumberInput_AdjustsAndSubmits", NumberInput_AdjustsAndSubmits);
        yield return new TestCase("Controls_NumberInput_SubmittedEvent_ReportsValue", NumberInput_SubmittedEvent_ReportsValue);
        yield return new TestCase("Controls_NumberInput_TryConsumeSubmission_IsSingleUse", NumberInput_TryConsumeSubmission_IsSingleUse);
        yield return new TestCase("Controls_DatePicker_MovesDate", DatePicker_MovesDate);
        yield return new TestCase("Controls_DatePicker_DateChangedEvent_ReportsTransition", DatePicker_DateChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_DatePicker_MouseClickSelectsDate", DatePicker_MouseClickSelectsDate);
        yield return new TestCase("Controls_TimePicker_AdjustsField", TimePicker_AdjustsField);
        yield return new TestCase("Controls_TimePicker_ValueChangedEvent_ReportsTransition", TimePicker_ValueChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_TimePicker_MouseWheelAdjustsField", TimePicker_MouseWheelAdjustsField);
        yield return new TestCase("Controls_Paginator_KeyboardNavigationAndBoundsClamping", Paginator_KeyboardNavigationAndBoundsClamping);
        yield return new TestCase("Controls_Paginator_PageChangedEvent_ReportsTransition", Paginator_PageChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_Paginator_MousePressOnHitTargets_ChangesPage", Paginator_MousePressOnHitTargets_ChangesPage);
        yield return new TestCase("Controls_Paginator_RendersCompactOneLineLayout", Paginator_RendersCompactOneLineLayout);
        yield return new TestCase("Controls_FuzzyFinder_SetQuery_FiltersResults", FuzzyFinder_SetQuery_FiltersResults);
        yield return new TestCase("Controls_FuzzyFinder_KeyboardNavigationAndEnter_RaisesSelection", FuzzyFinder_KeyboardNavigationAndEnter_RaisesSelection);
        yield return new TestCase("Controls_FuzzyFinder_Escape_ClearsThenCloses", FuzzyFinder_Escape_ClearsThenCloses);
        yield return new TestCase("Controls_FuzzyFinder_MousePress_SelectsAndActivatesRow", FuzzyFinder_MousePress_SelectsAndActivatesRow);
        yield return new TestCase("Controls_FuzzyFinder_RendersPlaceholderAndSelectedMarker", FuzzyFinder_RendersPlaceholderAndSelectedMarker);
        yield return new TestCase("Controls_PropertyGrid_KeyboardNavigationAndReadOnlySemantics", PropertyGrid_KeyboardNavigationAndReadOnlySemantics);
        yield return new TestCase("Controls_PropertyGrid_SelectionChangedEvent_ReportsTransition", PropertyGrid_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_PropertyGrid_RendersHeadersCategoriesAndSelection", PropertyGrid_RendersHeadersCategoriesAndSelection);
        yield return new TestCase("Controls_PropertyGrid_StyleHooks_EmitSgrFragments", PropertyGrid_StyleHooks_EmitSgrFragments);
        yield return new TestCase("Controls_KeyValueList_KeyboardNavigationAndBounds", KeyValueList_KeyboardNavigationAndBounds);
        yield return new TestCase("Controls_KeyValueList_SelectionChangedEvent_ReportsTransition", KeyValueList_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_KeyValueList_MousePressSelectsRow", KeyValueList_MousePressSelectsRow);
        yield return new TestCase("Controls_KeyValueList_StyleHooks_Rendered", KeyValueList_StyleHooks_Rendered);
        yield return new TestCase("Controls_SearchBox_UpdatesQueryAndRaisesEvent", SearchBox_UpdatesQueryAndRaisesEvent);
        yield return new TestCase("Controls_SearchBox_NavigationCommands_UpdateIndexAndRaiseEvent", SearchBox_NavigationCommands_UpdateIndexAndRaiseEvent);
        yield return new TestCase("Controls_SearchBox_MousePressOnHitTargets_Navigates", SearchBox_MousePressOnHitTargets_Navigates);
        yield return new TestCase("Controls_SearchBox_RendersPlaceholderAndMatchCounter", SearchBox_RendersPlaceholderAndMatchCounter);
        yield return new TestCase("Controls_MarkdownView_RendersMarkdown", MarkdownView_RendersMarkdown);
    }

    private static Task NumberInput_AdjustsAndSubmits()
    {
        var input = new NumberInput
        {
            IsFocused = true,
            Min = 0,
            Max = 10,
            Step = 2,
        };
        input.SetValue(2);
        input.Handle(new KeyPressed(Key.Up));
        input.Handle(new KeyPressed(Key.Up));
        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(Math.Abs(input.Value - 6) < 0.0001, "Number input should adjust value by step.");
        TestAssert.True(input.LastSubmittedValue.HasValue, "Number input should track submitted value.");

        input.SetValue(4);
        input.Handle(new KeyPressed(Key.Character, "1"));
        input.Handle(new KeyPressed(Key.Character, "2"));
        input.Handle(new KeyPressed(Key.Character, "."));
        input.Handle(new KeyPressed(Key.Character, "5"));
        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(Math.Abs(input.Value - 10) < 0.0001, "Number input should parse decimal text entry and clamp to range.");
        return Task.CompletedTask;
    }

    private static Task NumberInput_TryConsumeSubmission_IsSingleUse()
    {
        var input = new NumberInput
        {
            IsFocused = true,
        };
        input.SetValue(3);

        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(input.TryConsumeSubmission(out var submitted), "Number input should expose one-shot submit consumption.");
        TestAssert.True(Math.Abs(submitted - 3) < 0.0001, "Number input should consume the submitted numeric value.");
        TestAssert.True(!input.TryConsumeSubmission(out _), "Number input should not report the same submit twice.");
        return Task.CompletedTask;
    }

    private static Task NumberInput_SubmittedEvent_ReportsValue()
    {
        var input = new NumberInput
        {
            IsFocused = true,
        };
        input.SetValue(3);
        double submitted = -1;
        input.Submitted += (_, args) => submitted = args.Value;

        input.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(Math.Abs(submitted - 3) < 0.0001, "Number input submit event should expose the submitted numeric value.");
        return Task.CompletedTask;
    }

    private static Task DatePicker_MovesDate()
    {
        var picker = new DatePicker
        {
            IsFocused = true,
        };
        picker.SetDate(new DateOnly(2026, 3, 8));
        picker.Handle(new KeyPressed(Key.Right));
        picker.Handle(new KeyPressed(Key.Down));

        TestAssert.Equal(new DateOnly(2026, 3, 16), picker.SelectedDate, "Date picker should move day and week correctly.");
        return Task.CompletedTask;
    }

    private static Task DatePicker_MouseClickSelectsDate()
    {
        var picker = new DatePicker
        {
            Border = BorderStyle.None,
        };
        picker.SetDate(new DateOnly(2026, 3, 8));

        var changed = picker.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 4), new Rect(0, 0, 24, 10));

        TestAssert.True(changed, "Date picker click should select day under pointer.");
        TestAssert.Equal(new DateOnly(2026, 3, 9), picker.SelectedDate, "Date picker click should select correct calendar date.");
        return Task.CompletedTask;
    }

    private static Task DatePicker_DateChangedEvent_ReportsTransition()
    {
        var picker = new DatePicker
        {
            IsFocused = true,
        };
        picker.SetDate(new DateOnly(2026, 3, 8));
        DateChangedEventArgs? args = null;
        picker.DateChanged += (_, eventArgs) => args = eventArgs;

        picker.Handle(new KeyPressed(Key.Right));

        TestAssert.True(args is not null, "Date picker should raise date changed when the selected date changes.");
        TestAssert.Equal(new DateOnly(2026, 3, 8), args!.PreviousDate, "Date picker event should expose the previous date.");
        TestAssert.Equal(new DateOnly(2026, 3, 9), args.SelectedDate, "Date picker event should expose the selected date.");
        return Task.CompletedTask;
    }

    private static Task TimePicker_AdjustsField()
    {
        var picker = new TimePicker
        {
            IsFocused = true,
            MinuteStep = 5,
        };
        picker.SetValue(new TimeOnly(10, 0, 0));
        picker.Handle(new KeyPressed(Key.Right));
        picker.Handle(new KeyPressed(Key.Up));

        TestAssert.Equal(new TimeOnly(10, 5, 0), picker.Value, "Time picker should adjust minute field.");
        return Task.CompletedTask;
    }

    private static Task TimePicker_MouseWheelAdjustsField()
    {
        var picker = new TimePicker
        {
            Border = BorderStyle.None,
            MinuteStep = 5,
        };
        picker.SetValue(new TimeOnly(10, 0, 0));

        picker.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 3, 0), new Rect(0, 0, 12, 1));
        var changed = picker.Handle(new PointerInput(PointerEventKind.Wheel, PointerButton.WheelUp, 3, 0), new Rect(0, 0, 12, 1));

        TestAssert.True(changed, "Time picker wheel should adjust hovered/active field.");
        TestAssert.Equal(new TimeOnly(10, 5, 0), picker.Value, "Time picker wheel should increase minute field by configured step.");
        return Task.CompletedTask;
    }

    private static Task TimePicker_ValueChangedEvent_ReportsTransition()
    {
        var picker = new TimePicker
        {
            IsFocused = true,
            MinuteStep = 5,
        };
        picker.SetValue(new TimeOnly(10, 0, 0));
        TimeValueChangedEventArgs? args = null;
        picker.ValueChanged += (_, eventArgs) => args = eventArgs;

        picker.Handle(new KeyPressed(Key.Right));
        picker.Handle(new KeyPressed(Key.Up));

        TestAssert.True(args is not null, "Time picker should raise value changed when the selected time changes.");
        TestAssert.Equal(new TimeOnly(10, 0, 0), args!.PreviousValue, "Time picker event should expose the previous value.");
        TestAssert.Equal(new TimeOnly(10, 5, 0), args.Value, "Time picker event should expose the current value.");
        return Task.CompletedTask;
    }

    private static Task Paginator_KeyboardNavigationAndBoundsClamping()
    {
        var paginator = new Paginator
        {
            IsFocused = true,
            PageCount = 3,
        };

        var leftChanged = paginator.Handle(new KeyPressed(Key.Left));
        TestAssert.True(!leftChanged, "Paginator should not move left from the first page.");
        TestAssert.Equal(0, paginator.PageIndex, "Paginator should stay on first page when moving left from index 0.");

        paginator.SetPage(999);
        TestAssert.Equal(2, paginator.PageIndex, "Paginator should clamp out-of-range positive page requests.");
        paginator.SetPage(-5);
        TestAssert.Equal(0, paginator.PageIndex, "Paginator should clamp out-of-range negative page requests.");

        var pageDownChanged = paginator.Handle(new KeyPressed(Key.PageDown));
        TestAssert.True(pageDownChanged, "Paginator PageDown should move to next page.");
        TestAssert.Equal(1, paginator.PageIndex, "Paginator should move to page index 1 after PageDown.");

        var endChanged = paginator.Handle(new KeyPressed(Key.End));
        TestAssert.True(endChanged, "Paginator End should jump to the last page.");
        TestAssert.Equal(2, paginator.PageIndex, "Paginator End should select the last page.");

        var rightAtEndChanged = paginator.Handle(new KeyPressed(Key.Right));
        TestAssert.True(!rightAtEndChanged, "Paginator should not move right from the last page.");
        TestAssert.Equal(2, paginator.PageIndex, "Paginator should stay on last page when moving right from end.");
        return Task.CompletedTask;
    }

    private static Task Paginator_PageChangedEvent_ReportsTransition()
    {
        var paginator = new Paginator
        {
            PageCount = 4,
        };
        PageChangedEventArgs? args = null;
        paginator.PageChanged += (_, eventArgs) => args = eventArgs;

        paginator.SetPage(2);

        TestAssert.True(args is not null, "Paginator should raise page changed when page index updates.");
        TestAssert.Equal(0, args!.PreviousPageIndex, "Paginator event should expose previous page index.");
        TestAssert.Equal(2, args.NewPageIndex, "Paginator event should expose new page index.");
        return Task.CompletedTask;
    }

    private static Task Paginator_MousePressOnHitTargets_ChangesPage()
    {
        var paginator = new Paginator
        {
            PageCount = 3,
        };
        var bounds = new Rect(0, 0, 40, 1);

        var nextChanged = paginator.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 16, 0), bounds);
        TestAssert.True(nextChanged, "Paginator next hit target click should advance page.");
        TestAssert.Equal(1, paginator.PageIndex, "Paginator next hit target should select the next page.");

        var prevChanged = paginator.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0), bounds);
        TestAssert.True(prevChanged, "Paginator previous hit target click should move to previous page.");
        TestAssert.Equal(0, paginator.PageIndex, "Paginator previous hit target should select the previous page.");

        var prevDisabledChanged = paginator.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 0), bounds);
        TestAssert.True(!prevDisabledChanged, "Paginator previous hit target should not move before the first page.");
        TestAssert.Equal(0, paginator.PageIndex, "Paginator should stay on first page after disabled previous click.");
        return Task.CompletedTask;
    }

    private static Task Paginator_RendersCompactOneLineLayout()
    {
        var paginator = new Paginator
        {
            PageCount = 12,
        };
        paginator.SetPage(4);
        var canvas = new Canvas(40, 1);

        paginator.Render(canvas, new Rect(0, 0, 40, 1));
        var output = canvas.Render();

        TestAssert.True(
            output.Contains("Prev  Page 5/12  Next", StringComparison.Ordinal),
            "Paginator should render compact one-line layout with current page label.");
        return Task.CompletedTask;
    }

    private static Task FuzzyFinder_SetQuery_FiltersResults()
    {
        var finder = new FuzzyFinder
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        finder.SetItems(
        [
            new FuzzyFinderItem("api", "ApiClient.cs", "src/TeaSharp"),
            new FuzzyFinderItem("fuzzy", "FuzzyFinder.cs", "src/TeaSharp/Controls"),
            new FuzzyFinderItem("readme", "README.md", "docs"),
        ]);

        finder.SetQuery("ffc");

        TestAssert.Equal(1, finder.ResultCount, "FuzzyFinder should keep only matching results for a restrictive query.");
        TestAssert.True(finder.SelectedItem is not null, "FuzzyFinder should keep a selected item when results exist.");
        TestAssert.Equal("fuzzy", finder.SelectedItem!.Id, "FuzzyFinder should surface the matched item.");
        return Task.CompletedTask;
    }

    private static Task FuzzyFinder_KeyboardNavigationAndEnter_RaisesSelection()
    {
        var finder = new FuzzyFinder
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        finder.SetItems(["alpha", "beta", "gamma"]);

        FuzzyFinderSelectionChangedEventArgs? selectionChangedArgs = null;
        FuzzyFinderItemSelectedEventArgs? selectedArgs = null;
        finder.SelectionChanged += (_, args) => selectionChangedArgs = args;
        finder.ItemSelected += (_, args) => selectedArgs = args;

        finder.Handle(new KeyPressed(Key.Down));
        finder.Handle(new KeyPressed(Key.Down));
        finder.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal(2, finder.SelectedIndex, "FuzzyFinder should move selection with keyboard navigation.");
        TestAssert.True(selectionChangedArgs is not null, "FuzzyFinder should raise selection-changed when highlight moves.");
        TestAssert.True(selectedArgs is not null, "FuzzyFinder should raise selected event when Enter activates a row.");
        TestAssert.Equal("gamma", selectedArgs!.ItemId, "FuzzyFinder selected payload should include the selected item id.");
        TestAssert.True(
            string.Equals("gamma", finder.LastSelectedItemId, StringComparison.Ordinal),
            "FuzzyFinder should track the last activated item id.");
        return Task.CompletedTask;
    }

    private static Task FuzzyFinder_Escape_ClearsThenCloses()
    {
        var finder = new FuzzyFinder
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        finder.SetItems(["alpha", "beta"]);
        finder.SetQuery("a");

        var cleared = finder.Handle(new KeyPressed(Key.Escape));
        var closed = finder.Handle(new KeyPressed(Key.Escape));
        var ignored = finder.Handle(new KeyPressed(Key.Escape));

        TestAssert.True(cleared, "FuzzyFinder should handle Escape when query text is present.");
        TestAssert.Equal(string.Empty, finder.QueryText, "FuzzyFinder Escape should clear query first.");
        TestAssert.True(closed, "FuzzyFinder should handle Escape after query is cleared by closing results.");
        TestAssert.True(!finder.IsOpen, "FuzzyFinder should close results after second Escape.");
        TestAssert.True(!ignored, "FuzzyFinder should ignore Escape when already closed and query is empty.");
        return Task.CompletedTask;
    }

    private static Task FuzzyFinder_MousePress_SelectsAndActivatesRow()
    {
        var finder = new FuzzyFinder
        {
            Border = BorderStyle.None,
        };
        finder.SetItems(["one", "two", "three"]);

        FuzzyFinderItemSelectedEventArgs? selectedArgs = null;
        finder.ItemSelected += (_, args) => selectedArgs = args;
        var bounds = new Rect(0, 0, 40, 5);

        var changed = finder.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2), bounds);

        TestAssert.True(changed, "FuzzyFinder mouse press should select and activate a result row.");
        TestAssert.Equal(1, finder.SelectedIndex, "FuzzyFinder mouse press should map row coordinates to selected index.");
        TestAssert.True(selectedArgs is not null, "FuzzyFinder mouse press should raise item selected payload.");
        TestAssert.Equal("two", selectedArgs!.ItemId, "FuzzyFinder mouse press should activate clicked item.");
        return Task.CompletedTask;
    }

    private static Task FuzzyFinder_RendersPlaceholderAndSelectedMarker()
    {
        var finder = new FuzzyFinder
        {
            Border = BorderStyle.None,
            Placeholder = "search files",
            PlaceholderTextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightYellow),
            SelectedItemStyle = TeaStyle.Empty.WithBold(),
        };
        finder.SetItems(["alpha", "beta"]);
        var canvas = new Canvas(40, 4);

        finder.Render(canvas, new Rect(0, 0, 40, 4));
        var output = canvas.Render();

        TestAssert.True(output.Contains("search files", StringComparison.Ordinal), "FuzzyFinder should render placeholder text.");
        TestAssert.True(output.Contains("> alpha", StringComparison.Ordinal), "FuzzyFinder should render selected row marker.");
        TestAssert.True(output.Contains("\u001b[38;5;11m", StringComparison.Ordinal), "FuzzyFinder should apply placeholder style SGR.");
        return Task.CompletedTask;
    }

    private static Task PropertyGrid_KeyboardNavigationAndReadOnlySemantics()
    {
        var grid = new PropertyGrid
        {
            IsFocused = true,
        };
        grid.SetProperties(
        [
            new PropertyGridProperty("Host", "localhost", "General"),
            new PropertyGridProperty("Port", "5432", "General"),
            new PropertyGridProperty("Timeout", "30s", "Network"),
        ]);

        var upAtStart = grid.Handle(new KeyPressed(Key.Up));
        TestAssert.True(!upAtStart, "PropertyGrid should not move above the first row.");
        TestAssert.Equal(0, grid.SelectedIndex, "PropertyGrid should start with first row selected.");

        var downChanged = grid.Handle(new KeyPressed(Key.Down));
        TestAssert.True(downChanged, "PropertyGrid should move selection down.");
        TestAssert.Equal(1, grid.SelectedIndex, "PropertyGrid should move to second row after Down.");

        var endChanged = grid.Handle(new KeyPressed(Key.End));
        TestAssert.True(endChanged, "PropertyGrid should jump to last row on End.");
        TestAssert.Equal(2, grid.SelectedIndex, "PropertyGrid should select the last row on End.");

        grid.IsReadOnly = true;
        var blocked = grid.Handle(new KeyPressed(Key.Up));
        TestAssert.True(!blocked, "PropertyGrid should ignore navigation while read-only.");
        TestAssert.Equal(2, grid.SelectedIndex, "PropertyGrid selection should remain unchanged while read-only.");
        return Task.CompletedTask;
    }

    private static Task PropertyGrid_SelectionChangedEvent_ReportsTransition()
    {
        var grid = new PropertyGrid
        {
            IsFocused = true,
        };
        grid.SetProperties(
        [
            new PropertyGridProperty("User", "tea"),
            new PropertyGridProperty("Retries", "3"),
        ]);

        PropertyGridSelectionChangedEventArgs? args = null;
        grid.SelectionChanged += (_, eventArgs) => args = eventArgs;

        grid.Handle(new KeyPressed(Key.Down));

        TestAssert.True(args is not null, "PropertyGrid should raise selection changed when selection updates.");
        TestAssert.Equal(0, args!.PreviousIndex, "PropertyGrid event should expose previous index.");
        TestAssert.Equal(1, args.CurrentIndex, "PropertyGrid event should expose current index.");
        TestAssert.True(args.PreviousProperty is not null, "PropertyGrid event should expose previous property.");
        TestAssert.True(args.CurrentProperty is not null, "PropertyGrid event should expose current property.");
        TestAssert.Equal("User", args.PreviousProperty!.Name, "PropertyGrid event previous property should match previous selection.");
        TestAssert.Equal("Retries", args.CurrentProperty!.Name, "PropertyGrid event current property should match new selection.");
        return Task.CompletedTask;
    }

    private static Task PropertyGrid_RendersHeadersCategoriesAndSelection()
    {
        var grid = new PropertyGrid
        {
            Border = BorderStyle.None,
        };
        grid.SetProperties(
        [
            new PropertyGridProperty("Host", "localhost", "General"),
            new PropertyGridProperty("Port", "5432", "General"),
            new PropertyGridProperty("Timeout", "30s", "Network"),
        ]);
        grid.SetSelectedIndex(1);
        var canvas = new Canvas(52, 8);

        grid.Render(canvas, new Rect(0, 0, 52, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Property", StringComparison.Ordinal), "PropertyGrid should render key column header.");
        TestAssert.True(output.Contains("Value", StringComparison.Ordinal), "PropertyGrid should render value column header.");
        TestAssert.True(output.Contains("[General]", StringComparison.Ordinal), "PropertyGrid should render category header.");
        TestAssert.True(output.Contains("> Port", StringComparison.Ordinal), "PropertyGrid should render selected row marker.");
        return Task.CompletedTask;
    }

    private static Task PropertyGrid_StyleHooks_EmitSgrFragments()
    {
        var grid = new PropertyGrid
        {
            Border = BorderStyle.None,
            HeaderStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            KeyStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            ValueStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            SelectedRowStyle = TeaStyle.Empty.WithUnderline().WithBold(),
        };
        grid.SetProperties(
        [
            new PropertyGridProperty("Mode", "prod", "Runtime"),
            new PropertyGridProperty("Workers", "8", "Runtime"),
        ]);
        grid.SetSelectedIndex(1);
        var canvas = new Canvas(52, 6);

        grid.Render(canvas, new Rect(0, 0, 52, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("\u001b[38;5;12m", StringComparison.Ordinal), "PropertyGrid header style should emit SGR fragments.");
        TestAssert.True(output.Contains("\u001b[38;5;14m", StringComparison.Ordinal), "PropertyGrid key style should emit SGR fragments.");
        TestAssert.True(output.Contains("\u001b[38;5;10m", StringComparison.Ordinal), "PropertyGrid value style should emit SGR fragments.");
        var hasCombinedBoldUnderline = output.Contains("\u001b[1;4m", StringComparison.Ordinal)
            || output.Contains("\u001b[4;1m", StringComparison.Ordinal)
            || output.Contains("\u001b[1;4;", StringComparison.Ordinal)
            || output.Contains("\u001b[4;1;", StringComparison.Ordinal);
        var hasSeparateBoldUnderline = output.Contains("\u001b[1;", StringComparison.Ordinal)
            && output.Contains(";4m", StringComparison.Ordinal);
        TestAssert.True(
            hasCombinedBoldUnderline || hasSeparateBoldUnderline,
            "PropertyGrid selected row style should merge into row rendering.");
        return Task.CompletedTask;
    }

    private static Task KeyValueList_KeyboardNavigationAndBounds()
    {
        var list = new KeyValueList
        {
            IsFocused = true,
        };
        list.SetEntries(
        [
            new KeyValueListEntry("Host", "localhost"),
            new KeyValueListEntry("Port", "5432"),
            new KeyValueListEntry("Timeout", "30s"),
        ]);

        var upAtStart = list.Handle(new KeyPressed(Key.Up));
        TestAssert.True(!upAtStart, "KeyValueList should not move above first row.");
        TestAssert.Equal(0, list.SelectedIndex, "KeyValueList should select first row by default.");

        var downChanged = list.Handle(new KeyPressed(Key.Down));
        TestAssert.True(downChanged, "KeyValueList should move selection down.");
        TestAssert.Equal(1, list.SelectedIndex, "KeyValueList should move to second row after Down.");

        var endChanged = list.Handle(new KeyPressed(Key.End));
        TestAssert.True(endChanged, "KeyValueList End should jump to last row.");
        TestAssert.Equal(2, list.SelectedIndex, "KeyValueList should select last row after End.");

        list.IsReadOnly = true;
        var blocked = list.Handle(new KeyPressed(Key.Up));
        TestAssert.True(!blocked, "KeyValueList should ignore navigation when read-only.");
        TestAssert.Equal(2, list.SelectedIndex, "KeyValueList selection should remain unchanged while read-only.");
        return Task.CompletedTask;
    }

    private static Task KeyValueList_SelectionChangedEvent_ReportsTransition()
    {
        var list = new KeyValueList
        {
            IsFocused = true,
        };
        list.SetEntries(
        [
            new KeyValueListEntry("User", "tea"),
            new KeyValueListEntry("Retries", "3"),
        ]);

        KeyValueListSelectionChangedEventArgs? args = null;
        list.SelectionChanged += (_, eventArgs) => args = eventArgs;

        list.Handle(new KeyPressed(Key.Down));

        TestAssert.True(args is not null, "KeyValueList should raise selection changed when selection updates.");
        TestAssert.Equal(0, args!.PreviousIndex, "KeyValueList event should expose previous index.");
        TestAssert.Equal(1, args.CurrentIndex, "KeyValueList event should expose current index.");
        TestAssert.True(args.PreviousItem is not null, "KeyValueList event should expose previous item.");
        TestAssert.True(args.CurrentItem is not null, "KeyValueList event should expose current item.");
        TestAssert.Equal("User", args.PreviousItem!.Key, "KeyValueList previous item key should match previous selection.");
        TestAssert.Equal("Retries", args.CurrentItem!.Key, "KeyValueList current item key should match new selection.");
        return Task.CompletedTask;
    }

    private static Task KeyValueList_MousePressSelectsRow()
    {
        var list = new KeyValueList
        {
            Border = BorderStyle.None,
        };
        list.SetEntries(
        [
            new KeyValueListEntry("Host", "localhost"),
            new KeyValueListEntry("Port", "5432"),
            new KeyValueListEntry("Timeout", "30s"),
        ]);

        var changed = list.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 1), new Rect(0, 0, 48, 3));

        TestAssert.True(changed, "KeyValueList click should update selected row.");
        TestAssert.Equal(1, list.SelectedIndex, "KeyValueList click should select row by pointer Y offset.");
        TestAssert.True(list.SelectedItem is not null, "KeyValueList should expose selected item after click.");
        TestAssert.Equal("Port", list.SelectedItem!.Key, "KeyValueList click should select expected key.");
        return Task.CompletedTask;
    }

    private static Task KeyValueList_StyleHooks_Rendered()
    {
        var list = new KeyValueList
        {
            Border = BorderStyle.None,
            KeyStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            ValueStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            SeparatorStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightYellow),
            SelectedRowStyle = TeaStyle.Empty.WithBold(),
        };
        list.SetEntries(
        [
            new KeyValueListEntry("Host", "localhost"),
            new KeyValueListEntry("Port", "5432"),
        ]);
        list.SetSelectedIndex(1);
        var canvas = new Canvas(48, 3);

        list.Render(canvas, new Rect(0, 0, 48, 3));
        var output = canvas.Render();

        TestAssert.True(output.Contains("> Port", StringComparison.Ordinal), "KeyValueList should render selected row marker.");
        TestAssert.True(output.Contains("\u001b[38;5;14m", StringComparison.Ordinal), "KeyValueList key style should emit SGR fragments.");
        TestAssert.True(output.Contains("\u001b[38;5;10m", StringComparison.Ordinal), "KeyValueList value style should emit SGR fragments.");
        TestAssert.True(output.Contains("\u001b[38;5;11m", StringComparison.Ordinal), "KeyValueList separator style should emit SGR fragments.");
        return Task.CompletedTask;
    }

    private static Task SearchBox_UpdatesQueryAndRaisesEvent()
    {
        var search = new SearchBox
        {
            IsFocused = true,
        };
        SearchBoxQueryChangedEventArgs? args = null;
        var raised = 0;
        search.QueryChanged += (_, eventArgs) =>
        {
            raised++;
            args = eventArgs;
        };

        search.Handle(new KeyPressed(Key.Character, "a"));
        search.Handle(new KeyPressed(Key.Character, "b"));

        TestAssert.Equal("ab", search.QueryText, "SearchBox should update query text from typed characters.");
        TestAssert.Equal(2, raised, "SearchBox should raise query changed once per mutation.");
        TestAssert.True(args is not null, "SearchBox should provide query changed payload.");
        TestAssert.Equal("a", args!.PreviousQuery, "SearchBox query payload should expose previous query.");
        TestAssert.Equal("ab", args.Query, "SearchBox query payload should expose current query.");
        return Task.CompletedTask;
    }

    private static Task SearchBox_NavigationCommands_UpdateIndexAndRaiseEvent()
    {
        var search = new SearchBox
        {
            IsFocused = true,
        };
        search.SetMatchState(5, 0);

        SearchBoxNavigationRequestedEventArgs? args = null;
        var raised = 0;
        search.NavigationRequested += (_, eventArgs) =>
        {
            raised++;
            args = eventArgs;
        };

        search.Handle(new KeyPressed(Key.Enter));
        search.Handle(new KeyPressed(Key.F3));
        search.Handle(new KeyPressed(Key.F3, string.Empty, ModifierKeys.Shift));

        TestAssert.Equal(3, raised, "SearchBox should raise navigation requested per navigation command.");
        TestAssert.Equal(1, search.CurrentMatchIndex ?? -1, "SearchBox should update current match index based on navigation.");
        TestAssert.True(args is not null, "SearchBox should expose navigation payload details.");
        TestAssert.True(args!.Direction == SearchNavigationDirection.Previous, "SearchBox should report previous direction on Shift+F3.");
        TestAssert.Equal(2, args.PreviousMatchIndex ?? -1, "SearchBox should report previous index before navigation.");
        TestAssert.Equal(1, args.CurrentMatchIndex ?? -1, "SearchBox should report current index after navigation.");
        return Task.CompletedTask;
    }

    private static Task SearchBox_MousePressOnHitTargets_Navigates()
    {
        var search = new SearchBox
        {
            Border = BorderStyle.None,
        };
        search.SetMatchState(3, 0);
        var bounds = new Rect(0, 0, 30, 1);

        var nextChanged = search.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 26, 0), bounds);
        TestAssert.True(nextChanged, "SearchBox next hit target click should be handled.");
        TestAssert.Equal(1, search.CurrentMatchIndex ?? -1, "SearchBox next hit target should move to next match.");

        var previousChanged = search.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 21, 0), bounds);
        TestAssert.True(previousChanged, "SearchBox previous hit target click should be handled.");
        TestAssert.Equal(0, search.CurrentMatchIndex ?? -1, "SearchBox previous hit target should move to previous match.");
        return Task.CompletedTask;
    }

    private static Task SearchBox_RendersPlaceholderAndMatchCounter()
    {
        var search = new SearchBox
        {
            Border = BorderStyle.None,
            Placeholder = "find text",
        };
        search.SetMatchState(8, 2);
        var canvas = new Canvas(36, 1);

        search.Render(canvas, new Rect(0, 0, 36, 1));
        var output = canvas.Render();

        TestAssert.True(output.Contains("find text", StringComparison.Ordinal), "SearchBox should render placeholder text when query is empty.");
        TestAssert.True(output.Contains("3/8", StringComparison.Ordinal), "SearchBox should render current/total match label.");
        TestAssert.True(output.Contains("Prev", StringComparison.Ordinal), "SearchBox should render previous navigation label.");
        TestAssert.True(output.Contains("Next", StringComparison.Ordinal), "SearchBox should render next navigation label.");
        return Task.CompletedTask;
    }

    private static Task MarkdownView_RendersMarkdown()
    {
        var viewer = new MarkdownView
        {
            Border = BorderStyle.None,
        };
        viewer.SetMarkdown("# title\n- one\n```\ncode\n```");
        var canvas = new Canvas(40, 8);

        viewer.Render(canvas, new Rect(0, 0, 40, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("# TITLE", StringComparison.Ordinal), "Markdown viewer should render heading.");
        TestAssert.True(output.Contains("• one", StringComparison.Ordinal), "Markdown viewer should render bullets.");
        TestAssert.True(output.Contains("code", StringComparison.Ordinal), "Markdown viewer should render code block content.");
        return Task.CompletedTask;
    }
}
