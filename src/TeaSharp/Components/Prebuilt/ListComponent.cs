using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Components.Internal;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed class ListComponent<T> : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private int? _hoveredFilteredIndex;

    public ListComponent(IEnumerable<T> items, Func<T, string> toText)
    {
        Model = new ListModel<T>(items, toText);
    }

    public ListComponent(ListOptions<T> options)
        : this(options.Items, options.ToText)
    {
        Title = options.Title;
        Focused = options.Focused;
        Disabled = options.Disabled;
        ReadOnly = options.ReadOnly;
        ShowBorder = options.ShowBorder;
        KeyMap = options.KeyMap ?? ListKeyMap.Default;
    }

    public ListModel<T> Model { get; }

    public string Title { get; set; } = "List";

    public bool Focused { get; set; }

    public bool Disabled { get; set; }

    public bool ReadOnly { get; set; }

    public bool ShowBorder { get; set; } = true;

    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public Func<T, IReadOnlyCollection<WidgetVisualState>?>? ItemStateResolver { get; set; }

    public ListKeyMap KeyMap { get; set; } = ListKeyMap.Default;

    public bool Update(IMessage message)
    {
        if (Disabled)
        {
            return false;
        }

        return Model.Update(message, KeyMap);
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (Disabled)
        {
            return false;
        }
        return ListComponentMouseRouter.Update(message, bounds, ShowBorder, Model, SetHoveredFilteredIndex);
    }

    public void Render(Canvas canvas, Rect rect)
    {
        ListComponentRenderer.Render(
            canvas,
            rect,
            Model,
            Title,
            Focused,
            Disabled,
            ReadOnly,
            ShowBorder,
            _hoveredFilteredIndex,
            ItemStatePalette,
            ItemStateResolver);
    }

    private bool SetHoveredFilteredIndex(int? filteredIndex)
    {
        if (_hoveredFilteredIndex == filteredIndex)
        {
            return false;
        }

        _hoveredFilteredIndex = filteredIndex;
        return true;
    }
}
