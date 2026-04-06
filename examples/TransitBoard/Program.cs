using Tessera;
using Tessera.Examples.TransitBoard;

var app = TesseraApplication.CreateBuilder()
    .UseApp<TransitBoardApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = TransitBoardTheme.Default.Theme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera TransitBoard",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
