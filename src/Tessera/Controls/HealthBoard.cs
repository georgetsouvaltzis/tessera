using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a selectable service health board for operational dashboards.
/// </summary>
public sealed partial class HealthBoard : Control
{
    private readonly List<HealthService> _services = [];
    private int _hoveredIndex = -1;
    private int _lastViewportRows = 8;
    private int _scrollOffset;

    /// <summary>
    ///     Gets or sets control title text.
    /// </summary>
    public string Title { get; set; } = "Health";

    /// <summary>
    ///     Gets or sets marker appended to title while focused and <see cref="ShowFocusMarker" /> is enabled.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets a value indicating whether <see cref="FocusMarker" /> is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets text rendered when no services are configured.
    /// </summary>
    public string EmptyText { get; set; } = "(no services)";

    /// <summary>
    ///     Gets or sets title style while not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets title style while focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets border glyph style while not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets border glyph style while focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets base row style.
    /// </summary>
    public TesseraStyle ServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged for healthy rows.
    /// </summary>
    public TesseraStyle HealthyServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged for degraded rows.
    /// </summary>
    public TesseraStyle DegradedServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged for outage rows.
    /// </summary>
    public TesseraStyle OutageServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged for hovered rows.
    /// </summary>
    public TesseraStyle HoveredServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged for selected rows.
    /// </summary>
    public TesseraStyle SelectedServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged for selected rows while focused.
    /// </summary>
    public TesseraStyle FocusedSelectedServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged for acknowledged rows.
    /// </summary>
    public TesseraStyle AcknowledgedServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged for muted rows.
    /// </summary>
    public TesseraStyle MutedServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged while disabled.
    /// </summary>
    public TesseraStyle DisabledServiceStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style for empty-state text.
    /// </summary>
    public TesseraStyle EmptyStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets frame border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    ///     Gets or sets inner frame padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    ///     Gets or sets glyphs used for row markers and severity symbols.
    /// </summary>
    public HealthBoardGlyphSet Glyphs { get; set; } = HealthBoardGlyphSet.Default;

    /// <summary>
    ///     Gets configured service rows.
    /// </summary>
    public IReadOnlyList<HealthService> Services => _services;

    /// <summary>
    ///     Gets selected row index, or <c>-1</c> when empty.
    /// </summary>
    public int SelectedIndex { get; private set; } = -1;

    /// <summary>
    ///     Gets selected service row, if any.
    /// </summary>
    public HealthService? SelectedItem =>
        SelectedIndex >= 0 && SelectedIndex < _services.Count
            ? _services[SelectedIndex]
            : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    ///     Occurs when selected service changes.
    /// </summary>
    public event EventHandler<ListSelectionChangedEventArgs<HealthService>>? SelectionChanged;

    /// <summary>
    ///     Replaces all services shown by the control.
    /// </summary>
    /// <param name="services">Service rows to render.</param>
    public void SetServices(IEnumerable<HealthService> services)
    {
        ArgumentNullException.ThrowIfNull(services);
        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;

        _services.Clear();
        foreach (var service in services)
        {
            _services.Add(Clone(service));
        }

        if (_services.Count == 0)
        {
            SelectedIndex = -1;
            _hoveredIndex = -1;
            _scrollOffset = 0;
        }
        else
        {
            SelectedIndex = Math.Clamp(SelectedIndex < 0 ? 0 : SelectedIndex, 0, _services.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _services.Count - 1);
            EnsureSelectionVisible(_lastViewportRows);
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    ///     Sets selected row index using bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_services.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _services.Count - 1);
        if (clamped == SelectedIndex)
        {
            return false;
        }

        var previousIndex = SelectedIndex;
        var previousItem = SelectedItem;
        SelectedIndex = clamped;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseSelectionChanged(previousIndex, previousItem, SelectedIndex, SelectedItem);
        return true;
    }

    /// <summary>
    ///     Compatibility alias for <see cref="SetSelectedIndex" />.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool Select(int index)
    {
        return SetSelectedIndex(index);
    }

    /// <summary>
    ///     Marks a service as acknowledged.
    /// </summary>
    /// <param name="serviceId">Service identifier to acknowledge.</param>
    /// <returns><see langword="true" /> when service was found and changed; otherwise <see langword="false" />.</returns>
    public bool Acknowledge(string serviceId)
    {
        ArgumentNullException.ThrowIfNull(serviceId);
        for (var index = 0; index < _services.Count; index++)
        {
            if (!string.Equals(_services[index].Id, serviceId, StringComparison.Ordinal))
            {
                continue;
            }

            if (_services[index].IsAcknowledged)
            {
                return false;
            }

            _services[index].IsAcknowledged = true;
            return true;
        }

        return false;
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

    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_services.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }

        if (SelectedIndex < 0)
        {
            SelectedIndex = 0;
        }

        if (SelectedIndex < _scrollOffset)
        {
            _scrollOffset = SelectedIndex;
        }
        else if (SelectedIndex >= _scrollOffset + viewportRows)
        {
            _scrollOffset = SelectedIndex - viewportRows + 1;
        }

        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _services.Count - viewportRows));
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, HealthService? previousItem)
    {
        var selectedIndex = SelectedIndex;
        var selectedItem = SelectedItem;
        if (previousIndex == selectedIndex && IsSameService(previousItem, selectedItem))
        {
            return;
        }

        RaiseSelectionChanged(previousIndex, previousItem, selectedIndex, selectedItem);
    }

    private void RaiseSelectionChanged(int previousIndex, HealthService? previousItem, int selectedIndex,
        HealthService? selectedItem)
    {
        SelectionChanged?.Invoke(this,
            new ListSelectionChangedEventArgs<HealthService>(previousIndex, selectedIndex, previousItem, selectedItem));
    }

    private static bool IsSameService(HealthService? left, HealthService? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return string.Equals(left.Id, right.Id, StringComparison.Ordinal);
    }

    private static HealthService Clone(HealthService service)
    {
        return new HealthService(service.Id, service.Name, service.Severity, service.Summary, service.ObservedAt)
        {
            IsAcknowledged = service.IsAcknowledged,
            IsMuted = service.IsMuted
        };
    }
}
