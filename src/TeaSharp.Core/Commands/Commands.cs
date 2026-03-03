using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Commands;

public static class Commands
{
    public static Command? None => null;

    public static Command Quit =>
        _ => ValueTask.FromResult<IMessage?>(new QuitMsg());

    public static Command Interrupt =>
        _ => ValueTask.FromResult<IMessage?>(new InterruptMsg());

    public static Command FromMessage(IMessage message) =>
        _ => ValueTask.FromResult<IMessage?>(message);

    public static Command? Compact(params Command?[] commands)
    {
        var valid = new List<Command>(commands.Length);
        foreach (var command in commands)
        {
            if (command is not null)
            {
                valid.Add(command);
            }
        }

        return valid.Count switch
        {
            0 => null,
            1 => valid[0],
            _ => _ => ValueTask.FromResult<IMessage?>(new BatchMsg(valid)),
        };
    }

    public static Command? Batch(params Command?[] commands)
    {
        var compact = Compact(commands);
        if (compact is null)
        {
            return null;
        }

        return _ => ValueTask.FromResult<IMessage?>(new BatchMsg(GetValid(commands)));
    }

    public static Command? Sequence(params Command?[] commands)
    {
        var valid = GetValid(commands);
        return valid.Count switch
        {
            0 => null,
            1 => valid[0],
            _ => _ => ValueTask.FromResult<IMessage?>(new SequenceMsg(valid)),
        };
    }

    public static Command Tick(TimeSpan delay, Func<DateTimeOffset, IMessage> factory)
    {
        return async cancellationToken =>
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return factory(DateTimeOffset.UtcNow);
        };
    }

    public static Command Every(TimeSpan cadence, Func<DateTimeOffset, IMessage> factory)
    {
        return async cancellationToken =>
        {
            var now = DateTimeOffset.UtcNow;
            var next = new DateTimeOffset(now.Ticks - (now.Ticks % cadence.Ticks), TimeSpan.Zero).Add(cadence);
            var delay = next - now;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }

            return factory(DateTimeOffset.UtcNow);
        };
    }

    private static IReadOnlyList<Command> GetValid(IEnumerable<Command?> commands)
    {
        var valid = new List<Command>();
        foreach (var command in commands)
        {
            if (command is not null)
            {
                valid.Add(command);
            }
        }

        return valid;
    }
}
