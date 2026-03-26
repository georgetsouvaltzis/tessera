using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Styles;
using TeaSharp.Widgets;

namespace TeaSharp.Controls;

/// <summary>
/// Editable tag input control with separator-based commit behavior.
///
/// Programmatic mutation stays available through <see cref="SetTags(IEnumerable{string})"/>,
/// <see cref="AddTag(string)"/>, and <see cref="RemoveTagAt(int)"/>. The
/// <see cref="IsDisabled"/> and <see cref="IsReadOnly"/> guards only affect user interaction.
/// </summary>
public sealed partial class TagInput : Control
{
    private readonly List<string> _tags = [];
    private readonly TextInputModel _input = new();
    private int _selectedTagIndex = -1;
    private int _hoveredTagIndex = -1;

    /// <summary>
    /// Occurs when the committed tag collection changes.
    /// </summary>
    public event EventHandler<TagInputTagsChangedEventArgs>? TagsChanged;
    public TagInput()
    {
        Placeholder = "Add tag...";
    }
    public string Title { get; set => field = value ?? string.Empty; } = "Tags";
    public string FocusMarker { get; set => field = value ?? string.Empty; } = "*";
    public bool ShowFocusMarker { get; set; } = true;
    public string Placeholder { get => _input.Placeholder; set => _input.Placeholder = value ?? string.Empty; }
    public TagInputOptions Options { get; set; }
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle TagStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle SelectedTagStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedTagStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle HoveredTagStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle DisabledTagStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ErrorTagStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle ValueTextStyle { get; set; } = TeaStyle.Empty;
    public TeaStyle PlaceholderTextStyle { get; set; } = TeaStyle.Empty;
    /// <summary>
    /// Gets or sets the style used for the rendered caret indicator.
    /// </summary>
    public TeaStyle CaretStyle { get; set; } = TeaStyle.Empty;
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    public Thickness Padding { get; set; }
    /// <summary>
    /// Gets or sets the horizontal padding inserted inside each rendered tag chip.
    /// </summary>
    public int TagPadding { get; set; }

    /// <summary>
    /// Gets or sets the horizontal padding inserted around the input/placeholder text.
    /// </summary>
    public int InputPadding { get; set; }
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    public bool HasError { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether a caret indicator should be rendered while focused.
    /// </summary>
    public bool ShowCaret { get; set; } = true;

    /// <summary>
    /// Gets or sets the glyph used for the caret indicator.
    /// </summary>
    public string CaretGlyph { get; set => field = value ?? string.Empty; } = "|";
    public IReadOnlyList<string> Tags => _tags;
    public string InputValue => _input.Value;
    public int SelectedTagIndex => _selectedTagIndex;
    public string SelectedTag => _selectedTagIndex >= 0 && _selectedTagIndex < _tags.Count ? _tags[_selectedTagIndex] : string.Empty;
    public override bool IsFocused { get; set; }

    /// <summary>
    /// Gets or sets whether user interaction is ignored.
    /// </summary>
    /// <remarks>
    /// Programmatic mutation APIs such as <see cref="SetTags(IEnumerable{string})"/>,
    /// <see cref="AddTag(string)"/>, and <see cref="RemoveTagAt(int)"/> still work.
    /// </remarks>
    public override bool IsDisabled { get; set; }

    /// <summary>
    /// Gets or sets whether user interaction is read-only.
    /// </summary>
    /// <remarks>
    /// Programmatic mutation APIs such as <see cref="SetTags(IEnumerable{string})"/>,
    /// <see cref="AddTag(string)"/>, and <see cref="RemoveTagAt(int)"/> still work.
    /// </remarks>
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces the current tag collection programmatically.
    /// </summary>
    /// <param name="tags">The new tag values.</param>
    /// <remarks>
    /// This bypasses interaction guards and raises <see cref="TagsChanged"/> when the tag snapshot changes.
    /// </remarks>
    public void SetTags(IEnumerable<string> tags)
    {
        ArgumentNullException.ThrowIfNull(tags);
        var previousTags = SnapshotTags();
        _tags.Clear();
        foreach (var tag in tags)
        {
            _ = TryAddTagCore((tag ?? string.Empty).AsSpan());
        }
        if (_tags.Count == 0)
        {
            _selectedTagIndex = -1;
            _hoveredTagIndex = -1;
        }
        else
        {
            _selectedTagIndex = Math.Clamp(_selectedTagIndex, 0, _tags.Count - 1);
            _hoveredTagIndex = Math.Clamp(_hoveredTagIndex, -1, _tags.Count - 1);
        }

        RaiseTagsChangedIfNeeded(previousTags);
    }

    /// <summary>
    /// Adds a tag programmatically.
    /// </summary>
    /// <param name="tag">The tag value to add.</param>
    /// <returns><c>true</c> when the tag was added; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// This bypasses interaction guards and still respects duplicate and max-tag rules.
    /// </remarks>
    public bool AddTag(string tag)
    {
        var previousTags = SnapshotTags();
        var changed = TryAddTagCore((tag ?? string.Empty).AsSpan());
        if (changed)
        {
            RaiseTagsChangedIfNeeded(previousTags);
        }

        return changed;
    }

    /// <summary>
    /// Removes a tag programmatically by index.
    /// </summary>
    /// <param name="index">The zero-based tag index.</param>
    /// <returns><c>true</c> when a tag was removed; otherwise <c>false</c>.</returns>
    /// <remarks>
    /// This bypasses interaction guards.
    /// </remarks>
    public bool RemoveTagAt(int index)
    {
        if ((uint)index >= (uint)_tags.Count)
        {
            return false;
        }

        var previousTags = SnapshotTags();
        _tags.RemoveAt(index);
        if (_tags.Count == 0)
        {
            _selectedTagIndex = -1;
            _hoveredTagIndex = -1;
        }
        else
        {
            _selectedTagIndex = Math.Clamp(_selectedTagIndex, 0, _tags.Count - 1);
            _hoveredTagIndex = Math.Clamp(_hoveredTagIndex, -1, _tags.Count - 1);
        }

        RaiseTagsChangedIfNeeded(previousTags);
        return true;
    }
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
                    return MoveSelectedTag(-1);
                }
                if (key.Is(Key.Right))
                {
                    return MoveSelectedTag(1);
                }
                if (key.Is(Key.Backspace))
                {
                    return RemoveSelectedOrLastTag();
                }
                if (key.Is(Key.Delete))
                {
                    return RemoveSelectedTag();
                }
            }
            if (IsSeparatorCommitKey(key))
            {
                return CommitInput();
            }
            if (key.Is(Key.Enter))
            {
                return CommitInput() || true;
            }
        }
        var update = _input.Update(message);
        if (!update.Submitted)
        {
            return update.Changed;
        }
        return CommitInput() || true;
    }
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
        var inside = content.Contains(pointer.X, pointer.Y);
        if (!inside)
        {
            return pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press
                ? SetHoveredTagIndex(-1)
                : false;
        }
        var hovered = HitTagIndex(pointer.X, pointer.Y, content);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredTagIndex(hovered);
        }
        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            var changed = SetHoveredTagIndex(hovered);
            changed |= SetSelectedTagIndex(hovered);
            return changed || true;
        }
        return false;
    }
    private bool MoveSelectedTag(int delta)
    {
        if (_tags.Count == 0 || delta == 0)
        {
            return false;
        }
        var current = _selectedTagIndex < 0 ? 0 : _selectedTagIndex;
        var next = Math.Clamp(current + delta, 0, _tags.Count - 1);
        return SetSelectedTagIndex(next);
    }
    private bool RemoveSelectedOrLastTag()
    {
        if (_tags.Count == 0)
        {
            return false;
        }
        if (_selectedTagIndex >= 0)
        {
            return RemoveTagAt(_selectedTagIndex);
        }
        return RemoveTagAt(_tags.Count - 1);
    }
    private bool RemoveSelectedTag() => _selectedTagIndex >= 0 && _selectedTagIndex < _tags.Count && RemoveTagAt(_selectedTagIndex);
    private bool CommitInput()
    {
        var options = Options;
        var value = _input.Value.AsSpan();
        if (value.IsEmpty)
        {
            return false;
        }

        var previousTags = SnapshotTags();
        var changed = false;
        var separator = options.Separator;
        if (separator == '\0')
        {
            changed = TryAddTagCore(value);
        }
        else
        {
            var start = 0;
            for (var index = 0; index <= value.Length; index++)
            {
                if (index < value.Length && value[index] != separator)
                {
                    continue;
                }
                changed |= TryAddTagCore(value[start..index]);
                start = index + 1;
            }
        }
        _input.Clear();
        if (changed)
        {
            RaiseTagsChangedIfNeeded(previousTags);
        }

        return changed;
    }
    private bool TryAddTagCore(ReadOnlySpan<char> rawTag)
    {
        var normalized = Trim(rawTag);
        if (normalized.IsEmpty)
        {
            return false;
        }
        var options = Options;
        if (options.MaxTags > 0 && _tags.Count >= options.MaxTags)
        {
            return false;
        }
        var value = normalized.ToString();
        if (!options.AllowDuplicates && ContainsTag(value))
        {
            return false;
        }
        _tags.Add(value);
        if (_selectedTagIndex < 0)
        {
            _selectedTagIndex = _tags.Count - 1;
        }
        return true;
    }
    private bool ContainsTag(string value)
    {
        var comparison = Options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        for (var index = 0; index < _tags.Count; index++)
        {
            if (string.Equals(_tags[index], value, comparison))
            {
                return true;
            }
        }
        return false;
    }
    private bool IsSeparatorCommitKey(KeyPressed key)
    {
        if (key.Key != Key.Character || key.Text.Length != 1)
        {
            return false;
        }
        if (key.Modifiers.HasFlag(ModifierKeys.Ctrl)
            || key.Modifiers.HasFlag(ModifierKeys.Alt)
            || key.Modifiers.HasFlag(ModifierKeys.Meta))
        {
            return false;
        }
        var separator = Options.Separator;
        return separator != '\0' && key.Text[0] == separator;
    }
    private int HitTagIndex(int pointerX, int pointerY, Rect content)
    {
        var relativeY = pointerY - content.Y;
        if (relativeY < 0)
        {
            return -1;
        }

        var layout = BuildFlowLayout(content.Width);
        for (var index = 0; index < layout.Tags.Count; index++)
        {
            var placement = layout.Tags[index];
            if (placement.Y != relativeY)
            {
                continue;
            }

            var left = content.X + placement.X;
            var right = left + placement.Width;
            if (pointerX >= left && pointerX < right)
            {
                return placement.Index;
            }
        }

        return -1;
    }
    private bool SetSelectedTagIndex(int index)
    {
        var normalized = _tags.Count == 0
            ? -1
            : Math.Clamp(index, -1, _tags.Count - 1);
        if (normalized == _selectedTagIndex)
        {
            return false;
        }
        _selectedTagIndex = normalized;
        return true;
    }
    private bool SetHoveredTagIndex(int index)
    {
        var normalized = _tags.Count == 0
            ? -1
            : Math.Clamp(index, -1, _tags.Count - 1);
        if (normalized == _hoveredTagIndex)
        {
            return false;
        }
        _hoveredTagIndex = normalized;
        return true;
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
    private static string ApplyStyle(string text, TeaStyle style)
    {
        return style.IsEmpty || string.IsNullOrEmpty(text)
            ? text
            : style.Render(text);
    }

    private string[] SnapshotTags()
    {
        return [.. _tags];
    }

    private void RaiseTagsChangedIfNeeded(IReadOnlyList<string> previousTags)
    {
        if (AreTagsEqual(previousTags, _tags))
        {
            return;
        }

        TagsChanged?.Invoke(this, new TagInputTagsChangedEventArgs(previousTags, _tags));
    }

    private static bool AreTagsEqual(IReadOnlyList<string> previousTags, IReadOnlyList<string> currentTags)
    {
        if (previousTags.Count != currentTags.Count)
        {
            return false;
        }

        for (var index = 0; index < previousTags.Count; index++)
        {
            if (!string.Equals(previousTags[index], currentTags[index], StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
