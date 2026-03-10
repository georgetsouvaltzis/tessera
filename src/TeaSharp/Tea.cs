using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Commands;

namespace TeaSharp;

/// <summary>
/// Provides the primary application-facing entry points for TeaSharp programs and commands.
/// </summary>
public static class Tea
{
    /// <summary>
    /// Creates a program using the advanced runtime configuration surface.
    /// </summary>
    /// <param name="model">The initial application model.</param>
    /// <param name="options">Advanced runtime options for terminal, renderer, and host customization.</param>
    /// <returns>A program ready to run.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static TeaProgram NewProgram(IModel model, ProgramOptions? options = null) =>
        new(model, options);

    /// <summary>
    /// Creates a program using the stable application-facing host configuration surface.
    /// </summary>
    /// <param name="model">The initial application model.</param>
    /// <param name="options">Application-facing runtime options.</param>
    /// <returns>A program ready to run.</returns>
    public static TeaProgram NewProgram(IModel model, TeaProgramOptions options) =>
        new(model, options?.ToProgramOptions());

    /// <summary>
    /// Exposes the common command helpers used by TeaSharp applications.
    /// </summary>
    public static class Cmd
    {
        /// <summary>Creates a command that requests program shutdown.</summary>
        public static Command Quit => Commands.Quit;
        /// <summary>Creates a command that interrupts the running program.</summary>
        public static Command Interrupt => Commands.Interrupt;
        /// <summary>Creates a command that emits a message once after the given delay.</summary>
        public static Command Tick(TimeSpan delay, Func<DateTimeOffset, IMessage> factory) => Commands.Tick(delay, factory);
        /// <summary>Creates a repeating timer command that emits messages at the given interval.</summary>
        public static Command Every(TimeSpan delay, Func<DateTimeOffset, IMessage> factory) => Commands.Every(delay, factory);
        /// <summary>Creates a command that runs multiple commands concurrently.</summary>
        public static Command? Batch(params Command?[] commands) => Commands.Batch(commands);
        /// <summary>Creates a command that runs multiple commands in order.</summary>
        public static Command? Sequence(params Command?[] commands) => Commands.Sequence(commands);
        /// <summary>Creates a command that writes raw terminal output.</summary>
        public static Command Raw(string content) => Commands.Raw(content);
        /// <summary>Creates a command that requests a terminal capability report.</summary>
        public static Command RequestCapability(string name) => Commands.RequestCapability(name);
        /// <summary>Creates a command that writes clipboard content to the standard selection.</summary>
        public static Command SetClipboard(string content) => Commands.SetClipboard(content);
        /// <summary>Creates a command that reads clipboard content from the standard selection.</summary>
        public static Command ReadClipboard() => Commands.ReadClipboard();
        /// <summary>Creates a command that writes clipboard content to the primary selection.</summary>
        public static Command SetPrimaryClipboard(string content) => Commands.SetPrimaryClipboard(content);
        /// <summary>Creates a command that reads clipboard content from the primary selection.</summary>
        public static Command ReadPrimaryClipboard() => Commands.ReadPrimaryClipboard();
        /// <summary>Creates a command that requests the terminal foreground color.</summary>
        public static Command RequestForegroundColor() => Commands.RequestForegroundColor();
        /// <summary>Creates a command that requests the terminal background color.</summary>
        public static Command RequestBackgroundColor() => Commands.RequestBackgroundColor();
        /// <summary>Creates a command that requests the terminal cursor color.</summary>
        public static Command RequestCursorColor() => Commands.RequestCursorColor();
    }
}
