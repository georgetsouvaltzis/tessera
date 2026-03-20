using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a centered empty-state surface with optional primary action.
/// </summary>
public sealed class EmptyState : Control
{
    private bool _isActionHovered;

    /// <summary>
    /// Occurs when the action is activated.
    /// </summary>
    public event EventHandler? Activated;

    /// <summary>
    /// Gets or sets the primary title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Nothing here yet";

    /// <summary>
    /// Gets or sets the body message.
    /// </summary>
    public string Description
    {
        get;
        set => field = value ?? string.Empty;
    } = "There is no data to display.";

    /// <summary>
    /// Gets or sets the optional hint text.
    /// </summary>
    public string Hint
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets the action text.
    /// </summary>
    public string ActionText
    {
        get;
        set => field = value ?? string.Empty;
    } = "Retry";

    /// <summary>
    /// Gets or sets a value indicating whether the primary action should be rendered.
    /// </summary>
    public bool ShowAction { get; set; } = true;

    /// <summary>
    /// Gets or sets the marker shown in the title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether the focus marker should be rendered in the title.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets the title style when not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the title style when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the body style.
    /// </summary>
    public TeaStyle DescriptionStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the hint style.
    /// </summary>
    public TeaStyle HintStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the action style.
    /// </summary>
    public TeaStyle ActionStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the action style while focused.
    /// </summary>
    public TeaStyle FocusedActionStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the action style while hovered.
    /// </summary>
    public TeaStyle HoveredActionStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged while disabled.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    internal override bool CanFocus => ShowAction && !IsDisabled;

    public override bool Handle(Message message)
    {
        if (IsDisabled || !ShowAction || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            Activated?.Invoke(this, EventArgs.Empty);
            return true;
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer || !ShowAction || IsDisabled)
        {
            return Handle(message);
        }

        var actionRect = ResolveActionRect(bounds);
        if (actionRect.IsEmpty)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            return Handle(message);
        }

        var contains = actionRect.Contains(pointer.X, pointer.Y);
        if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            var hoverChanged = _isActionHovered != contains;
            _isActionHovered = contains;

            if (pointer.Kind == PointerEventKind.Press && contains && pointer.Button == PointerButton.Left)
            {
                IsFocused = true;
                Activated?.Invoke(this, EventArgs.Empty);
                return true;
            }

            return hoverChanged;
        }

        return false;
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var lines = BuildLines();
        if (lines.Count == 0)
        {
            return;
        }

        var startY = clipped.Y + Math.Max(0, (clipped.Height - lines.Count) / 2);
        for (var index = 0; index < lines.Count; index++)
        {
            var line = lines[index];
            var width = ControlTextLayout.MeasureDisplayWidth(line.Text);
            var x = clipped.X + Math.Max(0, (clipped.Width - width) / 2);
            canvas.WriteText(x, startY + index, ApplyStyle(line.Text, line.Style), clipped.Right - x);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var lines = BuildLines();
        var width = lines.Count == 0 ? 0 : lines.Max(static line => ControlTextLayout.MeasureDisplayWidth(line.Text));
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(lines.Count, 0, availableBounds.Height));
    }

    private List<(string Text, TeaStyle Style)> BuildLines()
    {
        var lines = new List<(string Text, TeaStyle Style)>(4)
        {
            (RenderTitle(), ResolveTitleStyle()),
        };

        if (!string.IsNullOrWhiteSpace(Description))
        {
            lines.Add((Description, ResolveDescriptionStyle()));
        }

        if (!string.IsNullOrWhiteSpace(Hint))
        {
            lines.Add((Hint, ResolveHintStyle()));
        }

        if (ShowAction && !string.IsNullOrWhiteSpace(ActionText))
        {
            lines.Add((RenderActionText(), ResolveActionStyle()));
        }

        return lines;
    }

    private string RenderTitle()
    {
        if (!IsFocused || !ShowFocusMarker || string.IsNullOrEmpty(FocusMarker))
        {
            return Title;
        }

        return $"{Title} {FocusMarker}";
    }

    private string RenderActionText()
    {
        return $"[{ActionText}]";
    }

    private TeaStyle ResolveTitleStyle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return MergeDisabledStyle(style);
    }

    private TeaStyle ResolveDescriptionStyle()
    {
        return MergeDisabledStyle(DescriptionStyle);
    }

    private TeaStyle ResolveHintStyle()
    {
        return MergeDisabledStyle(HintStyle);
    }

    private TeaStyle ResolveActionStyle()
    {
        var style = ActionStyle;
        if (IsFocused && !FocusedActionStyle.IsEmpty)
        {
            style = style.IsEmpty ? FocusedActionStyle : style.Merge(FocusedActionStyle);
        }

        if (_isActionHovered && !HoveredActionStyle.IsEmpty)
        {
            style = style.IsEmpty ? HoveredActionStyle : style.Merge(HoveredActionStyle);
        }

        return MergeDisabledStyle(style);
    }

    private TeaStyle MergeDisabledStyle(TeaStyle style)
    {
        if (!IsDisabled || DisabledStyle.IsEmpty)
        {
            return style;
        }

        return style.IsEmpty ? DisabledStyle : style.Merge(DisabledStyle);
    }

    private Rect ResolveActionRect(Rect bounds)
    {
        var clipped = Rect.Intersect(bounds, bounds);
        if (clipped.IsEmpty || !ShowAction || string.IsNullOrWhiteSpace(ActionText))
        {
            return new Rect(0, 0, 0, 0);
        }

        var lines = BuildLines();
        var actionLineIndex = lines.Count - 1;
        var startY = clipped.Y + Math.Max(0, (clipped.Height - lines.Count) / 2);
        var y = startY + actionLineIndex;
        if (y < clipped.Y || y >= clipped.Bottom)
        {
            return new Rect(0, 0, 0, 0);
        }

        var actionText = RenderActionText();
        var width = ControlTextLayout.MeasureDisplayWidth(actionText);
        var x = clipped.X + Math.Max(0, (clipped.Width - width) / 2);
        return new Rect(x, y, Math.Min(width, clipped.Right - x), 1);
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
