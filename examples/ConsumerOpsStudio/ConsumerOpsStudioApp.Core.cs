using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

internal sealed partial class ConsumerOpsStudioApp : TeaApp
{
    private readonly SideNavRail _navigation = new()
    {
        Title = "Workspace",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly CommandBar _commandBar = new()
    {
        Title = "Consumer Ops Studio",
        FocusMarker = "◆",
        ItemSeparator = "  ",
    };

    private readonly Tabs _tabs = new("Incidents", "Deployments", "SLO")
    {
        Title = "Panel",
        FocusMarker = "◆",
    };

    private readonly ListView<ServiceSnapshot> _serviceList = new(static service =>
        $"{service.Name,-20} {service.Region,-10} {service.Status,-10} p95 {service.LatencyP95,4:0}ms")
    {
        Title = "Services",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Table _workTable = new("Item", "Service", "Signal", "Owner", "State", "Detail")
    {
        Title = "Work Queue",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        PageSize = 8,
        FocusMarker = "◆",
    };

    private readonly LineSeries _p95Series = new("p95") { Capacity = 180 };
    private readonly LineSeries _p99Series = new("p99") { Capacity = 180 };
    private readonly LineSeries _errorSeries = new("error%") { Capacity = 180 };

    private readonly LinePlot _latencyPlot = new()
    {
        Title = "Selected Service Trend",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MinValue = 0,
        MaxValue = 260,
        Options = new LinePlotOptions(
            ShowAxes: true,
            ShowGrid: true,
            ShowLegend: true,
            ShowStats: true,
            XLabel: "ticks",
            YLabel: "ms / %"),
    };

    private readonly Gauge _errorBudgetGauge = new()
    {
        Title = "Error Budget",
        MinValue = 0,
        MaxValue = 100,
        Label = "100%",
    };

    private readonly Gauge _queueDepthGauge = new()
    {
        Title = "Queue Depth",
        MinValue = 0,
        MaxValue = 4000,
        Label = "0 msgs",
    };

    private readonly Button _ackButton = new()
    {
        Text = "Acknowledge",
        Description = "a",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Button _rollbackButton = new()
    {
        Text = "Rollback",
        Description = "r",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Button _freezeButton = new()
    {
        Text = "Freeze Writes",
        Description = "f",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Label _selectionSummary = new()
    {
        Title = "Selection",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Notifications _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MaxItems = 96,
        FocusMarker = "◆",
    };

    private readonly LogView _activityLog = new()
    {
        Title = "Activity Log",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly CommandPalette _palette = new()
    {
        Title = "Quick Actions",
        FocusMarker = "◆",
        ShowFocusMarker = true,
    };

    private readonly Dialog _confirmDialog = new()
    {
        Title = "Confirm Action",
        BodyLines =
        [
            "Press Enter to confirm.",
            "Press Esc to cancel.",
        ],
        FocusMarker = "◆",
    };

    private readonly StatusBar _status = new();
    private readonly Random _random = new(2227);
    private readonly List<ServiceSnapshot> _services = ConsumerOpsSeedData.CreateServices();
    private readonly List<IncidentTicket> _incidents = ConsumerOpsSeedData.CreateIncidents();
    private readonly List<DeploymentRun> _deployments = ConsumerOpsSeedData.CreateDeployments();
    private readonly List<string> _visibleWorkItemIds = [];

    private string _selectedServiceId = string.Empty;
    private string _selectedIncidentId = string.Empty;
    private string _selectedDeploymentId = string.Empty;
    private string _activeNavigationId = "overview";
    private string _statusText = "ready";
    private bool _alertThemeEnabled;
    private int _tick;
    private PendingDialogAction _pendingDialogAction;
    private string _pendingServiceId = string.Empty;

    public ConsumerOpsStudioApp()
    {
        _navigation.SetItems(ConsumerOpsSeedData.CreateNavigation());
        _commandBar.SetItems(ConsumerOpsSeedData.CreateCommandBarItems());
        _palette.SetItems(ConsumerOpsSeedData.CreatePaletteItems());
        _latencyPlot.SetSeries([_p95Series, _p99Series, _errorSeries]);

        _selectedServiceId = _services[0].Id;
        _selectedIncidentId = _incidents[0].Id;
        _selectedDeploymentId = _deployments[0].Id;

        WireEvents();
        ApplyLocalOverrides();
        SyncServiceListSelection();
        RefreshWorkRows();

        _notifications.Push("Consumer Ops Studio initialized", NotificationLevel.Success);
        _notifications.Push("Use mouse + keyboard: Tab focus, Enter activate, Ctrl+P palette", NotificationLevel.Info);
        AppendLog("Boot completed with seeded telemetry and workloads.");
    }

    public override TeaEffect? Initialize()
    {
        return TeaEffects.Periodic(TimeSpan.FromMilliseconds(320), static now => new OpsTick(now));
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key)
        {
            if (key.IsCharacter('c', ModifierKeys.Ctrl) || key.IsCharacter('q'))
            {
                return TeaEffects.Quit;
            }

            if (key.IsCharacter('p', ModifierKeys.Ctrl))
            {
                _palette.Open();
                _statusText = "palette open";
                return null;
            }

            if (key.IsCharacter('t'))
            {
                ToggleTheme();
                return null;
            }

            if (key.IsCharacter('1'))
            {
                _tabs.Select((int)OpsPanelTab.Incidents);
                return null;
            }

            if (key.IsCharacter('2'))
            {
                _tabs.Select((int)OpsPanelTab.Deployments);
                return null;
            }

            if (key.IsCharacter('3'))
            {
                _tabs.Select((int)OpsPanelTab.Slo);
                return null;
            }
        }

        if (message is OpsTick)
        {
            _tick++;
            AdvanceServiceSignals();
            AdvanceIncidents();
            AdvanceDeployments();
            AppendPlotSample();
            SyncServiceListSelection();
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshWorkRows();
        UpdateSummaryAndGauges(context);
        UpdateButtonState();

        _status.LeftText = "Tab focus  Enter activate  Ctrl+P palette  a/r/s/f shortcuts  t theme  q quit";
        _status.RightText = $"tick:{_tick:0000} nav:{_activeNavigationId} tab:{_tabs.Items[_tabs.SelectedIndex]} {_statusText}";

        var actions = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _ackButton,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _rollbackButton,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _freezeButton,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var gauges = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _errorBudgetGauge,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _queueDepthGauge,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var leftPanel = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _serviceList,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = gauges,
                    Length = 5,
                },
                new LayoutSlot
                {
                    Content = actions,
                    Length = 5,
                },
                new LayoutSlot
                {
                    Content = _selectionSummary,
                    Length = 8,
                },
            },
        };

        var logsAndNotifications = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _activityLog,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _notifications,
                    Length = Math.Min(46, Math.Max(36, context.Width / 4)),
                },
            },
        };

        var rightPanel = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _tabs,
                    Length = 1,
                },
                new LayoutSlot
                {
                    Content = _workTable,
                    Length = 12,
                },
                new LayoutSlot
                {
                    Content = _latencyPlot,
                    Length = 14,
                },
                new LayoutSlot
                {
                    Content = logsAndNotifications,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var body = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = leftPanel,
                    Length = Math.Min(58, Math.Max(46, context.Width / 3)),
                },
                new LayoutSlot
                {
                    Content = rightPanel,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        LayoutNode? overlay = null;
        if (_confirmDialog.IsVisible)
        {
            overlay = new CenterLayout
            {
                Content = _confirmDialog,
                Width = Math.Min(72, Math.Max(52, context.Width - 8)),
                Height = 10,
            };
        }
        else if (_palette.IsVisible)
        {
            overlay = new CenterLayout
            {
                Content = _palette,
                Width = Math.Min(86, Math.Max(56, context.Width - 8)),
                Height = Math.Min(15, Math.Max(10, context.Height - 8)),
            };
        }

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(1, _commandBar);
            window.Left(Math.Min(26, Math.Max(22, context.Width / 7)), _navigation);
            window.Body(body);

            if (overlay is not null)
            {
                window.Overlay(overlay);
            }

            window.Footer(1, _status);
        });
    }
}
