using Tessera;
using Tessera.Examples.DataWorkbench;

var app = TesseraApplication.CreateBuilder()
    .UseApp<DataWorkbenchApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = DataWorkbenchTheme.Default.Theme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera DataWorkbench",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build()
    ;

await app.RunAsync();
