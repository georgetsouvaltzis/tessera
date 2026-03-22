using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed partial class ExternalConsumerReviewApp
{
    private const int LatencyTargetMs = 60;

    private readonly SideNavRail _dashboardRail = new()
    {
        Title = "Dashboard API Surfaces",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly DashboardGrid _dashboardGrid = new()
    {
        Title = "Live Tiles",
        Border = BorderStyle.Rounded,
        TileBorder = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly BulletChart _latencyBudgetChart = new()
    {
        Title = "Latency Budget",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly HealthBoard _healthBoard = new()
    {
        Title = "Service Health Board",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly BoxPlot _distributionPlot = new()
    {
        Title = "Endpoint Distribution",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly JumpList _jumpList = new()
    {
        Title = "Jump List",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly TokenEditor _tokenEditor = new()
    {
        Title = "Deployment Tags",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Placeholder = "Add ownership tag...",
        FocusMarker = "◆",
    };

    private readonly AutocompleteInput _commandInput = new()
    {
        Title = "Command Autocomplete",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Placeholder = "Type command...",
        MaxVisibleSuggestions = 6,
        FocusMarker = "◆",
    };

    private readonly ResizablePaneGroup _workflowPanes = new()
    {
        Title = "Workflow Pane Group",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        DividerThickness = 1,
    };

    private readonly QuickOpenOverlay _quickOpenOverlay = new()
    {
        Title = "Quick Open",
        BorderStyle = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Placeholder = "Search service, endpoint, or action...",
        MaxVisibleItems = 9,
        FocusMarker = "◆",
    };

    private readonly Label _dashboardApiSummary = new()
    {
        Title = "Selection Snapshot",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly HashSet<string> _acknowledgedHealthServices = new(StringComparer.Ordinal);

    private void InitializeDashboardApiShowcase()
    {
        _dashboardRail.SetItems(
        [
            new NavItem("overview", "Overview", icon: "OVR"),
            new NavItem("services", "Services", icon: "SRV"),
            new NavItem("workflow", "Workflow", icon: "WRK"),
            new NavItem("quick-open", "Quick Open", icon: "QOP", badge: "Ctrl+P"),
        ]);
        _dashboardRail.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _statusText = $"dashboard rail -> {args.SelectedItem.Label}";
        };
        _dashboardRail.Activated += (_, args) =>
        {
            AppendActivity($"Rail activated -> {args.SelectedItem.Id}");
            if (string.Equals(args.SelectedItem.Id, "quick-open", StringComparison.Ordinal))
            {
                _quickOpenOverlay.Open();
            }
        };

        _dashboardGrid.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _statusText = $"tile {args.SelectedItem.Title}";
        };

        _healthBoard.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _statusText = $"health row {args.SelectedItem.Name}";
        };

        _jumpList.SetItems(
        [
            new JumpListItem("jump:deploy", "Queue deployment", isPinned: true),
            new JumpListItem("jump:rollback", "Run rollback"),
            new JumpListItem("jump:promote", "Promote canary", isRecent: true),
            new JumpListItem("jump:report", "Generate status report", isRecent: true),
        ]);
        _jumpList.Activated += (_, args) =>
        {
            _statusText = $"jump action {args.SelectedItem.Label}";
            _notifications.Push($"Jump action -> {args.SelectedItem.Label}", NotificationLevel.Info);
            AppendActivity($"JumpList activated -> {args.SelectedItem.Id}");
        };

        _tokenEditor.SetTokens(
        [
            new TokenItem("owner:platform"),
            new TokenItem("tier:critical"),
            new TokenItem("region:global"),
        ]);
        _tokenEditor.SelectionChanged += (_, args) =>
        {
            _statusText = args.SelectedToken is null
                ? "token selection cleared"
                : $"token selected {args.SelectedToken.Value}";
        };

        _commandInput.SetSuggestions(
        [
            "deploy checkout --region us-east-1",
            "rollback checkout --to previous",
            "scale search --replicas 8",
            "drain billing --reason maintenance",
            "acknowledge mailer --incident INC-9021",
            "notify oncall --service edge",
            "tail logs --service checkout --errors",
        ]);
        _commandInput.SuggestionCommitted += (_, args) =>
        {
            _statusText = $"command committed ({args.SuggestionIndex})";
            _notifications.Push($"Command committed: {args.Text}", NotificationLevel.Info);
        };

        _quickOpenOverlay.Submitted += (_, args) => HandleQuickOpenSubmitted(args);
        _quickOpenOverlay.Cancelled += (_, _) => _statusText = "quick-open canceled";

        _workflowPanes.SetPanes(
        [
            new PaneSpec("jump-pane", _jumpList, minSize: 16),
            new PaneSpec("token-pane", _tokenEditor, minSize: 24),
            new PaneSpec("command-pane", _commandInput, minSize: 22),
        ]);
        _workflowPanes.SetSplitRatio(0, 0.33d);
        _workflowPanes.SetSplitRatio(1, 0.72d);

        _latencyBudgetChart.SetRanges(
        [
            new BulletRange(0, 45, BulletRangeKind.Normal, "Healthy"),
            new BulletRange(45, 60, BulletRangeKind.Warning, "Risk"),
            new BulletRange(60, 120, BulletRangeKind.Critical, "Breach"),
        ]);
        _latencyBudgetChart.SetTarget(LatencyTargetMs);

        UpdateDashboardApiState();
    }

    private Screen BuildDashboardApiScreen(ScreenContext context)
    {
        _dashboardApiSummary.Text = BuildDashboardApiSummary(context);
        _status.LeftText =
            $"{CurrentThemeName()}  dashboard-api  tick={_tick:0000}  selected={GetSelectedService().Name}";
        _status.RightText =
            $"{_statusText}  1-5 tabs  t theme  d dialog  Ctrl+P quick-open  a ack  Ctrl+C quit";

        var topRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _dashboardGrid,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _latencyBudgetChart,
                    Length = Math.Min(52, Math.Max(40, context.Width / 3)),
                },
            },
        };

        var middleRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _healthBoard,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _distributionPlot,
                    Length = Math.Min(52, Math.Max(40, context.Width / 3)),
                },
            },
        };

        var paneRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _workflowPanes,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _dashboardApiSummary,
                    Length = Math.Min(46, Math.Max(36, context.Width / 4)),
                },
            },
        };

        var bottomRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _activity,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _notifications,
                    Length = Math.Min(48, Math.Max(34, context.Width / 4)),
                },
            },
        };

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(1, _navigation);
            window.Left(Math.Min(34, Math.Max(28, context.Width / 5)), _dashboardRail);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(10, topRow);
                column.Fixed(10, middleRow);
                column.Fixed(10, paneRow);
                column.Fill(bottomRow);
            }));
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
                Width = Math.Min(86, Math.Max(56, context.Width - 10)),
                Height = Math.Min(18, Math.Max(10, context.Height - 8)),
            });
        });
    }

    private void UpdateDashboardApiState()
    {
        var totalRps = 0;
        var totalP95 = 0;
        var degradedCount = 0;
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            totalRps += service.RequestsPerSecond;
            totalP95 += service.P95Ms;
            if (!string.Equals(service.State, "Healthy", StringComparison.Ordinal))
            {
                degradedCount++;
            }
        }

        var avgP95 = _services.Count == 0 ? 0 : totalP95 / _services.Count;
        _latencyBudgetChart.SetValue(avgP95);
        _latencyBudgetChart.SetTarget(LatencyTargetMs);

        _dashboardGrid.SetTiles(
        [
            new DashboardTile("tile-rps", "Throughput", 0, 0, subtitle: $"{totalRps.ToString(CultureInfo.InvariantCulture)} req/s"),
            new DashboardTile("tile-p95", "Latency", 1, 0, subtitle: $"{avgP95.ToString(CultureInfo.InvariantCulture)} ms"),
            new DashboardTile("tile-degraded", "Degraded", 2, 0, subtitle: degradedCount.ToString(CultureInfo.InvariantCulture)),
            new DashboardTile(
                "tile-alerts",
                "Active Alerts",
                0,
                1,
                subtitle: _endpoints.Count(static endpoint => endpoint.ErrorBasisPoints >= 120).ToString(CultureInfo.InvariantCulture)),
            new DashboardTile(
                "tile-unread",
                "Unread Notes",
                1,
                1,
                subtitle: _notifications.Items.Count(static item => !item.IsRead).ToString(CultureInfo.InvariantCulture)),
            new DashboardTile(
                "tile-selected",
                "Selected Service",
                2,
                1,
                subtitle: GetSelectedService().Name),
        ]);

        var rows = new List<HealthService>();
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            var severity = service.State switch
            {
                "Degraded" => HealthServiceSeverity.Outage,
                "Warning" => HealthServiceSeverity.Degraded,
                _ => service.P95Ms > 85 ? HealthServiceSeverity.Degraded : HealthServiceSeverity.Healthy,
            };

            var row = new HealthService(
                service.Id,
                service.Name,
                severity,
                $"{service.Environment}  p95 {service.P95Ms}ms  cpu {service.CpuPercent}%");
            row.IsAcknowledged = _acknowledgedHealthServices.Contains(service.Id);
            row.IsMuted = severity == HealthServiceSeverity.Healthy && service.P95Ms < 40;
            rows.Add(row);
        }

        _healthBoard.SetServices(rows);

        _distributionPlot.SetSeries(
        [
            BuildFiveNumberSeries("P95 ms", _endpoints.Select(static endpoint => (double)endpoint.P95Ms)),
            BuildFiveNumberSeries("Err bps", _endpoints.Select(static endpoint => (double)endpoint.ErrorBasisPoints)),
            BuildFiveNumberSeries("Req/s", _endpoints.Select(static endpoint => (double)endpoint.RequestsPerSecond)),
        ]);

        _quickOpenOverlay.SetItems(BuildQuickOpenItems());
    }

}
