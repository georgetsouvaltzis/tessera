using TeaSharp;
using TeaSharp.Examples.TransitBoard;

var app = Tea.CreateBuilder()
    .UseApp<TransitBoardApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = TransitBoardTheme.Default.Theme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp TransitBoard",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
