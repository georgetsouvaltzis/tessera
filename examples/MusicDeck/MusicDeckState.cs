using TeaSharp.Controls;

namespace TeaSharp.Examples.MusicDeck;

internal sealed class MusicDeckState
{
    private readonly List<MusicTrack> _queue;
    private int _currentIndex;
    private int _selectedIndex;
    private int _positionSeconds;

    private MusicDeckState(List<MusicTrack> queue)
    {
        _queue = queue;
        _currentIndex = 0;
        _selectedIndex = 0;
        _positionSeconds = 92;
    }

    public static MusicDeckState CreateSeed()
    {
        return new MusicDeckState(
        [
            new MusicTrack("md-001", "Night Window", "Mina Vale", "Velvet Proof", "A late-room opener with tape hiss and warm plate reverb.", "04:26", 266, 92, "Am", 2024, "Studio A / ribbon chain", "warm low-end, close vocal, amber synth wash"),
            new MusicTrack("md-002", "Slow Signal", "Mina Vale", "Velvet Proof", "A pulse-led bridge that keeps the room leaning forward.", "05:11", 311, 96, "Cm", 2024, "Sidecar print / tube bus", "percussion tucked, chorus blooms after bar 16"),
            new MusicTrack("md-003", "Rose Static", "Lune Harbor", "Guest Cuts", "Guest vocal cameo with dry drums and soft brass ghosts.", "03:47", 227, 88, "F", 2023, "Live room / close mono", "intimate lead, wide ad-lib bed"),
            new MusicTrack("md-004", "Afterglow Index", "Mina Vale", "Velvet Proof", "Instrumental sweep for transition and room reset.", "06:02", 362, 82, "Dm", 2024, "Half-speed print", "strings breathe, kick barely kisses"),
            new MusicTrack("md-005", "Cedar Motel", "Vesper Choir", "Field Notes", "Dusty closer with spoken-note fragments and bowed bass.", "04:41", 281, 79, "Gm", 2022, "Hotel lounge overdub", "narrative outro, crowd noise tucked low"),
        ]);
    }

    public IReadOnlyList<MusicTrack> Queue => _queue;
    public MusicTrack CurrentTrack => _queue[_currentIndex];
    public MusicTrack SelectedTrack => _queue[_selectedIndex];
    public int CurrentIndex => _currentIndex;
    public int SelectedIndex => _selectedIndex;
    public bool IsPlaying { get; private set; } = true;
    public bool ShowingLinerNotes { get; private set; }
    public string DeckLabel => IsPlaying ? "Live take rolling" : "Needle lifted";
    public static string DeviceLabel => "PMC mains";
    public static string RoomLabel => "amber room";
    public string LastAction { get; private set; } = "settled into side A";

    public void Tick()
    {
        if (!IsPlaying)
        {
            return;
        }

        _positionSeconds++;
        if (_positionSeconds >= CurrentTrack.DurationSeconds)
        {
            Next();
        }
    }

    public void TogglePlayPause()
    {
        IsPlaying = !IsPlaying;
        LastAction = IsPlaying ? $"rolled {CurrentTrack.Title.ToLowerInvariant()}" : $"paused on {FormatTime(_positionSeconds)}";
    }

    public void Next()
    {
        _currentIndex = (_currentIndex + 1) % _queue.Count;
        _selectedIndex = _currentIndex;
        _positionSeconds = Math.Min(26, CurrentTrack.DurationSeconds / 8);
        IsPlaying = true;
        LastAction = $"cut to {CurrentTrack.Title.ToLowerInvariant()}";
    }

    public void Previous()
    {
        _currentIndex = (_currentIndex - 1 + _queue.Count) % _queue.Count;
        _selectedIndex = _currentIndex;
        _positionSeconds = Math.Min(18, CurrentTrack.DurationSeconds / 10);
        IsPlaying = true;
        LastAction = $"back to {CurrentTrack.Title.ToLowerInvariant()}";
    }

    public void MoveSelection(int delta)
    {
        _selectedIndex = Math.Clamp(_selectedIndex + delta, 0, _queue.Count - 1);
        LastAction = $"browsing {SelectedTrack.Title.ToLowerInvariant()}";
    }

    public void CueSelected()
    {
        _currentIndex = _selectedIndex;
        _positionSeconds = Math.Min(12, CurrentTrack.DurationSeconds / 12);
        IsPlaying = true;
        LastAction = $"cued {CurrentTrack.Title.ToLowerInvariant()}";
    }

    public void ToggleDetailMode()
    {
        ShowingLinerNotes = !ShowingLinerNotes;
        LastAction = ShowingLinerNotes ? "opened liner notes" : "returned to lyric sheet";
    }

    public double Progress => CurrentTrack.DurationSeconds <= 0 ? 0 : _positionSeconds / (double)CurrentTrack.DurationSeconds;
    public string ProgressText => $"{FormatTime(_positionSeconds)} / {FormatTime(CurrentTrack.DurationSeconds)}";
    public string RemainingText => $"-{FormatTime(Math.Max(0, CurrentTrack.DurationSeconds - _positionSeconds))}";

    public string BuildQueueSummary()
    {
        return $"{_queue.Count} cuts queued  •  {_queue.Sum(static track => track.DurationSeconds) / 60:00} min room";
    }

    public string BuildHeroSummary()
    {
        return $"{CurrentTrack.Artist}  •  {CurrentTrack.Album}  •  {CurrentTrack.Year}";
    }

    public string BuildMetaText()
    {
        return string.Join(
            '\n',
            $"Room      {RoomLabel}",
            $"Output    {DeviceLabel}",
            $"Move      {LastAction}",
            $"Note      {CurrentTrack.MoodTag}");
    }

    public string BuildLyricsOrNotes()
    {
        return ShowingLinerNotes
            ? CurrentTrack.BuildLinerNotes()
            : CurrentTrack.BuildLyricSheet();
    }

    public IReadOnlyList<StatItem> BuildDeckStats()
    {
        return
        [
            new StatItem("BPM", $"{CurrentTrack.Bpm}"),
            new StatItem("Key", CurrentTrack.Key),
            new StatItem("Scene", DeckLabel),
        ];
    }

    public IReadOnlyList<StatItem> BuildSessionStats()
    {
        return
        [
            new StatItem("Album", CurrentTrack.Album),
            new StatItem("Print", CurrentTrack.SessionStamp),
            new StatItem("Mood", CurrentTrack.MoodTag),
        ];
    }

    private static string FormatTime(int totalSeconds)
    {
        var minutes = Math.Max(0, totalSeconds) / 60;
        var seconds = Math.Max(0, totalSeconds) % 60;
        return $"{minutes:00}:{seconds:00}";
    }
}

internal sealed record MusicTrack(
    string Id,
    string Title,
    string Artist,
    string Album,
    string Summary,
    string DisplayDuration,
    int DurationSeconds,
    int Bpm,
    string Key,
    int Year,
    string SessionStamp,
    string MoodTag)
{
    public string BuildLyricSheet()
    {
        return string.Join(
            '\n',
            "lyric sheet",
            "",
            $"the room keeps {Title.ToLowerInvariant()} under a low amber lamp",
            "thin kick in the carpet, vocal close enough to hear the breath",
            "hold the chorus back a half beat, let the tape lean into the phrase",
            "",
            "leave the door cracked for the reverb tail",
            "keep the ending human, never too polished");
    }

    public string BuildLinerNotes()
    {
        return string.Join(
            '\n',
            "liner notes",
            "",
            Summary,
            $"tracked at {SessionStamp}",
            $"tempo {Bpm} bpm  •  key {Key}  •  year {Year}",
            "",
            $"mix mood: {MoodTag}",
            "editorial note: preserve grain, keep the vocal in the lamp glow");
    }
}
