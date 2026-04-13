using Tessera;
using Tessera.Examples.CounterForm;

var app = TesseraApplication.CreateBuilder()
    .UseApp<CounterFormApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = CounterFormTheme.Default.Theme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "Tessera CounterForm",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion
        };
    })
    .Build();

await app.RunAsync();
