using TeaSharp;
using TeaSharp.TestFixtures;

await Tea.RunAsync(new CounterFixtureApp(), new TeaRuntimeOptions
{
    UseConsoleKeyEvents = false,
    Screen = new ScreenOptions
    {
        AltScreen = true,
        WindowTitle = "TeaSharp Counter Fixture",
    },
});
