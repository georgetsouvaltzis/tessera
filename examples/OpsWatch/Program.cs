using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Examples.OpsWatch;

var app = Tea.CreateBuilder()
    .UseApp<OpsWatchApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = OpsWatchTheme.Default.Theme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp OpsWatch",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
