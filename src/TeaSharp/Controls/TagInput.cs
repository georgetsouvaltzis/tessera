using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
using TeaSharp.Widgets;
namespace TeaSharp.Controls;
/// <summary>
/// Editable tag input control with separator-based commit behavior.
/// </summary>
public sealed class TagInput : Control
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
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    public Thickness Padding { get; set; }
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    public bool HasError { get; set; }
    public IReadOnlyList<string> Tags => _tags;
    public string InputValue => _input.Value;
    public int SelectedTagIndex => _selectedTagIndex;
    public string SelectedTag => _selectedTagIndex >= 0 && _selectedTagIndex < _tags.Count ? _tags[_selectedTagIndex] : string.Empty;
    public override bool IsFocused { get; set; }
    public override bool IsDisabled { get; set; }
    public override bool IsReadOnly { get; set; }
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
        var inside = content.Contains(pointer.X, pointer.Y) && pointer.Y == content.Y;
        if (!inside)
        {
            return pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press
                ? SetHoveredTagIndex(-1)
                : false;
        }
        var hovered = HitTagIndex(pointer.X, content);
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
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }
        RenderSingleLine(canvas, content);
    }
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var options = Options;
        var prefix = string.IsNullOrEmpty(options.TagPrefix) ? "[" : options.TagPrefix;
        var suffix = string.IsNullOrEmpty(options.TagSuffix) ? "]" : options.TagSuffix;
        var prefixWidth = ControlTextLayout.MeasureDisplayWidth(prefix);
        var suffixWidth = ControlTextLayout.MeasureDisplayWidth(suffix);
        var width = Math.Max(16, ControlTextLayout.MeasureDisplayWidth(FormatTitleText()) + 4);
        for (var index = 0; index < _tags.Count; index++)
        {
            var tagWidth = prefixWidth + ControlTextLayout.MeasureDisplayWidth(_tags[index]) + suffixWidth;
            width += tagWidth + 1;
        }
        width += 8;
        width += Padding.Horizontal;
        if (Border != BorderStyle.None)
        {
            width += 2;
        }
        var height = Padding.Vertical + 1;
        if (Border != BorderStyle.None)
        {
            height += 2;
        }
        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }
    private void RenderSingleLine(Canvas canvas, Rect content)
    {
        var options = Options;
        var prefix = string.IsNullOrEmpty(options.TagPrefix) ? "[" : options.TagPrefix;
        var suffix = string.IsNullOrEmpty(options.TagSuffix) ? "]" : options.TagSuffix;
        var prefixWidth = ControlTextLayout.MeasureDisplayWidth(prefix);
        var suffixWidth = ControlTextLayout.MeasureDisplayWidth(suffix);
        var x = content.X;
        var y = content.Y;
        var right = content.Right;
        for (var index = 0; index < _tags.Count && x < right; index++)
        {
            var tag = _tags[index];
            var tagWidth = prefixWidth + ControlTextLayout.MeasureDisplayWidth(tag) + suffixWidth;
            if (tagWidth <= 0 || x + tagWidth > right)
            {
                break;
            }
            var tagStyle = ResolveTagStyle(index);
            if (tagStyle.IsEmpty)
            {
                canvas.WriteText(x, y, prefix, right - x);
                x += prefixWidth;
                canvas.WriteText(x, y, tag, right - x);
                x += ControlTextLayout.MeasureDisplayWidth(tag);
                canvas.WriteText(x, y, suffix, right - x);
                x += suffixWidth;
            }
            else
            {
                var token = string.Concat(prefix, tag, suffix);
                canvas.WriteText(x, y, tagStyle.Render(token), right - x);
                x += tagWidth;
            }
            if (x < right)
            {
                canvas.Set(x, y, ' ');
                x++;
            }
        }
        var inputWidth = right - x;
        if (inputWidth <= 0)
        {
            return;
        }
        var frame = _input.BuildFrame(inputWidth);
        var inputStyle = frame.PlaceholderVisible ? PlaceholderTextStyle : ValueTextStyle;
        if (IsDisabled)
        {
            inputStyle = inputStyle.Merge(DisabledTagStyle);
        }
        if (HasError)
        {
            inputStyle = inputStyle.Merge(ErrorTagStyle);
        }
        canvas.WriteText(x, y, ApplyStyle(frame.Text, inputStyle), inputWidth);
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
    private int HitTagIndex(int pointerX, Rect content)
    {
        var options = Options;
        var prefix = string.IsNullOrEmpty(options.TagPrefix) ? "[" : options.TagPrefix;
        var suffix = string.IsNullOrEmpty(options.TagSuffix) ? "]" : options.TagSuffix;
        var prefixWidth = ControlTextLayout.MeasureDisplayWidth(prefix);
        var suffixWidth = ControlTextLayout.MeasureDisplayWidth(suffix);
        var x = content.X;
        for (var index = 0; index < _tags.Count; index++)
        {
            var width = prefixWidth + ControlTextLayout.MeasureDisplayWidth(_tags[index]) + suffixWidth;
            if (width <= 0)
            {
                continue;
            }
            if (pointerX >= x && pointerX < x + width)
            {
                return index;
            }
            x += width + 1;
            if (x >= content.Right)
            {
                break;
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
    private TeaStyle ResolveTagStyle(int tagIndex)
    {
        var style = TagStyle;
        if (tagIndex == _selectedTagIndex)
        {
            style = style.Merge(SelectedTagStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedTagStyle);
            }
        }
        if (tagIndex == _hoveredTagIndex)
        {
            style = style.Merge(HoveredTagStyle);
        }
        if (IsDisabled)
        {
            style = style.Merge(DisabledTagStyle);
        }
        if (HasError)
        {
            style = style.Merge(ErrorTagStyle);
        }
        return style;
    }
    private TeaStyle ResolveBorderStyleText()
    {
        var style = BorderStyleText;
        if (IsFocused)
        {
            style = style.Merge(FocusedBorderStyleText);
        }
        if (IsDisabled)
        {
            style = style.Merge(DisabledTagStyle);
        }
        if (HasError)
        {
            style = style.Merge(ErrorTagStyle);
        }
        return style;
    }
    private string RenderTitle()
    {
        return ApplyStyle(
            FormatTitleText(),
            IsFocused ? FocusedTitleStyle : TitleStyle);
    }
    private string FormatTitleText()
    {
        return string.IsNullOrEmpty(Title)
            ? string.Empty
            : IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker) ? $"{Title} {FocusMarker}" : Title;
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
