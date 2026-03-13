using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class DialogComponent : IStatefulComponent, IFocusableComponent
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
        BodyLines = options.BodyLines ?? ["Confirm?"];
        IsVisible = options.IsVisible;
        IsFocused = options.IsFocused;
        Border = options.Border;
        Padding = options.Padding;
        Theme = options.Theme ?? new UiTheme();
        AcceptKey = options.AcceptKey ?? new KeyBinding("enter/space", "accept", "enter", "space");
        DismissKey = options.DismissKey ?? new KeyBinding("esc", "dismiss", "escape");
    }

    public string Title { get; set; } = "Dialog";

    public IReadOnlyList<string> BodyLines { get; set; } = ["Confirm?"];

    public bool IsVisible { get; set; }

    public bool IsFocused { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.Rounded;

    public Thickness Padding { get; set; }

    public UiTheme Theme { get; set; }

    public DialogResult LastResult { get; private set; }

    public KeyBinding AcceptKey { get; set; } = new("enter/space", "accept", "enter", "space");

    public KeyBinding DismissKey { get; set; } = new("esc", "dismiss", "escape");

    /// <summary>
    /// Raised when the dialog is accepted.
    /// </summary>
    public event EventHandler? Accepted;

    /// <summary>
    /// Raised when the dialog is dismissed.
    /// </summary>
    public event EventHandler? Dismissed;

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
        if (!IsVisible || !IsFocused || message is not KeyPressMsg key)
        {
            return false;
        }

        if (DismissKey.Matches(key))
        {
            return ApplyResult(DialogResult.Dismissed);
        }

        if (AcceptKey.Matches(key))
        {
            return ApplyResult(DialogResult.Accepted);
        }

        return false;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!IsVisible)
        {
            return;
        }

        var modal = new ModalComponent
        {
            IsVisible = true,
            Title = Title,
            BodyLines = BodyLines,
            Border = Border,
            Padding = Padding,
            Theme = Theme,
        };
        modal.Render(canvas, rect);
    }

    private bool ApplyResult(DialogResult result)
    {
        IsVisible = false;
        LastResult = result;
        _resultVersion++;
        if (result == DialogResult.Accepted)
        {
            Accepted?.Invoke(this, EventArgs.Empty);
        }
        else if (result == DialogResult.Dismissed)
        {
            Dismissed?.Invoke(this, EventArgs.Empty);
        }

        return true;
    }
}
