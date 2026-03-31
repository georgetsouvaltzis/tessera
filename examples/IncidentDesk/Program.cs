using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Examples.IncidentDesk;

var app = Tea.CreateBuilder()
    .UseApp<IncidentDeskApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = IncidentDeskTheme.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp IncidentDesk",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
