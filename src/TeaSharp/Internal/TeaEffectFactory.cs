namespace TeaSharp.Internal;

internal static class TeaEffectFactory
{
    public static TeaEffect Quit { get; } = TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.Quit)!;

    public static TeaEffect Interrupt { get; } = TeaEffectAdapter.FromCore(global::TeaSharp.Core.Commands.Effects.Interrupt)!;

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
