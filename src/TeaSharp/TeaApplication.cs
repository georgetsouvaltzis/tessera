using TeaSharp.Internal;

namespace TeaSharp;

/// <summary>
/// Represents a configured TeaSharp application that can send messages and run the interactive loop.
/// </summary>
/// <remarks>
/// Use this type when you need more control than <see cref="Tea.RunAsync(TeaApp, TeaRuntimeOptions?, CancellationToken)"/>
/// provides, such as sending external messages or stopping the loop explicitly.
/// </remarks>
public sealed class TeaApplication
{
    private readonly TeaApp _app;
    private readonly TeaRuntimeOptions _options;
    private readonly global::TeaSharp.Hosting.TeaHostingOptions? _hosting;
    private readonly ITeaRuntime _runtime;

    internal TeaApplication(TeaApp app, TeaRuntimeOptions? options = null, global::TeaSharp.Hosting.TeaHostingOptions? hosting = null)
    {
        _app = app ?? throw new ArgumentNullException(nameof(app));
        _options = options ?? new TeaRuntimeOptions();
        _hosting = hosting;
        _runtime = TeaRuntimeFactory.Create(_app, _options, _hosting);
    }

    /// <summary>
    /// Gets the application instance that will be run.
    /// </summary>
    public TeaApp App => _app;

    /// <summary>
    /// Gets the runtime options applied to this application.
    /// </summary>
    public TeaRuntimeOptions Options => _options;

    internal global::TeaSharp.Hosting.TeaHostingOptions? HostingOptions => _hosting;

    /// <summary>
    /// Sends a message into the running application loop.
    /// </summary>
    /// <param name="message">The message to deliver.</param>
    public void Send(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _runtime.Send(message);
    }

    /// <summary>
    /// Runs the application until it exits or the supplied token is canceled.
    /// </summary>
    /// <param name="cancellationToken">The token used to cancel the run.</param>
    /// <returns>The application instance after the run completes.</returns>
    public async Task<TeaApp> RunAsync(CancellationToken cancellationToken = default)
    {
        await _runtime.RunAsync(cancellationToken).ConfigureAwait(false);
        return _app;
    }

    /// <summary>
    /// Requests the running application to stop.
    /// </summary>
    /// <param name="kill"><see langword="true"/> to force termination; otherwise, request a graceful stop.</param>
    /// <param name="cancellationToken">The token used to cancel the stop request.</param>
    /// <returns>A task that completes when the stop request has been processed.</returns>
    public Task StopAsync(bool kill = false, CancellationToken cancellationToken = default)
    {
        return _runtime.StopAsync(kill, cancellationToken);
    }
}
