using TeaSharp;
using TeaSharp.Components.Advanced;
using TeaSharp.Components.Primitives;
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
    private readonly ToggleSwitchComponent _toggle = new()
    {
        Title = "Feature Flag",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly SliderComponent _slider = new()
    {
        Title = "Concurrency",
        Min = 1,
        Max = 32,
        Step = 1,
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly SpinnerComponent _spinner = new()
    {
        Title = "Indexer",
        Label = "running",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly TreeViewComponent _tree = new()
    {
        Title = "Workspace",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly NotificationCenterComponent _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        MaxEntries = 48,
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
        _tree.SetRoots(
        [
            new TreeItemNode("root", "TeaSharp")
            {
                Expanded = true,
            },
            new TreeItemNode("runtime", "Runtime",
            [
                new TreeItemNode("input", "Input pipeline"),
                new TreeItemNode("render", "Renderer"),
                new TreeItemNode("screen", "Screen compiler"),
            ])
            {
                Expanded = true,
            },
            new TreeItemNode("controls", "Controls",
            [
                new TreeItemNode("core", "Root catalog"),
                new TreeItemNode("advanced", "Advanced catalog"),
            ])
            {
                Expanded = true,
            },
        ]);

        _notifications.Push("advanced demo booted", NotificationSeverity.Success);
        _notifications.Push("tab cycles focus", NotificationSeverity.Info);
        _notifications.Push("n pushes a notification", NotificationSeverity.Warning);
    }

    public override TeaEffect? Initialize() => TeaEffects.Tick(TimeSpan.FromMilliseconds(250), static now => new AdvancedTick(now));

    public override TeaEffect? Update(Message message)
    {
        if (HandleScreenInput(message))
        {
            return null;
        }

        if (message is AdvancedTick tick)
        {
            _tick++;
            if (_spinner.Running)
            {
                _spinner.Advance();
            }

            if (_tick % 12 == 0)
            {
                _notifications.Push($"heartbeat {tick.At:HH:mm:ss}", NotificationSeverity.Info);
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
                _notifications.Push($"manual event {_tick:000}", NotificationSeverity.Warning);
                _status.RightText = "notification pushed";
                return null;
            }
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _summary.Text =
            $"""
            Toggle: {(_toggle.Value ? "ON" : "OFF")}
            Concurrency: {_slider.Value:0}
            Spinner: {(_spinner.Running ? "running" : "paused")}
            Selected node: {_tree.SelectedNodeId ?? "none"}
            Notifications: {_notifications.Entries.Count}
            Size: {context.Width}x{context.Height}
            """;

        _status.LeftText = "Tab focus   Enter/Space activate   n notify   q quit";
        _status.RightText = $"tick={_tick:0000}";

        return Screen.From(
            new DockLayout(
                bottom: new LayoutSlot(_status, LayoutLength.Fixed(1)),
                fill: new LayoutSlot(
                    new SplitLayout(
                        LayoutOrientation.Horizontal,
                        new LayoutSlot(
                            new StackLayout(
                                LayoutOrientation.Vertical,
                                gap: 1,
                                children:
                                [
                                    new LayoutSlot(_toggle, LayoutLength.Fixed(5)),
                                    new LayoutSlot(_slider, LayoutLength.Fixed(6)),
                                    new LayoutSlot(_spinner, LayoutLength.Fixed(5)),
                                    new LayoutSlot(_summary, LayoutLength.Fill()),
                                ]),
                            LayoutLength.Fixed(Math.Min(38, Math.Max(30, context.Width / 3)))),
                        new LayoutSlot(
                            new StackLayout(
                                LayoutOrientation.Vertical,
                                gap: 1,
                                children:
                                [
                                    new LayoutSlot(_tree, LayoutLength.Fill()),
                                    new LayoutSlot(_notifications, LayoutLength.Fill()),
                                ]),
                            LayoutLength.Fill()),
                        gap: 1),
                    LayoutLength.Fill()),
                padding: Thickness.All(1)));
    }
}
