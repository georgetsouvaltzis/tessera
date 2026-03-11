namespace TeaSharp.Components;

/// <summary>
/// Defines the one-shot configuration used to construct a <see cref="LayoutContainerComponent"/>.
/// </summary>
public sealed record LayoutContainerOptions(
    LayoutContainerMode Mode = LayoutContainerMode.Vertical,
    int GridRows = 1,
    int GridColumns = 1,
    bool EnableMouseInteractions = true,
    bool EnableMouseResize = true,
    int SplitterHitThickness = 1,
    int MinPrimarySize = 8,
    int MinSecondarySize = 8,
    int? PrimarySize = null);
