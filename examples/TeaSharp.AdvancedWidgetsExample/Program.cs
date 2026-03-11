using TeaSharp.Components.Advanced;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.View;

var program = Tea.NewProgram(new AdvancedWidgetsModel(), new TeaProgramOptions
{
    UseConsoleKeyEvents = false,
});

try
{
    await program.RunAsync();
    return 0;
}
catch (TeaProgramInterruptedException)
{
    return 130;
}

internal sealed record AdvancedTickMsg(DateTimeOffset At) : IMessage;

internal enum AdvancedFocus
{
    Toggle = 0,
    Slider = 1,
    Spinner = 2,
    Tree = 3,
    Notifications = 4,
}

internal sealed class AdvancedWidgetsModel : IModel
{
    private readonly ToggleSwitchComponent _toggle = new()
    {
        Title = "Feature Flag",
    };

    private readonly SliderComponent _slider = new()
    {
        Title = "Concurrency",
        Min = 1,
        Max = 32,
        Step = 1,
    };

    private readonly SpinnerComponent _spinner = new()
    {
        Title = "Indexer",
        Label = "running",
    };

    private readonly BadgeComponent _badge = new()
    {
        Text = "warm",
    };

    private readonly TreeViewComponent _tree = new()
    {
        Title = "Workspace",
    };

    private readonly NotificationCenterComponent _notifications = new()
    {
        Title = "Notification Center",
    };

    private readonly CommandPaletteComponent _palette = new()
    {
        Title = "Command Palette",
        MaxVisibleItems = 9,
        Focused = true,
    };

    private readonly StatusBarComponent _status = new(new StatusBarOptions(
        Theme: new UiTheme(StatusFill: '·')));
    private AdvancedFocus _focus = AdvancedFocus.Toggle;
    private int _width = 120;
    private int _height = 40;
    private int _tickCount;
    private string _lastEvent = "ready";

    public AdvancedWidgetsModel()
    {
        _slider.SetValue(8);
        _tree.SetRoots(
        [
            new TreeItemNode("root", "TeaSharp")
            {
                States = { WidgetVisualState.Success },
            },
            new TreeItemNode("features", "Features",
            [
                new TreeItemNode("states", "State styling") { States = { WidgetVisualState.Completed } },
                new TreeItemNode("inheritance", "Palette inheritance") { States = { WidgetVisualState.Completed } },
                new TreeItemNode("advanced", "Advanced widgets"),
            ]),
            new TreeItemNode("releases", "Release")
            {
                States = { WidgetVisualState.Warning },
            },
        ]);

        _notifications.Push("bootstrap complete", NotificationSeverity.Success, "n1");
        _notifications.Push("press ctrl+p for palette", NotificationSeverity.Info, "n2");
        _notifications.Push("warn: update docs before release", NotificationSeverity.Warning, "n3");

        _palette.SetItems(
        [
            new CommandPaletteItem("notify.info", "Push info notification", "Adds an info event"),
            new CommandPaletteItem("notify.warn", "Push warning notification", "Adds a warning event", [WidgetVisualState.Warning]),
            new CommandPaletteItem("notify.error", "Push error notification", "Adds an error event", [WidgetVisualState.Error]),
            new CommandPaletteItem("feature.toggle", "Toggle feature flag", "Flip ON/OFF state"),
            new CommandPaletteItem("spinner.toggle", "Toggle spinner run state", "Pause or resume spinner"),
            new CommandPaletteItem("slider.max", "Set concurrency to max", "Move slider to maximum"),
            new CommandPaletteItem("tree.collapse", "Collapse selected tree node", "Acts like Left"),
            new CommandPaletteItem("tree.expand", "Expand selected tree node", "Acts like Right"),
            new CommandPaletteItem("notifications.clear", "Clear notifications", "Drops all events", [WidgetVisualState.Warning]),
        ]);

        SetFocus(AdvancedFocus.Toggle);
    }

    public Command? Init() => NextTick();

    public Command? Update(IMessage message)
    {
        if (message is AdvancedTickMsg)
        {
            _tickCount++;
            if (_spinner.Running)
            {
                _spinner.Advance();
            }

            if (_tickCount % 24 == 0)
            {
                _notifications.Push("heartbeat", NotificationSeverity.Info);
            }

            return NextTick();
        }

        if (message is WindowSizeMsg ws)
        {
            _width = ws.Width;
            _height = ws.Height;
            _lastEvent = $"resize:{_width}x{_height}";
            return null;
        }

        if (message is MouseMsg mouse)
        {
            var layout = GetInteractionRects();
            var mouseChanged = false;
            if (_palette.IsOpen)
            {
                var beforeCommand = _palette.LastExecutedItemId;
                mouseChanged |= _palette.UpdateMouse(mouse, layout.ContentRect);
                if (!string.Equals(beforeCommand, _palette.LastExecutedItemId, StringComparison.Ordinal))
                {
                    ExecutePaletteCommand(_palette.LastExecutedItemId);
                    mouseChanged = true;
                }
            }
            else if (layout.ToggleRect.Contains(mouse.X, mouse.Y)
                || (mouse is MouseWheelMsg && _focus == AdvancedFocus.Toggle))
            {
                mouseChanged |= _toggle.UpdateMouse(mouse, layout.ToggleRect);
                if (mouse is MouseClickMsg { Button: MouseButton.Left })
                {
                    SetFocus(AdvancedFocus.Toggle);
                    mouseChanged = true;
                }
            }
            else if (layout.SliderRect.Contains(mouse.X, mouse.Y)
                || (mouse is MouseWheelMsg && _focus == AdvancedFocus.Slider))
            {
                mouseChanged |= _slider.UpdateMouse(mouse, layout.SliderRect);
                if (mouse is MouseClickMsg { Button: MouseButton.Left })
                {
                    SetFocus(AdvancedFocus.Slider);
                    mouseChanged = true;
                }
            }
            else if (layout.SpinnerRect.Contains(mouse.X, mouse.Y)
                || (mouse is MouseWheelMsg && _focus == AdvancedFocus.Spinner))
            {
                mouseChanged |= _spinner.UpdateMouse(mouse, layout.SpinnerRect);
                if (mouse is MouseClickMsg { Button: MouseButton.Left })
                {
                    SetFocus(AdvancedFocus.Spinner);
                    mouseChanged = true;
                }
            }
            else if (layout.TreeRect.Contains(mouse.X, mouse.Y)
                || (mouse is MouseWheelMsg && _focus == AdvancedFocus.Tree))
            {
                mouseChanged |= _tree.UpdateMouse(mouse, layout.TreeRect);
                if (mouse is MouseClickMsg { Button: MouseButton.Left })
                {
                    SetFocus(AdvancedFocus.Tree);
                    mouseChanged = true;
                }
            }
            else if (layout.NotificationRect.Contains(mouse.X, mouse.Y)
                || (mouse is MouseWheelMsg && _focus == AdvancedFocus.Notifications))
            {
                mouseChanged |= _notifications.UpdateMouse(mouse, layout.NotificationRect);
                if (mouse is MouseClickMsg { Button: MouseButton.Left })
                {
                    SetFocus(AdvancedFocus.Notifications);
                    mouseChanged = true;
                }
            }

            if (mouseChanged)
            {
                _lastEvent = $"mouse:{mouse.EventType.ToString().ToLowerInvariant()}";
            }

            return null;
        }

        if (message is not KeyPressMsg key)
        {
            return null;
        }

        if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c'))
            || key.IsCharacter('q', KeyModifiers.None))
        {
            return Tea.Cmd.Quit;
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.None))
        {
            CycleFocus();
            _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
            return null;
        }

        var previousCommand = _palette.LastExecutedItemId;
        var paletteChanged = _palette.Update(key);
        if (!string.Equals(previousCommand, _palette.LastExecutedItemId, StringComparison.Ordinal))
        {
            ExecutePaletteCommand(_palette.LastExecutedItemId);
            return null;
        }

        if (_palette.IsOpen)
        {
            if (paletteChanged)
            {
                _lastEvent = key.Keystroke();
            }

            return null;
        }

        var changed = RouteFocusedInput(key);
        if (changed)
        {
            _lastEvent = key.Keystroke();
        }

        return null;
    }

    public ModelView View()
    {
        UpdateBadgeState();

        var width = Math.Max(72, _width);
        var height = Math.Max(24, _height);
        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var frame = new Rect(0, 0, width, height);
        canvas.DrawBox(frame, "TeaSharp Advanced Widgets", BorderStyle.Rounded);

        var body = frame.Inset(1, 1);
        canvas.WriteText(body.X, body.Y, "tab=focus ctrl+p=palette q=quit mouse:click+wheel tree/notifications", body.Width);

        var content = new Rect(body.X, body.Y + 1, body.Width, body.Height - 2);
        var (left, right) = Layout.SplitVertical(content, Math.Max(34, content.Width / 3), minFirst: 24, minSecond: 28);
        var (treeRect, notifyRect) = Layout.SplitHorizontal(left, Math.Max(10, left.Height / 2), minFirst: 8, minSecond: 8);
        _tree.Render(canvas, treeRect);
        _notifications.Render(canvas, notifyRect);

        var (controlsRect, detailsRect) = Layout.SplitHorizontal(right, 10, minFirst: 8, minSecond: 8);
        RenderControls(canvas, controlsRect);
        RenderDetails(canvas, detailsRect);

        _palette.Render(canvas, content);

        _status.LeftText = $"focus={_focus.ToString().ToLowerInvariant()} toggle={(_toggle.Value ? "on" : "off")} threads={_slider.Value:0}";
        _status.RightText = $"event={_lastEvent}";
        _status.Render(canvas, new Rect(0, height - 1, width, 1));

        return ModelView.From(canvas.Render()) with
        {
            Terminal = new ViewTerminal
            {
                AltScreen = true,
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                MouseMode = MouseMode.AllMotion,
                ForegroundColor = "#CDD6F4",
                BackgroundColor = "#1E1E2E",
                CursorColor = "#F5C2E7",
                WindowTitle = "TeaSharp Advanced Widgets Example",
            },
        };
    }

    private void RenderControls(Canvas canvas, Rect rect)
    {
        var first = new Rect(rect.X, rect.Y, rect.Width, 3);
        _toggle.Render(canvas, first);

        var second = new Rect(rect.X, rect.Y + 3, rect.Width, 4);
        _slider.Render(canvas, second);

        var third = new Rect(rect.X, rect.Y + 7, rect.Width, Math.Max(2, rect.Height - 7));
        _spinner.Render(canvas, third);
    }

    private void RenderDetails(Canvas canvas, Rect rect)
    {
        var badgeRect = new Rect(rect.X, rect.Y, rect.Width, 1);
        _badge.Render(canvas, badgeRect);

        var info = new LabelComponent
        {
            ShowBorder = true,
            Title = "Hints",
            Text =
                "Palette commands:\n" +
                "- notification pushes\n" +
                "- toggle/spinner/slider actions\n" +
                "- tree expand/collapse\n\n" +
                "Tree states + notifications demonstrate styling.\n",
        };
        var infoRect = new Rect(rect.X, rect.Y + 1, rect.Width, Math.Max(4, rect.Height - 1));
        info.Render(canvas, infoRect);
    }

    private bool RouteFocusedInput(KeyPressMsg key)
    {
        return _focus switch
        {
            AdvancedFocus.Toggle => _toggle.Update(key),
            AdvancedFocus.Slider => _slider.Update(key),
            AdvancedFocus.Spinner => _spinner.Update(key),
            AdvancedFocus.Tree => _tree.Update(key),
            AdvancedFocus.Notifications => _notifications.Update(key),
            _ => false,
        };
    }

    private void CycleFocus()
    {
        SetFocus(_focus switch
        {
            AdvancedFocus.Toggle => AdvancedFocus.Slider,
            AdvancedFocus.Slider => AdvancedFocus.Spinner,
            AdvancedFocus.Spinner => AdvancedFocus.Tree,
            AdvancedFocus.Tree => AdvancedFocus.Notifications,
            _ => AdvancedFocus.Toggle,
        });
    }

    private void SetFocus(AdvancedFocus focus)
    {
        _focus = focus;
        _toggle.Focused = focus == AdvancedFocus.Toggle;
        _slider.Focused = focus == AdvancedFocus.Slider;
        _spinner.Focused = focus == AdvancedFocus.Spinner;
        _tree.Focused = focus == AdvancedFocus.Tree;
        _notifications.Focused = focus == AdvancedFocus.Notifications;
    }

    private void ExecutePaletteCommand(string? commandId)
    {
        if (string.IsNullOrWhiteSpace(commandId))
        {
            return;
        }

        switch (commandId)
        {
            case "notify.info":
                _notifications.Push("info from palette", NotificationSeverity.Info);
                break;
            case "notify.warn":
                _notifications.Push("warning from palette", NotificationSeverity.Warning);
                break;
            case "notify.error":
                _notifications.Push("error from palette", NotificationSeverity.Error);
                break;
            case "feature.toggle":
                _toggle.SetValue(!_toggle.Value);
                break;
            case "spinner.toggle":
                _spinner.SetRunning(!_spinner.Running);
                break;
            case "slider.max":
                _slider.SetValue(_slider.Max);
                break;
            case "tree.collapse":
                _tree.Focused = true;
                _tree.Update(new KeyPressMsg(KeyCode.Left));
                break;
            case "tree.expand":
                _tree.Focused = true;
                _tree.Update(new KeyPressMsg(KeyCode.Right));
                break;
            case "notifications.clear":
                _notifications.Clear();
                break;
        }

        _lastEvent = $"cmd:{commandId}";
    }

    private void UpdateBadgeState()
    {
        if (!_toggle.Value)
        {
            _badge.Text = "feature off";
            _badge.State = WidgetVisualState.Warning;
            return;
        }

        if (_slider.Value >= 24)
        {
            _badge.Text = "high load";
            _badge.State = WidgetVisualState.Error;
            return;
        }

        _badge.Text = "healthy";
        _badge.State = WidgetVisualState.Success;
    }

    private InteractionRects GetInteractionRects()
    {
        var width = Math.Max(72, _width);
        var height = Math.Max(24, _height);
        var frame = new Rect(0, 0, width, height);
        var body = frame.Inset(1, 1);
        var content = new Rect(body.X, body.Y + 1, body.Width, body.Height - 2);
        var (left, right) = Layout.SplitVertical(content, Math.Max(34, content.Width / 3), minFirst: 24, minSecond: 28);
        var (treeRect, notifyRect) = Layout.SplitHorizontal(left, Math.Max(10, left.Height / 2), minFirst: 8, minSecond: 8);
        var (controlsRect, _) = Layout.SplitHorizontal(right, 10, minFirst: 8, minSecond: 8);
        var toggleRect = new Rect(controlsRect.X, controlsRect.Y, controlsRect.Width, 3);
        var sliderRect = new Rect(controlsRect.X, controlsRect.Y + 3, controlsRect.Width, 4);
        var spinnerRect = new Rect(controlsRect.X, controlsRect.Y + 7, controlsRect.Width, Math.Max(2, controlsRect.Height - 7));
        return new InteractionRects(content, treeRect, notifyRect, toggleRect, sliderRect, spinnerRect);
    }

    private readonly record struct InteractionRects(
        Rect ContentRect,
        Rect TreeRect,
        Rect NotificationRect,
        Rect ToggleRect,
        Rect SliderRect,
        Rect SpinnerRect);

    private static Command NextTick() => Tea.Cmd.Every(TimeSpan.FromMilliseconds(200), at => new AdvancedTickMsg(at));
}
