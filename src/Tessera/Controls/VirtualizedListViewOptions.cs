namespace Tessera.Controls;

/// <summary>
///     Defines advanced behavior options for <see cref="VirtualizedListView{T}" />.
/// </summary>
public readonly record struct VirtualizedListViewOptions(
    int WheelStep = 3,
    bool KeepSelectionCentered = false);
