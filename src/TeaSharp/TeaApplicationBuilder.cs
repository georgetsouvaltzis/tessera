using Microsoft.Extensions.DependencyInjection;

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
    private Func<IServiceProvider, TeaApp>? _appFactory;
    private readonly TeaRuntimeOptions _runtime = new();
    private readonly ServiceCollection _services = new();

    /// <summary>
    /// Gets the runtime options that will be applied to the built application.
    /// </summary>
    public TeaRuntimeOptions Runtime => _runtime;

    /// <summary>
    /// Gets the service collection used to construct the configured <see cref="TeaApp"/>.
    /// </summary>
    /// <remarks>
    /// Register dependencies here when the app type configured by <see cref="UseApp{TApp}"/> requires
    /// constructor injection.
    /// </remarks>
    public IServiceCollection Services => _services;

    /// <summary>
    /// Configures the application to create a new <typeparamref name="TApp"/> instance when built.
    /// </summary>
    /// <typeparam name="TApp">The application type.</typeparam>
    /// <returns>The current builder.</returns>
    public TeaApplicationBuilder UseApp<TApp>()
        where TApp : TeaApp
    {
        _appFactory = static services => ActivatorUtilities.CreateInstance<TApp>(services);
        return this;
    }

    /// <summary>
    /// Configures the application to create its <see cref="TeaApp"/> from the supplied service-aware factory.
    /// </summary>
    /// <param name="factory">The application factory.</param>
    /// <returns>The current builder.</returns>
    public TeaApplicationBuilder UseApp(Func<IServiceProvider, TeaApp> factory)
    {
        _appFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        return this;
    }

    /// <summary>
    /// Configures the application to create its <see cref="TeaApp"/> from the supplied factory.
    /// </summary>
    /// <param name="factory">The application factory.</param>
    /// <returns>The current builder.</returns>
    public TeaApplicationBuilder UseApp(Func<TeaApp> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        _appFactory = _ => factory();
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
        _appFactory = _ => app;
        return this;
    }

    /// <summary>
    /// Applies dependency registration before the application is built.
    /// </summary>
    /// <param name="configure">The callback that mutates <see cref="Services"/>.</param>
    /// <returns>The current builder.</returns>
    public TeaApplicationBuilder ConfigureServices(Action<IServiceCollection> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(_services);
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

        var services = _services.BuildServiceProvider();
        var app = _appFactory(services)
            ?? throw new InvalidOperationException("Configured TeaApp factory returned null.");
        return new TeaApplication(app, _runtime);
    }
}
