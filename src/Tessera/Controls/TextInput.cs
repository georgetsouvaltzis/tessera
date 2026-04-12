using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;
using Tessera.Widgets;
using System.ComponentModel;

namespace Tessera.Controls;

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

    /// <summary>
    /// Executes text input.
    /// </summary>
    /// <returns>The result of text input.</returns>
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
    /// Gets or sets the marker shown in the title when the control is focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker should be rendered in the title when focused.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets the title style applied when the control is not focused.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

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
    /// Gets or sets the style used for the input value text.
    /// </summary>
    public TesseraStyle ValueTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style used when the placeholder text is shown.
    /// </summary>
    public TesseraStyle PlaceholderTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to the title when the input is focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

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

    /// <inheritdoc />
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

    /// <summary>
    /// Gets or sets the last submitted value.
    /// </summary>
    public string LastSubmittedValue { get; private set; } = string.Empty;

    /// <summary>
    /// Gets or sets the last cancelled value.
    /// </summary>
    public string LastCancelledValue { get; private set; } = string.Empty;

    /// <summary>
    /// Represents submit count.
    /// </summary>
    public int SubmitCount => _submitCount;

    /// <summary>
    /// Represents cancel count.
    /// </summary>
    public int CancelCount => _cancelCount;

    /// <inheritdoc />
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

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None
            ? null
            : FormatTitle();
        if (title is not null)
        {
            var titleStyle = IsFocused ? FocusedTitleStyle : TitleStyle;
            if (!titleStyle.IsEmpty)
            {
                title = titleStyle.Render(title);
            }
        }

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        var frame = _input.BuildFrame(content.Width);
        var textStyle = frame.PlaceholderVisible ? PlaceholderTextStyle : ValueTextStyle;
        var text = textStyle.IsEmpty ? frame.Text : textStyle.Render(frame.Text);
        canvas.WriteText(content.X, content.Y, text, content.Width);
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

    private string FormatTitle()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled)
        {
            style = style.Merge(PlaceholderTextStyle);
        }

        return style;
    }
}
