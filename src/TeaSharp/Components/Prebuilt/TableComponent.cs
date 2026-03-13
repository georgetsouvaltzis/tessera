using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using System.ComponentModel;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Prebuilt;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class TableComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly SortableTableComponent _inner;

    public TableComponent(IReadOnlyList<string> headers)
    {
        _inner = new SortableTableComponent(headers);
    }

    public TableComponent(TableOptions options)
        : this(options.Headers)
    {
        Title = options.Title;
        IsFocused = options.IsFocused;
        Border = options.Border;
        Padding = options.Padding;
        if (options.PageSize.HasValue)
        {
            PageSize = options.PageSize.Value;
        }
    }

    public bool IsFocused { get; set; }

    public BorderStyle Border
    {
        get => _inner.Border;
        set => _inner.Border = value;
    }

    public Thickness Padding
    {
        get => _inner.Padding;
        set => _inner.Padding = value;
    }

    public string Title
    {
        get => _inner.Title;
        set => _inner.Title = value;
    }

    public int PageSize
    {
        get => _inner.PageSize;
        set => _inner.PageSize = value;
    }

    public int PageIndex => _inner.PageIndex;

    public int SortColumn => _inner.SortColumn;

    public bool SortDescending => _inner.SortDescending;

    public bool EnableVirtualization
    {
        get => _inner.EnableVirtualization;
        set => _inner.EnableVirtualization = value;
    }

    public int VirtualStartIndex => _inner.VirtualStartIndex;

    public int VirtualWindowSize => _inner.VirtualWindowSize;

    public WidgetInteractionProfile InteractionProfile
    {
        get => _inner.InteractionProfile;
        set => _inner.InteractionProfile = value;
    }

    public KeyBinding NextPageKey
    {
        get => _inner.NextPageKey;
        set => _inner.NextPageKey = value;
    }

    public KeyBinding PreviousPageKey
    {
        get => _inner.PreviousPageKey;
        set => _inner.PreviousPageKey = value;
    }

    public KeyBinding ToggleSortDirectionKey
    {
        get => _inner.ToggleSortDirectionKey;
        set => _inner.ToggleSortDirectionKey = value;
    }

    public KeyBinding NextSortColumnKey
    {
        get => _inner.NextSortColumnKey;
        set => _inner.NextSortColumnKey = value;
    }

    public KeyBinding VirtualForwardKey
    {
        get => _inner.VirtualForwardKey;
        set => _inner.VirtualForwardKey = value;
    }

    public KeyBinding VirtualBackwardKey
    {
        get => _inner.VirtualBackwardKey;
        set => _inner.VirtualBackwardKey = value;
    }

    public void SetRows(IEnumerable<IReadOnlyList<string>> rows)
    {
        _inner.SetRows(rows);
    }

    public void SetVirtualWindow(int startIndex, int windowSize)
    {
        _inner.SetVirtualWindow(startIndex, windowSize);
    }

    public bool Update(IMessage message)
    {
        return _inner.Update(message);
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        return _inner.UpdateMouse(message, bounds);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        var original = _inner.Title;
        _inner.Title = IsFocused ? $"{original} *" : original.Replace(" *", string.Empty, StringComparison.Ordinal);
        _inner.Render(canvas, rect);
        _inner.Title = original;
    }
}
