using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Tests;

internal static class TeaProgramOptionsTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase("TeaProgramOptions_Defaults_MapToStableProgramDefaults", Defaults_MapToStableProgramDefaults);
        yield return new TestCase("TeaProgramOptions_ConfiguredValues_MapToProgramOptions", ConfiguredValues_MapToProgramOptions);
        yield return new TestCase("TeaProgramOptions_TeaFactoryAcceptsStableDefaults", TeaFactory_AcceptsStableDefaults);
        yield return new TestCase("TeaProgramOptions_TeaFactoryAcceptsStableHostOptions", TeaFactory_AcceptsStableHostOptions);
    }

    private static Task Defaults_MapToStableProgramDefaults()
    {
        var options = new TeaProgramOptions();
        var mapped = options.ToProgramOptions();

        TestAssert.Equal(60, mapped.MaxFps, "TeaProgramOptions should preserve default max FPS.");
        TestAssert.True(mapped.AdaptiveFramePacing, "TeaProgramOptions should enable adaptive frame pacing by default.");
        TestAssert.True(mapped.UseConsoleKeyEvents, "TeaProgramOptions should enable console key events by default.");
        TestAssert.True(mapped.CatchCommandExceptions, "TeaProgramOptions should catch command exceptions by default.");
        TestAssert.True(mapped.EnableResizeSignals, "TeaProgramOptions should enable resize signals by default.");
        return Task.CompletedTask;
    }

    private static Task ConfiguredValues_MapToProgramOptions()
    {
        static IMessage Recover(Exception exception) => new CommandErrorMsg(exception);

        var options = new TeaProgramOptions
        {
            MaxFps = 30,
            AdaptiveFramePacing = false,
            DisableRenderer = true,
            DisableInput = true,
            UseConsoleKeyEvents = false,
            CatchCommandExceptions = false,
            RecoverCommandException = Recover,
            EscapeTimeout = TimeSpan.FromMilliseconds(75),
            EnableResizeSignals = false,
            ResizePollInterval = TimeSpan.FromMilliseconds(200),
            MinResizePollInterval = TimeSpan.FromMilliseconds(25),
        };
        var mapped = options.ToProgramOptions();

        TestAssert.Equal(30, mapped.MaxFps, "TeaProgramOptions should map max FPS.");
        TestAssert.True(!mapped.AdaptiveFramePacing, "TeaProgramOptions should map adaptive frame pacing.");
        TestAssert.True(mapped.DisableRenderer, "TeaProgramOptions should map renderer toggle.");
        TestAssert.True(mapped.DisableInput, "TeaProgramOptions should map input toggle.");
        TestAssert.True(!mapped.UseConsoleKeyEvents, "TeaProgramOptions should map console key event toggle.");
        TestAssert.True(!mapped.CatchCommandExceptions, "TeaProgramOptions should map command exception policy.");
        TestAssert.ReferenceSame((object)Recover, options.RecoverCommandException!, "TeaProgramOptions should preserve recovery delegate.");
        TestAssert.Equal(TimeSpan.FromMilliseconds(75), mapped.EscapeTimeout, "TeaProgramOptions should map escape timeout.");
        TestAssert.True(!mapped.EnableResizeSignals, "TeaProgramOptions should map resize signal toggle.");
        TestAssert.Equal(TimeSpan.FromMilliseconds(200), mapped.ResizePollInterval, "TeaProgramOptions should map resize poll interval.");
        TestAssert.Equal(TimeSpan.FromMilliseconds(25), mapped.MinResizePollInterval, "TeaProgramOptions should map minimum resize poll interval.");
        return Task.CompletedTask;
    }

    private static Task TeaFactory_AcceptsStableHostOptions()
    {
        var model = new NoOpModel();
        var program = Tea.NewProgram(model, new TeaProgramOptions
        {
            DisableInput = true,
            DisableRenderer = true,
            UseConsoleKeyEvents = false,
        });

        TestAssert.True(program is not null, "Tea factory should create a program from stable host options.");
        return Task.CompletedTask;
    }

    private static Task TeaFactory_AcceptsStableDefaults()
    {
        var program = Tea.NewProgram(new NoOpModel());

        TestAssert.True(program is not null, "Tea factory should create a program from stable default host options.");
        return Task.CompletedTask;
    }

    private sealed class NoOpModel : IModel
    {
        public Command? Init() => null;

        public Command? Update(IMessage message) => null;

        public TeaSharp.Core.Abstractions.View View() => TeaSharp.Core.Abstractions.View.From(string.Empty);
    }
}
