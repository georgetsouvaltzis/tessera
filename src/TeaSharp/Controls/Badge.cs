using LegacyBadgeComponent = TeaSharp.Components.Advanced.BadgeComponent;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;

namespace TeaSharp.Controls;

public sealed class Badge : Control
{
    private readonly LegacyBadgeComponent _component = new();

    public string Text
    {
        get => _component.Text;
        set => _component.Text = value ?? string.Empty;
    }

    public bool ShowBrackets
    {
        get => _component.ShowBrackets;
        set => _component.ShowBrackets = value;
    }

    public BadgeTone Tone { get; set; }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.State = Tone switch
        {
            BadgeTone.Success => WidgetVisualState.Success,
            BadgeTone.Warning => WidgetVisualState.Warning,
            BadgeTone.Error => WidgetVisualState.Error,
            _ => WidgetVisualState.Default,
        };

        _component.Render(canvas, rect);
    }
}
