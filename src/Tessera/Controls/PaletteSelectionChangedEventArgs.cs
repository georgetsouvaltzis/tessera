namespace Tessera.Controls;

/// <summary>
///     Provides previous/current values when <see cref="PaletteEditor.SelectionChanged" /> fires.
/// </summary>
public sealed class PaletteSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new selection-change payload.
    /// </summary>
    /// <param name="previousIndex">Index selected before the change.</param>
    /// <param name="selectedIndex">Index selected after the change.</param>
    /// <param name="previousSwatch">Swatch selected before the change.</param>
    /// <param name="selectedSwatch">Swatch selected after the change.</param>
    public PaletteSelectionChangedEventArgs(
        int previousIndex,
        int selectedIndex,
        PaletteSwatch? previousSwatch,
        PaletteSwatch? selectedSwatch)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousSwatch = previousSwatch;
        SelectedSwatch = selectedSwatch;
    }

    /// <summary>
    ///     Gets index selected before the change.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    ///     Gets index selected after the change.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    ///     Gets swatch selected before the change.
    /// </summary>
    public PaletteSwatch? PreviousSwatch { get; }

    /// <summary>
    ///     Gets swatch selected after the change.
    /// </summary>
    public PaletteSwatch? SelectedSwatch { get; }
}
