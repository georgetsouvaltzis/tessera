using NUnit.Framework;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class DataFormControlTests
{
    [Test]
    public void Controls_DataForm_RendersBorderTitleAndFields()
    {
        var model = new TestModel { Name = string.Empty, Email = "tea@example.dev" };
        var control = new DataForm<TestModel>
        {
            Title = "Profile",
            FocusMarker = "!",
            IsFocused = true,
        };
        control.RegisterField("name", "Name", m => m.Name, (m, value) => m.Name = value, placeholder: "type name");
        control.RegisterField("email", "Email", m => m.Email, (m, value) => m.Email = value);
        control.SetModel(model);

        var output = Render(control, width: 52, height: 8);

        Assert.That(output.Contains("Profile !", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Name", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("Email", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("tea@example.dev", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains('>'), Is.True);
    }

    [Test]
    public void Controls_DataForm_SelectionMode_IgnoresTyping_UntilEnterStartsEditing()
    {
        var model = new TestModel { Name = string.Empty };
        var control = new DataForm<TestModel>
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            IsFocused = true,
        };
        control.RegisterField("name", "Name", m => m.Name, (m, value) => m.Name = value, placeholder: "name");
        control.SetModel(model);

        var typedWhileSelected = control.Handle(new KeyPressed(Key.Character, "A"));

        Assert.That(typedWhileSelected, Is.False);
        Assert.That(control.IsEditing, Is.False);
        Assert.That(control.EditBuffer, Is.EqualTo(string.Empty));
        Assert.That(model.Name, Is.EqualTo(string.Empty));

        var output = Render(control, width: 48, height: 4);

        Assert.That(output.Contains("Press Enter to edit.", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("name |", StringComparison.Ordinal), Is.False);
    }

    [Test]
    public void Controls_DataForm_EnterStartsEditing_AndEnterCommit_UpdatesModelAndRaisesFieldCommitted()
    {
        var model = new TestModel { Name = string.Empty };
        var control = new DataForm<TestModel>
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            IsFocused = true,
        };
        control.RegisterField("name", "Name", m => m.Name, (m, value) => m.Name = value, placeholder: "name");
        control.SetModel(model);

        DataFormFieldCommittedEventArgs<TestModel>? committed = null;
        control.FieldCommitted += (_, args) => committed = args;

        var beginHandled = control.Handle(new KeyPressed(Key.Enter));
        _ = control.Handle(new KeyPressed(Key.Character, "A"));
        _ = control.Handle(new KeyPressed(Key.Character, "d"));
        _ = control.Handle(new KeyPressed(Key.Character, "a"));
        var handled = control.Handle(new KeyPressed(Key.Enter));

        Assert.That(beginHandled, Is.True);
        Assert.That(handled, Is.True);
        Assert.That(control.IsEditing, Is.False);
        Assert.That(model.Name, Is.EqualTo("Ada"));
        Assert.That(committed, Is.Not.Null);
        Assert.That(committed!.Success, Is.True);
        Assert.That(committed.PreviousValue, Is.EqualTo(string.Empty));
        Assert.That(committed.CommittedValue, Is.EqualTo("Ada"));
        Assert.That(committed.Field.Key, Is.EqualTo("name"));
    }

    [Test]
    public void Controls_DataForm_EscapeCancelsEditing_AndRestoresCommittedValue()
    {
        var model = new TestModel { Name = "Ada" };
        var control = new DataForm<TestModel>
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            IsFocused = true,
        };
        control.RegisterField("name", "Name", m => m.Name, (m, value) => m.Name = value);
        control.SetModel(model);

        _ = control.Handle(new KeyPressed(Key.Enter));
        _ = control.Handle(new KeyPressed(Key.Character, "!"));

        var outputWhileEditing = Render(control, width: 48, height: 4);
        var cancelHandled = control.Handle(new KeyPressed(Key.Escape));
        var outputAfterCancel = Render(control, width: 48, height: 4);

        Assert.That(outputWhileEditing.Contains("Ada!|", StringComparison.Ordinal), Is.True);
        Assert.That(cancelHandled, Is.True);
        Assert.That(control.IsEditing, Is.False);
        Assert.That(control.EditBuffer, Is.EqualTo("Ada"));
        Assert.That(model.Name, Is.EqualTo("Ada"));
        Assert.That(outputAfterCancel.Contains("Ada!|", StringComparison.Ordinal), Is.False);
        Assert.That(outputAfterCancel.Contains("Press Enter to edit.", StringComparison.Ordinal), Is.True);
    }

    [Test]
    public void Controls_DataForm_KeyboardAndPointerNavigation_RaisesSelectionChanged()
    {
        var model = new TestModel { Name = "A", Email = "B", Team = "C" };
        var control = new DataForm<TestModel>
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            IsFocused = true,
        };
        control.RegisterField("name", "Name", m => m.Name, (m, value) => m.Name = value);
        control.RegisterField("email", "Email", m => m.Email, (m, value) => m.Email = value);
        control.RegisterField("team", "Team", m => m.Team, (m, value) => m.Team = value);
        control.SetModel(model);

        var events = new List<DataFormSelectionChangedEventArgs<TestModel>>();
        control.SelectionChanged += (_, args) => events.Add(args);

        var downHandled = control.Handle(new KeyPressed(Key.Down));
        var upHandled = control.Handle(new KeyPressed(Key.Up));
        var clickHandled = control.Handle(
            new PointerInput(PointerEventKind.Press, PointerButton.Left, X: 1, Y: 2),
            new Rect(0, 0, 60, 4));

        Assert.That(downHandled, Is.True);
        Assert.That(upHandled, Is.True);
        Assert.That(clickHandled, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(events.Count, Is.GreaterThanOrEqualTo(3));
        Assert.That(events[^1].SelectedIndex, Is.EqualTo(2));
        Assert.That(events[^1].SelectedField?.Key, Is.EqualTo("team"));
    }

    [Test]
    public void Controls_DataForm_SelectFieldByKey_SelectsMatchingFieldAndRaisesSelectionChanged()
    {
        var model = new TestModel { Name = "A", Email = "B", Team = "C" };
        var control = new DataForm<TestModel>
        {
            Border = BorderStyle.None,
            Title = string.Empty,
        };
        control.RegisterField("name", "Name", m => m.Name, (m, value) => m.Name = value);
        control.RegisterField("email", "Email", m => m.Email, (m, value) => m.Email = value);
        control.RegisterField("team", "Team", m => m.Team, (m, value) => m.Team = value);
        control.SetModel(model);

        DataFormSelectionChangedEventArgs<TestModel>? changed = null;
        control.SelectionChanged += (_, args) => changed = args;

        var selected = control.SelectField("team");

        Assert.That(selected, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(2));
        Assert.That(control.SelectedField?.Key, Is.EqualTo("team"));
        Assert.That(changed, Is.Not.Null);
        Assert.That(changed!.PreviousField?.Key, Is.EqualTo("name"));
        Assert.That(changed.SelectedField?.Key, Is.EqualTo("team"));
    }

    [Test]
    public void Controls_DataForm_SelectFieldByKey_ReturnsFalse_WhenMissingOrAlreadySelected()
    {
        var model = new TestModel { Name = "A", Email = "B" };
        var control = new DataForm<TestModel>
        {
            Border = BorderStyle.None,
            Title = string.Empty,
        };
        control.RegisterField("name", "Name", m => m.Name, (m, value) => m.Name = value);
        control.RegisterField("email", "Email", m => m.Email, (m, value) => m.Email = value);
        control.SetModel(model);

        var missing = control.SelectField("missing");
        var alreadySelected = control.SelectField("name");
        var moved = control.SelectField("email");
        var alreadySelectedAfterMove = control.SelectField("email");

        Assert.That(missing, Is.False);
        Assert.That(alreadySelected, Is.False);
        Assert.That(moved, Is.True);
        Assert.That(alreadySelectedAfterMove, Is.False);
    }

    [Test]
    public void Controls_DataForm_DefaultRender_IsDeterministicAndMonochrome()
    {
        var model = new TestModel { Name = "Alice", Email = string.Empty };
        var control = new DataForm<TestModel>
        {
            Border = BorderStyle.None,
            Title = string.Empty,
        };
        control.RegisterField("name", "Name", m => m.Name, (m, value) => m.Name = value);
        control.RegisterField("email", "Email", m => m.Email, (m, value) => m.Email = value, placeholder: "n/a");
        control.SetModel(model);

        var first = Render(control, width: 60, height: 4);
        var second = Render(control, width: 60, height: 4);

        Assert.That(first, Is.EqualTo(second));
        Assert.That(first.Contains("\u001b[", StringComparison.Ordinal), Is.False);
    }

    [Test]
    public void Controls_DataForm_CommitFailure_RendersVisibleError_AndKeepsEditMode()
    {
        var model = new TestModel { Name = "Ada", Email = "tea@example.dev" };
        var control = new DataForm<TestModel>
        {
            Border = BorderStyle.None,
            Title = string.Empty,
            IsFocused = true,
        };
        control.RegisterField(
            "name",
            "Name",
            m => m.Name,
            (m, value) => m.Name = value,
            validator: value => value.Trim().Length >= 3 ? null : "min 3 chars");
        control.RegisterField("email", "Email", m => m.Email, (m, value) => m.Email = value);
        control.SetModel(model);

        DataFormFieldCommittedEventArgs<TestModel>? committed = null;
        control.FieldCommitted += (_, args) => committed = args;

        _ = control.Handle(new KeyPressed(Key.Enter));
        _ = control.Handle(new KeyPressed(Key.Backspace));
        _ = control.Handle(new KeyPressed(Key.Backspace));
        var commitHandled = control.Handle(new KeyPressed(Key.Enter));
        var moveHandled = control.Handle(new KeyPressed(Key.Down));
        var output = Render(control, width: 56, height: 5);

        Assert.That(commitHandled, Is.True);
        Assert.That(moveHandled, Is.True);
        Assert.That(committed, Is.Not.Null);
        Assert.That(committed!.Success, Is.False);
        Assert.That(control.IsEditing, Is.True);
        Assert.That(control.SelectedIndex, Is.EqualTo(0));
        Assert.That(control.LastCommitError, Is.EqualTo("min 3 chars"));
        Assert.That(model.Name, Is.EqualTo("Ada"));
        Assert.That(output.Contains("Error: min 3 chars", StringComparison.Ordinal), Is.True);
        Assert.That(output.Contains("A|", StringComparison.Ordinal), Is.True);
    }

    private static string Render(DataForm<TestModel> control, int width, int height)
    {
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        control.Render(canvas, new Rect(0, 0, width, height));
        return canvas.Render();
    }

    private sealed class TestModel
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;
    }
}
