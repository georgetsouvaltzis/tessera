using TeaSharp;
using TeaSharp.Examples.DataWorkbench;

var app = Tea.CreateBuilder()
    .UseApp<DataWorkbenchApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = DataWorkbenchTheme.Default.Theme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp DataWorkbench",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build()
    ;

await app.RunAsync();
