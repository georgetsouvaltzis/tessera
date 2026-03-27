using TeaSharp;
using TeaSharp.Controls;

var app = Tea.CreateBuilder()
    .UseApp<ComboBoxApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = ComboBoxApp.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp ComboBox",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
