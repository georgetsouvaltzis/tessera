using TeaSharp;
using TeaSharp.Controls;

using TeaSharp.Examples.GitConsole;

var app = Tea.CreateBuilder()
    .UseApp<GitConsoleApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = GitConsoleTheme.DefaultTheme;
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
