namespace Tessera.Core.Abstractions;

/// <summary>
/// Represents deferred work that can emit a follow-up message into the runtime loop.
/// </summary>
/// <param name="cancellationToken">Cancels the scheduled work before it completes.</param>
/// <returns>The next message to dispatch, or <see langword="null" /> when nothing should be emitted.</returns>
public delegate ValueTask<IMessage?> Effect(CancellationToken cancellationToken);
