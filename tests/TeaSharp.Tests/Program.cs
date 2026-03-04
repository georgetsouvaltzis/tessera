using TeaSharp.Tests;

var runner = new TestRunner();
runner.AddRange(ProgramRuntimeTests.Cases());
runner.AddRange(EventDecoderGoldenTests.Cases());
runner.AddRange(TerminalReaderBehaviorTests.Cases());
runner.AddRange(RendererBehaviorTests.Cases());

return await runner.RunAsync();
