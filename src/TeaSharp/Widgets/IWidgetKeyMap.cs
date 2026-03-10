using System.ComponentModel;

namespace TeaSharp.Widgets;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IWidgetKeyMap
{
    IReadOnlyList<KeyBinding> HelpBindings { get; }
}
