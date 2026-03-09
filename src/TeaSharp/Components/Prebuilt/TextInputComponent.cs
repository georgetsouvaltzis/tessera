using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class TextInputComponent : IStatefulComponent, IFocusableComponent
{
    public TextInputModel Input { get; } = new();

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
        WasCancelled = false;
        if (message is KeyPressMsg key && CancelKey.Matches(key))
        {
            WasCancelled = true;
            LastCancelledValue = Input.Value;
            CancelCount++;
            if (ClearOnCancel)
            {
                Input.Clear();
            }

            return true;
        }

        var result = Input.Update(message, KeyMap);
        if (!result.Submitted)
        {
            return result.Changed;
        }

        LastSubmittedValue = Input.Value;
        SubmitCount++;
        if (ClearOnSubmit)
        {
            Input.Clear();
        }

        return true;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        Rect content;
        if (ShowBorder)
        {
            canvas.DrawBox(clipped, Focused ? $"{Title} *" : Title);
            content = clipped.Inset(1, 1);
        }
        else
        {
            content = clipped;
        }

        if (content.IsEmpty)
        {
            return;
        }

        var frame = Input.BuildFrame(content.Width);
        canvas.WriteText(content.X, content.Y, frame.Text, content.Width);
        if (content.Height > 1)
        {
            var submitted = SubmitCount == 0
                ? "submit: -"
                : $"submit: {SubmitCount}";
            canvas.WriteText(content.X, content.Y + 1, submitted, content.Width);
        }
    }
}
