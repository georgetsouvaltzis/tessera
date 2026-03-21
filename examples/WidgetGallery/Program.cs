using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<WidgetGalleryApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = WidgetGalleryApp.DemoTheme;
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

internal enum GalleryThemeMode
{
    Catppuccin = 0,
    RosePine = 1,
}

internal sealed class WidgetGalleryApp : TeaApp
{
    public static readonly TeaTheme DemoTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly Tabs _tabs = new(
        "Overview",
        "Inputs",
        "Data",
        "Forms",
        "Workflow",
        "Workspace",
        "Plots",
        "Telemetry");

    private readonly Label _label = new()
    {
        Title = "TeaSharp",
        Text = "Public API is library-first, theme-first, and keyboard+pointer native.\nThis gallery is styled by default and demonstrates wave widgets.",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Button _button = new()
    {
        Text = "Deploy",
        Description = "Enter/Space activate • d opens dialog",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly ProgressBar _progress = new()
    {
        Title = "Deployment Progress",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly TextInput _textInput = new()
    {
        Title = "Command Input",
        Placeholder = "type command and press Enter",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        ClearOnSubmit = true,
    };

    private readonly TextArea _textArea = new()
    {
        Title = "Notes",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Wrap = true,
        ShowLineNumbers = true,
    };

    private readonly Choice _choice = new()
    {
        Title = "Environment",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MaxVisibleItems = 5,
    };

    private readonly ComboBox _comboBox = new()
    {
        Title = "Region Filter",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MaxVisibleItems = 5,
        Placeholder = "type to filter regions",
    };

    private readonly ListView<string> _list = new()
    {
        Title = "Services",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Table _table = new("Service", "Status", "P95", "Req/s")
    {
        Title = "Health Table",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        PageSize = 6,
    };

    private readonly LogView _logs = new()
    {
        Title = "Activity Stream",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Dialog _dialog = new()
    {
        Title = "Confirm Publish",
        BodyLines =
        [
            "Publish WidgetGallery package?",
            "Enter accepts",
            "Esc cancels",
        ],
    };

    private readonly Form _form = new()
    {
        Title = "Form",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "●",
        RequiredMarker = "!",
    };

    private readonly FieldSet _fieldSet = new()
    {
        Title = "FieldSet",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "●",
        SectionPrefix = "⟦",
        SectionSuffix = "⟧",
        SelectedMarker = "▸",
    };

    private readonly Wizard _wizard = new()
    {
        Title = "Wizard",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "●",
        ActiveMarker = "➤",
        CompletedMarker = "✓",
        PendingMarker = "·",
    };

    private readonly InspectorPanel _inspector = new()
    {
        Title = "InspectorPanel",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "●",
        ExpandedMarker = "▾",
        CollapsedMarker = "▸",
    };

    private readonly SplitView _splitView = new()
    {
        Orientation = SplitViewOrientation.Horizontal,
        Ratio = 0.44,
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        ShowDivider = true,
        DividerGlyph = '│',
    };

    private readonly DataForm<GalleryProfileModel> _dataForm = new()
    {
        Title = "DataForm<Profile>",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "●",
        SelectedMarker = "▸",
    };

    private readonly PaneTabs _paneTabs = new()
    {
        Title = "PaneTabs",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "●",
        SelectedPrefix = "⟦",
        SelectedSuffix = "⟧",
    };

    private readonly DockWorkspace _workspace = new()
    {
        Title = "DockWorkspace",
        Border = BorderStyle.Rounded,
        PaneBorder = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        PanePadding = Thickness.All(1),
        FocusMarker = "●",
        SelectedPaneMarker = "▸",
    };

    private readonly TerminalPanel _terminalPanel = new()
    {
        MaxLines = 120,
        FollowTail = true,
        ShowLineNumbers = true,
        Padding = Thickness.All(1),
    };

    private readonly TreeView _tree = new()
    {
        Title = "Tree View",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "●",
    };

    private readonly Notifications _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MaxItems = 32,
        FocusMarker = "●",
    };

    private readonly Gauge _gauge = new()
    {
        Title = "CPU",
        MaxValue = 100,
    };

    private readonly StatsCard _statsCard = new()
    {
        Title = "Cluster",
    };

    private readonly MiniLog _miniLog = new()
    {
        Title = "Mini Log",
    };

    private readonly BarChart _barChart = new()
    {
        Title = "Services",
    };

    private readonly LineChart _lineChart = new(48)
    {
        Title = "Latency",
    };

    private readonly LineSeries _lineP50Series = new("p50");
    private readonly LineSeries _lineP95Series = new("p95");

    private readonly LinePlot _linePlot = new()
    {
        Title = "Line Plot",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Options = new LinePlotOptions(
            ShowAxes: true,
            ShowLegend: true,
            ShowStats: true,
            XLabel: "time",
            YLabel: "ms"),
    };

    private readonly ScatterPlot _scatterPlot = new()
    {
        Title = "Scatter Plot",
        Options = new ScatterPlotOptions(
            ShowAxes: true,
            ShowLabels: false,
            Legend: "throughput vs errors",
            XLabel: "req/s",
            YLabel: "errors"),
    };

    private readonly Histogram _histogram = new()
    {
        Title = "Histogram",
        Options = new HistogramOptions(
            ShowAxes: true,
            ShowBucketLabels: true,
            ShowScale: true,
            Legend: "latency distribution",
            XLabel: "percentile",
            YLabel: "ms"),
    };

    private readonly Sparkline _sparkline = new(capacity: 96)
    {
        Title = "Sparkline",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Options = new SparklineOptions(
            ShowStats: true,
            Legend: "cpu%"),
    };

    private readonly AreaPlot _areaPlot = new(capacity: 96)
    {
        Title = "Area Plot",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Options = new AreaPlotOptions(
            ShowStats: true,
            Legend: "queue depth"),
    };

    private readonly StatusBar _status = new()
    {
        Fill = ' ',
    };

    private readonly GalleryProfileModel _profile = new()
    {
        Service = "Checkout API",
        Owner = "Platform Team",
        Region = "eu-central-1",
        Slo = "99.95",
        Budget = "4500",
    };

    private GalleryThemeMode _themeMode = GalleryThemeMode.Catppuccin;
    private int _tick;
    private string _statusText = "ready";

    public WidgetGalleryApp()
    {
        _tabs.Title = "Widget Gallery";
        _tabs.FocusMarker = "●";

        _choice.SetItems(["Development", "Staging", "Production", "Canary", "Benchmark"]);
        _progress.SetValue(0.25);
        _choice.SelectionChanged += (_, args) =>
        {
            _statusText = $"environment: {args.SelectedItem}";
            _logs.Append($"choice:{args.SelectedItem}");
        };
        _comboBox.SetItems(["us-east-1", "us-west-2", "eu-central-1", "eu-west-1", "ap-southeast-1", "ap-northeast-1"]);
        _comboBox.SelectionChanged += (_, args) =>
        {
            _statusText = $"region: {args.SelectedItem}";
            _logs.Append($"combo:{args.SelectedItem}");
        };

        _list.SetItems(["api", "worker", "scheduler", "gateway", "events", "billing", "search"]);
        _list.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null)
            {
                _statusText = $"service: {args.SelectedItem}";
            }
        };

        _table.SetRows(
        [
            ["api", "ok", "21ms", "410"],
            ["worker", "ok", "18ms", "350"],
            ["scheduler", "warn", "63ms", "120"],
            ["gateway", "ok", "25ms", "780"],
            ["events", "ok", "34ms", "190"],
            ["billing", "degraded", "92ms", "72"],
            ["search", "ok", "30ms", "640"],
        ]);

        _button.Activated += (_, _) =>
        {
            _logs.Append("deploy triggered");
            _notifications.Push("deployment queued", NotificationLevel.Success);
            _terminalPanel.Append("deploy --service checkout-api", TerminalPanelChannel.Command, "$");
            _terminalPanel.Append("deployment accepted by orchestrator", TerminalPanelChannel.System, "i");
            _statusText = "deploy triggered";
        };

        _textInput.Submitted += (_, args) =>
        {
            _logs.Append($"input:{args.Value}");
            _terminalPanel.Append(args.Value, TerminalPanelChannel.Command, "$");
            _statusText = $"submitted: {args.Value}";
        };

        _dialog.Accepted += (_, _) =>
        {
            _statusText = "dialog accepted";
            _logs.Append("dialog:accepted");
            _terminalPanel.Append("publish completed", TerminalPanelChannel.System, "✓");
        };
        _dialog.Dismissed += (_, _) =>
        {
            _statusText = "dialog cancelled";
            _logs.Append("dialog:dismissed");
        };

        _paneTabs.SetTabs(
        [
            new PaneTabItem("orders", "orders.cs") { IsDirty = true },
            new PaneTabItem("alerts", "alerts.rules"),
            new PaneTabItem("perf", "perf-notes.md"),
        ]);
        _paneTabs.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null)
            {
                _statusText = $"pane tab: {args.SelectedItem.Title}";
            }
        };

        _workspace.SetPanes(
        [
            new DockPane("navigator", "Navigator", DockPanePosition.Left, size: 23)
            {
                Lines =
                [
                    "src/",
                    "  TeaSharp/",
                    "  TeaSharp.Core/",
                    "tests/",
                    "examples/",
                ],
            },
            new DockPane("outline", "Outline", DockPanePosition.Right, size: 24)
            {
                IsMuted = true,
                Lines =
                [
                    "Build()",
                    "Update()",
                    "Theme switch",
                    "Status updates",
                ],
            },
            new DockPane("problems", "Problems", DockPanePosition.Bottom, size: 6)
            {
                IsMuted = true,
                Lines =
                [
                    "No active diagnostics",
                    "Type t to toggle theme",
                    "Press d to open modal",
                ],
            },
            new DockPane("editor", "Editor", DockPanePosition.Center)
            {
                Lines =
                [
                    "public override Screen Build(ScreenContext context)",
                    "{",
                    "    // polished, themed default",
                    "    return Screen.Build(...);",
                    "}",
                ],
            },
        ]);
        _workspace.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null)
            {
                _statusText = $"pane: {args.SelectedItem.Title}";
            }
        };

        _tree.SetItems(
        [
            new TreeItem("root", "Controls")
            {
                Expanded = true,
            },
            new TreeItem("forms", "Forms",
            [
                new TreeItem("form", "Form"),
                new TreeItem("fieldset", "FieldSet"),
                new TreeItem("dataform", "DataForm<T>"),
            ])
            {
                Expanded = true,
            },
            new TreeItem("workspace", "Workspace",
            [
                new TreeItem("dock", "DockWorkspace"),
                new TreeItem("tabs", "PaneTabs"),
                new TreeItem("terminal", "TerminalPanel"),
            ])
            {
                Expanded = true,
            },
        ]);

        _form.SetFields(
        [
            new FormField("service", "Service", _profile.Service, isRequired: true),
            new FormField("owner", "Owner", _profile.Owner, helperText: "on-call rotation"),
            new FormField("region", "Region", _profile.Region),
            new FormField("slo", "SLO (%)", _profile.Slo, helperText: "target objective"),
            new FormField("budget", "Budget", $"${_profile.Budget}", isDisabled: true),
        ]);

        _fieldSet.SetItems(
        [
            "Validation: enabled",
            "Auto-save: every 30s",
            "Fallback profile: staging",
            "Audit trail: immutable",
            "Rate limit: 250 req/s",
        ]);

        _wizard.SetSteps(
        [
            new WizardStep("draft", "Draft config", "set services and targets", isCompleted: true),
            new WizardStep("validate", "Validate", "run checks"),
            new WizardStep("approve", "Approval", "team lead signoff"),
            new WizardStep("deploy", "Deploy", "rolling strategy"),
            new WizardStep("observe", "Observe", "watch error budget"),
        ]);

        var runtimeSection = new InspectorSection("Runtime", isExpanded: true);
        runtimeSection.AddField("mode", "interactive");
        runtimeSection.AddField("mouse", "enabled");
        runtimeSection.AddField("paste", "enabled");
        runtimeSection.AddDetail("Use pointer to select rows and tabs.");

        var themeSection = new InspectorSection("Theme", isExpanded: true);
        themeSection.AddField("active", "Catppuccin Macchiato");
        themeSection.AddField("toggle", "press t");
        themeSection.AddDetail("Semantic tokens drive focused/selected state.");

        var buildSection = new InspectorSection("Build", isExpanded: false);
        buildSection.AddField("state", "clean");
        buildSection.AddField("warnings", "0");

        _inspector.SetSections([runtimeSection, themeSection, buildSection]);

        _splitView.First = _wizard;
        _splitView.Second = _inspector;

        _dataForm.RegisterField("service", "Service", static m => m.Service, static (m, value) => m.Service = value, placeholder: "checkout-api");
        _dataForm.RegisterField("owner", "Owner", static m => m.Owner, static (m, value) => m.Owner = value, placeholder: "platform-team");
        _dataForm.RegisterField("region", "Region", static m => m.Region, static (m, value) => m.Region = value, placeholder: "us-east-1");
        _dataForm.RegisterField(
            "slo",
            "SLO (%)",
            static m => m.Slo,
            static (m, value) => m.Slo = value,
            placeholder: "99.95",
            validator: static value =>
                decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var percent)
                && percent is >= 90m and <= 100m
                    ? null
                    : "Expected value in range 90..100");
        _dataForm.RegisterField(
            "budget",
            "Budget",
            static m => m.Budget,
            static (m, value) => m.Budget = value,
            placeholder: "4500",
            validator: static value =>
                decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _)
                    ? null
                    : "Numeric budget required");
        _dataForm.SetModel(_profile);
        _dataForm.SelectionChanged += (_, args) =>
        {
            if (args.SelectedField is not null)
            {
                _statusText = $"data form: {args.SelectedField.Label}";
            }
        };

        _textArea.SetValue(
            """
            Try this sequence:
            1) go to Forms and edit DataForm values.
            2) switch to Workflow and complete wizard steps.
            3) inspect Workspace panes and terminal stream.
            """);

        _notifications.Push("Widget gallery ready", NotificationLevel.Info);
        _terminalPanel.Append("dotnet run --project examples/WidgetGallery", TerminalPanelChannel.Command, "$");
        _terminalPanel.Append("WidgetGallery started", TerminalPanelChannel.System, "i");
        _terminalPanel.Append("monitor: api latency stable at 21ms", TerminalPanelChannel.StandardOutput, "·");
        _terminalPanel.Append("warn: scheduler lag reached 63ms", TerminalPanelChannel.StandardError, "!");

        _logs.Append("gallery booted");
        _miniLog.Append("telemetry ready");
        _barChart.SetBars(
        [
            new BarPoint("api", 42),
            new BarPoint("worker", 58),
            new BarPoint("events", 33),
        ]);
        _statsCard.SetItems(
        [
            new StatItem("runtime", ".NET 10"),
            new StatItem("theme", ThemeLabel()),
            new StatItem("mouse", "yes"),
        ]);

        _linePlot.SetSeries([_lineP50Series, _lineP95Series]);
        ApplyCurrentTheme();
        UpdatePlotWidgets();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Tick(TimeSpan.FromMilliseconds(300), static now => new GalleryTick(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is GalleryTick tick)
        {
            _tick++;
            _progress.SetValue((_tick % 100) / 100.0);
            _lineChart.Append(20 + Math.Sin(_tick / 6d) * 12 + (_tick % 5));
            UpdatePlotWidgets();
            _gauge.Value = (_tick * 7) % 100;
            _gauge.Label = $"{_gauge.Value:0}%";
            _barChart.SetValue("api", 30 + (_tick % 40));
            _barChart.SetValue("worker", 45 + ((_tick * 3) % 30));
            _barChart.SetValue("events", 20 + ((_tick * 5) % 25));
            _statsCard.SetValue("tick", _tick.ToString("0000", CultureInfo.InvariantCulture));
            _statsCard.SetValue("theme", ThemeLabel());
            if (_tick % 10 == 0)
            {
                _logs.Append($"{tick.At:HH:mm:ss} pulse={_tick:0000}");
                _miniLog.Append($"{tick.At:HH:mm:ss} p95={20 + (_tick % 15)}ms");
            }

            if (_tick % 14 == 0)
            {
                _terminalPanel.Append($"heartbeat {_tick:0000}", TerminalPanelChannel.StandardOutput, "·");
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
            _dialog.Show("Confirm Publish", "Publish WidgetGallery package?", "Enter accepts", "Esc cancels");
        }

        if (key.IsCharacter('t'))
        {
            _themeMode = _themeMode == GalleryThemeMode.Catppuccin
                ? GalleryThemeMode.RosePine
                : GalleryThemeMode.Catppuccin;
            ApplyCurrentTheme();
            _statusText = $"theme: {ThemeLabel()}";
            _notifications.Push($"theme switched to {ThemeLabel()}", NotificationLevel.Info);
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Tab={_tabs.Items[_tabs.SelectedIndex]}  Tick={_tick:0000}  arrows/tab switch  t theme  d dialog  q quit";
        _status.RightText = $"{_statusText}  [{ThemeLabel()}]";

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(1, _tabs);
            window.Body(BuildTabContent(context));
            window.Overlay(new CenterLayout
            {
                Content = _dialog,
                Width = 44,
                Height = 8,
            });
            window.Footer(1, _status);
        });
    }

    private LayoutNode BuildTabContent(ScreenContext context)
    {
        return _tabs.SelectedIndex switch
        {
            0 => CreateOverviewTab(),
            1 => CreateInputsTab(),
            2 => CreateDataTab(context),
            3 => CreateFormsTab(),
            4 => CreateWorkflowTab(),
            5 => CreateWorkspaceTab(),
            6 => CreatePlotsTab(),
            _ => CreateTelemetryTab(),
        };
    }

    private RowLayout CreateOverviewTab()
    {
        var left = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _label,
                    Length = 7,
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

        var right = new ColumnLayout
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
                    Length = 8,
                },
            },
        };

        return new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = left, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = right, Length = LayoutLength.Fill() },
            },
        };
    }

    private ColumnLayout CreateInputsTab()
    {
        return new ColumnLayout
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
                    Length = 7,
                },
                new LayoutSlot
                {
                    Content = _comboBox,
                    Length = 7,
                },
                new LayoutSlot
                {
                    Content = _textArea,
                    Length = LayoutLength.Fill(),
                },
            },
        };
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
                    Length = 11,
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
                Length = Math.Min(30, Math.Max(22, context.Width / 4)),
            },
            Body = details,
            Gap = 1,
        };
    }

    private RowLayout CreateFormsTab()
    {
        return new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _form,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _fieldSet,
                    Length = LayoutLength.Fill(),
                },
            },
        };
    }

    private ColumnLayout CreateWorkflowTab()
    {
        return new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _splitView,
                    Length = 13,
                },
                new LayoutSlot
                {
                    Content = _dataForm,
                    Length = LayoutLength.Fill(),
                },
            },
        };
    }

    private ColumnLayout CreateWorkspaceTab()
    {
        return new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _paneTabs,
                    Length = 4,
                },
                new LayoutSlot
                {
                    Content = _workspace,
                    Length = 14,
                },
                new LayoutSlot
                {
                    Content = _terminalPanel,
                    Length = LayoutLength.Fill(),
                },
            },
        };
    }

    private RowLayout CreateTelemetryTab()
    {
        var left = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _gauge,
                    Length = 5,
                },
                new LayoutSlot
                {
                    Content = _statsCard,
                    Length = 6,
                },
                new LayoutSlot
                {
                    Content = _miniLog,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var right = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _barChart,
                    Length = 10,
                },
                new LayoutSlot
                {
                    Content = _lineChart,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        return new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = left,
                    Length = 24,
                },
                new LayoutSlot
                {
                    Content = right,
                    Length = LayoutLength.Fill(),
                },
            },
        };
    }

    private RowLayout CreatePlotsTab()
    {
        var left = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _linePlot,
                    Length = 10,
                },
                new LayoutSlot
                {
                    Content = _histogram,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var right = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _scatterPlot,
                    Length = 10,
                },
                new LayoutSlot
                {
                    Content = _sparkline,
                    Length = 4,
                },
                new LayoutSlot
                {
                    Content = _areaPlot,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        return new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = left,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = right,
                    Length = LayoutLength.Fill(),
                },
            },
        };
    }

    private void ApplyCurrentTheme()
    {
        var theme = _themeMode == GalleryThemeMode.Catppuccin
            ? TeaThemes.Catppuccin(CatppuccinVariant.Macchiato)
            : TeaThemes.RosePine(RosePineVariant.Moon);

        _tabs.ApplyTheme(theme);
        _label.ApplyTheme(theme);
        _button.ApplyTheme(theme);
        _progress.ApplyTheme(theme);
        _textInput.ApplyTheme(theme);
        _textArea.ApplyTheme(theme);
        _choice.ApplyTheme(theme);
        _comboBox.ApplyTheme(theme);
        _list.ApplyTheme(theme);
        _table.ApplyTheme(theme);
        _logs.ApplyTheme(theme);
        _dialog.ApplyTheme(theme);
        _form.ApplyTheme(theme);
        _fieldSet.ApplyTheme(theme);
        _wizard.ApplyTheme(theme);
        _inspector.ApplyTheme(theme);
        _splitView.ApplyTheme(theme);
        _dataForm.ApplyTheme(theme);
        _tree.ApplyTheme(theme);
        _notifications.ApplyTheme(theme);
        _gauge.ApplyTheme(theme);
        _statsCard.ApplyTheme(theme);
        _miniLog.ApplyTheme(theme);
        _barChart.ApplyTheme(theme);
        _lineChart.ApplyTheme(theme);
        _linePlot.ApplyTheme(theme);
        _scatterPlot.ApplyTheme(theme);
        _histogram.ApplyTheme(theme);
        _sparkline.ApplyTheme(theme);
        _areaPlot.ApplyTheme(theme);
        _paneTabs.ApplyTheme(theme);
        _workspace.ApplyTheme(theme);
        _terminalPanel.ApplyTheme(theme);
        _status.ApplyTheme(theme);

        var titleStyle = theme.Accent.Primary.WithBold();
        var focusedTitleStyle = theme.Focus.Title.Merge(theme.Accent.Primary).WithBold();
        _tabs.TitleStyle = titleStyle;
        _tabs.FocusedTitleStyle = focusedTitleStyle;
        _status.LeftTextStyle = theme.Text.Secondary.WithBold();
        _status.RightTextStyle = theme.Accent.Secondary;
        _status.FillStyle = theme.Surface.Panel.Merge(theme.Text.Muted);

        var selectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        var focusedBorderStyle = theme.Border.Focused.Merge(theme.Focus.Border);

        _choice.Glyphs = new DropdownGlyphSet("⌄", "⌃", "▸", "◆");
        _choice.BorderStyleText = theme.Border.Strong;
        _choice.FocusedBorderStyleText = focusedBorderStyle;
        _choice.TitleStyle = theme.Accent.Primary.WithBold();
        _choice.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _choice.ValueStyle = theme.Text.Primary.WithBold();
        _choice.OptionStyle = theme.Text.Secondary;
        _choice.SelectedOptionStyle = selectedRowStyle.WithBold();
        _choice.HoveredOptionStyle = theme.Accent.Secondary.WithUnderline();
        _choice.HoveredValueStyle = theme.Accent.Primary.WithUnderline();

        _comboBox.Glyphs = new DropdownGlyphSet("⌄", "⌃", "▶", "◆");
        _comboBox.BorderStyleText = theme.Border.Strong;
        _comboBox.FocusedBorderStyleText = focusedBorderStyle;
        _comboBox.TitleStyle = theme.Accent.Primary.WithBold();
        _comboBox.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _comboBox.ValueTextStyle = theme.Text.Primary.WithBold();
        _comboBox.PlaceholderTextStyle = theme.Text.Muted.WithItalic();
        _comboBox.OptionStyle = theme.Text.Secondary;
        _comboBox.SelectedOptionStyle = selectedRowStyle.WithBold();
        _comboBox.HoveredOptionStyle = theme.Accent.Secondary.WithUnderline();
        _comboBox.HoveredValueStyle = theme.Accent.Primary.WithUnderline();

        _tree.Glyphs = new TreeViewGlyphSet("▾", "▸", "◦");
        _tree.BorderStyleText = theme.Border.Strong;
        _tree.FocusedBorderStyleText = focusedBorderStyle;
        _tree.TitleStyle = theme.Accent.Primary.WithBold();
        _tree.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _tree.BranchStyle = theme.Text.Secondary;
        _tree.LeafStyle = theme.Text.Primary;
        _tree.SelectedItemStyle = selectedRowStyle.WithBold();
        _tree.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();

        _list.BorderStyleText = theme.Border.Strong;
        _list.FocusedBorderStyleText = focusedBorderStyle;
        _list.TitleStyle = theme.Accent.Primary.WithBold();
        _list.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _list.DefaultRowStyle = theme.Text.Secondary;
        _list.SelectedRowStyle = selectedRowStyle.WithBold();
        _list.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();

        _table.BorderStyleText = theme.Border.Strong;
        _table.FocusedBorderStyleText = focusedBorderStyle;
        _table.TitleStyle = theme.Accent.Primary.WithBold();
        _table.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _table.HeaderStyle = theme.Text.Primary.WithBold();
        _table.RowStyle = theme.Text.Secondary;
        _table.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();
        _table.SelectedRowStyle = selectedRowStyle.WithBold();
    }

    private string ThemeLabel() =>
        _themeMode == GalleryThemeMode.Catppuccin
            ? "Catppuccin"
            : "Rose Pine";

    private void UpdatePlotWidgets()
    {
        var lineWindow = 72;
        var p50Samples = new double[lineWindow];
        var p95Samples = new double[lineWindow];
        for (var index = 0; index < lineWindow; index++)
        {
            var sampleTick = _tick - (lineWindow - 1 - index);
            if (sampleTick < 0)
            {
                sampleTick = 0;
            }

            var baseline = 18 + (Math.Sin(sampleTick / 7d) * 6) + (sampleTick % 4);
            p50Samples[index] = baseline;
            p95Samples[index] = baseline + 11 + (Math.Cos(sampleTick / 11d) * 3);
        }

        _lineP50Series.SetSamples(p50Samples);
        _lineP95Series.SetSamples(p95Samples);

        var latestP50 = p50Samples[lineWindow - 1];
        var latestP95 = p95Samples[lineWindow - 1];
        _histogram.SetBuckets(
        [
            new HistogramBucket("p50", Math.Round(latestP50, 2)),
            new HistogramBucket("p75", Math.Round(latestP50 + 3.8, 2)),
            new HistogramBucket("p90", Math.Round(latestP50 + 7.4, 2)),
            new HistogramBucket("p99", Math.Round(latestP95 + 4.2, 2)),
        ]);

        var pointCount = 36;
        var points = new ScatterPlotPoint[pointCount];
        for (var index = 0; index < pointCount; index++)
        {
            var pointTick = _tick - (pointCount - 1 - index);
            if (pointTick < 0)
            {
                pointTick = 0;
            }

            var throughput = 120 + (Math.Sin(pointTick / 4d) * 26) + (pointTick % 9);
            var errors = Math.Max(0, 1.5 + (Math.Cos(pointTick / 6d) * 1.8) + ((pointTick % 11) == 0 ? 2.5 : 0));
            points[index] = new ScatterPlotPoint(throughput, errors);
        }

        _scatterPlot.SetPoints(points);

        var cpu = 36 + (Math.Sin(_tick / 5d) * 18) + (_tick % 6);
        var queueDepth = 8 + Math.Max(0, (Math.Sin(_tick / 9d) * 6)) + ((_tick + 2) % 4);
        _sparkline.Append(cpu);
        _areaPlot.Append(queueDepth);
    }

    private sealed class GalleryProfileModel
    {
        public string Service { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string Slo { get; set; } = string.Empty;

        public string Budget { get; set; } = string.Empty;
    }
}
