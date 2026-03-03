using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Commands;

namespace TeaSharp;

public static class Tea
{
    public static TeaProgram NewProgram(IModel model, ProgramOptions? options = null) =>
        new(model, options);

    public static class Cmd
    {
        public static Command Quit => Commands.Quit;
        public static Command Interrupt => Commands.Interrupt;
        public static Command Tick(TimeSpan delay, Func<DateTimeOffset, IMessage> factory) => Commands.Tick(delay, factory);
        public static Command Every(TimeSpan delay, Func<DateTimeOffset, IMessage> factory) => Commands.Every(delay, factory);
        public static Command? Batch(params Command?[] commands) => Commands.Batch(commands);
        public static Command? Sequence(params Command?[] commands) => Commands.Sequence(commands);
    }
}
