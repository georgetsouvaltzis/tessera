using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Examples.TransitBoard;

internal sealed class TransitDepartureBoardControl : Control
{
    private readonly List<TransitService> _services = [];
    private int _selectedIndex;
    private int _scrollOffset;
    private int _lastViewportRows = 4;

    public event EventHandler<TransitServiceChangedEventArgs>? SelectionChanged;

    public string Title { get; set; } = "Live Board";
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DividerStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle EmptyStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle PrimaryTextStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SecondaryTextStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SelectedRowStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SelectedSecondaryStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DelayStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle WarningStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SuccessStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle PlatformStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle RouteStyle { get; set; } = TeaStyle.Empty;
    public string FocusMarker { get; set; } = "◆";

    public TransitService? SelectedService => _selectedIndex >= 0 && _selectedIndex < _services.Count ? _services[_selectedIndex] : null;

    public void SetServices(IEnumerable<TransitService> services)
    {
        ArgumentNullException.ThrowIfNull(services);
        var previous = SelectedService;
        _services.Clear();
        _services.AddRange(services);
        _selectedIndex = Math.Clamp(_selectedIndex, 0, Math.Max(0, _services.Count - 1));

        if (previous?.Id != SelectedService?.Id)
        {
            SelectionChanged?.Invoke(this, new TransitServiceChangedEventArgs(previous, SelectedService));
        }
    }

    public bool SelectService(string? serviceId)
    {
        if (string.IsNullOrWhiteSpace(serviceId))
        {
            return false;
        }

        var index = _services.FindIndex(service => string.Equals(service.Id, serviceId, StringComparison.Ordinal));
        return SetSelectedIndex(index);
    }

    public override bool Handle(Message message)
    {
        if (!IsFocused || _services.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.PageDown))
        {
            return SetSelectedIndex(Math.Min(_services.Count - 1, _selectedIndex + Math.Max(1, _lastViewportRows - 1)));
        }

        if (key.Is(Key.PageUp))
        {
            return SetSelectedIndex(Math.Max(0, _selectedIndex - Math.Max(1, _lastViewportRows - 1)));
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_services.Count - 1);
        }

        return false;
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer)
        {
            return Handle(message);
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1);
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1);
            }
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && bounds.Contains(pointer.X, pointer.Y))
        {
            RequestFocus();
            var row = pointer.Y - bounds.Y - 2;
            if (row < 0)
            {
                return false;
            }

            var serviceIndex = _scrollOffset + (row / 3);
            return SetSelectedIndex(serviceIndex);
        }

        return Handle(message);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = IsFocused ? $"{Title} {FocusMarker}" : Title;
        canvas.WriteText(clipped.X, clipped.Y, Render(IsFocused ? FocusedTitleStyle : TitleStyle, title), clipped.Width);
        if (clipped.Height > 1)
        {
            canvas.WriteText(clipped.X, clipped.Y + 1, Render(DividerStyle, new string('─', clipped.Width)), clipped.Width);
        }

        if (_services.Count == 0)
        {
            if (clipped.Height > 2)
            {
                canvas.WriteText(clipped.X, clipped.Y + 2, Render(EmptyStyle, "No live services in this slice."), clipped.Width);
            }
            return;
        }

        var rowHeight = 3;
        _lastViewportRows = Math.Max(1, (clipped.Height - 2) / rowHeight);
        EnsureVisible();

        for (var row = 0; row < _lastViewportRows; row++)
        {
            var index = _scrollOffset + row;
            if (index >= _services.Count)
            {
                break;
            }

            RenderRow(canvas, clipped, row, _services[index], index == _selectedIndex);
        }
    }

    private void RenderRow(Canvas canvas, Rect rect, int rowIndex, TransitService service, bool selected)
    {
        var y = rect.Y + 2 + (rowIndex * 3);
        if (y >= rect.Bottom)
        {
            return;
        }

        var primaryStyle = selected ? SelectedRowStyle : PrimaryTextStyle;
        var secondaryStyle = selected ? SelectedSecondaryStyle : SecondaryTextStyle;
        var markerStyle = ResolveMarkerStyle(service, selected);
        var routeStyle = selected ? SelectedRowStyle : RouteStyle;
        var platformStyle = selected ? SelectedRowStyle : PlatformStyle;

        var lineOne = $"{Render(primaryStyle, service.DisplayTime.PadRight(6))} {Render(routeStyle, $"[{service.RouteCode}]")} {Render(primaryStyle, service.Destination)}";
        var rightText = $"{Render(platformStyle, $"PF {service.Platform}")}  {Render(markerStyle, StatusText(service))}";
        canvas.WriteText(rect.X, y, lineOne, rect.Width);
        var rightX = Math.Max(rect.X, rect.Right - Math.Min(rightText.Length, rect.Width / 3));
        canvas.WriteText(rightX, y, rightText, rect.Right - rightX);

        if (y + 1 < rect.Bottom)
        {
            var lineTwo = $"{Render(secondaryStyle, service.Via)}  {Render(secondaryStyle, service.Concourse)}  {Render(markerStyle, service.MarkerText)}";
            canvas.WriteText(rect.X, y + 1, lineTwo, rect.Width);
        }

        if (y + 2 < rect.Bottom)
        {
            canvas.WriteText(rect.X, y + 2, Render(DividerStyle, new string('·', rect.Width)), rect.Width);
        }
    }

    private bool SetSelectedIndex(int index)
    {
        if (_services.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _services.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previous = SelectedService;
        _selectedIndex = clamped;
        SelectionChanged?.Invoke(this, new TransitServiceChangedEventArgs(previous, SelectedService));
        return true;
    }

    private void EnsureVisible()
    {
        if (_selectedIndex < _scrollOffset)
        {
            _scrollOffset = _selectedIndex;
            return;
        }

        if (_selectedIndex >= _scrollOffset + _lastViewportRows)
        {
            _scrollOffset = _selectedIndex - _lastViewportRows + 1;
        }
    }

    private TeaStyle ResolveMarkerStyle(TransitService service, bool selected)
    {
        if (selected)
        {
            return SelectedRowStyle;
        }

        return service.DelayMinutes switch
        {
            >= 8 => DelayStyle,
            > 0 => WarningStyle,
            _ when string.Equals(service.Status, "final call", StringComparison.OrdinalIgnoreCase) => WarningStyle,
            _ => SuccessStyle,
        };
    }

    private static string StatusText(TransitService service)
    {
        return service.DelayMinutes switch
        {
            > 0 => $"+{service.DelayMinutes:00}",
            _ when string.Equals(service.Status, "final call", StringComparison.OrdinalIgnoreCase) => "FINAL",
            _ when string.Equals(service.Status, "boarding", StringComparison.OrdinalIgnoreCase) => "BOARD",
            _ when service.IsArrival => "IN",
            _ => "OT",
        };
    }

    private static string Render(TeaStyle style, string text) => style.IsEmpty ? text : style.Render(text);
}

internal sealed record TransitServiceChangedEventArgs(TransitService? Previous, TransitService? Selected);
