using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Widgets;
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
    private readonly TextInputModel _input = new();
    private int _submitCount;
    private int _cancelCount;
    private int _consumedSubmitCount;
    private int _consumedCancelCount;

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
    }

    /// <summary>
    /// Gets or sets the field title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Text Input";

    /// <summary>
    /// Gets or sets the placeholder shown when the field is empty.
    /// </summary>
    public string Placeholder
    {
        get => _input.Placeholder;
        set => _input.Placeholder = value ?? string.Empty;
    }

    /// <summary>
    /// Gets the current input value.
    /// </summary>
    public string Value => _input.Value;

    /// <summary>
    /// Gets or sets the field border style.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets the inner padding applied to the field body.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field should clear after submission.
    /// </summary>
    public bool ClearOnSubmit
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field should clear after cancellation.
    /// </summary>
    public bool ClearOnCancel
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the maximum accepted input length.
    /// </summary>
    public int MaxLength
    {
        get => _input.MaxLength;
        set => _input.MaxLength = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the field masks typed characters.
    /// </summary>
    public bool MaskInput
    {
        get => _input.MaskInput;
        set => _input.MaskInput = value;
    }

    /// <summary>
    /// Gets or sets the masking character used when <see cref="MaskInput"/> is enabled.
    /// </summary>
    public char MaskCharacter
    {
        get => _input.MaskCharacter;
        set => _input.MaskCharacter = value;
    }

    public override bool IsFocused
    {
        get;
        set;
    }

    /// <summary>
    /// Replaces the current field value.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public void SetValue(string value) => _input.SetValue(value ?? string.Empty);

    /// <summary>
    /// Clears the current field value.
    /// </summary>
    public void Clear() => _input.Clear();

    /// <summary>
    /// Attempts to consume a pending submission.
    /// </summary>
    /// <param name="value">Receives the submitted value when available.</param>
    /// <returns><see langword="true"/> when a submission was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeSubmission(out string value)
    {
        if (_submitCount == _consumedSubmitCount)
        {
            value = string.Empty;
            return false;
        }

        _consumedSubmitCount = _submitCount;
        value = LastSubmittedValue;
        return true;
    }

    /// <summary>
    /// Attempts to consume a pending cancellation.
    /// </summary>
    /// <param name="value">Receives the canceled value when available.</param>
    /// <returns><see langword="true"/> when a cancellation was consumed; otherwise, <see langword="false"/>.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeCancellation(out string value)
    {
        if (_cancelCount == _consumedCancelCount)
        {
            value = string.Empty;
            return false;
        }

        _consumedCancelCount = _cancelCount;
        value = LastCancelledValue;
        return true;
    }

    public string LastSubmittedValue { get; private set; } = string.Empty;

    public string LastCancelledValue { get; private set; } = string.Empty;

    public int SubmitCount => _submitCount;

    public int CancelCount => _cancelCount;

    public override bool Handle(Message message)
    {
        if (IsDisabled || !IsFocused)
        {
            return false;
        }

        if (message is KeyPressed { Key: Key.Escape })
        {
            LastCancelledValue = _input.Value;
            _cancelCount++;
            if (ClearOnCancel)
            {
                _input.Clear();
            }

            Cancelled?.Invoke(this, new TextInputCancelledEventArgs(LastCancelledValue));
            return true;
        }

        var result = _input.Update(message);
        if (!result.Submitted)
        {
            return result.Changed;
        }

        LastSubmittedValue = _input.Value;
        _submitCount++;
        if (ClearOnSubmit)
        {
            _input.Clear();
        }

        Submitted?.Invoke(this, new TextInputSubmittedEventArgs(LastSubmittedValue));
        return true;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : IsFocused ? $"{Title} *" : Title,
            Border,
            Padding);
        if (content.IsEmpty)
        {
            return;
        }

        var frame = _input.BuildFrame(content.Width);
        canvas.WriteText(content.X, content.Y, frame.Text, content.Width);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var frame = _input.BuildFrame(Math.Max(1, availableBounds.Width));
        var width = ControlTextLayout.MeasureDisplayWidth(frame.Text) + Padding.Horizontal;
        var height = Padding.Vertical + 1;
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
}
