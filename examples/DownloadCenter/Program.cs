using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Examples.DownloadCenter;

var app = Tea.CreateBuilder()
    .UseApp<DownloadCenterApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = DownloadCenterTheme.Default;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp DownloadCenter",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
