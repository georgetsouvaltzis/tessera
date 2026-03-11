using TeaSharp.Components.Advanced.Internal;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components.Advanced;

public sealed class CommandPaletteComponent : IStatefulComponent, IMouseStatefulComponent, IFocusableComponent
{
    private readonly CommandPaletteController _controller = new();
    private WidgetInteractionProfile _interactionProfile = WidgetInteractionProfile.Default.Clone();
    private long _executionVersion;
    private long _consumedExecutionVersion;

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public TextInputModel Query { get; } = new();

    public TextInputKeyMap QueryKeyMap { get; set; } = TextInputKeyMap.Default;

    public string QueryText => Query.Value;

    public string Title { get; set; } = "Command Palette";

    public bool Focused { get; set; }

    public bool IsOpen { get; private set; }

    public int MaxVisibleItems { get; set; } = 8;

    public string? LastExecutedItemId { get; private set; }

    /// <summary>
    /// Raised when a command-palette item is executed.
    /// </summary>
    public event EventHandler<CommandPaletteItemExecutedEventArgs>? ItemExecuted;

    public KeyBinding OpenKey { get; set; } = new("ctrl+p", "open", "ctrl+p");

    public KeyBinding CloseKey { get; set; } = new("esc", "close", "escape");

    public KeyBinding NextItemKey { get; set; } = new("down/j", "next item", "down", "j");

    public KeyBinding PreviousItemKey { get; set; } = new("up/k", "previous item", "up", "k");

    public KeyBinding ExecuteKey { get; set; } = new("enter", "execute", "enter");

    public WidgetStatePalette ItemStatePalette { get; } = WidgetStatePalette.CreateDefault();

    public WidgetInteractionProfile InteractionProfile
    {
        get => _interactionProfile;
        set => _interactionProfile = WidgetInteractionProfile.CloneOrDefault(value);
    }

    /// <summary>
    /// Consumes the latest command execution exactly once.
    /// </summary>
    public bool TryConsumeExecution(out string itemId)
    {
        if (_executionVersion == _consumedExecutionVersion || string.IsNullOrEmpty(LastExecutedItemId))
        {
            itemId = string.Empty;
            return false;
        }

        _consumedExecutionVersion = _executionVersion;
        itemId = LastExecutedItemId;
        return true;
    }

    public void SetItems(IEnumerable<CommandPaletteItem> items)
    {
        _controller.SetItems(items, Query.Value);
    }

    public void SetQueryText(string query)
    {
        Query.SetValue(query ?? string.Empty);
        _controller.Refresh(Query.Value);
    }

    public void ClearQuery()
    {
        SetQueryText(string.Empty);
    }

    public void Open()
    {
        if (IsOpen)
        {
            return;
        }

        IsOpen = true;
        ClearQuery();
    }

    public void Close()
    {
        IsOpen = false;
    }

    public bool Update(IMessage message)
    {
        if (!Focused)
        {
            return false;
        }

        if (!IsOpen)
        {
            if (message is KeyPressMsg openKey && OpenKey.Matches(openKey))
            {
                Open();
                return true;
            }

            return false;
        }

        if (message is KeyPressMsg key)
        {
            if (CloseKey.Matches(key))
            {
                Close();
                return true;
            }

            if (NextItemKey.Matches(key) && _controller.FilteredCount > 0)
            {
                _controller.MoveNext();
                return true;
            }

            if (PreviousItemKey.Matches(key) && _controller.FilteredCount > 0)
            {
                _controller.MovePrevious();
                return true;
            }

            if (ExecuteKey.Matches(key))
            {
                return ExecuteSelected();
            }
        }

        var inputResult = Query.Update(message, QueryKeyMap);
        if (inputResult.Changed)
        {
            _controller.Refresh(Query.Value);
            return true;
        }

        return inputResult.Submitted && ExecuteSelected();
    }

    public bool UpdateMouse(MouseMsg message, Rect bounds)
    {
        if (!IsOpen || !CommandPaletteLayout.TryResolveModal(bounds, out var modal, out var content))
        {
            return false;
        }

        var insideModal = modal.Contains(message.X, message.Y);
        var changed = false;
        if (!insideModal)
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= _controller.SetHovered(-1);
            }

            if (message is MouseClickMsg { Button: MouseButton.Left } && InteractionProfile.ActivateOnClick)
            {
                Close();
                changed = true;
            }

            return changed;
        }

        if (message is MouseWheelMsg wheel && InteractionProfile.NavigateOnWheel && _controller.FilteredCount > 0)
        {
            if (wheel.Button == MouseButton.WheelDown)
            {
                _controller.MoveNext();
                changed = true;
            }
            else if (wheel.Button == MouseButton.WheelUp)
            {
                _controller.MovePrevious();
                changed = true;
            }
        }

        if (!content.Contains(message.X, message.Y))
        {
            if (message is MouseMotionMsg or MouseClickMsg)
            {
                changed |= _controller.SetHovered(-1);
            }

            return changed;
        }

        var hovered = RowToFilteredIndex(content, message.Y);
        if (message is MouseMotionMsg && InteractionProfile.HoverOnMotion)
        {
            changed |= _controller.SetHovered(hovered);
            return changed;
        }

        if (message is MouseClickMsg click)
        {
            if (InteractionProfile.HoverOnClick)
            {
                changed |= _controller.SetHovered(hovered);
            }

            if (click.Button == MouseButton.Left && InteractionProfile.ActivateOnClick && hovered >= 0)
            {
                changed |= _controller.SetSelectedFilteredIndex(hovered);
                changed |= ExecuteSelected();
            }
        }

        return changed;
    }

    public void Render(Canvas canvas, Rect rect)
    {
        if (!IsOpen)
        {
            return;
        }

        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (!CommandPaletteLayout.TryResolveModal(clipped, out var modal, out var content))
        {
            return;
        }

        canvas.DrawBox(modal, Title, BorderStyle.Rounded);

        var queryWidth = Math.Max(1, content.Width - 2);
        var frame = Query.BuildFrame(queryWidth);
        canvas.WriteText(content.X, content.Y, $"> {frame.Text}", content.Width);
        if (content.Height <= 1)
        {
            return;
        }

        if (_controller.FilteredCount == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, ItemStatePalette.Render("(no commands)", WidgetVisualState.Empty), content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = OptionListViewport.ComputeWindowStart(_controller.SelectedFilteredIndex, visibleRows, _controller.FilteredCount);
        var end = Math.Min(_controller.FilteredCount, start + visibleRows);
        var row = 0;
        for (var filteredIndex = start; filteredIndex < end; filteredIndex++, row++)
        {
            var item = _controller.GetFilteredItem(filteredIndex);
            var marker = filteredIndex == _controller.SelectedFilteredIndex ? ">" : " ";
            var summary = string.IsNullOrWhiteSpace(item.Description)
                ? item.Title
                : $"{item.Title} - {item.Description}";
            canvas.WriteText(content.X, content.Y + 1 + row, ItemStatePalette.Render($"{marker} {summary}", ResolveStates(filteredIndex, item)), content.Width);
        }
    }

    private List<WidgetVisualState> ResolveStates(int filteredIndex, CommandPaletteItem item)
    {
        var states = new List<WidgetVisualState>(4);
        if (filteredIndex == _controller.SelectedFilteredIndex)
        {
            states.Add(WidgetVisualState.Cursor);
            states.Add(WidgetVisualState.Selected);
        }

        if (filteredIndex == _controller.HoveredFilteredIndex)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (item.States is not null)
        {
            states.AddRange(item.States);
        }

        return states;
    }

    private bool ExecuteSelected()
    {
        var selected = _controller.GetSelectedItem();
        if (selected is null)
        {
            Close();
            return true;
        }

        LastExecutedItemId = selected.Id;
        _executionVersion++;
        ItemExecuted?.Invoke(this, new CommandPaletteItemExecutedEventArgs(selected));
        Close();
        return true;
    }

    private int RowToFilteredIndex(Rect content, int y)
    {
        return OptionListViewport.RowToVisibleIndex(content, y, MaxVisibleItems, _controller.FilteredCount, _controller.SelectedFilteredIndex);
    }
}
