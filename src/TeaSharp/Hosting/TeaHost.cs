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
