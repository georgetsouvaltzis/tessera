using System.ComponentModel;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal interface IWidgetKeyMap
{
    IReadOnlyList<KeyBinding> HelpBindings { get; }
}
