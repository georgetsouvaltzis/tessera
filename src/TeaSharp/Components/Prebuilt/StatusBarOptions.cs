namespace TeaSharp.Components.Prebuilt;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="StatusBarComponent"/>.
/// </summary>
public sealed record StatusBarOptions(
    string LeftText = "",
    string RightText = "",
    UiTheme? Theme = null);
