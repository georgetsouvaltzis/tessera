using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Charting;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct BarDatum(string Label, double Value);
