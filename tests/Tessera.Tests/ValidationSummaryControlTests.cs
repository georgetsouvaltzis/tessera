using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class ValidationSummaryControlTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "Controls_ValidationSummary_SetClearIssues_SelectionAndEvents",
            SetClearIssues_SelectionAndEvents);
        yield return new TestCase(
            "Controls_ValidationSummary_KeyboardNavigationAndEnterSelection",
            KeyboardNavigationAndEnterSelection);
        yield return new TestCase(
            "Controls_ValidationSummary_PointerHoverAndClick_InBounds",
            PointerHoverAndClick_InBounds);
        yield return new TestCase(
            "Controls_ValidationSummary_RenderStylesAndSeverity_Deterministic",
            RenderStylesAndSeverity_Deterministic);
    }

    private static Task SetClearIssues_SelectionAndEvents()
    {
        var summary = new ValidationSummary();
        var events = new List<ValidationSelectionChangedEventArgs>();
        summary.SelectionChanged += (_, args) => events.Add(args);

        summary.SetIssues(
        [
            new ValidationIssue("Name is required", ValidationSeverity.Error, "Name"),
            new ValidationIssue("Email format is unusual", ValidationSeverity.Warning, "Email")
        ]);

        TestAssert.Equal(2, summary.Issues.Count, "SetIssues should replace issue collection.");
        TestAssert.Equal(0, summary.SelectedIndex, "SetIssues should seed selection at first issue.");
        TestAssert.True(summary.SelectedItem?.Message == "Name is required", "SetIssues should expose selected item.");
        TestAssert.Equal(1, events.Count,
            "SetIssues should raise a selection event when going from empty to non-empty.");
        TestAssert.Equal(-1, events[0].PreviousIndex, "First event should expose previous empty selection.");
        TestAssert.Equal(0, events[0].CurrentIndex, "First event should expose current seeded selection.");

        var changed = summary.SetSelectedIndex(1);
        TestAssert.True(changed, "SetSelectedIndex should report change when selection moves.");
        TestAssert.Equal(1, summary.SelectedIndex, "SetSelectedIndex should move selection.");
        TestAssert.True(summary.SelectedItem?.Message == "Email format is unusual",
            "SelectedItem should track moved selection.");
        TestAssert.Equal(2, events.Count, "SetSelectedIndex should raise selection event on change.");
        TestAssert.Equal(0, events[1].PreviousIndex, "Selection event should expose previous index.");
        TestAssert.Equal(1, events[1].CurrentIndex, "Selection event should expose current index.");

        summary.ClearIssues();
        TestAssert.Equal(0, summary.Issues.Count, "ClearIssues should remove all issues.");
        TestAssert.Equal(-1, summary.SelectedIndex, "ClearIssues should clear selected index.");
        TestAssert.True(summary.SelectedItem is null, "ClearIssues should clear selected item.");
        TestAssert.Equal(3, events.Count, "ClearIssues should raise a selection event when selection is cleared.");
        TestAssert.Equal(1, events[2].PreviousIndex, "Clear event should expose prior selected index.");
        TestAssert.Equal(-1, events[2].CurrentIndex, "Clear event should expose empty selection.");
        return Task.CompletedTask;
    }

    private static Task KeyboardNavigationAndEnterSelection()
    {
        var summary = new ValidationSummary { Border = BorderStyle.None, IsFocused = true };
        summary.SetIssues(
        [
            new ValidationIssue("Issue A", ValidationSeverity.Info),
            new ValidationIssue("Issue B", ValidationSeverity.Warning),
            new ValidationIssue("Issue C")
        ]);

        var downChanged = summary.Handle(new KeyPressed(Key.Down));
        var upChanged = summary.Handle(new KeyPressed(Key.Up));
        var hoverChanged = summary.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2),
            new Rect(0, 0, 48, 4));
        var enterHandled = summary.Handle(new KeyPressed(Key.Enter));

        TestAssert.True(downChanged, "Down key should move selected index.");
        TestAssert.True(upChanged, "Up key should move selected index.");
        TestAssert.True(hoverChanged, "Pointer hover should update hovered issue.");
        TestAssert.True(enterHandled, "Enter key should be handled as selection confirmation.");
        TestAssert.Equal(2, summary.SelectedIndex, "Enter should select hovered issue.");
        TestAssert.True(summary.SelectedItem?.Message == "Issue C",
            "Enter should update selected item to hovered issue.");
        return Task.CompletedTask;
    }

    private static Task PointerHoverAndClick_InBounds()
    {
        var hoveredStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(61, 92, 123));
        var summary = new ValidationSummary { HoveredIssueStyle = hoveredStyle };
        summary.SetIssues(
        [
            new ValidationIssue("Issue A", ValidationSeverity.Info),
            new ValidationIssue("Issue B", ValidationSeverity.Warning),
            new ValidationIssue("Issue C")
        ]);

        var bounds = new Rect(0, 0, 48, 6);
        var hoverHandled = summary.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 2),
            bounds);
        var clickHandled = summary.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2),
            bounds);
        var output = Render(summary, 48, 6);

        TestAssert.True(hoverHandled, "Pointer hover in bounds should be handled.");
        TestAssert.True(clickHandled, "Pointer click in bounds should be handled.");
        TestAssert.Equal(1, summary.SelectedIndex, "Pointer click should select clicked row.");
        TestAssert.True(summary.SelectedItem?.Message == "Issue B", "Pointer click should update selected item.");
        TestAssert.True(output.Contains("38;2;61;92;123", StringComparison.Ordinal),
            "Hover style should be applied to hovered row.");

        var outside = new ValidationSummary();
        outside.SetIssues(
        [
            new ValidationIssue("Outside A"),
            new ValidationIssue("Outside B")
        ]);
        var outsideHandled = outside.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 80, 80),
            new Rect(0, 0, 24, 4));

        TestAssert.True(!outsideHandled, "Pointer click outside bounds should not be handled.");
        TestAssert.Equal(0, outside.SelectedIndex, "Pointer click outside bounds should not change selection.");
        return Task.CompletedTask;
    }

    private static Task RenderStylesAndSeverity_Deterministic()
    {
        var summary = new ValidationSummary
        {
            Border = BorderStyle.None,
            IsFocused = true,
            DefaultIssueStyle = TesseraStyle.Empty.WithItalic(),
            HoveredIssueStyle = TesseraStyle.Empty.WithOverline(),
            SelectedIssueStyle = TesseraStyle.Empty.WithBold(),
            FocusedIssueStyle = TesseraStyle.Empty.WithUnderline(),
            DisabledIssueStyle = TesseraStyle.Empty.WithDim(),
            InfoSeverityStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
            WarningSeverityStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(20, 21, 22)),
            ErrorSeverityStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(30, 31, 32))
        };
        summary.SetIssues(
        [
            new ValidationIssue("Info row", ValidationSeverity.Info),
            new ValidationIssue("Warning row", ValidationSeverity.Warning),
            new ValidationIssue("Error row")
        ]);

        _ = summary.Handle(
            new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1),
            new Rect(0, 0, 60, 4));

        var first = Render(summary, 60, 4);
        var second = Render(summary, 60, 4);

        TestAssert.Equal(first, second, "Rendering should be deterministic for stable state.");
        TestAssert.True(first.Contains("[I]", StringComparison.Ordinal), "Render should include info severity marker.");
        TestAssert.True(first.Contains("[W]", StringComparison.Ordinal),
            "Render should include warning severity marker.");
        TestAssert.True(first.Contains("[E]", StringComparison.Ordinal),
            "Render should include error severity marker.");
        TestAssert.True(first.Contains("38;2;10;11;12", StringComparison.Ordinal),
            "Info severity style should be rendered.");
        TestAssert.True(first.Contains("38;2;20;21;22", StringComparison.Ordinal),
            "Warning severity style should be rendered.");
        TestAssert.True(first.Contains("38;2;30;31;32", StringComparison.Ordinal),
            "Error severity style should be rendered.");
        TestAssert.True(first.Contains("1;3;4;38;2;10;11;12", StringComparison.Ordinal),
            "Focused selected row should merge selected, default, and focused styles.");
        TestAssert.True(first.Contains("3;53;38;2;20;21;22", StringComparison.Ordinal),
            "Hovered row should merge hovered and warning styles.");

        summary.IsDisabled = true;
        var disabled = Render(summary, 60, 4);
        TestAssert.True(disabled.Contains("2;3", StringComparison.Ordinal),
            "Disabled rows should merge disabled style.");
        return Task.CompletedTask;
    }

    private static string Render(ValidationSummary summary, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        summary.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}

[TestFixture]
[NonParallelizable]
public sealed class ValidationSummaryControlNUnitAdapter
{
    public static IEnumerable<TestCaseData> Cases()
    {
        foreach (var testCase in ValidationSummaryControlTests.Cases())
        {
            yield return new TestCaseData(testCase).SetName(testCase.Name);
        }
    }

    [TestCaseSource(nameof(Cases))]
    public async Task Execute(TestCase testCase)
    {
        Assert.That(testCase, Is.Not.Null);
        await testCase.Execute();
    }
}
