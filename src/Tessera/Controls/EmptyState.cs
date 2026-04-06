using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

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
    public TesseraStyle DefaultStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets focused text style.
    /// </summary>
    public TesseraStyle FocusedStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets hovered text style.
    /// </summary>
    public TesseraStyle HoveredStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets disabled text style.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets action text style.
    /// </summary>
    public TesseraStyle ActionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets title style.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets body style.
    /// </summary>
    public TesseraStyle DescriptionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets hint style.
    /// </summary>
    public TesseraStyle HintStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets focused action style.
    /// </summary>
    public TesseraStyle FocusedActionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets hovered action style.
    /// </summary>
    public TesseraStyle HoveredActionStyle { get; set; } = TesseraStyle.Empty;

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
        if (message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var inside = bounds.Contains(pointer.X, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHovered(inside);
        }

        if (IsDisabled || !HasAction())
        {
            return false;
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && inside)
        {
            IsFocused = true;
            RaiseActivated();
            return true;
        }

        if (pointer.Kind == PointerEventKind.Release)
        {
            return SetHovered(inside);
        }

        return false;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var y = clipped.Y;
        if (TryWriteLine(canvas, clipped, ref y, RenderTitle(), ResolveTitleLineStyle(), includeActionStyles: false))
        {
            return;
        }

        if (TryRenderBody(canvas, clipped, ref y))
        {
            return;
        }

        if (TryWriteLine(canvas, clipped, ref y, Hint, ResolveHintLineStyle(), includeActionStyles: false))
        {
            return;
        }

        if (!HasAction())
        {
            return;
        }

        _ = TryWriteLine(
            canvas,
            clipped,
            ref y,
            RenderActionText(),
            ResolveActionLineStyle(),
            includeActionStyles: true);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 0;
        var height = 0;

        var title = RenderTitle();
        if (!string.IsNullOrWhiteSpace(title))
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(title));
            height++;
        }

        MeasureBody(ref width, ref height);

        if (!string.IsNullOrWhiteSpace(Hint))
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(Hint));
            height++;
        }

        if (HasAction())
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(RenderActionText()));
            height++;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private bool TryRenderBody(Canvas canvas, Rect clipped, ref int y)
    {
        if (string.IsNullOrEmpty(Description))
        {
            return false;
        }

        var body = Description.AsSpan();
        var lineStart = 0;
        while (lineStart <= body.Length)
        {
            if (y >= clipped.Bottom)
            {
                return true;
            }

            var lineEnd = lineStart;
            while (lineEnd < body.Length && body[lineEnd] is not '\n' and not '\r')
            {
                lineEnd++;
            }

            if (lineEnd > lineStart)
            {
                var line = SliceToString(Description, lineStart, lineEnd - lineStart);
                canvas.WriteText(
                    clipped.X,
                    y,
                    ApplyStylePipeline(line, ResolveBodyLineStyle(), includeActionStyles: false),
                    clipped.Width);
            }

            y++;
            if (lineEnd >= body.Length)
            {
                break;
            }

            if (body[lineEnd] == '\r' && lineEnd + 1 < body.Length && body[lineEnd + 1] == '\n')
            {
                lineStart = lineEnd + 2;
            }
            else
            {
                lineStart = lineEnd + 1;
            }
        }

        return false;
    }

    private void MeasureBody(ref int width, ref int height)
    {
        if (string.IsNullOrEmpty(Description))
        {
            return;
        }

        var body = Description.AsSpan();
        var lineStart = 0;
        while (lineStart <= body.Length)
        {
            var lineEnd = lineStart;
            while (lineEnd < body.Length && body[lineEnd] is not '\n' and not '\r')
            {
                lineEnd++;
            }

            if (lineEnd > lineStart)
            {
                width = Math.Max(width, MeasureSpanDisplayWidth(body[lineStart..lineEnd]));
            }

            height++;
            if (lineEnd >= body.Length)
            {
                break;
            }

            if (body[lineEnd] == '\r' && lineEnd + 1 < body.Length && body[lineEnd + 1] == '\n')
            {
                lineStart = lineEnd + 2;
            }
            else
            {
                lineStart = lineEnd + 1;
            }
        }
    }

    private static int MeasureSpanDisplayWidth(ReadOnlySpan<char> span)
    {
        if (span.IsEmpty)
        {
            return 0;
        }

        var ascii = true;
        for (var index = 0; index < span.Length; index++)
        {
            var value = span[index];
            if (value < '\u0020' || value > '\u007e')
            {
                ascii = false;
                break;
            }
        }

        if (ascii)
        {
            return span.Length;
        }

        return ControlTextLayout.MeasureDisplayWidth(span.ToString());
    }

    private static string SliceToString(string source, int start, int length)
    {
        if (length <= 0)
        {
            return string.Empty;
        }

        return start == 0 && length == source.Length
            ? source
            : source.Substring(start, length);
    }

    private bool TryWriteLine(
        Canvas canvas,
        Rect clipped,
        ref int y,
        string text,
        TesseraStyle localStyle,
        bool includeActionStyles)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        if (y >= clipped.Bottom)
        {
            return true;
        }

        canvas.WriteText(clipped.X, y, ApplyStylePipeline(text, localStyle, includeActionStyles), clipped.Width);
        y++;
        return y >= clipped.Bottom;
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

    private TesseraStyle ResolveTitleLineStyle()
    {
        return IsFocused && !FocusedTitleStyle.IsEmpty
            ? FocusedTitleStyle
            : TitleStyle;
    }

    private TesseraStyle ResolveBodyLineStyle() => DescriptionStyle;

    private TesseraStyle ResolveHintLineStyle() => HintStyle;

    private TesseraStyle ResolveActionLineStyle()
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

        return style;
    }

    private string ApplyStylePipeline(string text, TesseraStyle localStyle, bool includeActionStyles)
    {
        var styled = ApplyStyle(text, DefaultStyle);
        if (IsFocused)
        {
            styled = ApplyStyle(styled, FocusedStyle);
        }

        if (_hovered)
        {
            styled = ApplyStyle(styled, HoveredStyle);
        }

        styled = ApplyStyle(styled, localStyle);
        if (includeActionStyles)
        {
            styled = ApplyStyle(styled, ActionStyle);
        }

        if (IsDisabled)
        {
            styled = ApplyStyle(styled, DisabledStyle);
        }

        return styled;
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

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }
}
