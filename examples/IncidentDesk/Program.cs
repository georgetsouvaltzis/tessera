using Tessera;
using Tessera.Controls;
using Tessera.Examples.IncidentDesk;

var app = TesseraApplication.CreateBuilder()
    .UseApp<IncidentDeskApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = IncidentDeskTheme.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera IncidentDesk",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
