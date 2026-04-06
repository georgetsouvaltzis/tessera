using Tessera;
using Tessera.Controls;
using Tessera.Examples.OpsWatch;

var app = TesseraApplication.CreateBuilder()
    .UseApp<OpsWatchApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = OpsWatchTheme.Default.Theme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera OpsWatch",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
