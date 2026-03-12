using TeaSharp.Internal;

namespace TeaSharp;

public static class TeaEffects
{
    public static TeaEffect? None => null;

    public static TeaEffect Emit(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        return _ => ValueTask.FromResult<Message?>(message);
    }

    public static TeaEffect Quit => TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.Quit)!;

    public static TeaEffect Interrupt => TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.Interrupt)!;

    public static TeaEffect Tick(TimeSpan delay, Func<DateTimeOffset, Message> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return async cancellationToken =>
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            return factory(DateTimeOffset.UtcNow);
        };
    }

    public static TeaEffect Every(TimeSpan delay, Func<DateTimeOffset, Message> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return async cancellationToken =>
        {
            var now = DateTimeOffset.UtcNow;
            var next = new DateTimeOffset(now.Ticks - (now.Ticks % delay.Ticks), TimeSpan.Zero).Add(delay);
            var wait = next - now;
            if (wait > TimeSpan.Zero)
            {
                await Task.Delay(wait, cancellationToken).ConfigureAwait(false);
            }

            return factory(DateTimeOffset.UtcNow);
        };
    }

    public static TeaEffect? Batch(params TeaEffect?[] effects)
    {
        var core = global::TeaSharp.Core.Commands.Effects.Batch(
            effects.Select(TeaEffectAdapter.ToCore).ToArray());
        return TeaEffectAdapter.FromCore(core);
    }

    public static TeaEffect? Sequence(params TeaEffect?[] effects)
    {
        var core = global::TeaSharp.Core.Commands.Effects.Sequence(
            effects.Select(TeaEffectAdapter.ToCore).ToArray());
        return TeaEffectAdapter.FromCore(core);
    }

    public static TeaEffect Raw(string content) =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.Raw(content))!;

    public static TeaEffect RequestCapability(string name) =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.RequestCapability(name))!;

    public static TeaEffect SetClipboard(string content) =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.SetClipboard(content))!;

    public static TeaEffect ReadClipboard() =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.ReadClipboard())!;

    public static TeaEffect SetPrimaryClipboard(string content) =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.SetPrimaryClipboard(content))!;

    public static TeaEffect ReadPrimaryClipboard() =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.ReadPrimaryClipboard())!;

    public static TeaEffect RequestForegroundColor() =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.RequestForegroundColor())!;

    public static TeaEffect RequestBackgroundColor() =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.RequestBackgroundColor())!;

    public static TeaEffect RequestCursorColor() =>
        TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.RequestCursorColor())!;
}
