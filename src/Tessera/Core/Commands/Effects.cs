using System.Globalization;
using System.Text;
using Tessera.Core.Abstractions;
using Tessera.Core.Messages;

namespace Tessera.Core.Commands;

/// <summary>
///     Creates common runtime effects for quitting, scheduling, terminal queries, and raw output.
/// </summary>
public static class Effects
{
    /// <summary>
    ///     Represents the absence of a scheduled effect.
    /// </summary>
    public static Effect? None => null;

    /// <summary>
    ///     Produces an effect that requests runtime shutdown.
    /// </summary>
    public static Effect Quit =>
        _ => ValueTask.FromResult<IMessage?>(new QuitMsg());

    /// <summary>
    ///     Produces an effect that interrupts the runtime loop.
    /// </summary>
    public static Effect Interrupt =>
        _ => ValueTask.FromResult<IMessage?>(new InterruptMsg());

    /// <summary>
    ///     Wraps a message as an effect.
    /// </summary>
    /// <param name="message">The message to emit.</param>
    /// <returns>An effect that returns the supplied message.</returns>
    public static Effect FromMessage(IMessage message)
    {
        return _ => ValueTask.FromResult<IMessage?>(message);
    }

    /// <summary>
    ///     Removes null effects and collapses the result to the smallest useful form.
    /// </summary>
    /// <param name="effects">The effect set to compact.</param>
    /// <returns><see langword="null" />, a single effect, or a batched effect.</returns>
    public static Effect? Compact(params Effect?[] effects)
    {
        var valid = effects.OfType<Effect>().ToList();

        return valid.Count switch
        {
            0 => null,
            1 => valid[0],
            _ => _ => ValueTask.FromResult<IMessage?>(new BatchMsg(valid))
        };
    }

    /// <summary>
    ///     Produces a batch effect that emits all valid child effects together.
    /// </summary>
    /// <param name="effects">The effects to batch.</param>
    /// <returns>A batch effect, a single effect, or <see langword="null" />.</returns>
    public static Effect? Batch(params Effect?[] effects)
    {
        var compact = Compact(effects);
        if (compact is null)
        {
            return null;
        }

        return _ => ValueTask.FromResult<IMessage?>(new BatchMsg(GetValid(effects)));
    }

    /// <summary>
    ///     Produces an effect sequence that preserves the order of valid child effects.
    /// </summary>
    /// <param name="effects">The effects to sequence.</param>
    /// <returns>A sequence effect, a single effect, or <see langword="null" />.</returns>
    public static Effect? Sequence(params Effect?[] effects)
    {
        var valid = GetValid(effects);
        return valid.Count switch
        {
            0 => null,
            1 => valid[0],
            _ => _ => ValueTask.FromResult<IMessage?>(new SequenceMsg(valid))
        };
    }

    /// <summary>
    ///     Schedules a one-shot effect that fires after the specified delay.
    /// </summary>
    /// <param name="delay">The delay before the message is created.</param>
    /// <param name="factory">Creates the message to emit when the delay elapses.</param>
    /// <returns>An asynchronous delayed effect.</returns>
    public static Effect Tick(TimeSpan delay, Func<DateTimeOffset, IMessage> factory)
    {
        return async cancellationToken =>
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return factory(DateTimeOffset.UtcNow);
        };
    }

    /// <summary>
    ///     Schedules an effect aligned to the next cadence boundary.
    /// </summary>
    /// <param name="cadence">The recurring cadence to align against.</param>
    /// <param name="factory">Creates the message to emit on the next cadence tick.</param>
    /// <returns>An asynchronous cadence-aligned effect.</returns>
    public static Effect Every(TimeSpan cadence, Func<DateTimeOffset, IMessage> factory)
    {
        return async cancellationToken =>
        {
            var now = DateTimeOffset.UtcNow;
            var next = new DateTimeOffset(now.Ticks - now.Ticks % cadence.Ticks, TimeSpan.Zero).Add(cadence);
            var delay = next - now;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }

            return factory(DateTimeOffset.UtcNow);
        };
    }

    /// <summary>
    ///     Emits a raw terminal escape sequence.
    /// </summary>
    /// <param name="content">The raw content to write to the terminal.</param>
    /// <returns>An effect that emits the raw content.</returns>
    public static Effect Raw(string content)
    {
        return _ => ValueTask.FromResult<IMessage?>(new RawOutputMsg(content));
    }

    /// <summary>
    ///     Requests a terminal capability report using the XTGETTCAP protocol.
    /// </summary>
    /// <param name="capabilityName">The capability name to request.</param>
    /// <returns>An effect that emits the capability query.</returns>
    public static Effect RequestCapability(string capabilityName)
    {
        var name = capabilityName?.Trim() ?? string.Empty;
        var payload = string.Concat(name.Select(static ch => ((int)ch).ToString("X2", CultureInfo.InvariantCulture)));
        return Raw($"\eP+q{payload}\e\\");
    }

    /// <summary>
    ///     Writes text to the standard clipboard selection.
    /// </summary>
    /// <param name="content">The clipboard text to publish.</param>
    /// <returns>An effect that emits the clipboard write sequence.</returns>
    public static Effect SetClipboard(string content)
    {
        return Raw(BuildClipboardWriteSequence(content, 'c'));
    }

    /// <summary>
    ///     Requests the current standard clipboard contents.
    /// </summary>
    /// <returns>An effect that emits the clipboard read sequence.</returns>
    public static Effect ReadClipboard()
    {
        return Raw(BuildClipboardReadSequence('c'));
    }

    /// <summary>
    ///     Writes text to the primary selection.
    /// </summary>
    /// <param name="content">The primary-selection text to publish.</param>
    /// <returns>An effect that emits the primary-selection write sequence.</returns>
    public static Effect SetPrimaryClipboard(string content)
    {
        return Raw(BuildClipboardWriteSequence(content, 'p'));
    }

    /// <summary>
    ///     Requests the current primary selection contents.
    /// </summary>
    /// <returns>An effect that emits the primary-selection read sequence.</returns>
    public static Effect ReadPrimaryClipboard()
    {
        return Raw(BuildClipboardReadSequence('p'));
    }

    /// <summary>
    ///     Requests the terminal foreground color.
    /// </summary>
    /// <returns>An effect that emits the foreground-color query.</returns>
    public static Effect RequestForegroundColor()
    {
        return Raw("\e]10;?\e\\");
    }

    /// <summary>
    ///     Requests the terminal background color.
    /// </summary>
    /// <returns>An effect that emits the background-color query.</returns>
    public static Effect RequestBackgroundColor()
    {
        return Raw("\e]11;?\e\\");
    }

    /// <summary>
    ///     Requests the terminal cursor color.
    /// </summary>
    /// <returns>An effect that emits the cursor-color query.</returns>
    public static Effect RequestCursorColor()
    {
        return Raw("\e]12;?\e\\");
    }

    private static List<Effect> GetValid(IEnumerable<Effect?> effects)
    {
        return effects.OfType<Effect>().ToList();
    }

    private static string BuildClipboardWriteSequence(string content, char selection)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var encoded = Convert.ToBase64String(bytes);
        return $"\e]52;{selection};{encoded}\e\\";
    }

    private static string BuildClipboardReadSequence(char selection)
    {
        return $"\e]52;{selection};?\e\\";
    }
}
