namespace TeaSharp.Components;

public sealed record StatusBarOptions(
    string LeftText = "",
    string RightText = "",
    UiTheme? Theme = null);
