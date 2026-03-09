namespace TeaSharp.Widgets;

public interface IWidgetKeyMap
{
    IReadOnlyList<KeyBinding> HelpBindings { get; }
}
