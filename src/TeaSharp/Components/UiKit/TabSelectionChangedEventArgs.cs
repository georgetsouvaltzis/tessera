using System.ComponentModel;

namespace TeaSharp.Components.UiKit;

/// <summary>
/// Describes a tab selection transition.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class TabSelectionChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new tab-selection payload.
    /// </summary>
    /// <param name="previousIndex">The previously selected tab index.</param>
    /// <param name="selectedIndex">The current selected tab index.</param>
    /// <param name="previousTab">The previously selected tab label.</param>
    /// <param name="selectedTab">The current selected tab label.</param>
    public TabSelectionChangedEventArgs(int previousIndex, int selectedIndex, string previousTab, string selectedTab)
    {
        PreviousIndex = previousIndex;
        SelectedIndex = selectedIndex;
        PreviousTab = previousTab ?? string.Empty;
        SelectedTab = selectedTab ?? string.Empty;
    }

    /// <summary>
    /// Gets the previously selected tab index.
    /// </summary>
    public int PreviousIndex { get; }

    /// <summary>
    /// Gets the current selected tab index.
    /// </summary>
    public int SelectedIndex { get; }

    /// <summary>
    /// Gets the previously selected tab label.
    /// </summary>
    public string PreviousTab { get; }

    /// <summary>
    /// Gets the current selected tab label.
    /// </summary>
    public string SelectedTab { get; }
}
