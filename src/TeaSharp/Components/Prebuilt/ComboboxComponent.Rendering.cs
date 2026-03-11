using TeaSharp.Components.UiKit;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt.Internal;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Components.Styling;
namespace TeaSharp.Components.Prebuilt;

public sealed partial class ComboboxComponent
{
    private Rect ResolveRenderContentRect(Canvas canvas, Rect clipped)
    {
        return FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : Focused ? $"{Title} *" : Title,
            Border,
            Padding);
    }

    private void RenderField(Canvas canvas, Rect content)
    {
        var frameWidth = Math.Max(1, content.Width - 2);
        var frame = _input.BuildFrame(frameWidth);
        var indicator = IsOpen ? "^" : "v";
        canvas.WriteText(
            content.X,
            content.Y,
            FieldStatePalette.Render($"{indicator} {frame.Text}", ResolveFieldStates()),
            content.Width);
    }

    private void RenderOpenOptions(Canvas canvas, Rect content)
    {
        if (!IsOpen || content.Height <= 1)
        {
            return;
        }

        if (_options.VisibleCount == 0)
        {
            canvas.WriteText(content.X, content.Y + 1, OptionStatePalette.Render("(no matches)", ResolveNoMatchStates()), content.Width);
            return;
        }

        var visibleRows = Math.Min(Math.Max(1, MaxVisibleItems), content.Height - 1);
        var start = OptionListViewport.ComputeWindowStart(_options.HighlightedVisibleIndex, visibleRows, _options.VisibleCount);
        var end = Math.Min(_options.VisibleCount, start + visibleRows);
        var row = 0;
        for (var visibleIndex = start; visibleIndex < end; visibleIndex++, row++)
        {
            var itemIndex = _options.VisibleItemIndexAt(visibleIndex);
            var highlight = visibleIndex == _options.HighlightedVisibleIndex ? ">" : " ";
            var selectedMarker = itemIndex == _options.SelectedIndex ? "*" : " ";
            var text = $"{highlight}{selectedMarker} {_options.Items[itemIndex]}";
            canvas.WriteText(content.X, content.Y + 1 + row, OptionStatePalette.Render(text, ResolveOptionStates(visibleIndex, itemIndex)), content.Width);
        }
    }

    private List<WidgetVisualState> ResolveFieldStates()
    {
        var states = new List<WidgetVisualState>(6);
        if (Focused)
        {
            states.Add(WidgetVisualState.Focused);
        }

        if (Disabled)
        {
            states.Add(WidgetVisualState.Disabled);
        }

        if (ReadOnly)
        {
            states.Add(WidgetVisualState.ReadOnly);
        }

        if (_options.Count == 0)
        {
            states.Add(WidgetVisualState.Empty);
        }

        if (!string.IsNullOrEmpty(_input.Value))
        {
            states.Add(WidgetVisualState.Editing);
        }

        if (_fieldHovered)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        return states;
    }

    private List<WidgetVisualState> ResolveNoMatchStates()
    {
        var states = new List<WidgetVisualState>(6);
        states.AddRange(ResolveFieldStates());
        states.Add(WidgetVisualState.Empty);
        if (!string.IsNullOrWhiteSpace(_input.Value))
        {
            states.Add(WidgetVisualState.FilteredOut);
        }

        return states;
    }

    private List<WidgetVisualState> ResolveOptionStates(int visibleIndex, int itemIndex)
    {
        var states = new List<WidgetVisualState>(7);
        states.AddRange(ResolveFieldStates());
        if (visibleIndex == _options.HighlightedVisibleIndex)
        {
            states.Add(WidgetVisualState.Cursor);
        }

        if (itemIndex == _options.SelectedIndex)
        {
            states.Add(WidgetVisualState.Selected);
        }

        if (visibleIndex == _options.HoveredVisibleIndex)
        {
            states.Add(WidgetVisualState.Hovered);
        }

        if (OptionStateResolver?.Invoke(_options.Items[itemIndex], itemIndex) is { } custom)
        {
            states.AddRange(custom);
        }

        return states;
    }
}
