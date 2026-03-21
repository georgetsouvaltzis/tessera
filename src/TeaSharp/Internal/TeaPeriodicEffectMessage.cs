namespace TeaSharp.Internal;

internal sealed record TeaPeriodicEffectMessage(
    TimeSpan Interval,
    Func<DateTimeOffset, Message> Factory,
    Message Payload) : Message
{
    public static Message TryUnwrap(Message message, out TeaPeriodicEffectMessage? periodic)
    {
        if (message is TeaPeriodicEffectMessage wrapped)
        {
            periodic = wrapped;
            return wrapped.Payload;
        }

        periodic = null;
        return message;
    }
}
