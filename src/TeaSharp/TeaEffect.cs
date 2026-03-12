namespace TeaSharp;

public delegate ValueTask<Message?> TeaEffect(CancellationToken cancellationToken);
