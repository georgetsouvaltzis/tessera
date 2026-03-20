using TeaSharp.Components.Primitives;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a centered empty-data surface with optional action affordance.
/// </summary>
public sealed class EmptyState : Control
{
    private bool _hovered;

    /// <summary>
    /// Occurs when the action is activated.
    /// </summary>
    public event EventHandler? Activated;

    /// <summary>
    /// Occurs when the action is activated.
    /// </summary>
    public event EventHandler? ActionInvoked;

    /// <summary>
    /// Gets or sets the title text.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Nothing here yet";

    /// <summary>
    /// Gets or sets the body text.
    /// </summary>
    public string Description
    {
        get;
        set => field = value ?? string.Empty;
    } = "There is no data to display.";

    /// <summary>
    /// Gets or sets the body text.
    /// </summary>
    public string Body
    {
        get => Description;
        set => Description = value;
    }

    /// <summary>
    /// Gets or sets optional hint text.
    /// </summary>
    public string Hint
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

    /// <summary>
    /// Gets or sets action label text.
    /// </summary>
    public string ActionText
    {
        get;
        set => field = value ?? string.Empty;
    } = "Retry";

    /// <summary>
    /// Gets or sets action label text.
    /// </summary>
    public string ActionLabel
    {
        get => ActionText;
        set => ActionText = value;
    }

    /// <summary>
    /// Gets or sets whether the action is visible and interactive.
    /// </summary>
    public bool ShowAction { get; set; } = true;

    /// <summary>
    /// Gets or sets focus marker text.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether focus marker should render while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets default text style.
    /// </summary>
    public TeaStyle DefaultStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets focused text style.
    /// </summary>
    public TeaStyle FocusedStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets hovered text style.
    /// </summary>
    public TeaStyle HoveredStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets disabled text style.
    /// </summary>
    public TeaStyle DisabledStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets action text style.
    /// </summary>
    public TeaStyle ActionStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets focused title style.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets body style.
    /// </summary>
    public TeaStyle DescriptionStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets hint style.
    /// </summary>
    public TeaStyle HintStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets focused action style.
    /// </summary>
    public TeaStyle FocusedActionStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets hovered action style.
    /// </summary>
    public TeaStyle HoveredActionStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets whether pointer is inside control bounds.
    /// </summary>
    public bool IsHovered => _hovered;

    internal override bool CanFocus => ShowAction && !IsDisabled;

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || !HasAction() || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (!key.Is(Key.Enter) && !key.IsCharacter(' '))
        {
            return false;
        }

        RaiseActivated();
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer || IsDisabled || !HasAction())
        {
            return Handle(message);
        }

        if (bounds.IsEmpty || pointer.Kind == PointerEventKind.Wheel)
        {
            return Handle(message);
        }

        var inside = bounds.Contains(pointer.X, pointer.Y);
        var changed = pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press or PointerEventKind.Release
            ? SetHovered(inside)
            : false;
        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && inside)
        {
            IsFocused = true;
            RaiseActivated();
            return true;
        }

        return changed || Handle(message);
    }

    /// <inheritdoc />
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
        var lines = new List<(string Text, TeaStyle Style)>
        {
            (RenderTitle(), ResolveTitleStyle()),
        };

        if (!string.IsNullOrWhiteSpace(Description))
        {
            lines.Add((Description, MergeStateStyle(DescriptionStyle)));
        }

        if (!string.IsNullOrWhiteSpace(Hint))
        {
            lines.Add((Hint, MergeStateStyle(HintStyle)));
        }

        if (HasAction())
        {
            lines.Add((RenderActionText(), ResolveActionStyle()));
        }

        return lines;
    }

    private bool HasAction() => ShowAction && !string.IsNullOrWhiteSpace(ActionText);

    private string RenderTitle()
    {
        if (!IsFocused || !ShowFocusMarker || string.IsNullOrEmpty(FocusMarker))
        {
            return Title;
        }

        return $"{Title} {FocusMarker}";
    }

    private string RenderActionText() => $"[{ActionText}]";

    private TeaStyle ResolveTitleStyle()
    {
        var style = IsFocused && !FocusedTitleStyle.IsEmpty ? FocusedTitleStyle : TitleStyle;
        return MergeStateStyle(style);
    }

    private TeaStyle ResolveActionStyle()
    {
        var style = ActionStyle;
        if (IsFocused && !FocusedActionStyle.IsEmpty)
        {
            style = style.IsEmpty ? FocusedActionStyle : style.Merge(FocusedActionStyle);
        }

        if (_hovered && !HoveredActionStyle.IsEmpty)
        {
            style = style.IsEmpty ? HoveredActionStyle : style.Merge(HoveredActionStyle);
        }

        return MergeStateStyle(style);
    }

    private TeaStyle MergeStateStyle(TeaStyle localStyle)
    {
        var style = DefaultStyle;
        if (IsFocused)
        {
            style = style.Merge(FocusedStyle);
        }

        if (_hovered)
        {
            style = style.Merge(HoveredStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStyle);
        }

        if (localStyle.IsEmpty)
        {
            return style;
        }

        return style.IsEmpty ? localStyle : style.Merge(localStyle);
    }

    private bool SetHovered(bool hovered)
    {
        if (_hovered == hovered)
        {
            return false;
        }

        _hovered = hovered;
        return true;
    }

    private void RaiseActivated()
    {
        Activated?.Invoke(this, EventArgs.Empty);
        ActionInvoked?.Invoke(this, EventArgs.Empty);
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
