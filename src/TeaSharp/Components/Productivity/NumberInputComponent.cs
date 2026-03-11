using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Productivity;

/// <summary>
/// Provides structured numeric entry on top of a text-backed editing surface.
/// </summary>
public sealed class NumberInputComponent : IStatefulComponent, IFocusableComponent
{
    private bool _replaceOnNextCharacter = true;
    private readonly TextInputModel _input = new();
    private long _submitVersion;
    private long _consumedSubmitVersion;

    public NumberInputComponent()
    {
    }

    public NumberInputComponent(NumberInputOptions options)
    {
        Title = options.Title;
        Focused = options.Focused;
        Disabled = options.Disabled;
        ReadOnly = options.ReadOnly;
        Border = options.Border;
        Padding = options.Padding;
        Min = options.Min;
        Max = options.Max;
        Step = options.Step;
        Precision = options.Precision;
        InputKeyMap = options.InputKeyMap ?? TextInputKeyMap.Default;
        IncreaseKey = options.IncreaseKey ?? IncreaseKey;
        DecreaseKey = options.DecreaseKey ?? DecreaseKey;
        SubmitKey = options.SubmitKey ?? SubmitKey;
        SetValue(options.InitialValue);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TextInputKeyMap InputKeyMap { get; set; } = TextInputKeyMap.Default;

    public string Title { get; set; } = "Number Input";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    public Thickness Padding { get; set; }

    public double Min { get; set; }

    public double Max { get; set; } = 100.0;

    public double Step { get; set; } = 1.0;

    public int Precision { get; set; } = 2;

    public double Value { get; private set; }

    public string Text => _input.Value;

    public double? LastSubmittedValue { get; private set; }

    /// <summary>
    /// Raised when the input submits a numeric value.
    /// </summary>
    public event EventHandler<NumberInputSubmittedEventArgs>? Submitted;

    public KeyBinding IncreaseKey { get; set; } = new("up/+", "increase", "up", "+");

    public KeyBinding DecreaseKey { get; set; } = new("down/-", "decrease", "down", "-");

    public KeyBinding SubmitKey { get; set; } = new("enter", "submit", "enter");

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public void SetValue(double value)
    {
        Value = NumberInputFormatting.Clamp(value, Min, Max);
        SyncInput();
    }

    /// <summary>
    /// Consumes the latest submitted numeric value exactly once.
    /// </summary>
    public bool TryConsumeSubmit(out double value)
    {
        if (_submitVersion == _consumedSubmitVersion || LastSubmittedValue is not double submitted)
        {
            value = default;
            return false;
        }

        _consumedSubmitVersion = _submitVersion;
        value = submitted;
        return true;
    }

    public bool Update(IMessage message)
    {
        if (!Focused || Disabled || ReadOnly)
        {
            return false;
        }

        if (message is KeyPressMsg key)
        {
            if (IncreaseKey.Matches(key))
            {
                var before = Value;
                Value = NumberInputFormatting.Clamp(Value + Step, Min, Max);
                SyncInput();
                return !NumberInputFormatting.AreClose(before, Value);
            }

            if (DecreaseKey.Matches(key))
            {
                var before = Value;
                Value = NumberInputFormatting.Clamp(Value - Step, Min, Max);
                SyncInput();
                return !NumberInputFormatting.AreClose(before, Value);
            }

            if (SubmitKey.Matches(key))
            {
                if (TryParseInput(out var parsed))
                {
                    SubmitValue(parsed);
                }

                return true;
            }

            if (_replaceOnNextCharacter
                && key.Code == KeyCode.Character
                && key.Modifiers == KeyModifiers.None
                && key.Text.Length == 1)
            {
                _input.SetValue(string.Empty);
                _replaceOnNextCharacter = false;
            }
        }

        var result = _input.Update(message, InputKeyMap);
        if (result.Changed)
        {
            _replaceOnNextCharacter = false;
        }

        if (!result.Changed && !result.Submitted)
        {
            return false;
        }

        if (result.Submitted && TryParseInput(out var submitted))
        {
            SubmitValue(submitted);
            return true;
        }

        if (result.Changed && TryParseInput(out var edited))
        {
            Value = NumberInputFormatting.Clamp(edited, Min, Max);
        }

        return result.Changed || result.Submitted;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        NumberInputRenderer.Render(canvas, rect, _input, StatePalette, Title, Focused, Disabled, ReadOnly, Border, Padding, Value, Min, Max, Precision);
    }

    private bool TryParseInput(out double value)
    {
        return NumberInputFormatting.TryParse(_input.Value, out value);
    }

    private void SyncInput()
    {
        _input.SetValue(NumberInputFormatting.Format(Value, Precision));
        _replaceOnNextCharacter = true;
    }

    private void SubmitValue(double parsed)
    {
        Value = NumberInputFormatting.Clamp(parsed, Min, Max);
        LastSubmittedValue = Value;
        _submitVersion++;
        Submitted?.Invoke(this, new NumberInputSubmittedEventArgs(Value));
        SyncInput();
    }
}
