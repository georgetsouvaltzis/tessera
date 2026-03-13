using TeaSharp;
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

    private readonly ProgressBar _progress = new()
    {
        Title = "Progress",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

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

    private readonly LogView _logs = new()
    {
        Title = "Logs",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

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

    private readonly TreeView _tree = new()
    {
        Title = "Tree",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Notifications _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        MaxItems = 32,
    };

    private readonly StatusBar _status = new();

    private int _tick;
    private string _statusText = "ready";

    public WidgetGalleryApp()
    {
        _choice.SetItems(["Development", "Staging", "Production", "Canary", "Benchmark"]);
        _progress.SetValue(0.25);
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
            _notifications.Push("deploy triggered", NotificationLevel.Success);
            _statusText = "deploy triggered";
        };
        _textInput.Submitted += (_, args) =>
        {
            _logs.Append($"input:{args.Value}");
            _statusText = $"submitted {args.Value}";
        };
        _dialog.Accepted += (_, _) =>
        {
            _statusText = "dialog accepted";
            _logs.Append("dialog:accepted");
        };
        _dialog.Dismissed += (_, _) =>
        {
            _statusText = "dialog cancelled";
            _logs.Append("dialog:dismissed");
        };

        _tree.SetItems(
        [
            new TreeItem("root", "Controls")
            {
                Expanded = true,
            },
            new TreeItem("root-catalog", "Root Catalog",
            [
                new TreeItem("label", "Label"),
                new TreeItem("input", "TextInput"),
                new TreeItem("list", "ListView"),
            ])
            {
                Expanded = true,
            },
            new TreeItem("advanced", "Advanced",
            [
                new TreeItem("tree", "TreeView"),
                new TreeItem("notify", "Notifications"),
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
        _notifications.Push("widget gallery ready", NotificationLevel.Info);
    }

    public override TeaEffect? Initialize() => TeaEffects.Tick(TimeSpan.FromMilliseconds(300), static now => new GalleryTick(now));

    public override TeaEffect? Update(Message message)
    {
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

        return Screen.From(new WindowLayout
        {
            Header = new LayoutSlot
            {
                Content = _tabs,
                Length = 1,
            },
            Footer = new LayoutSlot
            {
                Content = _status,
                Length = 1,
            },
            Body = BuildTabContent(context),
            Overlay = new CenterLayout
            {
                Content = _dialog,
                Width = 42,
                Height = 8,
            },
            Gap = 1,
            Padding = Thickness.All(1),
        });
    }

    private LayoutNode BuildTabContent(ScreenContext context)
    {
        return _tabs.SelectedIndex switch
        {
            0 => CreateBasicsTab(),
            1 => CreateInputsTab(),
            2 => CreateDataTab(context),
            3 => new CenterLayout
            {
                Content = new Label
                {
                    Title = "Overlay",
                    Text = "Press d to open the confirmation dialog.\nFocus and rendering stay on the new screen model.",
                    Border = BorderStyle.SingleLine,
                    Padding = Thickness.All(1),
                },
                Width = Math.Min(64, Math.Max(36, context.Width - 6)),
                Height = 8,
            },
            _ => CreateAdvancedTab(),
        };
    }

    private ColumnLayout CreateBasicsTab()
    {
        var content = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _label,
                    Length = 6,
                },
                new LayoutSlot
                {
                    Content = _button,
                    Length = 5,
                },
                new LayoutSlot
                {
                    Content = _progress,
                    Length = 4,
                },
            },
        };
        return content;
    }

    private ColumnLayout CreateInputsTab()
    {
        var content = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _textInput,
                    Length = 5,
                },
                new LayoutSlot
                {
                    Content = _choice,
                    Length = 8,
                },
                new LayoutSlot
                {
                    Content = _textArea,
                    Length = LayoutLength.Fill(),
                },
            },
        };
        return content;
    }

    private WindowLayout CreateDataTab(ScreenContext context)
    {
        var details = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _table,
                    Length = 10,
                },
                new LayoutSlot
                {
                    Content = _logs,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        return new WindowLayout
        {
            Left = new LayoutSlot
            {
                Content = _list,
                Length = Math.Min(28, Math.Max(22, context.Width / 4)),
            },
            Body = details,
            Gap = 1,
        };
    }

    private RowLayout CreateAdvancedTab()
    {
        var content = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _tree,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _notifications,
                    Length = LayoutLength.Fill(),
                },
            },
        };
        return content;
    }
}
