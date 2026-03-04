using TeaSharp.Tests;

var runner = new TestRunner();
runner.AddRange(ProgramRuntimeTests.Cases());
runner.AddRange(EventDecoderGoldenTests.Cases());
runner.AddRange(TerminalReaderBehaviorTests.Cases());

return await runner.RunAsync();
