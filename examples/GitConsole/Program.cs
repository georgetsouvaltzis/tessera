using Tessera;
using Tessera.Controls;

using Tessera.Examples.GitConsole;

var app = TesseraApplication.CreateBuilder()
    .UseApp<GitConsoleApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = GitConsoleTheme.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera GitConsole",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
