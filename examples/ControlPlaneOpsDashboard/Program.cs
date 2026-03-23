using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<ControlPlaneOpsDashboardApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = ControlPlaneOpsDashboardApp.DefaultTheme;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Ops Control Plane",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record OpsPulse(DateTimeOffset At) : Message;

internal sealed partial class ControlPlaneOpsDashboardApp : TeaApp
{
    internal static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);
    private static readonly TeaTheme RosePineTheme = TeaThemes.RosePine(RosePineVariant.Moon);

    private readonly Tabs _tabs = new("Overview", "Fleet", "Incidents", "Analytics", "Automation")
    {
        Title = "Ops Control Plane",
        FocusMarker = "◆",
    };

    private readonly SideNavRail _rail = new()
    {
        Title = "Navigation",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Button _deployButton = new()
    {
        Text = "Queue Deploy",
        Description = "d opens confirmation",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Button _ackButton = new()
    {
        Text = "Acknowledge",
        Description = "a acknowledges selected incident",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Label _selectionSummary = new()
    {
        Title = "Selection",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly DashboardGrid _overviewGrid = new()
    {
        Title = "Control Plane Health",
        Border = BorderStyle.Rounded,
        TileBorder = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly BulletChart _latencyBudget = new()
    {
        Title = "Latency SLO",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly ListView<ServiceNode> _serviceList = new(static service =>
        $"{service.Name,-14} {service.Region,-10} p95={service.P95Ms,3}ms cpu={service.CpuPercent,2}%")
    {
        Title = "Fleet Services",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Table _fleetTable = new("Service", "State", "P95", "CPU", "Req/s", "Err%")
    {
        Title = "Fleet Metrics",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        PageSize = 9,
        FocusMarker = "◆",
    };

    private readonly HealthBoard _healthBoard = new()
    {
        Title = "Incident Health Board",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly TaskRunnerPanel _pipelinePanel = new()
    {
        Title = "Pipeline Tasks",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        ShowTimestamp = false,
    };

    private readonly Notifications _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MaxItems = 96,
        FocusMarker = "◆",
    };

    private readonly LogView _activity = new()
    {
        Title = "Activity Stream",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Dialog _deployDialog = new()
    {
        Title = "Deploy Confirmation",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();

    private readonly List<ServiceNode> _services =
    [
        new ServiceNode("edge", "Edge API", "us-east-1", "Healthy", 24, 22, 860, 22),
        new ServiceNode("checkout", "Checkout", "us-east-1", "Healthy", 31, 30, 520, 48),
        new ServiceNode("billing", "Billing", "eu-west-1", "Warning", 67, 44, 188, 132),
        new ServiceNode("search", "Search", "us-east-1", "Healthy", 42, 35, 610, 56),
        new ServiceNode("mailer", "Mailer", "ap-southeast-1", "Degraded", 88, 68, 104, 212),
        new ServiceNode("auth", "Auth", "eu-central-1", "Healthy", 36, 24, 441, 39),
    ];

    private readonly List<EndpointNode> _endpoints =
    [
        new EndpointNode("/v1/orders", 52, 62, 330),
        new EndpointNode("/v1/checkout", 74, 138, 206),
        new EndpointNode("/v1/refunds", 89, 205, 74),
        new EndpointNode("/v1/catalog", 47, 34, 426),
        new EndpointNode("/v1/search", 58, 81, 271),
        new EndpointNode("/v1/payments", 93, 240, 98),
    ];

    private readonly HashSet<string> _acknowledgedServices = new(StringComparer.Ordinal);
    private readonly Random _random = new(417);

    private bool _useRosePine;
    private int _tick;
    private string _statusText = "Ready";
    private string _selectedServiceId = "edge";

    public ControlPlaneOpsDashboardApp()
    {
        _rail.SetItems(
        [
            new NavItem("overview", "Overview", icon: "OVR"),
            new NavItem("fleet", "Fleet", icon: "FLT"),
            new NavItem("incidents", "Incidents", icon: "INC", badge: "hot"),
            new NavItem("analytics", "Analytics", icon: "CHT"),
            new NavItem("automation", "Automation", icon: "OPS"),
            new NavItem("runbooks", "Runbooks", icon: "RBK", isDisabled: true),
        ]);

        _tabs.SelectionChanged += (_, args) =>
        {
            SyncRailWithTab();
            _statusText = $"tab -> {args.SelectedItem}";
            AppendActivity($"Top navigation -> {args.SelectedItem}");
        };

        _rail.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _statusText = $"rail -> {args.SelectedItem.Label}";
            var target = args.SelectedItem.Id switch
            {
                "overview" => 0,
                "fleet" => 1,
                "incidents" => 2,
                "analytics" => 3,
                "automation" => 4,
                _ => _tabs.SelectedIndex,
            };
            _tabs.SetSelectedIndex(target);
        };

        _rail.Activated += (_, args) =>
        {
            if (string.Equals(args.SelectedItem.Id, "runbooks", StringComparison.Ordinal))
            {
                _statusText = "Runbooks are disabled in this mock";
            }
        };

        _serviceList.SetItems(_services);
        _serviceList.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _selectedServiceId = args.SelectedItem.Id;
            _statusText = $"selected service -> {args.SelectedItem.Name}";
            SelectHealthService(args.SelectedItem.Id);
        };

        _healthBoard.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _selectedServiceId = args.SelectedItem.Id;
            _statusText = $"incident row -> {args.SelectedItem.Name}";
        };

        _deployButton.Activated += (_, _) => OpenDeployDialog();
        _ackButton.Activated += (_, _) => AcknowledgeSelectedIncident();
        _deployDialog.Closed += (_, args) => HandleDeployDialogClosed(args.Result);

        _latencyBudget.SetRanges(
        [
            new BulletRange(0, 45, BulletRangeKind.Normal, "Healthy"),
            new BulletRange(45, 60, BulletRangeKind.Warning, "Risk"),
            new BulletRange(60, 120, BulletRangeKind.Critical, "Breach"),
        ]);
        _latencyBudget.SetTarget(60);

        _notifications.Push("Control-plane dashboard booted", NotificationLevel.Success);
        _notifications.Push("Ctrl+P opens quick actions", NotificationLevel.Info);
        _notifications.Push("Press d to queue deployment", NotificationLevel.Info);
        AppendActivity("Dashboard startup complete.");

        SeedPipelineTasks();
        InitializeAnalyticsFeatures();
        InitializeAutomationFeatures();
        UpdateOverviewState();
        UpdateAnalyticsState();
        UpdateAutomationState();
        ApplyThemeAndOverrides();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(450), static now => new OpsPulse(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key)
        {
            if (HandleAutomationHotKeys(key))
            {
                return null;
            }

            if (key.IsCharacter('c', ModifierKeys.Ctrl))
            {
                return TeaEffects.Quit;
            }

            if (key.IsCharacter('1')) _tabs.SetSelectedIndex(0);
            if (key.IsCharacter('2')) _tabs.SetSelectedIndex(1);
            if (key.IsCharacter('3')) _tabs.SetSelectedIndex(2);
            if (key.IsCharacter('4')) _tabs.SetSelectedIndex(3);
            if (key.IsCharacter('5')) _tabs.SetSelectedIndex(4);

            if (key.IsCharacter('d'))
            {
                OpenDeployDialog();
                return null;
            }

            if (key.IsCharacter('a'))
            {
                AcknowledgeSelectedIncident();
                return null;
            }

            if (key.IsCharacter('n'))
            {
                _notifications.Push($"manual note @ tick {_tick}", NotificationLevel.Info);
                _statusText = "manual notification added";
                return null;
            }

            if (key.IsCharacter('p', ModifierKeys.Ctrl))
            {
                if (_quickOpenOverlay.IsOpen)
                {
                    _quickOpenOverlay.Close();
                    _statusText = "quick-open closed";
                }
                else
                {
                    _quickOpenOverlay.Open();
                    _statusText = "quick-open opened";
                }

                return null;
            }

            if (key.IsCharacter('t'))
            {
                _useRosePine = !_useRosePine;
                ApplyThemeAndOverrides();
                _statusText = $"theme -> {CurrentThemeName()}";
                _notifications.Push($"Theme switched -> {CurrentThemeName()}", NotificationLevel.Info);
                return null;
            }
        }

        if (message is OpsPulse pulse)
        {
            _tick++;
            SimulateTelemetry(pulse.At);
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        return _tabs.SelectedIndex switch
        {
            1 => BuildFleetScreen(context),
            2 => BuildIncidentsScreen(context),
            3 => BuildAnalyticsScreen(context),
            4 => BuildAutomationScreen(context),
            _ => BuildOverviewScreen(context),
        };
    }

    private Screen BuildOverviewScreen(ScreenContext context)
    {
        _fleetTable.SetRows(BuildFleetRows());
        _selectionSummary.Text = BuildSelectionSummary(context);
        ConfigureStatus("overview");

        var actionRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _deployButton, Length = 28 },
                new LayoutSlot { Content = _ackButton, Length = 28 },
                new LayoutSlot { Content = _selectionSummary, Length = LayoutLength.Fill() },
            },
        };

        var summaryRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _overviewGrid, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _latencyBudget, Length = Math.Min(50, Math.Max(36, context.Width / 3)) },
            },
        };

        var incidentRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _healthBoard, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _pipelinePanel, Length = Math.Min(58, Math.Max(42, context.Width / 3)) },
            },
        };

        var bottomRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _activity, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _notifications, Length = Math.Min(46, Math.Max(34, context.Width / 4)) },
            },
        };

        return BuildWithChrome(
            context,
            body =>
            {
                body.Column(column =>
                {
                    column.Gap(1);
                    column.Fixed(5, actionRow);
                    column.Fixed(10, summaryRow);
                    column.Fixed(10, incidentRow);
                    column.Fill(bottomRow);
                });
            });
    }

    private Screen BuildFleetScreen(ScreenContext context)
    {
        _fleetTable.SetRows(BuildFleetRows());
        _selectionSummary.Text = BuildSelectionSummary(context);
        ConfigureStatus("fleet");

        var topRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _fleetTable, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _serviceList, Length = Math.Min(58, Math.Max(42, context.Width / 3)) },
            },
        };

        var lowerRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _pipelinePanel, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _selectionSummary, Length = Math.Min(52, Math.Max(38, context.Width / 4)) },
            },
        };

        return BuildWithChrome(
            context,
            body =>
            {
                body.Column(column =>
                {
                    column.Gap(1);
                    column.Fixed(14, topRow);
                    column.Fixed(10, lowerRow);
                    column.Fill(_activity);
                });
            });
    }

    private Screen BuildIncidentsScreen(ScreenContext context)
    {
        _selectionSummary.Text = BuildSelectionSummary(context);
        ConfigureStatus("incidents");

        var topRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _healthBoard, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _notifications, Length = Math.Min(50, Math.Max(36, context.Width / 3)) },
            },
        };

        var lowerRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _pipelinePanel, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _selectionSummary, Length = Math.Min(50, Math.Max(36, context.Width / 4)) },
            },
        };

        return BuildWithChrome(
            context,
            body =>
            {
                body.Column(column =>
                {
                    column.Gap(1);
                    column.Fixed(12, topRow);
                    column.Fixed(10, lowerRow);
                    column.Fill(_activity);
                });
            });
    }

    private Screen BuildWithChrome(ScreenContext context, Action<ContentBuilder> bodyBuilder)
    {
        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(1, _tabs);
            window.Left(Math.Min(34, Math.Max(28, context.Width / 5)), _rail);
            window.Body(bodyBuilder);
            window.Footer(1, _status);
            window.Overlay(new CenterLayout
            {
                Content = _deployDialog,
                Width = Math.Min(66, Math.Max(48, context.Width - 8)),
                Height = 10,
            });
            window.Overlay(new CenterLayout
            {
                Content = _quickOpenOverlay,
                Width = Math.Min(90, Math.Max(58, context.Width - 10)),
                Height = Math.Min(18, Math.Max(11, context.Height - 8)),
            });
        });
    }

    private void SimulateTelemetry(DateTimeOffset now)
    {
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            service.P95Ms = Math.Clamp(service.P95Ms + _random.Next(-6, 7), 18, 150);
            service.CpuPercent = Math.Clamp(service.CpuPercent + _random.Next(-5, 6), 8, 96);
            service.RequestsPerSecond = Math.Clamp(service.RequestsPerSecond + _random.Next(-40, 41), 20, 1000);
            service.ErrorBasisPoints = Math.Clamp(service.ErrorBasisPoints + _random.Next(-24, 25), 0, 500);
            service.State = service.P95Ms switch
            {
                > 88 => "Degraded",
                > 65 => "Warning",
                _ => "Healthy",
            };
        }

        for (var index = 0; index < _endpoints.Count; index++)
        {
            var endpoint = _endpoints[index];
            endpoint.P95Ms = Math.Clamp(endpoint.P95Ms + _random.Next(-8, 9), 16, 220);
            endpoint.ErrorBasisPoints = Math.Clamp(endpoint.ErrorBasisPoints + _random.Next(-30, 31), 0, 900);
            endpoint.RequestsPerSecond = Math.Clamp(endpoint.RequestsPerSecond + _random.Next(-50, 51), 10, 800);
        }

        UpdateOverviewState();
        UpdateAnalyticsState();
        UpdateAutomationState();
        _quickOpenOverlay.SetItems(BuildQuickOpenItems());

        if (_tick % 8 == 0)
        {
            var service = GetSelectedService();
            _notifications.Push($"Pulse {_tick:0000}: {service.Name} p95={service.P95Ms}ms", NotificationLevel.Info);
        }

        if (_tick % 13 == 0)
        {
            var service = _services[_random.Next(_services.Count)];
            _notifications.Push($"Incident watch: {service.Name}", NotificationLevel.Warning);
            AppendActivity($"[{now:HH:mm:ss}] watcher raised for {service.Name}.");
        }
    }

    private void UpdateOverviewState()
    {
        var totalRps = 0;
        var totalErrors = 0;
        var degraded = 0;
        var totalP95 = 0;
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            totalRps += service.RequestsPerSecond;
            totalErrors += service.ErrorBasisPoints;
            totalP95 += service.P95Ms;
            if (!string.Equals(service.State, "Healthy", StringComparison.Ordinal))
            {
                degraded++;
            }
        }

        var avgP95 = _services.Count == 0 ? 0 : totalP95 / _services.Count;
        _latencyBudget.SetValue(avgP95);

        _overviewGrid.SetTiles(
        [
            new DashboardTile("throughput", "Throughput", 0, 0, subtitle: $"{totalRps} req/s"),
            new DashboardTile("latency", "Latency", 1, 0, subtitle: $"{avgP95} ms"),
            new DashboardTile("degraded", "Degraded", 2, 0, subtitle: degraded.ToString(CultureInfo.InvariantCulture)),
            new DashboardTile("errors", "Error bps", 0, 1, subtitle: totalErrors.ToString(CultureInfo.InvariantCulture)),
            new DashboardTile("unread", "Unread", 1, 1, subtitle: _notifications.Items.Count(static item => !item.IsRead).ToString(CultureInfo.InvariantCulture)),
            new DashboardTile("service", "Selected", 2, 1, subtitle: GetSelectedService().Name),
        ]);

        var boardRows = new List<HealthService>();
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            var severity = service.State switch
            {
                "Degraded" => HealthServiceSeverity.Outage,
                "Warning" => HealthServiceSeverity.Degraded,
                _ => HealthServiceSeverity.Healthy,
            };

            var row = new HealthService(
                service.Id,
                service.Name,
                severity,
                $"{service.Region}  p95 {service.P95Ms}ms  err {service.ErrorBasisPoints / 100d:0.00}%");
            row.IsAcknowledged = _acknowledgedServices.Contains(service.Id);
            row.IsMuted = severity == HealthServiceSeverity.Healthy && service.P95Ms < 40;
            boardRows.Add(row);
        }

        _healthBoard.SetServices(boardRows);
    }

    private void SeedPipelineTasks()
    {
        _pipelinePanel.SetItems(
        [
            new TaskRunItem("build", "Build", TaskRunStatus.Succeeded, "compiled"),
            new TaskRunItem("tests", "Tests", TaskRunStatus.Running, "integration"),
            new TaskRunItem("scan", "Security Scan", TaskRunStatus.Queued, "pending"),
            new TaskRunItem("deploy", "Deploy", TaskRunStatus.Queued, "waiting for approval"),
        ]);
    }

    private List<IReadOnlyList<string>> BuildFleetRows()
    {
        var rows = new List<IReadOnlyList<string>>();
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            rows.Add(
            [
                service.Name,
                service.State,
                $"{service.P95Ms}ms",
                $"{service.CpuPercent}%",
                service.RequestsPerSecond.ToString(CultureInfo.InvariantCulture),
                $"{service.ErrorBasisPoints / 100d:0.00}%",
            ]);
        }

        return rows;
    }

    private string BuildSelectionSummary(ScreenContext context)
    {
        var selectedService = GetSelectedService();
        var selectedIncident = _healthBoard.SelectedItem?.Name ?? "(none)";
        return
            $"""
             Service: {selectedService.Name} ({selectedService.Region})
             Incident: {selectedIncident}
             Theme: {CurrentThemeName()}  Tick: {_tick:0000}
             Viewport: {context.Width}x{context.Height}
             """;
    }

    private void ConfigureStatus(string viewName)
    {
        _status.LeftText = $"{CurrentThemeName()}  view={viewName}  tick={_tick:0000}  service={GetSelectedService().Name}";
        _status.RightText = $"{_statusText}  1-5 views  t theme  Ctrl+P quick-open  d deploy  a ack  Ctrl+C quit";
    }

    private void OpenDeployDialog()
    {
        var selected = GetSelectedService();
        _deployDialog.Show(
            $"Queue deployment for {selected.Name}?",
            $"Region: {selected.Region}",
            $"Current state: {selected.State}",
            "Enter confirms, Esc cancels.");
    }

    private void HandleDeployDialogClosed(DialogResult result)
    {
        var selected = GetSelectedService();
        if (result == DialogResult.Accepted)
        {
            _notifications.Push($"Deployment queued for {selected.Name}", NotificationLevel.Warning);
            AppendActivity($"Deployment queued for {selected.Name}.");
            _statusText = $"deploy queued -> {selected.Name}";
            return;
        }

        if (result == DialogResult.Dismissed)
        {
            _notifications.Push($"Deployment canceled for {selected.Name}", NotificationLevel.Info);
            _statusText = "deploy canceled";
        }
    }

    private void AcknowledgeSelectedIncident()
    {
        var selected = _healthBoard.SelectedItem;
        if (selected is null)
        {
            _statusText = "no incident selected";
            return;
        }

        if (!_acknowledgedServices.Add(selected.Id))
        {
            _statusText = $"{selected.Name} already acknowledged";
            return;
        }

        _healthBoard.Acknowledge(selected.Id);
        _notifications.Push($"Acknowledged {selected.Name}", NotificationLevel.Warning);
        AppendActivity($"Acknowledged incident row -> {selected.Name}");
        _statusText = $"acknowledged -> {selected.Name}";
    }

    private void SelectHealthService(string serviceId)
    {
        for (var index = 0; index < _healthBoard.Services.Count; index++)
        {
            if (!string.Equals(_healthBoard.Services[index].Id, serviceId, StringComparison.Ordinal))
            {
                continue;
            }

            _healthBoard.SetSelectedIndex(index);
            return;
        }
    }

    private ServiceNode GetSelectedService()
    {
        for (var index = 0; index < _services.Count; index++)
        {
            if (string.Equals(_services[index].Id, _selectedServiceId, StringComparison.Ordinal))
            {
                return _services[index];
            }
        }

        return _services[0];
    }

    private void SyncRailWithTab()
    {
        var navIndex = _tabs.SelectedIndex switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            3 => 3,
            4 => 4,
            _ => 0,
        };
        _rail.SetSelectedIndex(navIndex);
    }

    private void AppendActivity(string line)
    {
        _activity.Append($"[{DateTimeOffset.UtcNow:HH:mm:ss}] {line}");
    }

    private string CurrentThemeName() => _useRosePine ? "Rosé Pine" : "Catppuccin";

    private void ApplyThemeAndOverrides()
    {
        var theme = _useRosePine ? RosePineTheme : DefaultTheme;
        var bundle = TeaThemeOverrideBundle.CreateDashboardBundle(theme, focusMarker: "◆");

        _tabs.ApplyTheme(theme);
        _tabs.FocusMarker = bundle.FocusMarker;

        _rail.ApplyTheme(theme);
        _rail.FocusMarker = bundle.FocusMarker;
        _rail.BorderStyleText = bundle.BorderStyleText;
        _rail.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _rail.SelectedItemStyle = theme.Accent.Primary.WithBold();
        _rail.FocusedSelectedItemStyle = theme.Selection.Foreground.WithBold();
        _rail.HoveredItemStyle = theme.State.Info;
        _rail.DisabledItemStyle = theme.Text.Muted.WithDim();

        _deployButton.ApplyThemeAndDashboardOverrides(bundle);
        _ackButton.ApplyThemeAndDashboardOverrides(bundle);
        _serviceList.ApplyThemeAndDashboardOverrides(bundle);
        _fleetTable.ApplyThemeAndDashboardOverrides(bundle);
        _notifications.ApplyThemeAndDashboardOverrides(bundle);
        _activity.ApplyThemeAndDashboardOverrides(bundle);
        _deployDialog.ApplyThemeAndDashboardOverrides(bundle);

        _notifications.HoveredItemStyle = theme.State.Info;
        _notifications.ErrorItemStyle = theme.State.Error.WithBold();
        _notifications.WarningItemStyle = theme.State.Warning.WithBold();
        _notifications.DisabledItemStyle = theme.Text.Muted.WithDim();

        _overviewGrid.ApplyTheme(theme);
        _overviewGrid.FocusMarker = bundle.FocusMarker;
        _overviewGrid.BorderStyleText = bundle.BorderStyleText;
        _overviewGrid.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _overviewGrid.SelectedTileStyleText = theme.Selection.Background.WithBold();

        _latencyBudget.ApplyTheme(theme);
        _latencyBudget.FocusMarker = bundle.FocusMarker;
        _latencyBudget.BorderStyleText = bundle.BorderStyleText;
        _latencyBudget.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _latencyBudget.WarningRangeStyle = theme.State.Warning;
        _latencyBudget.CriticalRangeStyle = theme.State.Error;
        _latencyBudget.ValueBarStyle = theme.Accent.Primary.WithBold();
        _latencyBudget.TargetMarkerStyle = theme.Focus.Ring.WithBold();

        _healthBoard.ApplyTheme(theme);
        _healthBoard.FocusMarker = bundle.FocusMarker;
        _healthBoard.BorderStyleText = bundle.BorderStyleText;
        _healthBoard.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _healthBoard.DegradedServiceStyle = theme.State.Warning.WithBold();
        _healthBoard.OutageServiceStyle = theme.State.Error.WithBold();
        _healthBoard.SelectedServiceStyle = theme.Selection.Background.WithBold();

        _pipelinePanel.ApplyTheme(theme);
        _pipelinePanel.FocusMarker = bundle.FocusMarker;
        _pipelinePanel.BorderStyleText = bundle.BorderStyleText;
        _pipelinePanel.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _pipelinePanel.HoveredRowStyle = theme.State.Info;
        _pipelinePanel.SelectedRowStyle = theme.Selection.Background.WithBold();
        _pipelinePanel.FocusedSelectedRowStyle = theme.Selection.Foreground.WithBold();
        _pipelinePanel.SucceededStatusStyle = theme.State.Success.WithBold();
        _pipelinePanel.RunningStatusStyle = theme.State.Info.WithBold();
        _pipelinePanel.FailedStatusStyle = theme.State.Error.WithBold();
        _pipelinePanel.DisabledRowStyle = theme.Text.Muted.WithDim();

        _selectionSummary.ApplyTheme(theme);
        _selectionSummary.BorderStyleText = bundle.BorderStyleText;
        _selectionSummary.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _selectionSummary.TextStyle = theme.Text.Primary;

        _status.ApplyTheme(theme);

        ApplyAnalyticsTheme(theme, bundle);
        ApplyAutomationTheme(theme, bundle);
    }
}
