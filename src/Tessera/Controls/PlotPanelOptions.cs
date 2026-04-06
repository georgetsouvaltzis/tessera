using System.ComponentModel;

namespace Tessera.Controls;

/// <summary>
/// Defines advanced layout options for a <see cref="PlotPanel"/>.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct PlotPanelOptions(
    int Columns = 1,
    int Spacing = 1);
