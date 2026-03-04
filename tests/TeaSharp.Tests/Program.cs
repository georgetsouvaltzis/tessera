using TeaSharp.Tests;

var runner = new TestRunner();
runner.AddRange(ProgramRuntimeTests.Cases());
runner.AddRange(EventDecoderGoldenTests.Cases());
runner.AddRange(TerminalReaderBehaviorTests.Cases());
runner.AddRange(RendererBehaviorTests.Cases());
runner.AddRange(RendererSnapshotTests.Cases());
runner.AddRange(StyleRenderingTests.Cases());
runner.AddRange(WidgetStateTests.Cases());
runner.AddRange(ChartComponentTests.Cases());
runner.AddRange(DashboardComponentTests.Cases());
runner.AddRange(ProtocolFixtureTests.Cases());
runner.AddRange(TerminalCapabilityDetectorTests.Cases());
runner.AddRange(ComponentRenderingTests.Cases());

return await runner.RunAsync();
