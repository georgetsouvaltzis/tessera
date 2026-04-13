using Tessera.Core.Abstractions;

namespace Tessera.Internal;

internal static class TesseraEffectAdapter
{
    public static Effect? ToCore(TesseraEffect? effect)
    {
        if (effect is null)
        {
            return null;
        }

        return async cancellationToken =>
        {
            var message = await effect(cancellationToken).ConfigureAwait(false);
            return message is null ? null : TesseraMessageAdapter.ToCore(message);
        };
    }

    public static TesseraEffect? FromCore(Effect? effect)
    {
        if (effect is null)
        {
            return null;
        }

        return async cancellationToken =>
        {
            var message = await effect(cancellationToken).ConfigureAwait(false);
            return message is null ? null : TesseraMessageAdapter.ToPublic(message);
        };
    }
}
