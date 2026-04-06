using TeaSharp;
using TeaSharp.Examples.WorkspaceApp;

var app = Tea.CreateBuilder()
    .UseApp<WorkspaceApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = WorkspaceTheme.Default;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp WorkspaceApp",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
