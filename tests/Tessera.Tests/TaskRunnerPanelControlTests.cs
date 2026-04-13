using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TaskRunnerPanelControlTests
{
    [Test]
    public void ControlsTaskRunnerPanelRendersStatusMarkersAndRows()
    {
        var control = new TaskRunnerPanel { Border = BorderStyle.None, ShowTimestamp = false };
        control.SetItems(
        [
            new TaskRunItem("build", "Build", TaskRunStatus.Succeeded, "compiled"),
            new TaskRunItem("test", "Test", TaskRunStatus.Failed, "2 failed"),
            new TaskRunItem("deploy", "Deploy", TaskRunStatus.Running, "publishing")
        ]);

        var output = Render(control, 80, 4);

        Assert.That(output.Contains("✓ OK Build - compiled", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("✕ FAIL Test - 2 failed", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("▶ RUN Deploy - publishing", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsTaskRunnerPanelKeyboardAndPointerSelectionRaisesSelectionChanged()
    {
        var control = new TaskRunnerPanel { Border = BorderStyle.None, IsFocused = true, ShowTimestamp = false };
        control.SetItems(
        [
            new TaskRunItem("build", "Build"),
            new TaskRunItem("test", "Test"),
            new TaskRunItem("deploy", "Deploy")
        ]);

        var raised = 0;
        TaskRunnerSelectionChangedEventArgs? latest = null;
        control.SelectionChanged += (_, args) =>
        {
            raised++;
            latest = args;
        };

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var clickHandled = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2),
            new Rect(0, 0, 64, 5));

        Assert.That(downHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(raised, Is.GreaterThanOrEqualTo(2));
        Assert.That(latest?.SelectedItem?.Id, Is.EqualTo("deploy"));
    }

    [Test]
    public void ControlsTaskRunnerPanelHoverStyleRendersOnPointerMotion()
    {
        var hoveredStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(18, 52, 86));
        var control = new TaskRunnerPanel
        {
            Border = BorderStyle.None,
            ShowTimestamp = false,
            HoveredRowStyle = hoveredStyle
        };
        control.SetItems(
        [
            new TaskRunItem("build", "Build"),
            new TaskRunItem("test", "Test")
        ]);

        var handled = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 1, 1),
            new Rect(0, 0, 64, 4));
        var output = Render(control, 64, 4);

        Assert.That(handled, Is.True);
        Assert.That(output.Contains("38;2;18;52;86", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsTaskRunnerPanelStyleHooksRenderAnsiAndFocusedBorder()
    {
        var focusedBorderStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(120, 90, 40));
        var control = new TaskRunnerPanel
        {
            IsFocused = true,
            Border = BorderStyle.SingleLine,
            ShowTimestamp = false,
            BorderStyleText = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(10, 11, 12)),
            FocusedBorderStyleText = focusedBorderStyle,
            RunningStatusStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(30, 40, 50)),
            SelectedRowStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(60, 70, 80))
        };
        control.SetItems([new TaskRunItem("deploy", "Deploy", TaskRunStatus.Running)]);

        var output = Render(control, 48, 5);

        Assert.That(output.Contains("38;2;30;40;50", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("48;2;60;70;80", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains(focusedBorderStyle.Render("┌"), StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsTaskRunnerPanelRenderIsDeterministicWithoutStyles()
    {
        var control = new TaskRunnerPanel { Border = BorderStyle.None, ShowTimestamp = false };
        control.SetItems([new TaskRunItem("build", "Build", TaskRunStatus.Succeeded, "done")]);

        var first = Render(control, 32, 3);
        var second = Render(control, 32, 3);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\e[", StringComparison.Ordinal), Is.False);
    }

    private static string Render(TaskRunnerPanel control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }
}
