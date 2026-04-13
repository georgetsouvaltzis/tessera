using Tessera.Internal;

namespace Tessera;

/// <summary>
///     Provides factory helpers for common Tessera effects.
/// </summary>
public static class TesseraEffects
{
    /// <summary>
    ///     Gets the absence of an effect.
    /// </summary>
    public static TesseraEffect? None => null;

    /// <summary>
    ///     Gets an effect that terminates the application loop.
    /// </summary>
    public static TesseraEffect Quit => TesseraEffectFactory.Quit;

    /// <summary>
    ///     Gets an effect that interrupts the current program execution.
    /// </summary>
    public static TesseraEffect Interrupt => TesseraEffectFactory.Interrupt;

    /// <summary>
    ///     Emits a message immediately.
    /// </summary>
    /// <param name="message">The message to emit.</param>
    /// <returns>An effect that emits the supplied message.</returns>
    public static TesseraEffect Emit(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return _ => ValueTask.FromResult<Message?>(message);
    }

    /// <summary>
    ///     Emits a message after the supplied delay.
    /// </summary>
    /// <param name="delay">The delay to wait before emitting the message.</param>
    /// <param name="factory">The factory that creates the message from the current UTC time.</param>
    /// <returns>An effect that emits the produced message after the delay.</returns>
    public static TesseraEffect Tick(TimeSpan delay, Func<DateTimeOffset, Message> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return async cancellationToken =>
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return factory(DateTimeOffset.UtcNow);
        };
    }

    /// <summary>
    ///     Emits a message on the next interval boundary for the supplied delay.
    /// </summary>
    /// <param name="delay">The interval size.</param>
    /// <param name="factory">The factory that creates the message from the current UTC time.</param>
    /// <returns>An effect that emits the produced message on the next interval boundary.</returns>
    public static TesseraEffect Every(TimeSpan delay, Func<DateTimeOffset, Message> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return async cancellationToken =>
        {
            var now = DateTimeOffset.UtcNow;
            var next = new DateTimeOffset(now.Ticks - now.Ticks % delay.Ticks, TimeSpan.Zero).Add(delay);
            var wait = next - now;
            if (wait > TimeSpan.Zero)
            {
                await Task.Delay(wait, cancellationToken).ConfigureAwait(false);
            }

            return factory(DateTimeOffset.UtcNow);
        };
    }

    /// <summary>
    ///     Emits a message at the supplied interval and auto-reschedules itself through runtime plumbing.
    /// </summary>
    /// <param name="interval">The interval between emissions.</param>
    /// <param name="factory">The factory that creates the message from the current UTC time.</param>
    /// <returns>An effect that keeps emitting interval messages without app self-rescheduling in <c>Update(...)</c>.</returns>
    public static TesseraEffect Periodic(TimeSpan interval, Func<DateTimeOffset, Message> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return Tick(interval, now =>
        {
            var payload = factory(now);
            ArgumentNullException.ThrowIfNull(payload);
            return new TesseraPeriodicEffectMessage(interval, factory, payload);
        });
    }

    /// <summary>
    ///     Runs the supplied effects concurrently as a single effect.
    /// </summary>
    /// <param name="effects">The effects to batch.</param>
    /// <returns>The combined effect, or <see langword="null" /> when nothing is supplied.</returns>
    public static TesseraEffect? Batch(params TesseraEffect?[] effects)
    {
        return TesseraEffectFactory.Batch(effects);
    }

    /// <summary>
    ///     Runs the supplied effects sequentially as a single effect.
    /// </summary>
    /// <param name="effects">The effects to run in order.</param>
    /// <returns>The combined effect, or <see langword="null" /> when nothing is supplied.</returns>
    public static TesseraEffect? Sequence(params TesseraEffect?[] effects)
    {
        return TesseraEffectFactory.Sequence(effects);
    }

    /// <summary>
    ///     Emits raw terminal output.
    /// </summary>
    /// <param name="content">The raw terminal content to emit.</param>
    /// <returns>An effect that writes the supplied content.</returns>
    public static TesseraEffect Raw(string content)
    {
        return TesseraEffectFactory.Raw(content);
    }

    /// <summary>
    ///     Requests a terminal capability value by name.
    /// </summary>
    /// <param name="name">The capability name.</param>
    /// <returns>An effect that requests the capability.</returns>
    public static TesseraEffect RequestCapability(string name)
    {
        return TesseraEffectFactory.RequestCapability(name);
    }

    /// <summary>
    ///     Writes content to the clipboard.
    /// </summary>
    /// <param name="content">The content to store.</param>
    /// <returns>An effect that writes to the clipboard.</returns>
    public static TesseraEffect SetClipboard(string content)
    {
        return TesseraEffectFactory.SetClipboard(content);
    }

    /// <summary>
    ///     Reads the current clipboard content.
    /// </summary>
    /// <returns>An effect that requests clipboard content.</returns>
    public static TesseraEffect ReadClipboard()
    {
        return TesseraEffectFactory.ReadClipboard();
    }

    /// <summary>
    ///     Writes content to the primary clipboard selection.
    /// </summary>
    /// <param name="content">The content to store.</param>
    /// <returns>An effect that writes to the primary clipboard selection.</returns>
    public static TesseraEffect SetPrimaryClipboard(string content)
    {
        return TesseraEffectFactory.SetPrimaryClipboard(content);
    }

    /// <summary>
    ///     Reads the current primary clipboard selection.
    /// </summary>
    /// <returns>An effect that requests primary clipboard content.</returns>
    public static TesseraEffect ReadPrimaryClipboard()
    {
        return TesseraEffectFactory.ReadPrimaryClipboard();
    }

    /// <summary>
    ///     Requests the terminal foreground color.
    /// </summary>
    /// <returns>An effect that requests the foreground color.</returns>
    public static TesseraEffect RequestForegroundColor()
    {
        return TesseraEffectFactory.RequestForegroundColor();
    }

    /// <summary>
    ///     Requests the terminal background color.
    /// </summary>
    /// <returns>An effect that requests the background color.</returns>
    public static TesseraEffect RequestBackgroundColor()
    {
        return TesseraEffectFactory.RequestBackgroundColor();
    }

    /// <summary>
    ///     Requests the terminal cursor color.
    /// </summary>
    /// <returns>An effect that requests the cursor color.</returns>
    public static TesseraEffect RequestCursorColor()
    {
        return TesseraEffectFactory.RequestCursorColor();
    }
}
