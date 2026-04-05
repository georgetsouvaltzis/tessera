using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Examples.OpsWatch;

internal sealed partial class OpsWatchApp : TeaApp
{
    private OpsWatchThemePalette _palette = OpsWatchTheme.Default;
    private readonly OpsWatchState _state = OpsWatchState.CreateSeed();

    private readonly OpsWatchHeroControl _hero = new() { Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _fleetPulse = new() { Title = "Fleet Pulse", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _trafficPulse = new() { Title = "Traffic Shape", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _routePulse = new() { Title = "Route Pressure", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };

    private readonly SideNavRail _fleetRail = new() { Title = "Fleets · F1", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "*" };
    private readonly HealthBoard _healthBoard = new() { Title = "Node Watch Floor · F2", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "*" };
    private readonly ActivityFeed _feed = new() { Title = "Incident Stream · F3", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "*", ShowTimestamp = true, AutoFollow = true };

    private readonly StatsCard _cpuCard = new() { Title = "CPU Saturation", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _memoryCard = new() { Title = "Memory Headroom", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _networkCard = new() { Title = "Network Flux", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _diskCard = new() { Title = "Disk Pressure", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };

    private readonly TelemetryChart _cpuSpark = new(64) { Title = "cpu trace", Border = BorderStyle.Rounded, Padding = new Thickness(1, 0, 1, 0), FocusMarker = "*" };
    private readonly TelemetryChart _memorySpark = new(64) { Title = "mem trace", Border = BorderStyle.Rounded, Padding = new Thickness(1, 0, 1, 0), FocusMarker = "*" };
    private readonly TelemetryChart _networkSpark = new(64) { Title = "net trace", Border = BorderStyle.Rounded, Padding = new Thickness(1, 0, 1, 0), FocusMarker = "*" };
    private readonly TelemetryChart _diskSpark = new(64) { Title = "disk trace", Border = BorderStyle.Rounded, Padding = new Thickness(1, 0, 1, 0), FocusMarker = "*" };

    private readonly StatsCard _focusStats = new() { Title = "Node Focus", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Label _focusSummary = new() { Title = "Focus Readout", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Label _runbook = new() { Title = "Operator Lane", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly BulletChart _cpuBullet = new() { Title = "CPU ceiling", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "*" };
    private readonly BulletChart _memoryBullet = new() { Title = "Memory ceiling", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "*" };
    private readonly BulletChart _networkBullet = new() { Title = "Traffic burn", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "*" };
    private readonly BulletChart _diskBullet = new() { Title = "Disk burn", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "*" };

    private readonly Button _restartButton = new() { Text = "Restart", Description = "r", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _drainButton = new() { Text = "Drain", Description = "d", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _muteButton = new() { Text = "Mute Alerts", Description = "m", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _scaleButton = new() { Text = "Scale", Description = "s", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _inspectButton = new() { Text = "Inspect", Description = "i", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _failoverButton = new() { Text = "Failover", Description = "f", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _ackButton = new() { Text = "Acknowledge", Description = "a", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _veridianThemeButton = new() { Text = "1 Veridian", Description = string.Empty, Border = BorderStyle.None, Padding = Thickness.Symmetric(1, 0) };
    private readonly Button _tidalThemeButton = new() { Text = "2 Tidal", Description = string.Empty, Border = BorderStyle.None, Padding = Thickness.Symmetric(1, 0) };
    private readonly Button _redlineThemeButton = new() { Text = "3 Redline", Description = string.Empty, Border = BorderStyle.None, Padding = Thickness.Symmetric(1, 0) };

    private readonly StatusBar _footer = new() { Fill = ' ' };
    private bool _syncingFleetSelection;
    private bool _syncingNodeSelection;
    private string _lastAction = "steady watch";

    public OpsWatchApp()
    {
        ApplyTheme(_palette);
        WireEvents();
        SeedControls();
        _healthBoard.RequestFocus();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(850), _ => new OpsWatchTickMessage());

    public override TeaEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key:
                return HandleKey(key);
            case OpsWatchActionMessage action:
                _lastAction = _state.Execute(action.Action);
                return null;
            case OpsWatchThemeMessage theme:
                ApplyTheme(OpsWatchTheme.Resolve(theme.Theme));
                _lastAction = $"scene changed to {_palette.Label.ToLowerInvariant()}";
                return null;
            case OpsWatchTickMessage:
                _state.Advance();
                return null;
            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshChrome();
        RefreshControls();

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            ConfigureHeader(window, context);
            window.Body(body => ConfigureBody(body, context));
            window.Footer(1, _footer);
        });
    }

    private TeaEffect? HandleKey(KeyPressed key)
    {
        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.Is(Key.F1))
        {
            _fleetRail.RequestFocus();
            return null;
        }

        if (key.Is(Key.F2))
        {
            _healthBoard.RequestFocus();
            return null;
        }

        if (key.Is(Key.F3))
        {
            _feed.RequestFocus();
            return null;
        }

        if (key.IsCharacter('1'))
        {
            Post(new OpsWatchThemeMessage(OpsWatchThemeKind.Veridian));
            return null;
        }

        if (key.IsCharacter('2'))
        {
            Post(new OpsWatchThemeMessage(OpsWatchThemeKind.Tidal));
            return null;
        }

        if (key.IsCharacter('3'))
        {
            Post(new OpsWatchThemeMessage(OpsWatchThemeKind.Redline));
            return null;
        }

        if (key.IsCharacter('r'))
        {
            Post(new OpsWatchActionMessage(OpsWatchAction.Restart));
            return null;
        }

        if (key.IsCharacter('d'))
        {
            Post(new OpsWatchActionMessage(OpsWatchAction.Drain));
            return null;
        }

        if (key.IsCharacter('m'))
        {
            Post(new OpsWatchActionMessage(OpsWatchAction.MuteAlerts));
            return null;
        }

        if (key.IsCharacter('s'))
        {
            Post(new OpsWatchActionMessage(OpsWatchAction.Scale));
            return null;
        }

        if (key.IsCharacter('i'))
        {
            Post(new OpsWatchActionMessage(OpsWatchAction.Inspect));
            return null;
        }

        if (key.IsCharacter('f'))
        {
            Post(new OpsWatchActionMessage(OpsWatchAction.Failover));
            return null;
        }

        if (key.IsCharacter('a'))
        {
            Post(new OpsWatchActionMessage(OpsWatchAction.Acknowledge));
            return null;
        }

        return null;
    }

    private void SeedControls()
    {
        _fleetRail.SetItems(_state.BuildNavItems());
        _fleetRail.SetSelectedIndex(0);
        _healthBoard.SetServices(_state.BuildServices());
        _feed.MaxItems = 48;
        _feed.SetItems(_state.FeedItems);
        ConfigureBullet(_cpuBullet, "cpu");
        ConfigureBullet(_memoryBullet, "mem");
        ConfigureBullet(_networkBullet, "net");
        ConfigureBullet(_diskBullet, "disk");
    }

    private void WireEvents()
    {
        _fleetRail.SelectionChanged += (_, args) =>
        {
            if (_syncingFleetSelection || args.SelectedItem is null)
            {
                return;
            }

            _state.SelectCluster(args.SelectedItem.Id);
            _lastAction = $"tracking {args.SelectedItem.Label}";
        };

        _healthBoard.SelectionChanged += (_, args) =>
        {
            if (_syncingNodeSelection || args.SelectedItem is null)
            {
                return;
            }

            _state.SelectNode(args.SelectedItem.Id);
            _lastAction = $"focus on {args.SelectedItem.Name}";
        };

        _restartButton.Activated += (_, _) => Post(new OpsWatchActionMessage(OpsWatchAction.Restart));
        _drainButton.Activated += (_, _) => Post(new OpsWatchActionMessage(OpsWatchAction.Drain));
        _muteButton.Activated += (_, _) => Post(new OpsWatchActionMessage(OpsWatchAction.MuteAlerts));
        _scaleButton.Activated += (_, _) => Post(new OpsWatchActionMessage(OpsWatchAction.Scale));
        _inspectButton.Activated += (_, _) => Post(new OpsWatchActionMessage(OpsWatchAction.Inspect));
        _failoverButton.Activated += (_, _) => Post(new OpsWatchActionMessage(OpsWatchAction.Failover));
        _ackButton.Activated += (_, _) => Post(new OpsWatchActionMessage(OpsWatchAction.Acknowledge));
        _veridianThemeButton.Activated += (_, _) => Post(new OpsWatchThemeMessage(OpsWatchThemeKind.Veridian));
        _tidalThemeButton.Activated += (_, _) => Post(new OpsWatchThemeMessage(OpsWatchThemeKind.Tidal));
        _redlineThemeButton.Activated += (_, _) => Post(new OpsWatchThemeMessage(OpsWatchThemeKind.Redline));
    }

    private void RefreshChrome()
    {
        _hero.Title = "OpsWatch // Control Floor";
        _hero.ClockText = _state.ClockText;
        _hero.FleetText = _state.FleetBadge;
        _hero.ModeText = _state.ModeBadge;
        _hero.RouteText = _state.RouteBadge;
        _hero.PressureText = _state.PressureText;
        _hero.CrewText = _state.CrewText;
        _hero.CommandText = _state.CommandText;

        _fleetPulse.SetItems(_state.BuildFleetPulseItems());
        _trafficPulse.SetItems(_state.BuildTrafficPulseItems());
        _routePulse.SetItems(_state.BuildRoutePulseItems());

        _footer.LeftText = $"opswatch  {_palette.Label.ToLowerInvariant()}  {_state.SelectedClusterName}  {_state.AutomationMode}  alerts {_state.ActiveAlertCount:00}  drains {_state.DrainingCount:00}";
        _footer.RightText = "1/2/3 theme  F1 fleets  F2 nodes  F3 feed  r restart  d drain  m mute  s scale  f failover  a ack";
    }

    private void RefreshControls()
    {
        SyncFleetRail();
        SyncHealthBoard();
        SyncMetricDeck();
        SyncFocusLane();
        _feed.SetItems(_state.FeedItems);
    }

    private void SyncFleetRail()
    {
        _syncingFleetSelection = true;
        var items = _state.BuildNavItems();
        _fleetRail.SetItems(items);
        var selectedIndex = items
            .Select((item, index) => new { item, index })
            .FirstOrDefault(entry => string.Equals(entry.item.Label, _state.SelectedClusterName, StringComparison.Ordinal))?.index ?? -1;
        if (selectedIndex >= 0)
        {
            _fleetRail.SetSelectedIndex(selectedIndex);
        }

        _syncingFleetSelection = false;
    }

    private void SyncHealthBoard()
    {
        _syncingNodeSelection = true;
        var services = _state.BuildServices();
        _healthBoard.SetServices(services);
        var selectedIndex = Array.FindIndex(services.ToArray(), service => string.Equals(service.Id, _state.SelectedNode.Id, StringComparison.Ordinal));
        if (selectedIndex >= 0)
        {
            _healthBoard.SetSelectedIndex(selectedIndex);
        }

        _syncingNodeSelection = false;
    }

    private void SyncMetricDeck()
    {
        _cpuCard.SetItems(OpsWatchState.BuildMetricCardItems("cpu", _state.CpuAverage, DeltaText(_state.CpuTrend)));
        _memoryCard.SetItems(OpsWatchState.BuildMetricCardItems("mem", _state.MemoryAverage, DeltaText(_state.MemoryTrend)));
        _networkCard.SetItems(OpsWatchState.BuildMetricCardItems("net", _state.NetworkAverage, DeltaText(_state.NetworkTrend)));
        _diskCard.SetItems(OpsWatchState.BuildMetricCardItems("disk", _state.DiskAverage, DeltaText(_state.DiskTrend)));

        _cpuSpark.SetSamples(_state.CpuTrend);
        _memorySpark.SetSamples(_state.MemoryTrend);
        _networkSpark.SetSamples(_state.NetworkTrend);
        _diskSpark.SetSamples(_state.DiskTrend);

        SyncBullet(_cpuBullet, _state.SelectedNode.Cpu, 72, 90);
        SyncBullet(_memoryBullet, _state.SelectedNode.Memory, 74, 92);
        SyncBullet(_networkBullet, _state.SelectedNode.Network, 68, 88);
        SyncBullet(_diskBullet, _state.SelectedNode.Disk, 66, 86);
    }

    private void SyncFocusLane()
    {
        _focusStats.SetItems(_state.BuildSelectedNodeItems());
        _focusSummary.Text = _state.BuildFocusText();
        _runbook.Text = _state.BuildRunbookText();
    }

    private void ApplyTheme(OpsWatchThemePalette palette)
    {
        _palette = palette;
        var theme = palette.Theme;

        _fleetRail.ApplyTheme(theme);
        _healthBoard.ApplyTheme(theme);
        _feed.ApplyTheme(theme);
        _fleetPulse.ApplyTheme(theme);
        _trafficPulse.ApplyTheme(theme);
        _routePulse.ApplyTheme(theme);
        _cpuCard.ApplyTheme(theme);
        _memoryCard.ApplyTheme(theme);
        _networkCard.ApplyTheme(theme);
        _diskCard.ApplyTheme(theme);
        _cpuSpark.ApplyTheme(theme);
        _memorySpark.ApplyTheme(theme);
        _networkSpark.ApplyTheme(theme);
        _diskSpark.ApplyTheme(theme);
        _focusStats.ApplyTheme(theme);
        _focusSummary.ApplyTheme(theme);
        _runbook.ApplyTheme(theme);
        _cpuBullet.ApplyTheme(theme);
        _memoryBullet.ApplyTheme(theme);
        _networkBullet.ApplyTheme(theme);
        _diskBullet.ApplyTheme(theme);
        _restartButton.ApplyTheme(theme);
        _drainButton.ApplyTheme(theme);
        _muteButton.ApplyTheme(theme);
        _scaleButton.ApplyTheme(theme);
        _inspectButton.ApplyTheme(theme);
        _failoverButton.ApplyTheme(theme);
        _ackButton.ApplyTheme(theme);
        _veridianThemeButton.ApplyTheme(theme);
        _tidalThemeButton.ApplyTheme(theme);
        _redlineThemeButton.ApplyTheme(theme);
        _footer.ApplyTheme(theme);

        _hero.TitleStyle = OpsWatchTheme.Foreground(palette.HeroTitleColor).WithBold();
        _hero.ClockStyle = OpsWatchTheme.Foreground(palette.HeroClockColor).WithBold();
        _hero.BadgeStyle = OpsWatchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
        _hero.MetaStyle = theme.Text.Secondary;
        _hero.CommandStyle = OpsWatchTheme.Foreground(palette.HeroCommandColor).WithBold();
        _hero.BorderStyleText = theme.Border.Strong;
        _hero.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);

        ConfigurePulseCard(_fleetPulse, OpsWatchTheme.Foreground(palette.PulsePrimaryColor).WithBold(), palette);
        ConfigurePulseCard(_trafficPulse, OpsWatchTheme.Foreground(palette.PulseSecondaryColor).WithBold(), palette);
        ConfigurePulseCard(_routePulse, OpsWatchTheme.Foreground(palette.PulseTertiaryColor).WithBold(), palette);

        ConfigureMetricCard(_cpuCard, OpsWatchTheme.Foreground(palette.CpuColor).WithBold(), palette);
        ConfigureMetricCard(_memoryCard, OpsWatchTheme.Foreground(palette.MemoryColor).WithBold(), palette);
        ConfigureMetricCard(_networkCard, OpsWatchTheme.Foreground(palette.NetworkColor).WithBold(), palette);
        ConfigureMetricCard(_diskCard, OpsWatchTheme.Foreground(palette.DiskColor).WithBold(), palette);

        ConfigureSpark(_cpuSpark, OpsWatchTheme.Foreground(palette.CpuColor), palette);
        ConfigureSpark(_memorySpark, OpsWatchTheme.Foreground(palette.MemoryColor), palette);
        ConfigureSpark(_networkSpark, OpsWatchTheme.Foreground(palette.NetworkColor), palette);
        ConfigureSpark(_diskSpark, OpsWatchTheme.Foreground(palette.DiskColor), palette);
        _fleetRail.BorderStyleText = theme.Border.Strong;
        _fleetRail.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        _fleetRail.ItemStyle = theme.Text.Primary;
        _fleetRail.SelectedItemStyle = OpsWatchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
        _fleetRail.FocusedSelectedItemStyle = OpsWatchTheme.Chip(palette.HeroBadgeForeground, palette.FooterChipBackground);
        _fleetRail.HoveredItemStyle = theme.Accent.Secondary;

        _healthBoard.BorderStyleText = theme.Border.Strong;
        _healthBoard.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        _healthBoard.ServiceStyle = theme.Text.Primary;
        _healthBoard.HealthyServiceStyle = theme.State.Success;
        _healthBoard.DegradedServiceStyle = theme.State.Warning;
        _healthBoard.OutageServiceStyle = theme.State.Error.WithBold();
        _healthBoard.HoveredServiceStyle = theme.Accent.Secondary;
        _healthBoard.SelectedServiceStyle = OpsWatchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
        _healthBoard.FocusedSelectedServiceStyle = OpsWatchTheme.Chip(palette.HeroBadgeForeground, palette.FooterChipBackground);
        _healthBoard.AcknowledgedServiceStyle = theme.State.Info;
        _healthBoard.MutedServiceStyle = theme.Text.Muted;
        _healthBoard.Glyphs = new HealthBoardGlyphSet(".", ">", "+", "OK", "~", "!!", "ACK", " ");

        _feed.BorderStyleText = theme.Border.Strong;
        _feed.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        _feed.InfoItemStyle = theme.Text.Secondary;
        _feed.SuccessItemStyle = theme.State.Success;
        _feed.WarningItemStyle = theme.State.Warning;
        _feed.ErrorItemStyle = theme.State.Error;
        _feed.SelectedItemStyle = OpsWatchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
        _feed.FocusedSelectedItemStyle = OpsWatchTheme.Chip(palette.HeroBadgeForeground, palette.FooterChipBackground);
        _feed.HoveredItemStyle = theme.Accent.Secondary;
        _feed.TimestampStyle = theme.Text.Muted;
        _feed.SelectedMarker = ">";
        _feed.UnselectedMarker = ".";
        _feed.UnreadMarker = "*";

        _focusStats.BorderStyleText = theme.Border.Strong;
        _focusStats.ValueStyle = theme.Text.Primary.WithBold();
        _focusSummary.BorderStyleText = theme.Border.Strong;
        _focusSummary.TextStyle = theme.Text.Primary;
        _runbook.BorderStyleText = theme.Border.Strong;
        _runbook.TextStyle = theme.Text.Secondary;

        ConfigureActionButton(_restartButton, palette.HeroBadgeForeground, palette.PulseTertiaryColor, palette);
        ConfigureActionButton(_drainButton, palette.HeroBadgeForeground, palette.PulseSecondaryColor, palette);
        ConfigureActionButton(_muteButton, palette.HeroBadgeForeground, palette.FrameStrongColor, palette);
        ConfigureActionButton(_scaleButton, palette.HeroBadgeForeground, palette.PulsePrimaryColor, palette);
        ConfigureActionButton(_inspectButton, palette.HeroBadgeForeground, palette.MemoryColor, palette);
        ConfigureActionButton(_failoverButton, palette.HeroBadgeForeground, palette.DiskColor, palette);
        ConfigureActionButton(_ackButton, palette.HeroBadgeForeground, palette.CpuColor, palette);

        ConfigureThemeButton(_veridianThemeButton, OpsWatchThemeKind.Veridian);
        ConfigureThemeButton(_tidalThemeButton, OpsWatchThemeKind.Tidal);
        ConfigureThemeButton(_redlineThemeButton, OpsWatchThemeKind.Redline);

        _footer.LeftTextStyle = OpsWatchTheme.Chip(palette.FooterChipForeground, palette.FooterChipBackground);
        _footer.RightTextStyle = theme.Text.Secondary;
        _footer.FillStyle = theme.Surface.Panel;
    }

    private void ConfigureThemeButton(Button button, OpsWatchThemeKind kind)
    {
        var isSelected = _palette.Kind == kind;
        button.LabelStyle = isSelected
            ? OpsWatchTheme.Foreground(_palette.HeroBadgeForeground).WithBold()
            : _palette.Theme.Text.Secondary.WithBold();
        button.FocusedLabelStyle = isSelected
            ? OpsWatchTheme.Foreground(_palette.HeroBadgeForeground).WithBold()
            : _palette.Theme.Text.Secondary.WithBold();
        button.PressedLabelStyle = button.LabelStyle;
        button.SurfaceStyle = isSelected
            ? OpsWatchTheme.Background(_palette.HeroBadgeBackground)
            : _palette.Theme.Surface.Overlay;
        button.FocusedSurfaceStyle = button.SurfaceStyle;
        button.PressedSurfaceStyle = isSelected
            ? OpsWatchTheme.Background(_palette.FooterChipBackground)
            : _palette.Theme.Selection.Background;
        button.BorderStyleText = TeaStyle.Empty;
        button.FocusedBorderStyleText = TeaStyle.Empty;
        button.LabelPrefix = string.Empty;
        button.LabelSuffix = string.Empty;
    }

    private static void ConfigurePulseCard(StatsCard card, TeaStyle valueStyle, OpsWatchThemePalette palette)
    {
        card.TitleStyle = palette.Theme.Text.Secondary.WithBold();
        card.ValueStyle = valueStyle;
        card.KeyStyle = palette.Theme.Text.Muted;
        card.BorderStyleText = OpsWatchTheme.Foreground(palette.FrameStrongColor);
    }

    private static void ConfigureMetricCard(StatsCard card, TeaStyle valueStyle, OpsWatchThemePalette palette)
    {
        card.TitleStyle = palette.Theme.Text.Secondary.WithBold();
        card.ValueStyle = valueStyle;
        card.KeyStyle = palette.Theme.Text.Muted;
        card.BorderStyleText = OpsWatchTheme.Foreground(palette.FrameMutedColor);
    }

    private static void ConfigureSpark(TelemetryChart spark, TeaStyle dataStyle, OpsWatchThemePalette palette)
    {
        spark.Options = new TelemetryChartOptions(ShowStats: false, RenderMode: TelemetryChartRenderMode.Braille);
        spark.TitleStyle = palette.Theme.Text.Secondary.WithBold();
        spark.FocusedTitleStyle = palette.Theme.Focus.Title;
        spark.FillStyle = dataStyle.WithBold();
        spark.MetaStyle = palette.Theme.Text.Muted;
        spark.EmptyTextStyle = palette.Theme.Text.Muted;
        spark.BorderStyleText = OpsWatchTheme.Foreground(palette.FrameStrongColor);
        spark.FocusedBorderStyleText = palette.Theme.Focus.Border.Merge(dataStyle);
        spark.MinValue = null;
        spark.MaxValue = null;
    }

    private static void ConfigureActionButton(Button button, int foregroundRgb, int backgroundRgb, OpsWatchThemePalette palette)
    {
        var labelStyle = OpsWatchTheme.Foreground(foregroundRgb).WithBold();
        var surfaceStyle = OpsWatchTheme.Background(backgroundRgb);
        button.LabelStyle = labelStyle.WithBold();
        button.FocusedLabelStyle = labelStyle.WithBold();
        button.PressedLabelStyle = labelStyle.WithBold();
        button.SurfaceStyle = surfaceStyle;
        button.FocusedSurfaceStyle = surfaceStyle;
        button.PressedSurfaceStyle = surfaceStyle;
        button.BorderStyleText = OpsWatchTheme.Foreground(palette.FrameMutedColor);
        button.FocusedBorderStyleText = palette.Theme.Focus.Border;
        button.LabelPrefix = string.Empty;
        button.LabelSuffix = string.Empty;
    }

    private void ConfigureBullet(BulletChart chart, string unit)
    {
        chart.SetRanges(
        [
            new BulletRange(0, 60, BulletRangeKind.Normal),
            new BulletRange(60, 82, BulletRangeKind.Warning),
            new BulletRange(82, 100, BulletRangeKind.Critical),
        ]);
        chart.ValueLabelStyle = _palette.Theme.Text.Primary.WithBold();
        chart.BorderStyleText = OpsWatchTheme.Foreground(_palette.FrameMutedColor);
        chart.FocusedBorderStyleText = _palette.Theme.Focus.Border;
        chart.RangeStyle = OpsWatchTheme.Foreground(_palette.FrameMutedColor);
        chart.WarningRangeStyle = _palette.Theme.State.Warning;
        chart.CriticalRangeStyle = _palette.Theme.State.Error;
        chart.ValueBarStyle = OpsWatchTheme.Foreground(_palette.PulsePrimaryColor).WithBold();
        chart.TargetMarkerStyle = OpsWatchTheme.Foreground(_palette.PulseSecondaryColor).WithBold();
        chart.SetValue(0);
        chart.SetTarget(0);
    }

    private void SyncBullet(BulletChart chart, double value, double target, double warning)
    {
        chart.SetValue(value);
        chart.SetTarget(target);
        chart.ValueLabelStyle = value >= warning
            ? _palette.Theme.State.Error.WithBold()
            : _palette.Theme.Text.Primary.WithBold();
    }

    private static string DeltaText(IReadOnlyList<double> samples)
    {
        if (samples.Count < 2)
        {
            return "flat";
        }

        var delta = samples[^1] - samples[^2];
        return delta >= 0 ? $"+{delta:0}" : $"{delta:0}";
    }
}

internal sealed record OpsWatchTickMessage : Message;

internal sealed record OpsWatchActionMessage(OpsWatchAction Action) : Message;

internal sealed record OpsWatchThemeMessage(OpsWatchThemeKind Theme) : Message;
