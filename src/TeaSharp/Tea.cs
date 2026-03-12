namespace TeaSharp;

/// <summary>
/// Provides the primary application-facing entry points for TeaSharp applications.
/// </summary>
public static class Tea
{
    /// <summary>
    /// Creates a builder for the TeaSharp application startup surface.
    /// </summary>
    public static TeaApplicationBuilder CreateBuilder() => new();

    /// <summary>
    /// Creates an application using the TeaSharp-native startup surface.
    /// </summary>
    /// <param name="app">The app to host.</param>
    /// <param name="options">Runtime options for the application loop.</param>
    public static TeaApplication CreateApplication(TeaApp app, TeaRuntimeOptions? options = null) =>
        new(app, options);

    /// <summary>
    /// Runs an application using the TeaSharp-native startup surface.
    /// </summary>
    /// <param name="app">The app to run.</param>
    /// <param name="options">Runtime options for the application loop.</param>
    /// <param name="cancellationToken">A token that cancels application execution.</param>
    public static async Task<TeaApp> RunAsync(TeaApp app, TeaRuntimeOptions? options = null, CancellationToken cancellationToken = default)
    {
        var application = CreateApplication(app, options);
        await application.RunAsync(cancellationToken).ConfigureAwait(false);
        return app;
    }
}
