using System.ComponentModel;

namespace Tessera.Controls;

/// <summary>
///     Provides details when a <see cref="PropertyGrid" /> selection changes.
/// </summary>
public sealed class PropertyGridSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a new selection-change payload.
    /// </summary>
    /// <param name="previousIndex">The selected index before the change.</param>
    /// <param name="currentIndex">The selected index after the change.</param>
    /// <param name="previousProperty">The selected property before the change.</param>
    /// <param name="currentProperty">The selected property after the change.</param>
    public PropertyGridSelectionChangedEventArgs(
        int previousIndex,
        int currentIndex,
        PropertyGridProperty? previousProperty,
        PropertyGridProperty? currentProperty)
    {
        PreviousIndex = previousIndex;
        CurrentIndex = currentIndex;
        PreviousProperty = previousProperty;
        CurrentProperty = currentProperty;
    }

    /// <summary>
    ///     Gets the selected index before the change.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    ///     Gets the selected index after the change.
    ///     Compatibility alias for <see cref="SelectedIndex" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public int CurrentIndex { get; }

    /// <summary>
    ///     Gets the selected index after the change.
    ///     Canonical property for selection access.
    /// </summary>
    public int SelectedIndex => CurrentIndex;

    /// <summary>
    ///     Gets the selected property before the change.
    /// </summary>
    public PropertyGridProperty? PreviousProperty { get; }

    /// <summary>
    ///     Gets the selected property after the change.
    ///     Compatibility alias for <see cref="SelectedProperty" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public PropertyGridProperty? CurrentProperty { get; }

    /// <summary>
    ///     Gets the selected property after the change.
    ///     Canonical property for selection access.
    /// </summary>
    public PropertyGridProperty? SelectedProperty => CurrentProperty;
}
