using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;
using TeaSharp;

internal sealed class DashboardNavigationTabs : Control
{
    private readonly Tabs _inner;
    private event EventHandler<SelectionChangedEventArgs>? SelectionChangedCore;
    private bool _suppressSelectionChanged;

    public DashboardNavigationTabs(params string[] items)
    {
        _inner = new Tabs(items);
        _inner.SelectionChanged += OnInnerSelectionChanged;
    }

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
    {
        add => SelectionChangedCore += value;
        remove => SelectionChangedCore -= value;
    }

    public IReadOnlyList<string> Items => _inner.Items;

    public int SelectedIndex => _inner.SelectedIndex;

    public string Title
    {
        get => _inner.Title;
        set => _inner.Title = value ?? string.Empty;
    }

    public string FocusMarker
    {
        get => _inner.FocusMarker;
        set => _inner.FocusMarker = value ?? string.Empty;
    }

    public bool ShowFocusMarker
    {
        get => _inner.ShowFocusMarker;
        set => _inner.ShowFocusMarker = value;
    }

    public TeaStyle TitleStyle
    {
        get => _inner.TitleStyle;
        set => _inner.TitleStyle = value;
    }

    public TeaStyle FocusedTitleStyle
    {
        get => _inner.FocusedTitleStyle;
        set => _inner.FocusedTitleStyle = value;
    }

    public override bool IsFocused
    {
        get => _inner.IsFocused;
        set => _inner.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => _inner.IsDisabled;
        set => _inner.IsDisabled = value;
    }

    public override bool IsReadOnly
    {
        get => _inner.IsReadOnly;
        set => _inner.IsReadOnly = value;
    }

    public void ApplyTheme(TeaTheme theme) => _inner.ApplyTheme(theme);

    public bool SetSelectedIndex(int index) => _inner.SetSelectedIndex(index);

    public override bool Handle(Message message)
    {
        if (message is PointerInput pointer && IsBlockedNavigationPointer(pointer))
        {
            return true;
        }

        return _inner.Handle(message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer)
        {
            return _inner.Handle(message, bounds);
        }

        if (IsBlockedNavigationPointer(pointer))
        {
            return true;
        }

        if (pointer.Kind == PointerEventKind.Motion)
        {
            var selectedBefore = _inner.SelectedIndex;
            _suppressSelectionChanged = true;
            try
            {
                var handled = _inner.Handle(message, bounds);
                if (_inner.SelectedIndex != selectedBefore)
                {
                    _inner.SetSelectedIndex(selectedBefore);
                    return true;
                }

                return handled;
            }
            finally
            {
                _suppressSelectionChanged = false;
            }
        }

        return _inner.Handle(message, bounds);
    }

    public override void Render(Canvas canvas, Rect rect) => _inner.Render(canvas, rect);

    private void OnInnerSelectionChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (_suppressSelectionChanged)
        {
            return;
        }

        SelectionChangedCore?.Invoke(this, args);
    }

    private static bool IsBlockedNavigationPointer(PointerInput pointer)
    {
        if (pointer.Kind == PointerEventKind.Wheel)
        {
            return true;
        }

        return pointer.Button is PointerButton.WheelDown
            or PointerButton.WheelUp
            or PointerButton.WheelLeft
            or PointerButton.WheelRight;
    }
}
