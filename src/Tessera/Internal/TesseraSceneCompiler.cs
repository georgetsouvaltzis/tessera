using System.Collections.Concurrent;
using System.Reflection;
using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls;
using Tessera.Core.Abstractions;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Internal;

internal sealed class TesseraSceneCompiler : IScreenCompiler
{
    private static readonly MethodInfo ListViewThemeDefaultsApplierFactoryMethodDefinition =
        ResolveListViewThemeDefaultsApplierFactoryMethodDefinition();

    private static readonly ConcurrentDictionary<Type, Action<Control, TesseraTheme>> ListViewThemeDefaultsAppliers =
        new();

    private static readonly MethodInfo ListViewThemeDefaultsWithOverridesApplierFactoryMethodDefinition =
        ResolveListViewThemeDefaultsWithOverridesApplierFactoryMethodDefinition();

    private static readonly
        ConcurrentDictionary<Type, Action<Control, TesseraThemeOverrides, TesseraTheme, TesseraThemeVisualState>>
        ListViewThemeDefaultsWithOverridesAppliers = new();

    private string? _focusedRegionId;

    public ScreenRenderResult Compile(ScreenContent content, ScreenContext context, ScreenOptions options)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (content.Layout is null)
        {
            var textOutput = new ScreenOutput(ScreenFrame.From(content.Text ?? string.Empty))
            {
                Terminal = options.ToTerminalOutput()
            };

            return new ScreenRenderResult(textOutput, null);
        }

        if (context.Theme is { } theme)
        {
            if (context.ThemeOverrides is { } overrides)
            {
                ApplyThemeDefaults(content.Layout, theme, overrides, context.HasFocus);
            }
            else
            {
                ApplyThemeDefaults(content.Layout, theme);
            }
        }

        var canvas = context.CreateCanvas(CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var builder = new TesseraSceneBuilder(_focusedRegionId);
        if (!builder.TryBuild(content.Layout, canvas.Bounds, "root"))
        {
            throw new InvalidOperationException(
                $"TesseraSceneCompiler does not support layout node '{content.Layout.GetType().FullName}'.");
        }

        var interaction = builder.Build(focusedRegionId => _focusedRegionId = focusedRegionId);
        interaction.Render(canvas);
        _focusedRegionId = interaction.FocusedRegionId;

        var output = new ScreenOutput(ScreenFrame.From(canvas.Render())) { Terminal = options.ToTerminalOutput() };

        return new ScreenRenderResult(output, interaction.HasInteraction ? interaction : null);
    }

    private static void ApplyThemeDefaults(LayoutNode layout, TesseraTheme theme)
    {
        switch (layout)
        {
            case ComponentLayout component when component.Control is { } control:
                ApplyThemeDefaults(control, theme);
                return;
            case WindowLayout window:
                ApplyThemeDefaults(window.Header, theme);
                ApplyThemeDefaults(window.Footer, theme);
                ApplyThemeDefaults(window.Left, theme);
                ApplyThemeDefaults(window.Right, theme);
                if (window.Body is not null)
                {
                    ApplyThemeDefaults(window.Body, theme);
                }

                if (window.Overlay is not null)
                {
                    ApplyThemeDefaults(window.Overlay, theme);
                }

                return;
            case RowLayout row:
                ApplyThemeDefaults(row.Items, theme);
                return;
            case ColumnLayout column:
                ApplyThemeDefaults(column.Items, theme);
                return;
            case CenterLayout center:
                ApplyThemeDefaults(center.Content, theme);
                return;
            case PanelLayout panel:
                ApplyThemeDefaults(panel.Content, theme);
                return;
            case OverlayLayout overlay:
                for (var index = 0; index < overlay.Items.Count; index++)
                {
                    ApplyThemeDefaults(overlay.Items[index], theme);
                }

                return;
            case DockLayout dock:
                ApplyThemeDefaults(dock.Top, theme);
                ApplyThemeDefaults(dock.Bottom, theme);
                ApplyThemeDefaults(dock.Left, theme);
                ApplyThemeDefaults(dock.Right, theme);
                ApplyThemeDefaults(dock.Fill, theme);
                return;
            case StackLayout stack:
                ApplyThemeDefaults(stack.Children, theme);
                return;
            case SplitLayout split:
                ApplyThemeDefaults(split.First, theme);
                ApplyThemeDefaults(split.Second, theme);
                return;
            default:
                return;
        }
    }

    private static void ApplyThemeDefaults(LayoutNode layout, TesseraTheme theme, TesseraThemeOverrides overrides,
        bool hasTerminalFocus)
    {
        switch (layout)
        {
            case ComponentLayout component when component.Control is { } control:
                ApplyThemeDefaults(control, theme, overrides, hasTerminalFocus);
                return;
            case WindowLayout window:
                ApplyThemeDefaults(window.Header, theme, overrides, hasTerminalFocus);
                ApplyThemeDefaults(window.Footer, theme, overrides, hasTerminalFocus);
                ApplyThemeDefaults(window.Left, theme, overrides, hasTerminalFocus);
                ApplyThemeDefaults(window.Right, theme, overrides, hasTerminalFocus);
                if (window.Body is not null)
                {
                    ApplyThemeDefaults(window.Body, theme, overrides, hasTerminalFocus);
                }

                if (window.Overlay is not null)
                {
                    ApplyThemeDefaults(window.Overlay, theme, overrides, hasTerminalFocus);
                }

                return;
            case RowLayout row:
                ApplyThemeDefaults(row.Items, theme, overrides, hasTerminalFocus);
                return;
            case ColumnLayout column:
                ApplyThemeDefaults(column.Items, theme, overrides, hasTerminalFocus);
                return;
            case CenterLayout center:
                ApplyThemeDefaults(center.Content, theme, overrides, hasTerminalFocus);
                return;
            case PanelLayout panel:
                ApplyThemeDefaults(panel.Content, theme, overrides, hasTerminalFocus);
                return;
            case OverlayLayout overlay:
                for (var index = 0; index < overlay.Items.Count; index++)
                {
                    ApplyThemeDefaults(overlay.Items[index], theme, overrides, hasTerminalFocus);
                }

                return;
            case DockLayout dock:
                ApplyThemeDefaults(dock.Top, theme, overrides, hasTerminalFocus);
                ApplyThemeDefaults(dock.Bottom, theme, overrides, hasTerminalFocus);
                ApplyThemeDefaults(dock.Left, theme, overrides, hasTerminalFocus);
                ApplyThemeDefaults(dock.Right, theme, overrides, hasTerminalFocus);
                ApplyThemeDefaults(dock.Fill, theme, overrides, hasTerminalFocus);
                return;
            case StackLayout stack:
                ApplyThemeDefaults(stack.Children, theme, overrides, hasTerminalFocus);
                return;
            case SplitLayout split:
                ApplyThemeDefaults(split.First, theme, overrides, hasTerminalFocus);
                ApplyThemeDefaults(split.Second, theme, overrides, hasTerminalFocus);
                return;
            default:
                return;
        }
    }

    private static void ApplyThemeDefaults(IEnumerable<LayoutSlot> slots, TesseraTheme theme)
    {
        foreach (var slot in slots)
        {
            ApplyThemeDefaults(slot, theme);
        }
    }

    private static void ApplyThemeDefaults(
        IEnumerable<LayoutSlot> slots,
        TesseraTheme theme,
        TesseraThemeOverrides overrides,
        bool hasTerminalFocus)
    {
        foreach (var slot in slots)
        {
            ApplyThemeDefaults(slot, theme, overrides, hasTerminalFocus);
        }
    }

    private static void ApplyThemeDefaults(LayoutSlot? slot, TesseraTheme theme)
    {
        if (slot is not null)
        {
            ApplyThemeDefaults(slot.Content, theme);
        }
    }

    private static void ApplyThemeDefaults(LayoutSlot? slot, TesseraTheme theme, TesseraThemeOverrides overrides,
        bool hasTerminalFocus)
    {
        if (slot is not null)
        {
            ApplyThemeDefaults(slot.Content, theme, overrides, hasTerminalFocus);
        }
    }

    private static void ApplyThemeDefaults(Control control, TesseraTheme theme)
    {
        switch (control)
        {
            case Button button:
                button.ApplyThemeDefaults(theme);
                return;
            case StatusBar statusBar:
                statusBar.ApplyThemeDefaults(theme);
                return;
            case TextInput textInput:
                textInput.ApplyThemeDefaults(theme);
                return;
            case Table table:
                table.ApplyThemeDefaults(theme);
                return;
            case Tabs tabs:
                tabs.ApplyThemeDefaults(theme);
                return;
            default:
                ApplyListViewThemeDefaults(control, theme);
                return;
        }
    }

    private static void ApplyThemeDefaults(Control control, TesseraTheme theme, TesseraThemeOverrides overrides,
        bool hasTerminalFocus)
    {
        var state = ResolveVisualState(control, hasTerminalFocus);

        switch (control)
        {
            case Button button:
                button.ApplyThemeDefaults(overrides, theme, state);
                return;
            case StatusBar statusBar:
                statusBar.ApplyThemeDefaults(overrides, theme, state);
                return;
            case TextInput textInput:
                textInput.ApplyThemeDefaults(overrides, theme, state);
                return;
            case Table table:
                table.ApplyThemeDefaults(overrides, theme, state);
                return;
            case Tabs tabs:
                tabs.ApplyThemeDefaults(overrides, theme, state);
                return;
            default:
                ApplyListViewThemeDefaults(control, overrides, theme, state);
                return;
        }
    }

    private static void ApplyListViewThemeDefaults(Control control, TesseraTheme theme)
    {
        var controlType = control.GetType();
        if (!controlType.IsGenericType || controlType.GetGenericTypeDefinition() != typeof(ListView<>))
        {
            return;
        }

        var itemType = controlType.GetGenericArguments()[0];
        var applier =
            ListViewThemeDefaultsAppliers.GetOrAdd(itemType, static value => CreateListViewThemeDefaultsApplier(value));
        applier(control, theme);
    }

    private static void ApplyListViewThemeDefaults(
        Control control,
        TesseraThemeOverrides overrides,
        TesseraTheme theme,
        TesseraThemeVisualState state)
    {
        var controlType = control.GetType();
        if (!controlType.IsGenericType || controlType.GetGenericTypeDefinition() != typeof(ListView<>))
        {
            return;
        }

        var itemType = controlType.GetGenericArguments()[0];
        var applier = ListViewThemeDefaultsWithOverridesAppliers.GetOrAdd(
            itemType,
            static value => CreateListViewThemeDefaultsWithOverridesApplier(value));
        applier(control, overrides, theme, state);
    }

    private static Action<Control, TesseraTheme> CreateListViewThemeDefaultsApplier(Type itemType)
    {
        var factory = ListViewThemeDefaultsApplierFactoryMethodDefinition.MakeGenericMethod(itemType);
        return (Action<Control, TesseraTheme>)(factory.Invoke(null, null)
                                               ?? throw new InvalidOperationException(
                                                   "Failed to build ListView<T> theme applier delegate."));
    }

    private static Action<Control, TesseraThemeOverrides, TesseraTheme, TesseraThemeVisualState>
        CreateListViewThemeDefaultsWithOverridesApplier(
            Type itemType)
    {
        var factory = ListViewThemeDefaultsWithOverridesApplierFactoryMethodDefinition.MakeGenericMethod(itemType);
        return (Action<Control, TesseraThemeOverrides, TesseraTheme, TesseraThemeVisualState>)(factory.Invoke(null,
                null)
            ?? throw new InvalidOperationException("Failed to build ListView<T> theme+override applier delegate."));
    }

    private static MethodInfo ResolveListViewThemeDefaultsApplierFactoryMethodDefinition()
    {
        return typeof(TesseraSceneCompiler).GetMethod(
                   nameof(CreateListViewThemeDefaultsApplierCore),
                   BindingFlags.Public | BindingFlags.Static)
               ?? throw new InvalidOperationException("Unable to resolve ListView<T> theme applier factory method.");
    }

    private static MethodInfo ResolveListViewThemeDefaultsWithOverridesApplierFactoryMethodDefinition()
    {
        return typeof(TesseraSceneCompiler).GetMethod(
                   nameof(CreateListViewThemeDefaultsWithOverridesApplierCore),
                   BindingFlags.Public | BindingFlags.Static)
               ?? throw new InvalidOperationException(
                   "Unable to resolve ListView<T> theme+override applier factory method.");
    }

    public static Action<Control, TesseraTheme> CreateListViewThemeDefaultsApplierCore<TItem>()
    {
        return static (control, theme) =>
        {
            if (control is ListView<TItem> listView)
            {
                _ = listView.ApplyThemeDefaults(theme);
            }
        };
    }

    public static Action<Control, TesseraThemeOverrides, TesseraTheme, TesseraThemeVisualState>
        CreateListViewThemeDefaultsWithOverridesApplierCore<TItem>()
    {
        return static (control, overrides, theme, state) =>
        {
            if (control is ListView<TItem> listView)
            {
                _ = listView.ApplyThemeDefaults(overrides, theme, state);
            }
        };
    }

    private static TesseraThemeVisualState ResolveVisualState(Control control, bool hasTerminalFocus)
    {
        if (control.IsDisabled)
        {
            return TesseraThemeVisualState.Disabled;
        }

        if (hasTerminalFocus && control.IsFocused)
        {
            return TesseraThemeVisualState.Focused;
        }

        return TesseraThemeVisualState.Default;
    }

    private sealed class TesseraSceneBuilder(string? previousFocusedRegionId)
    {
        private readonly string? _previousFocusedRegionId = previousFocusedRegionId;
        private readonly List<TesseraSceneRegion> _regions = [];
        private string? _implicitFocusRegionId;
        private long _requestedFocusOrder;
        private string? _requestedFocusRegionId;

        public bool TryBuild(LayoutNode layout, in Rect bounds, string path)
        {
            return layout switch
            {
                WindowLayout window => TryBuildWindow(window, bounds, path),
                RowLayout row => TryBuildStack(true, AsReadOnlyList(row.Items), row.Gap, row.Padding, bounds, path),
                ColumnLayout column => TryBuildStack(false, AsReadOnlyList(column.Items), column.Gap, column.Padding,
                    bounds, path),
                CenterLayout center => TryBuildCenter(center, bounds, path),
                PanelLayout panel => TryBuildPanel(panel, bounds, path),
                OverlayLayout overlay => TryBuildOverlay(overlay, bounds, path),
                DockLayout dock => TryBuildDock(dock, bounds, path),
                StackLayout stack => TryBuildStack(stack.IsHorizontal, stack.Children, stack.Gap, stack.Padding, bounds,
                    path),
                SplitLayout split => TryBuildStack(split.IsHorizontal, [split.First, split.Second], split.Gap,
                    split.Padding, bounds, path),
                ComponentLayout component => TryBuildComponent(component, bounds, path),
                _ => false
            };
        }

        public TesseraSceneCompiledScreen Build(Action<string?> trackFocus)
        {
            return new TesseraSceneCompiledScreen(
                _regions,
                _previousFocusedRegionId,
                _requestedFocusRegionId,
                _requestedFocusOrder,
                _implicitFocusRegionId,
                trackFocus);
        }

        private static IReadOnlyList<LayoutSlot> AsReadOnlyList(IList<LayoutSlot> slots)
        {
            return slots as IReadOnlyList<LayoutSlot> ?? [.. slots];
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
                var measured = ResolveSlotExtent(header, false, working);
                var outer = new Rect(working.X, working.Y, working.Width, measured);
                if (!TryBuildDockSlot(header, outer, $"{path}/header"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y + measured + window.Gap, working.Width,
                    Math.Max(0, working.Height - measured - window.Gap));
            }

            if (window.Footer is { } footer && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(footer, false, working);
                var outer = new Rect(working.X, Math.Max(working.Y, working.Bottom - measured), working.Width,
                    measured);
                if (!TryBuildDockSlot(footer, outer, $"{path}/footer"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y, working.Width,
                    Math.Max(0, working.Height - measured - window.Gap));
            }

            if (window.Left is { } left && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(left, true, working);
                var outer = new Rect(working.X, working.Y, measured, working.Height);
                if (!TryBuildDockSlot(left, outer, $"{path}/left"))
                {
                    return false;
                }

                working = new Rect(working.X + measured + window.Gap, working.Y,
                    Math.Max(0, working.Width - measured - window.Gap), working.Height);
            }

            if (window.Right is { } right && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(right, true, working);
                var outer = new Rect(Math.Max(working.X, working.Right - measured), working.Y, measured,
                    working.Height);
                if (!TryBuildDockSlot(right, outer, $"{path}/right"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y, Math.Max(0, working.Width - measured - window.Gap),
                    working.Height);
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
                _regions.Add(new TesseraSceneRegion(
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
                var measured = ResolveSlotExtent(top, false, working);
                var outer = new Rect(working.X, working.Y, working.Width, measured);
                if (!TryBuildDockSlot(top, outer, $"{path}/top"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y + measured + dock.Gap, working.Width,
                    Math.Max(0, working.Height - measured - dock.Gap));
            }

            if (dock.Bottom is { } bottom && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(bottom, false, working);
                var outer = new Rect(working.X, Math.Max(working.Y, working.Bottom - measured), working.Width,
                    measured);
                if (!TryBuildDockSlot(bottom, outer, $"{path}/bottom"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y, working.Width,
                    Math.Max(0, working.Height - measured - dock.Gap));
            }

            if (dock.Left is { } left && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(left, true, working);
                var outer = new Rect(working.X, working.Y, measured, working.Height);
                if (!TryBuildDockSlot(left, outer, $"{path}/left"))
                {
                    return false;
                }

                working = new Rect(working.X + measured + dock.Gap, working.Y,
                    Math.Max(0, working.Width - measured - dock.Gap), working.Height);
            }

            if (dock.Right is { } right && !working.IsEmpty)
            {
                var measured = ResolveSlotExtent(right, true, working);
                var outer = new Rect(Math.Max(working.X, working.Right - measured), working.Y, measured,
                    working.Height);
                if (!TryBuildDockSlot(right, outer, $"{path}/right"))
                {
                    return false;
                }

                working = new Rect(working.X, working.Y, Math.Max(0, working.Width - measured - dock.Gap),
                    working.Height);
            }

            return dock.Fill is null || working.IsEmpty || TryBuildDockSlot(dock.Fill, working, $"{path}/fill");
        }

        private bool TryBuildStack(bool horizontal, IReadOnlyList<LayoutSlot> children, int gap, Thickness padding,
            in Rect bounds, string path)
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

            var totalWeight = 0;
            for (var index = 0; index < flexibleWeights.Length; index++)
            {
                totalWeight += flexibleWeights[index];
            }

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

                    var share = remaining * weight / totalWeight;
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
                    childBounds = new Rect(cursorX + margin.Left, inner.Y + margin.Top, Math.Max(0, primarySize),
                        Math.Max(0, cross));
                    cursorX += totalPrimary + gap;
                }
                else
                {
                    childBounds = new Rect(inner.X + margin.Left, cursorY + margin.Top, Math.Max(0, cross),
                        Math.Max(0, primarySize));
                    cursorY += totalPrimary + gap;
                }

                if (!childBounds.IsEmpty && !TryBuild(child.Content, childBounds, $"{path}/slot:{index}"))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryBuildDockSlot(LayoutSlot slot, in Rect outerBounds, string path)
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
                var isInteractiveVisible = IsInteractiveVisible(control);
                render = control.Render;
                update = isInteractiveVisible ? control.Handle : null;
                updateMouse = isInteractiveVisible ? control.Handle : null;
                setFocused = control.ApplyFocus;
                focusable = isInteractiveVisible && control.CanFocus;
                focusOnClick = isInteractiveVisible;
                interceptsPointer = isInteractiveVisible;
                requestedFocus = isInteractiveVisible && control.IsFocused;

                if (isInteractiveVisible && control.TryConsumeFocusRequest(out var explicitOrder))
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
            _regions.Add(new TesseraSceneRegion(
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

        private static bool IsInteractiveVisible(Control control)
        {
            return control switch
            {
                Dialog dialog => dialog.IsVisible,
                Modal modal => modal.IsVisible,
                ContextMenu contextMenu => contextMenu.IsVisible,
                CommandPalette commandPalette => commandPalette.IsVisible,
                KeyBindingHelpDialog keyBindingHelpDialog => keyBindingHelpDialog.IsVisible,
                QuickOpenOverlay quickOpenOverlay => quickOpenOverlay.IsOpen,
                _ => true
            };
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
                _ => measuredPrimary
            };

            return Math.Clamp(content + marginPrimary, 0, availablePrimary);
        }
    }

    private sealed class TesseraSceneCompiledScreen : ICompiledScreenInteraction
    {
        private readonly IReadOnlyList<TesseraSceneRegion> _regions;
        private readonly int[]? _renderOrder;
        private readonly Action<string?> _trackFocus;

        public TesseraSceneCompiledScreen(
            IReadOnlyList<TesseraSceneRegion> regions,
            string? previousFocusedRegionId,
            string? requestedFocusRegionId,
            long requestedFocusOrder,
            string? implicitFocusRegionId,
            Action<string?> trackFocus)
        {
            _regions = regions;
            _trackFocus = trackFocus;
            _renderOrder = BuildRenderOrder(regions);
            FocusedRegionId = ResolveInitialFocus(previousFocusedRegionId, requestedFocusRegionId, requestedFocusOrder,
                implicitFocusRegionId);
            ApplyFocus(FocusedRegionId, false);
            HasInteraction = HasInteractiveRegions(_regions);
        }

        public string? FocusedRegionId { get; private set; }

        public bool HasInteraction { get; }

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

        public void Render(Canvas canvas)
        {
            if (_renderOrder is null)
            {
                for (var index = 0; index < _regions.Count; index++)
                {
                    var region = _regions[index];
                    region.Render(canvas, region.Bounds);
                }

                return;
            }

            for (var orderIndex = 0; orderIndex < _renderOrder.Length; orderIndex++)
            {
                var region = _regions[_renderOrder[orderIndex]];
                region.Render(canvas, region.Bounds);
            }
        }

        private bool UpdateMouse(PointerInput message)
        {
            var changed = false;
            var targetIndex = FindTopMostRegion(message.X, message.Y);
            if (targetIndex < 0 && message.Kind == PointerEventKind.Wheel &&
                TryGetFocusedRegionIndex(out var focusedIndex))
            {
                targetIndex = focusedIndex;
            }

            if (targetIndex < 0)
            {
                return false;
            }

            var target = _regions[targetIndex];
            if (ShouldApplyFocusOnPointer(message) && target.Focusable && target.FocusOnClick)
            {
                changed |= ApplyFocus(target.Id, true);
            }

            if (target.UpdateMouse is not null)
            {
                changed |= target.UpdateMouse(message, target.Bounds);
            }

            return changed;
        }

        private static bool ShouldApplyFocusOnPointer(PointerInput message)
        {
            if (message is { Kind: PointerEventKind.Press, Button: PointerButton.Left })
            {
                return true;
            }

            return message is { Kind: PointerEventKind.Motion, Button: PointerButton.None, ClickCount: 0 };
        }

        private bool FocusRelative(int step)
        {
            if (_regions.Count == 0)
            {
                return false;
            }

            var hasFocusedRegion = TryGetFocusedRegionIndex(out var focusedIndex);
            var startIndex = hasFocusedRegion ? focusedIndex : _regions.Count;
            if (!hasFocusedRegion && step > 0)
            {
                startIndex = -1;
            }

            var targetIndex = FindFocusableIndex(startIndex, step);
            return targetIndex >= 0 && ApplyFocus(_regions[targetIndex].Id, true);
        }

        private string? ResolveInitialFocus(string? previousFocusedRegionId, string? requestedFocusRegionId,
            long requestedFocusOrder, string? implicitFocusRegionId)
        {
            if (requestedFocusOrder > 0 && requestedFocusRegionId is not null &&
                TryGetFocusableRegionIndex(requestedFocusRegionId, out _))
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
            if (_renderOrder is null)
            {
                for (var index = _regions.Count - 1; index >= 0; index--)
                {
                    var region = _regions[index];
                    if (region.Bounds.Contains(x, y) && region.InterceptsPointer)
                    {
                        return index;
                    }
                }

                return -1;
            }

            for (var orderIndex = _renderOrder.Length - 1; orderIndex >= 0; orderIndex--)
            {
                var regionIndex = _renderOrder[orderIndex];
                var region = _regions[regionIndex];
                if (region.Bounds.Contains(x, y) && region.InterceptsPointer)
                {
                    return regionIndex;
                }
            }

            return -1;
        }

        private static bool HasInteractiveRegions(IReadOnlyList<TesseraSceneRegion> regions)
        {
            for (var index = 0; index < regions.Count; index++)
            {
                var region = regions[index];
                if (region.Update is not null || region.UpdateMouse is not null || region.Focusable)
                {
                    return true;
                }
            }

            return false;
        }

        private static int[]? BuildRenderOrder(IReadOnlyList<TesseraSceneRegion> regions)
        {
            if (regions.Count <= 1)
            {
                return null;
            }

            var sorted = true;
            var previousLayer = regions[0].Layer;
            for (var index = 1; index < regions.Count; index++)
            {
                var layer = regions[index].Layer;
                if (layer < previousLayer)
                {
                    sorted = false;
                    break;
                }

                previousLayer = layer;
            }

            if (sorted)
            {
                return null;
            }

            var order = new int[regions.Count];
            for (var index = 0; index < order.Length; index++)
            {
                order[index] = index;
            }

            Array.Sort(
                order,
                (left, right) =>
                {
                    var layerCompare = regions[left].Layer.CompareTo(regions[right].Layer);
                    return layerCompare != 0 ? layerCompare : left.CompareTo(right);
                });
            return order;
        }

        private int FindFocusableIndex(int startIndex, int step)
        {
            for (var offset = 1; offset <= _regions.Count; offset++)
            {
                var index = startIndex + offset * step;
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

        private bool TryGetFocusedRegion(out TesseraSceneRegion region)
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

    private sealed record TesseraSceneRegion(
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
