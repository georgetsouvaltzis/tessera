using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Components.Primitives;

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
    }

    public override TeaEffect? Initialize() => TeaEffects.Tick(TimeSpan.FromMilliseconds(250), static now => new AdvancedTick(now));

    public override TeaEffect? Update(Message message)
    {
        if (InputHandled)
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
            Selected node: {_tree.SelectedId ?? "none"}
            Notifications: {_notifications.Count}
            Size: {context.Width}x{context.Height}
            """;

        _status.LeftText = "Tab focus   Enter/Space activate   n notify   q quit";
        _status.RightText = $"tick={_tick:0000}";

        var left = new ColumnLayout
        {
            Gap = 1,
        };
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

        return Screen.From(new WindowLayout
        {
            Footer = LayoutSlot.Fixed(_status, 1),
            Left = LayoutSlot.Fixed(left, Math.Min(38, Math.Max(30, context.Width / 3))),
            Body = right,
            Padding = Thickness.All(1),
            Gap = 1,
        });
    }
}
