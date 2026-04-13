using System.ComponentModel;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Components.Styling;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;
using Tessera.Widgets;

namespace Tessera.Controls;

/// <summary>
///     Represents a numeric text input with parsed submission events.
/// </summary>
public sealed class NumberInput : Control
{
    private readonly TextInputModel _input = new();
    private readonly WidgetStatePalette _statePalette = WidgetStatePalette.CreateDefault();
    private long _consumedSubmitVersion;
    private bool _replaceOnNextCharacter = true;
    private long _submitVersion;

    /// <summary>
    ///     Represents title.
    /// </summary>
    public string Title { get; set; } = "Number Input";

    /// <summary>
    ///     Represents focus marker.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Represents show focus marker.
    /// </summary>
    public bool ShowFocusMarker
    {
        get;
        set;
    } = true;

    /// <summary>
    ///     Represents title style.
    /// </summary>
    public TesseraStyle TitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents value text style.
    /// </summary>
    public TesseraStyle ValueTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents summary text style.
    /// </summary>
    public TesseraStyle SummaryTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents disabled text style.
    /// </summary>
    public TesseraStyle DisabledTextStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is not focused.
    /// </summary>
    public TesseraStyle BorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the style applied to border glyphs when the control is focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    ///     Represents border.
    /// </summary>
    public BorderStyle Border
    {
        get;
        set;
    } = BorderStyle.SingleLine;

    /// <summary>
    ///     Represents padding.
    /// </summary>
    public Thickness Padding
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents min.
    /// </summary>
    public double Min
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents max.
    /// </summary>
    public double Max
    {
        get;
        set;
    } = 100.0;

    /// <summary>
    ///     Represents step.
    /// </summary>
    public double Step
    {
        get;
        set;
    } = 1.0;

    /// <summary>
    ///     Represents precision.
    /// </summary>
    public int Precision
    {
        get;
        set;
    } = 2;

    /// <summary>
    ///     Gets or sets the value.
    /// </summary>
    public double Value { get; private set; }

    /// <summary>
    ///     Represents text.
    /// </summary>
    public string Text => _input.Value;

    /// <summary>
    ///     Gets or sets the last submitted value.
    /// </summary>
    public double? LastSubmittedValue { get; private set; }

    /// <inheritdoc />
    public override bool IsFocused
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsDisabled
    {
        get;
        set;
    }

    /// <inheritdoc />
    public override bool IsReadOnly
    {
        get;
        set;
    }

    /// <summary>
    ///     Represents submitted.
    /// </summary>
    public event EventHandler<NumberInputSubmittedEventArgs>? Submitted;

    /// <summary>
    ///     Executes set value.
    /// </summary>
    /// <param name="value">The value value.</param>
    public void SetValue(double value)
    {
        Value = NumberInputFormatting.Clamp(value, Min, Max);
        SyncInput();
    }

    /// <summary>
    ///     Executes try consume submission.
    /// </summary>
    /// <param name="value">The value value.</param>
    /// <returns><see langword="true" /> when try consume submission succeeds.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public bool TryConsumeSubmission(out double value)
    {
        if (_submitVersion == _consumedSubmitVersion || LastSubmittedValue is not { } submitted)
        {
            value = default;
            return false;
        }

        _consumedSubmitVersion = _submitVersion;
        value = submitted;
        return true;
    }

    /// <inheritdoc />
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

        var result = _input.Update(message);
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

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : FormatTitle();
        if (!string.IsNullOrEmpty(title))
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

        var states = NumberInputStateResolver.Resolve(IsFocused, IsDisabled, IsReadOnly);
        var frame = _input.BuildFrame(content.Width);
        var valueText = _statePalette.Render(frame.Text, states);
        var valueStyle = ResolveValueStyle();
        if (!valueStyle.IsEmpty)
        {
            valueText = valueStyle.Render(valueText);
        }

        canvas.WriteText(content.X, content.Y, valueText, content.Width);
        if (content.Height > 1)
        {
            var summary =
                $"value={NumberInputFormatting.Format(Value, Precision)} range=[{NumberInputFormatting.Format(Min, Precision)}, {NumberInputFormatting.Format(Max, Precision)}]";
            summary = _statePalette.Render(summary, states);
            var summaryStyle = ResolveSummaryStyle();
            if (!summaryStyle.IsEmpty)
            {
                summary = summaryStyle.Render(summary);
            }

            canvas.WriteText(content.X, content.Y + 1, summary, content.Width);
        }
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
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
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

    private string FormatTitle()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string FormatTitleForMeasure()
    {
        if (ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private TesseraStyle ResolveValueStyle()
    {
        if ((IsDisabled || IsReadOnly) && !DisabledTextStyle.IsEmpty)
        {
            return ValueTextStyle.IsEmpty
                ? DisabledTextStyle
                : ValueTextStyle.Merge(DisabledTextStyle);
        }

        return ValueTextStyle;
    }

    private TesseraStyle ResolveSummaryStyle()
    {
        if ((IsDisabled || IsReadOnly) && !DisabledTextStyle.IsEmpty)
        {
            return SummaryTextStyle.IsEmpty
                ? DisabledTextStyle
                : SummaryTextStyle.Merge(DisabledTextStyle);
        }

        return SummaryTextStyle;
    }

    private TesseraStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }

        if (IsDisabled || IsReadOnly)
        {
            style = style.Merge(DisabledTextStyle);
        }

        return style;
    }
}
