using NUnit.Framework;
using Tessera.Components.Primitives;
using Tessera.Controls;

namespace Tessera.Tests;

[TestFixture]
public sealed class TesseraSceneCompilerPointerVisibilityTests
{
    [Test]
    public void TesseraSceneCompilerHiddenDialogOverlayDoesNotInterceptPointerPress()
    {
        var app = new OverlayPointerProbeApp(showDialog: false);
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
            });

        _ = app.UpdateRuntime(new WindowResized(100, 30));
        _ = app.RenderRuntime();
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 2, 2));
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 50, 15));

        Assert.That(
            app.PointerPressCount,
            Is.EqualTo(2),
            "Hidden overlay controls should not intercept pointer hit-testing for underlying controls.");
    }

    [Test]
    public void TesseraSceneCompilerVisibleDialogOverlayContinuesToInterceptPointerPress()
    {
        var app = new OverlayPointerProbeApp(showDialog: true);
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions
            {
                PointerActivationPolicy = PointerActivationPolicy.SingleClick,
            });

        _ = app.UpdateRuntime(new WindowResized(100, 30));
        _ = app.RenderRuntime();
        _ = app.UpdateRuntime(new PointerInput(PointerEventKind.Press, PointerButton.Left, 50, 15));

        Assert.That(
            app.PointerPressCount,
            Is.EqualTo(0),
            "Visible overlays should keep blocking pointer passthrough to underlying controls.");
    }

    private sealed class OverlayPointerProbeApp : TesseraApp
    {
        private readonly Dialog _dialog;
        private readonly PointerProbeControl _probe = new();

        public OverlayPointerProbeApp(bool showDialog)
        {
            _dialog = new Dialog
            {
                Title = "Confirm",
                BodyLines = ["Overlay"],
                IsVisible = showDialog,
                IsFocused = showDialog,
            };
        }

        public int PointerPressCount => _probe.PointerPressCount;

        public override TesseraEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context)
        {
            return Screen.Build(window =>
            {
                window.Body(_probe);
                window.Overlay(overlay => overlay.Center(_dialog, width: 40, height: 12));
            });
        }
    }

    private sealed class PointerProbeControl : Control
    {
        public int PointerPressCount { get; private set; }

        public override void Render(Canvas canvas, Rect rect)
        {
        }

        public override bool Handle(Message message, Rect bounds)
        {
            if (message is PointerInput { Kind: PointerEventKind.Press, Button: PointerButton.Left } pointer
                && bounds.Contains(pointer.X, pointer.Y))
            {
                PointerPressCount++;
                return true;
            }

            return false;
        }
    }
}
