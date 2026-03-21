using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<ExternalConsumerReviewApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = ExternalConsumerReviewApp.DefaultTheme;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp External Consumer Review",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record DashboardPulse(DateTimeOffset At) : Message;

internal sealed partial class ExternalConsumerReviewApp : TeaApp
{
    internal static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private static readonly TeaTheme RosePineTheme = TeaThemes.RosePine(RosePineVariant.Moon);

    private readonly Tabs _navigation = new("Overview", "Operations", "Alerts", "Analytics")
    {
        Title = "External Consumer Review",
        FocusMarker = "◆",
    };

    private readonly Button _deploy = new()
    {
        Text = "Queue Deployment",
        Description = "d opens confirmation",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly ListView<ServiceHealth> _serviceList = new(static service => $"{service.Name,-14} [{service.Environment}]")
    {
        Title = "Services",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Table _serviceTable = new("Service", "State", "P95", "CPU", "Req/s")
    {
        Title = "Runtime Metrics",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        PageSize = 8,
        FocusMarker = "◆",
    };

    private readonly Notifications _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MaxItems = 72,
        FocusMarker = "◆",
    };

    private readonly LogView _activity = new()
    {
        Title = "Activity",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Label _selectionSummary = new()
    {
        Title = "Selection",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Dialog _deployDialog = new()
    {
        Title = "Deploy Service",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();
    private readonly Random _random = new(211);
    private readonly List<ServiceHealth> _services =
    [
        new ServiceHealth("edge", "Edge API", "us-east-1", "Healthy", 26, 19, 840),
        new ServiceHealth("checkout", "Checkout", "us-east-1", "Healthy", 32, 24, 510),
        new ServiceHealth("billing", "Billing", "eu-west-1", "Warning", 61, 41, 232),
        new ServiceHealth("scheduler", "Scheduler", "eu-west-1", "Healthy", 39, 18, 170),
        new ServiceHealth("search", "Search", "us-east-1", "Healthy", 43, 36, 624),
        new ServiceHealth("mailer", "Mailer", "ap-southeast-1", "Degraded", 88, 63, 108),
    ];

    private bool _useRosePine;
    private int _tick;
    private string _statusText = "Ready";
    private string _selectedServiceId = "edge";

    public ExternalConsumerReviewApp()
    {
        _serviceList.SetItems(_services);
        _serviceList.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _selectedServiceId = args.SelectedItem.Id;
            _statusText = $"selected {args.SelectedItem.Name}";
            AppendActivity($"Selected service -> {args.SelectedItem.Name}");
        };

        _navigation.SelectionChanged += (_, args) =>
        {
            _statusText = $"view {args.SelectedItem}";
            AppendActivity($"View switched -> {args.SelectedItem}");
        };

        _deploy.Activated += (_, _) => OpenDeployDialog();
        _deployDialog.Closed += (_, args) => HandleDeployDialogClosed(args.Result);

        _notifications.Push("Dashboard boot complete", NotificationLevel.Success);
        _notifications.Push("Press d to queue deployment", NotificationLevel.Info);
        _notifications.Push("Press t to switch theme", NotificationLevel.Info);
        _notifications.Push("Press 4 for analytics screen", NotificationLevel.Info);
        AppendActivity("External consumer dashboard initialized.");

        InitializeWave2Dashboard();
        ApplyThemeAndOverrides();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(450), static now => new DashboardPulse(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key)
        {
            if (key.IsCharacter('c', ModifierKeys.Ctrl))
            {
                return TeaEffects.Quit;
            }

            if (key.IsCharacter('d'))
            {
                OpenDeployDialog();
                return null;
            }

            if (key.IsCharacter('t'))
            {
                _useRosePine = !_useRosePine;
                ApplyThemeAndOverrides();
                _statusText = $"theme {CurrentThemeName()}";
                _notifications.Push($"Theme switched -> {CurrentThemeName()}", NotificationLevel.Info);
                return null;
            }

            if (key.IsCharacter('n'))
            {
                var selected = GetSelectedService();
                _notifications.Push($"Manual note for {selected.Name}", NotificationLevel.Info);
                _statusText = "notification appended";
                return null;
            }
        }

        if (message is DashboardPulse pulse)
        {
            _tick++;
            SimulateMetrics(pulse.At);
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        if (_navigation.SelectedIndex == 3)
        {
            return BuildAnalyticsScreen(context);
        }

        _serviceTable.SetRows(BuildRowsForCurrentView());
        _selectionSummary.Text = BuildSelectionSummary(context);

        _status.LeftText =
            $"{CurrentThemeName()}  tick={_tick:0000}  selected={GetSelectedService().Name}";
        _status.RightText =
            $"{_statusText}  1-4 tabs  t theme  d dialog  n note  Ctrl+C quit";

        var actionRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _deploy,
                    Length = 28,
                },
                new LayoutSlot
                {
                    Content = _selectionSummary,
                    Length = LayoutLength.Fill(),
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
            window.Left(Math.Min(40, Math.Max(30, context.Width / 4)), _serviceList);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(5, actionRow);
                column.Fixed(12, _serviceTable);
                column.Fill(bottomRow);
            }));
            window.Footer(1, _status);
            window.Overlay(new CenterLayout
            {
                Content = _deployDialog,
                Width = Math.Min(66, Math.Max(48, context.Width - 8)),
                Height = 10,
            });
        });
    }

    private void OpenDeployDialog()
    {
        var selected = GetSelectedService();
        _deployDialog.Show(
            $"Queue deployment for {selected.Name}?",
            $"Region: {selected.Environment}",
            $"Current state: {selected.State}",
            "Enter confirms, Esc cancels.");
    }

    private void HandleDeployDialogClosed(DialogResult result)
    {
        var selected = GetSelectedService();
        if (result == DialogResult.Accepted)
        {
            _statusText = $"deploy queued for {selected.Name}";
            _notifications.Push($"Queued deployment for {selected.Name}", NotificationLevel.Warning);
            AppendActivity($"Deployment queued for {selected.Name}.");
            return;
        }

        if (result == DialogResult.Dismissed)
        {
            _statusText = "deploy canceled";
            _notifications.Push($"Deployment canceled for {selected.Name}", NotificationLevel.Info);
            AppendActivity($"Deployment canceled for {selected.Name}.");
        }
    }

    private void SimulateMetrics(DateTimeOffset now)
    {
        for (var index = 0; index < _services.Count; index++)
        {
            var entry = _services[index];
            entry.P95Ms = Math.Clamp(entry.P95Ms + _random.Next(-5, 6), 18, 150);
            entry.CpuPercent = Math.Clamp(entry.CpuPercent + _random.Next(-6, 7), 8, 96);
            entry.RequestsPerSecond = Math.Clamp(entry.RequestsPerSecond + _random.Next(-45, 46), 20, 900);

            entry.State = entry.P95Ms switch
            {
                > 90 => "Degraded",
                > 65 => "Warning",
                _ => "Healthy",
            };
        }

        if (_tick % 9 == 0)
        {
            var selected = GetSelectedService();
            AppendActivity(
                $"{now:HH:mm:ss} {selected.Name} p95={selected.P95Ms}ms cpu={selected.CpuPercent}%");
        }

        if (_tick % 16 == 0)
        {
            var degraded = _services.FirstOrDefault(static item => item.State == "Degraded");
            if (degraded is not null)
            {
                _notifications.Push(
                    $"SLO alert: {degraded.Name} p95 {degraded.P95Ms}ms",
                    NotificationLevel.Error);
            }
        }

        UpdateWave2State();
    }

    private List<IReadOnlyList<string>> BuildRowsForCurrentView()
    {
        IEnumerable<ServiceHealth> source = _services;
        if (_navigation.SelectedIndex == 0)
        {
            source = _services.Take(4);
        }
        else if (_navigation.SelectedIndex == 2)
        {
            source = _services.Where(static service => service.State != "Healthy");
            if (!source.Any())
            {
                source = _services.Take(2);
            }
        }

        var rows = new List<IReadOnlyList<string>>();
        foreach (var item in source)
        {
            rows.Add(
            [
                item.Name,
                item.State,
                $"{item.P95Ms}ms",
                item.CpuPercent.ToString(CultureInfo.InvariantCulture) + "%",
                item.RequestsPerSecond.ToString(CultureInfo.InvariantCulture),
            ]);
        }

        return rows;
    }

    private string BuildSelectionSummary(ScreenContext context)
    {
        var selected = GetSelectedService();
        return
            $"""
             Service: {selected.Name}
             State: {selected.State}  P95: {selected.P95Ms}ms  CPU: {selected.CpuPercent}%
             Region: {selected.Environment}
             Viewport: {context.Width}x{context.Height}
             """;
    }

    private ServiceHealth GetSelectedService()
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

    private void AppendActivity(string line)
    {
        _activity.Append($"{DateTimeOffset.Now:HH:mm:ss}  {line}");
    }

    private string CurrentThemeName() => _useRosePine ? "Rose Pine" : "Catppuccin";

    private TeaTheme CurrentTheme() => _useRosePine ? RosePineTheme : DefaultTheme;

    private void ApplyThemeAndOverrides()
    {
        var theme = CurrentTheme();
        var bundle = TeaThemeOverrideBundle.CreateDashboardBundle(theme, focusMarker: "◆");

        _navigation.ApplyTheme(theme);
        _navigation.TitleStyle = theme.Accent.Primary.WithBold();
        _navigation.FocusedTitleStyle = theme.Focus.Title.Merge(theme.Accent.Primary).WithBold();

        _deploy.ApplyThemeAndDashboardOverrides(bundle);
        _serviceList.ApplyThemeAndDashboardOverrides(bundle);
        _serviceTable.ApplyThemeAndDashboardOverrides(bundle);
        _notifications.ApplyThemeAndDashboardOverrides(bundle);
        _activity.ApplyThemeAndDashboardOverrides(bundle);
        _deployDialog.ApplyThemeAndDashboardOverrides(bundle);
        _selectionSummary.ApplyTheme(theme);
        _status.ApplyTheme(theme);

        _selectionSummary.TextStyle = theme.Text.Primary;
        _selectionSummary.BorderStyleText = theme.Border.Default;
        _selectionSummary.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        ApplyWave2ThemeAndOverrides(theme, bundle);

        _status.Fill = '·';
        _status.FillStyle = theme.Surface.Panel;
        _status.LeftTextStyle = theme.Text.Secondary.WithBold();
        _status.RightTextStyle = theme.Accent.Secondary;
    }
}

internal sealed class ServiceHealth
{
    public ServiceHealth(
        string id,
        string name,
        string environment,
        string state,
        int p95Ms,
        int cpuPercent,
        int requestsPerSecond)
    {
        Id = id;
        Name = name;
        Environment = environment;
        State = state;
        P95Ms = p95Ms;
        CpuPercent = cpuPercent;
        RequestsPerSecond = requestsPerSecond;
    }

    public string Id { get; }

    public string Name { get; }

    public string Environment { get; }

    public string State { get; set; }

    public int P95Ms { get; set; }

    public int CpuPercent { get; set; }

    public int RequestsPerSecond { get; set; }
}
