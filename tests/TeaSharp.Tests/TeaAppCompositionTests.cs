using System.ComponentModel;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Core.Messages;
using TeaSharp.Layout;

namespace TeaSharp.Tests;

internal static class TeaAppCompositionTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TeaAppComposition_AutomaticallyRoutesButtonActivation",
            AutomaticallyRoutesButtonActivation);
        yield return new TestCase(
            "TeaAppComposition_AutomaticallyRoutesTabIntoTextInput",
            AutomaticallyRoutesTabIntoTextInput);
        yield return new TestCase(
            "TeaAppComposition_AutomaticallyRoutesChoiceSelection",
            AutomaticallyRoutesChoiceSelection);
        yield return new TestCase(
            "TeaAppComposition_AutomaticallyRoutesComboBoxSelection",
            AutomaticallyRoutesComboBoxSelection);
        yield return new TestCase(
            "TeaAppComposition_AutomaticallyRoutesTabsSelection",
            AutomaticallyRoutesTabsSelection);
        yield return new TestCase(
            "TeaAppComposition_AutomaticallyRoutesMenuBarActivation",
            AutomaticallyRoutesMenuBarActivation);
        yield return new TestCase(
            "TeaAppComposition_HandledControlInput_DoesNotReachDefaultUpdate",
            HandledControlInput_DoesNotReachDefaultUpdate);
        yield return new TestCase(
            "TeaAppComposition_RequestEffect_AllowsHandledControlInputToTriggerRuntimeEffect",
            RequestEffect_AllowsHandledControlInputToTriggerRuntimeEffect);
        yield return new TestCase(
            "TeaAppComposition_VisibleOverlayCanClaimFocusThroughRootLayouts",
            VisibleOverlayCanClaimFocusThroughRootLayouts);
        yield return new TestCase(
            "TeaAppComposition_LegacyLayoutHelpers_AreMarkedAdvanced",
            LegacyLayoutHelpers_AreMarkedAdvanced);
        yield return new TestCase(
            "TeaAppComposition_AdvancedLayoutOverloads_AreMarkedAdvanced",
            AdvancedLayoutOverloads_AreMarkedAdvanced);
        yield return new TestCase(
            "TeaAppComposition_LegacyCanvasComponentEntryPoints_AreMarkedAdvanced",
            LegacyCanvasComponentEntryPoints_AreMarkedAdvanced);
        yield return new TestCase(
            "TeaAppComposition_LowLevelTreeLayouts_AreMarkedAdvanced",
            LowLevelTreeLayouts_AreMarkedAdvanced);
        yield return new TestCase(
            "TeaAppComposition_ScreenAssemblyLayouts_RemainDiscoverable",
            ScreenAssemblyLayouts_RemainDiscoverable);
    }

    private static Task AutomaticallyRoutesButtonActivation()
    {
        var app = new ButtonApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(1, app.ActivationCount, "Enter should activate the focused button automatically before Update.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesTabIntoTextInput()
    {
        var app = new FormApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Tab));
        screen.Update(new KeyPressMsg(KeyCode.Character, "x"));
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("x", app.Input.Value, "Tab should move focus to the next control and route typing into the text input.");
        TestAssert.Equal("x", app.LastSubmittedValue, "Enter should raise submission through the routed text input.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesChoiceSelection()
    {
        var app = new ChoiceApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Down));
        screen.Update(new KeyPressMsg(KeyCode.Down));
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("History", app.Choice.SelectedItem, "Choice should route open, navigate, and confirm through the compiled screen.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesComboBoxSelection()
    {
        var app = new ComboBoxApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "w"));
        screen.Update(new KeyPressMsg(KeyCode.Down));
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("west", app.ComboBox.SelectedItem, "ComboBox should route filter, navigate, and confirm through the compiled screen.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesTabsSelection()
    {
        var app = new TabsApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "2"));

        TestAssert.Equal(1, app.Tabs.SelectedIndex, "Tabs should route numeric shortcuts through the compiled screen.");
        return Task.CompletedTask;
    }

    private static async Task AutomaticallyRoutesMenuBarActivation()
    {
        var app = new MenuApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "r"));

        await Task.Yield();
        TestAssert.Equal("refresh", app.LastActivatedItemId, "MenuBar activation should preserve the configured item id.");
    }

    private static Task HandledControlInput_DoesNotReachDefaultUpdate()
    {
        var app = new FilteredInputApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(0, app.KeyUpdateCount, "Handled control input should not flow into the default Update method.");
        TestAssert.Equal(1, app.ActivationCount, "The control should still receive the handled key.");
        return Task.CompletedTask;
    }

    private static Task RequestEffect_AllowsHandledControlInputToTriggerRuntimeEffect()
    {
        var app = new RequestedEffectApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        var effect = screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(effect is not null, "Handled control input should still be able to schedule runtime effects.");
        TestAssert.Equal(0, app.KeyUpdateCount, "Handled control input should not reach the default Update method.");
        return Task.CompletedTask;
    }

    private static Task VisibleOverlayCanClaimFocusThroughRootLayouts()
    {
        var app = new OverlayPaletteApp();
        var screen = app.RuntimeScreen;

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        TestAssert.True(app.Palette.IsFocused, "Visible overlay should own focus after the screen is composed.");
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("rollback", app.LastExecutedItemId, "Visible overlays should be able to claim focus through the root layout model.");
        TestAssert.Equal(0, app.ButtonActivationCount, "Overlay focus should keep the underlying body control from activating.");
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

    private static Task AdvancedLayoutOverloads_AreMarkedAdvanced()
    {
        var advancedCtors =
            new (Type Type, Type[] Parameters)[]
            {
                (typeof(LayoutSlot), [typeof(ICanvasComponent), typeof(LayoutLength), typeof(Thickness), typeof(ScreenRegionKey), typeof(int?), typeof(int?), typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
                (typeof(CenterLayout), [typeof(ICanvasComponent), typeof(int?), typeof(int?), typeof(Thickness), typeof(ScreenRegionKey), typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
                (typeof(PanelLayout), [typeof(ICanvasComponent), typeof(string), typeof(BorderStyle), typeof(Thickness), typeof(Thickness), typeof(ScreenRegionKey), typeof(int?), typeof(int?), typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
                (typeof(ComponentLayout), [typeof(ICanvasComponent), typeof(ScreenRegionKey), typeof(int?), typeof(int?), typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
            };

        foreach (var (type, parameters) in advancedCtors)
        {
            var ctor = type.GetConstructor(parameters);
            TestAssert.True(ctor is not null, $"{type.Name} advanced overload should exist for advanced callers.");
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(ctor!, typeof(EditorBrowsableAttribute));
            TestAssert.True(attribute is not null, $"{type.Name} advanced overload should be marked advanced.");
            TestAssert.True(attribute!.State == EditorBrowsableState.Advanced, $"{type.Name} advanced overload should be hidden from default discoverability.");
        }

        return Task.CompletedTask;
    }

    private static Task LegacyCanvasComponentEntryPoints_AreMarkedAdvanced()
    {
        var advancedMethods =
            new (Type Type, string Name, Type[] Parameters)[]
            {
                (typeof(Screen), nameof(Screen.From), [typeof(ICanvasComponent)]),
                (typeof(LayoutSlot), nameof(LayoutSlot.Auto), [typeof(ICanvasComponent), typeof(Thickness)]),
                (typeof(LayoutSlot), nameof(LayoutSlot.Fixed), [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(LayoutSlot), nameof(LayoutSlot.Fill), [typeof(ICanvasComponent), typeof(Thickness)]),
                (typeof(LayoutSlot), nameof(LayoutSlot.Weighted), [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(RowLayout), nameof(RowLayout.AddAuto), [typeof(ICanvasComponent), typeof(Thickness)]),
                (typeof(RowLayout), nameof(RowLayout.AddFixed), [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(RowLayout), nameof(RowLayout.AddFill), [typeof(ICanvasComponent), typeof(Thickness)]),
                (typeof(RowLayout), nameof(RowLayout.AddWeighted), [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(ColumnLayout), nameof(ColumnLayout.AddAuto), [typeof(ICanvasComponent), typeof(Thickness)]),
                (typeof(ColumnLayout), nameof(ColumnLayout.AddFixed), [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(ColumnLayout), nameof(ColumnLayout.AddFill), [typeof(ICanvasComponent), typeof(Thickness)]),
                (typeof(ColumnLayout), nameof(ColumnLayout.AddWeighted), [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
            };

        foreach (var (type, name, parameters) in advancedMethods)
        {
            var method = type.GetMethod(name, parameters);
            TestAssert.True(method is not null, $"{type.Name}.{name} legacy component overload should exist for advanced callers.");
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(method!, typeof(EditorBrowsableAttribute));
            TestAssert.True(attribute is not null, $"{type.Name}.{name} legacy component overload should be marked advanced.");
            TestAssert.True(attribute!.State == EditorBrowsableState.Advanced, $"{type.Name}.{name} legacy component overload should be hidden from the default path.");
        }

        var advancedCtors =
            new (Type Type, Type[] Parameters)[]
            {
                (typeof(LayoutSlot), [typeof(ICanvasComponent), typeof(LayoutLength), typeof(Thickness)]),
                (typeof(CenterLayout), [typeof(ICanvasComponent), typeof(int?), typeof(int?), typeof(Thickness)]),
                (typeof(PanelLayout), [typeof(ICanvasComponent), typeof(string), typeof(BorderStyle), typeof(Thickness), typeof(Thickness)]),
            };

        foreach (var (type, parameters) in advancedCtors)
        {
            var ctor = type.GetConstructor(parameters);
            TestAssert.True(ctor is not null, $"{type.Name} legacy component constructor should exist for advanced callers.");
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(ctor!, typeof(EditorBrowsableAttribute));
            TestAssert.True(attribute is not null, $"{type.Name} legacy component constructor should be marked advanced.");
            TestAssert.True(attribute!.State == EditorBrowsableState.Advanced, $"{type.Name} legacy component constructor should be hidden from the default path.");
        }

        return Task.CompletedTask;
    }

    private static Task LowLevelTreeLayouts_AreMarkedAdvanced()
    {
        Type[] layoutTypes =
        [
            typeof(StackLayout),
            typeof(SplitLayout),
            typeof(DockLayout),
            typeof(OverlayLayout),
            typeof(ComponentLayout),
        ];

        foreach (var type in layoutTypes)
        {
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
                type,
                typeof(EditorBrowsableAttribute));

            TestAssert.True(attribute is not null, $"{type.Name} should be explicitly marked as advanced.");
            TestAssert.True(
                attribute!.State == EditorBrowsableState.Advanced,
                $"{type.Name} should be hidden from the default composition path.");
        }

        return Task.CompletedTask;
    }

    private static Task ScreenAssemblyLayouts_RemainDiscoverable()
    {
        Type[] layoutTypes =
        [
            typeof(WindowLayout),
            typeof(RowLayout),
            typeof(ColumnLayout),
            typeof(PanelLayout),
            typeof(CenterLayout),
            typeof(LayoutSlot),
        ];

        foreach (var type in layoutTypes)
        {
            var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
                type,
                typeof(EditorBrowsableAttribute));

            TestAssert.True(attribute is null, $"{type.Name} should remain on the default composition path.");
        }

        return Task.CompletedTask;
    }

    private sealed class ButtonApp : TeaApp
    {
        public Button Button { get; } = new() { Text = "Run" };

        public int ActivationCount { get; private set; }

        public ButtonApp()
        {
            Button.Activated += (_, _) => ActivationCount++;
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) =>
            Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Button, width: 16, height: 3),
            });
    }

    private sealed class FormApp : TeaApp
    {
        public Button Button { get; } = new() { Text = "Send" };

        public TextInput Input { get; } = new() { Title = "Command" };

        public string LastSubmittedValue { get; private set; } = string.Empty;

        public FormApp()
        {
            Input.Submitted += (_, args) => LastSubmittedValue = args.Value;
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context)
        {
            var fields = new ColumnLayout
            {
                Gap = 1,
            };
            fields.AddFixed(Button, 3);
            fields.AddFixed(Input, 3);

            return Screen.From(new WindowLayout
            {
                Body = new CenterLayout(
                    new PanelLayout(fields, title: "Form", border: BorderStyle.SingleLine, padding: Thickness.All(1)),
                    width: 28,
                    height: 10),
            });
        }
    }

    private sealed class ChoiceApp : TeaApp
    {
        public Choice Choice { get; } = new() { Title = "Tab" };

        public ChoiceApp()
        {
            Choice.SetItems(["Open", "History", "Archived"]);
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) =>
            Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Choice, width: 28, height: 6),
            });
    }

    private sealed class TabsApp : TeaApp
    {
        public Tabs Tabs { get; } = new("Open", "History", "Archived");

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) =>
            Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Tabs, width: 36, height: 1),
            });
    }

    private sealed class ComboBoxApp : TeaApp
    {
        public ComboBox ComboBox { get; } = new()
        {
            Title = "Regions",
            IsFocused = true,
        };

        public ComboBoxApp()
        {
            ComboBox.SetItems(["east", "west", "north"]);
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) =>
            Screen.From(new WindowLayout
            {
                Body = new CenterLayout(ComboBox, width: 28, height: 6),
            });
    }

    private sealed class MenuApp : TeaApp
    {
        public MenuBar Menu { get; } = new();

        public string LastActivatedItemId { get; private set; } = string.Empty;

        public MenuApp()
        {
            Menu.SetItems([new MenuItem("refresh", "Refresh", 'r')]);
            Menu.ItemActivated += (_, args) => LastActivatedItemId = args.ItemId;
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) =>
            Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Menu, width: 24, height: 1),
            });
    }

    private sealed class FilteredInputApp : TeaApp
    {
        public Button Button { get; } = new() { Text = "Run" };

        public int ActivationCount { get; private set; }

        public int KeyUpdateCount { get; private set; }

        public FilteredInputApp()
        {
            Button.Activated += (_, _) => ActivationCount++;
        }

        public override TeaEffect? Update(Message message)
        {
            if (message is KeyPressed)
            {
                KeyUpdateCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context) =>
            Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Button, width: 16, height: 3),
            });
    }

    private sealed class RequestedEffectApp : TeaApp
    {
        public Button Button { get; } = new() { Text = "Quit" };

        public int KeyUpdateCount { get; private set; }

        public RequestedEffectApp()
        {
            Button.Activated += (_, _) => RequestEffect(TeaEffects.Quit);
        }

        public override TeaEffect? Update(Message message)
        {
            if (message is KeyPressed)
            {
                KeyUpdateCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context) =>
            Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Button, width: 16, height: 3),
            });
    }

    private sealed class OverlayPaletteApp : TeaApp
    {
        public Button Button { get; } = new() { Text = "Base" };

        public CommandPalette Palette { get; } = new()
        {
            Title = "Actions",
        };

        public int ButtonActivationCount { get; private set; }

        public string LastExecutedItemId { get; private set; } = string.Empty;

        public OverlayPaletteApp()
        {
            Button.Activated += (_, _) => ButtonActivationCount++;
            Palette.SetItems(
            [
                new global::TeaSharp.Controls.CommandPaletteItem("deploy", "Deploy", "publish release"),
                new global::TeaSharp.Controls.CommandPaletteItem("rollback", "Rollback", "restore previous"),
            ]);
            Palette.ItemExecuted += (_, args) => LastExecutedItemId = args.ItemId;
            Palette.Open();
            Palette.QueryText = "roll";
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) =>
            Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Button, width: 16, height: 3),
                Overlay = new CenterLayout(Palette, width: 48, height: 10),
            });
    }
}
