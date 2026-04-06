namespace Tessera.Internal;

internal static class TesseraEffectFactory
{
    public static TesseraEffect Quit { get; } = TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.Quit)!;

    public static TesseraEffect Interrupt { get; } = TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.Interrupt)!;

    public static TesseraEffect? Batch(params TesseraEffect?[] effects)
    {
        ArgumentNullException.ThrowIfNull(effects);
        var adapted = new global::Tessera.Core.Abstractions.Effect?[effects.Length];
        for (var index = 0; index < effects.Length; index++)
        {
            adapted[index] = TesseraEffectAdapter.ToCore(effects[index]);
        }

        var core = global::Tessera.Core.Commands.Effects.Batch(adapted);
        return TesseraEffectAdapter.FromCore(core);
    }

    public static TesseraEffect? Sequence(params TesseraEffect?[] effects)
    {
        ArgumentNullException.ThrowIfNull(effects);
        var adapted = new global::Tessera.Core.Abstractions.Effect?[effects.Length];
        for (var index = 0; index < effects.Length; index++)
        {
            adapted[index] = TesseraEffectAdapter.ToCore(effects[index]);
        }

        var core = global::Tessera.Core.Commands.Effects.Sequence(adapted);
        return TesseraEffectAdapter.FromCore(core);
    }

    public static TesseraEffect Raw(string content) =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.Raw(content))!;

    public static TesseraEffect RequestCapability(string name) =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.RequestCapability(name))!;

    public static TesseraEffect SetClipboard(string content) =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.SetClipboard(content))!;

    public static TesseraEffect ReadClipboard() =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.ReadClipboard())!;

    public static TesseraEffect SetPrimaryClipboard(string content) =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.SetPrimaryClipboard(content))!;

    public static TesseraEffect ReadPrimaryClipboard() =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.ReadPrimaryClipboard())!;

    public static TesseraEffect RequestForegroundColor() =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.RequestForegroundColor())!;

    public static TesseraEffect RequestBackgroundColor() =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.RequestBackgroundColor())!;

    public static TesseraEffect RequestCursorColor() =>
        TesseraEffectAdapter.FromCore(global::Tessera.Core.Commands.Effects.RequestCursorColor())!;
}
