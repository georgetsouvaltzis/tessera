using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class DialogComponent : IStatefulComponent, IFocusableComponent
{
    public DialogComponent()
    {
    }

    public DialogComponent(DialogOptions options)
    {
        Title = options.Title;
        Lines = options.Lines ?? ["Confirm?"];
        Visible = options.Visible;
        Focused = options.Focused;
        BorderStyle = options.BorderStyle;
        Theme = options.Theme ?? new UiTheme();
        AcceptKey = options.AcceptKey ?? new KeyBinding("enter/space", "accept", "enter", "space");
        DismissKey = options.DismissKey ?? new KeyBinding("esc", "dismiss", "escape");
    }

    public string Title { get; set; } = "Dialog";

    public IReadOnlyList<string> Lines { get; set; } = ["Confirm?"];

    public bool Visible { get; set; }

    public bool Focused { get; set; }

    public BorderStyle BorderStyle { get; set; } = BorderStyle.Rounded;

    public UiTheme Theme { get; set; } = new();

    public DialogResult LastResult { get; private set; }

    public KeyBinding AcceptKey { get; set; } = new("enter/space", "accept", "enter", "space");

    public KeyBinding DismissKey { get; set; } = new("esc", "dismiss", "escape");

    public bool Update(IMessage message)
    {
        if (!Visible || !Focused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (DismissKey.Matches(key))
        {
            Visible = false;
            LastResult = DialogResult.Dismissed;
            return true;
        }

        if (AcceptKey.Matches(key))
        {
            Visible = false;
            LastResult = DialogResult.Accepted;
            return true;
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!Visible)
        {
            return;
        }

        var modal = new ModalComponent
        {
            Visible = true,
            Title = Title,
            Lines = Lines,
            BorderStyle = BorderStyle,
            Theme = Theme,
        };
        modal.Render(canvas, rect);
    }
}
