namespace TeaSharp.Components;

public sealed record LayoutContainerOptions(
    LayoutContainerMode Mode = LayoutContainerMode.Vertical,
    int GridRows = 1,
    int GridColumns = 1,
    bool EnableMouseInteractions = true,
    bool ClickToFocusChildren = true,
    bool EnableMouseResize = true,
    int SplitterHitThickness = 1,
    int MinPrimarySize = 8,
    int MinSecondarySize = 8,
    KeyboardRoutingMode KeyboardRoutingMode = KeyboardRoutingMode.FocusedOnly,
    int? PrimarySize = null);
