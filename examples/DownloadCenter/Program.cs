using Tessera;
using Tessera.Examples.DownloadCenter;

var app = TesseraApplication.CreateBuilder()
    .UseApp<DownloadCenterApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = DownloadCenterTheme.Default;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera DownloadCenter",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion
        };
    })
    .Build();

await app.RunAsync();
