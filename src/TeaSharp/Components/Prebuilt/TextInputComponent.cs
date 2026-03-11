using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Components.Internal;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class TextInputComponent : IStatefulComponent, IFocusableComponent
{
    private readonly TextInputModel _input = new();

    public TextInputComponent()
    {
    }

    public TextInputComponent(TextInputOptions options)
    {
        Title = options.Title;
        Focused = options.Focused;
        ShowBorder = options.ShowBorder;
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

    public bool Focused { get; set; }

    public bool ShowBorder { get; set; } = true;

    public bool ClearOnSubmit { get; set; }

    public bool ClearOnCancel { get; set; }

    public string LastSubmittedValue { get; private set; } = string.Empty;

    public string LastCancelledValue { get; private set; } = string.Empty;

    public int SubmitCount { get; private set; }

    public int CancelCount { get; private set; }

    public bool WasCancelled { get; private set; }

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

    public bool Update(IMessage message)
    {
        var result = TextInputInteractionHandler.Update(_input, message, KeyMap, CancelKey, ClearOnCancel, ClearOnSubmit);
        WasCancelled = result.WasCancelled;
        if (result.CancelCount > 0)
        {
            LastCancelledValue = result.LastCancelledValue;
            CancelCount += result.CancelCount;
        }

        if (result.SubmitCount > 0)
        {
            LastSubmittedValue = result.LastSubmittedValue;
            SubmitCount += result.SubmitCount;
        }

        return result.Handled;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        TextInputRenderer.Render(canvas, rect, _input, Title, Focused, ShowBorder, SubmitCount);
    }

    private TextInputModel Input => _input;
}
