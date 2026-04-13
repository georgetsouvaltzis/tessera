using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a treemap chart for weighted hierarchical data.
/// </summary>
public sealed class TreeMapChart : Control
{
    private readonly List<LeafNode> _leaves = [];
    private readonly List<RenderedNode> _rendered = [];
    private readonly List<TreeMapNode> _roots = [];
    private int _hoveredIndex = -1;

    /// <summary>Gets or sets chart title.</summary>
    public string Title { get; set; } = "TreeMap";

    /// <summary>Gets or sets marker text appended to <see cref="Title" /> while focused.</summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>Gets or sets whether <see cref="FocusMarker" /> is rendered while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>Gets or sets text shown when there are no nodes.</summary>
    public string EmptyText { get; set; } = "(empty)";

    /// <summary>Gets or sets whether leaf labels are rendered inside cells.</summary>
    public bool ShowLabels { get; set; } = true;

    /// <summary>Gets or sets whether legend text is rendered in the footer row.</summary>
    public bool ShowLegend { get; set; } = true;

    /// <summary>Gets or sets border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }

    /// <summary>Gets or sets title style while not focused.</summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets title style while focused.</summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets border glyph style while not focused.</summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets border glyph style while focused.</summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets base style for all treemap cells.</summary>
    public TesseraStyle NodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style for low-weight cells.</summary>
    public TesseraStyle LowNodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style for medium-weight cells.</summary>
    public TesseraStyle MidNodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style for high-weight cells.</summary>
    public TesseraStyle HighNodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style for peak-weight cells.</summary>
    public TesseraStyle PeakNodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into hovered cells.</summary>
    public TesseraStyle HoveredNodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into selected cells.</summary>
    public TesseraStyle SelectedNodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged into selected cells while focused.</summary>
    public TesseraStyle FocusedSelectedNodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style merged while the control is disabled.</summary>
    public TesseraStyle DisabledNodeStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style used for labels and legend text.</summary>
    public TesseraStyle LabelStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets style used for empty-state text.</summary>
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>Gets or sets glyph for low-intensity cells.</summary>
    public char LowGlyph { get; set; } = '░';

    /// <summary>Gets or sets glyph for medium-intensity cells.</summary>
    public char MidGlyph { get; set; } = '▒';

    /// <summary>Gets or sets glyph for high-intensity cells.</summary>
    public char HighGlyph { get; set; } = '▓';

    /// <summary>Gets or sets glyph for peak-intensity cells.</summary>
    public char PeakGlyph { get; set; } = '█';

    /// <summary>Gets current root nodes.</summary>
    public IReadOnlyList<TreeMapNode> Nodes => _roots;

    /// <summary>Gets selected leaf index, or <c>-1</c> when there is no selection.</summary>
    public int SelectedIndex { get; private set; } = -1;

    /// <summary>Gets selected leaf node, if any.</summary>
    public TreeMapNode? SelectedNode =>
        SelectedIndex >= 0 && SelectedIndex < _leaves.Count ? _leaves[SelectedIndex].Node : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>Occurs when selected leaf node changes.</summary>
    public event EventHandler<ListSelectionChangedEventArgs<TreeMapNode?>>? SelectionChanged;

    /// <summary>Replaces current root nodes.</summary>
    /// <param name="nodes">Nodes to render.</param>
    public void SetNodes(IEnumerable<TreeMapNode> nodes)
    {
        ArgumentNullException.ThrowIfNull(nodes);
        var previousIndex = SelectedIndex;
        var previousNode = SelectedNode;
        _roots.Clear();
        foreach (var node in nodes)
        {
            _roots.Add(Clone(node));
        }

        RebuildLeaves();
        NormalizeSelection();
        _hoveredIndex = -1;
        RaiseSelectionChangedIfNeeded(previousIndex, previousNode);
    }

    /// <summary>Clears all nodes and selection state.</summary>
    public void Clear()
    {
        var previousIndex = SelectedIndex;
        var previousNode = SelectedNode;
        _roots.Clear();
        _leaves.Clear();
        _rendered.Clear();
        SelectedIndex = -1;
        _hoveredIndex = -1;
        RaiseSelectionChangedIfNeeded(previousIndex, previousNode);
    }

    /// <summary>Sets selected leaf index using bounds clamping.</summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true" /> when selection changed.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_leaves.Count == 0)
        {
            return false;
        }

        var next = Math.Clamp(index, 0, _leaves.Count - 1);
        if (SelectedIndex == next)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousNode = SelectedNode;
        SelectedIndex = next;
        RaiseSelectionChangedIfNeeded(previousIndex, previousNode);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _leaves.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        var current = SelectedIndex < 0 ? 0 : SelectedIndex;
        if (key.Is(Key.Right) || key.Is(Key.Down) || key.IsCharacter('j') || key.IsCharacter('l'))
        {
            return SetSelectedIndex(current + 1);
        }

        if (key.Is(Key.Left) || key.Is(Key.Up) || key.IsCharacter('k') || key.IsCharacter('h'))
        {
            return SetSelectedIndex(current - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        return key.Is(Key.End) && SetSelectedIndex(_leaves.Count - 1);
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (!BuildLayout(content, ShowLegend, out _, out _))
        {
            return Handle(message);
        }

        var hit = FindHitIndex(pointer.X, pointer.Y);
        return pointer.Kind switch
        {
            PointerEventKind.Motion => SetHoveredIndex(hit),
            PointerEventKind.Wheel when _leaves.Count > 0 => HandleWheel(pointer.Button),
            PointerEventKind.Press when pointer.Button == PointerButton.Left && hit >= 0 => HandlePress(hit),
            PointerEventKind.Press or PointerEventKind.Release when hit < 0 => SetHoveredIndex(-1),
            _ => false
        };
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped,
            Border == BorderStyle.None ? null : RenderTitle(), Border, Padding, ResolveBorderStyle());
        if (!BuildLayout(content, ShowLegend, out var min, out var max))
        {
            WriteText(canvas, content.X, content.Y, EmptyText, ResolveStyle(EmptyStyle), content.Width);
            return;
        }

        NormalizeSelection();
        foreach (var node in _rendered)
        {
            var (glyph, bandStyle) = ResolveBand(node.Weight, min, max);
            var stateStyle = ResolveStateStyle(node.LeafIndex);
            var fillStyle = ResolveStyle(NodeStyle.Merge(bandStyle).Merge(stateStyle));
            DrawFill(canvas, node.Bounds, glyph, fillStyle);
            if (ShowLabels && node.Bounds is { Width: > 1, Height: > 0 })
            {
                WriteText(canvas, node.Bounds.X, node.Bounds.Y, node.Node.Name,
                    ResolveStyle(LabelStyle.Merge(stateStyle)), node.Bounds.Width);
            }
        }

        if (ShowLegend && content.Height > 1)
        {
            RenderLegend(canvas, content.X, content.Bottom - 1, content.Width);
        }
    }

    /// <inheritdoc />
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var titleWidth = ControlTextLayout.MeasureDisplayWidth(MeasureTitle());
        var width = Math.Max(14, titleWidth + 4) + Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        var height = 8 + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private void RenderLegend(Canvas canvas, int x, int y, int width)
    {
        var cursor = x;
        LegendBand(canvas, x, ref cursor, y, width, LowGlyph, "low", LowNodeStyle);
        LegendBand(canvas, x, ref cursor, y, width, MidGlyph, "mid", MidNodeStyle);
        LegendBand(canvas, x, ref cursor, y, width, HighGlyph, "high", HighNodeStyle);
        LegendBand(canvas, x, ref cursor, y, width, PeakGlyph, "peak", PeakNodeStyle);
    }

    private void LegendBand(Canvas canvas, int start, ref int cursor, int y, int width, char glyph, string label,
        TesseraStyle style)
    {
        if (cursor >= start + width)
        {
            return;
        }

        var remaining = Math.Max(0, width - (cursor - start));
        var text = string.Concat(CharText(glyph), " ", label, " ");
        WriteText(canvas, cursor, y, text, ResolveStyle(LabelStyle.Merge(style)), remaining);
        cursor += text.Length;
    }

    private bool BuildLayout(Rect content, bool includeLegend, out double min, out double max)
    {
        _rendered.Clear();
        min = 0d;
        max = 1d;
        if (content.IsEmpty || _leaves.Count == 0)
        {
            return false;
        }

        var plot = includeLegend && content.Height > 1
            ? new Rect(content.X, content.Y, content.Width, content.Height - 1)
            : content;
        if (plot.IsEmpty)
        {
            return false;
        }

        LayoutRange(plot, _leaves, 0, _leaves.Count, plot.Width >= plot.Height, _rendered);
        if (_rendered.Count == 0)
        {
            return false;
        }

        min = double.PositiveInfinity;
        max = double.NegativeInfinity;
        foreach (var weight in _rendered.Select(static node => node.Weight))
        {
            if (weight < min)
            {
                min = weight;
            }

            if (weight > max)
            {
                max = weight;
            }
        }

        if (double.IsPositiveInfinity(min))
        {
            min = 0d;
            max = 1d;
        }
        else if (Math.Abs(max - min) < double.Epsilon)
        {
            max = min + 1d;
        }

        return true;
    }

    private static void LayoutRange(Rect bounds, List<LeafNode> leaves, int start, int count, bool horizontal,
        List<RenderedNode> output)
    {
        while (count > 0 && !bounds.IsEmpty)
        {
            if (count == 1 || bounds is { Width: 1, Height: 1 })
            {
                output.Add(new RenderedNode(leaves[start].Node, bounds, leaves[start].Weight, leaves[start].LeafIndex));
                return;
            }

            var total = 0d;
            for (var index = 0; index < count; index++)
            {
                total += Math.Max(0d, leaves[start + index].Weight);
            }

            if (total <= 0d)
            {
                output.Add(new RenderedNode(leaves[start].Node, bounds, leaves[start].Weight, leaves[start].LeafIndex));
                return;
            }

            var split = start;
            var leftWeight = 0d;
            var half = total / 2d;
            while (split < start + count - 1)
            {
                leftWeight += Math.Max(0d, leaves[split].Weight);
                if (leftWeight >= half)
                {
                    break;
                }

                split++;
            }

            var leftCount = split - start + 1;
            if (leftCount <= 0 || leftCount >= count)
            {
                output.Add(new RenderedNode(leaves[start].Node, bounds, leaves[start].Weight, leaves[start].LeafIndex));
                return;
            }

            if (horizontal && bounds.Width > 1)
            {
                var firstWidth =
                    Math.Clamp((int)Math.Round(bounds.Width * (leftWeight / total), MidpointRounding.AwayFromZero), 1,
                        bounds.Width - 1);
                LayoutRange(new Rect(bounds.X, bounds.Y, firstWidth, bounds.Height), leaves, start, leftCount,
                    !horizontal, output);
                bounds = new Rect(bounds.X + firstWidth, bounds.Y, bounds.Width - firstWidth, bounds.Height);
                start += leftCount;
                count -= leftCount;
                horizontal = !horizontal;
                continue;
            }

            if (bounds.Height > 1)
            {
                var firstHeight =
                    Math.Clamp((int)Math.Round(bounds.Height * (leftWeight / total), MidpointRounding.AwayFromZero), 1,
                        bounds.Height - 1);
                LayoutRange(new Rect(bounds.X, bounds.Y, bounds.Width, firstHeight), leaves, start, leftCount,
                    !horizontal, output);
                bounds = new Rect(bounds.X, bounds.Y + firstHeight, bounds.Width, bounds.Height - firstHeight);
                start += leftCount;
                count -= leftCount;
                horizontal = !horizontal;
                continue;
            }

            output.Add(new RenderedNode(leaves[start].Node, bounds, leaves[start].Weight, leaves[start].LeafIndex));
            return;
        }
    }

    private void RebuildLeaves()
    {
        _leaves.Clear();
        var index = 0;
        foreach (var root in _roots)
        {
            CollectLeaves(root, ref index);
        }
    }

    private void CollectLeaves(TreeMapNode node, ref int index)
    {
        if (!node.HasChildren)
        {
            var weight = node.ResolveWeight();
            if (weight > 0d)
            {
                _leaves.Add(new LeafNode(node, weight, index++));
            }

            return;
        }

        var before = _leaves.Count;
        foreach (var child in node.Children)
        {
            CollectLeaves(child, ref index);
        }

        if (_leaves.Count > before)
        {
            return;
        }

        var resolvedWeight = node.ResolveWeight();
        if (resolvedWeight > 0d)
        {
            _leaves.Add(new LeafNode(node, resolvedWeight, index++));
        }
    }

    private void NormalizeSelection()
    {
        if (_leaves.Count == 0)
        {
            SelectedIndex = -1;
            _hoveredIndex = -1;
            return;
        }

        SelectedIndex = Math.Clamp(SelectedIndex < 0 ? 0 : SelectedIndex, 0, _leaves.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _leaves.Count - 1);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, TreeMapNode? previousNode)
    {
        if (previousIndex == SelectedIndex)
        {
            return;
        }

        SelectionChanged?.Invoke(this,
            new ListSelectionChangedEventArgs<TreeMapNode?>(previousIndex, SelectedIndex, previousNode, SelectedNode));
    }

    private int FindHitIndex(int x, int y)
    {
        return _rendered
            .Where(node => node.Bounds.Contains(x, y))
            .Select(static node => node.LeafIndex)
            .DefaultIfEmpty(-1)
            .First();
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private (char Glyph, TesseraStyle Style) ResolveBand(double value, double min, double max)
    {
        var normalized = Math.Clamp((value - min) / (max - min), 0d, 1d);
        return normalized switch
        {
            <= 0.25d => (LowGlyph, LowNodeStyle),
            <= 0.5d => (MidGlyph, MidNodeStyle),
            <= 0.75d => (HighGlyph, HighNodeStyle),
            _ => (PeakGlyph, PeakNodeStyle)
        };
    }

    private TesseraStyle ResolveStateStyle(int leafIndex)
    {
        var style = TesseraStyle.Empty;
        if (leafIndex == _hoveredIndex)
        {
            style = style.Merge(HoveredNodeStyle);
        }

        if (leafIndex == SelectedIndex)
        {
            style = style.Merge(SelectedNodeStyle);
        }

        if (leafIndex == SelectedIndex && IsFocused)
        {
            style = style.Merge(FocusedSelectedNodeStyle);
        }

        return style;
    }

    private TesseraStyle ResolveStyle(TesseraStyle style)
    {
        return IsDisabled ? style.Merge(DisabledNodeStyle) : style;
    }

    private TesseraStyle ResolveBorderStyle()
    {
        return ResolveStyle(IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText);
    }

    private string RenderTitle()
    {
        return Style(MeasureTitle(), IsFocused ? FocusedTitleStyle : TitleStyle);
    }

    private string MeasureTitle()
    {
        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? string.Concat(Title, " ", FocusMarker)
            : Title;
    }

    private static TreeMapNode Clone(TreeMapNode node)
    {
        var clone = new TreeMapNode(node.Name, node.Value);
        foreach (var child in node.Children)
        {
            clone.Children.Add(Clone(child));
        }

        return clone;
    }

    private bool HandleWheel(PointerButton button)
    {
        var current = SelectedIndex < 0 ? 0 : SelectedIndex;
        return button switch
        {
            PointerButton.WheelDown => SetSelectedIndex(current + 1),
            PointerButton.WheelUp => SetSelectedIndex(current - 1),
            _ => false
        };
    }

    private bool HandlePress(int hit)
    {
        RequestFocus();
        var changed = SetHoveredIndex(hit);
        return SetSelectedIndex(hit) || changed;
    }

    private static void DrawFill(Canvas canvas, Rect bounds, char glyph, TesseraStyle style)
    {
        if (style.IsEmpty)
        {
            for (var row = 0; row < bounds.Height; row++)
            {
                canvas.DrawHorizontalLine(bounds.X, bounds.Y + row, bounds.Width, glyph);
            }

            return;
        }

        var token = style.Render(CharText(glyph));
        for (var row = 0; row < bounds.Height; row++)
        {
            for (var column = 0; column < bounds.Width; column++)
            {
                canvas.WriteText(bounds.X + column, bounds.Y + row, token, 1);
            }
        }
    }

    private static void WriteText(Canvas canvas, int x, int y, string text, TesseraStyle style, int width)
    {
        canvas.WriteText(x, y, Style(text, style), width);
    }

    private static string Style(string text, TesseraStyle style)
    {
        return style.IsEmpty ? text : style.Render(text);
    }

    private static string CharText(char glyph)
    {
        return glyph switch { '░' => "░", '▒' => "▒", '▓' => "▓", '█' => "█", _ => glyph.ToString() };
    }

    private readonly record struct LeafNode(TreeMapNode Node, double Weight, int LeafIndex);

    private readonly record struct RenderedNode(TreeMapNode Node, Rect Bounds, double Weight, int LeafIndex);
}
