using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<AdvancedWidgetsApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Advanced Widgets",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record AdvancedTick(DateTimeOffset At) : Message;

internal sealed class AdvancedWidgetsApp : TeaApp
{
    private readonly Badge _modeBadge = new()
    {
        Text = "stable",
        Tone = BadgeTone.Success,
    };

    private readonly Toggle _toggle = new()
    {
        Title = "Feature Flag",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Slider _slider = new()
    {
        Title = "Concurrency",
        Min = 1,
        Max = 32,
        Step = 1,
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Spinner _spinner = new()
    {
        Title = "Indexer",
        Label = "running",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly TreeView _tree = new()
    {
        Title = "Workspace",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Notifications _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        MaxItems = 48,
    };

    private readonly CommandPalette _palette = new()
    {
        Title = "Workspace Actions",
    };

    private readonly ContextMenu _contextMenu = new()
    {
        Title = "Quick Actions",
    };

    private readonly Label _summary = new()
    {
        Title = "Summary",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();

    private int _tick;

    public AdvancedWidgetsApp()
    {
        _slider.SetValue(8);
        _toggle.SetValue(true);
        _tree.SetItems(
        [
            new TreeItem("root", "TeaSharp")
            {
                Expanded = true,
            },
            new TreeItem("runtime", "Runtime",
            [
                new TreeItem("input", "Input pipeline"),
                new TreeItem("render", "Renderer"),
                new TreeItem("screen", "Screen compiler"),
            ])
            {
                Expanded = true,
            },
            new TreeItem("controls", "Controls",
            [
                new TreeItem("core", "Root catalog"),
                new TreeItem("advanced", "Advanced catalog"),
            ])
            {
                Expanded = true,
            },
        ]);

        _notifications.Push("advanced demo booted", NotificationLevel.Success);
        _notifications.Push("tab cycles focus", NotificationLevel.Info);
        _notifications.Push("n pushes a notification", NotificationLevel.Warning);
        _notifications.Push("p opens command palette", NotificationLevel.Info);
        _notifications.Push("x opens context menu", NotificationLevel.Info);

        _palette.SetItems(
        [
            new CommandPaletteItem("refresh", "Refresh workspace", "poll files and redraw"),
            new CommandPaletteItem("notify", "Push notification", "emit a manual notification"),
            new CommandPaletteItem("pause", "Pause spinner", "toggle indexer activity"),
        ]);
        _palette.ItemExecuted += (_, args) =>
        {
            _notifications.Push($"palette: {args.Item.Title}", NotificationLevel.Success);
            if (args.ItemId == "notify")
            {
                _notifications.Push($"manual event {_tick:000}", NotificationLevel.Warning);
            }
            else if (args.ItemId == "pause")
            {
                _spinner.SetRunning(!_spinner.Running);
            }
        };

        _contextMenu.SetItems(
        [
            new ContextMenuItem("copy", "Copy summary"),
            new ContextMenuItem("clear", "Clear notifications"),
        ]);
        _contextMenu.ItemExecuted += (_, args) =>
        {
            _notifications.Push($"menu: {args.Item.Title}", NotificationLevel.Info);
            if (args.ItemId == "clear")
            {
                _notifications.Clear();
                _notifications.Push("notifications cleared", NotificationLevel.Warning);
            }
        };
    }

    public override TeaEffect? Initialize() => TeaEffects.Tick(TimeSpan.FromMilliseconds(250), static now => new AdvancedTick(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is AdvancedTick tick)
        {
            _tick++;
            if (_spinner.Running)
            {
                _spinner.Advance();
            }

            if (_tick % 12 == 0)
            {
                _notifications.Push($"heartbeat {tick.At:HH:mm:ss}", NotificationLevel.Info);
            }

            return TeaEffects.Tick(TimeSpan.FromMilliseconds(250), static now => new AdvancedTick(now));
        }

        if (message is KeyPressed key)
        {
            if (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            {
                return TeaEffects.Quit;
            }

            if (key.IsCharacter('n'))
            {
                _notifications.Push($"manual event {_tick:000}", NotificationLevel.Warning);
                _status.RightText = "notification pushed";
                return null;
            }

            if (key.IsCharacter('p'))
            {
                _palette.Open();
                _status.RightText = "palette open";
                return null;
            }

            if (key.IsCharacter('x'))
            {
                _contextMenu.OpenAt(2, 2);
                _status.RightText = "context menu open";
                return null;
            }
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _summary.Text =
            $"""
            Mode: {_modeBadge.Text}
            Toggle: {(_toggle.Value ? "ON" : "OFF")}
            Concurrency: {_slider.Value:0}
            Spinner: {(_spinner.Running ? "running" : "paused")}
            Selected node: {_tree.SelectedId ?? "none"}
            Notifications: {_notifications.Count}
            Palette: {(_palette.IsVisible ? "open" : "closed")}
            Context menu: {(_contextMenu.IsVisible ? "open" : "closed")}
            Size: {context.Width}x{context.Height}
            """;

        _modeBadge.Text = _toggle.Value ? "live" : "stable";
        _modeBadge.Tone = _toggle.Value ? BadgeTone.Warning : BadgeTone.Success;

        _status.LeftText = "Tab focus   Enter/Space activate   n notify   p palette   x menu   q quit";
        _status.RightText = $"tick={_tick:0000}";

        var left = new ColumnLayout
        {
            Gap = 1,
        };
        left.AddFixed(_modeBadge, 1);
        left.AddFixed(_toggle, 5);
        left.AddFixed(_slider, 6);
        left.AddFixed(_spinner, 5);
        left.AddFill(_summary);

        var right = new ColumnLayout
        {
            Gap = 1,
        };
        right.AddFill(_tree);
        right.AddFill(_notifications);

        LayoutNode? overlay = null;
        if (_palette.IsVisible)
        {
            overlay = new CenterLayout(
                _palette,
                width: Math.Min(72, Math.Max(48, context.Width - 6)),
                height: Math.Min(14, Math.Max(8, context.Height - 4)));
        }
        else if (_contextMenu.IsVisible)
        {
            overlay = new CenterLayout(_contextMenu, width: 32, height: 8);
        }

        return Screen.From(new WindowLayout
        {
            Footer = LayoutSlot.Fixed(_status, 1),
            Left = LayoutSlot.Fixed(left, Math.Min(38, Math.Max(30, context.Width / 3))),
            Body = right,
            Overlay = overlay,
            Padding = Thickness.All(1),
            Gap = 1,
        });
    }
}
