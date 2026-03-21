namespace TeaSharp.Controls;

public sealed partial class LogTailPanel
{
    /// <summary>
    /// Replaces all log entries.
    /// </summary>
    /// <param name="entries">Entries to render.</param>
    public void SetEntries(IEnumerable<LogEntry> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        var previousIndex = _selectedIndex;
        var previousItem = SelectedEntry?.Message ?? string.Empty;
        _entries.Clear();
        _entryBodyCache.Clear();
        foreach (var entry in entries)
        {
            if (entry is null)
            {
                continue;
            }

            _entries.Add(
                new LogEntry(entry.Message, entry.Level, entry.Timestamp, entry.Source)
                {
                    IsMuted = entry.IsMuted,
                    HasError = entry.HasError,
                });
        }

        TrimToMaxEntries();
        NormalizeSelection();
        _entryCacheDirty = true;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Appends one pre-built log entry.
    /// </summary>
    /// <param name="entry">Entry to append.</param>
    public void Append(LogEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);
        var previousIndex = _selectedIndex;
        var previousItem = SelectedEntry?.Message ?? string.Empty;
        _entries.Add(
            new LogEntry(entry.Message, entry.Level, entry.Timestamp, entry.Source)
            {
                IsMuted = entry.IsMuted,
                HasError = entry.HasError,
            });

        _entryBodyCache.Add(string.Empty);
        TrimToMaxEntries();
        if (_entries.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
        }
        else if (AutoFollow)
        {
            _selectedIndex = _entries.Count - 1;
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _entries.Count - 1);
        }
        else
        {
            NormalizeSelection();
        }

        _entryCacheDirty = true;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Appends one log entry from primitive values.
    /// </summary>
    /// <param name="message">Message text.</param>
    /// <param name="level">Severity level.</param>
    /// <param name="timestamp">Optional timestamp.</param>
    /// <param name="source">Optional source label.</param>
    public void Append(string message, LogLevel level = LogLevel.Info, DateTimeOffset? timestamp = null, string? source = null)
    {
        Append(new LogEntry(message, level, timestamp, source));
    }

    /// <summary>
    /// Clears all entries.
    /// </summary>
    public void Clear()
    {
        var previousIndex = _selectedIndex;
        var previousItem = SelectedEntry?.Message ?? string.Empty;
        _entries.Clear();
        _entryBodyCache.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        _entryCacheDirty = false;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Sets selected row using bounds clamping.
    /// </summary>
    /// <param name="index">Requested index.</param>
    /// <returns><see langword="true"/> when selection changed; otherwise <see langword="false"/>.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_entries.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _entries.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousItem = SelectedEntry?.Message ?? string.Empty;
        _selectedIndex = clamped;
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _entries.Count - 1);
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    private void NormalizeSelection()
    {
        if (_entries.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
            return;
        }

        _selectedIndex = _selectedIndex < 0 ? _entries.Count - 1 : Math.Clamp(_selectedIndex, 0, _entries.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _entries.Count - 1);
    }

    private void TrimToMaxEntries()
    {
        var overflow = _entries.Count - MaxEntries;
        if (overflow <= 0)
        {
            return;
        }

        _entries.RemoveRange(0, overflow);
        _entryBodyCache.RemoveRange(0, overflow);
        _selectedIndex -= overflow;
        _hoveredIndex -= overflow;
        _selectedIndex = Math.Clamp(_selectedIndex, -1, _entries.Count - 1);
        _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _entries.Count - 1);
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, string previousItem)
    {
        if (previousIndex == _selectedIndex)
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new SelectionChangedEventArgs(
                previousIndex,
                _selectedIndex,
                previousItem,
                SelectedEntry?.Message ?? string.Empty));
    }
}
