namespace TeaSharp;

public sealed class TeaApplication
{
    private readonly TeaApp _app;
    private readonly TeaRuntimeOptions _options;
    private readonly global::TeaSharp.Hosting.TeaHostingOptions? _hosting;
    private readonly global::TeaSharp.Core.Application.TeaProgram _program;

    internal TeaApplication(TeaApp app, TeaRuntimeOptions? options = null, global::TeaSharp.Hosting.TeaHostingOptions? hosting = null)
    {
        _app = app ?? throw new ArgumentNullException(nameof(app));
        _options = options ?? new TeaRuntimeOptions();
        _hosting = hosting;
        _app.ConfigureRuntimeScreen(_options.Screen);
        _program = new global::TeaSharp.Core.Application.TeaProgram(_app.RuntimeScreen, _options.ToProgramOptions(_app, _hosting));
    }

    public TeaApp App => _app;

    public TeaRuntimeOptions Options => _options;

    internal global::TeaSharp.Hosting.TeaHostingOptions? HostingOptions => _hosting;

    public void Send(Message message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _program.Send(TeaSharp.Internal.TeaMessageAdapter.ToCore(message));
    }

    public async Task<TeaApp> RunAsync(CancellationToken cancellationToken = default)
    {
        await _program.RunAsync(cancellationToken).ConfigureAwait(false);
        return _app;
    }

    public Task StopAsync(bool kill = false, CancellationToken cancellationToken = default)
    {
        return _program.StopAsync(kill, cancellationToken);
    }
}
