using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Examples.DownloadCenter;

internal sealed partial class DownloadCenterApp : TeaApp
{
    private readonly TeaTheme _theme = DownloadCenterTheme.Default;
    private readonly DownloadCenterState _state = DownloadCenterState.CreateSeed();

    private readonly DownloadHeroControl _hero = new() { Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _lanePulse = new() { Title = "Live Lanes", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _pipePulse = new() { Title = "Throughput Crest", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _retryPulse = new() { Title = "Retry Pressure", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly TransferQueueControl _queue = new() { Title = "Grouped Jobs · F1", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◈" };
    private readonly Label _selectionCard = new() { Title = "Selected Transfer", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly ProgressBar _progress = new() { Title = "Seal Progress", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◈" };
    private readonly Label _runbook = new() { Title = "Action Bar", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly TelemetryChart _throughputChart = new(64) { Title = "throughput crest", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly TelemetryChart _retryChart = new(64) { Title = "retry turbulence", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly ActivityFeed _feed = new() { Title = "Transfer Feed · F2", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◈", ShowTimestamp = true };
    private readonly Button _pauseButton = new() { Text = "Pause/Resume", Description = "p", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _retryButton = new() { Text = "Retry Now", Description = "r", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _boostButton = new() { Text = "Boost Lane", Description = "b", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _purgeButton = new() { Text = "Purge Done", Description = "u", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly StatusBar _footer = new() { Fill = ' ' };

    public DownloadCenterApp()
    {
        ConfigureTheme();
        WireEvents();
        SeedControls();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(900), _ => new DownloadCenterTickMessage());

    public override TeaEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key:
                return HandleKey(key);
            case DownloadCenterTickMessage:
                _state.Advance();
                return null;
            case DownloadCenterActionMessage action:
                _state.Execute(action.Action);
                return null;
            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
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

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            _state.MoveSelection(-1);
            return null;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            _state.MoveSelection(1);
            return null;
        }

        if (key.IsCharacter('p'))
        {
            _state.Execute(DownloadCenterAction.PauseResume);
            return null;
        }

        if (key.IsCharacter('r'))
        {
            _state.Execute(DownloadCenterAction.RetryNow);
            return null;
        }

        if (key.IsCharacter('b'))
        {
            _state.Execute(DownloadCenterAction.BoostLane);
            return null;
        }

        if (key.IsCharacter('u'))
        {
            _state.Execute(DownloadCenterAction.PurgeCompleted);
            return null;
        }

        return null;
    }

    private void SeedControls()
    {
        _feed.MaxItems = 48;
        _feed.SelectedMarker = "▶";
        _feed.UnselectedMarker = "·";
        _feed.UnreadMarker = "◆";
    }

    private void WireEvents()
    {
        _pauseButton.Activated += (_, _) => Post(new DownloadCenterActionMessage(DownloadCenterAction.PauseResume));
        _retryButton.Activated += (_, _) => Post(new DownloadCenterActionMessage(DownloadCenterAction.RetryNow));
        _boostButton.Activated += (_, _) => Post(new DownloadCenterActionMessage(DownloadCenterAction.BoostLane));
        _purgeButton.Activated += (_, _) => Post(new DownloadCenterActionMessage(DownloadCenterAction.PurgeCompleted));
    }

    private void RefreshControls()
    {
        _hero.Title = "Download Center // Kinetic Relay";
        _hero.ClockText = _state.ClockText;
        _hero.SummaryText = _state.SummaryBadge;
        _hero.ThroughputText = _state.ThroughputBadge;
        _hero.PressureText = _state.PressureBadge;
        _hero.CommandText = _state.BuildCommandText();

        _lanePulse.SetItems(_state.BuildPulseItems("lanes"));
        _pipePulse.SetItems(_state.BuildPulseItems("pipe"));
        _retryPulse.SetItems(_state.BuildPulseItems("retry"));

        _queue.SetSections(_state.BuildSections());
        _queue.SelectedId = _state.SelectedJob.Id;
        _selectionCard.Text = _state.BuildSelectionSummary();
        _progress.SetValue(_state.SelectedJob.ProgressPercent / 100d);
        _runbook.Text = string.Join('\n',
            "shortcuts",
            "↑/↓ move focus lane",
            "p pause or resume selected",
            "r force retry immediately",
            "b reserve fast lane",
            "u archive sealed jobs");

        _throughputChart.SetSamples(_state.ThroughputTrend);
        _retryChart.SetSamples(_state.RetryTrend);
        _feed.SetItems(_state.FeedItems);

        _footer.LeftText = $"download center  {_state.SummaryBadge.ToLowerInvariant()}";
        _footer.RightText = "↑/↓ select  p pause  r retry  b boost  u purge  Ctrl+C quit";
    }

    private void ConfigureTheme()
    {
        _lanePulse.ApplyTheme(_theme);
        _pipePulse.ApplyTheme(_theme);
        _retryPulse.ApplyTheme(_theme);
        _selectionCard.ApplyTheme(_theme);
        _progress.ApplyTheme(_theme);
        _runbook.ApplyTheme(_theme);
        _throughputChart.ApplyTheme(_theme);
        _retryChart.ApplyTheme(_theme);
        _feed.ApplyTheme(_theme);
        _pauseButton.ApplyTheme(_theme);
        _retryButton.ApplyTheme(_theme);
        _boostButton.ApplyTheme(_theme);
        _purgeButton.ApplyTheme(_theme);
        _footer.ApplyTheme(_theme);

        _hero.TitleStyle = _theme.Text.Primary.WithBold();
        _hero.ClockStyle = _theme.Accent.Primary.WithBold();
        _hero.BadgeStyle = DownloadCenterTheme.Chip(0xF6FBFF, 0x1D3A68);
        _hero.MetaStyle = _theme.Text.Secondary;
        _hero.CommandStyle = DownloadCenterTheme.Foreground(0xFFD166).WithBold();
        _hero.BorderStyleText = _theme.Border.Strong;

        ConfigurePulse(_lanePulse, _theme.Accent.Primary.WithBold());
        ConfigurePulse(_pipePulse, DownloadCenterTheme.Foreground(0x7FDBFF).WithBold());
        ConfigurePulse(_retryPulse, DownloadCenterTheme.Foreground(0xFF9B71).WithBold());

        _queue.TitleStyle = _theme.Text.Secondary.WithBold();
        _queue.FocusedTitleStyle = _theme.Focus.Title;
        _queue.BorderStyleText = _theme.Border.Strong;
        _queue.FocusedBorderStyleText = _theme.Focus.Border;
        _queue.SectionStyle = DownloadCenterTheme.Foreground(0xD5B3FF).WithBold();
        _queue.MetaStyle = _theme.Text.Muted;
        _queue.ItemStyle = _theme.Text.Primary;
        _queue.SelectedItemStyle = DownloadCenterTheme.Chip(0xF7FBFF, 0x275590);
        _queue.ActiveStyle = DownloadCenterTheme.Foreground(0x7FDBFF).WithBold();
        _queue.RetryStyle = DownloadCenterTheme.Foreground(0xFF9B71).WithBold();
        _queue.CompleteStyle = DownloadCenterTheme.Foreground(0x5EF0A5).WithBold();
        _queue.QueuedStyle = _theme.Text.Secondary;

        _selectionCard.TitleStyle = _theme.Text.Secondary.WithBold();
        _selectionCard.BorderStyleText = _theme.Border.Strong;
        _selectionCard.TextStyle = _theme.Text.Primary;

        _progress.TitleStyle = _theme.Text.Secondary.WithBold();
        _progress.BorderStyleText = _theme.Border.Strong;
        _progress.FocusedBorderStyleText = _theme.Focus.Border;
        _progress.FillStyle = DownloadCenterTheme.Foreground(0x7FDBFF).WithBold();
        _progress.TrackStyle = DownloadCenterTheme.Foreground(0x2C3858);
        _progress.LabelStyle = _theme.Text.Primary.WithBold();

        _runbook.TitleStyle = _theme.Text.Secondary.WithBold();
        _runbook.BorderStyleText = _theme.Border.Strong;
        _runbook.TextStyle = _theme.Text.Secondary;

        ConfigureTelemetry(_throughputChart, DownloadCenterTheme.Foreground(0x7FDBFF).WithBold(), "pipe crest");
        ConfigureTelemetry(_retryChart, DownloadCenterTheme.Foreground(0xFF9B71).WithBold(), "retry pressure");

        _feed.TitleStyle = _theme.Text.Secondary.WithBold();
        _feed.FocusedTitleStyle = _theme.Focus.Title;
        _feed.BorderStyleText = _theme.Border.Strong;
        _feed.FocusedBorderStyleText = _theme.Focus.Border;
        _feed.InfoItemStyle = _theme.Accent.Primary;
        _feed.SuccessItemStyle = _theme.State.Success;
        _feed.WarningItemStyle = _theme.State.Warning;
        _feed.ErrorItemStyle = _theme.State.Error;
        _feed.SelectedItemStyle = DownloadCenterTheme.Chip(0xF7FBFF, 0x275590);
        _feed.FocusedSelectedItemStyle = DownloadCenterTheme.Chip(0xF7FBFF, 0x375E99);
        _feed.TimestampStyle = _theme.Text.Muted;

        ConfigureAction(_pauseButton, 0x08101F, 0x7FDBFF);
        ConfigureAction(_retryButton, 0x08101F, 0xFF9B71);
        ConfigureAction(_boostButton, 0x08101F, 0xD5B3FF);
        ConfigureAction(_purgeButton, 0x08101F, 0x5EF0A5);

        _footer.LeftTextStyle = DownloadCenterTheme.Chip(0xF7FBFF, 0x234A77);
        _footer.RightTextStyle = _theme.Text.Secondary;
        _footer.FillStyle = _theme.Surface.Panel;
    }

    private static void ConfigurePulse(StatsCard card, TeaStyle valueStyle)
    {
        card.TitleStyle = DownloadCenterTheme.Foreground(0xA5B4D4).WithBold();
        card.ValueStyle = valueStyle;
        card.KeyStyle = DownloadCenterTheme.Foreground(0x6B7899);
        card.BorderStyleText = DownloadCenterTheme.Foreground(0x34517B);
    }

    private static void ConfigureTelemetry(TelemetryChart chart, TeaStyle fillStyle, string legend)
    {
        chart.Options = new TelemetryChartOptions(ShowStats: true, Legend: legend, RenderMode: TelemetryChartRenderMode.Braille);
        chart.TitleStyle = DownloadCenterTheme.Foreground(0xA5B4D4).WithBold();
        chart.FillStyle = fillStyle;
        chart.MetaStyle = DownloadCenterTheme.Foreground(0x6B7899);
        chart.BorderStyleText = DownloadCenterTheme.Foreground(0x34517B);
        chart.FocusedBorderStyleText = DownloadCenterTheme.Foreground(0xFF9B71).WithBold();
    }

    private static void ConfigureAction(Button button, int foregroundRgb, int backgroundRgb)
    {
        var labelStyle = DownloadCenterTheme.Foreground(foregroundRgb).WithBold();
        var surfaceStyle = DownloadCenterTheme.Background(backgroundRgb);
        button.LabelStyle = labelStyle;
        button.FocusedLabelStyle = labelStyle;
        button.PressedLabelStyle = labelStyle;
        button.SurfaceStyle = surfaceStyle;
        button.FocusedSurfaceStyle = surfaceStyle;
        button.PressedSurfaceStyle = surfaceStyle;
        button.BorderStyleText = DownloadCenterTheme.Foreground(0x34517B);
        button.FocusedBorderStyleText = DownloadCenterTheme.Foreground(0xFF9B71).WithBold();
        button.LabelPrefix = string.Empty;
        button.LabelSuffix = string.Empty;
    }
}

internal sealed record DownloadCenterTickMessage : Message;

internal sealed record DownloadCenterActionMessage(DownloadCenterAction Action) : Message;
