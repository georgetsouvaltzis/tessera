using TeaSharp;
using TeaSharp.Controls;

var app = Tea.CreateBuilder()
    .UseApp<TokenEditorApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = TokenEditorApp.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp TokenEditor",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
