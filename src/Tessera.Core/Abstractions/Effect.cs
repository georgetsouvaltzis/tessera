namespace Tessera.Core.Abstractions;

public delegate ValueTask<IMessage?> Effect(CancellationToken cancellationToken);
