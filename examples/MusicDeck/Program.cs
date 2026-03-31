using TeaSharp;
using TeaSharp.Controls;

using TeaSharp.Examples.MusicDeck;

var app = Tea.CreateBuilder()
    .UseApp<MusicDeckApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = MusicDeckTheme.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp MusicDeck",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
