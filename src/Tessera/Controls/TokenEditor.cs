using Tessera.Components.Primitives;
using Tessera.Components.Primitives.Internal;
using Tessera.Controls.Internal;
using Tessera.Styles;
using Tessera.Widgets;

namespace Tessera.Controls;

/// <summary>
/// Represents an editable token-chip input control.
/// </summary>
public sealed partial class TokenEditor : Control
{
    private readonly List<TokenItem> _tokens = [];
    private readonly TextInputModel _input = new();
    private int _selectedTokenIndex = -1;
    private int _hoveredTokenIndex = -1;

    /// <summary>
    /// Occurs when selected token changes.
    /// </summary>
    public event EventHandler<TokenEditorSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Occurs when the token collection changes.
    /// </summary>
    public event EventHandler<TokenEditorTokensChangedEventArgs>? TokensChanged;

    /// <summary>
    /// Control title rendered on the frame.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Tokens";

    /// <summary>
    /// Marker appended to the title while focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Whether the focus marker is rendered while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Placeholder rendered when input is empty.
    /// </summary>
    public string Placeholder
    {
        get => _input.Placeholder;
        set => _input.Placeholder = value ?? string.Empty;
    }

    /// <summary>
    /// Style applied to title while not focused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style applied to title while focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Base style for token chips.
    /// </summary>
    public TesseraStyle TokenStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style merged into selected token chips.
    /// </summary>
    public TesseraStyle SelectedTokenStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style merged into selected token chips while focused.
    /// </summary>
    public TesseraStyle FocusedSelectedTokenStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style merged into hovered token chips.
    /// </summary>
    public TesseraStyle HoveredTokenStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style merged into disabled chips and disabled control state.
    /// </summary>
    public TesseraStyle DisabledTokenStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style applied to editable input text.
    /// </summary>
    public TesseraStyle ValueTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style applied to placeholder text.
    /// </summary>
    public TesseraStyle PlaceholderTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style applied to border glyphs while not focused.
    /// </summary>
    public TesseraStyle BorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Style merged into border glyphs while focused.
    /// </summary>
    public TesseraStyle FocusedBorderStyleText { get; set; } = TesseraStyle.Empty;

    /// <summary>
    /// Glyph set used for chip rendering.
    /// </summary>
    public TokenEditorGlyphSet Glyphs { get; set; } = TokenEditorGlyphSet.Default;

    /// <summary>
    /// Border style around the control.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Inner padding applied inside the frame.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Configured tokens.
    /// </summary>
    public IReadOnlyList<TokenItem> Tokens => _tokens;

    /// <summary>
    /// Selected token index, or <c>-1</c> when no token is selected.
    /// </summary>
    public int SelectedTokenIndex => _selectedTokenIndex;

    /// <summary>
    /// Selected token, if any.
    /// </summary>
    public TokenItem? SelectedToken => _selectedTokenIndex >= 0 && _selectedTokenIndex < _tokens.Count
        ? _tokens[_selectedTokenIndex]
        : null;

    /// <summary>
    /// Current input text value.
    /// </summary>
    public string InputValue => _input.Value;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Initializes a token editor.
    /// </summary>
    public TokenEditor()
    {
        Placeholder = "Add token...";
    }

    /// <summary>
    /// Replaces all tokens.
    /// </summary>
    /// <param name="tokens">Tokens to render.</param>
    public void SetTokens(IEnumerable<TokenItem> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        var previousTokens = SnapshotTokens();
        var previousIndex = _selectedTokenIndex;
        var previousToken = SelectedToken;

        _tokens.Clear();
        foreach (var token in tokens)
        {
            if (token is not null)
            {
                _tokens.Add(Clone(token));
            }
        }

        if (_tokens.Count == 0)
        {
            _selectedTokenIndex = -1;
            _hoveredTokenIndex = -1;
        }
        else
        {
            _selectedTokenIndex = previousIndex < 0
                ? 0
                : Math.Clamp(previousIndex, 0, _tokens.Count - 1);
            _hoveredTokenIndex = Math.Clamp(_hoveredTokenIndex, -1, _tokens.Count - 1);
        }

        RaiseTokensChangedIfNeeded(previousTokens);
        RaiseSelectionChangedIfNeeded(previousIndex, previousToken);
    }

    /// <summary>
    /// Adds a token from text input.
    /// </summary>
    /// <param name="value">Token text.</param>
    /// <returns><see langword="true" /> when token was added.</returns>
    public bool AddToken(string value)
    {
        var normalized = Trim(value.AsSpan());
        if (normalized.IsEmpty)
        {
            return false;
        }

        var previousTokens = SnapshotTokens();
        var previousIndex = _selectedTokenIndex;
        var previousToken = SelectedToken;
        _tokens.Add(new TokenItem(normalized.ToString()));
        if (_selectedTokenIndex < 0)
        {
            _selectedTokenIndex = 0;
        }

        RaiseTokensChangedIfNeeded(previousTokens);
        RaiseSelectionChangedIfNeeded(previousIndex, previousToken);
        return true;
    }

    /// <summary>
    /// Removes the selected token.
    /// </summary>
    /// <returns><see langword="true" /> when a token was removed.</returns>
    public bool RemoveSelectedToken()
    {
        if (_selectedTokenIndex < 0 || _selectedTokenIndex >= _tokens.Count)
        {
            return false;
        }

        var previousTokens = SnapshotTokens();
        var previousIndex = _selectedTokenIndex;
        var previousToken = SelectedToken;
        _tokens.RemoveAt(_selectedTokenIndex);

        if (_tokens.Count == 0)
        {
            _selectedTokenIndex = -1;
            _hoveredTokenIndex = -1;
        }
        else
        {
            _selectedTokenIndex = Math.Clamp(previousIndex, 0, _tokens.Count - 1);
            _hoveredTokenIndex = Math.Clamp(_hoveredTokenIndex, -1, _tokens.Count - 1);
        }

        RaiseTokensChangedIfNeeded(previousTokens);
        RaiseSelectionChangedIfNeeded(previousIndex, previousToken);
        return true;
    }

    /// <summary>
    /// Sets selected token index.
    /// </summary>
    /// <param name="index">Selection index, or <c>-1</c> to clear selection.</param>
    /// <returns><see langword="true" /> when selection changed.</returns>
    public bool SetSelectedTokenIndex(int index)
    {
        var normalized = _tokens.Count == 0
            ? -1
            : Math.Clamp(index, -1, _tokens.Count - 1);
        if (normalized == _selectedTokenIndex)
        {
            return false;
        }

        var previousIndex = _selectedTokenIndex;
        var previousToken = SelectedToken;
        _selectedTokenIndex = normalized;
        RaiseSelectionChangedIfNeeded(previousIndex, previousToken);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused)
        {
            return false;
        }

        if (message is KeyPressed key)
        {
            if (_input.Value.Length == 0)
            {
                if (key.Is(Key.Left))
                {
                    return MoveSelection(-1);
                }

                if (key.Is(Key.Right))
                {
                    return MoveSelection(1);
                }

                if (key.Is(Key.Delete) || key.Is(Key.Backspace))
                {
                    return RemoveSelectedToken();
                }
            }

            if (key.Is(Key.Enter))
            {
                return CommitInput() || true;
            }
        }

        var result = _input.Update(message);
        if (!result.Submitted)
        {
            return result.Changed;
        }

        return CommitInput() || true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        if (IsDisabled || IsReadOnly)
        {
            return false;
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return false;
        }

        var insideRow = content.Contains(pointer.X, pointer.Y) && pointer.Y == content.Y;
        if (!insideRow)
        {
            return pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press
                ? SetHoveredTokenIndex(-1)
                : false;
        }

        var hovered = HitTokenIndex(pointer.X, content);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredTokenIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            var changed = SetHoveredTokenIndex(hovered);
            changed |= SetSelectedTokenIndex(hovered);
            return changed || true;
        }

        return false;
    }

    private bool MoveSelection(int delta)
    {
        if (_tokens.Count == 0 || delta == 0)
        {
            return false;
        }

        var current = _selectedTokenIndex < 0 ? 0 : _selectedTokenIndex;
        var next = Math.Clamp(current + delta, 0, _tokens.Count - 1);
        return SetSelectedTokenIndex(next);
    }

    private bool CommitInput()
    {
        var changed = AddToken(_input.Value);
        _input.Clear();
        return changed;
    }

    private int HitTokenIndex(int pointerX, Rect content)
    {
        var glyphs = ResolveGlyphs();
        var markerSeparatorWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.MarkerSeparator);
        var tokenPrefixWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.TokenPrefix);
        var tokenSuffixWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.TokenSuffix);
        var selectedMarkerWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.SelectedMarker);
        var unselectedMarkerWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.UnselectedMarker);
        var tokenSeparatorWidth = ControlTextLayout.MeasureDisplayWidth(glyphs.TokenSeparator);

        var x = content.X;
        for (var index = 0; index < _tokens.Count; index++)
        {
            var markerWidth = index == _selectedTokenIndex ? selectedMarkerWidth : unselectedMarkerWidth;
            var valueWidth = ControlTextLayout.MeasureDisplayWidth(_tokens[index].Value);
            var width = markerWidth + markerSeparatorWidth + tokenPrefixWidth + valueWidth + tokenSuffixWidth;
            var separatorWidth = index < _tokens.Count - 1 ? tokenSeparatorWidth : 0;
            var right = x + width + separatorWidth;
            if (pointerX >= x && pointerX < right)
            {
                return index;
            }

            x = right;
            if (x >= content.Right)
            {
                break;
            }
        }

        return -1;
    }

    private bool SetHoveredTokenIndex(int index)
    {
        var normalized = _tokens.Count == 0
            ? -1
            : Math.Clamp(index, -1, _tokens.Count - 1);
        if (normalized == _hoveredTokenIndex)
        {
            return false;
        }

        _hoveredTokenIndex = normalized;
        return true;
    }

    private static TokenItem Clone(TokenItem token)
    {
        return new TokenItem(token.Value, token.IsDisabled);
    }

    private TokenItem[] SnapshotTokens()
    {
        var snapshot = new TokenItem[_tokens.Count];
        for (var index = 0; index < _tokens.Count; index++)
        {
            snapshot[index] = Clone(_tokens[index]);
        }

        return snapshot;
    }

    private void RaiseTokensChangedIfNeeded(IReadOnlyList<TokenItem> previousTokens)
    {
        if (AreTokensEqual(previousTokens, _tokens))
        {
            return;
        }

        TokensChanged?.Invoke(this, new TokenEditorTokensChangedEventArgs(previousTokens, _tokens));
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, TokenItem? previousToken)
    {
        var selected = SelectedToken;
        if (previousIndex == _selectedTokenIndex
            && string.Equals(previousToken?.Value, selected?.Value, StringComparison.Ordinal)
            && previousToken?.IsDisabled == selected?.IsDisabled)
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new TokenEditorSelectionChangedEventArgs(previousIndex, _selectedTokenIndex, previousToken, selected));
    }

    private static ReadOnlySpan<char> Trim(ReadOnlySpan<char> value)
    {
        var start = 0;
        var end = value.Length - 1;
        while (start <= end && char.IsWhiteSpace(value[start]))
        {
            start++;
        }

        while (end >= start && char.IsWhiteSpace(value[end]))
        {
            end--;
        }

        return end < start ? ReadOnlySpan<char>.Empty : value[start..(end + 1)];
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return style.IsEmpty || string.IsNullOrEmpty(text)
            ? text
            : style.Render(text);
    }

    private static bool AreTokensEqual(IReadOnlyList<TokenItem> previousTokens, IReadOnlyList<TokenItem> currentTokens)
    {
        if (previousTokens.Count != currentTokens.Count)
        {
            return false;
        }

        for (var index = 0; index < previousTokens.Count; index++)
        {
            if (!string.Equals(previousTokens[index].Value, currentTokens[index].Value, StringComparison.Ordinal)
                || previousTokens[index].IsDisabled != currentTokens[index].IsDisabled)
            {
                return false;
            }
        }

        return true;
    }
}
