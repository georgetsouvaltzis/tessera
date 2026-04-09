using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class SearchResultsViewControlTests
{
    [Test]
    public void ControlsSearchResultsViewRendersRankMatchAndSelectionMarkers()
    {
        var control = new SearchResultsView
        {
            Border = BorderStyle.None,
            Query = "foo",
            Glyphs = new SearchResultsGlyphSet("·", "▸", "▶", "~", "."),
            ShowRankMarker = true,
        };
        control.SetItems(["foo result", "bar result"]);
        var canvas = new Canvas(48, 4);

        control.Render(canvas, new Rect(0, 0, 48, 4));
        var output = canvas.Render();

        TestAssert.True(output.Contains("▶ 1. ~ foo result", StringComparison.Ordinal), "Selected row should render selected marker, rank, and match marker.");
        TestAssert.True(output.Contains("· 2. bar result", StringComparison.Ordinal), "Default row should render default marker and rank.");
    }

    [Test]
    public void ControlsSearchResultsViewKeyboardNavigationUpdatesSelectionAndRaisesEvent()
    {
        var control = new SearchResultsView
        {
            IsFocused = true,
        };
        control.SetItems(["alpha", "beta", "gamma"]);
        var changes = 0;
        control.SelectionChanged += (_, _) => changes++;

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var endHandled = control.Handle(new KeyPressed(Key.End));
        var upHandled = control.Handle(new KeyPressed(Key.Up));

        TestAssert.True(downHandled, "Down key should be handled while focused.");
        TestAssert.True(endHandled, "End key should be handled while focused.");
        TestAssert.True(upHandled, "Up key should be handled while focused.");
        TestAssert.Equal(1, control.SelectedIndex, "Selection should end at the expected index after key sequence.");
        TestAssert.Equal(3, changes, "Each selection transition should raise SelectionChanged.");
    }

    [Test]
    public void ControlsSearchResultsViewDisabledPointerInputDoesNotHoverOrSelect()
    {
        var control = new SearchResultsView
        {
            Border = BorderStyle.None,
            IsDisabled = true,
            HoveredRowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(91, 92, 93)),
        };
        control.SetItems(["one", "two"]);
        var bounds = new Rect(0, 0, 32, 4);

        var motionHandled = control.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1),
            bounds);
        var pressHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1),
            bounds);

        var canvas = new Canvas(32, 4, CanvasTextMode.GraphemeAware);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(!motionHandled, "Disabled control should ignore hover motion.");
        TestAssert.True(!pressHandled, "Disabled control should ignore pointer press.");
        TestAssert.True(!output.Contains("38;2;91;92;93", StringComparison.Ordinal), "Disabled pointer input should not apply hovered style.");
        TestAssert.Equal(0, control.SelectedIndex, "Disabled pointer input should not change selection.");
    }

    [Test]
    public void ControlsSearchResultsViewStateStylesApplyFocusedHoveredPressedAndError()
    {
        var control = new SearchResultsView
        {
            Border = BorderStyle.None,
            IsFocused = true,
            HasError = true,
            DefaultRowStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(1, 2, 3)),
            HoveredRowStyle = TesseraStyle.Empty.WithBold(),
            SelectedRowStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(21, 22, 23)),
            FocusedSelectedRowStyle = TesseraStyle.Empty.WithItalic(),
            PressedRowStyle = TesseraStyle.Empty.WithUnderline(),
            ErrorRowStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
        };
        control.SetItems(["first", "second"]);
        var bounds = new Rect(0, 0, 40, 4);

        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 0), bounds);
        _ = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 0), bounds);

        var canvas = new Canvas(40, 4, CanvasTextMode.GraphemeAware);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.True(output.Contains("48;2;1;2;3", StringComparison.Ordinal), "Default style should be present.");
        TestAssert.True(output.Contains("[1;", StringComparison.Ordinal), "Hovered style should be present.");
        TestAssert.True(output.Contains("48;2;21;22;23", StringComparison.Ordinal), "Selected style should be present.");
        TestAssert.True(output.Contains(";3m", StringComparison.Ordinal), "Focused-selected style should be present.");
        TestAssert.True(
            output.Contains(";4;", StringComparison.Ordinal) || output.Contains("[4m", StringComparison.Ordinal),
            "Pressed style should be present.");
        TestAssert.True(output.Contains("38;2;31;32;33", StringComparison.Ordinal), "Error style should be present.");
    }

    [Test]
    public void ControlsSearchResultsViewFocusedBorderMergesBaseAndFocusedStyles()
    {
        var control = new SearchResultsView
        {
            IsFocused = true,
            Border = BorderStyle.Single,
            BorderStyleText = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(9, 8, 7)),
            FocusedBorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
        };
        control.SetItems(["alpha"]);

        var canvas = new Canvas(24, 4, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, 24, 4));
        var output = canvas.Render();

        TestAssert.True(output.Contains("48;2;9;8;7", StringComparison.Ordinal), "Focused border should preserve base border style.");
        TestAssert.True(output.Contains("38;2;1;2;3", StringComparison.Ordinal), "Focused border should merge focused border style.");
    }

    [Test]
    public void ThemeSearchResultsViewApplyAndDefaultsMapExpectedTokensAndPreserveExplicitStyles()
    {
        var theme = new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(1, 2, 3)),
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(4, 5, 6)),
                Muted = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(7, 8, 9)),
            },
            Accent = new TesseraThemeAccentTokens
            {
                Secondary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(11, 12, 13)),
            },
            Selection = new TesseraThemeSelectionTokens
            {
                Foreground = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(14, 15, 16)),
                Background = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(17, 18, 19)),
            },
            Focus = new TesseraThemeFocusTokens
            {
                Ring = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
                Title = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(23, 24, 25)),
                Border = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(26, 27, 28)),
            },
            Border = new TesseraThemeBorderTokens
            {
                Default = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(29, 30, 31)),
                Focused = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(32, 33, 34)),
            },
            State = new TesseraThemeStateTokens
            {
                Error = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(35, 36, 37)),
            },
        };

        var mapped = new SearchResultsView().ApplyTheme(theme);
        TestAssert.Equal(theme.Text.Secondary, mapped.TitleStyle, "Title style should map to Text.Secondary.");
        TestAssert.Equal(theme.Focus.Title, mapped.FocusedTitleStyle, "Focused title style should map to Focus.Title.");
        TestAssert.Equal(theme.Text.Primary, mapped.DefaultRowStyle, "Default row style should map to Text.Primary.");
        TestAssert.Equal(theme.Accent.Secondary, mapped.HoveredRowStyle, "Hovered row style should map to Accent.Secondary.");
        TestAssert.Equal(theme.State.Error, mapped.ErrorRowStyle, "Error row style should map to State.Error.");
        TestAssert.Equal(theme.Border.Default, mapped.BorderStyleText, "Border style should map to Border.Default.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), mapped.FocusedBorderStyleText, "Focused border style should map to focused border tokens.");

        var explicitStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(201, 202, 203));
        var defaults = new SearchResultsView
        {
            DefaultRowStyle = explicitStyle,
            BorderStyleText = explicitStyle,
        };

        defaults.ApplyThemeDefaults(theme);

        TestAssert.Equal(explicitStyle, defaults.DefaultRowStyle, "Defaults should not overwrite explicit DefaultRowStyle.");
        TestAssert.Equal(explicitStyle, defaults.BorderStyleText, "Defaults should not overwrite explicit BorderStyleText.");
        TestAssert.Equal(theme.State.Error, defaults.ErrorRowStyle, "Defaults should fill ErrorRowStyle.");
        TestAssert.Equal(theme.Border.Focused.Merge(theme.Focus.Border), defaults.FocusedBorderStyleText, "Defaults should fill focused border style.");
    }
}
