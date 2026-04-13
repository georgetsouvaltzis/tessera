using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Defines the axis used by <see cref="SplitView" /> to split its content area.
/// </summary>
public enum SplitViewOrientation
{
    /// <summary>
    ///     Splits left/right using a vertical divider.
    /// </summary>
    Horizontal = 0,

    /// <summary>
    ///     Splits top/bottom using a horizontal divider.
    /// </summary>
    Vertical = 1
}

/// <summary>
///     Represents a two-pane split container with draggable divider and focus handoff.
/// </summary>
public sealed class SplitView : Control
{
    private bool _isDraggingDivider;

    /// <summary>
    ///     Gets or sets primary pane control.
    /// </summary>
    public Control? First { get; set; }

    /// <summary>
    ///     Gets or sets secondary pane control.
    /// </summary>
    public Control? Second { get; set; }

    /// <summary>
    ///     Gets or sets split orientation.
    /// </summary>
    public SplitViewOrientation Orientation { get; set; } = SplitViewOrientation.Horizontal;

    /// <summary>
    ///     Gets or sets ratio used by the first pane in range [0.05, 0.95].
    /// </summary>
    public double Ratio
    {
        get => field;
        set => field = Math.Clamp(value, 0.05d, 0.95d);
    } = 0.5d;

    /// <summary>
    ///     Gets or sets minimum size for first pane in cells.
    /// </summary>
    public int MinFirstSize { get; set; } = 4;

    /// <summary>
    ///     Gets or sets minimum size for second pane in cells.
    /// </summary>
    public int MinSecondSize { get; set; } = 4;

    /// <summary>
    ///     Gets or sets whether divider glyphs are rendered.
    /// </summary>
    public bool ShowDivider { get; set; } = true;

    /// <summary>
    ///     Gets or sets divider thickness in cells.
    /// </summary>
    public int DividerThickness
    {
        get => field;
        set => field = Math.Max(1, value);
    } = 1;

    /// <summary>
    ///     Gets or sets divider glyph. Use <c>'\0'</c> to auto-select by orientation.
    /// </summary>
    public char DividerGlyph { get; set; }

    /// <summary>
    ///     Gets or sets border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    ///     Gets or sets content padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    ///     Gets or sets border style while not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets border style merged while focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets divider style while not focused.
    /// </summary>
    public TesseraStyle DividerStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets divider style merged while focused.
    /// </summary>
    public TesseraStyle FocusedDividerStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged while disabled.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets index of pane that currently receives keyboard input (0 or 1).
    /// </summary>
    public int ActivePaneIndex { get; private set; }

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        ApplyPaneFocus();
        if (IsDisabled)
        {
            return false;
        }

        if (message is not KeyPressed key || !IsFocused)
        {
            return ForwardToActivePane(message);
        }

        if (key.Is(Key.Tab))
        {
            return SetActivePane(ActivePaneIndex == 0 ? 1 : 0);
        }

        return Orientation switch
        {
            SplitViewOrientation.Horizontal when key.Is(Key.Left, ModifierKeys.Ctrl) => SetRatio(Ratio - 0.05d),
            SplitViewOrientation.Horizontal when key.Is(Key.Right, ModifierKeys.Ctrl) => SetRatio(Ratio + 0.05d),
            SplitViewOrientation.Vertical when key.Is(Key.Up, ModifierKeys.Ctrl) => SetRatio(Ratio - 0.05d),
            SplitViewOrientation.Vertical when key.Is(Key.Down, ModifierKeys.Ctrl) => SetRatio(Ratio + 0.05d),
            _ => ForwardToActivePane(message)
        };
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        ApplyPaneFocus();
        if (IsDisabled)
        {
            return false;
        }

        if (message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (!TryResolveLayout(content, out var layout))
        {
            return false;
        }

        if (ShowDivider && pointer is { Button: PointerButton.Left, Kind: PointerEventKind.Press } &&
            layout.Divider.Contains(pointer.X, pointer.Y))
        {
            _isDraggingDivider = true;
            RequestFocus();
            return SetRatioFromPointer(pointer, layout);
        }

        if (_isDraggingDivider && pointer.Kind == PointerEventKind.Motion)
        {
            return SetRatioFromPointer(pointer, layout);
        }

        if (_isDraggingDivider && pointer.Kind == PointerEventKind.Release)
        {
            _isDraggingDivider = false;
            return true;
        }

        if (pointer.Kind == PointerEventKind.Wheel && layout.Divider.Contains(pointer.X, pointer.Y))
        {
            return pointer.Button switch
            {
                PointerButton.WheelDown => SetRatio(Ratio + 0.05d),
                PointerButton.WheelUp => SetRatio(Ratio - 0.05d),
                _ => false
            };
        }

        if (layout.First.Contains(pointer.X, pointer.Y))
        {
            if (pointer is { Kind: PointerEventKind.Press, Button: PointerButton.Left })
            {
                RequestFocus();
            }

            var changed = SetActivePane(0);
            return ForwardPointer(First, pointer, layout.First) || changed;
        }

        if (layout.Second.Contains(pointer.X, pointer.Y))
        {
            if (pointer is { Kind: PointerEventKind.Press, Button: PointerButton.Left })
            {
                RequestFocus();
            }

            var changed = SetActivePane(1);
            return ForwardPointer(Second, pointer, layout.Second) || changed;
        }

        return false;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        ApplyPaneFocus();
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content =
            FrameLayout.DrawFrameAndResolveContent(canvas, clipped, null, Border, Padding, ResolveBorderStyle());
        if (!TryResolveLayout(content, out var layout))
        {
            return;
        }

        if (ShowDivider && !layout.Divider.IsEmpty)
        {
            DrawDivider(canvas, layout.Divider);
        }

        First?.Render(canvas, layout.First);
        Second?.Render(canvas, layout.Second);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return new LayoutMeasurement(
            Math.Clamp(availableBounds.Width, 0, availableBounds.Width),
            Math.Clamp(availableBounds.Height, 0, availableBounds.Height));
    }

    private bool TryResolveLayout(Rect content, out SplitLayoutInfo layout)
    {
        layout = default;
        if (content.IsEmpty)
        {
            return false;
        }

        var divider = ShowDivider ? DividerThickness : 0;
        if (Orientation == SplitViewOrientation.Horizontal)
        {
            if (content.Width <= divider)
            {
                return false;
            }

            var available = content.Width - divider;
            var firstSize = ResolvePaneSize(available, MinFirstSize, MinSecondSize);
            var secondSize = available - firstSize;
            var firstRect = new Rect(content.X, content.Y, firstSize, content.Height);
            var dividerRect = divider == 0
                ? new Rect(0, 0, 0, 0)
                : new Rect(firstRect.Right, content.Y, divider, content.Height);
            var secondRect = new Rect(firstRect.Right + divider, content.Y, secondSize, content.Height);
            layout = new SplitLayoutInfo(firstRect, secondRect, dividerRect);
            return true;
        }

        if (content.Height <= divider)
        {
            return false;
        }

        var availableHeight = content.Height - divider;
        var topSize = ResolvePaneSize(availableHeight, MinFirstSize, MinSecondSize);
        var bottomSize = availableHeight - topSize;
        var topRect = new Rect(content.X, content.Y, content.Width, topSize);
        var dividerRow = divider == 0
            ? new Rect(0, 0, 0, 0)
            : new Rect(content.X, topRect.Bottom, content.Width, divider);
        var bottomRect = new Rect(content.X, topRect.Bottom + divider, content.Width, bottomSize);
        layout = new SplitLayoutInfo(topRect, bottomRect, dividerRow);
        return true;
    }

    private int ResolvePaneSize(int available, int minFirst, int minSecond)
    {
        if (available <= 1)
        {
            return available;
        }

        var first = (int)Math.Round(available * Ratio, MidpointRounding.AwayFromZero);
        var firstMin = Math.Clamp(minFirst, 1, available);
        var firstMax = Math.Max(firstMin, available - Math.Clamp(minSecond, 1, available));
        return Math.Clamp(first, firstMin, firstMax);
    }

    private void DrawDivider(Canvas canvas, Rect divider)
    {
        var glyph = ResolveDividerGlyph();
        var token = ResolveDividerStyle().Render(glyph.ToString());
        if (ResolveDividerStyle().IsEmpty)
        {
            if (Orientation == SplitViewOrientation.Horizontal)
            {
                for (var x = 0; x < divider.Width; x++)
                {
                    canvas.DrawVerticalLine(divider.X + x, divider.Y, divider.Height, glyph);
                }
            }
            else
            {
                for (var y = 0; y < divider.Height; y++)
                {
                    canvas.DrawHorizontalLine(divider.X, divider.Y + y, divider.Width, glyph);
                }
            }

            return;
        }

        for (var y = 0; y < divider.Height; y++)
        {
            for (var x = 0; x < divider.Width; x++)
            {
                canvas.WriteText(divider.X + x, divider.Y + y, token, 1);
            }
        }
    }

    private bool SetRatio(double ratio)
    {
        var clamped = Math.Clamp(ratio, 0.05d, 0.95d);
        if (Math.Abs(clamped - Ratio) < double.Epsilon)
        {
            return false;
        }

        Ratio = clamped;
        return true;
    }

    private bool SetRatioFromPointer(PointerInput pointer, SplitLayoutInfo layout)
    {
        if (Orientation == SplitViewOrientation.Horizontal)
        {
            var total = layout.First.Width + layout.Second.Width;
            return total > 0 && SetRatio((pointer.X - layout.First.X + 1d) / total);
        }

        var totalHeight = layout.First.Height + layout.Second.Height;
        return totalHeight > 0 && SetRatio((pointer.Y - layout.First.Y + 1d) / totalHeight);
    }

    private bool ForwardToActivePane(Message message)
    {
        var target = ActivePaneIndex == 0 ? First : Second;
        return target?.Handle(message) ?? false;
    }

    private static bool ForwardPointer(Control? control, PointerInput pointer, Rect bounds)
    {
        return control?.Handle(pointer, bounds) ?? false;
    }

    private bool SetActivePane(int index)
    {
        var next = Math.Clamp(index, 0, 1);
        if (ActivePaneIndex == next)
        {
            return false;
        }

        ActivePaneIndex = next;
        ApplyPaneFocus();
        return true;
    }

    private void ApplyPaneFocus()
    {
        if (First is not null)
        {
            First.IsFocused = IsFocused && ActivePaneIndex == 0;
        }

        if (Second is not null)
        {
            Second.IsFocused = IsFocused && ActivePaneIndex == 1;
        }
    }

    private char ResolveDividerGlyph()
    {
        if (DividerGlyph != '\0')
        {
            return DividerGlyph;
        }

        return Orientation == SplitViewOrientation.Horizontal ? '│' : '─';
    }

    private TesseraStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private TesseraStyle ResolveDividerStyle()
    {
        var style = IsFocused ? DividerStyle.Merge(FocusedDividerStyle) : DividerStyle;
        return IsDisabled ? style.Merge(DisabledStyle) : style;
    }

    private readonly record struct SplitLayoutInfo(Rect First, Rect Second, Rect Divider);
}
