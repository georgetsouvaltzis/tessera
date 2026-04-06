using TeaSharp;
using TeaSharp.Examples.CounterForm;

var app = Tea.CreateBuilder()
    .UseApp<CounterFormApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = CounterFormTheme.Default.Theme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp CounterForm",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
