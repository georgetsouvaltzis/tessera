using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a compact one-line paginator.
/// </summary>
public sealed class Paginator : Control
{
    private const string PreviousLabel = "Prev";
    private const string NextLabel = "Next";

    private int _pageIndex;
    private int _pageCount = 1;

    /// <summary>
    /// Occurs when <see cref="PageIndex"/> changes.
    /// </summary>
    public event EventHandler<PageChangedEventArgs>? PageChanged;

    /// <summary>
    /// Gets or sets the optional title shown before the pager labels.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = string.Empty;

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
    /// Gets or sets the title style applied when the control is focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the base style used for pager labels.
    /// </summary>
    public TesseraStyle LabelStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into the page label.
    /// </summary>
    public TesseraStyle ActivePageLabelStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the style merged into disabled navigation labels.
    /// </summary>
    public TesseraStyle DisabledNavigationLabelStyle
    {
        get;
        set;
    } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets the current page index (zero-based).
    /// </summary>
    public int PageIndex
    {
        get => _pageIndex;
        set
        {
            var clamped = Math.Clamp(value, 0, _pageCount - 1);
            if (clamped == _pageIndex)
            {
                return;
            }

            var previous = _pageIndex;
            _pageIndex = clamped;
            PageChanged?.Invoke(this, new PageChangedEventArgs(previous, _pageIndex));
        }
    }

    /// <summary>
    /// Gets or sets the total page count.
    /// </summary>
    /// <remarks>
    /// Values less than one are normalized to <c>1</c>.
    /// </remarks>
    public int PageCount
    {
        get => _pageCount;
        set => SetPageCount(value);
    }

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
    /// Moves to the next page when possible.
    /// </summary>
    public void NextPage() => TrySetPage(_pageIndex + 1);

    /// <summary>
    /// Moves to the previous page when possible.
    /// </summary>
    public void PreviousPage() => TrySetPage(_pageIndex - 1);

    /// <summary>
    /// Sets the current page index using bounds clamping.
    /// </summary>
    /// <param name="pageIndex">The requested page index.</param>
    public void SetPage(int pageIndex) => TrySetPage(pageIndex);

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.Is(Key.PageUp))
        {
            return TrySetPage(_pageIndex - 1);
        }

        if (key.Is(Key.Right) || key.Is(Key.PageDown))
        {
            return TrySetPage(_pageIndex + 1);
        }

        if (key.Is(Key.Home))
        {
            return TrySetPage(0);
        }

        if (key.Is(Key.End))
        {
            return TrySetPage(_pageCount - 1);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left || pointer.Y != bounds.Y)
        {
            return Handle(message);
        }

        if (pointer.X < bounds.X || pointer.X >= bounds.Right)
        {
            return false;
        }

        var hitTargets = ResolveHitTargets(bounds);
        if (pointer.X >= hitTargets.PreviousStart && pointer.X < hitTargets.PreviousEnd)
        {
            return TrySetPage(_pageIndex - 1);
        }

        if (pointer.X >= hitTargets.NextStart && pointer.X < hitTargets.NextEnd)
        {
            return TrySetPage(_pageIndex + 1);
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

        var x = clipped.X;
        var y = clipped.Y;
        var plainTitle = FormatTitleText();
        if (!string.IsNullOrEmpty(plainTitle))
        {
            canvas.WriteText(x, y, RenderTitle(plainTitle), clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(plainTitle) + 1;
        }

        if (x >= clipped.Right)
        {
            return;
        }

        var previousStyle = CanMovePrevious()
            ? LabelStyle
            : LabelStyle.Merge(DisabledNavigationLabelStyle);
        var nextStyle = CanMoveNext()
            ? LabelStyle
            : LabelStyle.Merge(DisabledNavigationLabelStyle);
        var pageStyle = LabelStyle.Merge(ActivePageLabelStyle);

        WriteSegment(canvas, x, y, PreviousLabel, previousStyle, clipped.Right);
        x += ControlTextLayout.MeasureDisplayWidth(PreviousLabel) + 2;
        WriteSegment(canvas, x, y, CurrentPageLabel(), pageStyle, clipped.Right);
        x += ControlTextLayout.MeasureDisplayWidth(CurrentPageLabel()) + 2;
        WriteSegment(canvas, x, y, NextLabel, nextStyle, clipped.Right);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = ControlTextLayout.MeasureDisplayWidth(PreviousLabel)
            + 2
            + ControlTextLayout.MeasureDisplayWidth(CurrentPageLabel())
            + 2
            + ControlTextLayout.MeasureDisplayWidth(NextLabel);
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            width += ControlTextLayout.MeasureDisplayWidth(title) + 1;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(1, 0, availableBounds.Height));
    }

    private void SetPageCount(int pageCount)
    {
        var normalized = Math.Max(1, pageCount);
        if (_pageCount == normalized)
        {
            return;
        }

        _pageCount = normalized;
        TrySetPage(_pageIndex);
    }

    private bool TrySetPage(int requestedPageIndex)
    {
        var previous = _pageIndex;
        PageIndex = requestedPageIndex;
        return previous != _pageIndex;
    }

    private bool CanMovePrevious() => _pageIndex > 0;

    private bool CanMoveNext() => _pageIndex < (_pageCount - 1);

    private string CurrentPageLabel() => $"Page {_pageIndex + 1}/{_pageCount}";

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return $"{Title} {FocusMarker}";
        }

        return Title;
    }

    private string RenderTitle(string title)
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        if (style.IsEmpty || string.IsNullOrEmpty(title))
        {
            return title;
        }

        return style.Render(title);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        if (string.IsNullOrEmpty(text) || style.IsEmpty)
        {
            return text;
        }

        return style.Render(text);
    }

    private static void WriteSegment(Canvas canvas, int x, int y, string text, TesseraStyle style, int right)
    {
        if (x >= right || string.IsNullOrEmpty(text))
        {
            return;
        }

        canvas.WriteText(x, y, ApplyStyle(text, style), right - x);
    }

    private PagerHitTargets ResolveHitTargets(Rect bounds)
    {
        var cursor = bounds.X;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            cursor += ControlTextLayout.MeasureDisplayWidth(title) + 1;
        }

        var previousStart = cursor;
        var previousEnd = previousStart + ControlTextLayout.MeasureDisplayWidth(PreviousLabel);
        cursor = previousEnd + 2;

        cursor += ControlTextLayout.MeasureDisplayWidth(CurrentPageLabel()) + 2;
        var nextStart = cursor;
        var nextEnd = nextStart + ControlTextLayout.MeasureDisplayWidth(NextLabel);

        return new PagerHitTargets(previousStart, previousEnd, nextStart, nextEnd);
    }

    private readonly record struct PagerHitTargets(int PreviousStart, int PreviousEnd, int NextStart, int NextEnd);
}
