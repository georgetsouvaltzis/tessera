using TeaSharp;

var app = Tea.CreateBuilder()
    .UseApp<ConsumerOpsStudioApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = ConsumerOpsStudioTheme.Default;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Consumer Ops Studio",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
