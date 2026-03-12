using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using TeaSharp.Components.Primitives;

namespace TeaSharp.Layout;

/// <summary>
/// Creates ordered row and column layouts.
/// </summary>
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "Stack is the intentional public layout noun for ordered rows and columns.")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public static class Stack
{
    /// <summary>
    /// Arranges the provided slots left-to-right.
    /// </summary>
    public static StackLayout Row(int gap = 0, Thickness padding = default, params LayoutSlot[] children) =>
        new(true, children, gap, padding);

    /// <summary>
    /// Arranges the provided slots top-to-bottom.
    /// </summary>
    public static StackLayout Column(int gap = 0, Thickness padding = default, params LayoutSlot[] children) =>
        new(false, children, gap, padding);
}
