using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

public sealed class TextInputComponent : IStatefulComponent, IFocusableComponent
{
    private readonly TextInputModel _input = new();
    private int _consumedSubmitCount;
    private int _consumedCancelCount;

    public TextInputComponent()
    {
    }

    public TextInputComponent(TextInputOptions options)
    {
        Title = options.Title;
        IsFocused = options.IsFocused;
        Border = options.Border;
        Padding = options.Padding;
        ClearOnSubmit = options.ClearOnSubmit;
        ClearOnCancel = options.ClearOnCancel;
        KeyMap = options.KeyMap ?? TextInputKeyMap.Default;
        CancelKey = options.CancelKey ?? new KeyBinding("esc", "cancel", "escape");
        Placeholder = options.Placeholder;
        MaxLength = options.MaxLength;
        MaskInput = options.MaskInput;
        MaskCharacter = options.MaskCharacter;
        if (!string.IsNullOrEmpty(options.InitialValue))
        {
            SetValue(options.InitialValue);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TextInputKeyMap KeyMap { get; set; } = TextInputKeyMap.Default;

    public KeyBinding CancelKey { get; set; } = new("esc", "cancel", "escape");

    public string Title { get; set; } = "Text Input";

    public bool IsFocused { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public bool ClearOnSubmit { get; set; }

    public bool ClearOnCancel { get; set; }

    public string LastSubmittedValue { get; private set; } = string.Empty;

    public string LastCancelledValue { get; private set; } = string.Empty;

    public int SubmitCount { get; private set; }

    public int CancelCount { get; private set; }

    public bool WasCancelled { get; private set; }

    /// <summary>
    /// Raised when the input submits a value.
    /// </summary>
    public event EventHandler<TextInputSubmittedEventArgs>? Submitted;

    /// <summary>
    /// Raised when the input cancels editing.
    /// </summary>
    public event EventHandler<TextInputCancelledEventArgs>? Cancelled;

    public string Value => Input.Value;

    public string Placeholder
    {
        get => Input.Placeholder;
        set => Input.Placeholder = value;
    }

    public int MaxLength
    {
        get => Input.MaxLength;
        set => Input.MaxLength = value;
    }

    public bool MaskInput
    {
        get => Input.MaskInput;
        set => Input.MaskInput = value;
    }

    public char MaskCharacter
    {
        get => Input.MaskCharacter;
        set => Input.MaskCharacter = value;
    }

    public void SetValue(string value)
    {
        Input.SetValue(value);
    }

    public void Clear()
    {
        Input.Clear();
    }

    /// <summary>
    /// Consumes the latest submitted text exactly once.
    /// </summary>
    public bool TryConsumeSubmit(out string value)
    {
        if (SubmitCount == _consumedSubmitCount)
        {
            value = string.Empty;
            return false;
        }

        _consumedSubmitCount = SubmitCount;
        value = LastSubmittedValue;
        return true;
    }

    /// <summary>
    /// Consumes the latest cancelled text exactly once.
    /// </summary>
    public bool TryConsumeCancel(out string value)
    {
        if (CancelCount == _consumedCancelCount)
        {
            value = string.Empty;
            return false;
        }

        _consumedCancelCount = CancelCount;
        WasCancelled = false;
        value = LastCancelledValue;
        return true;
    }

    public bool Update(IMessage message)
    {
        var result = TextInputInteractionHandler.Update(_input, message, KeyMap, CancelKey, ClearOnCancel, ClearOnSubmit);
        WasCancelled = result.WasCancelled;
        if (result.CancelCount > 0)
        {
            LastCancelledValue = result.LastCancelledValue;
            CancelCount += result.CancelCount;
            Cancelled?.Invoke(this, new TextInputCancelledEventArgs(LastCancelledValue));
        }

        if (result.SubmitCount > 0)
        {
            LastSubmittedValue = result.LastSubmittedValue;
            SubmitCount += result.SubmitCount;
            Submitted?.Invoke(this, new TextInputSubmittedEventArgs(LastSubmittedValue));
        }

        return result.Handled;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        TextInputRenderer.Render(canvas, rect, _input, Title, IsFocused, Border, Padding, SubmitCount);
    }

    private TextInputModel Input => _input;
}
