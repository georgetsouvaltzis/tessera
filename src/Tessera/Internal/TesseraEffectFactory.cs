using Tessera.Core.Abstractions;
using Tessera.Core.Commands;

namespace Tessera.Internal;

internal static class TesseraEffectFactory
{
    public static TesseraEffect Quit { get; } = FromCoreRequired(Effects.Quit);

    public static TesseraEffect Interrupt { get; } = FromCoreRequired(Effects.Interrupt);

    public static TesseraEffect? Batch(params TesseraEffect?[] effects)
    {
        ArgumentNullException.ThrowIfNull(effects);
        var adapted = new Effect?[effects.Length];
        for (var index = 0; index < effects.Length; index++)
        {
            adapted[index] = TesseraEffectAdapter.ToCore(effects[index]);
        }

        var core = Effects.Batch(adapted);
        return TesseraEffectAdapter.FromCore(core);
    }

    public static TesseraEffect? Sequence(params TesseraEffect?[] effects)
    {
        ArgumentNullException.ThrowIfNull(effects);
        var adapted = new Effect?[effects.Length];
        for (var index = 0; index < effects.Length; index++)
        {
            adapted[index] = TesseraEffectAdapter.ToCore(effects[index]);
        }

        var core = Effects.Sequence(adapted);
        return TesseraEffectAdapter.FromCore(core);
    }

    public static TesseraEffect Raw(string content)
    {
        return FromCoreRequired(Effects.Raw(content));
    }

    public static TesseraEffect RequestCapability(string name)
    {
        return FromCoreRequired(Effects.RequestCapability(name));
    }

    public static TesseraEffect SetClipboard(string content)
    {
        return FromCoreRequired(Effects.SetClipboard(content));
    }

    public static TesseraEffect ReadClipboard()
    {
        return FromCoreRequired(Effects.ReadClipboard());
    }

    public static TesseraEffect SetPrimaryClipboard(string content)
    {
        return FromCoreRequired(Effects.SetPrimaryClipboard(content));
    }

    public static TesseraEffect ReadPrimaryClipboard()
    {
        return FromCoreRequired(Effects.ReadPrimaryClipboard());
    }

    public static TesseraEffect RequestForegroundColor()
    {
        return FromCoreRequired(Effects.RequestForegroundColor());
    }

    public static TesseraEffect RequestBackgroundColor()
    {
        return FromCoreRequired(Effects.RequestBackgroundColor());
    }

    public static TesseraEffect RequestCursorColor()
    {
        return FromCoreRequired(Effects.RequestCursorColor());
    }

    private static TesseraEffect FromCoreRequired(Effect effect)
    {
        return TesseraEffectAdapter.FromCore(effect)
               ?? throw new InvalidOperationException("Core effect adapter returned null for a required effect.");
    }
}
