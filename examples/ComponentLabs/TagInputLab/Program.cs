using TeaSharp;
using TeaSharp.Controls;

var app = Tea.CreateBuilder()
    .UseApp<TagInputLabApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = TagInputLabApp.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp TagInput Lab",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
