namespace TeaSharp;

public sealed class TeaApplicationBuilder
{
    private Func<TeaApp>? _appFactory;
    private readonly TeaRuntimeOptions _runtime = new();

    public TeaRuntimeOptions Runtime => _runtime;

    public TeaApplicationBuilder UseApp<TApp>()
        where TApp : TeaApp, new()
    {
        _appFactory = static () => new TApp();
        return this;
    }

    public TeaApplicationBuilder UseApp(Func<TeaApp> factory)
    {
        _appFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        return this;
    }

    public TeaApplicationBuilder UseApp(TeaApp app)
    {
        ArgumentNullException.ThrowIfNull(app);
        _appFactory = () => app;
        return this;
    }

    public TeaApplicationBuilder ConfigureRuntime(Action<TeaRuntimeOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        configure(_runtime);
        return this;
    }

    public TeaApplication Build()
    {
        if (_appFactory is null)
        {
            throw new InvalidOperationException("No TeaApp factory configured. Call UseApp(...) before Build().");
        }

        return new TeaApplication(_appFactory(), _runtime);
    }
}
