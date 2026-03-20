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
    private readonly Tabs _tabs = new("Basics", "Inputs", "Data", "Overlay", "Advanced", "Plots", "Telemetry");
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
        Border = BorderStyle.SingleLine,
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
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Options = new SparklineOptions(
            ShowStats: true,
            Legend: "cpu%"),
    };

    private readonly AreaPlot _areaPlot = new(capacity: 96)
    {
        Title = "Area Plot",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Options = new AreaPlotOptions(
            ShowStats: true,
            Legend: "queue depth"),
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
        _miniLog.Append("telemetry ready");
        _barChart.SetBars(
        [
            new BarPoint("api", 42),
            new BarPoint("worker", 58),
            new BarPoint("events", 33),
        ]);
        _statsCard.SetItems(
        [
            new StatItem("raw", "yes"),
            new StatItem("mouse", "yes"),
            new StatItem("paste", "yes"),
        ]);

        _linePlot.SetSeries([_lineP50Series, _lineP95Series]);
        UpdatePlotWidgets();
    }

    public override TeaEffect? Initialize() => TeaEffects.Tick(TimeSpan.FromMilliseconds(300), static now => new GalleryTick(now));

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
            _statsCard.SetValue("tick", _tick.ToString("0000", System.Globalization.CultureInfo.InvariantCulture));
            _statsCard.SetValue("focus", Context.HasFocus ? "yes" : "no");
            if (_tick % 10 == 0)
            {
                _logs.Append($"{tick.At:HH:mm:ss} pulse={_tick:0000}");
                _miniLog.Append($"{tick.At:HH:mm:ss} p95={20 + (_tick % 15)}ms");
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
        _status.LeftText = $"Tab={_tabs.Items[_tabs.SelectedIndex]}   Tick={_tick:0000}   arrows/tab switch   d dialog   q quit";
        _status.RightText = _statusText;

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(1, _tabs);
            window.Body(BuildTabContent(context));
            window.Overlay(new CenterLayout
            {
                Content = _dialog,
                Width = 42,
                Height = 8,
            });
            window.Footer(1, _status);
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
            4 => CreateAdvancedTab(),
            5 => CreatePlotsTab(),
            _ => CreateTelemetryTab(),
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
}
