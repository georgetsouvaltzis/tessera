using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using System.ComponentModel;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a single-line editable text field.
/// </summary>
/// <remarks>
/// Use <see cref="Submitted"/> and <see cref="Cancelled"/> as the normal interaction hooks. The advanced
/// polling methods remain only for transitional interop.
/// </remarks>
public sealed class TextInput : Control
{
    private readonly TextInputComponent _component = new();

    /// <summary>
    /// Occurs when the current value is submitted.
    /// </summary>
    public event EventHandler<TextInputSubmittedEventArgs>? Submitted;

    /// <summary>
    /// Occurs when editing is canceled.
    /// </summary>
    public event EventHandler<TextInputCancelledEventArgs>? Cancelled;

    public TextInput()
    {
        _component.Submitted += (_, args) => Submitted?.Invoke(this, new TextInputSubmittedEventArgs(args.Value));
        _component.Cancelled += (_, args) => Cancelled?.Invoke(this, new TextInputCancelledEventArgs(args.Value));
    }

    /// <summary>
    /// Gets or sets the field title.
    /// </summary>
    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets the placeholder shown when the field is empty.
    /// </summary>
    public string Placeholder
    {
        get => _component.Placeholder;
        set => _component.Placeholder = value ?? string.Empty;
    }

    /// <summary>
    /// Gets the current input value.
    /// </summary>
    public string Value => _component.Value;

    /// <summary>
    /// Gets or sets the field border style.
    /// </summary>
    public BorderStyle Border
    {
        get => _component.Border;
        set => _component.Border = value;
    }

    /// <summary>
    /// Gets or sets the inner padding applied to the field body.
    /// </summary>
    public Thickness Padding
    {
        get => _component.Padding;
        set => _component.Padding = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field should clear after submission.
    /// </summary>
    public bool ClearOnSubmit
    {
        get => _component.ClearOnSubmit;
        set => _component.ClearOnSubmit = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field should clear after cancellation.
    /// </summary>
    public bool ClearOnCancel
    {
        get => _component.ClearOnCancel;
        set => _component.ClearOnCancel = value;
    }

    /// <summary>
    /// Gets or sets the maximum accepted input length.
    /// </summary>
    public int MaxLength
    {
        get => _component.MaxLength;
        set => _component.MaxLength = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field masks typed characters.
    /// </summary>
    public bool MaskInput
    {
        get => _component.MaskInput;
        set => _component.MaskInput = value;
    }

    /// <summary>
    /// Gets or sets the masking character used when <see cref="MaskInput"/> is enabled.
    /// </summary>
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

    /// <summary>
    /// Replaces the current field value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void SetValue(string value) => _component.SetValue(value ?? string.Empty);

    /// <summary>
    /// Clears the current field value.
    /// </summary>
    public void Clear() => _component.Clear();

    /// <summary>
    /// Attempts to consume a pending submission from the wrapped legacy component.
    /// </summary>
    /// <param name="value">Receives the submitted value when available.</param>
    /// <returns><see langword="true"/> when a submission was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeSubmission(out string value) => _component.TryConsumeSubmit(out value);

    /// <summary>
    /// Attempts to consume a pending cancellation from the wrapped legacy component.
    /// </summary>
    /// <param name="value">Receives the canceled value when available.</param>
    /// <returns><see langword="true"/> when a cancellation was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeCancellation(out string value) => _component.TryConsumeCancel(out value);

    public override bool Handle(Message message)
    {
        return ControlForwarder.Forward(_component, message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _component.Render(canvas, rect);
    }
}
