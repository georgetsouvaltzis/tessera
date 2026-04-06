using Tessera;
using Tessera.TestFixtures;

await TesseraApplication.RunAsync(new CounterFixtureApp(), new TesseraRuntimeOptions
{
    UseConsoleKeyEvents = false,
    Screen = new ScreenOptions
    {
        AltScreen = true,
        WindowTitle = "Tessera Counter Fixture",
    },
});
