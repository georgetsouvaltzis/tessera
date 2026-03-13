namespace TeaSharp;

/// <summary>
/// Provides the primary application-facing entry points for TeaSharp applications.
/// </summary>
/// <remarks>
/// Use <see cref="RunAsync"/> for the smallest startup path. Use <see cref="CreateBuilder"/> when you need to
/// configure runtime options before building a controllable <see cref="TeaApplication"/> instance.
/// </remarks>
public static class Tea
{
    /// <summary>
    /// Creates a builder for the TeaSharp application startup surface.
    /// </summary>
    /// <returns>A builder for configuration-first application startup.</returns>
    public static TeaApplicationBuilder CreateBuilder() => new();

    /// <summary>
    /// Creates an application using the TeaSharp-native startup surface.
    /// </summary>
    /// <param name="app">The app to host.</param>
    /// <param name="options">Runtime options for the application loop.</param>
    /// <returns>A built application that can be run, sent messages, or stopped explicitly.</returns>
    public static TeaApplication CreateApplication(TeaApp app, TeaRuntimeOptions? options = null) =>
        new(app, options);

    /// <summary>
    /// Runs an application using the TeaSharp-native startup surface.
    /// </summary>
    /// <param name="app">The app to run.</param>
    /// <param name="options">Runtime options for the application loop.</param>
    /// <param name="cancellationToken">A token that cancels application execution.</param>
    /// <returns>The application instance after the run completes.</returns>
    public static async Task<TeaApp> RunAsync(TeaApp app, TeaRuntimeOptions? options = null, CancellationToken cancellationToken = default)
    {
        var application = CreateApplication(app, options);
        await application.RunAsync(cancellationToken).ConfigureAwait(false);
        return app;
    }
}
