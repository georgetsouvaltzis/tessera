using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class WizardControlTests
{
    [Test]
    public void ControlsWizardRendersMarkersAndStepContent()
    {
        var wizard = new Wizard
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            ShowStepNumbers = true,
            ActiveMarker = ">",
            CompletedMarker = "✓",
            PendingMarker = "·",
        };
        wizard.SetSteps(
        [
            new WizardStep("account", "Account", "Create credentials", isCompleted: true),
            new WizardStep("profile", "Profile", "Fill profile"),
            new WizardStep("confirm", "Confirm", "Review and submit"),
        ]);
        _ = wizard.SelectStep(1);

        var output = Render(wizard, width: 96, height: 6);

        Assert.That(output.Contains("✓ 1. Account - Create credentials", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("> 2. Profile - Fill profile", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("· 3. Confirm - Review and submit", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void ControlsWizardKeyboardAndPointerNavigationRaisesStepChanged()
    {
        var wizard = new Wizard
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            IsFocused = true,
        };
        wizard.SetSteps(
        [
            new WizardStep("a", "Account"),
            new WizardStep("b", "Profile"),
            new WizardStep("c", "Confirm", isDisabled: true),
            new WizardStep("d", "Done"),
        ]);

        var raised = 0;
        WizardStepChangedEventArgs? latest = null;
        wizard.StepChanged += (_, args) =>
        {
            raised++;
            latest = args;
        };

        var downHandled = wizard.Handle(new KeyPressed(Key.Down));
        var rightHandled = wizard.Handle(new KeyPressed(Key.Right));
        var clickHandled = wizard.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 1),
            new Rect(0, 0, 96, 6));
        var completeHandled = wizard.Handle(new KeyPressed(Key.Enter));

        Assert.That(downHandled, Is.True);
        Assert.That(rightHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(completeHandled, Is.True);
        Assert.That(wizard.CurrentIndex, Is.EqualTo(1));
        Assert.That(wizard.CurrentStep?.Id, Is.EqualTo("b"));
        Assert.That(wizard.CurrentStep?.IsCompleted, Is.True);
        Assert.That(raised, Is.GreaterThanOrEqualTo(3));
        Assert.That(latest?.CurrentStep?.Id, Is.EqualTo("b"));
    }

    [Test]
    public void ControlsWizardDefaultRenderIsDeterministicAndMonochrome()
    {
        var wizard = new Wizard
        {
            Border = BorderStyle.None,
            Title = string.Empty,
        };
        wizard.SetSteps(
        [
            new WizardStep("a", "Account"),
            new WizardStep("b", "Profile"),
            new WizardStep("c", "Confirm"),
        ]);

        var first = Render(wizard, width: 80, height: 5);
        var second = Render(wizard, width: 80, height: 5);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    [Test]
    public void ControlsWizardStyleHooksEmitAnsi()
    {
        var wizard = new Wizard
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            IsFocused = true,
            ActiveStepStyle = TesseraStyle.Empty.WithBold(),
            FocusedActiveStepStyle = TesseraStyle.Empty.WithUnderline(),
            HoveredStepStyle = TesseraStyle.Empty.WithBackground(AnsiColor.Rgb(10, 20, 30)),
            CompletedStepStyle = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
        };
        wizard.SetSteps(
        [
            new WizardStep("a", "Account", isCompleted: true),
            new WizardStep("b", "Profile"),
        ]);
        _ = wizard.SelectStep(1);
        _ = wizard.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 2, 0), new Rect(0, 0, 80, 5));

        var output = Render(wizard, width: 80, height: 5);
        Assert.That(output.Contains("38;2;31;32;33", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("48;2;10;20;30", StringComparison.Ordinal), Is.True);
        Assert.That(ContainsSgrParameter(output, "1"), Is.True);
        Assert.That(ContainsSgrParameter(output, "4"), Is.True);
    }

    private static string Render(Wizard wizard, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        wizard.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }

    private static bool ContainsSgrParameter(string text, string parameter)
    {
        var startIndex = 0;
        while (startIndex < text.Length)
        {
            var escapeIndex = text.IndexOf("\u001b[", startIndex, StringComparison.Ordinal);
            if (escapeIndex < 0)
            {
                return false;
            }

            var endIndex = text.IndexOf('m', escapeIndex + 2);
            if (endIndex < 0)
            {
                return false;
            }

            var body = text.Substring(escapeIndex + 2, endIndex - (escapeIndex + 2));
            var segments = body.Split(';');
            for (var index = 0; index < segments.Length; index++)
            {
                if (string.Equals(segments[index], parameter, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            startIndex = endIndex + 1;
        }

        return false;
    }
}
