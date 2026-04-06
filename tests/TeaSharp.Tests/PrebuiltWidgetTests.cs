using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Controls;
using TeaSharp.Styles;
using System.Globalization;
using System.Text.RegularExpressions;
using TeaSharp.Core.Messages;
namespace TeaSharp.Tests;

internal static class PrebuiltWidgetTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("Controls_Label_RendersText", Label_RendersText);
        yield return new TestCase("Controls_Label_FocusedBorderStyleText_StylesFrameGlyphs", Label_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase("Controls_Button_ActivatesWhenFocused", Button_ActivatesWhenFocused);
        yield return new TestCase("Controls_Button_MouseClickActivatesAndTracksState", Button_MouseClickActivatesAndTracksState);
        yield return new TestCase("Controls_Button_ActivatedEvent_FiresOnActivation", Button_ActivatedEvent_FiresOnActivation);
        yield return new TestCase("Controls_Button_TryConsumeActivation_IsSingleUse", Button_TryConsumeActivation_IsSingleUse);
        yield return new TestCase("Controls_Button_RendersBorderedState", Button_RendersBorderedState);
        yield return new TestCase("Controls_Button_FocusedBorderStyleText_StylesFrameGlyphs", Button_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase("Controls_Button_MouseClickOnPadding_ActivatesWithinButtonBox", Button_MouseClickOnPadding_ActivatesWithinButtonBox);
        yield return new TestCase("Controls_Button_LabelChrome_CanBeRemoved", Button_LabelChrome_CanBeRemoved);
        yield return new TestCase("Controls_Button_SurfaceStyle_FillsPaddedInterior", Button_SurfaceStyle_FillsPaddedInterior);
        yield return new TestCase("Controls_Button_LabelStyles_DoNotCreateNestedBackgroundChrome", Button_LabelStyles_DoNotCreateNestedBackgroundChrome);
        yield return new TestCase("Controls_Button_Measure_UsesLongestLineAcrossLabelAndDescription", Button_Measure_UsesLongestLineAcrossLabelAndDescription);
        yield return new TestCase("Controls_Button_DisabledBorder_DoesNotBorrowLabelStyle", Button_DisabledBorder_DoesNotBorrowLabelStyle);
        yield return new TestCase("Controls_Button_CenteredLabel_DoesNotBreakFilledSurface", Button_CenteredLabel_DoesNotBreakFilledSurface);
        yield return new TestCase("Controls_Button_NarrowSurfaceShell_DropsChromeBeforeClippingLabel", Button_NarrowSurfaceShell_DropsChromeBeforeClippingLabel);
        yield return new TestCase("Controls_Button_CompactSurfaceShell_FallsBackToReadableFilledLabel", Button_CompactSurfaceShell_FallsBackToReadableFilledLabel);
        yield return new TestCase("Controls_Button_CompactRoundedSurface_ClampsPaddingToKeepLabelVisible", Button_CompactRoundedSurface_ClampsPaddingToKeepLabelVisible);
        yield return new TestCase("Controls_Button_RoundedSurfaceMode_InsetBody_RendersBorderAndInsetFill", Button_RoundedSurfaceMode_InsetBody_RendersBorderAndInsetFill);
        yield return new TestCase("Controls_Button_RoundedSurfaceMode_InsetBody_CompactHeight_FallsBackToReadableFilledLabel", Button_RoundedSurfaceMode_InsetBody_CompactHeight_FallsBackToReadableFilledLabel);
        yield return new TestCase("Controls_Button_RoundedSurfaceMode_InsetBody_DefaultChrome_UsesPlainLabelAndBreathingRoom", Button_RoundedSurfaceMode_InsetBody_DefaultChrome_UsesPlainLabelAndBreathingRoom);
        yield return new TestCase("Controls_Button_RoundedSurfaceMode_InsetBody_ReservesTallerAutoRoundedHeight", Button_RoundedSurfaceMode_InsetBody_ReservesTallerAutoRoundedHeight);
        yield return new TestCase("Controls_TextInput_SubmitsValue", TextInput_SubmitsValue);
        yield return new TestCase("Controls_TextInput_Events_ReportSubmitAndCancelValues", TextInput_Events_ReportSubmitAndCancelValues);
        yield return new TestCase("Controls_TextInput_TryConsumeSubmissionAndCancellation_AreSingleUse", TextInput_TryConsumeSubmissionAndCancellation_AreSingleUse);
        yield return new TestCase("Controls_TextInput_CancelSignalsAndCanClear", TextInput_CancelSignalsAndCanClear);
        yield return new TestCase("Controls_TextInput_HidesBorderWhenConfigured", TextInput_HidesBorderWhenConfigured);
        yield return new TestCase("Controls_TextInput_FocusMarkerAndBorderStyleHooks_Rendered", TextInput_FocusMarkerAndBorderStyleHooks_Rendered);
        yield return new TestCase("Controls_TextArea_RendersMultilineContent", TextArea_RendersMultilineContent);
        yield return new TestCase("Controls_TextArea_EnterInsertsNewline", TextArea_EnterInsertsNewline);
        yield return new TestCase("Controls_TextArea_FocusMarkerAndBorderStyleHooks_Rendered", TextArea_FocusMarkerAndBorderStyleHooks_Rendered);
        yield return new TestCase("Controls_Tabs_CycleAndSelectByNumber", Tabs_CycleAndSelectByNumber);
        yield return new TestCase("Controls_Tabs_ZeroShortcut_SelectsTenthTab", Tabs_ZeroShortcut_SelectsTenthTab);
        yield return new TestCase("Controls_Tabs_MouseClickSelectsTab", Tabs_MouseClickSelectsTab);
        yield return new TestCase("Controls_Tabs_MouseMotionDoesNotSelectHoveredTab", Tabs_MouseMotionDoesNotSelectHoveredTab);
        yield return new TestCase("Controls_Tabs_SelectionChangedEvent_ReportsTab", Tabs_SelectionChangedEvent_ReportsTab);
        yield return new TestCase("Controls_Breadcrumb_NavigatesSelection", Breadcrumb_NavigatesSelection);
        yield return new TestCase("Controls_Breadcrumb_MouseClickSelectsItem", Breadcrumb_MouseClickSelectsItem);
        yield return new TestCase("Controls_Breadcrumb_SelectionChangedEvent_ReportsTransition", Breadcrumb_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_ListView_NavigatesSelection", ListView_NavigatesSelection);
        yield return new TestCase("Controls_ListView_SelectionChangedEvent_ReportsTransition", ListView_SelectionChangedEvent_ReportsTransition);
        yield return new TestCase("Controls_ListView_MouseClickSelectsRow", ListView_MouseClickSelectsRow);
        yield return new TestCase("Controls_ListView_MouseClickRowWhitespace_SelectsRow", ListView_MouseClickRowWhitespace_SelectsRow);
        yield return new TestCase("Controls_ListView_MouseMotionShowsHoverMarker", ListView_MouseMotionShowsHoverMarker);
        yield return new TestCase("Controls_ListView_CustomRowMarkers_RenderCustomMarkers", ListView_CustomRowMarkers_RenderCustomMarkers);
        yield return new TestCase("Controls_ListView_FocusedBorderStyleText_StylesFrameGlyphs", ListView_FocusedBorderStyleText_StylesFrameGlyphs);
        yield return new TestCase("Controls_Choice_SelectsOpenMenuItem", Choice_SelectsOpenMenuItem);
        yield return new TestCase("Controls_Choice_SelectionChangedEvent_ReportsSelection", Choice_SelectionChangedEvent_ReportsSelection);
        yield return new TestCase("Controls_Choice_HidesBorderWhenConfigured", Choice_HidesBorderWhenConfigured);
        yield return new TestCase("Controls_Choice_MouseClickOpensAndSelects", Choice_MouseClickOpensAndSelects);
        yield return new TestCase("Controls_Choice_CustomGlyphSet_RendersCustomGlyphs", Choice_CustomGlyphSet_RendersCustomGlyphs);
        yield return new TestCase("Controls_ComboBox_FiltersAndSelects", ComboBox_FiltersAndSelects);
        yield return new TestCase("Controls_ComboBox_SelectionChangedEvent_ReportsSelection", ComboBox_SelectionChangedEvent_ReportsSelection);
        yield return new TestCase("Controls_ComboBox_MouseWheelNavigatesAndSelects", ComboBox_MouseWheelNavigatesAndSelects);
        yield return new TestCase("Controls_ComboBox_CustomGlyphSet_RendersCustomGlyphs", ComboBox_CustomGlyphSet_RendersCustomGlyphs);
        yield return new TestCase("Controls_MenuBar_ActivatesShortcut", MenuBar_ActivatesShortcut);
        yield return new TestCase("Controls_MenuBar_ItemActivatedEvent_ReportsItem", MenuBar_ItemActivatedEvent_ReportsItem);
        yield return new TestCase("Controls_MenuBar_TryConsumeActivation_IsSingleUse", MenuBar_TryConsumeActivation_IsSingleUse);
        yield return new TestCase("Controls_MenuBar_MouseClickActivatesItem", MenuBar_MouseClickActivatesItem);
        yield return new TestCase("Controls_MenuBar_MouseMotionDoesNotSelectHoveredItem", MenuBar_MouseMotionDoesNotSelectHoveredItem);
        yield return new TestCase("Controls_MenuBar_CustomGlyphsAndFocusedBorderStyleText_Rendered", MenuBar_CustomGlyphsAndFocusedBorderStyleText_Rendered);
        yield return new TestCase("Controls_CommandBar_KeyboardNavigationAndActivation", CommandBar_KeyboardNavigationAndActivation);
        yield return new TestCase("Controls_CommandBar_ItemActivatedEvent_ReportsItem", CommandBar_ItemActivatedEvent_ReportsItem);
        yield return new TestCase("Controls_CommandBar_MouseClickSelectsAndActivatesItem", CommandBar_MouseClickSelectsAndActivatesItem);
        yield return new TestCase("Controls_CommandBar_DisabledItemDoesNotActivate", CommandBar_DisabledItemDoesNotActivate);
        yield return new TestCase("Controls_CommandBar_FocusMarkerAndStyleHooks_Rendered", CommandBar_FocusMarkerAndStyleHooks_Rendered);
        yield return new TestCase("Controls_ContextMenu_ExecutesAndCloses", ContextMenu_ExecutesAndCloses);
        yield return new TestCase("Controls_ContextMenu_ItemExecutedEvent_ReportsItem", ContextMenu_ItemExecutedEvent_ReportsItem);
        yield return new TestCase("Controls_ContextMenu_TryConsumeExecution_IsSingleUse", ContextMenu_TryConsumeExecution_IsSingleUse);
        yield return new TestCase("Controls_ContextMenu_MouseClickExecutesAndCloses", ContextMenu_MouseClickExecutesAndCloses);
        yield return new TestCase("Controls_ContextMenu_MouseMotionDoesNotSelectOrExecute", ContextMenu_MouseMotionDoesNotSelectOrExecute);
        yield return new TestCase("Controls_ContextMenu_MouseReleaseWithoutLeftButton_DoesNotExecuteOrClose", ContextMenu_MouseReleaseWithoutLeftButton_DoesNotExecuteOrClose);
        yield return new TestCase("Controls_ContextMenu_MouseLeftReleaseExecutesAndCloses", ContextMenu_MouseLeftReleaseExecutesAndCloses);
        yield return new TestCase("Controls_ContextMenu_SetItems_RecomputesLayoutFromCachedWidths", ContextMenu_SetItems_RecomputesLayoutFromCachedWidths);
        yield return new TestCase("Controls_ContextMenu_CustomGlyphsFocusMarkerAndBorderStyleText_Rendered", ContextMenu_CustomGlyphsFocusMarkerAndBorderStyleText_Rendered);
        yield return new TestCase("Controls_ContextMenu_GlyphUpdate_RebuildsCachedRows", ContextMenu_GlyphUpdate_RebuildsCachedRows);
        yield return new TestCase("Controls_CommandPalette_FiltersAndExecutes", CommandPalette_FiltersAndExecutes);
        yield return new TestCase("Controls_CommandPalette_ItemExecutedEvent_ReportsItem", CommandPalette_ItemExecutedEvent_ReportsItem);
        yield return new TestCase("Controls_CommandPalette_TryConsumeExecution_IsSingleUse", CommandPalette_TryConsumeExecution_IsSingleUse);
        yield return new TestCase("Controls_CommandPalette_MouseClickExecutesSelection", CommandPalette_MouseClickExecutesSelection);
        yield return new TestCase("Controls_CommandPalette_ExposesQueryAccessors", CommandPalette_ExposesQueryAccessors);
        yield return new TestCase("Controls_CommandPalette_Open_ClearsQueryWhenClosed", CommandPalette_Open_ClearsQueryWhenClosed);
        yield return new TestCase("Controls_CommandPalette_LettersRemainQueryable", CommandPalette_LettersRemainQueryable);
        yield return new TestCase("Controls_CommandPalette_SetItems_RefreshesCachedRowsAndFilter", CommandPalette_SetItems_RefreshesCachedRowsAndFilter);
        yield return new TestCase("Controls_CommandPalette_QueryTransitions_KeepFilterAccurate", CommandPalette_QueryTransitions_KeepFilterAccurate);
        yield return new TestCase("Controls_CommandPalette_CustomGlyphsFocusMarkerAndBorderStyleText_Rendered", CommandPalette_CustomGlyphsFocusMarkerAndBorderStyleText_Rendered);
        yield return new TestCase("Controls_CommandPalette_GlyphUpdate_RebuildsCachedRows", CommandPalette_GlyphUpdate_RebuildsCachedRows);
        yield return new TestCase("Controls_FileExplorer_KeyboardNavigationAndExpansion", FileExplorer_KeyboardNavigationAndExpansion);
        yield return new TestCase("Controls_FileExplorer_SelectPathAndEvent_ReportsTransition", FileExplorer_SelectPathAndEvent_ReportsTransition);
        yield return new TestCase("Controls_FileExplorer_MouseClickSelectsRow", FileExplorer_MouseClickSelectsRow);
        yield return new TestCase("Controls_FileExplorer_RendersTitleAndStyleHooks", FileExplorer_RendersTitleAndStyleHooks);
        yield return new TestCase("Controls_FileExplorer_BorderStyleHooks_Rendered", FileExplorer_BorderStyleHooks_Rendered);
        yield return new TestCase("Controls_DataGrid_KeyboardNavigationTracksSelection", DataGrid_KeyboardNavigationTracksSelection);
        yield return new TestCase("Controls_DataGrid_SortComparerAndSortHook", DataGrid_SortComparerAndSortHook);
        yield return new TestCase("Controls_DataGrid_MouseClickSelectsRow", DataGrid_MouseClickSelectsRow);
        yield return new TestCase("Controls_DataGrid_CustomColumnSeparator_HitTestingAndRendering", DataGrid_CustomColumnSeparator_HitTestingAndRendering);
        yield return new TestCase("Controls_DataGrid_RendersTitleAndStyleHooks", DataGrid_RendersTitleAndStyleHooks);
        yield return new TestCase("Controls_DataGrid_UnstyledCells_ClearTrailingContentOnReusedCanvas", DataGrid_UnstyledCells_ClearTrailingContentOnReusedCanvas);
        yield return new TestCase("Controls_DiffView_ComputesLineEntries", DiffView_ComputesLineEntries);
        yield return new TestCase("Controls_DiffView_NavigatesSelectionAndTogglesMode", DiffView_NavigatesSelectionAndTogglesMode);
        yield return new TestCase("Controls_DiffView_MouseClickSelectsEntry", DiffView_MouseClickSelectsEntry);
        yield return new TestCase("Controls_DiffView_RendersStyledEntries", DiffView_RendersStyledEntries);
        yield return new TestCase("Controls_DiffView_BorderStyleHooks_Rendered", DiffView_BorderStyleHooks_Rendered);
        yield return new TestCase("Controls_Table_ForwardsSortHotkeys", Table_ForwardsSortHotkeys);
        yield return new TestCase("Controls_Table_FocusMarkerAndBorderStyleHooks_Rendered", Table_FocusMarkerAndBorderStyleHooks_Rendered);
        yield return new TestCase("Controls_ProgressBar_AdjustsValue", ProgressBar_AdjustsValue);
        yield return new TestCase("Controls_ProgressBar_FocusMarkerAndBorderStyleHooks_Rendered", ProgressBar_FocusMarkerAndBorderStyleHooks_Rendered);
        yield return new TestCase("Controls_StatusBar_RendersLeftAndRightText", StatusBar_RendersLeftAndRightText);
        yield return new TestCase("Controls_LogView_AppendsAndFilters", LogView_AppendsAndFilters);
        yield return new TestCase("Controls_Dialog_AcceptsAndDismisses", Dialog_AcceptsAndDismisses);
        yield return new TestCase("Controls_Dialog_Events_FirePerDecision", Dialog_Events_FirePerDecision);
        yield return new TestCase("Controls_Dialog_TryConsumeResult_IsSingleUse", Dialog_TryConsumeResult_IsSingleUse);
        yield return new TestCase("Controls_Dialog_Render_ClipsBackdropToRequestedRect", Dialog_Render_ClipsBackdropToRequestedRect);
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

    private static Task Label_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(55, 88, 144));
        var label = new Label
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = string.Empty,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(13, 16, 19)),
            FocusedBorderStyleText = focusedBorderStyle,
            Text = "content",
        };
        var canvas = new Canvas(24, 4, CanvasTextMode.GraphemeAware);

        label.Render(canvas, new Rect(0, 0, 24, 4));
        var output = canvas.Render();

        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "Label should style focused border glyphs.");
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

    private static Task Button_MouseClickOnPadding_ActivatesWithinButtonBox()
    {
        var button = new Button
        {
            Text = "Deploy",
            Border = BorderStyle.Rounded,
            Padding = Thickness.Symmetric(3, 1),
        };
        var bounds = new Rect(0, 0, 18, 5);

        var clickChanged = button.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);
        var releaseChanged = button.Handle(new PointerInput(PointerEventKind.Release, PointerButton.Left, 1, 1), bounds);

        TestAssert.True(clickChanged, "Mouse press inside the padded button box should activate the button.");
        TestAssert.True(releaseChanged, "Mouse release inside the padded button box should clear the pressed state.");
        TestAssert.Equal(1, button.ActivationCount, "Padding area should remain part of the clickable button box.");
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

    private static Task Button_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(128, 72, 44)).WithBold();
        var button = new Button
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(30, 30, 30)),
            FocusedBorderStyleText = focusedBorderStyle,
            Text = "Run",
        };
        var canvas = new Canvas(20, 5, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 20, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "Button should style focused border glyphs.");
        return Task.CompletedTask;
    }

    private static Task Button_LabelChrome_CanBeRemoved()
    {
        var button = new Button
        {
            Text = "Play",
            LabelPrefix = string.Empty,
            LabelSuffix = string.Empty,
            Border = BorderStyle.Rounded,
            Padding = Thickness.Symmetric(2, 1),
        };
        var canvas = new Canvas(18, 5, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 18, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Play", StringComparison.Ordinal), "Button should render plain label text when label chrome is disabled.");
        TestAssert.True(!output.Contains("[Play]", StringComparison.Ordinal), "Button should not force bracket chrome when prefix and suffix are empty.");
        TestAssert.True(output.Contains("╭", StringComparison.Ordinal), "Rounded button should render rounded shell glyphs.");
        return Task.CompletedTask;
    }

    private static Task Button_SurfaceStyle_FillsPaddedInterior()
    {
        var surfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(40, 30, 20));
        var borderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(210, 180, 150));
        var button = new Button
        {
            Text = "Go",
            Border = BorderStyle.Rounded,
            Padding = Thickness.Symmetric(2, 1),
            SurfaceStyle = surfaceStyle,
            BorderStyleText = borderStyle,
        };
        var canvas = new Canvas(16, 5, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 16, 5));
        var output = canvas.Render();
        var visibleOutput = StripAnsi(output);
        var shellStyle = borderStyle.Merge(surfaceStyle);

        TestAssert.True(output.Contains(surfaceStyle.Render("[Go]"), StringComparison.Ordinal), "Button surface style should keep the label row on the same filled surface as the rounded shell.");
        TestAssert.True(output.Contains(shellStyle.Render("▀"), StringComparison.Ordinal), "Filled rounded buttons should merge shell border styling with the surface fill.");
        TestAssert.True(output.Contains(shellStyle.Render("▌"), StringComparison.Ordinal), "Filled rounded buttons should merge side rails with the surface fill.");
        TestAssert.True(visibleOutput.Contains("▛▀▀▀▀▀▀▀▀▀▀▀▀▀▀▜", StringComparison.Ordinal), "Filled rounded buttons should render a filled top shell row.");
        TestAssert.True(visibleOutput.Contains("▌              ▐", StringComparison.Ordinal), "Filled rounded buttons should render filled interior rows between the shell rails.");
        TestAssert.True(visibleOutput.Contains("▌     [Go]     ▐", StringComparison.Ordinal), "Filled rounded buttons should keep the label centered inside the unified filled shell.");
        TestAssert.True(visibleOutput.Contains("▙▄▄▄▄▄▄▄▄▄▄▄▄▄▄▟", StringComparison.Ordinal), "Filled rounded buttons should render a filled bottom shell row.");
        return Task.CompletedTask;
    }

    private static Task Button_LabelStyles_DoNotCreateNestedBackgroundChrome()
    {
        var labelStyle = TeaStyle.Empty
            .WithForeground(AnsiColor.Rgb(230, 220, 210))
            .WithBackground(AnsiColor.Rgb(90, 40, 40))
            .WithBold();
        var expectedLabelStyle = TeaStyle.Empty
            .WithForeground(AnsiColor.Rgb(230, 220, 210))
            .WithBold();
        var surfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(30, 20, 20));
        var button = new Button
        {
            Text = "Play",
            LabelPrefix = string.Empty,
            LabelSuffix = string.Empty,
            Border = BorderStyle.Rounded,
            Padding = Thickness.Symmetric(2, 1),
            LabelStyle = labelStyle,
            SurfaceStyle = surfaceStyle,
        };
        var canvas = new Canvas(18, 5, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 18, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains(surfaceStyle.Merge(expectedLabelStyle).Render("Play"), StringComparison.Ordinal), "Button label should keep text styling while the surface owns the background.");
        TestAssert.True(!output.Contains(labelStyle.Render("Play"), StringComparison.Ordinal), "Button label should ignore nested background chrome from label styles.");
        return Task.CompletedTask;
    }

    private static Task Button_Measure_UsesLongestLineAcrossLabelAndDescription()
    {
        var button = new Button
        {
            Text = "Go",
            Description = "click or press enter",
            Border = BorderStyle.Rounded,
            Padding = Thickness.Symmetric(2, 1),
        };

        var measurement = button.Measure(new Rect(0, 0, 80, 10));
        var expectedWidth = "click or press enter".Length + button.Padding.Horizontal + 2;

        TestAssert.Equal(expectedWidth, measurement.Width, "Button measure should size to the widest rendered line.");
        return Task.CompletedTask;
    }

    private static Task Button_DisabledBorder_DoesNotBorrowLabelStyle()
    {
        var button = new Button
        {
            Text = "Run",
            IsDisabled = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(10, 20, 30)),
            DisabledLabelStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(200, 100, 50)).WithDim(),
        };
        var canvas = new Canvas(18, 5, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 18, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains(button.BorderStyleText.Render("┌"), StringComparison.Ordinal), "Disabled button should keep border-domain styling.");
        TestAssert.True(!output.Contains(button.DisabledLabelStyle.Render("┌"), StringComparison.Ordinal), "Disabled label style should not leak into the border shell.");
        return Task.CompletedTask;
    }

    private static Task Button_CenteredLabel_DoesNotBreakFilledSurface()
    {
        var surfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(30, 20, 20));
        var borderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(190, 180, 170));
        var button = new Button
        {
            Text = "Play",
            LabelPrefix = string.Empty,
            LabelSuffix = string.Empty,
            Border = BorderStyle.None,
            SurfaceStyle = surfaceStyle,
            BorderStyleText = borderStyle,
        };
        var measurement = button.Measure(new Rect(0, 0, 18, 5));
        var canvas = new Canvas(measurement.Width, measurement.Height, CanvasTextMode.GraphemeAware);

        TestAssert.Equal(8, measurement.Width, "Borderless surface-styled buttons should reserve symmetric chip width without example hints.");
        TestAssert.Equal(3, measurement.Height, "Borderless surface-styled buttons should reserve rounded-shell height when surface chrome is present.");

        button.Render(canvas, new Rect(0, 0, measurement.Width, measurement.Height));
        var output = canvas.Render();
        var visibleOutput = StripAnsi(output);
        var shellStyle = borderStyle.Merge(surfaceStyle);

        TestAssert.True(output.Contains(shellStyle.Render("▀"), StringComparison.Ordinal), "Surface-chromed borderless buttons should use the filled shell glyph contract.");
        TestAssert.True(visibleOutput.Contains("▛▀▀▀▀▀▀▜", StringComparison.Ordinal), "Surface-chromed borderless buttons should render a filled top shell row.");
        TestAssert.True(visibleOutput.Contains("▌ Play ▐", StringComparison.Ordinal), "Surface-chromed borderless buttons should keep centered labels inside the filled chip body.");
        TestAssert.True(visibleOutput.Contains("▙▄▄▄▄▄▄▟", StringComparison.Ordinal), "Surface-chromed borderless buttons should render a filled bottom shell row.");
        return Task.CompletedTask;
    }

    private static Task Button_NarrowSurfaceShell_DropsChromeBeforeClippingLabel()
    {
        var surfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(30, 20, 20));
        var labelStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(220, 210, 200)).WithBold();
        var button = new Button
        {
            Text = "Run",
            Border = BorderStyle.None,
            SurfaceStyle = surfaceStyle,
            LabelStyle = labelStyle,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(180, 150, 120)),
        };
        var canvas = new Canvas(5, 3, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 5, 3));
        var output = canvas.Render();
        var visibleOutput = StripAnsi(output);

        TestAssert.True(visibleOutput.Contains("▌Run▐", StringComparison.Ordinal), "Narrow rounded surface buttons should keep the readable label instead of clipping it behind default chrome.");
        TestAssert.True(!visibleOutput.Contains("[Run]", StringComparison.Ordinal), "Narrow rounded surface buttons should drop decorative chrome before it clips the label.");
        return Task.CompletedTask;
    }

    private static Task Button_CompactSurfaceShell_FallsBackToReadableFilledLabel()
    {
        var surfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(30, 20, 20));
        var button = new Button
        {
            Text = "Run",
            Border = BorderStyle.None,
            SurfaceStyle = surfaceStyle,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(180, 150, 120)),
        };
        var canvas = new Canvas(5, 2, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 5, 2));
        var output = canvas.Render();
        var visibleOutput = StripAnsi(output);

        TestAssert.True(output.Contains("Run", StringComparison.Ordinal), "Compact surface buttons should preserve readable text when the rounded shell lacks a dedicated middle row.");
        TestAssert.True(!visibleOutput.Contains('▛'), "Compact surface buttons should fall back to a filled label row instead of drawing an empty rounded shell.");
        return Task.CompletedTask;
    }

    private static Task Button_CompactRoundedSurface_ClampsPaddingToKeepLabelVisible()
    {
        var button = new Button
        {
            Text = "Run",
            Description = "r",
            Border = BorderStyle.Rounded,
            Padding = Thickness.All(1),
            SurfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(30, 20, 20)),
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(180, 150, 120)),
        };
        var canvas = new Canvas(8, 3, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 8, 3));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Run", StringComparison.Ordinal), "Compact rounded surface buttons should clamp vertical padding instead of losing the label row.");
        TestAssert.True(!output.Contains(" r ", StringComparison.Ordinal), "Compact rounded surface buttons should prefer the primary label when there is only room for one content row.");
        return Task.CompletedTask;
    }

    private static Task Button_RoundedSurfaceMode_InsetBody_RendersBorderAndInsetFill()
    {
        var surfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(40, 30, 20));
        var borderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(210, 180, 150));
        var button = new Button
        {
            Text = "Go",
            Border = BorderStyle.Rounded,
            Padding = Thickness.Symmetric(2, 1),
            SurfaceStyle = surfaceStyle,
            BorderStyleText = borderStyle,
            RoundedSurfaceMode = ButtonRoundedSurfaceMode.InsetBody,
        };
        var canvas = new Canvas(16, 5, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 16, 5));
        var output = canvas.Render();
        var visibleOutput = StripAnsi(output);

        TestAssert.True(output.Contains(borderStyle.Render("╭"), StringComparison.Ordinal), "Inset-body rounded buttons should keep a distinct border ring.");
        TestAssert.True(!output.Contains(surfaceStyle.Render("╭"), StringComparison.Ordinal), "Inset-body rounded buttons should not tint the border ring with the body fill.");
        TestAssert.True(output.Contains(surfaceStyle.Render("Go"), StringComparison.Ordinal), "Inset-body rounded buttons should still render the label on the filled inner body.");
        TestAssert.True(visibleOutput.Contains("╭──────────────╮", StringComparison.Ordinal), "Inset-body rounded buttons should render a pure rounded top border.");
        TestAssert.True(visibleOutput.Contains("│              │", StringComparison.Ordinal), "Inset-body rounded buttons should keep a full inner body between the border rails.");
        TestAssert.True(visibleOutput.Contains("│      Go      │", StringComparison.Ordinal), "Inset-body rounded buttons should center the label inside the filled body.");
        TestAssert.True(visibleOutput.Contains("╰──────────────╯", StringComparison.Ordinal), "Inset-body rounded buttons should render a pure rounded bottom border.");
        return Task.CompletedTask;
    }

    private static Task Button_RoundedSurfaceMode_InsetBody_CompactHeight_FallsBackToReadableFilledLabel()
    {
        var surfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(40, 30, 20));
        var button = new Button
        {
            Text = "Run",
            LabelPrefix = string.Empty,
            LabelSuffix = string.Empty,
            Border = BorderStyle.None,
            Padding = Thickness.Symmetric(1, 0),
            SurfaceStyle = surfaceStyle,
            RoundedSurfaceMode = ButtonRoundedSurfaceMode.InsetBody,
        };
        var canvas = new Canvas(7, 1, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, 7, 1));
        var output = canvas.Render();
        var visibleOutput = StripAnsi(output);

        TestAssert.True(output.Contains(surfaceStyle.Render(" "), StringComparison.Ordinal), "Compact inset-body buttons should keep the filled surface when the bordered shell cannot fit.");
        TestAssert.True(visibleOutput.Contains(" Run ", StringComparison.Ordinal), "Compact inset-body buttons should fall back to a readable filled label row.");
        TestAssert.True(!visibleOutput.Contains('╭'), "Compact inset-body buttons should not draw an empty border ring when there is no room for it.");
        return Task.CompletedTask;
    }

    private static Task Button_RoundedSurfaceMode_InsetBody_DefaultChrome_UsesPlainLabelAndBreathingRoom()
    {
        var button = new Button
        {
            Text = "Run",
            Border = BorderStyle.None,
            SurfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(40, 30, 20)),
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(210, 180, 150)),
            RoundedSurfaceMode = ButtonRoundedSurfaceMode.InsetBody,
        };

        var measurement = button.Measure(new Rect(0, 0, 18, 8));
        var canvas = new Canvas(measurement.Width, measurement.Height, CanvasTextMode.GraphemeAware);

        button.Render(canvas, new Rect(0, 0, measurement.Width, measurement.Height));
        var output = canvas.Render();
        var visibleOutput = StripAnsi(output);

        TestAssert.Equal(7, measurement.Width, "Inset-body rounded buttons should own minimum horizontal breathing room even when examples do not set padding.");
        TestAssert.Equal(5, measurement.Height, "Inset-body rounded buttons should keep the taller bordered-body contract.");
        TestAssert.True(visibleOutput.Contains("│ Run │", StringComparison.Ordinal), "Inset-body rounded buttons should use a plain centered label by default.");
        TestAssert.True(!visibleOutput.Contains("[Run]", StringComparison.Ordinal), "Inset-body rounded buttons should not require manual bracket-chrome removal.");
        return Task.CompletedTask;
    }

    private static Task Button_RoundedSurfaceMode_InsetBody_ReservesTallerAutoRoundedHeight()
    {
        var button = new Button
        {
            Text = "Play",
            LabelPrefix = string.Empty,
            LabelSuffix = string.Empty,
            Border = BorderStyle.None,
            SurfaceStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(30, 20, 20)),
            RoundedSurfaceMode = ButtonRoundedSurfaceMode.InsetBody,
        };

        var measurement = button.Measure(new Rect(0, 0, 18, 8));

        TestAssert.Equal(8, measurement.Width, "Inset-body surface buttons should preserve the shared symmetric chip width contract.");
        TestAssert.Equal(5, measurement.Height, "Inset-body surface buttons should reserve enough height for a bordered shell plus inset body.");
        return Task.CompletedTask;
    }

    private static string StripAnsi(string value)
    {
        return Regex.Replace(value, "\u001B\\[[0-9;]*m", string.Empty);
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

    private static Task TextInput_FocusMarkerAndBorderStyleHooks_Rendered()
    {
        var input = new TextInput
        {
            Title = "Input",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            FocusedBorderStyleText = TeaStyle.Empty.WithBold(),
            PlaceholderTextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
        };
        input.SetValue("abc");

        var focusedCanvas = new Canvas(24, 3, CanvasTextMode.GraphemeAware);
        input.Render(focusedCanvas, new Rect(0, 0, 24, 3));
        var focusedOutput = focusedCanvas.Render();

        TestAssert.True(focusedOutput.Contains("Input !", StringComparison.Ordinal), "Text input should render custom focus marker in title.");
        TestAssert.True(ContainsBoldSgr(focusedOutput), "Text input should merge focused border style into border glyph rendering.");
        TestAssert.True(ContainsBlueForegroundSgr(focusedOutput), "Text input should apply configured border color style.");

        input.IsFocused = false;
        input.IsDisabled = true;
        var disabledCanvas = new Canvas(24, 3, CanvasTextMode.GraphemeAware);
        input.Render(disabledCanvas, new Rect(0, 0, 24, 3));
        var disabledOutput = disabledCanvas.Render();

        TestAssert.True(ContainsMutedForegroundSgr(disabledOutput), "Text input disabled border should merge muted styling.");
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

    private static Task TextArea_FocusMarkerAndBorderStyleHooks_Rendered()
    {
        var borderStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue);
        var focusedBorderStyle = TeaStyle.Empty.WithBold();
        var mergedBorderStyle = borderStyle.Merge(focusedBorderStyle);
        var area = new TextArea
        {
            Title = "Notes",
            IsFocused = true,
            FocusMarker = "!",
            Border = BorderStyle.SingleLine,
            BorderStyleText = borderStyle,
            FocusedBorderStyleText = focusedBorderStyle,
            DisabledValueTextStyle = TeaStyle.Empty.WithDim(),
        };
        area.SetValue("line1\nline2");

        var focusedCanvas = new Canvas(30, 6, CanvasTextMode.GraphemeAware);
        area.Render(focusedCanvas, new Rect(0, 0, 30, 6));
        var focusedOutput = focusedCanvas.Render();

        TestAssert.True(focusedOutput.Contains("Notes !", StringComparison.Ordinal), "TextArea should render custom focus marker in title.");
        TestAssert.True(focusedOutput.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal), "TextArea should style focused border glyphs.");

        area.IsFocused = false;
        area.IsDisabled = true;
        var disabledCanvas = new Canvas(30, 6, CanvasTextMode.GraphemeAware);
        area.Render(disabledCanvas, new Rect(0, 0, 30, 6));
        var disabledOutput = disabledCanvas.Render();

        TestAssert.True(disabledOutput.Contains("\u001b[2;", StringComparison.Ordinal), "TextArea disabled border should include dim styling.");
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

    private static Task Tabs_MouseMotionDoesNotSelectHoveredTab()
    {
        var tabs = new Tabs("Overview", "Data", "Forms");

        var changed = tabs.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 15, 0), new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Tab hover should update hover state.");
        TestAssert.Equal(0, tabs.SelectedIndex, "Tab hover should not mutate selected tab.");
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

    private static Task Breadcrumb_NavigatesSelection()
    {
        var breadcrumb = new Breadcrumb
        {
            IsFocused = true,
        };
        breadcrumb.SetItems(
        [
            new BreadcrumbItem("home", "Home"),
            new BreadcrumbItem("projects", "Projects"),
            new BreadcrumbItem("build", "Build"),
        ]);

        breadcrumb.Handle(new KeyPressed(Key.End));
        breadcrumb.Handle(new KeyPressed(Key.Left));
        breadcrumb.Handle(new KeyPressed(Key.Home));
        breadcrumb.Handle(new KeyPressed(Key.Right));

        TestAssert.Equal(1, breadcrumb.SelectedIndex, "Breadcrumb keyboard navigation should handle Home/End/Left/Right transitions.");
        TestAssert.Equal("projects", breadcrumb.SelectedItem?.Id ?? string.Empty, "Breadcrumb keyboard navigation should select the expected item.");
        return Task.CompletedTask;
    }

    private static Task Breadcrumb_MouseClickSelectsItem()
    {
        var breadcrumb = new Breadcrumb();
        breadcrumb.SetItems(
        [
            new BreadcrumbItem("home", "Home"),
            new BreadcrumbItem("docs", "Docs"),
            new BreadcrumbItem("api", "API"),
        ]);

        var changed = breadcrumb.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 9, 0), new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Breadcrumb mouse click should select the clicked segment.");
        TestAssert.Equal(1, breadcrumb.SelectedIndex, "Breadcrumb mouse click should select the clicked item index.");
        TestAssert.Equal("docs", breadcrumb.SelectedItem?.Id ?? string.Empty, "Breadcrumb mouse click should select the clicked item.");
        return Task.CompletedTask;
    }

    private static Task Breadcrumb_SelectionChangedEvent_ReportsTransition()
    {
        var breadcrumb = new Breadcrumb
        {
            IsFocused = true,
        };
        breadcrumb.SetItems(
        [
            new BreadcrumbItem("home", "Home"),
            new BreadcrumbItem("projects", "Projects"),
            new BreadcrumbItem("build", "Build"),
        ]);

        BreadcrumbSelectionChangedEventArgs? args = null;
        breadcrumb.SelectionChanged += (_, eventArgs) => args = eventArgs;

        breadcrumb.Handle(new KeyPressed(Key.Right));

        TestAssert.True(args is not null, "Breadcrumb should raise selection changed when the selected item changes.");
        TestAssert.Equal(0, args!.PreviousIndex, "Breadcrumb event should expose previous index.");
        TestAssert.Equal(1, args.SelectedIndex, "Breadcrumb event should expose selected index.");
        TestAssert.Equal("home", args.PreviousItem?.Id ?? string.Empty, "Breadcrumb event should expose previous item.");
        TestAssert.Equal("projects", args.SelectedItem?.Id ?? string.Empty, "Breadcrumb event should expose selected item.");
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

    private static Task ListView_MouseClickRowWhitespace_SelectsRow()
    {
        var list = new ListView<string>(x => x)
        {
            Border = BorderStyle.None,
        };
        list.SetItems(["one", "two", "three"]);

        list.Handle(new KeyPressed(Key.Down));
        var changed = list.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 18, 2), new Rect(0, 0, 20, 3));

        TestAssert.True(changed, "List mouse click in trailing whitespace should select the clicked row.");
        TestAssert.Equal("three", list.SelectedItem ?? string.Empty, "List mouse click in trailing whitespace should select the row at the clicked Y coordinate.");
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

    private static Task ListView_CustomRowMarkers_RenderCustomMarkers()
    {
        var list = new ListView<string>(x => x)
        {
            Border = BorderStyle.None,
            RowMarkers = new ListViewMarkerSet(".", ">", "+"),
        };
        list.SetItems(["one", "two", "three"]);

        list.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 0, 1), new Rect(0, 0, 20, 3));
        var canvas = new Canvas(20, 3);

        list.Render(canvas, new Rect(0, 0, 20, 3));
        var output = canvas.Render();

        TestAssert.True(output.Contains("+ one", StringComparison.Ordinal), "Selected rows should render the custom selected marker.");
        TestAssert.True(output.Contains("> two", StringComparison.Ordinal), "Hovered rows should render the custom hovered marker.");
        TestAssert.True(output.Contains(". three", StringComparison.Ordinal), "Unselected rows should render the custom default marker.");
        return Task.CompletedTask;
    }

    private static Task ListView_FocusedBorderStyleText_StylesFrameGlyphs()
    {
        var focusedBorderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(123, 45, 67));
        var list = new ListView<string>(x => x)
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(11, 22, 33)),
            FocusedBorderStyleText = focusedBorderStyle,
            Title = string.Empty,
        };
        list.SetItems(["one"]);
        var canvas = new Canvas(16, 4, CanvasTextMode.GraphemeAware);

        list.Render(canvas, new Rect(0, 0, 16, 4));
        var output = canvas.Render();

        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "Focused border style should be applied to frame glyphs.");
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

        TestAssert.True(output.Contains("▾ alpha", StringComparison.Ordinal), "Dropdown should render selected item in borderless mode.");
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

    private static Task Choice_CustomGlyphSet_RendersCustomGlyphs()
    {
        var dropdown = new Choice
        {
            IsFocused = true,
            Border = BorderStyle.None,
            Glyphs = new DropdownGlyphSet("v", "^", ">", "+"),
        };
        dropdown.SetItems(["alpha", "beta", "gamma"]);
        dropdown.Handle(new KeyPressed(Key.Enter));

        var canvas = new Canvas(24, 6);
        dropdown.Render(canvas, new Rect(0, 0, 24, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("^ alpha", StringComparison.Ordinal), "Choice should render custom expanded indicator glyph.");
        TestAssert.True(output.Contains(">+ alpha", StringComparison.Ordinal), "Choice should render custom option marker glyphs.");
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

    private static Task ComboBox_CustomGlyphSet_RendersCustomGlyphs()
    {
        var combobox = new ComboBox
        {
            IsFocused = true,
            Border = BorderStyle.None,
            Glyphs = new DropdownGlyphSet("v", "^", ">", "+"),
        };
        combobox.SetItems(["alpha", "beta", "gamma"]);
        combobox.Handle(new KeyPressed(Key.Down));

        var canvas = new Canvas(24, 6);
        combobox.Render(canvas, new Rect(0, 0, 24, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("^ ", StringComparison.Ordinal), "ComboBox should render custom expanded indicator glyph.");
        TestAssert.True(output.Contains(">  alpha", StringComparison.Ordinal), "ComboBox should render custom highlighted marker glyph.");
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

    private static Task MenuBar_MouseMotionDoesNotSelectHoveredItem()
    {
        var menu = new MenuBar();
        menu.SetItems(
        [
            new MenuItem("file", "File", 'f'),
            new MenuItem("edit", "Edit", 'e'),
            new MenuItem("help", "Help", 'h'),
        ]);

        var changed = menu.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 12, 0), new Rect(0, 0, 40, 1));

        TestAssert.True(changed, "Menu hover should still update hover state.");
        TestAssert.Equal(0, menu.SelectedIndex, "Menu hover should not mutate selected item.");
        TestAssert.True(string.IsNullOrEmpty(menu.LastActivatedItemId), "Menu hover should not activate an item.");
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

    private static Task MenuBar_CustomGlyphsAndFocusedBorderStyleText_Rendered()
    {
        var focusedBorderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(85, 44, 21));
        var menu = new MenuBar
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Glyphs = new MenuBarGlyphSet("(", ")", " ", " ", "{", "}", "{", "}"),
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
            FocusedBorderStyleText = focusedBorderStyle,
        };
        menu.SetItems(
        [
            new MenuItem("file", "File", 'f'),
            new MenuItem("edit", "Edit", 'e'),
        ]);
        var canvas = new Canvas(40, 3, CanvasTextMode.GraphemeAware);

        menu.Render(canvas, new Rect(0, 0, 40, 3));
        var output = canvas.Render();

        TestAssert.True(output.Contains("(File{f})", StringComparison.Ordinal), "MenuBar should render selected labels using custom glyph delimiters.");
        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "MenuBar should style focused border glyphs.");
        return Task.CompletedTask;
    }

    private static Task CommandBar_KeyboardNavigationAndActivation()
    {
        var bar = new CommandBar
        {
            IsFocused = true,
        };
        bar.SetItems(
        [
            new CommandBarItem("build", "Build", 'b'),
            new CommandBarItem("test", "Test", 't'),
            new CommandBarItem("deploy", "Deploy", 'd'),
        ]);

        bar.Handle(new KeyPressed(Key.Right));
        bar.Handle(new KeyPressed(Key.End));
        bar.Handle(new KeyPressed(Key.Left));
        var activated = bar.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(activated, "Command bar enter should activate the selected command.");
        TestAssert.Equal(1, bar.SelectedIndex, "Command bar keyboard navigation should honor End/Left transitions.");
        TestAssert.Equal("test", bar.LastActivatedItemId ?? string.Empty, "Command bar should activate the selected command id.");
        return Task.CompletedTask;
    }

    private static Task CommandBar_ItemActivatedEvent_ReportsItem()
    {
        var bar = new CommandBar
        {
            IsFocused = true,
        };
        bar.SetItems(
        [
            new CommandBarItem("build", "Build", 'b'),
            new CommandBarItem("deploy", "Deploy", 'd'),
        ]);
        string? activated = null;
        bar.ItemActivated += (_, args) => activated = args.ItemId;

        bar.Handle(new KeyPressed(Key.Character, "d"));

        TestAssert.Equal("deploy", activated ?? string.Empty, "Command bar activation event should expose the activated command id.");
        return Task.CompletedTask;
    }

    private static Task CommandBar_MouseClickSelectsAndActivatesItem()
    {
        var bar = new CommandBar();
        bar.SetItems(
        [
            new CommandBarItem("build", "Build", 'b'),
            new CommandBarItem("test", "Test", 't'),
            new CommandBarItem("deploy", "Deploy", 'd'),
        ]);

        var changed = bar.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 13, 0), new Rect(0, 0, 60, 1));

        TestAssert.True(changed, "Command bar mouse click should select and activate the clicked command.");
        TestAssert.Equal(1, bar.SelectedIndex, "Command bar mouse click should select the clicked command index.");
        TestAssert.Equal("test", bar.LastActivatedItemId ?? string.Empty, "Command bar mouse click should activate the clicked command id.");
        return Task.CompletedTask;
    }

    private static Task CommandBar_DisabledItemDoesNotActivate()
    {
        var bar = new CommandBar
        {
            IsFocused = true,
        };
        bar.SetItems(
        [
            new CommandBarItem("build", "Build", 'b'),
            new CommandBarItem("deploy", "Deploy", 'd', IsDisabled: true),
        ]);

        var changed = bar.Handle(new KeyPressed(Key.Character, "d"));

        TestAssert.True(changed, "Command bar shortcut should still move selection to disabled command.");
        TestAssert.Equal(1, bar.SelectedIndex, "Command bar should select disabled command when shortcut matches.");
        TestAssert.True(bar.LastActivatedItemId is null, "Command bar should not activate disabled command.");
        return Task.CompletedTask;
    }

    private static Task CommandBar_FocusMarkerAndStyleHooks_Rendered()
    {
        var bar = new CommandBar
        {
            Title = "Cmd",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            SelectedPrefix = "<",
            SelectedSuffix = ">",
        };
        bar.SetItems(
        [
            new CommandBarItem("open", "Open", 'o'),
            new CommandBarItem("save", "Save", 's', IsDisabled: true),
        ]);
        var canvas = new Canvas(64, 1);

        bar.Render(canvas, new Rect(0, 0, 64, 1));
        var focusedOutput = canvas.Render();

        TestAssert.True(focusedOutput.Contains("Cmd !", StringComparison.Ordinal), "Command bar should render custom focus marker in title.");
        TestAssert.True(focusedOutput.Contains("<Open(o)>", StringComparison.Ordinal), "Command bar should render selected item with custom selection delimiters.");

        bar.ShowFocusMarker = false;
        var withoutMarkerCanvas = new Canvas(64, 1);
        bar.Render(withoutMarkerCanvas, new Rect(0, 0, 64, 1));
        var withoutMarker = withoutMarkerCanvas.Render();

        TestAssert.True(!withoutMarker.Contains('!'), "Command bar should allow hiding the focus marker.");

        bar.ItemStyle = TeaStyle.Empty.WithStrikethrough();
        bar.SelectedItemStyle = TeaStyle.Empty.WithBold();
        bar.DisabledItemStyle = TeaStyle.Empty.WithDim();
        var styledCanvas = new Canvas(64, 1);
        bar.Render(styledCanvas, new Rect(0, 0, 64, 1));
        var styledOutput = styledCanvas.Render();

        TestAssert.True(ContainsStrikethroughSgr(styledOutput), "Command bar should apply item style hooks during rendering.");
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

    private static Task ContextMenu_MouseMotionDoesNotSelectOrExecute()
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

        var changed = menu.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 0, 1), new Rect(0, 0, 20, 6));
        TestAssert.True(changed, "Context menu mouse motion should update hover state.");

        var canvas = new Canvas(20, 6);
        menu.Render(canvas, new Rect(0, 0, 20, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("> Copy", StringComparison.Ordinal), "Context menu motion should not change selection.");
        TestAssert.True(string.IsNullOrEmpty(menu.LastExecutedItemId), "Context menu motion should not execute actions.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_MouseReleaseWithoutLeftButton_DoesNotExecuteOrClose()
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

        TestAssert.True(changed, "Context menu release without left button may still update hover state.");
        TestAssert.True(string.IsNullOrEmpty(menu.LastExecutedItemId), "Context menu should not execute on non-left release.");
        TestAssert.True(menu.IsVisible, "Context menu should stay open on non-left release.");
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

    private static Task ContextMenu_MouseLeftReleaseExecutesAndCloses()
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

        var changed = menu.Handle(new PointerInput(PointerEventKind.Release, PointerButton.Left, 0, 1), new Rect(0, 0, 20, 6));

        TestAssert.True(changed, "Context menu left release should execute row action.");
        TestAssert.Equal("paste", menu.LastExecutedItemId ?? string.Empty, "Context menu left release should execute hovered item.");
        TestAssert.True(!menu.IsVisible, "Context menu should close after left-release execute.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_SetItems_RecomputesLayoutFromCachedWidths()
    {
        var menu = new ContextMenu
        {
            Border = BorderStyle.None,
        };
        menu.SetItems(
        [
            new ContextMenuItem("long", "This is a very long title that should stretch width"),
        ]);
        menu.OpenAt(18, 0);
        menu.SetItems(
        [
            new ContextMenuItem("short", "B"),
        ]);

        var canvas = new Canvas(20, 4);
        menu.Render(canvas, new Rect(0, 0, 20, 4));

        TestAssert.Equal(' ', canvas.Get(0, 0), "Recomputed width should keep short menu anchored near the right edge.");
        TestAssert.Equal('>', canvas.Get(8, 0), "Recomputed width should place selected row at updated x-offset.");
        TestAssert.True(canvas.Render().Contains("> B", StringComparison.Ordinal), "Context menu should render rows from the refreshed cache after SetItems.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_CustomGlyphsFocusMarkerAndBorderStyleText_Rendered()
    {
        var focusedBorderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(72, 33, 10));
        var menu = new ContextMenu
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            ShowFocusMarker = true,
            FocusMarker = "!",
            Glyphs = new ContextMenuGlyphSet(".", "▶", "~", ":"),
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(2, 3, 4)),
            FocusedBorderStyleText = focusedBorderStyle,
        };
        menu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
            new ContextMenuItem("paste", "Paste"),
        ]);
        menu.OpenAt(0, 0);

        var canvas = new Canvas(32, 8, CanvasTextMode.GraphemeAware);
        menu.Render(canvas, new Rect(0, 0, 32, 8));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Context", StringComparison.Ordinal) && output.Contains('!'), "ContextMenu should render the focus marker when enabled.");
        TestAssert.True(output.Contains("▶:Copy", StringComparison.Ordinal), "ContextMenu should render selected rows with custom marker glyphs.");
        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "ContextMenu should style focused border glyphs.");
        return Task.CompletedTask;
    }

    private static Task ContextMenu_GlyphUpdate_RebuildsCachedRows()
    {
        var menu = new ContextMenu
        {
            Border = BorderStyle.None,
        };
        menu.SetItems(
        [
            new ContextMenuItem("copy", "Copy"),
        ]);
        menu.OpenAt(0, 0);

        var beforeCanvas = new Canvas(24, 4);
        menu.Render(beforeCanvas, new Rect(0, 0, 24, 4));
        var before = beforeCanvas.Render();

        menu.Glyphs = new ContextMenuGlyphSet(".", "SEL", "HOV", "::");

        var afterCanvas = new Canvas(24, 4);
        menu.Render(afterCanvas, new Rect(0, 0, 24, 4));
        var after = afterCanvas.Render();

        TestAssert.True(before.Contains("> Copy", StringComparison.Ordinal), "ContextMenu should render default selected marker before glyph update.");
        TestAssert.True(after.Contains("SEL::Copy", StringComparison.Ordinal), "ContextMenu should rebuild cached row text when glyphs change.");
        TestAssert.True(!after.Contains("> Copy", StringComparison.Ordinal), "ContextMenu should not keep stale row marker cache after glyph update.");
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

    private static Task CommandPalette_SetItems_RefreshesCachedRowsAndFilter()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
        ]);
        palette.Open();

        palette.SetItems(
        [
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);
        palette.SetQueryText("roll");

        var canvas = new Canvas(80, 20);
        palette.Render(canvas, new Rect(0, 0, 80, 20));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Rollback - restore previous", StringComparison.Ordinal), "Command palette should render rows from the refreshed item cache.");
        TestAssert.True(!output.Contains("Deploy - publish release", StringComparison.Ordinal), "Command palette should not keep stale cached row text after SetItems.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_QueryTransitions_KeepFilterAccurate()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
            new CommandPaletteItem("run", "Run", "execute pipeline"),
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
        ]);
        palette.Open();

        palette.SetQueryText("ro");
        var narrowedCanvas = new Canvas(80, 20);
        palette.Render(narrowedCanvas, new Rect(0, 0, 80, 20));
        var narrowed = narrowedCanvas.Render();
        TestAssert.True(narrowed.Contains("Rollback - restore previous", StringComparison.Ordinal), "Narrowed query should keep matching command.");
        TestAssert.True(!narrowed.Contains("Run - execute pipeline", StringComparison.Ordinal), "Narrowed query should hide non-matching commands.");

        palette.SetQueryText("r");
        var expandedCanvas = new Canvas(80, 20);
        palette.Render(expandedCanvas, new Rect(0, 0, 80, 20));
        var expanded = expandedCanvas.Render();
        TestAssert.True(expanded.Contains("Rollback - restore previous", StringComparison.Ordinal), "Shrinking query should retain prior match.");
        TestAssert.True(expanded.Contains("Run - execute pipeline", StringComparison.Ordinal), "Shrinking query should restore broader matches.");

        palette.SetQueryText("de");
        var transitionedCanvas = new Canvas(80, 20);
        palette.Render(transitionedCanvas, new Rect(0, 0, 80, 20));
        var transitioned = transitionedCanvas.Render();
        TestAssert.True(transitioned.Contains("Deploy - publish release", StringComparison.Ordinal), "Non-prefix transition should rescan and find unrelated matches.");
        TestAssert.True(!transitioned.Contains("Rollback - restore previous", StringComparison.Ordinal), "Non-prefix transition should drop stale prefix-only matches.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_CustomGlyphsFocusMarkerAndBorderStyleText_Rendered()
    {
        var focusedBorderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(61, 14, 77));
        var palette = new CommandPalette
        {
            IsFocused = true,
            ShowFocusMarker = true,
            FocusMarker = "!",
            Border = BorderStyle.SingleLine,
            Glyphs = new CommandPaletteGlyphSet("?", ".", "*", "~", ":"),
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(12, 13, 14)),
            FocusedBorderStyleText = focusedBorderStyle,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
            new CommandPaletteItem("rollback", "Rollback", "restore previous"),
        ]);
        palette.Open();
        palette.SetQueryText("de");

        var canvas = new Canvas(80, 20, CanvasTextMode.GraphemeAware);
        palette.Render(canvas, new Rect(0, 0, 80, 20));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Command Palette !", StringComparison.Ordinal), "CommandPalette should render title focus marker when enabled.");
        TestAssert.True(output.Contains("?:de", StringComparison.Ordinal), "CommandPalette should render query prompt with custom glyphs.");
        TestAssert.True(output.Contains("*:Deploy - publish release", StringComparison.Ordinal), "CommandPalette should render selected row with custom marker glyphs.");
        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "CommandPalette should style focused border glyphs.");
        return Task.CompletedTask;
    }

    private static Task CommandPalette_GlyphUpdate_RebuildsCachedRows()
    {
        var palette = new CommandPalette
        {
            IsFocused = true,
        };
        palette.SetItems(
        [
            new CommandPaletteItem("deploy", "Deploy", "publish release"),
        ]);
        palette.Open();
        palette.SetQueryText("de");

        var beforeCanvas = new Canvas(80, 12);
        palette.Render(beforeCanvas, new Rect(0, 0, 80, 12));
        var before = beforeCanvas.Render();

        palette.Glyphs = new CommandPaletteGlyphSet("Q", ".", "SEL", "HOV", "::");

        var afterCanvas = new Canvas(80, 12);
        palette.Render(afterCanvas, new Rect(0, 0, 80, 12));
        var after = afterCanvas.Render();

        TestAssert.True(before.Contains("> Deploy - publish release", StringComparison.Ordinal), "CommandPalette should render default selected marker before glyph update.");
        TestAssert.True(after.Contains("SEL::Deploy - publish release", StringComparison.Ordinal), "CommandPalette should rebuild cached row text when glyphs change.");
        TestAssert.True(after.Contains("Q::de", StringComparison.Ordinal), "CommandPalette should render updated query prompt glyphs.");
        TestAssert.True(!after.Contains("> Deploy - publish release", StringComparison.Ordinal), "CommandPalette should not keep stale row marker cache after glyph update.");
        return Task.CompletedTask;
    }

    private static Task FileExplorer_KeyboardNavigationAndExpansion()
    {
        var src = new FileExplorerItem(
            "src",
            isDirectory: true,
            path: "/src",
            children:
            [
                new FileExplorerItem("app.cs", isDirectory: false, path: "/src/app.cs"),
            ])
        {
            IsExpanded = false,
        };
        var readme = new FileExplorerItem("README.md", isDirectory: false, path: "/README.md");
        var explorer = new FileExplorer
        {
            IsFocused = true,
        };
        explorer.SetItems([src, readme]);

        TestAssert.Equal("/src", explorer.SelectedPath ?? string.Empty, "FileExplorer should select first row by default.");

        var expanded = explorer.Handle(new KeyPressed(Key.Right));
        TestAssert.True(expanded, "FileExplorer Right should expand collapsed directory.");
        TestAssert.Equal("/src", explorer.SelectedPath ?? string.Empty, "FileExplorer should remain on directory row after expand.");

        var downChanged = explorer.Handle(new KeyPressed(Key.Down));
        TestAssert.True(downChanged, "FileExplorer Down should move to next visible row.");
        TestAssert.Equal("/src/app.cs", explorer.SelectedPath ?? string.Empty, "FileExplorer Down should move into expanded child row.");

        var leftChanged = explorer.Handle(new KeyPressed(Key.Left));
        TestAssert.True(leftChanged, "FileExplorer Left on child row should move to parent.");
        TestAssert.Equal("/src", explorer.SelectedPath ?? string.Empty, "FileExplorer Left should select parent row.");

        var collapsed = explorer.Handle(new KeyPressed(Key.Enter));
        TestAssert.True(collapsed, "FileExplorer Enter should toggle directory expansion.");
        TestAssert.Equal("/src", explorer.SelectedPath ?? string.Empty, "FileExplorer should keep selected directory after collapse.");
        return Task.CompletedTask;
    }

    private static Task FileExplorer_SelectPathAndEvent_ReportsTransition()
    {
        var explorer = new FileExplorer();
        explorer.SetItems(
        [
            new FileExplorerItem(
                "src",
                isDirectory: true,
                path: "/src",
                children:
                [
                    new FileExplorerItem("app.cs", isDirectory: false, path: "/src/app.cs"),
                ]),
        ]);
        FileExplorerSelectionChangedEventArgs? args = null;
        explorer.SelectionChanged += (_, eventArgs) => args = eventArgs;

        var changed = explorer.SelectPath("/src/app.cs");

        TestAssert.True(changed, "FileExplorer should select path when node exists.");
        TestAssert.True(args is not null, "FileExplorer should raise selection changed when selecting a path.");
        TestAssert.Equal("/src", args!.PreviousPath ?? string.Empty, "FileExplorer event should expose previous path.");
        TestAssert.Equal("/src/app.cs", args.CurrentPath ?? string.Empty, "FileExplorer event should expose current path.");
        TestAssert.True(!explorer.SelectPath("/missing"), "FileExplorer should not change selection for unknown path.");
        return Task.CompletedTask;
    }

    private static Task FileExplorer_MouseClickSelectsRow()
    {
        var explorer = new FileExplorer
        {
            Border = BorderStyle.None,
        };
        explorer.SetItems(
        [
            new FileExplorerItem(
                "src",
                isDirectory: true,
                path: "/src",
                children:
                [
                    new FileExplorerItem("app.cs", isDirectory: false, path: "/src/app.cs"),
                ]),
            new FileExplorerItem("README.md", isDirectory: false, path: "/README.md"),
        ]);

        var changed = explorer.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 2),
            new Rect(0, 0, 40, 4));

        TestAssert.True(changed, "FileExplorer click should select clicked row.");
        TestAssert.Equal("/README.md", explorer.SelectedPath ?? string.Empty, "FileExplorer click should select expected row path.");
        return Task.CompletedTask;
    }

    private static Task FileExplorer_RendersTitleAndStyleHooks()
    {
        var explorer = new FileExplorer
        {
            Title = "Files",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
            DirectoryStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            FileStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            SelectedStyle = TeaStyle.Empty.WithBold(),
        };
        explorer.SetItems(
        [
            new FileExplorerItem(
                "src",
                isDirectory: true,
                path: "/src",
                children:
                [
                    new FileExplorerItem("app.cs", isDirectory: false, path: "/src/app.cs"),
                ]),
        ]);
        explorer.SelectPath("/src/app.cs");
        var canvas = new Canvas(48, 6);

        explorer.Render(canvas, new Rect(0, 0, 48, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Files !", StringComparison.Ordinal), "FileExplorer should render focused title marker.");
        TestAssert.True(output.Contains("src", StringComparison.Ordinal), "FileExplorer should render directory rows.");
        TestAssert.True(output.Contains("app.cs", StringComparison.Ordinal), "FileExplorer should render file rows.");
        var hasDirectoryColor = output.Contains("\u001b[38;5;14m", StringComparison.Ordinal)
            || output.Contains("\u001b[96m", StringComparison.Ordinal);
        var hasFileColor = output.Contains("\u001b[38;5;10m", StringComparison.Ordinal)
            || output.Contains(";5;10m", StringComparison.Ordinal)
            || output.Contains("\u001b[92m", StringComparison.Ordinal)
            || output.Contains(";92m", StringComparison.Ordinal)
            || output.Contains("\u001b[32m", StringComparison.Ordinal)
            || output.Contains(";32m", StringComparison.Ordinal);
        TestAssert.True(hasDirectoryColor, "FileExplorer should emit directory style fragments.");
        TestAssert.True(hasFileColor, "FileExplorer should emit file style fragments.");
        return Task.CompletedTask;
    }

    private static Task FileExplorer_BorderStyleHooks_Rendered()
    {
        var explorer = new FileExplorer
        {
            Title = "Files",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            FocusedBorderStyleText = TeaStyle.Empty.WithBold(),
            MutedStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlack),
        };
        explorer.SetItems([new FileExplorerItem("readme.md", isDirectory: false, path: "/readme.md")]);

        var focusedCanvas = new Canvas(36, 5, CanvasTextMode.GraphemeAware);
        explorer.Render(focusedCanvas, new Rect(0, 0, 36, 5));
        var focusedOutput = focusedCanvas.Render();

        TestAssert.True(focusedOutput.Contains("Files !", StringComparison.Ordinal), "FileExplorer should render custom focus marker in title.");
        TestAssert.True(ContainsBoldSgr(focusedOutput), "FileExplorer should merge focused border style into border glyph rendering.");
        TestAssert.True(ContainsBlueForegroundSgr(focusedOutput), "FileExplorer should apply configured border color style.");

        explorer.IsFocused = false;
        explorer.IsDisabled = true;
        var disabledCanvas = new Canvas(36, 5, CanvasTextMode.GraphemeAware);
        explorer.Render(disabledCanvas, new Rect(0, 0, 36, 5));
        var disabledOutput = disabledCanvas.Render();

        TestAssert.True(ContainsMutedForegroundSgr(disabledOutput), "FileExplorer disabled border should merge muted styling.");
        return Task.CompletedTask;
    }

    private static Task DiffView_ComputesLineEntries()
    {
        var diff = new DiffView();
        diff.SetTexts("alpha\nbeta\ngamma", "alpha\ngamma\ndelta");

        var entries = diff.Entries;
        TestAssert.Equal(4, entries.Count, "Diff view should produce line-level entries for unchanged/removed/added lines.");
        TestAssert.Equal((int)DiffLineKind.Unchanged, (int)entries[0].Kind, "First line should be unchanged.");
        TestAssert.Equal((int)DiffLineKind.Removed, (int)entries[1].Kind, "Second line should be removed.");
        TestAssert.Equal((int)DiffLineKind.Unchanged, (int)entries[2].Kind, "Third line should align back to unchanged.");
        TestAssert.Equal((int)DiffLineKind.Added, (int)entries[3].Kind, "Trailing new line should be added.");
        TestAssert.Equal("beta", entries[1].OldText, "Removed entry should keep old text.");
        TestAssert.Equal("delta", entries[3].NewText, "Added entry should keep new text.");
        return Task.CompletedTask;
    }

    private static Task DiffView_NavigatesSelectionAndTogglesMode()
    {
        var diff = new DiffView
        {
            IsFocused = true,
        };
        diff.SetTexts("one\ntwo\nthree", "one\nthree\nfour");

        diff.Handle(new KeyPressed(Key.Down));
        diff.Handle(new KeyPressed(Key.End));
        diff.Handle(new KeyPressed(Key.Up));
        var toggled = diff.Handle(new KeyPressed(Key.Tab));

        TestAssert.Equal(2, diff.SelectedIndex, "Diff view keyboard navigation should move and clamp selected entry.");
        TestAssert.True(toggled, "Diff view tab should toggle render mode.");
        TestAssert.Equal((int)DiffViewMode.SideBySide, (int)diff.Mode, "Diff view tab should switch to side-by-side mode.");
        return Task.CompletedTask;
    }

    private static Task DiffView_MouseClickSelectsEntry()
    {
        var diff = new DiffView
        {
            Border = BorderStyle.None,
        };
        diff.SetTexts("a\nb\nc\nd", "a\nc\nd\ne");

        var changed = diff.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 0, 2), new Rect(0, 0, 40, 6));

        TestAssert.True(changed, "Diff view click should select the clicked entry row.");
        TestAssert.Equal(1, diff.SelectedIndex, "Diff view click should select the expected entry index after header offset.");
        return Task.CompletedTask;
    }

    private static Task DiffView_RendersStyledEntries()
    {
        var diff = new DiffView
        {
            Border = BorderStyle.None,
            AddedLineStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            RemovedLineStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightRed),
            UnchangedLineStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightWhite),
            SelectedLineStyle = TeaStyle.Empty.WithBold(),
            HeaderStyle = TeaStyle.Empty.WithUnderline(),
        };
        diff.SetTexts("a\nb\nc", "a\nc\nd");
        var canvas = new Canvas(48, 6);

        diff.Render(canvas, new Rect(0, 0, 48, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Old -> New", StringComparison.Ordinal), "Diff view should render header.");
        TestAssert.True(output.Contains('+'), "Diff view should render added line marker.");
        TestAssert.True(output.Contains('-'), "Diff view should render removed line marker.");
        TestAssert.True(output.Contains("\u001b[4m", StringComparison.Ordinal) || output.Contains(";4m", StringComparison.Ordinal), "Diff view should apply header style.");
        return Task.CompletedTask;
    }

    private static Task DiffView_BorderStyleHooks_Rendered()
    {
        var diff = new DiffView
        {
            Title = "Diff",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            Border = BorderStyle.SingleLine,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            FocusedBorderStyleText = TeaStyle.Empty.WithBold(),
        };
        diff.SetTexts("one\ntwo", "one\nthree");

        var canvas = new Canvas(48, 6, CanvasTextMode.GraphemeAware);
        diff.Render(canvas, new Rect(0, 0, 48, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Diff !", StringComparison.Ordinal), "DiffView should render custom focus marker in title.");
        TestAssert.True(ContainsBoldSgr(output), "DiffView should merge focused border style into border glyph rendering.");
        TestAssert.True(ContainsBlueForegroundSgr(output), "DiffView should apply configured border color style.");
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

    private static Task Table_FocusMarkerAndBorderStyleHooks_Rendered()
    {
        var table = new Table("Name", "State")
        {
            Title = "T",
            IsFocused = true,
            FocusMarker = "!",
            ShowFocusMarker = true,
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue),
            FocusedBorderStyleText = TeaStyle.Empty.WithBold(),
        };
        table.SetRows(
        [
            ["svc-a", "ok"],
            ["svc-b", "warn"],
        ]);

        var canvas = new Canvas(36, 6, CanvasTextMode.GraphemeAware);
        table.Render(canvas, new Rect(0, 0, 36, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("T !", StringComparison.Ordinal), "Table should render custom focus marker in title.");
        TestAssert.True(ContainsBoldSgr(output), "Table should merge focused border style into border glyph rendering.");
        TestAssert.True(ContainsBlueForegroundSgr(output), "Table should apply configured border color style.");
        return Task.CompletedTask;
    }

    private static Task DataGrid_KeyboardNavigationTracksSelection()
    {
        var grid = new DataGrid
        {
            IsFocused = true,
            Border = BorderStyle.None,
            PageSize = 2,
        };
        grid.SetColumns(
        [
            new DataGridColumn("name", "Name"),
            new DataGridColumn("status", "Status"),
        ]);
        grid.SetRows(
        [
            ["A", "Open"],
            ["B", "Done"],
            ["C", "Todo"],
        ]);

        grid.Handle(new KeyPressed(Key.Right));
        grid.Handle(new KeyPressed(Key.Down));
        grid.Handle(new KeyPressed(Key.PageDown));
        grid.Handle(new KeyPressed(Key.Home));
        grid.Handle(new KeyPressed(Key.End));

        TestAssert.Equal(2, grid.SelectedRowIndex, "DataGrid End should move to last row.");
        TestAssert.Equal(1, grid.SelectedColumnIndex, "DataGrid End should move to last column.");
        TestAssert.Equal("Todo", grid.SelectedCellValue ?? string.Empty, "DataGrid should expose selected cell value.");
        return Task.CompletedTask;
    }

    private static Task DataGrid_SortComparerAndSortHook()
    {
        var grid = new DataGrid
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        grid.SetColumns(
        [
            new DataGridColumn("value", "Value")
            {
                IsSortable = true,
                SortComparer = static (left, right) => string.CompareOrdinal(left, right),
            },
        ]);
        grid.SetRows(
        [
            ["b"],
            ["a"],
        ]);

        var sortedAscending = grid.SortByColumn(0);
        var sortedDescending = grid.SortByColumn(0);

        TestAssert.True(sortedAscending, "DataGrid should sort ascending using column comparer.");
        TestAssert.True(sortedDescending, "DataGrid should toggle and sort descending on repeat sort request.");
        TestAssert.Equal("b", grid.Rows[0][0], "DataGrid descending sort should place lexicographically larger value first.");

        var hookGrid = new DataGrid
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };
        hookGrid.SetColumns(
        [
            new DataGridColumn("score", "Score")
            {
                IsSortable = true,
            },
        ]);
        hookGrid.SetRows(
        [
            ["2"],
            ["1"],
        ]);

        DataGridSortRequestedEventArgs? eventArgs = null;
        hookGrid.SortRequested += (_, args) =>
        {
            eventArgs = args;
            args.Handled = true;
        };

        var handledByHook = hookGrid.SortByColumn(0);
        TestAssert.True(handledByHook, "DataGrid should allow external sort hook handling when comparer is not provided.");
        TestAssert.True(eventArgs is not null, "DataGrid should raise sort request event.");
        TestAssert.Equal(0, eventArgs!.ColumnIndex, "DataGrid sort event should expose requested column.");
        return Task.CompletedTask;
    }

    private static Task DataGrid_MouseClickSelectsRow()
    {
        var grid = new DataGrid
        {
            Border = BorderStyle.None,
            IsFocused = true,
        };
        grid.SetColumns(
        [
            new DataGridColumn("name", "Name"),
            new DataGridColumn("status", "Status"),
        ]);
        grid.SetRows(
        [
            ["A", "Open"],
            ["B", "Done"],
            ["C", "Todo"],
        ]);

        var changed = grid.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2),
            new Rect(0, 0, 32, 4));

        TestAssert.True(changed, "DataGrid click should update selected row.");
        TestAssert.Equal(1, grid.SelectedRowIndex, "DataGrid click should select the row under pointer.");
        return Task.CompletedTask;
    }

    private static Task DataGrid_CustomColumnSeparator_HitTestingAndRendering()
    {
        var grid = new DataGrid
        {
            Border = BorderStyle.None,
            IsFocused = true,
            ColumnSeparatorText = " || ",
        };
        grid.SetColumns(
        [
            new DataGridColumn("name", "Name"),
            new DataGridColumn("status", "Status"),
        ]);
        grid.SetRows(
        [
            ["A", "Open"],
        ]);

        var changed = grid.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 10, 1),
            new Rect(0, 0, 30, 3));
        var canvas = new Canvas(30, 3);
        grid.Render(canvas, new Rect(0, 0, 30, 3));
        var output = canvas.Render();

        TestAssert.True(changed, "DataGrid click should handle selection with custom column separator width.");
        TestAssert.Equal(1, grid.SelectedColumnIndex, "DataGrid click should hit-test into second column with custom separator width.");
        TestAssert.True(output.Contains(" || ", StringComparison.Ordinal), "DataGrid should render custom column separator text.");
        return Task.CompletedTask;
    }

    private static Task DataGrid_RendersTitleAndStyleHooks()
    {
        var borderStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightBlue);
        var focusedBorderStyle = TeaStyle.Empty.WithBold();
        var mergedBorderStyle = borderStyle.Merge(focusedBorderStyle);
        var grid = new DataGrid
        {
            Title = "Grid",
            IsFocused = true,
            FocusMarker = "!",
            Border = BorderStyle.SingleLine,
            FocusedTitleStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightMagenta),
            HeaderStyle = TeaStyle.Empty.WithUnderline().WithForeground(AnsiColor.BrightYellow),
            RowStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
            SelectedRowStyle = TeaStyle.Empty.WithBold(),
            SelectedCellStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            MutedStyle = TeaStyle.Empty.WithDim(),
            BorderStyleText = borderStyle,
            FocusedBorderStyleText = focusedBorderStyle,
            ColumnSeparatorText = "¦",
            SortAscendingMarker = "^",
            MutedRowPredicate = static (rowIndex, _) => rowIndex == 0,
        };
        grid.SetColumns(
        [
            new DataGridColumn("name", "Name")
            {
                IsSortable = true,
                SortComparer = static (left, right) => string.CompareOrdinal(left, right),
            },
            new DataGridColumn("status", "Status"),
        ]);
        grid.SetRows(
        [
            ["A", "Open"],
            ["B", "Done"],
        ]);
        grid.SelectCell(1, 1);
        grid.SortByColumn(0, DataGridSortDirection.Ascending);
        var canvas = new Canvas(48, 6, CanvasTextMode.GraphemeAware);

        grid.Render(canvas, new Rect(0, 0, 48, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Grid !", StringComparison.Ordinal), "DataGrid should render focused title marker.");
        TestAssert.True(output.Contains("Name ^", StringComparison.Ordinal), "DataGrid should render custom sort marker for sorted headers.");
        TestAssert.True(output.Contains('¦'), "DataGrid should render custom column separator text.");
        TestAssert.True(output.Contains(mergedBorderStyle.Render("┌"), StringComparison.Ordinal), "DataGrid should style focused border glyphs.");
        TestAssert.True(output.Contains("\u001b[4;38;5;13m", StringComparison.Ordinal), "DataGrid should render focused title style.");
        TestAssert.True(output.Contains("\u001b[4;38;5;11m", StringComparison.Ordinal), "DataGrid should render header style.");
        TestAssert.True(output.Contains("\u001b[1;38;5;10m", StringComparison.Ordinal), "DataGrid should render selected cell style.");
        TestAssert.True(output.Contains("\u001b[2;38;5;14m", StringComparison.Ordinal), "DataGrid should render muted row style.");
        return Task.CompletedTask;
    }

    private static Task DataGrid_UnstyledCells_ClearTrailingContentOnReusedCanvas()
    {
        var grid = new DataGrid
        {
            Border = BorderStyle.None,
            ShowHeader = false,
        };
        grid.SetColumns(
        [
            new DataGridColumn("name", "Name")
            {
                Width = 8,
            },
        ]);
        grid.SetRows(
        [
            ["LONGTEXT"],
        ]);

        var canvas = new Canvas(12, 2);
        grid.Render(canvas, new Rect(0, 0, 12, 2));

        grid.SetRows(
        [
            ["A"],
        ]);
        grid.Render(canvas, new Rect(0, 0, 12, 2));

        var firstLine = canvas.Render().Split('\n')[0];
        TestAssert.True(
            firstLine.StartsWith("A       ", StringComparison.Ordinal),
            "Unstyled DataGrid cells should clear trailing content when new value is shorter.");
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

    private static Task ProgressBar_FocusMarkerAndBorderStyleHooks_Rendered()
    {
        var focusedBorderStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(84, 109, 60)).WithBold();
        var progress = new ProgressBar
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            Title = "Load",
            FocusMarker = "!",
            BorderStyleText = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(7, 7, 7)),
            FocusedBorderStyleText = focusedBorderStyle,
        };
        progress.SetValue(0.4);
        var canvas = new Canvas(28, 5, CanvasTextMode.GraphemeAware);

        progress.Render(canvas, new Rect(0, 0, 28, 5));
        var output = canvas.Render();

        TestAssert.True(output.Contains("Load !", StringComparison.Ordinal), "ProgressBar should render custom focus marker when focused.");
        TestAssert.True(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), "ProgressBar should style focused border glyphs.");
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

    private static Task Dialog_Render_ClipsBackdropToRequestedRect()
    {
        var canvas = new Canvas(12, 6);
        canvas.Clear('#');
        var dialog = new Dialog
        {
            IsVisible = true,
            IsFocused = true,
            Title = "Confirm",
            BodyLines = ["Apply?"],
        };

        dialog.Render(canvas, new Rect(2, 1, 8, 4));

        TestAssert.Equal('#', canvas.Get(0, 0), "Dialog should not mutate cells outside the requested rect.");
        TestAssert.Equal('#', canvas.Get(11, 5), "Dialog should preserve content outside the clipped backdrop bounds.");
        TestAssert.Equal('·', canvas.Get(2, 1), "Dialog should fill backdrop cells inside the requested rect.");
        TestAssert.Equal('#', canvas.Get(1, 1), "Dialog backdrop fill should remain clipped and not bleed left.");
        return Task.CompletedTask;
    }

    private static bool ContainsStrikethroughSgr(string value)
    {
        return value.Contains("\u001b[9m", StringComparison.Ordinal)
            || value.Contains("\u001b[2;9m", StringComparison.Ordinal)
            || value.Contains(";9m", StringComparison.Ordinal)
            || value.Contains(";9;", StringComparison.Ordinal)
            || value.Contains("[9;", StringComparison.Ordinal);
    }

    private static bool ContainsBoldSgr(string value)
    {
        return value.Contains("\u001b[1m", StringComparison.Ordinal)
            || value.Contains(";1m", StringComparison.Ordinal)
            || value.Contains("[1;", StringComparison.Ordinal)
            || value.Contains(";1;", StringComparison.Ordinal);
    }

    private static bool ContainsBlueForegroundSgr(string value)
    {
        return value.Contains("\u001b[94m", StringComparison.Ordinal)
            || value.Contains(";94m", StringComparison.Ordinal)
            || value.Contains("\u001b[38;5;12m", StringComparison.Ordinal)
            || value.Contains(";5;12m", StringComparison.Ordinal)
            || value.Contains("\u001b[34m", StringComparison.Ordinal)
            || value.Contains(";34m", StringComparison.Ordinal);
    }

    private static bool ContainsMutedForegroundSgr(string value)
    {
        return value.Contains("\u001b[90m", StringComparison.Ordinal)
            || value.Contains(";90m", StringComparison.Ordinal)
            || value.Contains("\u001b[38;5;8m", StringComparison.Ordinal)
            || value.Contains(";5;8m", StringComparison.Ordinal)
            || value.Contains("\u001b[30m", StringComparison.Ordinal)
            || value.Contains(";30m", StringComparison.Ordinal);
    }
}
