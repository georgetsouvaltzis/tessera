using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;

namespace TeaSharp.Hosting;

/// <summary>
/// Exposes advanced runtime-program hosting seams below the default TeaApp startup surface.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class TeaHost
{
    /// <summary>
    /// Creates an application using the advanced hosting surface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TeaApplication CreateApplication(TeaApp app, TeaRuntimeOptions? options = null, TeaHostingOptions? hosting = null) =>
        new(app, options, hosting);

    /// <summary>
    /// Runs an application using the advanced hosting surface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static async Task<TeaApp> RunAsync(TeaApp app, TeaRuntimeOptions? options = null, TeaHostingOptions? hosting = null, CancellationToken cancellationToken = default)
    {
        var application = CreateApplication(app, options, hosting);
        await application.RunAsync(cancellationToken).ConfigureAwait(false);
        return app;
    }

    /// <summary>
    /// Creates a program using the advanced program-hosting surface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TeaProgram CreateProgram(IScreen screen) =>
        new(screen, new TeaProgramOptions().ToProgramOptions());

    /// <summary>
    /// Creates a program using the advanced runtime configuration surface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TeaProgram CreateProgram(IScreen screen, ProgramOptions? options) =>
        new(screen, options);

    /// <summary>
    /// Creates a program using the advanced program-hosting surface with legacy runtime options.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TeaProgram CreateProgram(IScreen screen, TeaProgramOptions options) =>
        new(screen, options?.ToProgramOptions());
}
