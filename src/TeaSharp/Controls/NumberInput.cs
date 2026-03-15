using System.ComponentModel;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity.Internal;
using TeaSharp.Components.Styling;
using TeaSharp.Internal;
using TeaSharp.Layout;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a numeric text input with parsed submission events.
/// </summary>
public sealed class NumberInput : Control
{
    private readonly TextInputModel _input = new();
    private readonly WidgetStatePalette _statePalette = WidgetStatePalette.CreateDefault();
    private bool _replaceOnNextCharacter = true;
    private long _submitVersion;
    private long _consumedSubmitVersion;

    public event EventHandler<NumberInputSubmittedEventArgs>? Submitted;

    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Number Input";

    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    public Thickness Padding
    {
        get;
        set;
    }

    public double Min
    {
        get;
        set;
    }

    public double Max
    {
        get;
        set;
    } = 100.0;

    public double Step
    {
        get;
        set;
    } = 1.0;

    public int Precision
    {
        get;
        set;
    } = 2;

    public double Value { get; private set; }

    public string Text => _input.Value;

    public double? LastSubmittedValue { get; private set; }

    public override bool IsFocused
    {
        get;
        set;
    }

    public override bool IsDisabled
    {
        get;
        set;
    }

    public override bool IsReadOnly
    {
        get;
        set;
    }

    public void SetValue(double value)
    {
        Value = NumberInputFormatting.Clamp(value, Min, Max);
        SyncInput();
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeSubmission(out double value)
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

    public override bool Handle(Message message)
    {
        if (!IsFocused || IsDisabled || IsReadOnly)
        {
            return false;
        }

        if (message is KeyPressed key)
        {
            if (key.Is(Key.Up) || key.IsCharacter('+'))
            {
                var before = Value;
                Value = NumberInputFormatting.Clamp(Value + Step, Min, Max);
                SyncInput();
                return !NumberInputFormatting.AreClose(before, Value);
            }

            if (key.Is(Key.Down) || key.IsCharacter('-'))
            {
                var before = Value;
                Value = NumberInputFormatting.Clamp(Value - Step, Min, Max);
                SyncInput();
                return !NumberInputFormatting.AreClose(before, Value);
            }

            if (key.Is(Key.Enter))
            {
                if (TryParseInput(out var parsed))
                {
                    SubmitValue(parsed);
                }

                return true;
            }

            if (_replaceOnNextCharacter
                && key.Key == Key.Character
                && key.Modifiers == ModifierKeys.None
                && key.Text.Length == 1)
            {
                _input.SetValue(string.Empty);
                _replaceOnNextCharacter = false;
            }
        }

        var result = _input.Update(TeaMessageAdapter.ToCore(message));
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

    public override void Render(Canvas canvas, Rect rect)
    {
        NumberInputRenderer.Render(canvas, rect, _input, _statePalette, Title, IsFocused, IsDisabled, IsReadOnly, Border, Padding, Value, Min, Max, Precision);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var frame = _input.BuildFrame(Math.Max(1, availableBounds.Width));
        var valueText = NumberInputFormatting.Format(Value, Precision);
        var minText = NumberInputFormatting.Format(Min, Precision);
        var maxText = NumberInputFormatting.Format(Max, Precision);
        var summary = $"value={valueText} range=[{minText}, {maxText}]";
        var width = Math.Max(frame.Text.Length, summary.Length) + Padding.Horizontal;
        var height = Padding.Vertical + 2;
        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
            width = Math.Max(width, Title.Length + 4);
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
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
