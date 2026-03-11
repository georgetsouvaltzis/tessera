using System.ComponentModel;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Interaction;

[EditorBrowsable(EditorBrowsableState.Advanced)]
public sealed class WidgetInteractionProfile
{
    public static WidgetInteractionProfile Default { get; } = new();

    public static WidgetInteractionProfile KeyboardOnly { get; } = new()
    {
        HoverOnMotion = false,
        HoverOnClick = false,
        ActivateOnClick = false,
        NavigateOnWheel = false,
        OpenOnClick = false,
    };

    public bool HoverOnMotion { get; set; } = true;

    public bool HoverOnClick { get; set; } = true;

    public bool ActivateOnClick { get; set; } = true;

    public bool NavigateOnWheel { get; set; } = true;

    public bool OpenOnClick { get; set; } = true;

    internal static WidgetInteractionProfile CloneOrDefault(WidgetInteractionProfile? profile)
    {
        return profile?.Clone() ?? Default.Clone();
    }

    public WidgetInteractionProfile Clone()
    {
        return new WidgetInteractionProfile
        {
            HoverOnMotion = HoverOnMotion,
            HoverOnClick = HoverOnClick,
            ActivateOnClick = ActivateOnClick,
            NavigateOnWheel = NavigateOnWheel,
            OpenOnClick = OpenOnClick,
        };
    }
}
