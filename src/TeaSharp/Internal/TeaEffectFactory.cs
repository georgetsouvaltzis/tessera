namespace TeaSharp.Internal;

internal static class TeaEffectFactory
{
    public static TeaEffect Quit { get; } = TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.Quit)!;

    public static TeaEffect Interrupt { get; } = TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.Interrupt)!;

    public static TeaEffect? Batch(params TeaEffect?[] effects)
    {
        ArgumentNullException.ThrowIfNull(effects);
        var adapted = new global::TeaSharp.Core.Abstractions.Effect?[effects.Length];
        for (var index = 0; index < effects.Length; index++)
        {
            adapted[index] = TeaEffectAdapter.ToCore(effects[index]);
        }

        var core = global::TeaSharp.Core.Commands.Effects.Batch(adapted);
        return TeaEffectAdapter.FromCore(core);
    }

    public static TeaEffect? Sequence(params TeaEffect?[] effects)
    {
        ArgumentNullException.ThrowIfNull(effects);
        var adapted = new global::TeaSharp.Core.Abstractions.Effect?[effects.Length];
        for (var index = 0; index < effects.Length; index++)
        {
            adapted[index] = TeaEffectAdapter.ToCore(effects[index]);
        }

        var core = global::TeaSharp.Core.Commands.Effects.Sequence(adapted);
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
