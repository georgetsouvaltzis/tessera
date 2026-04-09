using NUnit.Framework;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class DialogClosedEventTests
{
    [Test]
    public void DialogClosedEventAcceptRaisesAcceptedThenClosedOnce()
    {
        var dialog = CreateVisibleFocusedDialog();
        var sequence = new List<string>();
        var acceptedCount = 0;
        var dismissedCount = 0;
        var closedCount = 0;

        dialog.Accepted += (_, _) =>
        {
            acceptedCount++;
            sequence.Add("accepted");
        };
        dialog.Dismissed += (_, _) =>
        {
            dismissedCount++;
            sequence.Add("dismissed");
        };
        dialog.Closed += (_, args) =>
        {
            closedCount++;
            sequence.Add($"closed:{args.Result}");
        };

        var handled = dialog.Handle(new KeyPressed(Key.Enter));
        var ignored = dialog.Handle(new KeyPressed(Key.Enter));

        Assert.That(handled, Is.True);
        Assert.That(ignored, Is.False);
        Assert.That(dialog.LastResult, Is.EqualTo(DialogResult.Accepted));
        Assert.That(acceptedCount, Is.EqualTo(1));
        Assert.That(dismissedCount, Is.EqualTo(0));
        Assert.That(closedCount, Is.EqualTo(1));
        Assert.That(sequence, Is.EqualTo(new[] { "accepted", "closed:Accepted" }));
    }

    [Test]
    public void DialogClosedEventDismissRaisesDismissedThenClosedOnce()
    {
        var dialog = CreateVisibleFocusedDialog();
        var sequence = new List<string>();
        var acceptedCount = 0;
        var dismissedCount = 0;
        var closedCount = 0;

        dialog.Accepted += (_, _) =>
        {
            acceptedCount++;
            sequence.Add("accepted");
        };
        dialog.Dismissed += (_, _) =>
        {
            dismissedCount++;
            sequence.Add("dismissed");
        };
        dialog.Closed += (_, args) =>
        {
            closedCount++;
            sequence.Add($"closed:{args.Result}");
        };

        var handled = dialog.Handle(new KeyPressed(Key.Escape));
        var ignored = dialog.Handle(new KeyPressed(Key.Escape));

        Assert.That(handled, Is.True);
        Assert.That(ignored, Is.False);
        Assert.That(dialog.LastResult, Is.EqualTo(DialogResult.Dismissed));
        Assert.That(acceptedCount, Is.EqualTo(0));
        Assert.That(dismissedCount, Is.EqualTo(1));
        Assert.That(closedCount, Is.EqualTo(1));
        Assert.That(sequence, Is.EqualTo(new[] { "dismissed", "closed:Dismissed" }));
    }

    [Test]
    public void DialogClosedEventFiresPerDecisionAndTryConsumeResultRemainsSingleUse()
    {
        var dialog = CreateVisibleFocusedDialog();
        var acceptedCount = 0;
        var dismissedCount = 0;
        var closedResults = new List<DialogResult>();

        dialog.Accepted += (_, _) => acceptedCount++;
        dialog.Dismissed += (_, _) => dismissedCount++;
        dialog.Closed += (_, args) => closedResults.Add(args.Result);

        Assert.That(dialog.Handle(new KeyPressed(Key.Enter)), Is.True);
        Assert.That(dialog.TryConsumeResult(out var accepted), Is.True);
        Assert.That(accepted, Is.EqualTo(DialogResult.Accepted));
        Assert.That(dialog.TryConsumeResult(out _), Is.False);

        dialog.Show("Confirm", "dismiss?");
        dialog.IsFocused = true;
        Assert.That(dialog.Handle(new KeyPressed(Key.Escape)), Is.True);
        Assert.That(dialog.TryConsumeResult(out var dismissed), Is.True);
        Assert.That(dismissed, Is.EqualTo(DialogResult.Dismissed));
        Assert.That(dialog.TryConsumeResult(out _), Is.False);

        Assert.That(acceptedCount, Is.EqualTo(1));
        Assert.That(dismissedCount, Is.EqualTo(1));
        Assert.That(closedResults, Is.EqualTo(new[] { DialogResult.Accepted, DialogResult.Dismissed }));
    }

    private static Dialog CreateVisibleFocusedDialog()
    {
        return new Dialog
        {
            IsVisible = true,
            IsFocused = true,
        };
    }
}
