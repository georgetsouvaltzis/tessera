namespace TeaSharp.Controls;

/// <summary>
/// Represents one command entry rendered by a <see cref="CommandBar"/>.
/// </summary>
/// <param name="Id">Stable command identifier used by activation handlers.</param>
/// <param name="Text">Command label shown in the bar.</param>
/// <param name="Shortcut">Optional single-key shortcut that can activate this command.</param>
/// <param name="IsDisabled"><see langword="true"/> when the command should be shown but not activated.</param>
public sealed record CommandBarItem(string Id, string Text, char Shortcut = '\0', bool IsDisabled = false);
