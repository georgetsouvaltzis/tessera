using TeaSharp;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal sealed class DashboardNavigationTabs : Control
{
    private readonly Tabs _tabs;

    public DashboardNavigationTabs(params string[] items)
    {
        _tabs = new Tabs(items);
        _tabs.SelectionChanged += (_, args) => SelectionChanged?.Invoke(this, args);
    }

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged;

    public int SelectedIndex => _tabs.SelectedIndex;

    public string Title
    {
        get => _tabs.Title;
        set => _tabs.Title = value ?? string.Empty;
    }

    public string FocusMarker
    {
        get => _tabs.FocusMarker;
        set => _tabs.FocusMarker = value ?? string.Empty;
    }

    public bool ShowFocusMarker
    {
        get => _tabs.ShowFocusMarker;
        set => _tabs.ShowFocusMarker = value;
    }

    public TeaStyle TitleStyle
    {
        get => _tabs.TitleStyle;
        set => _tabs.TitleStyle = value;
    }

    public TeaStyle FocusedTitleStyle
    {
        get => _tabs.FocusedTitleStyle;
        set => _tabs.FocusedTitleStyle = value;
    }

    public override bool IsFocused
    {
        get => _tabs.IsFocused;
        set => _tabs.IsFocused = value;
    }

    public override bool IsDisabled
    {
        get => _tabs.IsDisabled;
        set => _tabs.IsDisabled = value;
    }

    public override bool IsReadOnly
    {
        get => _tabs.IsReadOnly;
        set => _tabs.IsReadOnly = value;
    }

    public void SetItems(IEnumerable<string> items)
    {
        _tabs.SetItems(items);
    }

    public bool SetSelectedIndex(int index)
    {
        return _tabs.SetSelectedIndex(index);
    }

    public override bool Handle(Message message)
    {
        return _tabs.Handle(message);
    }

    public override bool Handle(Message message, Rect bounds)
    {
        if (message is PointerInput { Kind: PointerEventKind.Wheel } pointer)
        {
            return bounds.Contains(pointer.X, pointer.Y) && pointer.Y == bounds.Y;
        }

        return _tabs.Handle(message, bounds);
    }

    public override void Render(Canvas canvas, Rect rect)
    {
        _tabs.Render(canvas, rect);
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        return _tabs.Measure(availableBounds);
    }
}
