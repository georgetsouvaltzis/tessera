using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using System.ComponentModel;
namespace TeaSharp.Components.Dashboard;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public readonly record struct StatsCardItem(string Label, string Value);
