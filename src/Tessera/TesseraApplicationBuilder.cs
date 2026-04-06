namespace Tessera;

/// <summary>
/// Builds a runnable Tessera application from a <see cref="TesseraApp"/> and runtime options.
/// </summary>
/// <remarks>
/// Use this type when startup needs multiple steps or when you want a built <see cref="TesseraApplication"/>
/// instance that can be controlled explicitly after creation.
/// </remarks>
public sealed class TesseraApplicationBuilder
{
    private Func<TesseraApp>? _appFactory;
    private readonly TesseraRuntimeOptions _runtime = new();

    /// <summary>
    /// Gets the runtime options that will be applied to the built application.
    /// </summary>
    public TesseraRuntimeOptions Runtime => _runtime;

    /// <summary>
    /// Configures the application to create a new <typeparamref name="TApp"/> instance when built.
    /// </summary>
    /// <typeparam name="TApp">The application type.</typeparam>
    /// <returns>The current builder.</returns>
    public TesseraApplicationBuilder UseApp<TApp>()
        where TApp : TesseraApp, new()
    {
        _appFactory = static () => new TApp();
        return this;
    }

    /// <summary>
    /// Configures the application to create its <see cref="TesseraApp"/> from the supplied factory.
    /// </summary>
    /// <param name="factory">The application factory.</param>
    /// <returns>The current builder.</returns>
    public TesseraApplicationBuilder UseApp(Func<TesseraApp> factory)
    {
        _appFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        return this;
    }

    /// <summary>
    /// Configures the application to run the supplied <see cref="TesseraApp"/> instance.
    /// </summary>
    /// <param name="app">The application instance to run.</param>
    /// <returns>The current builder.</returns>
    public TesseraApplicationBuilder UseApp(TesseraApp app)
    {
        ArgumentNullException.ThrowIfNull(app);
        _appFactory = () => app;
        return this;
    }

    /// <summary>
    /// Applies additional runtime configuration before the application is built.
    /// </summary>
    /// <param name="configure">The callback that mutates <see cref="Runtime"/>.</param>
    /// <returns>The current builder.</returns>
    public TesseraApplicationBuilder ConfigureRuntime(Action<TesseraRuntimeOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(_runtime);
        return this;
    }

    /// <summary>
    /// Builds a runnable <see cref="TesseraApplication"/>.
    /// </summary>
    /// <returns>The built application.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no application has been configured.</exception>
    public TesseraApplication Build()
    {
        if (_appFactory is null)
        {
            throw new InvalidOperationException("No TesseraApp factory configured. Call UseApp(...) before Build().");
        }

        return new TesseraApplication(_appFactory(), _runtime);
    }
}
