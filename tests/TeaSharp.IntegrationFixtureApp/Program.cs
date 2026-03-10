using TeaSharp;
using TeaSharp.Core.Application;
using TeaSharp.Core.Terminal;
using TeaSharp.TestFixtures;

var terminal = new ConsoleTerminalAdapter();
var program = Tea.NewProgram(new CounterFixtureModel(), new ProgramOptions
{
    Terminal = terminal,
    UseConsoleKeyEvents = false,
});

await program.RunAsync();
