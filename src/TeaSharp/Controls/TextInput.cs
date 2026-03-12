using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Controls;

public sealed class TextInput : Control
{
    private readonly TextInputComponent _component = new();

    public event EventHandler<TextInputSubmittedEventArgs>? Submitted;

    public event EventHandler<TextInputCancelledEventArgs>? Cancelled;

    public TextInput()
    {
        _component.Submitted += (_, args) => Submitted?.Invoke(this, new TextInputSubmittedEventArgs(args.Value));
        _component.Cancelled += (_, args) => Cancelled?.Invoke(this, new TextInputCancelledEventArgs(args.Value));
    }

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    public string Placeholder
    {
        get => _component.Placeholder;
        set => _component.Placeholder = value ?? string.Empty;
    }

    public string Value => _component.Value;

    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    public bool ClearOnSubmit
    {
        get => _component.ClearOnSubmit;
        set => _component.ClearOnSubmit = value;
    }

    public bool ClearOnCancel
    {
        get => _component.ClearOnCancel;
        set => _component.ClearOnCancel = value;
    }

    public int MaxLength
    {
        get => _component.MaxLength;
        set => _component.MaxLength = value;
    }

    public bool MaskInput
    {
        get => _component.MaskInput;
        set => _component.MaskInput = value;
    }

    public char MaskCharacter
    {
        get => _component.MaskCharacter;
        set => _component.MaskCharacter = value;
    }

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public void SetValue(string value) => _component.SetValue(value ?? string.Empty);

    public void Clear() => _component.Clear();

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeSubmission(out string value) => _component.TryConsumeSubmit(out value);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeCancellation(out string value) => _component.TryConsumeCancel(out value);

    public override bool Handle(Message message)
    {
        return Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
