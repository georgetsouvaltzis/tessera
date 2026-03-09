namespace TeaSharp.Core.Abstractions;

public delegate ValueTask<IMessage?> Command(CancellationToken cancellationToken);
