namespace TeaSharp.Internal;

internal static class TeaEffectAdapter
{
    public static global::TeaSharp.Core.Abstractions.Effect? ToCore(TeaEffect? effect)
    {
        if (effect is null)
        {
            return null;
        }

        return async cancellationToken =>
        {
            var message = await effect(cancellationToken).ConfigureAwait(false);
            return message is null ? null : TeaMessageAdapter.ToCore(message);
        };
    }

    public static TeaEffect? FromCore(global::TeaSharp.Core.Abstractions.Effect? effect)
    {
        if (effect is null)
        {
            return null;
        }

        return async cancellationToken =>
        {
            var message = await effect(cancellationToken).ConfigureAwait(false);
            return message is null ? null : TeaMessageAdapter.ToPublic(message);
        };
    }
}
