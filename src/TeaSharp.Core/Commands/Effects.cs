using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;

namespace TeaSharp.Core.Commands;

public static class Effects
{
    public static Effect? None => null;

    public static Effect Quit =>
        _ => ValueTask.FromResult<IMessage?>(new QuitMsg());

    public static Effect Interrupt =>
        _ => ValueTask.FromResult<IMessage?>(new InterruptMsg());

    public static Effect FromMessage(IMessage message) =>
        _ => ValueTask.FromResult<IMessage?>(message);

    public static Effect? Compact(params Effect?[] effects)
    {
        var valid = new List<Effect>(effects.Length);
        foreach (var effect in effects)
        {
            if (effect is not null)
            {
                valid.Add(effect);
            }
        }

        return valid.Count switch
        {
            0 => null,
            1 => valid[0],
            _ => _ => ValueTask.FromResult<IMessage?>(new BatchMsg(valid)),
        };
    }

    public static Effect? Batch(params Effect?[] effects)
    {
        var compact = Compact(effects);
        if (compact is null)
        {
            return null;
        }

        return _ => ValueTask.FromResult<IMessage?>(new BatchMsg(GetValid(effects)));
    }

    public static Effect? Sequence(params Effect?[] effects)
    {
        var valid = GetValid(effects);
        return valid.Count switch
        {
            0 => null,
            1 => valid[0],
            _ => _ => ValueTask.FromResult<IMessage?>(new SequenceMsg(valid)),
        };
    }

    public static Effect Tick(TimeSpan delay, Func<DateTimeOffset, IMessage> factory)
    {
        return async cancellationToken =>
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return factory(DateTimeOffset.UtcNow);
        };
    }

    public static Effect Every(TimeSpan cadence, Func<DateTimeOffset, IMessage> factory)
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

    public static Effect Raw(string content) =>
        _ => ValueTask.FromResult<IMessage?>(new RawOutputMsg(content));

    public static Effect RequestCapability(string capabilityName)
    {
        var name = capabilityName?.Trim() ?? string.Empty;
        var payload = string.Concat(name.Select(static ch => ((int)ch).ToString("X2", CultureInfo.InvariantCulture)));
        return Raw($"\u001bP+q{payload}\u001b\\");
    }

    public static Effect SetClipboard(string content) =>
        Raw(BuildClipboardWriteSequence(content, selection: 'c'));

    public static Effect ReadClipboard() =>
        Raw(BuildClipboardReadSequence(selection: 'c'));

    public static Effect SetPrimaryClipboard(string content) =>
        Raw(BuildClipboardWriteSequence(content, selection: 'p'));

    public static Effect ReadPrimaryClipboard() =>
        Raw(BuildClipboardReadSequence(selection: 'p'));

    public static Effect RequestForegroundColor() =>
        Raw("\u001b]10;?\u001b\\");

    public static Effect RequestBackgroundColor() =>
        Raw("\u001b]11;?\u001b\\");

    public static Effect RequestCursorColor() =>
        Raw("\u001b]12;?\u001b\\");

    private static List<Effect> GetValid(IEnumerable<Effect?> effects)
    {
        var valid = new List<Effect>();
        foreach (var effect in effects)
        {
            if (effect is not null)
            {
                valid.Add(effect);
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
