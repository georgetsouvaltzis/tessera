using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;
using Tessera.Widgets;

namespace Tessera.Controls;

/// <summary>Compact single-line search field with match metadata and navigation commands.</summary>
public sealed class SearchBox : Control
{
    private readonly TextInputModel _input = new();
    private int? _matchCount;

    /// <summary>Initializes a new search box.</summary>
    public SearchBox()
    {
        _input.Placeholder = "Search...";
    }

    /// <summary>Gets or sets the field title.</summary>
    public string Title { get; set; } = "Search";

    /// <summary>Gets or sets the focus marker appended to the title.</summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>Gets or sets whether the focus marker is rendered.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets the title style when unfocused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets the title style when focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets placeholder text shown when query is empty.</summary>
    public string Placeholder
    {
        get => _input.Placeholder;
        set => _input.Placeholder = value;
    }

    /// <summary>Gets or sets the current query text.</summary>
    public string QueryText
    {
        get => _input.Value;
        set => SetQueryText(value);
    }

    /// <summary>Gets the known total match count, when available.</summary>
    public int? MatchCount => _matchCount;

    /// <summary>Gets the current zero-based match index, when available.</summary>
    public int? CurrentMatchIndex { get; private set; }

    /// <summary>Gets or sets the border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets style for query text.</summary>
    public TesseraStyle ValueTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style for placeholder text.</summary>
    public TesseraStyle PlaceholderTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style applied to border glyphs when the control is not focused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style applied to border glyphs when the control is focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style for the match counter label.</summary>
    public TesseraStyle MatchCounterStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged when matches are available.</summary>
    public TesseraStyle MatchHighlightStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style for previous/next labels.</summary>
    public TesseraStyle NavigationLabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into disabled previous/next labels.</summary>
    public TesseraStyle DisabledNavigationLabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets whether previous/next labels are shown.</summary>
    public bool ShowNavigationLabels { get; set; } = true;

    /// <summary>Gets or sets the previous-match label.</summary>
    public string PreviousLabel { get; set; } = "Prev";

    /// <summary>Gets or sets the next-match label.</summary>
    public string NextLabel { get; set; } = "Next";

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Occurs when <see cref="QueryText" /> changes.</summary>
    public event EventHandler<SearchBoxQueryChangedEventArgs>? QueryChanged;

    /// <summary>Occurs when next/previous navigation is requested.</summary>
    public event EventHandler<SearchBoxNavigationRequestedEventArgs>? NavigationRequested;

    /// <summary>Replaces query text and raises <see cref="QueryChanged" /> when needed.</summary>
    /// <param name="query">The query text to set.</param>
    public void SetQueryText(string query)
    {
        var normalized = query;
        if (string.Equals(_input.Value, normalized, StringComparison.Ordinal))
        {
            return;
        }

        var previous = _input.Value;
        _input.SetValue(normalized);
        QueryChanged?.Invoke(this, new SearchBoxQueryChangedEventArgs(previous, _input.Value));
    }

    /// <summary>Clears query text.</summary>
    public void ClearQuery()
    {
        SetQueryText(string.Empty);
    }

    /// <summary>Replaces optional match metadata.</summary>
    /// <param name="matchCount">Total matches, or <see langword="null" /> to clear state.</param>
    /// <param name="currentMatchIndex">Current zero-based index. Clamped when matches exist.</param>
    public void SetMatchState(int? matchCount, int? currentMatchIndex = null)
    {
        if (!matchCount.HasValue || matchCount.Value <= 0)
        {
            _matchCount = null;
            CurrentMatchIndex = null;
            return;
        }

        var count = matchCount.Value;
        _matchCount = count;
        CurrentMatchIndex = Math.Clamp(currentMatchIndex ?? CurrentMatchIndex ?? 0, 0, count - 1);
    }

    /// <summary>Clears optional match metadata.</summary>
    public void ClearMatchState()
    {
        _matchCount = null;
        CurrentMatchIndex = null;
    }

    /// <summary>Requests next match navigation.</summary>
    public void NextMatch()
    {
        RequestNavigation(SearchNavigationDirection.Next);
    }

    /// <summary>Requests previous match navigation.</summary>
    public void PreviousMatch()
    {
        RequestNavigation(SearchNavigationDirection.Previous);
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled)
        {
            return false;
        }

        if (message is KeyPressed key && IsFocused && key.Key is Key.Enter or Key.F3)
        {
            if (key.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                PreviousMatch();
            }
            else
            {
                NextMatch();
            }

            return true;
        }

        if (!IsFocused || IsReadOnly)
        {
            return false;
        }

        var previousQuery = _input.Value;
        var result = _input.Update(message);
        if (!result.Changed)
        {
            return false;
        }

        if (!string.Equals(previousQuery, _input.Value, StringComparison.Ordinal))
        {
            QueryChanged?.Invoke(this, new SearchBoxQueryChangedEventArgs(previousQuery, _input.Value));
        }

        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var insideContent = content.Contains(pointer.X, pointer.Y);
        if (insideContent && pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                NextMatch();
                return true;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                PreviousMatch();
                return true;
            }
        }

        if (pointer.Kind != PointerEventKind.Press
            || pointer.Button != PointerButton.Left
            || !insideContent)
        {
            return Handle(message);
        }

        RequestFocus();
        if (pointer.Y != content.Y)
        {
            return true;
        }

        var layout = ResolveInlineLayout(content.Width);
        var localX = pointer.X - content.X;
        if (IsHit(localX, layout.PreviousStart, layout.PreviousLabel))
        {
            PreviousMatch();
            return true;
        }

        if (IsHit(localX, layout.NextStart, layout.NextLabel))
        {
            NextMatch();
            return true;
        }

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

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            title,
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty || content.Height < 1)
        {
            return;
        }

        var layout = ResolveInlineLayout(content.Width);
        var frame = _input.BuildFrame(layout.QueryWidth);
        var valueStyle = frame.PlaceholderVisible ? PlaceholderTextStyle : ValueTextStyle;
        canvas.WriteText(content.X, content.Y, ApplyStyle(frame.Text, valueStyle), layout.QueryWidth);

        WriteSegment(canvas, content, layout.MatchStart, ApplyStyle(layout.MatchLabel, ResolveMatchStyle()));
        var navStyle = ResolveNavigationStyle(layout.NavigationEnabled);
        WriteSegment(canvas, content, layout.PreviousStart, ApplyStyle(layout.PreviousLabel, navStyle));
        WriteSegment(canvas, content, layout.NextStart, ApplyStyle(layout.NextLabel, navStyle));
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var widthBound = Math.Max(1, availableBounds.Width);
        var layout = ResolveInlineLayout(widthBound);
        var frame = _input.BuildFrame(layout.QueryWidth);

        var width = ControlTextLayout.MeasureDisplayWidth(frame.Text) + layout.ExtrasWidth + Padding.Horizontal;
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

    private void RequestNavigation(SearchNavigationDirection direction)
    {
        var previous = CurrentMatchIndex;
        if (_matchCount is > 0)
        {
            var count = _matchCount.Value;
            var current = CurrentMatchIndex ?? 0;
            CurrentMatchIndex = direction == SearchNavigationDirection.Next
                ? (current + 1) % count
                : (current + count - 1) % count;
        }

        NavigationRequested?.Invoke(this,
            new SearchBoxNavigationRequestedEventArgs(direction, previous, CurrentMatchIndex, _matchCount));
    }

    private TesseraStyle ResolveMatchStyle()
    {
        var style = MatchCounterStyle;
        if (_matchCount.HasValue)
        {
            style = style.Merge(MatchHighlightStyle);
        }

        return style;
    }

    private TesseraStyle ResolveNavigationStyle(bool enabled)
    {
        var style = NavigationLabelStyle;
        if (!enabled || IsDisabled || IsReadOnly)
        {
            style = style.Merge(DisabledNavigationLabelStyle);
        }

        return style;
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
            style = style.Merge(DisabledNavigationLabelStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return ApplyStyle(title, style);
    }

    private string BuildMatchLabel()
    {
        if (_matchCount is not int count || count <= 0)
        {
            return string.Empty;
        }

        var displayIndex = Math.Clamp((CurrentMatchIndex ?? 0) + 1, 1, count);
        return $"{displayIndex}/{count}";
    }

    private InlineLayout ResolveInlineLayout(int contentWidth)
    {
        var safeWidth = Math.Max(1, contentWidth);
        var matchLabel = BuildMatchLabel();
        var hasPrevious = ShowNavigationLabels && !string.IsNullOrEmpty(PreviousLabel);
        var hasNext = ShowNavigationLabels && !string.IsNullOrEmpty(NextLabel);

        var extras = 0;
        if (!string.IsNullOrEmpty(matchLabel))
        {
            extras += 1 + ControlTextLayout.MeasureDisplayWidth(matchLabel);
        }

        if (hasPrevious)
        {
            extras += 1 + ControlTextLayout.MeasureDisplayWidth(PreviousLabel);
        }

        if (hasNext)
        {
            extras += 1 + ControlTextLayout.MeasureDisplayWidth(NextLabel);
        }

        var queryWidth = Math.Max(1, safeWidth - extras);
        var cursor = queryWidth;
        var matchStart = -1;
        var previousStart = -1;
        var nextStart = -1;

        if (!string.IsNullOrEmpty(matchLabel))
        {
            cursor += 1;
            matchStart = cursor;
            cursor += ControlTextLayout.MeasureDisplayWidth(matchLabel);
        }

        if (hasPrevious)
        {
            cursor += 1;
            previousStart = cursor;
            cursor += ControlTextLayout.MeasureDisplayWidth(PreviousLabel);
        }

        if (hasNext)
        {
            cursor += 1;
            nextStart = cursor;
        }

        var navigationEnabled = _matchCount is null || _matchCount > 1;
        return new InlineLayout(
            queryWidth,
            extras,
            matchLabel,
            matchStart,
            hasPrevious ? PreviousLabel : string.Empty,
            previousStart,
            hasNext ? NextLabel : string.Empty,
            nextStart,
            navigationEnabled);
    }

    private static bool IsHit(int localX, int start, string label)
    {
        if (start < 0 || string.IsNullOrEmpty(label))
        {
            return false;
        }

        var end = start + ControlTextLayout.MeasureDisplayWidth(label);
        return localX >= start && localX < end;
    }

    private static void WriteSegment(Canvas canvas, Rect content, int start, string text)
    {
        if (start < 0 || string.IsNullOrEmpty(text))
        {
            return;
        }

        var x = content.X + start;
        if (x >= content.Right)
        {
            return;
        }

        canvas.WriteText(x, content.Y, text, content.Right - x);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }

    private readonly record struct InlineLayout(
        int QueryWidth,
        int ExtrasWidth,
        string MatchLabel,
        int MatchStart,
        string PreviousLabel,
        int PreviousStart,
        string NextLabel,
        int NextStart,
        bool NavigationEnabled);
}
