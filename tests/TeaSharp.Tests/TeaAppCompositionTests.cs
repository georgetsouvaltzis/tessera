using System.ComponentModel;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Layout;

namespace TeaSharp.Tests;

internal static class TeaAppCompositionTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TeaAppComposition_HandleScreenInput_RoutesButtonActivation",
            HandleScreenInput_RoutesButtonActivation);
        yield return new TestCase(
            "TeaAppComposition_HandleScreenInput_UsesTabToMoveFocusIntoTextInput",
            HandleScreenInput_UsesTabToMoveFocusIntoTextInput);
        yield return new TestCase(
            "TeaAppComposition_HandleScreenInput_RoutesChoiceSelection",
            HandleScreenInput_RoutesChoiceSelection);
        yield return new TestCase(
            "TeaAppComposition_LegacyLayoutHelpers_AreMarkedAdvanced",
            LegacyLayoutHelpers_AreMarkedAdvanced);
    }

    private static Task HandleScreenInput_RoutesButtonActivation()
    {
        var app = new ButtonApp();
        var screen = (IScreen)app;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(app.Button.TryConsumeActivation(), "Enter should activate the focused button through HandleScreenInput.");
        return Task.CompletedTask;
    }

    private static Task HandleScreenInput_UsesTabToMoveFocusIntoTextInput()
    {
        var app = new FormApp();
        var screen = (IScreen)app;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Tab));
        screen.Update(new KeyPressMsg(KeyCode.Character, "x"));
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("x", app.Input.Value, "Tab should move focus to the next control and route typing into the text input.");
        TestAssert.True(app.Input.TryConsumeSubmission(out var value), "Enter should submit through the routed text input.");
        TestAssert.Equal("x", value, "Submitted text should match the typed value.");
        return Task.CompletedTask;
    }

    private static Task HandleScreenInput_RoutesChoiceSelection()
    {
        var app = new ChoiceApp();
        var screen = (IScreen)app;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Down));
        screen.Update(new KeyPressMsg(KeyCode.Down));
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("History", app.Choice.SelectedItem, "Choice should route open, navigate, and confirm through the compiled screen.");
        return Task.CompletedTask;
    }

    private static Task LegacyLayoutHelpers_AreMarkedAdvanced()
    {
        Type[] helperTypes =
        [
            typeof(Stack),
            typeof(Split),
            typeof(Panel),
            typeof(Dock),
            typeof(Overlay),
            typeof(Center),
            typeof(Slot),
        ];

        foreach (var helperType in helperTypes)
        {
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
                helperType,
                typeof(EditorBrowsableAttribute));

            TestAssert.True(attribute is not null, $"{helperType.Name} should be explicitly marked as advanced.");
            TestAssert.True(
                attribute!.State == EditorBrowsableState.Advanced,
                $"{helperType.Name} should be hidden from the default composition path.");
        }

        return Task.CompletedTask;
    }

    private sealed class ButtonApp : TeaApp
    {
        public Button Button { get; } = new() { Text = "Run" };

        public override TeaEffect? Update(Message message)
        {
            HandleScreenInput(message);
            return null;
        }

        public override Screen Build(ScreenContext context) =>
            Screen.From(new CenterLayout(Button, width: 16, height: 3));
    }

    private sealed class FormApp : TeaApp
    {
        public Button Button { get; } = new() { Text = "Send" };

        public TextInput Input { get; } = new() { Title = "Command" };

        public override TeaEffect? Update(Message message)
        {
            HandleScreenInput(message);
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            var layout = new StackLayout(
                LayoutOrientation.Vertical,
                gap: 1,
                children:
                [
                    new LayoutSlot(Button, LayoutLength.Fixed(3)),
                    new LayoutSlot(Input, LayoutLength.Fixed(3)),
                ]);

            return Screen.From(new PanelLayout(layout, title: "Form", border: BorderStyle.SingleLine, padding: Thickness.All(1)));
        }
    }

    private sealed class ChoiceApp : TeaApp
    {
        public Choice Choice { get; } = new() { Title = "Tab" };

        public ChoiceApp()
        {
            Choice.SetItems(["Open", "History", "Archived"]);
        }

        public override TeaEffect? Update(Message message)
        {
            HandleScreenInput(message);
            return null;
        }

        public override Screen Build(ScreenContext context) =>
            Screen.From(new CenterLayout(Choice, width: 28, height: 6));
    }
}
