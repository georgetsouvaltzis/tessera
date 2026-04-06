using Tessera.Controls;
using Tessera.Components.Primitives;
using Tessera.Core.Messages;
using Tessera.Internal;
using System.Reflection;

namespace Tessera.Tests;

internal static class TesseraAppFoundationTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "TesseraApp_ContextTracksResizeMessages",
            TesseraApp_ContextTracksResizeMessages);
        yield return new TestCase(
            "TesseraApp_InitializeEffect_RoundTripsCustomMessages",
            TesseraApp_InitializeEffect_RoundTripsCustomMessages);
        yield return new TestCase(
            "TesseraAppBuilder_CreatesConfiguredApplication",
            TesseraAppBuilder_CreatesConfiguredApplication);
        yield return new TestCase(
            "TesseraApp_Post_QueuesControlEventMessagesAsEffects",
            TesseraApp_Post_QueuesControlEventMessagesAsEffects);
        yield return new TestCase(
            "TesseraApp_RuntimeScreenBridge_IsRemoved",
            TesseraApp_RuntimeScreenBridge_IsRemoved);
        yield return new TestCase(
            "TesseraApplication_RuntimePath_DoesNotStoreLegacyProgramWrapper",
            TesseraApplication_RuntimePath_DoesNotStoreLegacyProgramWrapper);
        yield return new TestCase(
            "TesseraRuntimeBridge_RuntimePath_DoesNotStoreLegacyProgramWrapper",
            TesseraRuntimeBridge_RuntimePath_DoesNotStoreLegacyProgramWrapper);
    }

    private static Task TesseraApp_ContextTracksResizeMessages()
    {
        var app = new ResizeAwareApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(120, 40));
        var rendered = screen.Render();

        TestAssert.Equal("120x40", rendered.Frame.Content, "TesseraApp should expose the latest terminal size through ScreenContext.");
        return Task.CompletedTask;
    }

    private static async Task TesseraApp_InitializeEffect_RoundTripsCustomMessages()
    {
        var app = new BootApp();
        var screen = new TesseraAppDriver(app);

        var init = screen.Init();
        TestAssert.True(init is not null, "TesseraApp.Initialize should adapt to the runtime effect contract.");

        var message = await init!(CancellationToken.None);
        TestAssert.True(message is not null, "Custom messages should be wrapped for runtime delivery.");

        screen.Update(message!);
        var rendered = screen.Render();

        TestAssert.Equal("booted", rendered.Frame.Content, "Custom messages emitted by TesseraEffects should round-trip back into TesseraApp.Update.");
    }

    private static Task TesseraAppBuilder_CreatesConfiguredApplication()
    {
        var application = TesseraApplication.CreateBuilder()
            .UseApp<ResizeAwareApp>()
            .ConfigureRuntime(options =>
            {
                options.MaxFps = 24;
                options.Screen = new ScreenOptions
                {
                    AltScreen = true,
                    WindowTitle = "Tessera Test",
                };
            })
            .Build();

        TestAssert.True(application.App is ResizeAwareApp, "Tessera builder should create the configured app type.");
        TestAssert.Equal(24, application.Options.MaxFps, "Tessera builder should preserve runtime options.");
        TestAssert.True(application.Options.Screen.AltScreen == true, "Tessera builder should preserve screen defaults.");
        return Task.CompletedTask;
    }

    private static async Task TesseraApp_Post_QueuesControlEventMessagesAsEffects()
    {
        var app = new PostingApp();
        var screen = new TesseraAppDriver(app);

        screen.Update(new WindowSizeMsg(80, 24));
        screen.Render();
        var effect = screen.Update(new KeyPressMsg(KeyCode.Enter));

        TestAssert.True(effect is not null, "Post should queue a follow-up effect when a control event emits a message.");

        var message = await effect!(CancellationToken.None);
        TestAssert.True(message is not null, "Post should emit a follow-up message.");
        TestAssert.Equal(0, app.Count, "Post should not mutate app state until the queued message is processed by the runtime.");
    }

    private static Task TesseraApp_RuntimeScreenBridge_IsRemoved()
    {
        var runtimeScreen = typeof(TesseraApp).GetProperty("RuntimeScreen", BindingFlags.Instance | BindingFlags.NonPublic);
        var adapterType = typeof(TesseraApp).Assembly.GetType("Tessera.Internal.TesseraAppRuntimeScreen", throwOnError: false);

        TestAssert.True(runtimeScreen is null, "TesseraApp should no longer expose a runtime-screen adapter property.");
        TestAssert.True(adapterType is null, "Tessera should no longer ship a TesseraAppRuntimeScreen adapter type.");
        return Task.CompletedTask;
    }

    private static Task TesseraApplication_RuntimePath_DoesNotStoreLegacyProgramWrapper()
    {
        const string legacyTypeName = "Tessera.Core.Application.TesseraProgram";
        var fields = typeof(TesseraApplication).GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        var legacyField = fields.FirstOrDefault(field => field.FieldType.FullName == legacyTypeName);

        TestAssert.True(
            legacyField is null,
            "TesseraApplication should depend on the internal runtime seam rather than storing TesseraProgram directly.");
        return Task.CompletedTask;
    }

    private static Task TesseraRuntimeBridge_RuntimePath_DoesNotStoreLegacyProgramWrapper()
    {
        const string legacyTypeName = "Tessera.Core.Application.TesseraProgram";
        var runtimeType = typeof(TesseraApplication).Assembly.GetType("Tessera.Internal.TesseraAppRuntime", throwOnError: true)!;
        var fields = runtimeType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        var legacyField = fields.FirstOrDefault(field => field.FieldType.FullName == legacyTypeName);

        TestAssert.True(
            legacyField is null,
            "TesseraAppRuntime should depend on the extracted runtime loop rather than storing TesseraProgram directly.");
        return Task.CompletedTask;
    }

    private sealed class ResizeAwareApp : TesseraApp
    {
        public override TesseraEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From($"{context.Width}x{context.Height}");
    }

    private sealed record Booted : Message;

    private sealed class BootApp : TesseraApp
    {
        private bool _booted;

        public override TesseraEffect? Initialize() => TesseraEffects.Emit(new Booted());

        public override TesseraEffect? Update(Message message)
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

    private sealed record IncrementRequested : Message;

    private sealed class TesseraAppDriver
    {
        private readonly TesseraApp _app;

        public TesseraAppDriver(TesseraApp app)
        {
            _app = app;
        }

        public global::Tessera.Core.Abstractions.Effect? Init() => TesseraEffectAdapter.ToCore(_app.InitializeRuntime());

        public global::Tessera.Core.Abstractions.Effect? Update(global::Tessera.Core.Abstractions.IMessage message) =>
            TesseraEffectAdapter.ToCore(_app.UpdateRuntime(TesseraMessageAdapter.ToPublic(message)));

        public global::Tessera.Core.Abstractions.ScreenOutput Render() => _app.RenderRuntime().Output;
    }

    private sealed class PostingApp : TesseraApp
    {
        private readonly Button _button = new()
        {
            Text = "Increment",
            IsFocused = true,
        };

        private int _count;

        public int Count => _count;

        public PostingApp()
        {
            _button.Activated += (_, _) => Post(new IncrementRequested());
        }

        public override TesseraEffect? Update(Message message)
        {
            if (message is IncrementRequested)
            {
                _count++;
            }

            return null;
        }

        public override Screen Build(ScreenContext context) => Screen.From(_button);
    }
}
