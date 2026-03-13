namespace TeaSharp.Controls;

/// <summary>
/// Represents one action exposed by a <see cref="MenuBar"/>.
/// </summary>
public sealed record MenuItem(string Id, string Text, char Shortcut = '\0');
