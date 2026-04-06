namespace Tessera.Internal;

internal sealed record TesseraPeriodicEffectMessage(
    TimeSpan Interval,
    Func<DateTimeOffset, Message> Factory,
    Message Payload) : Message
{
    public static Message TryUnwrap(Message message, out TesseraPeriodicEffectMessage? periodic)
    {
        if (message is TesseraPeriodicEffectMessage wrapped)
        {
            periodic = wrapped;
            return wrapped.Payload;
        }

        periodic = null;
        return message;
    }
}
