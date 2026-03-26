using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class TagInputControlTests
{
    [Test]
    public void Controls_TagInput_TagsChanged_RaisesForSetAddRemoveWithSnapshots()
    {
        var control = new TagInput();
        var events = new List<TagInputTagsChangedEventArgs>();
        control.TagsChanged += (_, args) => events.Add(args);

        control.SetTags(["ops", "infra"]);
        var added = control.AddTag("alerts");
        var removed = control.RemoveTagAt(1);

        Assert.That(added, Is.True);
        Assert.That(removed, Is.True);
        Assert.That(events, Has.Count.EqualTo(3));

        Assert.That(events[0].PreviousTags, Is.Empty);
        Assert.That(events[0].Tags, Is.EqualTo(new[] { "ops", "infra" }));
        Assert.That(events[1].PreviousTags, Is.EqualTo(new[] { "ops", "infra" }));
        Assert.That(events[1].Tags, Is.EqualTo(new[] { "ops", "infra", "alerts" }));
        Assert.That(events[2].PreviousTags, Is.EqualTo(new[] { "ops", "infra", "alerts" }));
        Assert.That(events[2].Tags, Is.EqualTo(new[] { "ops", "alerts" }));
    }

    [Test]
    public void Controls_TagInput_TagsChanged_DoesNotRaiseForNoOpMutations()
    {
        var control = new TagInput();
        var events = 0;
        control.TagsChanged += (_, _) => events++;

        control.SetTags(["ops"]);
        var duplicateAdded = control.AddTag("ops");
        var invalidRemoved = control.RemoveTagAt(9);
        control.SetTags(["ops"]);

        Assert.That(duplicateAdded, Is.False);
        Assert.That(invalidRemoved, Is.False);
        Assert.That(events, Is.EqualTo(1));
    }

    [Test]
    public void Controls_TagInput_EnterCommitsTag()
    {
        var control = new TagInput
        {
            IsFocused = true,
            Border = BorderStyle.None,
        };

        _ = control.Handle(new KeyPressed(Key.Character, "o"));
        _ = control.Handle(new KeyPressed(Key.Character, "p"));
        _ = control.Handle(new KeyPressed(Key.Character, "s"));
        _ = control.Handle(new KeyPressed(Key.Enter));

        TestAssert.Equal(1, control.Tags.Count, "Enter should commit one tag.");
        TestAssert.Equal("ops", control.Tags[0], "Committed tag should match typed value.");
    }

    [Test]
    public void Controls_TagInput_SeparatorCommitAndBackspaceRemoval_Work()
    {
        var control = new TagInput
        {
            IsFocused = true,
            Border = BorderStyle.None,
            Options = new TagInputOptions(Separator: ','),
        };

        _ = control.Handle(new KeyPressed(Key.Character, "a"));
        _ = control.Handle(new KeyPressed(Key.Character, ","));
        _ = control.Handle(new KeyPressed(Key.Character, "b"));
        _ = control.Handle(new KeyPressed(Key.Enter));
        _ = control.Handle(new KeyPressed(Key.Right));
        _ = control.Handle(new KeyPressed(Key.Backspace));

        TestAssert.Equal(1, control.Tags.Count, "Backspace should remove selected tag when input is empty.");
        TestAssert.Equal("a", control.Tags[0], "Expected remaining tag after removal.");
    }

    [Test]
    public void Controls_TagInput_PointerSelectionAndStateStyles_RenderExpectedAnsi()
    {
        var control = new TagInput
        {
            IsFocused = true,
            HasError = true,
            Border = BorderStyle.None,
            SelectedTagStyle = TeaStyle.Empty.WithBackground(AnsiColor.Rgb(11, 12, 13)),
            FocusedTagStyle = TeaStyle.Empty.WithItalic(),
            HoveredTagStyle = TeaStyle.Empty.WithUnderline(),
            ErrorTagStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(31, 32, 33)),
        };
        control.SetTags(["alpha", "beta"]);
        var bounds = new Rect(0, 0, 40, 4);

        _ = control.Handle(new PointerInput(PointerEventKind.Motion, PointerButton.None, 9, 0), bounds);
        _ = control.Handle(new PointerInput(PointerEventKind.Press, PointerButton.Left, 9, 0), bounds);
        var canvas = new Canvas(40, 4, CanvasTextMode.GraphemeAware);
        control.Render(canvas, bounds);
        var output = canvas.Render();

        TestAssert.Equal(1, control.SelectedTagIndex, "Pointer press should select the hovered tag.");
        TestAssert.True(output.Contains("48;2;11;12;13", StringComparison.Ordinal), "Selected tag style should render.");
        TestAssert.True(output.Contains("38;2;31;32;33", StringComparison.Ordinal), "Error style should render.");
        TestAssert.True(
            output.Contains(";4;", StringComparison.Ordinal) || output.Contains("[4m", StringComparison.Ordinal),
            "Hovered style should render.");
    }

    [Test]
    public void Controls_TagInput_DefaultRender_IsDeterministicAndMonochrome()
    {
        var control = new TagInput
        {
            Border = BorderStyle.None,
        };
        control.SetTags(["one", "two"]);
        var bounds = new Rect(0, 0, 32, 4);
        var firstCanvas = new Canvas(32, 4);
        var secondCanvas = new Canvas(32, 4);

        control.Render(firstCanvas, bounds);
        control.Render(secondCanvas, bounds);
        var first = firstCanvas.Render();
        var second = secondCanvas.Render();

        TestAssert.Equal(first, second, "Tag input render should be deterministic.");
        TestAssert.True(!first.Contains("\u001b[", StringComparison.Ordinal), "Default tag input should render monochrome output.");
    }
}
