using TeaSharp;
using TeaSharp.Controls;

var app = Tea.CreateBuilder()
    .UseApp<GitConsoleApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = GitConsoleApp.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp GitConsole",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
