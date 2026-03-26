using TeaSharp;
using TeaSharp.Controls;

var app = Tea.CreateBuilder()
    .UseApp<WidgetCoverageWorkflowLabApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = WidgetCoverageWorkflowLabApp.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Widget Coverage Workflow Lab",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();
