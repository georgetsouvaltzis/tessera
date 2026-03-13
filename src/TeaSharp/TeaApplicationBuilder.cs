namespace TeaSharp;

/// <summary>
/// Builds a runnable TeaSharp application from a <see cref="TeaApp"/> and runtime options.
/// </summary>
/// <remarks>
/// Use this type when startup needs multiple steps or when you want a built <see cref="TeaApplication"/>
/// instance that can be controlled explicitly after creation.
/// </remarks>
public sealed class TeaApplicationBuilder
{
    private Func<TeaApp>? _appFactory;
    private readonly TeaRuntimeOptions _runtime = new();

    /// <summary>
    /// Gets the runtime options that will be applied to the built application.
    /// </summary>
    public TeaRuntimeOptions Runtime => _runtime;

    /// <summary>
    /// Configures the application to create a new <typeparamref name="TApp"/> instance when built.
    /// </summary>
    /// <typeparam name="TApp">The application type.</typeparam>
    /// <returns>The current builder.</returns>
    public TeaApplicationBuilder UseApp<TApp>()
        where TApp : TeaApp, new()
    {
        _appFactory = static () => new TApp();
        return this;
    }

    /// <summary>
    /// Configures the application to create its <see cref="TeaApp"/> from the supplied factory.
    /// </summary>
    /// <param name="factory">The application factory.</param>
    /// <returns>The current builder.</returns>
    public TeaApplicationBuilder UseApp(Func<TeaApp> factory)
    {
        _appFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        return this;
    }

    /// <summary>
    /// Configures the application to run the supplied <see cref="TeaApp"/> instance.
    /// </summary>
    /// <param name="app">The application instance to run.</param>
    /// <returns>The current builder.</returns>
    public TeaApplicationBuilder UseApp(TeaApp app)
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
    public TeaApplicationBuilder ConfigureRuntime(Action<TeaRuntimeOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(_runtime);
        return this;
    }

    /// <summary>
    /// Builds a runnable <see cref="TeaApplication"/>.
    /// </summary>
    /// <returns>The built application.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no application has been configured.</exception>
    public TeaApplication Build()
    {
        if (_appFactory is null)
        {
            throw new InvalidOperationException("No TeaApp factory configured. Call UseApp(...) before Build().");
        }

        return new TeaApplication(_appFactory(), _runtime);
    }
}
