using System.ComponentModel;

namespace Tessera.Hosting;

/// <summary>
///     Exposes advanced runtime-program hosting seams below the default TesseraApp startup surface.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class TesseraHost
{
    /// <summary>
    ///     Creates an application using the advanced hosting surface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TesseraApplication CreateApplication(TesseraApp app, TesseraRuntimeOptions? options = null,
        TesseraHostingOptions? hosting = null)
    {
        return new TesseraApplication(app, options, hosting);
    }

    /// <summary>
    ///     Runs an application using the advanced hosting surface.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static async Task<TesseraApp> RunAsync(TesseraApp app, TesseraRuntimeOptions? options = null,
        TesseraHostingOptions? hosting = null, CancellationToken cancellationToken = default)
    {
        var application = CreateApplication(app, options, hosting);
        await application.RunAsync(cancellationToken).ConfigureAwait(false);
        return app;
    }
}
