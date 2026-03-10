using TeaSharp;
using TeaSharp.TestFixtures;

var program = Tea.NewProgram(new CounterFixtureModel(), new TeaProgramOptions
{
    UseConsoleKeyEvents = false,
});

await program.RunAsync();
