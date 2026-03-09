using System.Globalization;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class NumberInputComponent : IStatefulComponent, IFocusableComponent
{
    private bool _replaceOnNextCharacter = true;

    public TextInputModel Input { get; } = new();

    public TextInputKeyMap InputKeyMap { get; set; } = TextInputKeyMap.Default;

    public string Title { get; set; } = "Number Input";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public double Min { get; set; } = 0.0;

    public double Max { get; set; } = 100.0;

    public double Step { get; set; } = 1.0;

    public int Precision { get; set; } = 2;

    public double Value { get; private set; }

    public double? LastSubmittedValue { get; private set; }

    public KeyBinding IncreaseKey { get; set; } = new("up/+", "increase", "up", "+");

    public KeyBinding DecreaseKey { get; set; } = new("down/-", "decrease", "down", "-");

    public KeyBinding SubmitKey { get; set; } = new("enter", "submit", "enter");

    public WidgetStatePalette StatePalette { get; } = WidgetStatePalette.CreateDefault();

    public void SetValue(double value)
    {
        Value = Clamp(value);
        SyncInput();
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
                Value = Clamp(Value + Step);
                SyncInput();
                return !AreClose(before, Value);
            }

            if (DecreaseKey.Matches(key))
            {
                var before = Value;
                Value = Clamp(Value - Step);
                SyncInput();
                return !AreClose(before, Value);
            }

            if (SubmitKey.Matches(key))
            {
                if (TryParseInput(out var parsed))
                {
                    Value = Clamp(parsed);
                    LastSubmittedValue = Value;
                    SyncInput();
                }

                return true;
            }

            if (_replaceOnNextCharacter
                && key.Code == KeyCode.Character
                && key.Modifiers == KeyModifiers.None
                && key.Text.Length == 1)
            {
                Input.SetValue(string.Empty);
                _replaceOnNextCharacter = false;
            }
        }

        var result = Input.Update(message, InputKeyMap);
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
            Value = Clamp(submitted);
            LastSubmittedValue = Value;
            SyncInput();
            return true;
        }

        if (result.Changed && TryParseInput(out var edited))
        {
            Value = Clamp(edited);
        }

        return result.Changed || result.Submitted;
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

        var states = ResolveStates();
        var frame = Input.BuildFrame(content.Width);
        canvas.WriteText(content.X, content.Y, StatePalette.Render(frame.Text, states), content.Width);
        if (content.Height > 1)
        {
            canvas.WriteText(content.X, content.Y + 1, StatePalette.Render($"value={FormatValue(Value)} range=[{FormatValue(Min)}, {FormatValue(Max)}]", states), content.Width);
        }
    }

    private List<WidgetVisualState> ResolveStates()
    {
        var states = new List<WidgetVisualState>(4);
        if (Focused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (Disabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (ReadOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        return states;
    }

    private bool TryParseInput(out double value)
    {
        var text = Input.Value.Trim();
        if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
        {
            return true;
        }

        if (double.TryParse(text, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
        {
            return true;
        }

        text = text.Replace(',', '.');
        return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private void SyncInput()
    {
        Input.SetValue(FormatValue(Value));
        _replaceOnNextCharacter = true;
    }

    private string FormatValue(double value)
    {
        var precision = Math.Clamp(Precision, 0, 8);
        return value.ToString($"F{precision}", CultureInfo.InvariantCulture).TrimEnd('0').TrimEnd('.');
    }

    private double Clamp(double value)
    {
        if (Max <= Min)
        {
            return Min;
        }

        return Math.Clamp(value, Min, Max);
    }

    private static bool AreClose(double left, double right)
    {
        return Math.Abs(left - right) <= 0.000001;
    }
}

