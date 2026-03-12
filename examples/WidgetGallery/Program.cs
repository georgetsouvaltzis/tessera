using TeaSharp;
using TeaSharp.Components.Advanced;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<WidgetGalleryApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Widget Gallery",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
            EnableBracketedPaste = true,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record GalleryTick(DateTimeOffset At) : Message;

internal sealed class WidgetGalleryApp : TeaApp
{
    private readonly Tabs _tabs = new("Basics", "Inputs", "Data", "Overlay", "Advanced");
    private readonly Label _label = new()
    {
        Title = "Label",
        Text = "TeaSharp now teaches object-based screens, root controls, and a separate advanced layer.",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Button _button = new()
    {
        Text = "Deploy",
        Description = "Enter/Space activate",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly ProgressBarComponent _progress = new(new ProgressBarOptions(
        Title: "Progress",
        InitialValue: 0.25,
        Border: BorderStyle.SingleLine,
        Padding: Thickness.All(1)));

    private readonly TextInput _textInput = new()
    {
        Title = "Text Input",
        Placeholder = "type and press Enter",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        ClearOnSubmit = true,
    };

    private readonly TextArea _textArea = new()
    {
        Title = "Text Area",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Wrap = true,
        ShowLineNumbers = true,
    };

    private readonly Choice _choice = new()
    {
        Title = "Environment",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        MaxVisibleItems = 5,
    };

    private readonly ListView<string> _list = new()
    {
        Title = "List",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Table _table = new("Service", "Status", "P95")
    {
        Title = "Table",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        PageSize = 5,
    };

    private readonly LogViewerComponent _logs = new(new LogViewerOptions(
        Title: "Logs",
        Border: BorderStyle.SingleLine,
        Padding: Thickness.All(1)));

    private readonly Dialog _dialog = new()
    {
        Title = "Confirm",
        BodyLines =
        [
            "Publish widget package?",
            "Enter accepts",
            "Esc cancels",
        ],
    };

    private readonly TreeViewComponent _tree = new()
    {
        Title = "Tree",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly NotificationCenterComponent _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        MaxEntries = 32,
    };

    private readonly StatusBar _status = new();

    private int _tick;
    private string _statusText = "ready";

    public WidgetGalleryApp()
    {
        _choice.SetItems(["Development", "Staging", "Production", "Canary", "Benchmark"]);
        _choice.SelectionChanged += (_, args) =>
        {
            _statusText = $"selected {args.SelectedItem}";
            _logs.Append($"choice:{args.SelectedItem}");
        };

        _list.SetItems(["alpha", "beta", "gamma", "delta", "epsilon", "zeta", "eta"]);
        _list.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null)
            {
                _statusText = $"list {args.SelectedItem}";
            }
        };

        _table.SetRows(
        [
            ["api", "ok", "21ms"],
            ["worker", "ok", "18ms"],
            ["scheduler", "warn", "63ms"],
            ["gateway", "ok", "25ms"],
            ["events", "ok", "34ms"],
            ["billing", "degraded", "92ms"],
            ["search", "ok", "30ms"],
        ]);

        _button.Activated += (_, _) =>
        {
            _logs.Append("button:deploy");
            _notifications.Push("deploy triggered", NotificationSeverity.Success);
            _statusText = "deploy triggered";
        };

        _tree.SetRoots(
        [
            new TreeItemNode("root", "Controls")
            {
                Expanded = true,
            },
            new TreeItemNode("root-catalog", "Root Catalog",
            [
                new TreeItemNode("label", "Label"),
                new TreeItemNode("input", "TextInput"),
                new TreeItemNode("list", "ListView"),
            ])
            {
                Expanded = true,
            },
            new TreeItemNode("advanced", "Advanced",
            [
                new TreeItemNode("tree", "TreeView"),
                new TreeItemNode("notify", "NotificationCenter"),
            ])
            {
                Expanded = true,
            },
        ]);

        _textArea.SetValue(
            """
            Multi-line controls stay available.
            The default API now hides routing and region registration.
            Advanced widgets remain opt-in.
            """);

        _logs.Append("gallery booted");
        _notifications.Push("widget gallery ready", NotificationSeverity.Info);
    }

    public override TeaEffect? Initialize() => TeaEffects.Tick(TimeSpan.FromMilliseconds(300), static now => new GalleryTick(now));

    public override TeaEffect? Update(Message message)
    {
        if (HandleScreenInput(message))
        {
            if (_textInput.TryConsumeSubmission(out var value))
            {
                _logs.Append($"input:{value}");
                _statusText = $"submitted {value}";
            }

            if (_dialog.TryConsumeResult(out var result))
            {
                _statusText = result == TeaSharp.Controls.DialogResult.Accepted ? "dialog accepted" : "dialog cancelled";
                _logs.Append($"dialog:{result}");
            }

            return null;
        }

        if (message is GalleryTick tick)
        {
            _tick++;
            _progress.SetValue((_tick % 100) / 100.0);
            if (_tick % 10 == 0)
            {
                _logs.Append($"{tick.At:HH:mm:ss} pulse={_tick:0000}");
            }

            return TeaEffects.Tick(TimeSpan.FromMilliseconds(300), static now => new GalleryTick(now));
        }

        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.IsCharacter('d'))
        {
            _dialog.Show("Confirm", "Publish widget package?", "Enter accepts", "Esc cancels");
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Tab={_tabs.Items[_tabs.SelectedIndex]}   Tick={_tick:0000}";
        _status.RightText = _statusText;

        return Screen.From(
            new OverlayLayout(
                new DockLayout(
                    top: new LayoutSlot(_tabs, LayoutLength.Fixed(1)),
                    bottom: new LayoutSlot(_status, LayoutLength.Fixed(1)),
                    fill: new LayoutSlot(BuildTabContent(context), LayoutLength.Fill()),
                    gap: 1,
                    padding: Thickness.All(1)),
                new CenterLayout(_dialog, width: 42, height: 8)));
    }

    private LayoutNode BuildTabContent(ScreenContext context)
    {
        return _tabs.SelectedIndex switch
        {
            0 => new StackLayout(
                LayoutOrientation.Vertical,
                gap: 1,
                children:
                [
                    new LayoutSlot(_label, LayoutLength.Fixed(6)),
                    new LayoutSlot(_button, LayoutLength.Fixed(5)),
                    new LayoutSlot(_progress, LayoutLength.Fixed(4)),
                ]),
            1 => new StackLayout(
                LayoutOrientation.Vertical,
                gap: 1,
                children:
                [
                    new LayoutSlot(_textInput, LayoutLength.Fixed(5)),
                    new LayoutSlot(_choice, LayoutLength.Fixed(8)),
                    new LayoutSlot(_textArea, LayoutLength.Fill()),
                ]),
            2 => new SplitLayout(
                LayoutOrientation.Horizontal,
                new LayoutSlot(_list, LayoutLength.Fixed(Math.Min(28, Math.Max(22, context.Width / 4)))),
                new LayoutSlot(
                    new StackLayout(
                        LayoutOrientation.Vertical,
                        gap: 1,
                        children:
                        [
                            new LayoutSlot(_table, LayoutLength.Fixed(10)),
                            new LayoutSlot(_logs, LayoutLength.Fill()),
                        ]),
                    LayoutLength.Fill()),
                gap: 1),
            3 => new CenterLayout(
                new Label
                {
                    Title = "Overlay",
                    Text = "Press d to open the confirmation dialog.\nFocus and rendering stay on the new screen model.",
                    Border = BorderStyle.SingleLine,
                    Padding = Thickness.All(1),
                },
                width: Math.Min(64, Math.Max(36, context.Width - 6)),
                height: 8),
            _ => new SplitLayout(
                LayoutOrientation.Horizontal,
                new LayoutSlot(_tree, LayoutLength.Fill()),
                new LayoutSlot(_notifications, LayoutLength.Fill()),
                gap: 1),
        };
    }
}
