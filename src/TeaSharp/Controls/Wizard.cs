using TeaSharp.Components.Primitives;
using TeaSharp.Components.Primitives.Internal;
using TeaSharp.Controls.Internal;
using TeaSharp.Layout;
using TeaSharp.Styles;
namespace TeaSharp.Controls;
/// <summary>
/// Represents a selectable multi-step wizard flow control.
/// </summary>
public sealed class Wizard : Control
{
    private readonly List<WizardStep> _steps = [];
    private int _currentIndex = -1;
    private int _hoveredIndex = -1;
    private int _scrollOffset;
    private int _lastViewportRows = 8;
    /// <summary>
    /// Occurs when selection changes.
    /// Canonical event for selection transition handling.
    /// </summary>
    public event EventHandler<WizardStepChangedEventArgs>? SelectionChanged;
    /// <summary>
    /// Occurs when <see cref="CurrentIndex"/> changes.
    /// Compatibility alias for <see cref="SelectionChanged"/>.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Advanced)]
    public event EventHandler<WizardStepChangedEventArgs>? StepChanged;
    /// <summary>Gets or sets control title.</summary>
    public string Title { get; set; } = "Wizard";
    /// <summary>Gets or sets marker appended to title while focused.</summary>
    public string FocusMarker { get; set; } = "*";
    /// <summary>Gets or sets whether <see cref="FocusMarker"/> is rendered while focused.</summary>
    public bool ShowFocusMarker { get; set; } = true;
    /// <summary>Gets or sets text shown when no steps exist.</summary>
    public string EmptyText { get; set; } = "(no steps)";
    /// <summary>Gets or sets border style.</summary>
    public BorderStyle Border { get; set; } = BorderStyle.SingleLine;
    /// <summary>Gets or sets inner padding.</summary>
    public Thickness Padding { get; set; }
    /// <summary>Gets or sets whether row numbering is rendered.</summary>
    public bool ShowStepNumbers { get; set; } = true;
    /// <summary>Gets or sets marker rendered for active step.</summary>
    public string ActiveMarker { get; set; } = ">";
    /// <summary>Gets or sets marker rendered for completed step.</summary>
    public string CompletedMarker { get; set; } = "✓";
    /// <summary>Gets or sets marker rendered for pending step.</summary>
    public string PendingMarker { get; set; } = "·";
    /// <summary>Gets or sets marker rendered for disabled step.</summary>
    public string DisabledMarker { get; set; } = "x";
    /// <summary>Gets or sets title style while unfocused.</summary>
    public TeaStyle TitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets title style while focused.</summary>
    public TeaStyle FocusedTitleStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets border style while unfocused.</summary>
    public TeaStyle BorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets border style while focused.</summary>
    public TeaStyle FocusedBorderStyleText { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets base step style.</summary>
    public TeaStyle StepStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets active-step style.</summary>
    public TeaStyle ActiveStepStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets active-step style while focused.</summary>
    public TeaStyle FocusedActiveStepStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets completed-step style.</summary>
    public TeaStyle CompletedStepStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets pending-step style.</summary>
    public TeaStyle PendingStepStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets hovered-step style.</summary>
    public TeaStyle HoveredStepStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets disabled-step style.</summary>
    public TeaStyle DisabledStepStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets or sets empty-state style.</summary>
    public TeaStyle EmptyStyle { get; set; } = TeaStyle.Empty;
    /// <summary>Gets configured steps.</summary>
    public IReadOnlyList<WizardStep> Steps => _steps;
    /// <summary>Gets current step index, or <c>-1</c> when none.</summary>
    public int CurrentIndex => _currentIndex;
    /// <summary>Gets selected step index, or <c>-1</c> when none. Canonical property for selection access.</summary>
    public int SelectedIndex => CurrentIndex;
    /// <summary>Gets current step, if any.</summary>
    public WizardStep? CurrentStep => _currentIndex >= 0 && _currentIndex < _steps.Count ? _steps[_currentIndex] : null;
    /// <summary>Gets selected step, if any. Canonical property for selection access.</summary>
    public WizardStep? SelectedStep => CurrentStep;
    /// <inheritdoc />
    public override bool IsFocused { get; set; }
    /// <inheritdoc />
    public override bool IsDisabled { get; set; }
    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }
    /// <summary>
    /// Replaces all configured steps.
    /// </summary>
    /// <param name="steps">Step definitions to render.</param>
    public void SetSteps(IEnumerable<WizardStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        var previousIndex = _currentIndex;
        var previousStep = CurrentStep;
        var previousId = previousStep?.Id;
        _steps.Clear();
        foreach (var step in steps)
        {
            if (step is null)
            {
                continue;
            }
            _steps.Add(new WizardStep(step.Id, step.Title, step.Description, step.IsCompleted, step.IsDisabled));
        }
        _currentIndex = ResolveInitialCurrentIndex(previousId);
        _hoveredIndex = -1;
        _scrollOffset = 0;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseStepChangedIfNeeded(previousIndex, previousStep);
    }
    /// <summary>
    /// Replaces all steps using label text as both id and title.
    /// </summary>
    /// <param name="stepTitles">Step title sequence.</param>
    public void SetSteps(IEnumerable<string> stepTitles)
    {
        ArgumentNullException.ThrowIfNull(stepTitles);
        SetSteps(stepTitles.Select(static value => new WizardStep(value ?? string.Empty, value ?? string.Empty)));
    }
    /// <summary>Selects next enabled step.</summary>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool NextStep() => MoveCurrent(+1);
    /// <summary>Selects previous enabled step.</summary>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool PreviousStep() => MoveCurrent(-1);
    /// <summary>
    /// Selects a specific step by index.
    /// </summary>
    /// <param name="index">Requested step index.</param>
    /// <returns><see langword="true"/> when selection changed.</returns>
    public bool SelectStep(int index)
    {
        if (_steps.Count == 0)
        {
            return false;
        }
        var clamped = Math.Clamp(index, 0, _steps.Count - 1);
        if (_steps[clamped].IsDisabled && !TryFindEnabledFrom(clamped, +1, out clamped) && !TryFindEnabledFrom(clamped, -1, out clamped))
        {
            return false;
        }
        if (clamped == _currentIndex)
        {
            return false;
        }
        var previousIndex = _currentIndex;
        var previousStep = CurrentStep;
        _currentIndex = clamped;
        EnsureSelectionVisible(_lastViewportRows);
        RaiseStepChangedIfNeeded(previousIndex, previousStep);
        return true;
    }
    /// <summary>
    /// Sets completion state for a step.
    /// </summary>
    /// <param name="index">Step index.</param>
    /// <param name="isCompleted">Completion state value.</param>
    /// <returns><see langword="true"/> when state changed.</returns>
    public bool SetStepCompleted(int index, bool isCompleted = true)
    {
        if (index < 0 || index >= _steps.Count)
        {
            return false;
        }
        var step = _steps[index];
        if (step.IsCompleted == isCompleted)
        {
            return false;
        }
        step.IsCompleted = isCompleted;
        return true;
    }
    /// <summary>
    /// Marks current step as completed.
    /// </summary>
    /// <returns><see langword="true"/> when state changed.</returns>
    public bool CompleteCurrentStep() => SetStepCompleted(_currentIndex, isCompleted: true);
    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _steps.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }
        if (key.Is(Key.Down) || key.Is(Key.Right) || key.IsCharacter('j')) return NextStep();
        if (key.Is(Key.Up) || key.Is(Key.Left) || key.IsCharacter('k')) return PreviousStep();
        if (key.Is(Key.Home)) return SelectEdge(selectLast: false);
        if (key.Is(Key.End)) return SelectEdge(selectLast: true);
        if (key.Is(Key.Enter) || key.IsCharacter(' ') || key.IsCharacter('c')) return CompleteCurrentStep();
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
        var stepsTop = content.Y;
        var hasTitle = !string.IsNullOrWhiteSpace(Title);
        if (hasTitle)
        {
            stepsTop++;
        }
        var stepsHeight = Math.Max(0, content.Bottom - stepsTop);
        _lastViewportRows = Math.Max(1, stepsHeight);
        var inside = content.Contains(pointer.X, pointer.Y);
        var changed = false;
        if (!inside && pointer.Kind is PointerEventKind.Motion or PointerEventKind.Press)
        {
            changed = SetHoveredIndex(-1);
        }
        if (!inside || _steps.Count == 0 || pointer.Y < stepsTop || stepsHeight <= 0)
        {
            return changed || Handle(message);
        }
        if (pointer.Kind == PointerEventKind.Wheel)
        {
            if (pointer.Button == PointerButton.WheelDown) return NextStep() || changed;
            if (pointer.Button == PointerButton.WheelUp) return PreviousStep() || changed;
        }
        EnsureSelectionVisible(stepsHeight);
        var hovered = _scrollOffset + (pointer.Y - stepsTop);
        if (hovered < 0 || hovered >= _steps.Count)
        {
            hovered = -1;
        }
        if (pointer.Kind == PointerEventKind.Motion)
        {
            return SetHoveredIndex(hovered);
        }
        if (pointer.Kind == PointerEventKind.Press && pointer.Button == PointerButton.Left && hovered >= 0)
        {
            RequestFocus();
            changed |= SetHoveredIndex(hovered);
            changed |= SelectStep(hovered);
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
        var content = FrameLayout.DrawFrameAndResolveContent(
            canvas,
            clipped,
            Border == BorderStyle.None ? null : RenderTitle(),
            Border,
            Padding,
            ResolveBorderStyle());
        if (content.IsEmpty)
        {
            return;
        }
        var y = content.Y;
        if (!string.IsNullOrWhiteSpace(Title))
        {
            WriteStyledText(canvas, content.X, y, RenderTitleText(), ResolveTitleStyle(), content.Width);
            y++;
        }
        var stepsHeight = Math.Max(0, content.Bottom - y);
        _lastViewportRows = Math.Max(1, stepsHeight);
        if (_steps.Count == 0 || stepsHeight <= 0)
        {
            if (stepsHeight > 0)
            {
                WriteStyledText(canvas, content.X, y, EmptyText, ResolveEmptyStyle(), content.Width);
            }
            return;
        }
        EnsureSelectionVisible(stepsHeight);
        var visible = Math.Min(stepsHeight, _steps.Count - _scrollOffset);
        for (var row = 0; row < visible; row++)
        {
            var index = _scrollOffset + row;
            var line = BuildStepLine(index, _steps[index]);
            WriteStyledText(canvas, content.X, y + row, line, ResolveStepStyle(index), content.Width);
        }
    }
    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 20;
        for (var index = 0; index < _steps.Count; index++)
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(BuildStepLine(index, _steps[index])));
        }
        if (!string.IsNullOrWhiteSpace(Title))
        {
            width = Math.Max(width, ControlTextLayout.MeasureDisplayWidth(RenderTitleText()));
        }
        width += Padding.Horizontal + (Border == BorderStyle.None ? 0 : 2);
        var height = Math.Max(1, _steps.Count) + (string.IsNullOrWhiteSpace(Title) ? 0 : 1) + Padding.Vertical + (Border == BorderStyle.None ? 0 : 2);
        return new LayoutMeasurement(Math.Clamp(width, 0, availableBounds.Width), Math.Clamp(height, 0, availableBounds.Height));
    }
    private int ResolveInitialCurrentIndex(string? previousId)
    {
        if (_steps.Count == 0)
        {
            return -1;
        }
        if (!string.IsNullOrWhiteSpace(previousId))
        {
            var index = _steps.FindIndex(step => string.Equals(step.Id, previousId, StringComparison.Ordinal));
            if (index >= 0 && !_steps[index].IsDisabled)
            {
                return index;
            }
        }
        if (TryFindEnabledFrom(0, +1, out var firstEnabled))
        {
            return firstEnabled;
        }
        return -1;
    }
    private bool MoveCurrent(int direction)
    {
        if (_steps.Count == 0)
        {
            return false;
        }
        var origin = _currentIndex >= 0 ? _currentIndex : (direction > 0 ? -1 : _steps.Count);
        return TryFindEnabledFrom(origin + direction, direction, out var target) && SelectStep(target);
    }
    private bool SelectEdge(bool selectLast)
    {
        if (_steps.Count == 0)
        {
            return false;
        }
        var start = selectLast ? _steps.Count - 1 : 0;
        var direction = selectLast ? -1 : +1;
        return TryFindEnabledFrom(start, direction, out var target) && SelectStep(target);
    }
    private bool TryFindEnabledFrom(int start, int direction, out int found)
    {
        for (var index = start; index >= 0 && index < _steps.Count; index += direction)
        {
            if (!_steps[index].IsDisabled)
            {
                found = index;
                return true;
            }
        }
        found = -1;
        return false;
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
    private void EnsureSelectionVisible(int viewportRows)
    {
        if (_steps.Count == 0 || viewportRows <= 0)
        {
            _scrollOffset = 0;
            return;
        }
        if (_currentIndex < _scrollOffset)
        {
            _scrollOffset = _currentIndex;
        }
        else if (_currentIndex >= _scrollOffset + viewportRows)
        {
            _scrollOffset = _currentIndex - viewportRows + 1;
        }
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, _steps.Count - viewportRows));
    }
    private void RaiseStepChangedIfNeeded(int previousIndex, WizardStep? previousStep)
    {
        if (previousIndex == _currentIndex && ReferenceEquals(previousStep, CurrentStep))
        {
            return;
        }
        var args = new WizardStepChangedEventArgs(previousIndex, _currentIndex, previousStep, CurrentStep);
        SelectionChanged?.Invoke(this, args);
        StepChanged?.Invoke(this, args);
    }
    private string BuildStepLine(int index, WizardStep step)
    {
        var marker = ResolveMarker(index, step);
        var numberPrefix = ShowStepNumbers ? $"{index + 1}. " : string.Empty;
        var title = NormalizeSingleLine(step.Title);
        var description = NormalizeSingleLine(step.Description);
        if (!string.IsNullOrWhiteSpace(description))
        {
            return string.Concat(marker, " ", numberPrefix, title, " - ", description);
        }
        return string.Concat(marker, " ", numberPrefix, title);
    }
    private string ResolveMarker(int index, WizardStep step)
    {
        if (step.IsDisabled) return DisabledMarker;
        if (index == _currentIndex) return ActiveMarker;
        if (step.IsCompleted) return CompletedMarker;
        return PendingMarker;
    }
    private TeaStyle ResolveStepStyle(int index)
    {
        var step = _steps[index];
        var style = StepStyle;
        if (step.IsDisabled) style = style.Merge(DisabledStepStyle);
        else if (index == _currentIndex)
        {
            style = style.Merge(ActiveStepStyle);
            if (IsFocused) style = style.Merge(FocusedActiveStepStyle);
        }
        else if (step.IsCompleted) style = style.Merge(CompletedStepStyle);
        else style = style.Merge(PendingStepStyle);
        if (index == _hoveredIndex) style = style.Merge(HoveredStepStyle);
        if (IsDisabled) style = style.Merge(DisabledStepStyle);
        return style;
    }
    private TeaStyle ResolveBorderStyle()
    {
        var style = IsFocused ? BorderStyleText.Merge(FocusedBorderStyleText) : BorderStyleText;
        return IsDisabled ? style.Merge(DisabledStepStyle) : style;
    }
    private TeaStyle ResolveTitleStyle()
    {
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return IsDisabled ? style.Merge(DisabledStepStyle) : style;
    }
    private TeaStyle ResolveEmptyStyle()
    {
        var style = EmptyStyle;
        return IsDisabled ? style.Merge(DisabledStepStyle) : style;
    }
    private string RenderTitle()
    {
        var titleText = RenderTitleText();
        var style = IsFocused ? FocusedTitleStyle : TitleStyle;
        return style.IsEmpty ? titleText : style.Render(titleText);
    }
    private string RenderTitleText()
    {
        if (IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker))
        {
            return string.Concat(Title, " ", FocusMarker);
        }
        return Title;
    }
    private static string NormalizeSingleLine(string? value)
    {
        return string.IsNullOrEmpty(value)
            ? string.Empty
            : value.Replace('\r', ' ').Replace('\n', ' ');
    }
    private static void WriteStyledText(Canvas canvas, int x, int y, string text, TeaStyle style, int width)
    {
        if (width <= 0)
        {
            return;
        }
        canvas.WriteText(x, y, style.IsEmpty ? text : style.Render(text), width);
    }
}
