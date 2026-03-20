using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Controls;

/// <summary>
/// Represents a selectable summary list of validation issues.
/// </summary>
public sealed class ValidationSummary : Control
{
    private readonly List<ValidationIssue> _issues = [];
    private int _selectedIndex = -1;
    private int _hoveredIndex = -1;

    /// <summary>
    /// Occurs when <see cref="SelectedIndex" /> changes.
    /// </summary>
    public event EventHandler<ValidationSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Gets or sets the summary title.
    /// </summary>
    public string Title
    {
        get;
        set => field = value ?? string.Empty;
    } = "Validation";

    /// <summary>
    /// Gets or sets the marker shown in the title when focused.
    /// </summary>
    public string FocusMarker
    {
        get;
        set => field = value ?? string.Empty;
    } = "*";

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="FocusMarker" /> is shown while focused.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    /// Gets or sets the title style used when not focused.
    /// </summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the title style used when focused.
    /// </summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when not focused.
    /// </summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the style applied to border glyphs when focused.
    /// </summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets the list border style.
    /// </summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;

    /// <summary>
    /// Gets or sets inner padding for the issue list.
    /// </summary>
    public Thickness Padding { get; set; }

    /// <summary>
    /// Gets or sets style used for rows before state/severity styles are merged.
    /// </summary>
    public TeaStyle DefaultIssueStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into hovered rows.
    /// </summary>
    public TeaStyle HoveredIssueStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows.
    /// </summary>
    public TeaStyle SelectedIssueStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into selected rows while the control is focused.
    /// </summary>
    public TeaStyle FocusedIssueStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rows when the control is disabled.
    /// </summary>
    public TeaStyle DisabledIssueStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into informational rows.
    /// </summary>
    public TeaStyle InfoSeverityStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into informational rows.
    /// </summary>
    /// <remarks>
    /// Alias for <see cref="InfoSeverityStyle"/> retained for style extension compatibility.
    /// </remarks>
    public TeaStyle InfoIssueStyle
    {
        get => InfoSeverityStyle;
        set => InfoSeverityStyle = value;
    }

    /// <summary>
    /// Gets or sets style merged into warning rows.
    /// </summary>
    public TeaStyle WarningSeverityStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into warning rows.
    /// </summary>
    /// <remarks>
    /// Alias for <see cref="WarningSeverityStyle"/> retained for style extension compatibility.
    /// </remarks>
    public TeaStyle WarningIssueStyle
    {
        get => WarningSeverityStyle;
        set => WarningSeverityStyle = value;
    }

    /// <summary>
    /// Gets or sets style merged into error rows.
    /// </summary>
    public TeaStyle ErrorSeverityStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into error rows.
    /// </summary>
    /// <remarks>
    /// Alias for <see cref="ErrorSeverityStyle"/> retained for style extension compatibility.
    /// </remarks>
    public TeaStyle ErrorIssueStyle
    {
        get => ErrorSeverityStyle;
        set => ErrorSeverityStyle = value;
    }

    /// <summary>
    /// Gets or sets style used for empty-state text.
    /// </summary>
    public TeaStyle EmptyStyle { get; set; } = TeaStyle.Empty;

    /// <summary>
    /// Gets or sets style merged into rows when disabled.
    /// </summary>
    /// <remarks>
    /// Alias for <see cref="DisabledIssueStyle"/> retained for style extension compatibility.
    /// </remarks>
    public TeaStyle DisabledStyle
    {
        get => DisabledIssueStyle;
        set => DisabledIssueStyle = value;
    }

    /// <summary>
    /// Gets or sets the text rendered when no issues are available.
    /// </summary>
    public string EmptyText
    {
        get;
        set => field = value ?? string.Empty;
    } = "(no issues)";

    /// <summary>
    /// Gets current issues.
    /// </summary>
    public IReadOnlyList<ValidationIssue> Issues => _issues;

    /// <summary>
    /// Gets the selected issue index, or <c>-1</c> when there are no issues.
    /// </summary>
    public int SelectedIndex => _selectedIndex;

    /// <summary>
    /// Gets the currently selected issue, if any.
    /// </summary>
    public ValidationIssue? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _issues.Count
        ? _issues[_selectedIndex]
        : null;

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    /// Replaces all issues shown by the summary.
    /// </summary>
    /// <param name="issues">The issues to render.</param>
    public void SetIssues(IEnumerable<ValidationIssue> issues)
    {
        ArgumentNullException.ThrowIfNull(issues);
        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;

        _issues.Clear();
        foreach (var issue in issues)
        {
            if (issue is not null)
            {
                _issues.Add(issue);
            }
        }

        if (_issues.Count == 0)
        {
            _selectedIndex = -1;
            _hoveredIndex = -1;
        }
        else
        {
            var seed = _selectedIndex < 0 ? 0 : _selectedIndex;
            _selectedIndex = Math.Clamp(seed, 0, _issues.Count - 1);
            _hoveredIndex = Math.Clamp(_hoveredIndex, -1, _issues.Count - 1);
        }

        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Clears all issues from the summary.
    /// </summary>
    public void ClearIssues()
    {
        if (_issues.Count == 0 && _selectedIndex == -1)
        {
            return;
        }

        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _issues.Clear();
        _selectedIndex = -1;
        _hoveredIndex = -1;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
    }

    /// <summary>
    /// Sets the selected issue index using bounds clamping.
    /// </summary>
    /// <param name="index">The requested index.</param>
    /// <returns><see langword="true" /> when selection changed; otherwise <see langword="false" />.</returns>
    public bool SetSelectedIndex(int index)
    {
        if (_issues.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _issues.Count - 1);
        if (clamped == _selectedIndex)
        {
            return false;
        }

        var previousIndex = _selectedIndex;
        var previousItem = SelectedItem;
        _selectedIndex = clamped;
        RaiseSelectionChangedIfNeeded(previousIndex, previousItem);
        return true;
    }

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _issues.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Down) || key.IsCharacter('j'))
        {
            return SetSelectedIndex(_selectedIndex + 1);
        }

        if (key.Is(Key.Up) || key.IsCharacter('k'))
        {
            return SetSelectedIndex(_selectedIndex - 1);
        }

        if (key.Is(Key.Home))
        {
            return SetSelectedIndex(0);
        }

        if (key.Is(Key.End))
        {
            return SetSelectedIndex(_issues.Count - 1);
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' '))
        {
            if (_hoveredIndex >= 0)
            {
                _ = SetSelectedIndex(_hoveredIndex);
            }

            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Handle(Message message, Rect bounds)
    {
        if (IsDisabled || IsReadOnly || message is not PointerInput pointer || bounds.IsEmpty)
        {
            return Handle(message);
        }

        var content = FrameLayout.ResolveContentRect(bounds, Border, Padding);
        if (content.IsEmpty)
        {
            return Handle(message);
        }

        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside)
        {
            if (pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
            {
                changed |= SetHoveredIndex(-1);
            }

            if (pointer.Kind != PointerEventKind.Wheel)
            {
                return changed;
            }
        }

        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown)
            {
                return SetSelectedIndex(_selectedIndex + 1) || changed;
            }

            if (pointer.Button == PointerButton.WheelUp)
            {
                return SetSelectedIndex(_selectedIndex - 1) || changed;
            }

            return changed;
        }

        if (!inside)
        {
            return changed;
        }

        var hovered = ResolveIndexFromPointer(content, pointer.Y);
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }

        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left)
        {
            RequestFocus();
            changed |= SetHoveredIndex(hovered);
            if (hovered >= 0)
            {
                changed |= SetSelectedIndex(hovered);
            }

            return changed;
        }

        return changed;
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty)
        {
            return;
        }

        var title = Border == BorderStyle.None ? null : RenderTitle();
        var content = FrameLayout.DrawFrameAndResolveContent(canvas, clipped, title, Border, Padding, ResolveBorderStyleText());
        if (content.IsEmpty)
        {
            return;
        }

        if (_issues.Count == 0)
        {
            canvas.WriteText(content.X, content.Y, ApplyStyle(EmptyText, ResolveEmptyStyle()), content.Width);
            return;
        }

        var start = ComputeWindowStart(content.Height);
        var end = Math.Min(_issues.Count, start + content.Height);
        for (var row = 0; row < end - start; row++)
        {
            var index = start + row;
            var issue = _issues[index];
            var selected = index == _selectedIndex;
            var hovered = index == _hoveredIndex;
            var marker = selected ? ">" : " ";
            var line = $"{marker} [{ResolveSeverityMarker(issue.Severity)}] {ResolveIssueText(issue)}";
            canvas.WriteText(content.X, content.Y + row, ApplyStyle(line, ResolveRowStyle(issue, selected, hovered)), content.Width);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = Math.Max(12, ControlTextLayout.MeasureDisplayWidth(Title) + 4);
        for (var index = 0; index < _issues.Count; index++)
        {
            var rowWidth = 6 + ControlTextLayout.MeasureDisplayWidth(ResolveIssueText(_issues[index]));
            width = Math.Max(width, rowWidth);
        }

        var height = Math.Max(1, _issues.Count);
        width += Padding.Horizontal;
        height += Padding.Vertical;

        if (Border != BorderStyle.None)
        {
            width += 2;
            height += 2;
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(height, 0, availableBounds.Height));
    }

    private int ComputeWindowStart(int viewportHeight)
    {
        if (viewportHeight <= 0 || _issues.Count == 0)
        {
            return 0;
        }

        var anchor = _selectedIndex < 0 ? 0 : _selectedIndex;
        return Math.Clamp(anchor - (viewportHeight / 2), 0, Math.Max(0, _issues.Count - viewportHeight));
    }

    private int ResolveIndexFromPointer(Rect content, int pointerY)
    {
        if (pointerY < content.Y || pointerY >= content.Bottom)
        {
            return -1;
        }

        var row = pointerY - content.Y;
        var index = ComputeWindowStart(content.Height) + row;
        return index >= 0 && index < _issues.Count ? index : -1;
    }

    private bool SetHoveredIndex(int index)
    {
        if (_hoveredIndex == index)
        {
            return false;
        }

        _hoveredIndex = index;
        return true;
    }

    private void RaiseSelectionChangedIfNeeded(int previousIndex, ValidationIssue? previousItem)
    {
        if (previousIndex == _selectedIndex && ReferenceEquals(previousItem, SelectedItem))
        {
            return;
        }

        SelectionChanged?.Invoke(
            this,
            new ValidationSelectionChangedEventArgs(previousIndex, _selectedIndex, previousItem, SelectedItem));
    }

    private string RenderTitle()
    {
        var title = IsFocused && ShowFocusMarker && FocusMarker.Length > 0
            ? $"{Title} {FocusMarker}"
            : Title;
        return ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle);
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
            style = style.Merge(DisabledIssueStyle);
        }

        return style;
    }

    private TeaStyle ResolveRowStyle(ValidationIssue issue, bool selected, bool hovered)
    {
        var style = DefaultIssueStyle.Merge(ResolveSeverityStyle(issue.Severity));
        if (hovered)
        {
            style = style.Merge(HoveredIssueStyle);
        }

        if (selected)
        {
            style = style.Merge(SelectedIssueStyle);
            if (IsFocused)
            {
                style = style.Merge(FocusedIssueStyle);
            }
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledIssueStyle);
        }

        return style;
    }

    private TeaStyle ResolveSeverityStyle(ValidationSeverity severity)
    {
        return severity switch
        {
            ValidationSeverity.Info => InfoSeverityStyle,
            ValidationSeverity.Warning => WarningSeverityStyle,
            _ => ErrorSeverityStyle,
        };
    }

    private TeaStyle ResolveEmptyStyle()
    {
        var style = EmptyStyle.IsEmpty ? DefaultIssueStyle : EmptyStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledIssueStyle);
        }

        return style;
    }

    private static char ResolveSeverityMarker(ValidationSeverity severity)
    {
        return severity switch
        {
            ValidationSeverity.Info => 'I',
            ValidationSeverity.Warning => 'W',
            _ => 'E',
        };
    }

    private static string ResolveIssueText(ValidationIssue issue)
    {
        var message = issue.Message ?? string.Empty;
        if (string.IsNullOrWhiteSpace(issue.Field))
        {
            return message;
        }

        return $"{issue.Field}: {message}";
    }

    private static string ApplyStyle(string text, TeaStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }
}
