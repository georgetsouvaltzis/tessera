using Tessera.Hosting;
using Tessera.Internal;

namespace Tessera;

/// <summary>
///     Represents a configured Tessera application that can send messages and run the interactive loop.
/// </summary>
/// <remarks>
///     Use this type when you need more control than
///     <see cref="RunAsync(TesseraApp, TesseraRuntimeOptions?, CancellationToken)" />
///     provides, such as sending external messages or stopping the loop explicitly.
/// </remarks>
public sealed class TesseraApplication
{
    private readonly ITeaRuntime _runtime;

    internal TesseraApplication(TesseraApp app, TesseraRuntimeOptions? options = null,
        TesseraHostingOptions? hosting = null)
    {
        App = app ?? throw new ArgumentNullException(nameof(app));
        Options = options ?? new TesseraRuntimeOptions();
        HostingOptions = hosting;
        _runtime = TesseraRuntimeFactory.Create(App, Options, HostingOptions);
    }

    /// <summary>
    ///     Gets the application instance that will be run.
    /// </summary>
    public TesseraApp App { get; }

    /// <summary>
    ///     Gets the runtime options applied to this application.
    /// </summary>
    public TesseraRuntimeOptions Options { get; }

    internal TesseraHostingOptions? HostingOptions { get; }

    /// <summary>
    ///     Creates a builder for the Tessera application startup surface.
    /// </summary>
    /// <returns>A builder for configuration-first application startup.</returns>
    public static TesseraApplicationBuilder CreateBuilder()
    {
        return new TesseraApplicationBuilder();
    }

    /// <summary>
    ///     Creates a configured application from the supplied app and runtime options.
    /// </summary>
    /// <param name="app">The app to host.</param>
    /// <param name="options">Runtime options for the application loop.</param>
    /// <returns>A built application that can be run, sent messages, or stopped explicitly.</returns>
    public static TesseraApplication CreateApplication(TesseraApp app, TesseraRuntimeOptions? options = null)
    {
        return new TesseraApplication(app, options);
    }

    /// <summary>
    ///     Runs an application using the Tessera startup surface.
    /// </summary>
    /// <param name="app">The app to run.</param>
    /// <param name="options">Runtime options for the application loop.</param>
    /// <param name="cancellationToken">A token that cancels application execution.</param>
    /// <returns>The application instance after the run completes.</returns>
    public static async Task<TesseraApp> RunAsync(TesseraApp app, TesseraRuntimeOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var application = CreateApplication(app, options);
        await application.RunAsync(cancellationToken).ConfigureAwait(false);
        return app;
    }

    /// <summary>
    ///     Sends a message into the running application loop.
    /// </summary>
    /// <param name="message">The message to deliver.</param>
    public void Send(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _runtime.Send(message);
    }

    /// <summary>
    ///     Runs the application until it exits or the supplied token is canceled.
    /// </summary>
    /// <param name="cancellationToken">The token used to cancel the run.</param>
    /// <returns>The application instance after the run completes.</returns>
    public async Task<TesseraApp> RunAsync(CancellationToken cancellationToken = default)
    {
        await _runtime.RunAsync(cancellationToken).ConfigureAwait(false);
        return App;
    }

    /// <summary>
    ///     Requests the running application to stop.
    /// </summary>
    /// <param name="kill"><see langword="true" /> to force termination; otherwise, request a graceful stop.</param>
    /// <param name="cancellationToken">The token used to cancel the stop request.</param>
    /// <returns>A task that completes when the stop request has been processed.</returns>
    public Task StopAsync(bool kill = false, CancellationToken cancellationToken = default)
    {
        return _runtime.StopAsync(kill, cancellationToken);
    }
}
