using Tessera;
using Tessera.Examples.HelloWorld;

var app = TesseraApplication.CreateBuilder()
    .UseApp<HelloWorldApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = HelloWorldTheme.Default;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera HelloWorld",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion
        };
    })
    .Build();

await app.RunAsync();
