using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<ConsumerTelemetryLabApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = ConsumerTelemetryLabApp.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Consumer Telemetry Lab",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.CellMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record TelemetryTick(DateTimeOffset At) : Message;

internal enum LabThemeMode
{
    Catppuccin,
    RosePine,
}

internal enum LoadProfile
{
    Nominal,
    Incident,
}
