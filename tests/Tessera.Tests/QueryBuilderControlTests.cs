using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class QueryBuilderControlTests
{
    [Test]
    public void Controls_QueryBuilder_RendersRulesAndPreview()
    {
        var control = new QueryBuilder
        {
            Border = BorderStyle.None,
        };
        control.SetRules(
        [
            new QueryRule("status", QueryOperator.Equals, "open"),
            new QueryRule("title", QueryOperator.Contains, "error budget"),
        ]);
        var canvas = new Canvas(64, 6);

        control.Render(canvas, new Rect(0, 0, 64, 6));
        var output = canvas.Render();

        TestAssert.True(output.Contains("AND status = open", StringComparison.Ordinal), "Preview row should include combinator and first rule.");
        TestAssert.True(output.Contains("▸ status = open", StringComparison.Ordinal), "Selected rule row should render marker and expression.");
        TestAssert.True(output.Contains("title ~ error budget", StringComparison.Ordinal), "Second rule row should render operator and value.");
        TestAssert.True(control.QueryText.Contains("title ~ \"error budget\"", StringComparison.Ordinal), "Query text should quote whitespace values.");
    }

    [Test]
    public void Controls_QueryBuilder_ApiMutations_RaiseQueryChanged()
    {
        var control = new QueryBuilder();
        var changes = 0;
        var lastQuery = string.Empty;
        control.QueryChanged += (_, args) =>
        {
            changes++;
            lastQuery = args.Query;
        };

        _ = control.AddRule("status", QueryOperator.Equals, "open");
        _ = control.AddRule("team", QueryOperator.Equals, "ops");
        control.ToggleCombinator();
        _ = control.UpdateRule(1, value: "core");
        _ = control.RemoveRuleAt(0);

        TestAssert.True(changes >= 4, "Mutations should raise query changed events.");
        TestAssert.True(control.UseOr, "Toggle should switch combinator to OR.");
        TestAssert.Equal("team = core", control.QueryText, "Final query text should match remaining updated rule.");
        TestAssert.Equal(control.QueryText, lastQuery, "Last event query should match current query text.");
    }

    [Test]
    public void Controls_QueryBuilder_KeyboardAndPointerNavigation_UpdateSelection()
    {
        var control = new QueryBuilder
        {
            Border = BorderStyle.None,
            ShowQueryPreview = false,
            IsFocused = true,
        };
        control.SetRules(
        [
            new QueryRule("a", QueryOperator.Equals, "1"),
            new QueryRule("b", QueryOperator.Equals, "2"),
            new QueryRule("c", QueryOperator.Equals, "3"),
        ]);

        _ = control.Handle(new KeyPressed(Key.Down));
        _ = control.Handle(new KeyPressed(Key.End));
        _ = control.Handle(new KeyPressed(Key.Up));
        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 0), new Rect(0, 0, 30, 4));
        _ = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 0), new Rect(0, 0, 30, 4));

        TestAssert.Equal(0, control.SelectedIndex, "Pointer press should select the hovered row.");
    }

    [Test]
    public void Controls_QueryBuilder_StateStyles_RenderExpectedAnsi()
    {
        var control = new QueryBuilder
        {
            Border = BorderStyle.None,
            ShowQueryPreview = false,
            IsFocused = true,
            HasError = true,
            SelectedRuleStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(1, 2, 3)),
            FocusedRuleStyle = TesseraStyle.Empty.WithItalic(),
            HoveredRuleStyle = TesseraStyle.Empty.WithUnderline(),
            DisabledRuleStyle = TesseraStyle.Empty.WithDim(),
            ErrorRuleStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
        };
        control.SetRules(
        [
            new QueryRule("status", QueryOperator.Equals, "open"),
            new QueryRule("team", QueryOperator.Equals, "core")
            {
                IsDisabled = true,
                HasError = true,
            },
        ]);
        var bounds = new Rect(0, 0, 48, 5);
        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1), bounds);
        _ = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 1, 1), bounds);
        var canvas = new Canvas(48, 5, CanvasTextMode.GraphemeAware);

        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.Equal(1, control.SelectedIndex, "Pointer press should select second rule.");
        TestAssert.True(output.Contains("48;2;1;2;3", StringComparison.Ordinal), "Selected style should render.");
        TestAssert.True(output.Contains("38;2;31;32;33", StringComparison.Ordinal), "Error style should render.");
        TestAssert.True(
            output.Contains(";4;", StringComparison.Ordinal) || output.Contains("[4m", StringComparison.Ordinal),
            "Hovered style should render.");
    }

    [Test]
    public void Controls_QueryBuilder_DefaultRender_IsDeterministicAndMonochrome()
    {
        var control = new QueryBuilder
        {
            Border = BorderStyle.None,
        };
        control.SetRules(
        [
            new QueryRule("region", QueryOperator.Equals, "eu"),
            new QueryRule("latency", QueryOperator.LessThan, "100"),
        ]);
        var bounds = new Rect(0, 0, 40, 5);
        var firstCanvas = new Canvas(40, 5);
        var secondCanvas = new Canvas(40, 5);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        TestAssert.Equal(first, second, "QueryBuilder render should be deterministic.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default QueryBuilder output should remain monochrome.");
    }
}
