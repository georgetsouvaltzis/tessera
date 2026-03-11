using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class DialogComponent : IStatefulComponent, IFocusableComponent
{
    private long _resultVersion;
    private long _consumedResultVersion;

    public DialogComponent()
    {
        Theme = new UiTheme();
    }

    public DialogComponent(DialogOptions options)
    {
        Title = options.Title;
        Lines = options.Lines ?? ["Confirm?"];
        Visible = options.Visible;
        Focused = options.Focused;
        Border = options.Border;
        Padding = options.Padding;
        Theme = options.Theme ?? new UiTheme();
        AcceptKey = options.AcceptKey ?? new KeyBinding("enter/space", "accept", "enter", "space");
        DismissKey = options.DismissKey ?? new KeyBinding("esc", "dismiss", "escape");
    }

    public string Title { get; set; } = "Dialog";

    public IReadOnlyList<string> Lines { get; set; } = ["Confirm?"];

    public bool Visible { get; set; }

    public bool Focused { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.Rounded;

    public Thickness Padding { get; set; }

    public UiTheme Theme { get; set; }

    public DialogResult LastResult { get; private set; }

    public KeyBinding AcceptKey { get; set; } = new("enter/space", "accept", "enter", "space");

    public KeyBinding DismissKey { get; set; } = new("esc", "dismiss", "escape");

    /// <summary>
    /// Consumes the latest dialog result exactly once.
    /// </summary>
    public bool TryConsumeResult(out DialogResult result)
    {
        if (_resultVersion == _consumedResultVersion)
        {
            result = DialogResult.None;
            return false;
        }

        _consumedResultVersion = _resultVersion;
        result = LastResult;
        return true;
    }

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
            _resultVersion++;
            return true;
        }

        if (AcceptKey.Matches(key))
        {
            Visible = false;
            LastResult = DialogResult.Accepted;
            _resultVersion++;
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
            Border = Border,
            Padding = Padding,
            Theme = Theme,
        };
        modal.Render(canvas, rect);
    }
}
