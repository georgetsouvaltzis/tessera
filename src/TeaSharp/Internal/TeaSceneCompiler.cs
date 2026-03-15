using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Core.Abstractions;
using TeaSharp.Layout;

namespace TeaSharp.Internal;

internal sealed class TeaSceneCompiler : IScreenCompiler
{
    private string? _focusedRegionId;

    public ScreenRenderResult Compile(ScreenContent content, ScreenContext context, ScreenOptions options)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (content.Layout is null)
        {
            var textOutput = new ScreenOutput(ScreenFrame.From(content.Text ?? string.Empty))
            {
                Terminal = options.ToTerminalOutput(),
            };

            return new ScreenRenderResult(textOutput, null);
        }

        var canvas = context.CreateCanvas(CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var builder = new TeaSceneBuilder(_focusedRegionId);
        if (!builder.TryBuild(content.Layout, canvas.Bounds, "root"))
        {
            throw new InvalidOperationException(
                $"TeaSceneCompiler does not support layout node '{content.Layout.GetType().FullName}'.");
        }

        var interaction = builder.Build(focusedRegionId => _focusedRegionId = focusedRegionId);
        interaction.Render(canvas);
        _focusedRegionId = interaction.FocusedRegionId;

        var output = new ScreenOutput(ScreenFrame.From(canvas.Render()))
        {
            Terminal = options.ToTerminalOutput(),
        };

        return new ScreenRenderResult(output, interaction.HasInteraction ? interaction : null);
    }

    private sealed class TeaSceneBuilder
    {
        private readonly string? _previousFocusedRegionId;
        private readonly List<TeaSceneRegion> _regions = [];
        private string? _requestedFocusRegionId;
        private long _requestedFocusOrder;
        private string? _implicitFocusRegionId;

        public TeaSceneBuilder(string? previousFocusedRegionId)
        {
            _previousFocusedRegionId = previousFocusedRegionId;
        }

        public bool TryBuild(LayoutNode layout, in Rect bounds, string path)
        {
            return layout switch
            {
                WindowLayout window => TryBuildWindow(window, bounds, path),
                RowLayout row => TryBuildStack(true, row.Items.ToArray(), row.Gap, row.Padding, bounds, path),
                ColumnLayout column => TryBuildStack(false, column.Items.ToArray(), column.Gap, column.Padding, bounds, path),
                CenterLayout center => TryBuildCenter(center, bounds, path),
                PanelLayout panel => TryBuildPanel(panel, bounds, path),
                OverlayLayout overlay => TryBuildOverlay(overlay, bounds, path),
                DockLayout dock => TryBuildDock(dock, bounds, path),
                StackLayout stack => TryBuildStack(stack.IsHorizontal, stack.Children, stack.Gap, stack.Padding, bounds, path),
                SplitLayout split => TryBuildStack(split.IsHorizontal, [split.First, split.Second], split.Gap, split.Padding, bounds, path),
                ComponentLayout component => TryBuildComponent(component, bounds, path),
                _ => false,
            };
        }

        public TeaSceneCompiledScreen Build(Action<string?> trackFocus)
        {
            return new TeaSceneCompiledScreen(
                _regions,
                _previousFocusedRegionId,
                _requestedFocusRegionId,
                _requestedFocusOrder,
                _implicitFocusRegionId,
                trackFocus);
        }

        private bool TryBuildWindow(WindowLayout window, in Rect bounds, string path)
        {
            var inner = Rect.Intersect(bounds.Inset(window.Padding), bounds);
            if (inner.IsEmpty)
            {
                return true;
            }

            var working = inner;

            if (window.Header is { } header)
            {
                var measured = ResolveSlotExtent(header, horizontal: false, working);
                var outer = new Rect(working.X, working.Y, working.Width, measured);
                if (!TryBuildDockSlot(header, outer, horizontal: false, $"{path}/header"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y + measured + window.Gap, working.Width, Math.Max(0, working.Height - measured - window.Gap));
            }

            if (window.Footer is { } footer && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(footer, horizontal: false, working);
                var outer = new Rect(working.X, Math.Max(working.Y, working.Bottom - measured), working.Width, measured);
                if (!TryBuildDockSlot(footer, outer, horizontal: false, $"{path}/footer"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y, working.Width, Math.Max(0, working.Height - measured - window.Gap));
            }

            if (window.Left is { } left && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(left, horizontal: true, working);
                var outer = new Rect(working.X, working.Y, measured, working.Height);
                if (!TryBuildDockSlot(left, outer, horizontal: true, $"{path}/left"))
                {
                    return false;
                }

                working = new Rect(working.X + measured + window.Gap, working.Y, Math.Max(0, working.Width - measured - window.Gap), working.Height);
            }

            if (window.Right is { } right && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(right, horizontal: true, working);
                var outer = new Rect(Math.Max(working.X, working.Right - measured), working.Y, measured, working.Height);
                if (!TryBuildDockSlot(right, outer, horizontal: true, $"{path}/right"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y, Math.Max(0, working.Width - measured - window.Gap), working.Height);
            }

            if (window.Body is { } body && !working.IsEmpty && !TryBuild(body, working, $"{path}/body"))
            {
                return false;
            }

            return window.Overlay is null || TryBuild(window.Overlay, inner, $"{path}/overlay");
        }

        private bool TryBuildOverlay(OverlayLayout overlay, in Rect bounds, string path)
        {
            for (var index = 0; index < overlay.Items.Count; index++)
            {
                if (!TryBuild(overlay.Items[index], bounds, $"{path}/overlay:{index}"))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryBuildPanel(PanelLayout panel, in Rect bounds, string path)
        {
            var outer = Rect.Intersect(bounds.Inset(panel.Margin), bounds);
            if (outer.IsEmpty)
            {
                return true;
            }

            if (panel.Border != BorderStyle.None)
            {
                _regions.Add(new TeaSceneRegion(
                    $"{path}/panel",
                    outer,
                    (canvas, rect) => canvas.DrawBox(rect, panel.Title, panel.Border),
                    null,
                    null,
                    false,
                    false,
                    false,
                    0,
                    null,
                    null));
            }

            var contentRect = FrameLayout.ResolveContentRect(outer, panel.Border, panel.Padding);
            return contentRect.IsEmpty || TryBuild(panel.Content, contentRect, $"{path}/content");
        }

        private bool TryBuildCenter(CenterLayout center, in Rect bounds, string path)
        {
            var inner = Rect.Intersect(bounds.Inset(center.Margin), bounds);
            if (inner.IsEmpty)
            {
                return true;
            }

            var measured = center.Content.Measure(inner);
            var width = Math.Clamp(center.Width ?? measured.Width, 0, inner.Width);
            var height = Math.Clamp(center.Height ?? measured.Height, 0, inner.Height);
            var x = inner.X + Math.Max(0, (inner.Width - width) / 2);
            var y = inner.Y + Math.Max(0, (inner.Height - height) / 2);
            return TryBuild(center.Content, new Rect(x, y, width, height), $"{path}/center");
        }

        private bool TryBuildDock(DockLayout dock, in Rect bounds, string path)
        {
            var inner = Rect.Intersect(bounds.Inset(dock.Padding), bounds);
            if (inner.IsEmpty)
            {
                return true;
            }

            var working = inner;

            if (dock.Top is { } top)
            {
                var measured = ResolveSlotExtent(top, horizontal: false, working);
                var outer = new Rect(working.X, working.Y, working.Width, measured);
                if (!TryBuildDockSlot(top, outer, horizontal: false, $"{path}/top"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y + measured + dock.Gap, working.Width, Math.Max(0, working.Height - measured - dock.Gap));
            }

            if (dock.Bottom is { } bottom && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(bottom, horizontal: false, working);
                var outer = new Rect(working.X, Math.Max(working.Y, working.Bottom - measured), working.Width, measured);
                if (!TryBuildDockSlot(bottom, outer, horizontal: false, $"{path}/bottom"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y, working.Width, Math.Max(0, working.Height - measured - dock.Gap));
            }

            if (dock.Left is { } left && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(left, horizontal: true, working);
                var outer = new Rect(working.X, working.Y, measured, working.Height);
                if (!TryBuildDockSlot(left, outer, horizontal: true, $"{path}/left"))
                {
                    return false;
                }

                working = new Rect(working.X + measured + dock.Gap, working.Y, Math.Max(0, working.Width - measured - dock.Gap), working.Height);
            }

            if (dock.Right is { } right && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(right, horizontal: true, working);
                var outer = new Rect(Math.Max(working.X, working.Right - measured), working.Y, measured, working.Height);
                if (!TryBuildDockSlot(right, outer, horizontal: true, $"{path}/right"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y, Math.Max(0, working.Width - measured - dock.Gap), working.Height);
            }

            return dock.Fill is null || working.IsEmpty || TryBuildDockSlot(dock.Fill, working, horizontal: true, $"{path}/fill");
        }

        private bool TryBuildStack(bool horizontal, IReadOnlyList<LayoutSlot> children, int gap, Thickness padding, in Rect bounds, string path)
        {
            var inner = Rect.Intersect(bounds.Inset(padding), bounds);
            if (inner.IsEmpty || children.Count == 0)
            {
                return true;
            }

            var primaryAvailable = horizontal ? inner.Width : inner.Height;
            var crossAvailable = horizontal ? inner.Height : inner.Width;
            var gapTotal = children.Count > 1 ? gap * (children.Count - 1) : 0;
            var primarySizes = new int[children.Count];
            var flexibleWeights = new int[children.Count];
            var remaining = Math.Max(0, primaryAvailable - gapTotal);

            for (var index = 0; index < children.Count; index++)
            {
                var marginPrimary = horizontal ? children[index].Margin.Horizontal : children[index].Margin.Vertical;
                remaining = Math.Max(0, remaining - marginPrimary);
            }

            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];
                switch (child.Length.Kind)
                {
                    case LayoutLengthKind.Fixed:
                        primarySizes[index] = Math.Clamp(child.Length.Value, 0, remaining);
                        remaining = Math.Max(0, remaining - primarySizes[index]);
                        break;
                    case LayoutLengthKind.Auto:
                        var measured = child.Content.Measure(inner);
                        var autoSize = horizontal ? measured.Width : measured.Height;
                        primarySizes[index] = Math.Clamp(autoSize, 0, remaining);
                        remaining = Math.Max(0, remaining - primarySizes[index]);
                        break;
                    case LayoutLengthKind.Fill:
                        flexibleWeights[index] = 1;
                        break;
                    case LayoutLengthKind.Weighted:
                        flexibleWeights[index] = Math.Max(1, child.Length.Value);
                        break;
                }
            }

            var totalWeight = flexibleWeights.Sum();
            if (totalWeight > 0 && remaining > 0)
            {
                var assigned = 0;
                for (var index = 0; index < children.Count; index++)
                {
                    var weight = flexibleWeights[index];
                    if (weight <= 0)
                    {
                        continue;
                    }

                    var share = (remaining * weight) / totalWeight;
                    primarySizes[index] = share;
                    assigned += share;
                }

                var leftover = remaining - assigned;
                for (var index = 0; index < children.Count && leftover > 0; index++)
                {
                    if (flexibleWeights[index] <= 0)
                    {
                        continue;
                    }

                    primarySizes[index]++;
                    leftover--;
                }
            }

            var cursorX = inner.X;
            var cursorY = inner.Y;
            for (var index = 0; index < children.Count; index++)
            {
                var child = children[index];
                var margin = child.Margin;
                var primarySize = primarySizes[index];
                var totalPrimary = primarySize + (horizontal ? margin.Horizontal : margin.Vertical);
                var cross = Math.Max(0, crossAvailable - (horizontal ? margin.Vertical : margin.Horizontal));
                Rect childBounds;
                if (horizontal)
                {
                    childBounds = new Rect(cursorX + margin.Left, inner.Y + margin.Top, Math.Max(0, primarySize), Math.Max(0, cross));
                    cursorX += totalPrimary + gap;
                }
                else
                {
                    childBounds = new Rect(inner.X + margin.Left, cursorY + margin.Top, Math.Max(0, cross), Math.Max(0, primarySize));
                    cursorY += totalPrimary + gap;
                }

                if (!childBounds.IsEmpty && !TryBuild(child.Content, childBounds, $"{path}/slot:{index}"))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryBuildDockSlot(LayoutSlot slot, in Rect outerBounds, bool horizontal, string path)
        {
            var bounds = new Rect(
                outerBounds.X + slot.Margin.Left,
                outerBounds.Y + slot.Margin.Top,
                Math.Max(0, outerBounds.Width - slot.Margin.Horizontal),
                Math.Max(0, outerBounds.Height - slot.Margin.Vertical));

            return bounds.IsEmpty || TryBuild(slot.Content, bounds, path);
        }

        private bool TryBuildComponent(ComponentLayout component, in Rect bounds, string path)
        {
            if (bounds.IsEmpty)
            {
                return true;
            }

            Action<Canvas, Rect> render;
            Func<Message, bool>? update = null;
            Func<PointerInput, Rect, bool>? updateMouse = null;
            Action<bool>? setFocused = null;
            var focusable = false;
            var focusOnClick = false;
            var interceptsPointer = false;
            var requestedFocus = false;
            var requestOrder = 0L;

            if (component.Control is { } control)
            {
                render = control.Render;
                update = control.Handle;
                updateMouse = control.Handle;
                setFocused = control.ApplyFocus;
                focusable = control.CanFocus;
                focusOnClick = true;
                interceptsPointer = true;
                requestedFocus = control.IsFocused;

                if (control.TryConsumeFocusRequest(out var explicitOrder))
                {
                    requestedFocus = true;
                    requestOrder = explicitOrder;
                }
            }
            else
            {
                render = component.CanvasComponent!.Render;
            }

            var id = $"{path}/component";
            _regions.Add(new TeaSceneRegion(
                id,
                bounds,
                render,
                update,
                updateMouse,
                focusable,
                focusOnClick,
                interceptsPointer,
                0,
                setFocused,
                null));

            if (requestedFocus)
            {
                if (requestOrder > 0)
                {
                    if (_requestedFocusRegionId is null || requestOrder >= _requestedFocusOrder)
                    {
                        _requestedFocusRegionId = id;
                        _requestedFocusOrder = requestOrder;
                    }
                }
                else
                {
                    _implicitFocusRegionId = id;
                }
            }

            return true;
        }

        private static int ResolveSlotExtent(LayoutSlot slot, bool horizontal, in Rect availableBounds)
        {
            var marginPrimary = horizontal ? slot.Margin.Horizontal : slot.Margin.Vertical;
            var measured = slot.Content.Measure(availableBounds);
            var availablePrimary = horizontal ? availableBounds.Width : availableBounds.Height;
            var measuredPrimary = horizontal ? measured.Width : measured.Height;
            var content = slot.Length.Kind switch
            {
                LayoutLengthKind.Fixed => slot.Length.Value,
                LayoutLengthKind.Weighted => Math.Max(0, (availablePrimary - marginPrimary) * slot.Length.Value),
                LayoutLengthKind.Fill => Math.Max(0, availablePrimary - marginPrimary),
                _ => measuredPrimary,
            };

            return Math.Clamp(content + marginPrimary, 0, availablePrimary);
        }
    }

    private sealed class TeaSceneCompiledScreen : ICompiledScreenInteraction
    {
        private readonly IReadOnlyList<TeaSceneRegion> _regions;
        private readonly Action<string?> _trackFocus;

        public TeaSceneCompiledScreen(
            IReadOnlyList<TeaSceneRegion> regions,
            string? previousFocusedRegionId,
            string? requestedFocusRegionId,
            long requestedFocusOrder,
            string? implicitFocusRegionId,
            Action<string?> trackFocus)
        {
            _regions = regions;
            _trackFocus = trackFocus;
            FocusedRegionId = ResolveInitialFocus(previousFocusedRegionId, requestedFocusRegionId, requestedFocusOrder, implicitFocusRegionId);
            ApplyFocus(FocusedRegionId, invokeFocus: false);
            HasInteraction = _regions.Any(region => region.Update is not null || region.UpdateMouse is not null || region.Focusable);
        }

        public string? FocusedRegionId { get; private set; }

        public bool HasInteraction { get; }

        public void Render(Canvas canvas)
        {
            foreach (var region in _regions.OrderBy(static region => region.Layer))
            {
                region.Render(canvas, region.Bounds);
            }
        }

        public bool Handle(Message message)
        {
            ArgumentNullException.ThrowIfNull(message);

            if (message is KeyPressed key)
            {
                if (key.Is(Key.Tab, ModifierKeys.Shift))
                {
                    return FocusRelative(-1);
                }

                if (key.Is(Key.Tab))
                {
                    return FocusRelative(1);
                }
            }

            if (message is PointerInput pointer)
            {
                return UpdateMouse(pointer);
            }

            return TryGetFocusedRegion(out var focused) && focused.Update is not null && focused.Update(message);
        }

        private bool UpdateMouse(PointerInput message)
        {
            var changed = false;
            var targetIndex = FindTopMostRegion(message.X, message.Y);
            if (targetIndex < 0 && message.Kind == PointerEventKind.Wheel && TryGetFocusedRegionIndex(out var focusedIndex))
            {
                targetIndex = focusedIndex;
            }

            if (targetIndex < 0)
            {
                return false;
            }

            var target = _regions[targetIndex];
            if (message is { Kind: PointerEventKind.Press, Button: PointerButton.Left } && target.Focusable && target.FocusOnClick)
            {
                changed |= ApplyFocus(target.Id, invokeFocus: true);
            }

            if (target.UpdateMouse is not null)
            {
                changed |= target.UpdateMouse(message, target.Bounds);
            }

            return changed;
        }

        private bool FocusRelative(int step)
        {
            if (_regions.Count == 0)
            {
                return false;
            }

            var startIndex = TryGetFocusedRegionIndex(out var focusedIndex)
                ? focusedIndex
                : step > 0 ? -1 : _regions.Count;

            var targetIndex = FindFocusableIndex(startIndex, step);
            return targetIndex >= 0 && ApplyFocus(_regions[targetIndex].Id, invokeFocus: true);
        }

        private string? ResolveInitialFocus(string? previousFocusedRegionId, string? requestedFocusRegionId, long requestedFocusOrder, string? implicitFocusRegionId)
        {
            if (requestedFocusOrder > 0 && requestedFocusRegionId is not null && TryGetFocusableRegionIndex(requestedFocusRegionId, out _))
            {
                return requestedFocusRegionId;
            }

            if (implicitFocusRegionId is not null && TryGetFocusableRegionIndex(implicitFocusRegionId, out _))
            {
                return implicitFocusRegionId;
            }

            if (previousFocusedRegionId is not null && TryGetFocusableRegionIndex(previousFocusedRegionId, out _))
            {
                return previousFocusedRegionId;
            }

            var firstFocusable = FindFocusableIndex(-1, 1);
            return firstFocusable >= 0 ? _regions[firstFocusable].Id : null;
        }

        private bool ApplyFocus(string? regionId, bool invokeFocus)
        {
            var matched = false;
            foreach (var region in _regions)
            {
                var shouldFocus = region.Focusable && region.Id == regionId;
                region.ApplyFocus(shouldFocus, invokeFocus && shouldFocus);
                matched |= shouldFocus;
            }

            FocusedRegionId = matched ? regionId : null;
            _trackFocus(FocusedRegionId);
            return matched;
        }

        private int FindTopMostRegion(int x, int y)
        {
            TeaSceneRegion? best = null;
            var bestIndex = -1;
            for (var index = 0; index < _regions.Count; index++)
            {
                var region = _regions[index];
                if (!region.Bounds.Contains(x, y) || !region.InterceptsPointer)
                {
                    continue;
                }

                if (best is null || region.Layer >= best.Layer)
                {
                    best = region;
                    bestIndex = index;
                }
            }

            return bestIndex;
        }

        private int FindFocusableIndex(int startIndex, int step)
        {
            for (var offset = 1; offset <= _regions.Count; offset++)
            {
                var index = startIndex + (offset * step);
                if (index < 0)
                {
                    index += _regions.Count;
                }
                else if (index >= _regions.Count)
                {
                    index -= _regions.Count;
                }

                if (_regions[index].Focusable)
                {
                    return index;
                }
            }

            return -1;
        }

        private bool TryGetFocusableRegionIndex(string regionId, out int index)
        {
            for (var i = 0; i < _regions.Count; i++)
            {
                if (_regions[i].Focusable && _regions[i].Id == regionId)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        private bool TryGetFocusedRegion(out TeaSceneRegion region)
        {
            if (FocusedRegionId is not null && TryGetFocusableRegionIndex(FocusedRegionId, out var focusedIndex))
            {
                region = _regions[focusedIndex];
                return true;
            }

            region = null!;
            return false;
        }

        private bool TryGetFocusedRegionIndex(out int index)
        {
            if (FocusedRegionId is not null)
            {
                return TryGetFocusableRegionIndex(FocusedRegionId, out index);
            }

            index = -1;
            return false;
        }
    }

    private sealed record TeaSceneRegion(
        string Id,
        Rect Bounds,
        Action<Canvas, Rect> Render,
        Func<Message, bool>? Update,
        Func<PointerInput, Rect, bool>? UpdateMouse,
        bool Focusable,
        bool FocusOnClick,
        bool InterceptsPointer,
        int Layer,
        Action<bool>? SetFocused,
        Action? OnFocus)
    {
        public void ApplyFocus(bool focused, bool invokeFocus)
        {
            SetFocused?.Invoke(focused);

            if (focused && invokeFocus)
            {
                OnFocus?.Invoke();
            }
        }
    }
}
