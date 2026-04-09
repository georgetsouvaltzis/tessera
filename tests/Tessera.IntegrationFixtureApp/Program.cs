using Tessera;
using Tessera.IntegrationFixtureApp;

await TesseraApplication.RunAsync(new CounterFixtureApp(), new TesseraRuntimeOptions
{
    UseConsoleKeyEvents = false,
    Screen = new ScreenOptions
    {
        AltScreen = true,
        WindowTitle = "Tessera Counter Fixture",
    },
});
