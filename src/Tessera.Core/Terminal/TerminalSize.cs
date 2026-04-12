namespace Tessera.Core.Terminal;

/// <summary>
/// Represents the current terminal viewport size in character cells.
/// </summary>
/// <param name="Width">The terminal width in columns.</param>
/// <param name="Height">The terminal height in rows.</param>
public readonly record struct TerminalSize(int Width, int Height);
