namespace TeaSharp;

/// <summary>
/// Represents an asynchronous operation that can emit a follow-up <see cref="Message"/>.
/// </summary>
/// <remarks>
/// Effects are the runtime work counterpart to <see cref="TeaApp.Update(Message)"/>. They are scheduled after
/// an update pass and may later produce a new message for the application.
/// </remarks>
/// <param name="cancellationToken">The token used to cancel the effect.</param>
/// <returns>The message produced by the effect, or <see langword="null"/>.</returns>
public delegate ValueTask<Message?> TeaEffect(CancellationToken cancellationToken);
