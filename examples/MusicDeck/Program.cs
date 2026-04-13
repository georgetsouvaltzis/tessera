using Tessera;
using Tessera.Examples.MusicDeck;

var app = TesseraApplication.CreateBuilder()
    .UseApp<MusicDeckApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = MusicDeckTheme.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera MusicDeck",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion
        };
    })
    .Build();

await app.RunAsync();
