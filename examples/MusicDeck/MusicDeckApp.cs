using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.MusicDeck;

internal sealed partial class MusicDeckApp : TesseraApp
{
    private readonly Button _backButton = new() { Text = "Back", Padding = Thickness.Symmetric(2, 1) };

    private readonly StatsCard _deckStats =
        new() { Title = "Playback Stats", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };

    private readonly Button _detailButton = new() { Text = "Notes", Padding = Thickness.Symmetric(2, 1) };
    private readonly StatusBar _footer = new() { Fill = ' ' };

    private readonly Label _lyrics = new()
    {
        Title = "Lyric Sheet · F2",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(2, 1)
    };

    private readonly Button _nextButton = new() { Text = "Next", Padding = Thickness.Symmetric(2, 1) };

    private readonly MusicDeckNowPlayingControl _nowPlaying =
        new() { Border = BorderStyle.Rounded, Padding = Thickness.All(1) };

    private readonly Button _playPauseButton = new() { Text = "Pause", Padding = Thickness.Symmetric(2, 1) };

    private readonly ProgressBar _progress = new()
    {
        Title = "Playback Drift",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "✦"
    };

    private readonly MusicDeckQueueControl _queue = new()
    {
        Title = "Queue · F1",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1)
    };

    private readonly Label _sessionMeta = new()
    {
        Title = "Session Notes",
        Border = BorderStyle.Rounded,
        Padding = Thickness.Symmetric(2, 1)
    };

    private readonly MusicDeckState _state = MusicDeckState.CreateSeed();

    private readonly StatsCard _trackStats =
        new() { Title = "Track Details", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };

    private readonly Label _transportLeftSpacer = new() { Border = BorderStyle.None, Text = string.Empty };
    private readonly Label _transportRightSpacer = new() { Border = BorderStyle.None, Text = string.Empty };

    public MusicDeckApp()
    {
        ConfigureTheme();
        WireEvents();
        _queue.RequestFocus();
    }

    public override TesseraEffect? Initialize()
    {
        return TesseraEffects.Periodic(TimeSpan.FromMilliseconds(1000), _ => new MusicDeckTickMessage());
    }

    public override TesseraEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key:
                return HandleKey(key);
            case MusicDeckTickMessage:
                _state.Tick();
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
            window.Body(body => ConfigureBody(body));
            window.Footer(1, _footer);
        });
    }

    private TesseraEffect? HandleKey(KeyPressed key)
    {
        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TesseraEffects.Quit;
        }

        if (key.Is(Key.F1))
        {
            _queue.RequestFocus();
            return null;
        }

        if (key.Is(Key.F2))
        {
            _lyrics.RequestFocus();
            return null;
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

        if (key.Is(Key.Enter))
        {
            _state.CueSelected();
            return null;
        }

        if (key.IsCharacter(' '))
        {
            _state.TogglePlayPause();
            return null;
        }

        if (key.IsCharacter('n'))
        {
            _state.Next();
            return null;
        }

        if (key.IsCharacter('p'))
        {
            _state.Previous();
            return null;
        }

        if (key.IsCharacter('l'))
        {
            _state.ToggleDetailMode();
            return null;
        }

        return null;
    }

    private void WireEvents()
    {
        _backButton.Activated += (_, _) => _state.Previous();
        _playPauseButton.Activated += (_, _) => _state.TogglePlayPause();
        _nextButton.Activated += (_, _) => _state.Next();
        _detailButton.Activated += (_, _) => _state.ToggleDetailMode();
    }

    private void RefreshControls()
    {
        var current = _state.CurrentTrack;
        _nowPlaying.TrackTitle = current.Title.ToUpperInvariant();
        _nowPlaying.ArtistLine = _state.BuildHeroSummary();
        _nowPlaying.SceneChip = _state.DeckLabel;
        _nowPlaying.DeviceChip = MusicDeckState.DeviceLabel;
        _nowPlaying.RoomChip = MusicDeckState.RoomLabel;
        _nowPlaying.ProgressLine = _state.ProgressText;
        _nowPlaying.RemainingLine = _state.RemainingText;
        _nowPlaying.SummaryLine = current.Summary;

        _queue.SetItems(_state.Queue);
        _queue.CurrentIndex = _state.CurrentIndex;
        _queue.SelectedIndex = _state.SelectedIndex;

        _progress.SetValue(_state.Progress);
        _playPauseButton.Text = _state.IsPlaying ? "Pause" : "Play";
        _detailButton.Text = _state.ShowingLinerNotes ? "Lyrics" : "Notes";
        _deckStats.SetItems(_state.BuildDeckStats());
        _trackStats.SetItems(_state.BuildSessionStats());
        _sessionMeta.Text = _state.BuildMetaText();
        _lyrics.Title = _state.ShowingLinerNotes ? "Liner Notes · F2" : "Lyric Sheet · F2";
        _lyrics.Text = _state.BuildLyricsOrNotes();
        _footer.LeftText =
            $"musicdeck  {_state.CurrentTrack.Artist}  {_state.CurrentTrack.Album}  {_state.ProgressText}";
        _footer.RightText =
            "F1 queue  F2 notes  j/k browse  enter cue  space play/pause  p previous  n next  l swap sheet";
    }

    private void ConfigureTheme()
    {
        var theme = MusicDeckTheme.DefaultTheme;

        _progress.ApplyTheme(theme);
        _deckStats.ApplyTheme(theme);
        _trackStats.ApplyTheme(theme);
        _sessionMeta.ApplyTheme(theme);
        _lyrics.ApplyTheme(theme);
        _backButton.ApplyTheme(theme);
        _playPauseButton.ApplyTheme(theme);
        _nextButton.ApplyTheme(theme);
        _detailButton.ApplyTheme(theme);
        _footer.ApplyTheme(theme);

        _nowPlaying.TitleStyle = theme.Text.Secondary.WithBold();
        _nowPlaying.TrackStyle = MusicDeckTheme.Foreground(0xFFF4DE).WithBold();
        _nowPlaying.ArtistStyle = theme.Accent.Secondary.WithBold();
        _nowPlaying.ChipStyle = MusicDeckTheme.Chip(0x1B1010, 0xF1B577);
        _nowPlaying.SummaryStyle = theme.Text.Secondary;
        _nowPlaying.ProgressStyle = theme.Accent.Primary.WithBold();
        _nowPlaying.BorderStyleText = theme.Border.Strong;

        _queue.TitleStyle = theme.Text.Secondary.WithBold();
        _queue.FocusedTitleStyle = theme.Focus.Title;
        _queue.ItemStyle = theme.Text.Primary;
        _queue.CurrentItemStyle = theme.Accent.Primary.WithBold();
        _queue.SelectedItemStyle = MusicDeckTheme.Chip(0x1B1010, 0xE79BA8);
        _queue.MetaStyle = theme.Text.Muted;
        _queue.BorderStyleText = theme.Border.Strong;
        _queue.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);

        _progress.TitleStyle = theme.Text.Secondary.WithBold();
        _progress.FocusedTitleStyle = theme.Focus.Title;
        _progress.BorderStyleText = theme.Border.Strong;
        _progress.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        _progress.FillStyle = MusicDeckTheme.ForegroundBackground(0x1B1010, 0xF1B577).WithBold();
        _progress.TrackStyle = MusicDeckTheme.ForegroundBackground(0xA08172, 0x2A1820);
        _progress.LabelStyle = theme.Text.Secondary.WithBold();

        ConfigureButton(_backButton);
        ConfigureButton(_playPauseButton);
        ConfigureButton(_nextButton);
        ConfigureButton(_detailButton);

        ConfigureCard(_deckStats, theme.Accent.Primary.WithBold());
        ConfigureCard(_trackStats, theme.Accent.Secondary.WithBold());

        _sessionMeta.TitleStyle = theme.Text.Secondary.WithBold();
        _sessionMeta.BorderStyleText = theme.Border.Strong;
        _sessionMeta.TextStyle = theme.Text.Primary;

        _lyrics.TitleStyle = theme.Text.Secondary.WithBold();
        _lyrics.FocusedTitleStyle = theme.Focus.Title;
        _lyrics.BorderStyleText = theme.Border.Strong;
        _lyrics.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        _lyrics.TextStyle = theme.Text.Primary;

        _footer.LeftTextStyle = MusicDeckTheme.Chip(0x1B1010, 0xF1B577);
        _footer.RightTextStyle = theme.Text.Secondary;
        _footer.FillStyle = theme.Surface.Panel;
    }

    private static void ConfigureButton(Button button)
    {
        button.LabelStyle = MusicDeckTheme.Foreground(0xFFF4DE).WithBold();
        button.FocusedLabelStyle = MusicDeckTheme.Foreground(0xFFF4DE).WithBold();
        button.SurfaceStyle = MusicDeckTheme.Background(0x34202B);
        button.FocusedSurfaceStyle = MusicDeckTheme.Background(0x39232E);
        button.PressedSurfaceStyle = MusicDeckTheme.Background(0x4A2D3A);
    }

    private static void ConfigureCard(StatsCard card, TesseraStyle valueStyle)
    {
        card.TitleStyle = MusicDeckTheme.DefaultTheme.Text.Secondary.WithBold();
        card.ValueStyle = valueStyle;
        card.KeyStyle = MusicDeckTheme.DefaultTheme.Text.Muted;
        card.BorderStyleText = MusicDeckTheme.DefaultTheme.Border.Strong;
    }
}

internal sealed record MusicDeckTickMessage : Message;
