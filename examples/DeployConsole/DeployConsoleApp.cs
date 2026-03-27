using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class DeployConsoleApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private static readonly DeployPhase[] Phases =
    [
        new("Queueing", [".  ", ".. ", "..."], 7, AnsiColor.Rgb(249, 226, 175)),
        new("Rolling", [">  ", ">> ", ">>>"], 9, AnsiColor.Rgb(137, 180, 250)),
        new("Verifying", ["|", "/", "-", "\\"], 8, AnsiColor.Rgb(166, 227, 161)),
    ];

    private readonly SearchBox _search = new()
    {
        Title = "Filter",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Placeholder = "type service, region, or status",
        FocusMarker = "◆",
    };

    private readonly Choice _environment = new()
    {
        Title = "Environment",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        MaxVisibleItems = 3,
    };

    private readonly Spinner _deploy = new()
    {
        Title = "Deploy",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly ListView<ServiceItem> _services = new(static service => service.RenderLabel())
    {
        Title = "Services",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        PageSize = 10,
    };

    private readonly NotificationInbox _events = new()
    {
        Title = "Events",
        Padding = Thickness.All(1),
        PageSize = 12,
        FocusMarker = "◆",
        ShowTimestamp = true,
        ShowSource = true,
    };

    private readonly StatusBar _status = new();
    private readonly List<ServiceItem> _allServices = CreateServices();
    private readonly Control[] _focusOrder;

    private int _phaseIndex;
    private int _phaseTicks;
    private ServiceItem? _deployTarget;
    private string _deployEnvironment = string.Empty;

    public DeployConsoleApp()
    {
        _focusOrder = [_search, _environment, _services, _events];

        ThemeScope.Apply(DefaultTheme, _search, _environment, _deploy, _services, _events, _status);
        ConfigureTheme();

        _environment.SetItems(["staging", "prod"]);
        _environment.SetSelectedIndex(0);

        SeedEvents();
        ApplyServiceFilter();
        SetIdleSpinner();
        _services.RequestFocus();

        _search.QueryChanged += (_, _) => ApplyServiceFilter();
        _search.NavigationRequested += (_, args) => NavigateMatches(args.Direction == SearchNavigationDirection.Next ? 1 : -1);
        _services.SelectionChanged += (_, _) => UpdateSearchMatchState();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(100), static now => new DeployPulse(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key)
        {
            if (key.IsCharacter('c', ModifierKeys.Ctrl))
            {
                return TeaEffects.Quit;
            }

            if (key.Is(Key.Tab))
            {
                CycleFocus(key.Modifiers.HasFlag(ModifierKeys.Shift) ? -1 : 1);
                return null;
            }

            if (key.IsCharacter('d') && !HasActiveDeployment())
            {
                StartDeployment();
                return null;
            }

            if (key.IsCharacter('p') && _deployTarget is not null)
            {
                _deploy.SetRunning(!_deploy.Running);
                PushEvent(
                    _deploy.Running ? "Deployment resumed." : "Deployment paused.",
                    NotificationLevel.Info,
                    "Deploy");
                return null;
            }
        }

        if (message is not DeployPulse || !HasActiveDeployment() || !_deploy.Running)
        {
            return null;
        }

        _deploy.Advance();
        _phaseTicks++;
        if (_phaseTicks < Phases[_phaseIndex].Ticks)
        {
            return null;
        }

        if (_phaseIndex < Phases.Length - 1)
        {
            _phaseIndex++;
            _phaseTicks = 0;
            ApplyDeployPhase();
            return null;
        }

        CompleteDeployment();
        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        UpdateFooter();

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.HeaderRow(5, row =>
            {
                row.Weighted(4, _search);
                row.Fixed(24, _environment);
                row.Weighted(3, _deploy);
            });
            window.Left(Math.Min(44, Math.Max(34, context.Width / 3)), _services);
            window.Body(_events);
            window.Footer(1, _status);
        });
    }

    private void ConfigureTheme()
    {
        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);

        _search.TitleStyle = theme.Text.Primary;
        _search.FocusedTitleStyle = focusedBorder.WithBold();
        _search.BorderStyleText = theme.Border.Strong;
        _search.FocusedBorderStyleText = focusedBorder;
        _search.ValueTextStyle = theme.Text.Primary;
        _search.PlaceholderTextStyle = theme.Text.Muted.WithItalic();
        _search.MatchCounterStyle = theme.Text.Secondary;
        _search.MatchHighlightStyle = theme.Accent.Primary.WithBold();
        _search.NavigationLabelStyle = theme.Accent.Secondary;
        _search.DisabledNavigationLabelStyle = theme.Text.Muted.WithDim();

        _environment.TitleStyle = theme.Text.Primary;
        _environment.FocusedTitleStyle = focusedBorder.WithBold();
        _environment.BorderStyleText = theme.Border.Strong;
        _environment.FocusedBorderStyleText = focusedBorder;
        _environment.ValueStyle = theme.Text.Primary;
        _environment.HoveredValueStyle = theme.Accent.Secondary.WithUnderline();
        _environment.OptionStyle = theme.Text.Primary;
        _environment.SelectedOptionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _environment.HoveredOptionStyle = theme.Accent.Secondary.WithUnderline();
        _environment.MutedStyle = theme.Text.Muted.WithItalic();

        _services.TitleStyle = theme.Text.Primary;
        _services.FocusedTitleStyle = focusedBorder.WithBold();
        _services.BorderStyleText = theme.Border.Strong;
        _services.FocusedBorderStyleText = focusedBorder;
        _services.DefaultRowStyle = theme.Text.Primary;
        _services.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();
        _services.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _services.RowMarkers = new ListViewMarkerSet("·", "▶", "▸");

        _events.TitleStyle = theme.Text.Primary;
        _events.FocusedTitleStyle = focusedBorder.WithBold();
        _events.ItemStyle = theme.Text.Primary;
        _events.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _events.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();
        _events.UnreadItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(205, 214, 244));
        _events.MutedItemStyle = theme.Text.Muted.WithDim();
        _events.InfoItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(137, 180, 250));
        _events.SuccessItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(166, 227, 161));
        _events.WarningItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175));
        _events.ErrorItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(243, 139, 168));
        _events.PinnedItemStyle = TeaStyle.Empty.WithBold();
        _events.DisabledStyle = theme.Text.Muted.WithDim();
        _events.EmptyTextStyle = theme.Text.Muted.WithItalic();
        _events.SelectedMarker = ">";
        _events.UnselectedMarker = " ";
    }

    private void ApplyServiceFilter()
    {
        var currentName = _services.SelectedItem?.Name;
        var query = _search.QueryText;

        var visible = _allServices
            .Where(service => MatchesQuery(service, query))
            .ToArray();

        _services.SetItems(visible);
        if (visible.Length > 0)
        {
            var nextIndex = currentName is null
                ? 0
                : Array.FindIndex(visible, service => string.Equals(service.Name, currentName, StringComparison.Ordinal));
            _services.SetSelectedIndex(nextIndex >= 0 ? nextIndex : 0);
        }

        UpdateSearchMatchState();
    }

    private void UpdateSearchMatchState()
    {
        if (_services.Count <= 0)
        {
            _search.ClearMatchState();
            return;
        }

        _search.SetMatchState(_services.Count, _services.SelectedIndex < 0 ? 0 : _services.SelectedIndex);
    }

    private void NavigateMatches(int delta)
    {
        if (_services.Count <= 0)
        {
            return;
        }

        var current = _services.SelectedIndex < 0 ? 0 : _services.SelectedIndex;
        var next = (current + delta + _services.Count) % _services.Count;
        _services.SetSelectedIndex(next);
        UpdateSearchMatchState();
    }

    private void CycleFocus(int delta)
    {
        var current = Array.FindIndex(_focusOrder, static control => control.IsFocused);
        var next = current < 0
            ? 0
            : (current + delta + _focusOrder.Length) % _focusOrder.Length;
        _focusOrder[next].RequestFocus();
    }

    private void StartDeployment()
    {
        var selected = _services.SelectedItem;
        if (selected is null)
        {
            PushEvent("No service selected for deployment.", NotificationLevel.Warning, "Deploy");
            return;
        }

        _deployTarget = selected;
        _deployEnvironment = _environment.SelectedItem;
        _phaseIndex = 0;
        _phaseTicks = 0;
        _deployTarget.Status = "deploying";
        ApplyDeployPhase();
        PushEvent($"Queued {_deployTarget.Name} for {_deployEnvironment}.", NotificationLevel.Warning, "Deploy");
        ApplyServiceFilter();
    }

    private void ApplyDeployPhase()
    {
        if (_deployTarget is null)
        {
            return;
        }

        var phase = Phases[_phaseIndex];
        var accent = TeaStyle.Empty.WithForeground(phase.Accent).WithBold();
        _deploy.SetFrames(phase.Frames);
        _deploy.SetRunning(true);
        _deploy.Label = $"{phase.Label} {_deployTarget.Name} -> {_deployEnvironment}";
        _deploy.TitleStyle = DefaultTheme.Text.Primary;
        _deploy.FocusedTitleStyle = accent;
        _deploy.ValueStyle = DefaultTheme.Text.Primary;
        _deploy.RunningValueStyle = accent;
        _deploy.StoppedValueStyle = DefaultTheme.Text.Secondary;
        _deploy.DisabledValueStyle = DefaultTheme.Text.Muted.WithDim();
        _deploy.BorderStyleText = DefaultTheme.Border.Strong;
        _deploy.FocusedBorderStyleText = DefaultTheme.Border.Focused.Merge(DefaultTheme.Focus.Border).Merge(accent);

        if (_phaseIndex > 0)
        {
            PushEvent($"{phase.Label} {_deployTarget.Name} on {_deployEnvironment}.", NotificationLevel.Info, "Deploy");
        }
    }

    private void CompleteDeployment()
    {
        if (_deployTarget is null)
        {
            return;
        }

        _deployTarget.Status = "healthy";
        _deployTarget.Version++;
        _deploy.SetRunning(false);
        _deploy.SetFrames(["*"]);
        _deploy.Label = $"{_deployTarget.Name} ready on {_deployEnvironment}";

        var accent = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(166, 227, 161)).WithBold();
        _deploy.FocusedTitleStyle = accent;
        _deploy.RunningValueStyle = accent;
        _deploy.StoppedValueStyle = accent;
        _deploy.FocusedBorderStyleText = DefaultTheme.Border.Focused.Merge(DefaultTheme.Focus.Border).Merge(accent);

        PushEvent(
            $"Deploy succeeded: {_deployTarget.Name} {_deployEnvironment} v{_deployTarget.Version}.",
            NotificationLevel.Success,
            "Deploy");

        _deployTarget = null;
        _deployEnvironment = string.Empty;
        _phaseIndex = 0;
        _phaseTicks = 0;
        ApplyServiceFilter();
    }

    private void SetIdleSpinner()
    {
        _deploy.SetFrames(["-"]);
        _deploy.SetRunning(false);
        _deploy.Label = "Idle - press d to deploy";
        _deploy.TitleStyle = DefaultTheme.Text.Primary;
        _deploy.FocusedTitleStyle = DefaultTheme.Text.Primary.WithBold();
        _deploy.ValueStyle = DefaultTheme.Text.Primary;
        _deploy.RunningValueStyle = DefaultTheme.Accent.Primary.WithBold();
        _deploy.StoppedValueStyle = DefaultTheme.Text.Secondary;
        _deploy.DisabledValueStyle = DefaultTheme.Text.Muted.WithDim();
        _deploy.BorderStyleText = DefaultTheme.Border.Strong;
        _deploy.FocusedBorderStyleText = DefaultTheme.Border.Focused.Merge(DefaultTheme.Focus.Border);
    }

    private void SeedEvents()
    {
        _events.SetItems(
        [
            new InboxItem("deploy-0", "gateway deployed to staging", NotificationLevel.Success, new DateTimeOffset(2026, 3, 27, 8, 12, 0, TimeSpan.Zero), "Deploy", isRead: true),
            new InboxItem("deploy-1", "worker drained old tasks", NotificationLevel.Info, new DateTimeOffset(2026, 3, 27, 8, 20, 0, TimeSpan.Zero), "Runtime", isRead: false),
            new InboxItem("deploy-2", "edge latency elevated in gru", NotificationLevel.Warning, new DateTimeOffset(2026, 3, 27, 8, 24, 0, TimeSpan.Zero), "Edge", isRead: false),
            new InboxItem("deploy-3", "billing rollback avoided", NotificationLevel.Success, new DateTimeOffset(2026, 3, 27, 8, 31, 0, TimeSpan.Zero), "Deploy", isRead: true, isPinned: true),
        ]);
    }

    private void PushEvent(string message, NotificationLevel level, string source)
    {
        _events.Add(message, level, source);
    }

    private bool HasActiveDeployment() => _deployTarget is not null;

    private void UpdateFooter()
    {
        var focus = ResolveFocusName();
        var service = _services.SelectedItem?.Name ?? "-";
        var deploy = _deployTarget is null
            ? "idle"
            : _deploy.Running ? $"deploying:{Phases[_phaseIndex].Label.ToLowerInvariant()}" : "paused";

        _status.LeftText =
            $"focus={focus} env={_environment.SelectedItem} service={service} matches={_services.Count} deploy={deploy}";
        _status.RightText =
            $"Tab cycle focus | type filter Enter/F3 next-match | choice open Enter | d deploy p pause/resume | inbox j/k read/pin/delete | ^C quit";
    }

    private string ResolveFocusName()
    {
        if (_search.IsFocused)
        {
            return "filter";
        }

        if (_environment.IsFocused)
        {
            return "env";
        }

        if (_services.IsFocused)
        {
            return "services";
        }

        if (_events.IsFocused)
        {
            return "events";
        }

        return "-";
    }

    private static bool MatchesQuery(ServiceItem service, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return true;
        }

        return service.SearchText().Contains(query.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static List<ServiceItem> CreateServices() =>
    [
        new("api", "healthy", "iad", 17),
        new("worker", "healthy", "sfo", 12),
        new("billing", "warning", "dub", 9),
        new("search", "healthy", "sin", 21),
        new("edge", "warning", "gru", 14),
        new("cron", "healthy", "lhr", 7),
    ];

    private sealed record DeployPulse(DateTimeOffset Timestamp) : Message;

    private sealed record DeployPhase(string Label, string[] Frames, int Ticks, AnsiColor Accent);

    private sealed class ServiceItem(string name, string status, string region, int version)
    {
        public string Name { get; } = name;

        public string Status { get; set; } = status;

        public string Region { get; } = region;

        public int Version { get; set; } = version;

        public string RenderLabel() =>
            $"{Name.PadRight(12)} {Status.PadRight(10)} {Region.PadRight(4)} v{Version}";

        public string SearchText() =>
            $"{Name} {Status} {Region} v{Version}";
    }
}
