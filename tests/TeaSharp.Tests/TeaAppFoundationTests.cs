using TeaSharp.Controls;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class TeaAppFoundationTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TeaApp_ContextTracksResizeMessages",
            TeaApp_ContextTracksResizeMessages);
        yield return new TestCase(
            "TeaApp_InitializeEffect_RoundTripsCustomMessages",
            TeaApp_InitializeEffect_RoundTripsCustomMessages);
        yield return new TestCase(
            "TeaAppBuilder_CreatesConfiguredApplication",
            TeaAppBuilder_CreatesConfiguredApplication);
        yield return new TestCase(
            "TeaControl_Bridge_MapsCoreMessagesToPublicMessages",
            TeaControl_Bridge_MapsCoreMessagesToPublicMessages);
    }

    private static Task TeaApp_ContextTracksResizeMessages()
    {
        var app = new ResizeAwareApp();
        var screen = (IScreen)app;

        screen.Update(new WindowSizeMsg(120, 40));
        var rendered = screen.Render();

        TestAssert.Equal("120x40", rendered.Frame.Content, "TeaApp should expose the latest terminal size through ScreenContext.");
        return Task.CompletedTask;
    }

    private static async Task TeaApp_InitializeEffect_RoundTripsCustomMessages()
    {
        var app = new BootApp();
        var screen = (IScreen)app;

        var init = screen.Init();
        TestAssert.True(init is not null, "TeaApp.Initialize should adapt to the legacy runtime effect contract.");

        var message = await init!(CancellationToken.None);
        TestAssert.True(message is MessageEnvelope, "Custom messages should be wrapped for the legacy runtime.");

        screen.Update(message!);
        var rendered = screen.Render();

        TestAssert.Equal("booted", rendered.Frame.Content, "Custom messages emitted by TeaEffects should round-trip back into TeaApp.Update.");
    }

    private static Task TeaAppBuilder_CreatesConfiguredApplication()
    {
        var application = Tea.CreateBuilder()
            .UseApp<ResizeAwareApp>()
            .ConfigureRuntime(options =>
            {
                options.MaxFps = 24;
                options.Screen = new ScreenOptions
                {
                    AltScreen = true,
                    WindowTitle = "TeaSharp Test",
                };
            })
            .Build();

        TestAssert.True(application.App is ResizeAwareApp, "Tea builder should create the configured app type.");
        TestAssert.Equal(24, application.Options.MaxFps, "Tea builder should preserve runtime options.");
        TestAssert.True(application.Options.Screen.AltScreen == true, "Tea builder should preserve screen defaults.");
        return Task.CompletedTask;
    }

    private static Task TeaControl_Bridge_MapsCoreMessagesToPublicMessages()
    {
        var control = new RecordingControl();
        var stateful = (IStatefulComponent)control.Component;
        var mouseStateful = (IMouseStatefulComponent)control.Component;

        stateful.Update(new KeyPressMsg(KeyCode.Enter));
        mouseStateful.UpdateMouse(new MouseClickMsg(MouseButton.Left, 2, 3), new Rect(0, 0, 10, 10));

        TestAssert.True(control.LastMessage is KeyPressed, "Control should map keyboard input to the new public message model.");
        TestAssert.True(control.LastPointer is PointerInput { Kind: PointerEventKind.Press, X: 2, Y: 3 }, "Control should map pointer input to the new public message model.");
        return Task.CompletedTask;
    }

    private sealed class ResizeAwareApp : TeaApp
    {
        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From($"{context.Width}x{context.Height}");
    }

    private sealed record Booted : Message;

    private sealed class BootApp : TeaApp
    {
        private bool _booted;

        public override TeaEffect? Initialize() => TeaEffects.Emit(new Booted());

        public override TeaEffect? Update(Message message)
        {
            if (message is Booted)
            {
                _booted = true;
            }

            return null;
        }

        public override Screen Build(ScreenContext context) => Screen.From(_booted ? "booted" : "cold");
    }

    private sealed class RecordingControl : Control
    {
        public Message? LastMessage { get; private set; }

        public Message? LastPointer { get; private set; }

        public override void Render(Canvas canvas, Rect rect)
        {
        }

        public override bool Handle(Message message)
        {
            LastMessage = message;
            return true;
        }

        public override bool Handle(Message message, Rect bounds)
        {
            LastPointer = message;
            return true;
        }
    }
}
