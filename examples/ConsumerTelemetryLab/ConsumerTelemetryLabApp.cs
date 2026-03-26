using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed partial class ConsumerTelemetryLabApp : TeaApp
{
    internal static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly Random _random = new(260326);

    private readonly Tabs _tabs = new("Overview", "Capacity", "Incidents")
    {
        Title = "Consumer Telemetry Lab",
        FocusMarker = "◆",
    };

    private readonly ListView<string> _clusterFilter = new(static item => item)
    {
        Title = "Cluster Filter",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        RowMarkers = new ListViewMarkerSet("·", "▸", "◆"),
        PageSize = 5,
    };

    private readonly ListView<ServiceState> _serviceList = new(static service =>
        $"{service.Name,-12} {service.Health,-8} p95:{service.P95Ms,4:0} err:{service.ErrorRatePct,4:0.0}%")
    {
        Title = "Services",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        RowMarkers = new ListViewMarkerSet(" ", "▸", "◆"),
        PageSize = 12,
    };

    private readonly Table _serviceTable = new("Svc", "Health", "CPU", "Mem", "P95", "Err%", "Req/s")
    {
        Title = "Service Capacity",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        PageSize = 10,
    };

    private readonly Table _incidentTable = new("Id", "Sev", "Service", "State", "Age", "Summary")
    {
        Title = "Incident Queue",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        PageSize = 8,
    };

    private readonly Label _incidentDetail = new()
    {
        Title = "Incident Drilldown",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Notifications _alerts = new()
    {
        Title = "Alerts",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        MaxItems = 80,
    };

    private readonly LogView _activity = new()
    {
        Title = "Activity",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly StatusBar _status = new()
    {
        Fill = ' ',
    };

    private readonly PlotPanel _overviewPlotPanel = new()
    {
        Title = "Live Telemetry",
        Border = BorderStyle.Rounded,
        FocusMarker = "◆",
        Padding = Thickness.All(1),
        Options = new PlotPanelOptions(Columns: 2, Spacing: 1),
    };

    private readonly Sparkline _cpuTrend = new(capacity: 180)
    {
        Title = "CPU %",
        Border = BorderStyle.SingleLine,
        FocusMarker = "◆",
        MinValue = 0,
        MaxValue = 100,
        Options = new SparklineOptions(Steps: "▁▂▃▄▅▆▇█", ShowStats: true, Legend: "avg"),
    };

    private readonly AreaPlot _memoryTrend = new(capacity: 180)
    {
        Title = "Memory (GiB)",
        Border = BorderStyle.SingleLine,
        FocusMarker = "◆",
        MinValue = 2,
        MaxValue = 64,
        Options = new AreaPlotOptions(
            FillGlyph: '░',
            LineGlyph: '█',
            ShowBaseline: true,
            BaselineGlyph: '─',
            ShowStats: true,
            Legend: "rss"),
    };

    private readonly LineSeries _latP50 = new("p50");
    private readonly LineSeries _latP95 = new("p95");
    private readonly LineSeries _latP99 = new("p99");

    private readonly LinePlot _latencyTimeline = new()
    {
        Title = "Latency Timeline (ms)",
        Border = BorderStyle.SingleLine,
        FocusMarker = "◆",
        MinValue = 0,
        MaxValue = 450,
    };

    private readonly ScatterPlot _jitterPlot = new()
    {
        Title = "Jitter Cloud",
        FocusMarker = "◆",
        Capacity = 120,
        Options = new ScatterPlotOptions(
            ShowAxes: true,
            ShowLabels: false,
            Legend: "samples",
            XLabel: "tick",
            YLabel: "p95",
            PointGlyph: '◆'),
    };

    private readonly Histogram _errorMix = new()
    {
        Title = "Error Mix",
        FocusMarker = "◆",
        MaxValue = 100,
        Options = new HistogramOptions(
            ShowAxes: true,
            ShowBucketLabels: true,
            ShowScale: true,
            Legend: "ratio",
            XLabel: "type",
            YLabel: "%",
            BarGlyph: '▓'),
    };

    private readonly List<ServiceState> _services =
    [
        new("api", "api-gateway", "prod-us", "iad"),
        new("worker", "worker", "prod-us", "iad"),
        new("scheduler", "scheduler", "prod-eu", "fra"),
        new("billing", "billing", "prod-eu", "fra"),
        new("search", "search", "edge", "sea"),
        new("cache", "cache", "edge", "sea"),
        new("ingest", "ingest", "batch", "iad"),
    ];

    private readonly List<IncidentState> _incidents = [];
    private readonly List<string> _clusters = ["all", "prod-us", "prod-eu", "edge", "batch"];

    private LabThemeMode _themeMode = LabThemeMode.Catppuccin;
    private LoadProfile _profile = LoadProfile.Nominal;
    private string _activeCluster = "all";
    private string _selectedServiceId = "api";
    private string? _selectedIncidentId;
    private string _tableSyncNote = "table sync: direct (user click)";
    private bool _paused;
    private int _tick;
    private int _nextIncident = 420;

    public ConsumerTelemetryLabApp()
    {
        _overviewPlotPanel.SetPlots([
            _cpuTrend,
            _memoryTrend,
            _latencyTimeline,
            _jitterPlot,
            _errorMix,
        ]);

        _latencyTimeline
            .ConfigureAxes(showAxes: true, xLabel: "samples", sharedAxisLabel: "ms")
            .ConfigureGrid(showGrid: true)
            .ConfigureLegend(showLegend: true);
        _latencyTimeline.SetSeries([_latP50, _latP95, _latP99]);

        _tabs.SelectionChanged += (_, args) => _activity.Append($"view -> {args.SelectedItem}");

        _clusterFilter.SetItems(_clusters);
        _clusterFilter.SelectionChanged += (_, args) =>
        {
            if (string.IsNullOrWhiteSpace(args.SelectedItem))
            {
                return;
            }

            _activeCluster = args.SelectedItem;
            RefreshListsAndTables();
            _activity.Append($"filter -> cluster:{_activeCluster}");
        };

        _serviceList.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _selectedServiceId = args.SelectedItem.Id;
            TryRequestIncidentDrilldownForService(args.SelectedItem.Id, "service-select");
        };

        _serviceTable.SelectionChanged += (_, args) =>
        {
            var serviceName = args.SelectedItem is { Count: > 0 } ? args.SelectedItem[0] : string.Empty;
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                return;
            }

            var service = _services.FirstOrDefault(candidate =>
                string.Equals(candidate.Name, serviceName, StringComparison.Ordinal));
            if (service is null)
            {
                return;
            }

            _selectedServiceId = service.Id;
            SyncServiceListSelection();
        };

        _incidentTable.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not { Count: > 0 })
            {
                return;
            }

            _selectedIncidentId = args.SelectedItem[0];
            _tableSyncNote = "table sync: direct (user clicked row)";
        };

        _alerts.SelectionChanged += (_, args) =>
        {
            var id = args.SelectedItem?.Id;
            if (string.IsNullOrWhiteSpace(id) || !id.StartsWith("inc:", StringComparison.Ordinal))
            {
                return;
            }

            RequestIncidentDrilldown(id[4..], "alert-select");
        };

        SeedInitialMetrics();
        SeedInitialIncidents();

        RefreshListsAndTables();
        ApplyThemeAndStyles();

        _alerts.Push("Telemetry lab initialized", NotificationLevel.Success, "sys:init");
        _alerts.Push("Single-click pointer mode active", NotificationLevel.Info, "sys:pointer");
        _activity.Append("startup complete");
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(280), static now => new TelemetryTick(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key)
        {
            if (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            {
                return TeaEffects.Quit;
            }

            if (key.IsCharacter('t') || key.IsCharacter('t', ModifierKeys.Ctrl))
            {
                _themeMode = _themeMode == LabThemeMode.Catppuccin ? LabThemeMode.RosePine : LabThemeMode.Catppuccin;
                ApplyThemeAndStyles();
                _alerts.Push($"theme -> {ThemeLabel()}", NotificationLevel.Info, $"sys:theme:{_tick}");
                return null;
            }

            if (key.IsCharacter('p'))
            {
                _paused = !_paused;
                _alerts.Push(_paused ? "stream paused" : "stream resumed", NotificationLevel.Info, $"sys:pause:{_tick}");
                return null;
            }

            if (key.IsCharacter('l'))
            {
                _profile = _profile == LoadProfile.Nominal ? LoadProfile.Incident : LoadProfile.Nominal;
                _alerts.Push($"load profile -> {ProfileLabel()}", NotificationLevel.Warning, $"sys:load:{_tick}");
                return null;
            }

            if (key.IsCharacter('a'))
            {
                _alerts.MarkAllRead();
                _activity.Append("alerts marked read");
                return null;
            }

            if (key.IsCharacter('n'))
            {
                SelectNextService();
                return null;
            }

            if (key.IsCharacter('i'))
            {
                var incident = MostRecentIncidentForService(_selectedServiceId);
                if (incident is not null)
                {
                    RequestIncidentDrilldown(incident.Id, "hotkey-i");
                }

                return null;
            }

            if (key.IsCharacter('r'))
            {
                ResetTelemetryAndIncidents();
                return null;
            }
        }

        if (message is TelemetryTick tick)
        {
            _tick++;
            if (!_paused)
            {
                SimulateTick(tick.At);
                RefreshListsAndTables();
            }

            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _incidentDetail.Text = BuildIncidentDetail(context);

        _status.LeftText =
            $"theme:{ThemeLabel()} profile:{ProfileLabel()} cluster:{_activeCluster} service:{SelectedService()?.Name ?? "-"} tick:{_tick:0000}";
        _status.RightText =
            $"q quit  t theme  p pause  l load  n next-svc  i drilldown  a ack  r reset  click tabs/lists/table  {_tableSyncNote}";

        var leftPanel = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                LayoutSlot.Fixed(_clusterFilter, 8),
                LayoutSlot.Fill(_serviceList),
            },
        };

        var rightPanel = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                LayoutSlot.Fixed(_incidentDetail, 11),
                LayoutSlot.Fill(_alerts),
            },
        };

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            window.Header(1, _tabs);
            window.Left(Math.Min(36, Math.Max(28, context.Width / 4)), leftPanel);
            window.Right(Math.Min(50, Math.Max(38, context.Width / 3)), rightPanel);
            window.Body(BuildBodyForSelectedTab());
            window.Footer(1, _status);
        });
    }

    private LayoutNode BuildBodyForSelectedTab()
    {
        return _tabs.SelectedIndex switch
        {
            1 => new ColumnLayout
            {
                Gap = 1,
                Items =
                {
                    LayoutSlot.Fixed(_serviceTable, 14),
                    LayoutSlot.Fill(_activity),
                },
            },
            2 => new ColumnLayout
            {
                Gap = 1,
                Items =
                {
                    LayoutSlot.Fixed(_incidentTable, 14),
                    LayoutSlot.Fill(_activity),
                },
            },
            _ => new ColumnLayout
            {
                Gap = 1,
                Items =
                {
                    LayoutSlot.Fixed(_overviewPlotPanel, 16),
                    LayoutSlot.Fill(_activity),
                },
            },
        };
    }

    private void ApplyThemeAndStyles()
    {
        var theme = _themeMode == LabThemeMode.Catppuccin
            ? TeaThemes.Catppuccin(CatppuccinVariant.Macchiato)
            : TeaThemes.RosePine(RosePineVariant.Moon);

        ThemeScope.Apply(
            theme,
            _tabs,
            _clusterFilter,
            _serviceList,
            _serviceTable,
            _incidentTable,
            _incidentDetail,
            _alerts,
            _activity,
            _status,
            _overviewPlotPanel,
            _cpuTrend,
            _memoryTrend,
            _latencyTimeline,
            _jitterPlot,
            _errorMix);

        var selected = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _tabs.TitleStyle = theme.Accent.Primary.WithBold();
        _tabs.FocusedTitleStyle = theme.Focus.Title.Merge(theme.Accent.Secondary).WithBold();

        _clusterFilter.SelectedRowStyle = selected;
        _clusterFilter.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();

        _serviceList.SelectedRowStyle = selected;
        _serviceList.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();

        _serviceTable.HeaderStyle = theme.Text.Primary.WithBold();
        _serviceTable.SelectedRowStyle = selected;
        _serviceTable.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();

        _incidentTable.HeaderStyle = theme.Text.Primary.WithBold();
        _incidentTable.SelectedRowStyle = selected;
        _incidentTable.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();

        _alerts.WarningItemStyle = theme.State.Warning.WithBold();
        _alerts.ErrorItemStyle = theme.State.Error.WithBold();
        _alerts.SuccessItemStyle = theme.State.Success.WithBold();

        _latP50.Style = theme.Accent.Primary;
        _latP95.Style = theme.Accent.Secondary;
        _latP99.Style = theme.State.Warning;
        _latP50.PointGlyph = '●';
        _latP95.PointGlyph = '◆';
        _latP99.PointGlyph = '▲';

        _status.LeftTextStyle = theme.Text.Secondary.WithBold();
        _status.RightTextStyle = theme.Text.Primary;
        _status.FillStyle = theme.Surface.Panel;
    }

    private string ThemeLabel() => _themeMode == LabThemeMode.Catppuccin ? "catppuccin" : "rose-pine";

    private string ProfileLabel() => _profile == LoadProfile.Nominal ? "nominal" : "incident";
}
