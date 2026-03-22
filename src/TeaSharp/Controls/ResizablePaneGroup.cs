using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a bordered horizontal pane group with draggable splits.
/// </summary>
public sealed partial class ResizablePaneGroup : Control
{
    private readonly List<PaneSpec> _panes = [];
    private readonly List<double> _splitRatios = [];
    private int _selectedPaneIndex;
    private int _dragDividerIndex = -1;
    private bool _isDraggingDivider;

    /// <summary>
    /// Occurs when selected pane changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<PaneSpec>>? SelectionChanged;

    /// <summary>
    /// Gets or sets group title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Resizable Pane Group";

    /// <summary>
    /// Gets or sets marker appended to title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets whether focus marker should be shown when focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets border style while not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets border style while focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style while not focused.
    /// </summary>
    public TeaStyle TitleStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style while not focused.
    /// </summary>
    /// <remarks>
    /// Canonical alias for cross-control title style naming consistency.
    /// </remarks>
    public TeaStyle TitleStyle
    {
        get => TitleStyleText;
        set => TitleStyleText = value;
    }

    /// <summary>
    /// Gets or sets title style while focused.
    /// </summary>
    public TeaStyle FocusedTitleStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets title style while focused.
    /// </summary>
    /// <remarks>
    /// Canonical alias for cross-control title style naming consistency.
    /// </remarks>
    public TeaStyle FocusedTitleStyle
    {
        get => FocusedTitleStyleText;
        set => FocusedTitleStyleText = value;
    }

    /// <summary>
    /// Gets or sets divider style while not focused.
    /// </summary>
    public TeaStyle DividerStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets divider style while focused.
    /// </summary>
    public TeaStyle FocusedDividerStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets pane text style.
    /// </summary>
    public TeaStyle PaneStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets selected pane text style.
    /// </summary>
    public TeaStyle SelectedPaneStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged while disabled.
    /// </summary>
    public TeaStyle DisabledStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets outer border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets divider glyph. Use <c>'\0'</c> for orientation default.
    /// </summary>
    public char DividerGlyph { get; set; } = '\0';

    /// <summary>
    /// Gets or sets divider thickness in cells.
    /// </summary>
    public int DividerThickness
    {
        get => field;
        set => field = Math.Max(1, value);
    } = 1;

    /// <summary>
    /// Gets or sets whether dividers are rendered.
    /// </summary>
    public bool ShowDividers { get; set; } = true;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets configured panes.
    /// </summary>
    public IReadOnlyList<PaneSpec> Panes => _panes;

    /// <summary>
    /// Gets selected pane index, or <c>-1</c> when no panes exist.
    /// </summary>
    public int SelectedPaneIndex => _panes.Count == 0 ? -1 : _selectedPaneIndex;

    /// <summary>
    /// Gets selected pane index, or <c>-1</c> when no panes exist.
    /// </summary>
    /// <remarks>
    /// Canonical selection alias for cross-control API consistency.
    /// </remarks>
    public int SelectedIndex => SelectedPaneIndex;

    /// <summary>
    /// Gets selected pane.
    /// </summary>
    public PaneSpec? SelectedPane => _panes.Count == 0 ? null : _panes[_selectedPaneIndex];

    /// <summary>
    /// Gets selected pane.
    /// </summary>
    /// <remarks>
    /// Canonical selection alias for cross-control API consistency.
    /// </remarks>
    public PaneSpec? SelectedItem => SelectedPane;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces pane definitions and resets split ratios to even layout.
    /// </summary>
    /// <param name="panes">Pane descriptors in visual order.</param>
    public void SetPanes(IEnumerable<PaneSpec> panes)
    {
        ArgumentNullException.ThrowIfNull(panes);
        var previousIndex = SelectedPaneIndex;
        var previousPane = SelectedPane;
        var previousId = previousPane?.Id;

        _panes.Clear();
        foreach (var pane in panes)
        {
            if (pane is not null)
            {
                _panes.Add(pane with { });
            }
        }

        RebuildEvenSplits();
        NormalizeSelection(previousId);
        RaiseSelectionChangedIfNeeded(previousIndex, previousPane);
    }

    /// <summary>
    /// Sets selected pane by index.
    /// </summary>
    /// <param name="index">Requested selected pane index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SetSelectedPaneIndex(int index)
    {
        if (_panes.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _panes.Count - 1);
        if (clamped == _selectedPaneIndex)
        {
            return false;
        }

        var previousIndex = _selectedPaneIndex;
        var previousPane = _panes[previousIndex];
        _selectedPaneIndex = clamped;
        RaiseSelectionChanged(previousIndex, previousPane);
        return true;
    }

    /// <summary>
    /// Sets selected pane by index.
    /// </summary>
    /// <param name="index">Requested selected pane index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    /// <remarks>
    /// Canonical selection mutator alias for cross-control API consistency.
    /// </remarks>
    public bool SetSelectedIndex(int index) => SetSelectedPaneIndex(index);

    /// <summary>
    /// Sets split ratio for divider index.
    /// </summary>
    /// <param name="splitIndex">Divider index between pane <c>splitIndex</c> and <c>splitIndex+1</c>.</param>
    /// <param name="ratio">Target cumulative ratio in range [0,1].</param>
    /// <returns><see langword="true" /> when ratio changed; otherwise <see langword="false" />.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="splitIndex" /> is outside divider range.</exception>
    public bool SetSplitRatio(int splitIndex, double ratio)
    {
        if (splitIndex < 0 || splitIndex >= _splitRatios.Count)
        {
            throw new ArgumentOutOfRangeException(nameof(splitIndex));
        }

        var previous = splitIndex == 0 ? 0d : _splitRatios[splitIndex - 1];
        var next = splitIndex == _splitRatios.Count - 1 ? 1d : _splitRatios[splitIndex + 1];
        const double minGap = 0.05d;
        var bounded = Math.Clamp(ratio, previous + minGap, next - minGap);
        if (Math.Abs(_splitRatios[splitIndex] - bounded) <= double.Epsilon)
        {
            return false;
        }

        _splitRatios[splitIndex] = bounded;
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        ApplyPaneFocus();
        if (IsDisabled || IsReadOnly || !IsFocused || _panes.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Left))
        {
            return SetSelectedPaneIndex(_selectedPaneIndex - 1);
        }

        if (key.Is(Key.Right))
        {
            return SetSelectedPaneIndex(_selectedPaneIndex + 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedPaneIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedPaneIndex(_panes.Count - 1);
        }

        if (key.Is(Key.Left, ModifierKeys.Ctrl) && _selectedPaneIndex > 0)
        {
            return SetSplitRatio(_selectedPaneIndex - 1, _splitRatios[_selectedPaneIndex - 1] - 0.05d);
        }

        if (key.Is(Key.Right, ModifierKeys.Ctrl) && _selectedPaneIndex < _panes.Count - 1)
        {
            return SetSplitRatio(_selectedPaneIndex, _splitRatios[_selectedPaneIndex] + 0.05d);
        }

        return ForwardToSelectedPane(message);
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        ApplyPaneFocus();
        if (IsDisabled || IsReadOnly || bounds.IsEmpty || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (!TryResolveLayout(content, out var layout))
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            var hitDivider = HitTestDivider(layout, pointer.X, pointer.Y);
            if (hitDivider >= 0)
            {
                _isDraggingDivider = true;
                _dragDividerIndex = hitDivider;
                RequestFocus();
                return UpdateDraggedSplit(layout, pointer.X);
            }

            var hitPane = HitTestPane(layout, pointer.X, pointer.Y);
            if (hitPane >= 0)
            {
                RequestFocus();
                var changed = SetSelectedPaneIndex(hitPane);
                return ForwardToPane(_panes[hitPane], pointer, layout.Panes[hitPane]) || changed;
            }
        }

        if (_isDraggingDivider && pointer.Kind == PointerEventKind.Motion)
        {
            return UpdateDraggedSplit(layout, pointer.X);
        }

        if (_isDraggingDivider && pointer.Kind == PointerEventKind.Release)
        {
            _isDraggingDivider = false;
            _dragDividerIndex = -1;
            return true;
        }

        var paneIndex = HitTestPane(layout, pointer.X, pointer.Y);
        if (paneIndex >= 0)
        {
            return ForwardToPane(_panes[paneIndex], pointer, layout.Panes[paneIndex]);
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

        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (!TryResolveLayout(content, out var layout))
        {
            return;
        }

        DrawDividers(canvas, layout);
        for (var index = 0; index < _panes.Count; index++)
        {
            var paneBounds = layout.Panes[index];
            if (paneBounds.IsEmpty)
            {
                continue;
            }

            var pane = _panes[index];
            if (pane.Content is { } contentControl)
            {
                contentControl.Render(canvas, paneBounds);
                continue;
            }

            var style = ResolvePaneStyle(index);
            var text = index == _selectedPaneIndex
                ? $"> {ResolvePaneText(pane)}"
                : ResolvePaneText(pane);
            canvas.WriteText(paneBounds.X, paneBounds.Y, ApplyStyle(text, style), paneBounds.Width);
        }
    }

    private void ApplyPaneFocus()
    {
        for (var index = 0; index < _panes.Count; index++)
        {
            _panes[index].Content?.ApplyFocus(IsFocused && index == _selectedPaneIndex);
        }
    }

    private bool ForwardToSelectedPane(Message message)
    {
        if (_selectedPaneIndex < 0 || _selectedPaneIndex >= _panes.Count)
        {
            return false;
        }

        return _panes[_selectedPaneIndex].Content?.Handle(message) == true;
    }

    private static bool ForwardToPane(PaneSpec pane, PointerInput pointer, Rect bounds)
    {
        return pane.Content?.Handle(pointer, bounds) == true;
    }

    private bool UpdateDraggedSplit(PaneLayoutInfo layout, int pointerX)
    {
        if (_dragDividerIndex < 0 || _dragDividerIndex >= _splitRatios.Count)
        {
            return false;
        }

        var dividerCount = ShowDividers ? _splitRatios.Count : 0;
        var availableWidth = Math.Max(1, layout.Content.Width - (dividerCount * DividerThickness));
        var raw = pointerX - layout.Content.X - (_dragDividerIndex * DividerThickness);
        var ratio = (double)raw / availableWidth;
        return SetSplitRatio(_dragDividerIndex, ratio);
    }

    private void RebuildEvenSplits()
    {
        _splitRatios.Clear();
        if (_panes.Count <= 1)
        {
            return;
        }

        for (var index = 1; index < _panes.Count; index++)
        {
            _splitRatios.Add((double)index / _panes.Count);
        }
    }

    private void NormalizeSelection(string? preferredPaneId)
    {
        if (_panes.Count == 0)
        {
            _selectedPaneIndex = 0;
            return;
        }

        if (!string.IsNullOrWhiteSpace(preferredPaneId))
        {
            for (var index = 0; index < _panes.Count; index++)
            {
                if (string.Equals(_panes[index].Id, preferredPaneId, StringComparison.Ordinal))
                {
                    _selectedPaneIndex = index;
                    return;
                }
            }
        }

        _selectedPaneIndex = Math.Clamp(_selectedPaneIndex, 0, _panes.Count - 1);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, PaneSpec? previousPane)
    {
        if (previousIndex == SelectedPaneIndex
            && string.Equals(previousPane?.Id, SelectedPane?.Id, StringComparison.Ordinal))
        {
            return;
        }

        RaiseSelectionChanged(previousIndex, previousPane);
    }

    private void RaiseSelectionChanged(int previousIndex, PaneSpec? previousPane)
    {
        SelectionChanged?.Invoke(
            this,
            new ListSelectionChangedEventArgs<PaneSpec>(previousIndex, SelectedPaneIndex, previousPane, SelectedPane));
    }

}
