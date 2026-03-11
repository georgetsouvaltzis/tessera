using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Commands;

namespace TeaSharp;

/// <summary>
/// Provides the primary application-facing entry points for TeaSharp programs and effects.
/// </summary>
public static class Tea
{
    /// <summary>
    /// Creates a program using the stable application-facing host defaults.
    /// </summary>
    /// <param name="screen">The initial application screen.</param>
    /// <returns>A program ready to run.</returns>
    public static TeaProgram CreateProgram(IScreen screen) =>
        new(screen, new TeaProgramOptions().ToProgramOptions());

    /// <summary>
    /// Creates a program using the advanced runtime configuration surface.
    /// </summary>
    /// <param name="screen">The initial application screen.</param>
    /// <param name="options">Advanced runtime options for terminal, renderer, and host customization.</param>
    /// <returns>A program ready to run.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TeaProgram CreateProgram(IScreen screen, ProgramOptions? options) =>
        new(screen, options);

    /// <summary>
    /// Creates a program using the stable application-facing host configuration surface.
    /// </summary>
    /// <param name="screen">The initial application screen.</param>
    /// <param name="options">Application-facing runtime options.</param>
    /// <returns>A program ready to run.</returns>
    public static TeaProgram CreateProgram(IScreen screen, TeaProgramOptions options) =>
        new(screen, options?.ToProgramOptions());

    /// <summary>
    /// Exposes the common effect helpers used by TeaSharp applications.
    /// </summary>
    public static class Effects
    {
        /// <summary>Creates an effect that requests program shutdown.</summary>
        public static Effect Quit => TeaSharp.Core.Commands.Effects.Quit;
        /// <summary>Creates an effect that interrupts the running program.</summary>
        public static Effect Interrupt => TeaSharp.Core.Commands.Effects.Interrupt;
        /// <summary>Creates an effect that emits a message once after the given delay.</summary>
        public static Effect Tick(TimeSpan delay, Func<DateTimeOffset, IMessage> factory) => TeaSharp.Core.Commands.Effects.Tick(delay, factory);
        /// <summary>Creates a repeating timer effect that emits messages at the given interval.</summary>
        public static Effect Every(TimeSpan delay, Func<DateTimeOffset, IMessage> factory) => TeaSharp.Core.Commands.Effects.Every(delay, factory);
        /// <summary>Creates an effect that runs multiple effects concurrently.</summary>
        public static Effect? Batch(params Effect?[] effects) => TeaSharp.Core.Commands.Effects.Batch(effects);
        /// <summary>Creates an effect that runs multiple effects in order.</summary>
        public static Effect? Sequence(params Effect?[] effects) => TeaSharp.Core.Commands.Effects.Sequence(effects);
        /// <summary>Creates an effect that writes raw terminal output.</summary>
        public static Effect Raw(string content) => TeaSharp.Core.Commands.Effects.Raw(content);
        /// <summary>Creates an effect that requests a terminal capability report.</summary>
        public static Effect RequestCapability(string name) => TeaSharp.Core.Commands.Effects.RequestCapability(name);
        /// <summary>Creates an effect that writes clipboard content to the standard selection.</summary>
        public static Effect SetClipboard(string content) => TeaSharp.Core.Commands.Effects.SetClipboard(content);
        /// <summary>Creates an effect that reads clipboard content from the standard selection.</summary>
        public static Effect ReadClipboard() => TeaSharp.Core.Commands.Effects.ReadClipboard();
        /// <summary>Creates an effect that writes clipboard content to the primary selection.</summary>
        public static Effect SetPrimaryClipboard(string content) => TeaSharp.Core.Commands.Effects.SetPrimaryClipboard(content);
        /// <summary>Creates an effect that reads clipboard content from the primary selection.</summary>
        public static Effect ReadPrimaryClipboard() => TeaSharp.Core.Commands.Effects.ReadPrimaryClipboard();
        /// <summary>Creates an effect that requests the terminal foreground color.</summary>
        public static Effect RequestForegroundColor() => TeaSharp.Core.Commands.Effects.RequestForegroundColor();
        /// <summary>Creates an effect that requests the terminal background color.</summary>
        public static Effect RequestBackgroundColor() => TeaSharp.Core.Commands.Effects.RequestBackgroundColor();
        /// <summary>Creates an effect that requests the terminal cursor color.</summary>
        public static Effect RequestCursorColor() => TeaSharp.Core.Commands.Effects.RequestCursorColor();
    }
}
