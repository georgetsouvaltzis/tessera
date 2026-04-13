using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Lane-based kanban board control for dashboard workflows.
/// </summary>
public sealed class KanbanBoard : Control
{
    private readonly List<KanbanLane> _lanes = [];
    private int _hoveredCardIndex = -1;
    private int _hoveredLaneIndex = -1;

    /// <summary>
    ///     Represents title.
    /// </summary>
    public string Title { get; set; } = "Kanban Board";

    /// <summary>
    ///     Represents focus marker.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether show focus marker.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Represents selected card marker.
    /// </summary>
    public string SelectedCardMarker { get; set; } = "▸";

    /// <summary>
    ///     Represents unselected card marker.
    /// </summary>
    public string UnselectedCardMarker { get; set; } = " ";

    /// <summary>
    ///     Gets or sets the title style.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the focused title style.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the lane header style.
    /// </summary>
    public TesseraStyle LaneHeaderStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the selected lane header style.
    /// </summary>
    public TesseraStyle SelectedLaneHeaderStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the card style.
    /// </summary>
    public TesseraStyle CardStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the selected card style.
    /// </summary>
    public TesseraStyle SelectedCardStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the focused card style.
    /// </summary>
    public TesseraStyle FocusedCardStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the hovered card style.
    /// </summary>
    public TesseraStyle HoveredCardStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the disabled card style.
    /// </summary>
    public TesseraStyle DisabledCardStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the error card style.
    /// </summary>
    public TesseraStyle ErrorCardStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the border.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    ///     Gets or sets the padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    ///     Gets or sets the border style text.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets the focused border style text.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets whether has error.
    /// </summary>
    public bool HasError { get; set; }

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    ///     Represents lanes.
    /// </summary>
    public IReadOnlyList<KanbanLane> Lanes => _lanes;

    /// <summary>
    ///     Represents selected lane index.
    /// </summary>
    public int SelectedLaneIndex { get; private set; } = -1;

    /// <summary>
    ///     Represents selected card index.
    /// </summary>
    public int SelectedCardIndex { get; private set; } = -1;

    /// <summary>
    ///     Represents selected lane.
    /// </summary>
    public KanbanLane? SelectedLane =>
        SelectedLaneIndex >= 0 && SelectedLaneIndex < _lanes.Count ? _lanes[SelectedLaneIndex] : null;

    /// <summary>
    ///     Represents selected card.
    /// </summary>
    public KanbanCard? SelectedCard =>
        SelectedLane is { } lane && SelectedCardIndex >= 0 && SelectedCardIndex < lane.Count
            ? lane.Cards[SelectedCardIndex]
            : null;

    /// <summary>
    ///     Represents selection changed.
    /// </summary>
    public event EventHandler<KanbanSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    ///     Executes set lanes.
    /// </summary>
    /// <param name="lanes">The lanes value.</param>
    public void SetLanes(IEnumerable<KanbanLane> lanes)
    {
        ArgumentNullException.ThrowIfNull(lanes);
        var previousLaneIndex = SelectedLaneIndex;
        var previousCardIndex = SelectedCardIndex;
        var previousLane = SelectedLane;
        var previousCard = SelectedCard;
        _lanes.Clear();
        foreach (var lane in lanes)
        {
            _lanes.Add(lane);
        }

        NormalizeSelection();
        RaiseSelectionChangedIfNeeded(previousLaneIndex, previousCardIndex, previousLane, previousCard);
    }

    /// <summary>
    ///     Executes set selected.
    /// </summary>
    /// <param name="laneIndex">The lane index value.</param>
    /// <param name="cardIndex">The card index value.</param>
    /// <returns><see langword="true" /> when set selected succeeds.</returns>
    public bool SetSelected(int laneIndex, int cardIndex)
    {
        if (_lanes.Count == 0)
        {
            return false;
        }

        var normalizedLaneIndex = Math.Clamp(laneIndex, 0, _lanes.Count - 1);
        var normalizedCardIndex = ResolveCardIndex(_lanes[normalizedLaneIndex], cardIndex);
        if (normalizedLaneIndex == SelectedLaneIndex && normalizedCardIndex == SelectedCardIndex)
        {
            return false;
        }

        var previousLaneIndex = SelectedLaneIndex;
        var previousCardIndex = SelectedCardIndex;
        var previousLane = SelectedLane;
        var previousCard = SelectedCard;
        SelectedLaneIndex = normalizedLaneIndex;
        SelectedCardIndex = normalizedCardIndex;
        RaiseSelectionChangedIfNeeded(previousLaneIndex, previousCardIndex, previousLane, previousCard);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _lanes.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left) || key.IsCharacter('h'))
        {
            return MoveLane(-1);
        }

        if (key.Is(Key.Right) || key.IsCharacter('l'))
        {
            return MoveLane(1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return MoveCard(-1);
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return MoveCard(1);
        }

        if (key.Is(Key.Home) || key.Is(Key.PageUp))
        {
            return SetSelected(0, SelectedCardIndex);
        }

        if (key.Is(Key.End) || key.Is(Key.PageDown))
        {
            return SetSelected(_lanes.Count - 1, SelectedCardIndex);
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer || bounds.IsEmpty || _lanes.Count == 0)
        {
            return Handle(message);
        }

        if (IsDisabled || IsReadOnly)
        {
            return false;
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return false;
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return MoveCard(1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return MoveCard(-1);
            }

            return false;
        }

        if (!content.Contains(pointer.X, pointer.Y))
        {
            return pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press
                   && SetHovered(-1, -1);
        }

        var hit = TryHitLaneCard(content, pointer.X, pointer.Y, out var laneIndex, out var cardIndex);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return hit && SetHovered(laneIndex, cardIndex);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            var changed = SetHovered(hit ? laneIndex : -1, hit ? cardIndex : -1);
            if (!hit)
            {
                return changed;
            }

            _ = SetSelected(laneIndex, cardIndex >= 0 ? cardIndex : SelectedCardIndex);
            return true;
        }

        return false;
    }

    /// <inheritdoc />
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
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        if (_lanes.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle("(no lanes)", LaneHeaderStyle), content.Width);
            return;
        }

        var lanesToRender = Math.Min(_lanes.Count, Math.Max(1, content.Width));
        var baseWidth = content.Width / lanesToRender;
        var remainder = content.Width % lanesToRender;
        var laneX = content.X;
        for (var laneIndex = 0; laneIndex < lanesToRender; laneIndex++)
        {
            var laneWidth = baseWidth + (laneIndex < remainder ? 1 : 0);
            if (laneWidth <= 0)
            {
                continue;
            }

            RenderLane(canvas, new Rect(laneX, content.Y, laneWidth, content.Height), laneIndex);
            laneX += laneWidth;
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var laneCount = Math.Max(1, Math.Min(3, _lanes.Count));
        var width = Math.Max(24, ControlTextLayout.MeasureDisplayWidth(FormatTitleForMeasure()) + 4);
        var laneWidth = 12;
        for (var index = 0; index < _lanes.Count; index++)
        {
            laneWidth = Math.Max(laneWidth, ControlTextLayout.MeasureDisplayWidth(_lanes[index].Title) + 2);
        }

        width = Math.Max(width, laneWidth * laneCount);
        var height = Math.Max(5, (_lanes.Count == 0 ? 1 : _lanes.Max(static lane => lane.Count)) + 3);
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderLane(Canvas canvas, Rect laneRect, int laneIndex)
    {
        if (laneRect.IsEmpty)
        {
            return;
        }

        var lane = _lanes[laneIndex];
        var headerText = lane.Title;
        canvas.WriteText(laneRect.X, laneRect.Y, ApplyStyle(headerText, ResolveLaneHeaderStyle(laneIndex)),
            laneRect.Width);
        if (laneRect.Height <= 1)
        {
            return;
        }

        var rowsAvailable = laneRect.Height - 1;
        if (lane.Count == 0)
        {
            canvas.WriteText(laneRect.X, laneRect.Y + 1, ApplyStyle("(empty)", CardStyle), laneRect.Width);
            return;
        }

        var cardRows = lane.Count > rowsAvailable
            ? Math.Max(0, rowsAvailable - 1)
            : Math.Min(rowsAvailable, lane.Count);
        for (var cardIndex = 0; cardIndex < cardRows; cardIndex++)
        {
            var card = lane.Cards[cardIndex];
            var marker = laneIndex == SelectedLaneIndex && cardIndex == SelectedCardIndex
                ? SelectedCardMarker
                : UnselectedCardMarker;
            var line = $"{marker} {card.Title}";
            canvas.WriteText(
                laneRect.X,
                laneRect.Y + 1 + cardIndex,
                ApplyStyle(line, ResolveCardStyle(laneIndex, cardIndex, card)),
                laneRect.Width);
        }

        if (lane.Count > cardRows && rowsAvailable > 0)
        {
            var remaining = lane.Count - cardRows;
            var overflowText = $"… +{remaining}";
            canvas.WriteText(laneRect.X, laneRect.Bottom - 1, ApplyStyle(overflowText, LaneHeaderStyle),
                laneRect.Width);
        }
    }

    private TesseraStyle ResolveLaneHeaderStyle(int laneIndex)
    {
        var style = LaneHeaderStyle;
        if (laneIndex == SelectedLaneIndex)
        {
            style = style.Merge(SelectedLaneHeaderStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledCardStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorCardStyle);
        }

        return style;
    }

    private TesseraStyle ResolveCardStyle(int laneIndex, int cardIndex, KanbanCard card)
    {
        var style = CardStyle;
        var isSelected = laneIndex == SelectedLaneIndex && cardIndex == SelectedCardIndex;
        if (isSelected)
        {
            style = style.Merge(SelectedCardStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedCardStyle);
            }
        }

        if (laneIndex == _hoveredLaneIndex && cardIndex == _hoveredCardIndex)
        {
            style = style.Merge(HoveredCardStyle);
        }

        if (IsDisabled || card.IsDisabled)
        {
            style = style.Merge(DisabledCardStyle);
        }

        if (HasError || card.HasError)
        {
            style = style.Merge(ErrorCardStyle);
        }

        return style;
    }

    private string RenderTitle()
    {
        return ApplyStyle(FormatTitleText(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private string FormatTitleForMeasure()
    {
        return ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title;
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
            style = style.Merge(DisabledCardStyle);
        }

        if (HasError)
        {
            style = style.Merge(ErrorCardStyle);
        }

        return style;
    }

    private void NormalizeSelection()
    {
        if (_lanes.Count == 0)
        {
            SelectedLaneIndex = -1;
            SelectedCardIndex = -1;
            _hoveredLaneIndex = -1;
            _hoveredCardIndex = -1;
            return;
        }

        if (SelectedLaneIndex < 0)
        {
            SelectedLaneIndex = 0;
        }
        else if (SelectedLaneIndex >= _lanes.Count)
        {
            SelectedLaneIndex = _lanes.Count - 1;
        }

        SelectedCardIndex = ResolveCardIndex(_lanes[SelectedLaneIndex], SelectedCardIndex);
        _hoveredLaneIndex = Math.Clamp(_hoveredLaneIndex, -1, _lanes.Count - 1);
        if (_hoveredLaneIndex >= 0)
        {
            _hoveredCardIndex = ResolveCardIndex(_lanes[_hoveredLaneIndex], _hoveredCardIndex);
        }
        else
        {
            _hoveredCardIndex = -1;
        }
    }

    private static int ResolveCardIndex(KanbanLane lane, int desiredCardIndex)
    {
        if (lane.Count == 0)
        {
            return -1;
        }

        return desiredCardIndex < 0
            ? 0
            : Math.Clamp(desiredCardIndex, 0, lane.Count - 1);
    }

    private bool MoveLane(int delta)
    {
        if (_lanes.Count == 0 || delta == 0)
        {
            return false;
        }

        var laneIndex = SelectedLaneIndex < 0 ? 0 : SelectedLaneIndex;
        var next = Math.Clamp(laneIndex + delta, 0, _lanes.Count - 1);
        return SetSelected(next, SelectedCardIndex);
    }

    private bool MoveCard(int delta)
    {
        var lane = SelectedLane;
        if (lane is null || lane.Count == 0 || delta == 0)
        {
            return false;
        }

        var cardIndex = SelectedCardIndex < 0 ? 0 : SelectedCardIndex;
        var next = Math.Clamp(cardIndex + delta, 0, lane.Count - 1);
        return SetSelected(SelectedLaneIndex, next);
    }

    private bool TryHitLaneCard(Rect content, int x, int y, out int laneIndex, out int cardIndex)
    {
        laneIndex = -1;
        cardIndex = -1;
        var lanesToRender = Math.Min(_lanes.Count, Math.Max(1, content.Width));
        var baseWidth = content.Width / lanesToRender;
        var remainder = content.Width % lanesToRender;
        var laneX = content.X;
        for (var index = 0; index < lanesToRender; index++)
        {
            var laneWidth = baseWidth + (index < remainder ? 1 : 0);
            if (laneWidth <= 0)
            {
                continue;
            }

            var laneRect = new Rect(laneX, content.Y, laneWidth, content.Height);
            laneX += laneWidth;
            if (!laneRect.Contains(x, y))
            {
                continue;
            }

            laneIndex = index;
            if (y == laneRect.Y)
            {
                cardIndex = -1;
                return true;
            }

            var row = y - (laneRect.Y + 1);
            if (row < 0 || row >= _lanes[index].Count)
            {
                cardIndex = -1;
                return true;
            }

            cardIndex = row;
            return true;
        }

        return false;
    }

    private bool SetHovered(int laneIndex, int cardIndex)
    {
        var changed = _hoveredLaneIndex != laneIndex || _hoveredCardIndex != cardIndex;
        _hoveredLaneIndex = laneIndex;
        _hoveredCardIndex = cardIndex;
        return changed;
    }

    private void RaiseSelectionChangedIfNeeded(
        int previousLaneIndex,
        int previousCardIndex,
        KanbanLane? previousLane,
        KanbanCard? previousCard)
    {
        if (previousLaneIndex == SelectedLaneIndex
            && previousCardIndex == SelectedCardIndex
            && ReferenceEquals(previousLane, SelectedLane)
            && ReferenceEquals(previousCard, SelectedCard))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new KanbanSelectionChangedEventArgs(
                previousLaneIndex,
                previousCardIndex,
                SelectedLaneIndex,
                SelectedCardIndex,
                previousLane,
                previousCard,
                SelectedLane,
                SelectedCard));
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty || string.IsNullOrEmpty(text)
            ? text
            : style.Render(text);
    }
}
