using Tessera.Components.Primitives;
using Tessera.Widgets;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
/// Represents a single-line input with in-place auto-complete suggestions.
/// </summary>
/// <remarks>
/// Suggestions are filtered using case-insensitive substring matching against <see cref="Text" /> and ranked by earliest match position.
/// </remarks>
public sealed partial class AutocompleteInput : Control
{
    private readonly TextInputModel _input = new();
    private readonly List<string> _suggestions = [];
    private readonly List<int> _filteredSuggestionIndices = [];
    private readonly List<(int SourceIndex, int MatchIndex)> _matchBuffer = [];
    private AutocompleteInputGlyphSet _glyphs = AutocompleteInputGlyphSet.Default;
    private int _selectedSuggestionIndex = -1;
    private int _hoveredSuggestionIndex = -1;

    /// <summary>
    /// Occurs when a suggestion is committed.
    /// </summary>
    public event EventHandler<AutocompleteInputSuggestionCommittedEventArgs>? SuggestionCommitted;

    /// <summary>
    /// Gets or sets the control title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Autocomplete";

    /// <summary>
    /// Gets or sets marker appended to <see cref="Title" /> while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker" /> should be shown while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets placeholder text shown when <see cref="Text" /> is empty.
    /// </summary>
    public string Placeholder
    {
        get => _input.Placeholder;
        set => _input.Placeholder = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets current input text.
    /// </summary>
    public string Text
    {
        get => _input.Value;
        set => SetText(value);
    }

    /// <summary>
    /// Gets configured suggestion values.
    /// </summary>
    public IReadOnlyList<string> Suggestions => _suggestions;

    /// <summary>
    /// Gets selected suggestion index in the current filtered suggestion list, or <c>-1</c>.
    /// </summary>
    public int SelectedSuggestionIndex => _selectedSuggestionIndex;

    /// <summary>
    /// Gets selected suggestion text, or <see langword="null"/> when no suggestion is selected.
    /// </summary>
    public string? SelectedSuggestion => TryGetSuggestion(_selectedSuggestionIndex, out var suggestion, out _)
        ? suggestion
        : null;

    /// <summary>
    /// Gets style used for title text when not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style used for title text when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style used for input text.
    /// </summary>
    public TesseraStyle InputTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style used for placeholder text.
    /// </summary>
    public TesseraStyle PlaceholderTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style applied to popup rows before suggestion state styles are merged.
    /// </summary>
    public TesseraStyle PopupStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style used for non-selected suggestions.
    /// </summary>
    public TesseraStyle SuggestionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style merged for hovered suggestions.
    /// </summary>
    public TesseraStyle HoveredSuggestionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style merged for selected suggestions.
    /// </summary>
    public TesseraStyle SelectedSuggestionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style merged for selected suggestions while focused.
    /// </summary>
    public TesseraStyle FocusedSelectedSuggestionStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style merged into all rendered text while disabled.
    /// </summary>
    public TesseraStyle DisabledStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style applied to border glyphs when not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style applied to border glyphs when focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets style applied to commit hint marker.
    /// </summary>
    public TesseraStyle CommitMarkerStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Gets or sets control border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets maximum number of visible suggestion rows.
    /// </summary>
    public int MaxVisibleSuggestions { get; set; } = 5;

    /// <summary>
    /// Gets or sets glyphs used by selected suggestion and commit hint markers.
    /// </summary>
    public AutocompleteInputGlyphSet Glyphs
    {
        get => _glyphs;
        set => _glyphs = value;
    }

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces current input text.
    /// </summary>
    /// <param name="text">Text to set.</param>
    public void SetText(string text)
    {
        _input.SetValue(text ?? string.Empty);
        RefreshFilteredSuggestions();
    }

    /// <summary>
    /// Replaces source suggestion values.
    /// </summary>
    /// <param name="suggestions">Suggestions used by auto-complete matching.</param>
    public void SetSuggestions(IEnumerable<string> suggestions)
    {
        ArgumentNullException.ThrowIfNull(suggestions);

        _suggestions.Clear();
        foreach (var suggestion in suggestions)
        {
            if (suggestion is not null)
            {
                _suggestions.Add(suggestion);
            }
        }

        RefreshFilteredSuggestions();
    }

    /// <summary>
    /// Sets selected suggestion index within the current filtered suggestion list.
    /// </summary>
    /// <param name="index">Filtered suggestion index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SetSelectedSuggestionIndex(int index)
    {
        if (_filteredSuggestionIndices.Count == 0)
        {
            _selectedSuggestionIndex = -1;
            return false;
        }

        var clamped = Math.Clamp(index, 0, _filteredSuggestionIndices.Count - 1);
        if (_selectedSuggestionIndex == clamped)
        {
            return false;
        }

        _selectedSuggestionIndex = clamped;
        return true;
    }

    private bool CommitSelection()
    {
        if (!TryGetSuggestion(_selectedSuggestionIndex, out var suggestion, out var sourceIndex))
        {
            return false;
        }

        var previous = _input.Value;
        _input.SetValue(suggestion);
        RefreshFilteredSuggestions();
        SuggestionCommitted?.Invoke(this, new AutocompleteInputSuggestionCommittedEventArgs(suggestion, sourceIndex, previous));
        return true;
    }

    private bool MoveSelection(int delta)
    {
        if (_filteredSuggestionIndices.Count == 0)
        {
            return false;
        }

        var next = _selectedSuggestionIndex < 0
            ? 0
            : (_selectedSuggestionIndex + delta + _filteredSuggestionIndices.Count) % _filteredSuggestionIndices.Count;
        return SetSelectedSuggestionIndex(next);
    }

    private void RefreshFilteredSuggestions()
    {
        var previousSelectedSource = TryGetSuggestion(_selectedSuggestionIndex, out _, out var sourceIndex)
            ? sourceIndex
            : -1;

        _filteredSuggestionIndices.Clear();
        var text = _input.Value;
        if (string.IsNullOrEmpty(text))
        {
            for (var index = 0; index < _suggestions.Count; index++)
            {
                _filteredSuggestionIndices.Add(index);
            }
        }
        else
        {
            _matchBuffer.Clear();
            for (var index = 0; index < _suggestions.Count; index++)
            {
                var matchIndex = _suggestions[index].IndexOf(text, StringComparison.OrdinalIgnoreCase);
                if (matchIndex >= 0)
                {
                    _matchBuffer.Add((index, matchIndex));
                }
            }

            _matchBuffer.Sort(static (left, right) =>
            {
                var rank = left.MatchIndex.CompareTo(right.MatchIndex);
                return rank != 0 ? rank : left.SourceIndex.CompareTo(right.SourceIndex);
            });

            for (var index = 0; index < _matchBuffer.Count; index++)
            {
                _filteredSuggestionIndices.Add(_matchBuffer[index].SourceIndex);
            }
        }

        _hoveredSuggestionIndex = -1;
        _selectedSuggestionIndex = ResolvePreferredSelectedIndex(previousSelectedSource);
    }

    private int ResolvePreferredSelectedIndex(int previousSelectedSource)
    {
        if (_filteredSuggestionIndices.Count == 0)
        {
            return -1;
        }

        for (var index = 0; index < _filteredSuggestionIndices.Count; index++)
        {
            if (_filteredSuggestionIndices[index] == previousSelectedSource)
            {
                return index;
            }
        }

        return 0;
    }

    private bool IsPopupVisible => _filteredSuggestionIndices.Count > 0;

    private bool TryGetSuggestion(int filteredIndex, out string suggestion, out int sourceIndex)
    {
        if (filteredIndex >= 0 && filteredIndex < _filteredSuggestionIndices.Count)
        {
            sourceIndex = _filteredSuggestionIndices[filteredIndex];
            suggestion = _suggestions[sourceIndex];
            return true;
        }

        suggestion = string.Empty;
        sourceIndex = -1;
        return false;
    }
}
