using Tessera;
using Tessera.Examples.WorkspaceApp;

var app = TesseraApplication.CreateBuilder()
    .UseApp<WorkspaceApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = WorkspaceTheme.Default;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera WorkspaceApp",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
