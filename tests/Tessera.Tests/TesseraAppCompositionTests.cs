using System.ComponentModel;
using System.Reflection;
using Tessera.Components.Composition;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;
using Tessera.Internal;
using Tessera.Layout;

namespace Tessera.Tests;

internal static class TesseraAppCompositionTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TesseraAppComposition_AutomaticallyRoutesButtonActivation",
            AutomaticallyRoutesButtonActivation);
        yield return new TestCase(
            "TesseraAppComposition_AutomaticallyRoutesTabIntoTextInput",
            AutomaticallyRoutesTabIntoTextInput);
        yield return new TestCase(
            "TesseraAppComposition_AutomaticallyRoutesChoiceSelection",
            AutomaticallyRoutesChoiceSelection);
        yield return new TestCase(
            "TesseraAppComposition_AutomaticallyRoutesComboBoxSelection",
            AutomaticallyRoutesComboBoxSelection);
        yield return new TestCase(
            "TesseraAppComposition_AutomaticallyRoutesTabsSelection",
            AutomaticallyRoutesTabsSelection);
        yield return new TestCase(
            "TesseraAppComposition_AutomaticallyRoutesMenuBarActivation",
            AutomaticallyRoutesMenuBarActivation);
        yield return new TestCase(
            "TesseraAppComposition_HandledControlKeyInput_StillReachesAppUpdate",
            HandledControlKeyInput_StillReachesAppUpdate);
        yield return new TestCase(
            "TesseraAppComposition_RequestEffect_AllowsHandledControlInputToTriggerRuntimeEffect",
            RequestEffect_AllowsHandledControlInputToTriggerRuntimeEffect);
        yield return new TestCase(
            "TesseraAppComposition_GlobalHotkeys_ReachAppUpdate_WhenFocusedControlConsumesKeyEvents",
            GlobalHotkeys_ReachAppUpdate_WhenFocusedControlConsumesKeyEvents);
        yield return new TestCase(
            "TesseraAppComposition_VisibleOverlayCanClaimFocusThroughRootLayouts",
            VisibleOverlayCanClaimFocusThroughRootLayouts);
        yield return new TestCase(
            "TesseraAppComposition_DialogShow_RequestsFocusForOverlayComposition",
            DialogShow_RequestsFocusForOverlayComposition);
        yield return new TestCase(
            "TesseraAppComposition_FocusRequestsPreferLatestRequestOverCompositionOrder",
            FocusRequestsPreferLatestRequestOverCompositionOrder);
        yield return new TestCase(
            "TesseraAppComposition_FocusRequests_AreOneShotAcrossLaterBuilds",
            FocusRequests_AreOneShotAcrossLaterBuilds);
        yield return new TestCase(
            "TesseraAppComposition_ScreenBuilder_ComposesAndRoutesDefaultControls",
            ScreenBuilder_ComposesAndRoutesDefaultControls);
        yield return new TestCase(
            "TesseraAppComposition_RootLayouts_UseSceneCompilerInsteadOfLegacyCompiledScreen",
            RootLayouts_UseSceneCompilerInsteadOfLegacyCompiledScreen);
        yield return new TestCase(
            "TesseraAppComposition_LegacyCompilerTypes_AreRemovedFromDefaultCompilerPath",
            LegacyCompilerTypes_AreRemovedFromDefaultCompilerPath);
        yield return new TestCase(
            "TesseraAppComposition_LegacyLayoutHelpers_AreRemoved",
            LegacyLayoutHelpers_AreRemoved);
        yield return new TestCase(
            "TesseraAppComposition_LegacyCanvasComponentBridgeCtors_AreRemoved",
            LegacyCanvasComponentBridgeCtors_AreRemoved);
        yield return new TestCase(
            "TesseraAppComposition_LegacyCanvasComponentEntryPoints_AreMarkedAdvanced",
            LegacyCanvasComponentEntryPoints_AreMarkedAdvanced);
        yield return new TestCase(
            "TesseraAppComposition_LowLevelComponentContracts_AreRemoved",
            LowLevelComponentContracts_AreRemoved);
        yield return new TestCase(
            "TesseraAppComposition_LowLevelTreeLayouts_AreInternalized",
            LowLevelTreeLayouts_AreInternalized);
        yield return new TestCase(
            "TesseraAppComposition_ComponentLayout_IsInternalized",
            ComponentLayout_IsInternalized);
        yield return new TestCase(
            "TesseraAppComposition_ScreenAssemblyLayouts_RemainDiscoverable",
            ScreenAssemblyLayouts_RemainDiscoverable);
    }

    private static Task AutomaticallyRoutesButtonActivation()
    {
        var app = new ButtonApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(1, app.ActivationCount,
            "Enter should activate the focused button automatically before Update.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesTabIntoTextInput()
    {
        var app = new FormApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Tab));
        screen.Update(new KeyPressMsg(KeyCode.Character, "x"));
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("x", app.Input.Value,
            "Tab should move focus to the next control and route typing into the text input.");
        TestAssert.Equal("x", app.LastSubmittedValue, "Enter should raise submission through the routed text input.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesChoiceSelection()
    {
        var app = new ChoiceApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Down));
        screen.Update(new KeyPressMsg(KeyCode.Down));
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("History", app.Choice.SelectedItem,
            "Choice should route open, navigate, and confirm through the compiled screen.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesComboBoxSelection()
    {
        var app = new ComboBoxApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "w"));
        screen.Update(new KeyPressMsg(KeyCode.Down));
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("west", app.ComboBox.SelectedItem,
            "ComboBox should route filter, navigate, and confirm through the compiled screen.");
        return Task.CompletedTask;
    }

    private static Task AutomaticallyRoutesTabsSelection()
    {
        var app = new TabsApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "2"));

        TestAssert.Equal(1, app.Tabs.SelectedIndex, "Tabs should route numeric shortcuts through the compiled screen.");
        return Task.CompletedTask;
    }

    private static async Task AutomaticallyRoutesMenuBarActivation()
    {
        var app = new MenuApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "r"));

        await Task.Yield();
        TestAssert.Equal("refresh", app.LastActivatedItemId,
            "MenuBar activation should preserve the configured item id.");
    }

    private static Task HandledControlKeyInput_StillReachesAppUpdate()
    {
        var app = new FilteredInputApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(1, app.KeyUpdateCount,
            "Handled key input should still flow into app Update for global hotkey handling.");
        TestAssert.Equal(1, app.ActivationCount, "The control should still receive the handled key.");
        return Task.CompletedTask;
    }

    private static Task RequestEffect_AllowsHandledControlInputToTriggerRuntimeEffect()
    {
        var app = new RequestedEffectApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        var effect = screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(effect is not null, "Handled control input should still be able to schedule runtime effects.");
        TestAssert.Equal(1, app.KeyUpdateCount,
            "Handled key input should still reach app Update while preserving control-side effects.");
        return Task.CompletedTask;
    }

    private static Task GlobalHotkeys_ReachAppUpdate_WhenFocusedControlConsumesKeyEvents()
    {
        var app = new GlobalHotkeyApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Character, "c", KeyModifiers.Ctrl));
        screen.Update(new KeyPressMsg(KeyCode.Character, "t"));
        screen.Update(new KeyReleaseMsg(KeyCode.Character, "t"));

        TestAssert.True(app.QuitRequested,
            "Ctrl+C should reach app Update even when a focused control consumes key messages.");
        TestAssert.Equal(1, app.ThemeToggleCount,
            "Theme hotkey should reach app Update even when the focused control consumes key messages.");
        TestAssert.Equal(1, app.KeyReleaseCount, "Key release should reach app Update under the same conditions.");
        return Task.CompletedTask;
    }

    private static Task VisibleOverlayCanClaimFocusThroughRootLayouts()
    {
        var app = new OverlayPaletteApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        TestAssert.True(app.Palette.IsFocused, "Visible overlay should own focus after the screen is composed.");
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal("rollback", app.LastExecutedItemId,
            "Visible overlays should be able to claim focus through the root layout model.");
        TestAssert.Equal(0, app.ButtonActivationCount,
            "Overlay focus should keep the underlying body control from activating.");
        return Task.CompletedTask;
    }

    private static Task DialogShow_RequestsFocusForOverlayComposition()
    {
        var app = new OverlayDialogApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        TestAssert.True(app.Dialog.IsFocused, "Dialog.Show should request focus for the next composition pass.");
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(app.Dialog.LastResult == DialogResult.Accepted,
            "Enter should be routed into the shown dialog.");
        TestAssert.Equal(0, app.ButtonActivationCount,
            "Dialog focus should keep the underlying body control from activating.");
        return Task.CompletedTask;
    }

    private static Task FocusRequestsPreferLatestRequestOverCompositionOrder()
    {
        var app = new FocusRequestOrderingApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(1, app.LeftActivationCount,
            "The most recent RequestFocus call should win even when the control is composed earlier.");
        TestAssert.Equal(0, app.RightActivationCount, "Composition order should not override focus request order.");
        return Task.CompletedTask;
    }

    private static Task FocusRequests_AreOneShotAcrossLaterBuilds()
    {
        var app = new OneShotFocusRequestApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Tab));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(0, app.LeftActivationCount,
            "Consumed focus requests should not steal focus back on later builds.");
        TestAssert.Equal(1, app.RightActivationCount,
            "User focus changes should persist after a one-shot focus request is consumed.");
        return Task.CompletedTask;
    }

    private static Task ScreenBuilder_ComposesAndRoutesDefaultControls()
    {
        var app = new BuilderAuthoredApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.Equal(1, app.ActivationCount,
            "The imperative Screen.Build path should still compile into routable default controls.");
        return Task.CompletedTask;
    }

    private static Task RootLayouts_UseSceneCompilerInsteadOfLegacyCompiledScreen()
    {
        var app = new BuilderAuthoredApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();

        var field = typeof(TesseraApp).GetField("_interactiveScreen", BindingFlags.Instance | BindingFlags.NonPublic);
        var compiled = field?.GetValue(app);

        TestAssert.True(compiled is not null, "Rendering a root layout should capture an interactive screen snapshot.");
        TestAssert.True(
            compiled!.GetType().FullName != "Tessera.Internal.LegacyCompiledScreen",
            "Root layout screens should compile through the new scene compiler instead of the legacy ScreenComposer bridge.");
        return Task.CompletedTask;
    }

    private static Task LegacyCompilerTypes_AreRemovedFromDefaultCompilerPath()
    {
        var assembly = typeof(TesseraApp).Assembly;

        TestAssert.True(
            assembly.GetType("Tessera.Internal.HybridScreenCompiler", false) is null,
            "HybridScreenCompiler should be removed once built-in root layouts compile directly through TesseraSceneCompiler.");
        TestAssert.True(
            assembly.GetType("Tessera.Internal.LegacyScreenCompiler", false) is null,
            "LegacyScreenCompiler should no longer exist as a silent fallback in the default compiler path.");
        TestAssert.True(
            assembly.GetType("Tessera.Internal.LegacyCompiledScreen", false) is null,
            "LegacyCompiledScreen should be removed once root layout interaction is handled by the new scene compiler.");
        return Task.CompletedTask;
    }

    private static Task LegacyLayoutHelpers_AreRemoved()
    {
        string[] helperTypeNames =
        [
            "Tessera.Layout.Stack",
            "Tessera.Layout.Split",
            "Tessera.Layout.Panel",
            "Tessera.Layout.Dock",
            "Tessera.Layout.Overlay",
            "Tessera.Layout.Center",
            "Tessera.Layout.Slot"
        ];

        var assembly = typeof(LayoutSlot).Assembly;

        foreach (var helperTypeName in helperTypeNames)
        {
            var helperType = assembly.GetType(helperTypeName, false);
            TestAssert.True(helperType is null,
                $"{helperTypeName} should be removed once object-based layout assembly is the only supported path.");
        }

        return Task.CompletedTask;
    }

    private static Task LegacyCanvasComponentBridgeCtors_AreRemoved()
    {
        var removedCtors =
            new (Type Type, Type[] Parameters)[]
            {
                (typeof(LayoutSlot),
                [
                    typeof(ICanvasComponent), typeof(LayoutLength), typeof(Thickness), typeof(int?), typeof(int?),
                    typeof(bool?), typeof(bool), typeof(bool), typeof(int), typeof(Action)
                ]),
                (typeof(CenterLayout),
                [
                    typeof(ICanvasComponent), typeof(int?), typeof(int?), typeof(Thickness), typeof(bool?),
                    typeof(bool), typeof(bool), typeof(int), typeof(Action)
                ]),
                (typeof(PanelLayout),
                [
                    typeof(ICanvasComponent), typeof(string), typeof(BorderStyle), typeof(Thickness),
                    typeof(Thickness), typeof(int?), typeof(int?), typeof(bool?), typeof(bool), typeof(bool),
                    typeof(int), typeof(Action)
                ])
            };

        foreach (var (type, parameters) in removedCtors)
        {
            var publicCtor = type.GetConstructor(parameters);
            TestAssert.True(publicCtor is null,
                $"{type.Name} legacy advanced canvas bridge constructor should be removed.");

            var internalCtor = type.GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                parameters,
                null);
            TestAssert.True(internalCtor is null,
                $"{type.Name} legacy advanced canvas bridge constructor should be removed instead of kept internal.");
        }

        return Task.CompletedTask;
    }

    private static Task LegacyCanvasComponentEntryPoints_AreMarkedAdvanced()
    {
        var advancedMethods =
            new (Type Type, string Name, Type[] Parameters)[]
            {
                (typeof(Screen), nameof(Screen.From), [typeof(ICanvasComponent)]),
                (typeof(LayoutSlot), nameof(LayoutSlot.Auto), [typeof(ICanvasComponent), typeof(Thickness)]), (
                    typeof(LayoutSlot), nameof(LayoutSlot.Fixed),
                    [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(LayoutSlot), nameof(LayoutSlot.Fill), [typeof(ICanvasComponent), typeof(Thickness)]), (
                    typeof(LayoutSlot), nameof(LayoutSlot.Weighted),
                    [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(RowLayout), nameof(RowLayout.AddAuto), [typeof(ICanvasComponent), typeof(Thickness)]), (
                    typeof(RowLayout), nameof(RowLayout.AddFixed),
                    [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(RowLayout), nameof(RowLayout.AddFill), [typeof(ICanvasComponent), typeof(Thickness)]), (
                    typeof(RowLayout), nameof(RowLayout.AddWeighted),
                    [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(ColumnLayout), nameof(ColumnLayout.AddAuto), [typeof(ICanvasComponent), typeof(Thickness)]), (
                    typeof(ColumnLayout), nameof(ColumnLayout.AddFixed),
                    [typeof(ICanvasComponent), typeof(int), typeof(Thickness)]),
                (typeof(ColumnLayout), nameof(ColumnLayout.AddFill), [typeof(ICanvasComponent), typeof(Thickness)]), (
                    typeof(ColumnLayout), nameof(ColumnLayout.AddWeighted),
                    [typeof(ICanvasComponent), typeof(int), typeof(Thickness)])
            };

        foreach (var (type, name, parameters) in advancedMethods)
        {
            var method = type.GetMethod(name, parameters);
            TestAssert.True(method is not null,
                $"{type.Name}.{name} legacy component overload should exist for advanced callers.");
            var attribute =
                (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(method!, typeof(EditorBrowsableAttribute));
            TestAssert.True(attribute is not null,
                $"{type.Name}.{name} legacy component overload should be marked advanced.");
            TestAssert.True(attribute!.State == EditorBrowsableState.Advanced,
                $"{type.Name}.{name} legacy component overload should be hidden from the default path.");
        }

        var advancedCtors =
            new (Type Type, Type[] Parameters)[]
            {
                (typeof(LayoutSlot), [typeof(ICanvasComponent), typeof(LayoutLength), typeof(Thickness)]),
                (typeof(CenterLayout), [typeof(ICanvasComponent), typeof(int?), typeof(int?), typeof(Thickness)]), (
                    typeof(PanelLayout),
                    [
                        typeof(ICanvasComponent), typeof(string), typeof(BorderStyle), typeof(Thickness),
                        typeof(Thickness)
                    ])
            };

        foreach (var (type, parameters) in advancedCtors)
        {
            var ctor = type.GetConstructor(parameters);
            TestAssert.True(ctor is not null,
                $"{type.Name} legacy component constructor should exist for advanced callers.");
            var attribute =
                (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(ctor!, typeof(EditorBrowsableAttribute));
            TestAssert.True(attribute is not null,
                $"{type.Name} legacy component constructor should be marked advanced.");
            TestAssert.True(attribute!.State == EditorBrowsableState.Advanced,
                $"{type.Name} legacy component constructor should be hidden from the default path.");
        }

        return Task.CompletedTask;
    }

    private static Task LowLevelComponentContracts_AreRemoved()
    {
        var canvasContract = typeof(ICanvasComponent);
        var canvasAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            canvasContract,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(canvasAttribute is not null, "ICanvasComponent should remain explicitly marked as advanced.");
        TestAssert.True(canvasAttribute!.State == EditorBrowsableState.Advanced,
            "ICanvasComponent should stay hidden from the default custom-widget path.");

        string[] contractNames =
        [
            "Tessera.Components.Composition.IStatefulComponent",
            "Tessera.Components.Composition.IMouseStatefulComponent",
            "Tessera.Components.Composition.IFocusableComponent",
            "Tessera.Components.Composition.IInteractiveComponent"
        ];

        var assembly = typeof(Screen).Assembly;

        foreach (var contractName in contractNames)
        {
            var contract = assembly.GetType(contractName, false);
            TestAssert.True(contract is null,
                $"{contractName} should be removed once the scene compiler owns control interaction directly.");
        }

        return Task.CompletedTask;
    }

    private static Task LowLevelTreeLayouts_AreInternalized()
    {
        string[] layoutTypeNames =
        [
            "Tessera.Layout.StackLayout",
            "Tessera.Layout.SplitLayout",
            "Tessera.Layout.DockLayout",
            "Tessera.Layout.OverlayLayout",
            "Tessera.Layout.LayoutOrientation"
        ];

        var assembly = typeof(Screen).Assembly;

        foreach (var typeName in layoutTypeNames)
        {
            var type = assembly.GetType(typeName, false);
            TestAssert.True(type is not null, $"{typeName} should continue to exist as an internal bridge.");
            TestAssert.True(type!.IsNotPublic, $"{typeName} should no longer be public on the root layout path.");
        }

        return Task.CompletedTask;
    }

    private static Task ComponentLayout_IsInternalized()
    {
        var type = typeof(LayoutSlot).Assembly.GetType("Tessera.Layout.ComponentLayout", false);
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
            typeof(LayoutSlot)
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

    private sealed class TesseraAppDriver
    {
        private readonly TesseraApp _app;

        public TesseraAppDriver(TesseraApp app)
        {
            _app = app;
        }

        public Effect? Update(IMessage message)
        {
            return TesseraEffectAdapter.ToCore(_app.UpdateRuntime(TesseraMessageAdapter.ToPublic(message)));
        }

        public void Render()
        {
            _ = _app.RenderRuntime().Output;
        }
    }

    private sealed class ButtonApp : TesseraApp
    {
        public ButtonApp()
        {
            Button.Activated += (_, _) => ActivationCount++;
        }

        public Button Button { get; } = new() { Text = "Run" };

        public int ActivationCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout { Body = new CenterLayout(Button, 16, 3) });
        }
    }

    private sealed class FormApp : TesseraApp
    {
        public FormApp()
        {
            Input.Submitted += (_, args) => LastSubmittedValue = args.Value;
        }

        public Button Button { get; } = new() { Text = "Send" };

        public TextInput Input { get; } = new() { Title = "Command" };

        public string LastSubmittedValue { get; private set; } = string.Empty;

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            var fields = new ColumnLayout { Gap = 1 };
            fields.AddFixed(Button, 3);
            fields.AddFixed(Input, 3);

            return Screen.From(new WindowLayout
            {
                Body = new CenterLayout(
                    new PanelLayout(fields, "Form", BorderStyle.SingleLine, Thickness.All(1)),
                    28,
                    10)
            });
        }
    }

    private sealed class ChoiceApp : TesseraApp
    {
        public ChoiceApp()
        {
            Choice.SetItems(["Open", "History", "Archived"]);
        }

        public Choice Choice { get; } = new() { Title = "Tab" };

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout { Body = new CenterLayout(Choice, 28, 6) });
        }
    }

    private sealed class TabsApp : TesseraApp
    {
        public Tabs Tabs { get; } = new("Open", "History", "Archived");

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout { Body = new CenterLayout(Tabs, 36, 1) });
        }
    }

    private sealed class ComboBoxApp : TesseraApp
    {
        public ComboBoxApp()
        {
            ComboBox.SetItems(["east", "west", "north"]);
        }

        public ComboBox ComboBox { get; } = new() { Title = "Regions", IsFocused = true };

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout { Body = new CenterLayout(ComboBox, 28, 6) });
        }
    }

    private sealed class MenuApp : TesseraApp
    {
        public MenuApp()
        {
            Menu.SetItems([new MenuItem("refresh", "Refresh", 'r')]);
            Menu.ItemActivated += (_, args) => LastActivatedItemId = args.ItemId;
        }

        public MenuBar Menu { get; } = new();

        public string LastActivatedItemId { get; private set; } = string.Empty;

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout { Body = new CenterLayout(Menu, 24, 1) });
        }
    }

    private sealed class FilteredInputApp : TesseraApp
    {
        public FilteredInputApp()
        {
            Button.Activated += (_, _) => ActivationCount++;
        }

        public Button Button { get; } = new() { Text = "Run" };

        public int ActivationCount { get; private set; }

        public int KeyUpdateCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            if (message is KeyPressed)
            {
                KeyUpdateCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout { Body = new CenterLayout(Button, 16, 3) });
        }
    }

    private sealed class RequestedEffectApp : TesseraApp
    {
        public RequestedEffectApp()
        {
            Button.Activated += (_, _) => RequestEffect(TesseraEffects.Quit);
        }

        public Button Button { get; } = new() { Text = "Quit" };

        public int KeyUpdateCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            if (message is KeyPressed)
            {
                KeyUpdateCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout { Body = new CenterLayout(Button, 16, 3) });
        }
    }

    private sealed class GlobalHotkeyApp : TesseraApp
    {
        public ConsumingKeyControl Control { get; } = new();

        public bool QuitRequested { get; private set; }

        public int ThemeToggleCount { get; private set; }

        public int KeyReleaseCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            if (message is KeyPressed key)
            {
                if (key.IsCharacter('c', ModifierKeys.Ctrl))
                {
                    QuitRequested = true;
                }

                if (key.IsCharacter('t'))
                {
                    ThemeToggleCount++;
                }
            }

            if (message is KeyReleased)
            {
                KeyReleaseCount++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout { Body = new CenterLayout(Control, 18, 3) });
        }
    }

    private sealed class ConsumingKeyControl : Control
    {
        public override void Render(Canvas canvas, Rect rect)
        {
        }

        public override bool Handle(Message message)
        {
            return message is KeyPressed or KeyReleased;
        }
    }

    private sealed class OverlayPaletteApp : TesseraApp
    {
        public OverlayPaletteApp()
        {
            Button.Activated += (_, _) => ButtonActivationCount++;
            Palette.SetItems(
            [
                new CommandPaletteItem("deploy", "Deploy", "publish release"),
                new CommandPaletteItem("rollback", "Rollback", "restore previous")
            ]);
            Palette.ItemExecuted += (_, args) => LastExecutedItemId = args.ItemId;
            Palette.Open();
            Palette.QueryText = "roll";
        }

        public Button Button { get; } = new() { Text = "Base" };

        public CommandPalette Palette { get; } = new() { Title = "Actions" };

        public int ButtonActivationCount { get; private set; }

        public string LastExecutedItemId { get; private set; } = string.Empty;

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Button, 16, 3),
                Overlay = new CenterLayout(Palette, 48, 10)
            });
        }
    }

    private sealed class FocusRequestOrderingApp : TesseraApp
    {
        public FocusRequestOrderingApp()
        {
            LeftButton.Activated += (_, _) => LeftActivationCount++;
            RightButton.Activated += (_, _) => RightActivationCount++;

            RightButton.RequestFocus();
            LeftButton.RequestFocus();
        }

        public Button LeftButton { get; } = new() { Text = "Left" };

        public Button RightButton { get; } = new() { Text = "Right" };

        public int LeftActivationCount { get; private set; }

        public int RightActivationCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            var row = new RowLayout { Gap = 2 };
            row.AddFixed(LeftButton, 12);
            row.AddFixed(RightButton, 12);

            return Screen.From(new WindowLayout { Body = new CenterLayout(row, 28, 3) });
        }
    }

    private sealed class OverlayDialogApp : TesseraApp
    {
        public OverlayDialogApp()
        {
            Button.Activated += (_, _) => ButtonActivationCount++;
            Dialog.Show("Confirm", "Apply changes?");
        }

        public Button Button { get; } = new() { Text = "Primary" };

        public Dialog Dialog { get; } = new() { Padding = Thickness.All(1) };

        public int ButtonActivationCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(new WindowLayout
            {
                Body = new CenterLayout(Button, 18, 3),
                Overlay = new CenterLayout(Dialog, 48, 10)
            });
        }
    }

    private sealed class OneShotFocusRequestApp : TesseraApp
    {
        public OneShotFocusRequestApp()
        {
            LeftButton.Activated += (_, _) => LeftActivationCount++;
            RightButton.Activated += (_, _) => RightActivationCount++;
            LeftButton.RequestFocus();
        }

        public Button LeftButton { get; } = new() { Text = "Left" };

        public Button RightButton { get; } = new() { Text = "Right" };

        public int LeftActivationCount { get; private set; }

        public int RightActivationCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            var row = new RowLayout { Gap = 2 };
            row.AddFixed(LeftButton, 12);
            row.AddFixed(RightButton, 12);

            return Screen.From(new WindowLayout { Body = new CenterLayout(row, 28, 3) });
        }
    }

    private sealed class BuilderAuthoredApp : TesseraApp
    {
        private readonly Button _button = new() { Text = "Run" };
        private readonly StatusBar _status = new();

        public BuilderAuthoredApp()
        {
            _button.Activated += (_, _) => ActivationCount++;
            _button.RequestFocus();
        }

        public int ActivationCount { get; private set; }

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            _status.LeftText = "Enter activates";
            _status.RightText = $"Size {context.Width}x{context.Height}";

            return Screen.Build(window =>
            {
                window.Padding(1);
                window.Footer(1, _status);
                window.Body(body => body.Center(_button, 18, 3));
            });
        }
    }
}
