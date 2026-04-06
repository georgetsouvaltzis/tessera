using TeaSharp;
using TeaSharp.Examples.HelloWorld;

var app = Tea.CreateBuilder()
    .UseApp<HelloWorldApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = HelloWorldTheme.Default;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp HelloWorld",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
