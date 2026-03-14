using System.ComponentModel;
using System.Reflection;
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
            "TeaAppComposition_FocusRequestsPreferLatestRequestOverCompositionOrder",
            FocusRequestsPreferLatestRequestOverCompositionOrder);
        yield return new TestCase(
            "TeaAppComposition_FocusRequests_AreOneShotAcrossLaterBuilds",
            FocusRequests_AreOneShotAcrossLaterBuilds);
        yield return new TestCase(
            "TeaAppComposition_ScreenBuilder_ComposesAndRoutesDefaultControls",
            ScreenBuilder_ComposesAndRoutesDefaultControls);
        yield return new TestCase(
            "TeaAppComposition_RootLayouts_UseSceneCompilerInsteadOfLegacyCompiledScreen",
            RootLayouts_UseSceneCompilerInsteadOfLegacyCompiledScreen);
        yield return new TestCase(
            "TeaAppComposition_LegacyCompilerTypes_AreRemovedFromDefaultCompilerPath",
            LegacyCompilerTypes_AreRemovedFromDefaultCompilerPath);
        yield return new TestCase(
            "TeaAppComposition_LegacyLayoutHelpers_AreInternalized",
            LegacyLayoutHelpers_AreInternalized);
        yield return new TestCase(
            "TeaAppComposition_RegionKeyInteropOverloads_AreInternalized",
            RegionKeyInteropOverloads_AreInternalized);
        yield return new TestCase(
            "TeaAppComposition_LegacyCanvasComponentEntryPoints_AreMarkedAdvanced",
            LegacyCanvasComponentEntryPoints_AreMarkedAdvanced);
        yield return new TestCase(
            "TeaAppComposition_LowLevelComponentContracts_AreInternalized",
            LowLevelComponentContracts_AreInternalized);
        yield return new TestCase(
            "TeaAppComposition_LowLevelTreeLayouts_AreInternalized",
            LowLevelTreeLayouts_AreInternalized);
        yield return new TestCase(
            "TeaAppComposition_ComponentLayout_IsInternalized",
            ComponentLayout_IsInternalized);
        yield return new TestCase(
            "TeaAppComposition_ScreenAssemblyLayouts_RemainDiscoverable",
            ScreenAssemblyLayouts_RemainDiscoverable);
    }

    private static Task AutomaticallyRoutesButtonActivation()
    {
        var app = new ButtonApp();
        var screen = new TeaAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(1, app.ActivationCount, "Enter should activate the focused button automatically before Update.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesTabIntoTextInput()
    {
        var app = new FormApp();
        var screen = new TeaAppDriver(app);

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
        var screen = new TeaAppDriver(app);

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
        var screen = new TeaAppDriver(app);

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
        var screen = new TeaAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "2"));

        TestAssert.Equal(1, app.Tabs.SelectedIndex, "Tabs should route numeric shortcuts through the compiled screen.");
        return Task.CompletedTask;
    }

    private static async Task AutomaticallyRoutesMenuBarActivation()
    {
        var app = new MenuApp();
        var screen = new TeaAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "r"));

        await Task.Yield();
        TestAssert.Equal("refresh", app.LastActivatedItemId, "MenuBar activation should preserve the configured item id.");
    }

    private static Task HandledControlInput_DoesNotReachDefaultUpdate()
    {
        var app = new FilteredInputApp();
        var screen = new TeaAppDriver(app);

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
        var screen = new TeaAppDriver(app);

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
        var screen = new TeaAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        TestAssert.True(app.Palette.IsFocused, "Visible overlay should own focus after the screen is composed.");
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("rollback", app.LastExecutedItemId, "Visible overlays should be able to claim focus through the root layout model.");
        TestAssert.Equal(0, app.ButtonActivationCount, "Overlay focus should keep the underlying body control from activating.");
        return Task.CompletedTask;
    }

    private static Task FocusRequestsPreferLatestRequestOverCompositionOrder()
    {
        var app = new FocusRequestOrderingApp();
        var screen = new TeaAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(1, app.LeftActivationCount, "The most recent RequestFocus call should win even when the control is composed earlier.");
        TestAssert.Equal(0, app.RightActivationCount, "Composition order should not override focus request order.");
        return Task.CompletedTask;
    }

    private static Task FocusRequests_AreOneShotAcrossLaterBuilds()
    {
        var app = new OneShotFocusRequestApp();
        var screen = new TeaAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Tab));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(0, app.LeftActivationCount, "Consumed focus requests should not steal focus back on later builds.");
        TestAssert.Equal(1, app.RightActivationCount, "User focus changes should persist after a one-shot focus request is consumed.");
        return Task.CompletedTask;
    }

    private static Task ScreenBuilder_ComposesAndRoutesDefaultControls()
    {
        var app = new BuilderAuthoredApp();
        var screen = new TeaAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(1, app.ActivationCount, "The imperative Screen.Build path should still compile into routable default controls.");
        return Task.CompletedTask;
    }

    private static Task RootLayouts_UseSceneCompilerInsteadOfLegacyCompiledScreen()
    {
        var app = new BuilderAuthoredApp();
        var screen = new TeaAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();

        var field = typeof(TeaApp).GetField("_interactiveScreen", BindingFlags.Instance | BindingFlags.NonPublic);
        var compiled = field?.GetValue(app);

        TestAssert.True(compiled is not null, "Rendering a root layout should capture an interactive screen snapshot.");
        TestAssert.True(
            compiled!.GetType().FullName != "TeaSharp.Internal.LegacyCompiledScreen",
            "Root layout screens should compile through the new scene compiler instead of the legacy ScreenComposer bridge.");
        return Task.CompletedTask;
    }

    private static Task LegacyCompilerTypes_AreRemovedFromDefaultCompilerPath()
    {
        var assembly = typeof(TeaApp).Assembly;

        TestAssert.True(
            assembly.GetType("TeaSharp.Internal.HybridScreenCompiler", throwOnError: false) is null,
            "HybridScreenCompiler should be removed once built-in root layouts compile directly through TeaSceneCompiler.");
        TestAssert.True(
            assembly.GetType("TeaSharp.Internal.LegacyScreenCompiler", throwOnError: false) is null,
            "LegacyScreenCompiler should no longer exist as a silent fallback in the default compiler path.");
        TestAssert.True(
            assembly.GetType("TeaSharp.Internal.LegacyCompiledScreen", throwOnError: false) is null,
            "LegacyCompiledScreen should be removed once root layout interaction is handled by the new scene compiler.");
        return Task.CompletedTask;
    }

    private static Task LegacyLayoutHelpers_AreInternalized()
    {
        string[] helperTypeNames =
        [
            "TeaSharp.Layout.Stack",
            "TeaSharp.Layout.Split",
            "TeaSharp.Layout.Panel",
            "TeaSharp.Layout.Dock",
            "TeaSharp.Layout.Overlay",
            "TeaSharp.Layout.Center",
            "TeaSharp.Layout.Slot",
        ];

        var assembly = typeof(LayoutSlot).Assembly;

        foreach (var helperTypeName in helperTypeNames)
        {
            var helperType = assembly.GetType(helperTypeName, throwOnError: false);
            TestAssert.True(helperType is not null, $"{helperTypeName} should still exist as an internal legacy bridge.");
            TestAssert.True(
                helperType!.IsNotPublic,
                $"{helperTypeName} should no longer be public on the default composition path.");
        }

        return Task.CompletedTask;
    }

    private static Task RegionKeyInteropOverloads_AreInternalized()
    {
        var internalCtors =
            new (Type Type, Type[] Parameters)[]
            {
                (typeof(LayoutSlot), [typeof(ICanvasComponent), typeof(LayoutLength), typeof(Thickness), typeof(ScreenRegionKey), typeof(int?), typeof(int?), typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
                (typeof(CenterLayout), [typeof(ICanvasComponent), typeof(int?), typeof(int?), typeof(Thickness), typeof(ScreenRegionKey), typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
                (typeof(PanelLayout), [typeof(ICanvasComponent), typeof(string), typeof(BorderStyle), typeof(Thickness), typeof(Thickness), typeof(ScreenRegionKey), typeof(int?), typeof(int?), typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)]),
            };

        foreach (var (type, parameters) in internalCtors)
        {
            var publicCtor = type.GetConstructor(parameters);
            TestAssert.True(publicCtor is null, $"{type.Name} region-key overload should no longer be public.");

            var internalCtor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                binder: null,
                types: parameters,
                modifiers: null);
            TestAssert.True(internalCtor is not null, $"{type.Name} region-key overload should remain as an internal bridge.");
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

    private static Task LowLevelComponentContracts_AreInternalized()
    {
        var canvasContract = typeof(ICanvasComponent);
        var canvasAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            canvasContract,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(canvasAttribute is not null, "ICanvasComponent should remain explicitly marked as advanced.");
        TestAssert.True(canvasAttribute!.State == EditorBrowsableState.Advanced, "ICanvasComponent should stay hidden from the default custom-widget path.");

        string[] contractNames =
        [
            "TeaSharp.Components.Composition.IStatefulComponent",
            "TeaSharp.Components.Composition.IMouseStatefulComponent",
            "TeaSharp.Components.Composition.IFocusableComponent",
            "TeaSharp.Components.Composition.IInteractiveComponent",
        ];

        var assembly = typeof(Screen).Assembly;

        foreach (var contractName in contractNames)
        {
            var contract = assembly.GetType(contractName, throwOnError: false);
            TestAssert.True(contract is not null, $"{contractName} should continue to exist as an internal bridge.");
            TestAssert.True(contract!.IsNotPublic, $"{contractName} should no longer be public.");
        }

        return Task.CompletedTask;
    }

    private static Task LowLevelTreeLayouts_AreInternalized()
    {
        string[] layoutTypeNames =
        [
            "TeaSharp.Layout.StackLayout",
            "TeaSharp.Layout.SplitLayout",
            "TeaSharp.Layout.DockLayout",
            "TeaSharp.Layout.OverlayLayout",
            "TeaSharp.Layout.LayoutOrientation",
        ];

        var assembly = typeof(Screen).Assembly;

        foreach (var typeName in layoutTypeNames)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is not null, $"{typeName} should continue to exist as an internal bridge.");
            TestAssert.True(type!.IsNotPublic, $"{typeName} should no longer be public on the root layout path.");
        }

        return Task.CompletedTask;
    }

    private static Task ComponentLayout_IsInternalized()
    {
        var type = typeof(LayoutSlot).Assembly.GetType("TeaSharp.Layout.ComponentLayout", throwOnError: false);
        TestAssert.True(type is not null, "ComponentLayout should continue to exist as an internal bridge leaf.");
        TestAssert.True(type!.IsNotPublic, "ComponentLayout should no longer be public.");
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

    private sealed class TeaAppDriver
    {
        private readonly TeaApp _app;

        public TeaAppDriver(TeaApp app)
        {
            _app = app;
        }

        public global::TeaSharp.Core.Abstractions.Effect? Update(global::TeaSharp.Core.Abstractions.IMessage message)
        {
            return _app.UpdateCore(message);
        }

        public global::TeaSharp.Core.Abstractions.ScreenOutput Render() => _app.RenderCore();
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

    private sealed class FocusRequestOrderingApp : TeaApp
    {
        public Button LeftButton { get; } = new() { Text = "Left" };

        public Button RightButton { get; } = new() { Text = "Right" };

        public int LeftActivationCount { get; private set; }

        public int RightActivationCount { get; private set; }

        public FocusRequestOrderingApp()
        {
            LeftButton.Activated += (_, _) => LeftActivationCount++;
            RightButton.Activated += (_, _) => RightActivationCount++;

            RightButton.RequestFocus();
            LeftButton.RequestFocus();
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context)
        {
            var row = new RowLayout
            {
                Gap = 2,
            };
            row.AddFixed(LeftButton, 12);
            row.AddFixed(RightButton, 12);

            return Screen.From(new WindowLayout
            {
                Body = new CenterLayout(row, width: 28, height: 3),
            });
        }
    }

    private sealed class OneShotFocusRequestApp : TeaApp
    {
        public Button LeftButton { get; } = new() { Text = "Left" };

        public Button RightButton { get; } = new() { Text = "Right" };

        public int LeftActivationCount { get; private set; }

        public int RightActivationCount { get; private set; }

        public OneShotFocusRequestApp()
        {
            LeftButton.Activated += (_, _) => LeftActivationCount++;
            RightButton.Activated += (_, _) => RightActivationCount++;
            LeftButton.RequestFocus();
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context)
        {
            var row = new RowLayout
            {
                Gap = 2,
            };
            row.AddFixed(LeftButton, 12);
            row.AddFixed(RightButton, 12);

            return Screen.From(new WindowLayout
            {
                Body = new CenterLayout(row, width: 28, height: 3),
            });
        }
    }

    private sealed class BuilderAuthoredApp : TeaApp
    {
        private readonly Button _button = new() { Text = "Run" };
        private readonly StatusBar _status = new();

        public BuilderAuthoredApp()
        {
            _button.Activated += (_, _) => ActivationCount++;
            _button.RequestFocus();
        }

        public int ActivationCount { get; private set; }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context)
        {
            _status.LeftText = "Enter activates";
            _status.RightText = $"Size {context.Width}x{context.Height}";

            return Screen.Build(window =>
            {
                window.Padding(1);
                window.Footer(1, _status);
                window.Body(body => body.Center(_button, width: 18, height: 3));
            });
        }
    }
}
