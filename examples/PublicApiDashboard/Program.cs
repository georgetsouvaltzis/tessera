using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<PublicApiDashboardApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = PublicApiDashboardApp.DefaultTheme;
        runtime.PointerActivationPolicy = PointerActivationPolicy.SingleClick;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Public API Dashboard",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.CellMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record DashboardTick(DateTimeOffset At) : Message;

internal sealed class PublicApiDashboardApp : TeaApp
{
    internal static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly DashboardNavigationTabs _navigation = new("Overview", "Operations", "Audit")
    {
        Title = "Public API Dashboard",
        FocusMarker = "◆",
    };

    private readonly Button _deploy = new()
    {
        Text = "Deploy",
        Description = "d opens confirmation",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly ListView<ServiceSnapshot> _serviceList = new(static service => $"{service.Name,-12} [{service.Status}]")
    {
        Title = "Services",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Table _metricsTable = new("Service", "Status", "P95", "Req/s")
    {
        Title = "Live Metrics",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        PageSize = 8,
        FocusMarker = "◆",
    };

    private readonly LogView _activityLog = new()
    {
        Title = "Activity Log",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Notifications _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MaxItems = 64,
        FocusMarker = "◆",
    };

    private readonly Label _selectionSummary = new()
    {
        Title = "Selection",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Dialog _confirmDeploy = new()
    {
        Title = "Confirm Deployment",
        BodyLines =
        [
            "Deploy selected service?",
            "Enter accepts, Esc cancels.",
        ],
    };

    private readonly StatusBar _status = new();
    private readonly List<ServiceSnapshot> _services =
    [
        new ServiceSnapshot("api", "API", "Healthy", 24, 520),
        new ServiceSnapshot("worker", "Worker", "Healthy", 31, 340),
        new ServiceSnapshot("scheduler", "Scheduler", "Warning", 66, 140),
        new ServiceSnapshot("gateway", "Gateway", "Healthy", 28, 790),
        new ServiceSnapshot("billing", "Billing", "Degraded", 89, 74),
        new ServiceSnapshot("search", "Search", "Healthy", 36, 610),
    ];

    private readonly Random _random = new(73);
    private bool _useRosePine;
    private int _tick;
    private string _statusText = "Ready";
    private string _selectedServiceId = "api";

    public PublicApiDashboardApp()
    {
        _navigation.SelectionChanged += (_, args) =>
        {
            _statusText = $"view: {args.SelectedItem}";
            AppendAudit($"Navigation -> {args.SelectedItem}");
        };

        _serviceList.SetItems(_services);
        _serviceList.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null)
            {
                _selectedServiceId = args.SelectedItem.Id;
                _statusText = $"selected: {args.SelectedItem.Name}";
                AppendAudit($"Selected service {args.SelectedItem.Name}");
            }
        };

        _deploy.Activated += (_, _) => OpenDeployDialog();
        _confirmDeploy.Accepted += (_, _) => ConfirmDeployment(accepted: true);
        _confirmDeploy.Dismissed += (_, _) => ConfirmDeployment(accepted: false);

        _notifications.Push("Dashboard initialized", NotificationLevel.Info);
        _notifications.Push("Press Ctrl+D (or d) to deploy selected service", NotificationLevel.Success);
        AppendAudit("Dashboard boot complete.");
        ApplyThemeAndOverrides();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(350), static now => new DashboardTick(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key)
        {
            if (IsQuitShortcut(key))
            {
                return TeaEffects.Quit;
            }

            if (IsDeployShortcut(key))
            {
                OpenDeployDialog();
                return null;
            }

            if (IsThemeShortcut(key))
            {
                _useRosePine = !_useRosePine;
                ApplyThemeAndOverrides();
                _statusText = $"theme: {CurrentThemeName()}";
                _notifications.Push($"Theme switched to {CurrentThemeName()}", NotificationLevel.Info);
                return null;
            }
        }

        if (message is DashboardTick tick)
        {
            _tick++;
            SimulateTelemetry(tick.At);
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _metricsTable.SetRows(BuildRowsForCurrentTab());
        _selectionSummary.Text = BuildSummaryText(context);
        _status.LeftText = $"{CurrentThemeName()}  tick={_tick:0000}  tab={_navigation.Items[_navigation.SelectedIndex]}";
        _status.RightText = $"{_statusText}  single-click pointer mode  click tab to switch  wheel ignored  Ctrl+D deploy  Ctrl+T theme  Ctrl+C quit";

        var topRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _deploy,
                    Length = 26,
                },
                new LayoutSlot
                {
                    Content = _selectionSummary,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var lowerRow = new RowLayout
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
                    Length = Math.Min(46, Math.Max(34, context.Width / 4)),
                },
            },
        };

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(1, _navigation);
            window.Left(Math.Min(36, Math.Max(28, context.Width / 4)), _serviceList);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(5, topRow);
                column.Fixed(12, _metricsTable);
                column.Fill(lowerRow);
            }));
            window.Footer(1, _status);
            window.Overlay(new CenterLayout
            {
                Content = _confirmDeploy,
                Width = Math.Min(60, Math.Max(44, context.Width - 8)),
                Height = 9,
            });
        });
    }

    private void OpenDeployDialog()
    {
        var selected = GetSelectedService();
        _confirmDeploy.Show(
            $"Deploy {selected.Name}?",
            $"Status: {selected.Status}",
            "Press Enter to continue.",
            "Press Esc to cancel.");
    }

    private void ConfirmDeployment(bool accepted)
    {
        var selected = GetSelectedService();
        if (!accepted)
        {
            _statusText = $"deploy canceled: {selected.Name}";
            AppendAudit($"Deployment canceled for {selected.Name}.");
            return;
        }

        _statusText = $"deploy started: {selected.Name}";
        AppendAudit($"Deployment started for {selected.Name}.");
        _notifications.Push($"Deploying {selected.Name}", NotificationLevel.Warning);
    }

    private void SimulateTelemetry(DateTimeOffset now)
    {
        for (var index = 0; index < _services.Count; index++)
        {
            var entry = _services[index];
            var p95Delta = _random.Next(-4, 5);
            var reqDelta = _random.Next(-25, 26);
            entry.P95Ms = Math.Clamp(entry.P95Ms + p95Delta, 18, 140);
            entry.RequestsPerSecond = Math.Clamp(entry.RequestsPerSecond + reqDelta, 40, 900);
            entry.Status = entry.P95Ms switch
            {
                > 90 => "Degraded",
                > 60 => "Warning",
                _ => "Healthy",
            };
        }

        if (_tick % 8 == 0)
        {
            var selected = GetSelectedService();
            AppendAudit($"{now:HH:mm:ss} pulse p95={selected.P95Ms}ms req/s={selected.RequestsPerSecond}");
        }
    }

    private List<IReadOnlyList<string>> BuildRowsForCurrentTab()
    {
        var rows = new List<IReadOnlyList<string>>();
        var takeCount = _navigation.SelectedIndex == 0 ? Math.Min(4, _services.Count) : _services.Count;
        for (var index = 0; index < takeCount; index++)
        {
            var service = _services[index];
            rows.Add(
            [
                service.Name,
                service.Status,
                $"{service.P95Ms}ms",
                service.RequestsPerSecond.ToString(CultureInfo.InvariantCulture)
            ]);
        }

        return rows;
    }

    private string BuildSummaryText(ScreenContext context)
    {
        var selected = GetSelectedService();
        return
            $"""
             Selected: {selected.Name}
             Status: {selected.Status}
             P95: {selected.P95Ms}ms  Req/s: {selected.RequestsPerSecond}
             Viewport: {context.Width}x{context.Height}
             """;
    }

    private ServiceSnapshot GetSelectedService()
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

    private void AppendAudit(string line)
    {
        _activityLog.Append(line);
    }

    private static bool IsQuitShortcut(KeyPressed key)
    {
        return key.IsCharacter('c', ModifierKeys.Ctrl) || IsControlCharacter(key, '\u0003');
    }

    private static bool IsDeployShortcut(KeyPressed key)
    {
        return key.IsCharacter('d')
            || key.IsCharacter('d', ModifierKeys.Ctrl)
            || IsControlCharacter(key, '\u0004');
    }

    private static bool IsThemeShortcut(KeyPressed key)
    {
        return key.IsCharacter('t')
            || key.IsCharacter('t', ModifierKeys.Ctrl)
            || IsControlCharacter(key, '\u0014');
    }

    private static bool IsControlCharacter(KeyPressed key, char controlChar)
    {
        return key.Key == Key.Character
            && key.Text.Length == 1
            && key.Text[0] == controlChar;
    }

    private string CurrentThemeName() => _useRosePine ? "Rose Pine" : "Catppuccin";

    private void ApplyThemeAndOverrides()
    {
        var theme = _useRosePine
            ? TeaThemes.RosePine(RosePineVariant.Moon)
            : TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);
        var bundle = TeaThemeOverrideBundle.CreateDashboardBundle(theme, focusMarker: "◆");

        _navigation.ApplyTheme(theme);
        _deploy.ApplyThemeAndDashboardOverrides(bundle);
        _serviceList.ApplyThemeAndDashboardOverrides(bundle);
        _metricsTable.ApplyThemeAndDashboardOverrides(bundle);
        _activityLog.ApplyThemeAndDashboardOverrides(bundle);
        _notifications.ApplyThemeAndDashboardOverrides(bundle);
        _selectionSummary.ApplyTheme(theme);
        _status.ApplyTheme(theme);
        _confirmDeploy.ApplyThemeAndDashboardOverrides(bundle);

        _navigation.TitleStyle = theme.Accent.Primary.WithBold();
        _navigation.FocusedTitleStyle = theme.Focus.Title.Merge(theme.Accent.Primary).WithBold();
        _selectionSummary.BorderStyleText = theme.Border.Default;
        _selectionSummary.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _selectionSummary.TextStyle = theme.Text.Primary;

        _status.Fill = '·';
        _status.FillStyle = theme.Surface.Panel;
        _status.LeftTextStyle = theme.Text.Secondary.WithBold();
        _status.RightTextStyle = theme.Accent.Secondary;
    }
}

internal sealed class ServiceSnapshot
{
    public ServiceSnapshot(string id, string name, string status, int p95Ms, int requestsPerSecond)
    {
        Id = id;
        Name = name;
        Status = status;
        P95Ms = p95Ms;
        RequestsPerSecond = requestsPerSecond;
    }

    public string Id { get; }

    public string Name { get; }

    public string Status { get; set; }

    public int P95Ms { get; set; }

    public int RequestsPerSecond { get; set; }
}
