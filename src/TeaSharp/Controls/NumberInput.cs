using System.ComponentModel;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;

namespace TeaSharp.Controls;

public sealed class NumberInput : Control
{
    private readonly NumberInputComponent _component = new();

    public event EventHandler<NumberInputSubmittedEventArgs>? Submitted;

    public NumberInput()
    {
        _component.Submitted += (_, args) => Submitted?.Invoke(this, new NumberInputSubmittedEventArgs(args.Value));
    }

    public string Title
    {
        get => _component.Title;
        set => _component.Title = value ?? string.Empty;
    }

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

    public double Min
    {
        get => _component.Min;
        set => _component.Min = value;
    }

    public double Max
    {
        get => _component.Max;
        set => _component.Max = value;
    }

    public double Step
    {
        get => _component.Step;
        set => _component.Step = value;
    }

    public int Precision
    {
        get => _component.Precision;
        set => _component.Precision = value;
    }

    public double Value => _component.Value;

    public string Text => _component.Text;

    public double? LastSubmittedValue => _component.LastSubmittedValue;

    public override bool IsFocused
    {
        get => _component.IsFocused;
        set => _component.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => _component.IsDisabled;
        set => _component.IsDisabled = value;
    }

    public override bool IsReadOnly
    {
        get => _component.IsReadOnly;
        set => _component.IsReadOnly = value;
    }

    public void SetValue(double value) => _component.SetValue(value);

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeSubmission(out double value) => _component.TryConsumeSubmit(out value);

    public override bool Handle(Message message) => ControlForwarder.Forward(_component, message);

    public override void Render(Canvas canvas, Rect rect) => _component.Render(canvas, rect);
}
