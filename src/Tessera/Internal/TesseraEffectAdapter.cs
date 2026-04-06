namespace Tessera.Internal;

internal static class TesseraEffectAdapter
{
    public static global::Tessera.Core.Abstractions.Effect? ToCore(TesseraEffect? effect)
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

    public static TesseraEffect? FromCore(global::Tessera.Core.Abstractions.Effect? effect)
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
