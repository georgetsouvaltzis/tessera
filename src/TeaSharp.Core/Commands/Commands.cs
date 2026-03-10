using System.Globalization;
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

    public static Command Raw(string content) =>
        _ => ValueTask.FromResult<IMessage?>(new RawOutputMsg(content));

    public static Command RequestCapability(string capabilityName)
    {
        var name = capabilityName?.Trim() ?? string.Empty;
        var payload = string.Concat(name.Select(static ch => ((int)ch).ToString("X2", CultureInfo.InvariantCulture)));
        return Raw($"\u001bP+q{payload}\u001b\\");
    }

    public static Command SetClipboard(string content) =>
        Raw(BuildClipboardWriteSequence(content, selection: 'c'));

    public static Command ReadClipboard() =>
        Raw(BuildClipboardReadSequence(selection: 'c'));

    public static Command SetPrimaryClipboard(string content) =>
        Raw(BuildClipboardWriteSequence(content, selection: 'p'));

    public static Command ReadPrimaryClipboard() =>
        Raw(BuildClipboardReadSequence(selection: 'p'));

    public static Command RequestForegroundColor() =>
        Raw("\u001b]10;?\u001b\\");

    public static Command RequestBackgroundColor() =>
        Raw("\u001b]11;?\u001b\\");

    public static Command RequestCursorColor() =>
        Raw("\u001b]12;?\u001b\\");

    private static List<Command> GetValid(IEnumerable<Command?> commands)
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

    private static string BuildClipboardWriteSequence(string content, char selection)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(content ?? string.Empty);
        var encoded = Convert.ToBase64String(bytes);
        return $"\u001b]52;{selection};{encoded}\u001b\\";
    }

    private static string BuildClipboardReadSequence(char selection)
    {
        return $"\u001b]52;{selection};?\u001b\\";
    }
}
