using System.ComponentModel;
using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Controls;

/// <summary>
///     Represents a compact wizard-step flow control.
/// </summary>
public sealed partial class Stepper : Control
{
    private readonly List<StepperStep> _steps = [];
    private int _currentIndex = -1;

    /// <summary>
    ///     Gets configured steps.
    /// </summary>
    public IReadOnlyList<StepperStep> Steps => _steps;

    /// <summary>
    ///     Gets or sets current step index.
    ///     Returns <c>-1</c> when no selectable steps exist.
    /// </summary>
    public int CurrentIndex
    {
        get => _currentIndex;
        set => SetCurrentStep(value);
    }

    /// <summary>
    ///     Gets or sets selected step index.
    ///     Canonical property for selection access.
    /// </summary>
    public int SelectedIndex
    {
        get => CurrentIndex;
        set => SetCurrentStep(value);
    }

    /// <summary>
    ///     Gets current step, when available.
    /// </summary>
    public StepperStep? CurrentStep => _currentIndex >= 0 && _currentIndex < _steps.Count
        ? _steps[_currentIndex]
        : null;

    /// <summary>
    ///     Gets selected step, when available.
    ///     Canonical property for selection access.
    /// </summary>
    public StepperStep? SelectedStep => CurrentStep;

    /// <summary>
    ///     Gets or sets optional title shown before steps.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets marker shown in title when focused.
    /// </summary>
    public string FocusMarker { get; set; } = "*";

    /// <summary>
    ///     Gets or sets whether the focused title marker should be shown.
    /// </summary>
    public bool ShowFocusMarker { get; set; } = true;

    /// <summary>
    ///     Gets or sets style used for title when unfocused.
    /// </summary>
    public TesseraStyle TitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for title when focused.
    /// </summary>
    public TesseraStyle FocusedTitleStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets base style for step text.
    /// </summary>
    public TesseraStyle StepTextStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into active step text.
    /// </summary>
    public TesseraStyle ActiveStepStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into completed step text.
    /// </summary>
    public TesseraStyle CompletedStepStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into pending step text.
    /// </summary>
    public TesseraStyle PendingStepStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style merged into disabled step text.
    /// </summary>
    public TesseraStyle DisabledStepStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets style used for connector text between steps.
    /// </summary>
    public TesseraStyle ConnectorStyle { get; set; } = TesseraStyle.Empty;

    /// <summary>
    ///     Gets or sets connector text rendered between steps.
    /// </summary>
    public string Connector { get; set; } = " -> ";

    /// <inheritdoc />
    public override bool IsFocused { get; set; }

    /// <inheritdoc />
    public override bool IsDisabled { get; set; }

    /// <inheritdoc />
    public override bool IsReadOnly { get; set; }

    /// <summary>
    ///     Occurs when selection changes.
    ///     Canonical event for selection transition handling.
    /// </summary>
    public event EventHandler<StepperCurrentStepChangedEventArgs>? SelectionChanged;

    /// <summary>
    ///     Occurs when <see cref="CurrentIndex" /> changes.
    ///     Compatibility alias for <see cref="SelectionChanged" />.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public event EventHandler<StepperCurrentStepChangedEventArgs>? CurrentStepChanged;

    /// <summary>
    ///     Replaces all steps.
    /// </summary>
    /// <param name="steps">Step definitions.</param>
    public void SetSteps(IEnumerable<StepperStep> steps)
    {
        ArgumentNullException.ThrowIfNull(steps);
        var previousIndex = _currentIndex;
        var previousStep = CurrentStep;
        var previousId = previousStep?.Id;

        _steps.Clear();
        foreach (var step in steps)
        {

            _steps.Add(new StepperStep(step.Id, step.Label, step.IsCompleted, step.IsDisabled));
        }

        _currentIndex = ResolveInitialCurrentIndex(previousId);
        RaiseCurrentStepChangedIfNeeded(previousIndex, previousStep);
    }

    /// <summary>
    ///     Replaces all steps from labels.
    /// </summary>
    /// <param name="labels">Labels used as both id and label.</param>
    public void SetSteps(IEnumerable<string> labels)
    {
        ArgumentNullException.ThrowIfNull(labels);
        SetSteps(labels.Select(static label => new StepperStep(label, label)));
    }

    /// <summary>
    ///     Selects the next enabled step.
    /// </summary>
    /// <returns><see langword="true" /> when current step changed; otherwise <see langword="false" />.</returns>
    public bool NextStep()
    {
        return MoveCurrent(+1);
    }

    /// <summary>
    ///     Selects the previous enabled step.
    /// </summary>
    /// <returns><see langword="true" /> when current step changed; otherwise <see langword="false" />.</returns>
    public bool PreviousStep()
    {
        return MoveCurrent(-1);
    }

    /// <summary>
    ///     Selects a specific step index.
    /// </summary>
    /// <param name="index">Requested step index.</param>
    /// <returns><see langword="true" /> when current step changed; otherwise <see langword="false" />.</returns>
    public bool SetCurrentStep(int index)
    {
        if (_steps.Count == 0)
        {
            return false;
        }

        var clamped = Math.Clamp(index, 0, _steps.Count - 1);
        if (_steps[clamped].IsDisabled && !TryFindEnabledFrom(clamped, +1, out clamped) &&
            !TryFindEnabledFrom(clamped, -1, out clamped))
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
        RaiseCurrentStepChangedIfNeeded(previousIndex, previousStep);
        return true;
    }

    /// <summary>
    ///     Marks the current step as completed.
    /// </summary>
    /// <returns><see langword="true" /> when state changed; otherwise <see langword="false" />.</returns>
    public bool CompleteCurrentStep()
    {
        return SetStepCompleted(_currentIndex);
    }

    /// <summary>
    ///     Sets completion state for a specific step.
    /// </summary>
    /// <param name="index">Step index.</param>
    /// <param name="isCompleted">Completion state.</param>
    /// <returns><see langword="true" /> when state changed; otherwise <see langword="false" />.</returns>
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

    /// <inheritdoc />
    public override bool Handle(Message message)
    {
        if (IsDisabled || IsReadOnly || !IsFocused || _steps.Count == 0 || message is not KeyPressed key)
        {
            return false;
        }

        if (key.Is(Key.Right) || key.Is(Key.Down) || key.Is(Key.PageDown))
        {
            return NextStep();
        }

        if (key.Is(Key.Left) || key.Is(Key.Up) || key.Is(Key.PageUp))
        {
            return PreviousStep();
        }

        if (key.Is(Key.Home))
        {
            return SelectEdge(false);
        }

        if (key.Is(Key.End))
        {
            return SelectEdge(true);
        }

        if (key.Is(Key.Enter) || key.IsCharacter(' ') || key.IsCharacter('c'))
        {
            return CompleteCurrentStep();
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

        if (pointer.Kind != PointerEventKind.Press || pointer.Button != PointerButton.Left)
        {
            return Handle(message);
        }

        if (!bounds.Contains(pointer.X, pointer.Y) || pointer.Y != bounds.Y)
        {
            return Handle(message);
        }

        var hitIndex = HitTestStepIndex(pointer.X, bounds);
        return hitIndex >= 0 && SetCurrentStep(hitIndex);
    }

    /// <inheritdoc />
    public override void Render(Canvas canvas, Rect rect)
    {
        var clipped = Rect.Intersect(rect, canvas.Bounds);
        if (clipped.IsEmpty || clipped.Height < 1)
        {
            return;
        }

        var x = clipped.X;
        var y = clipped.Y;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            canvas.WriteText(x, y, ApplyStyle(title, IsFocused ? FocusedTitleStyle : TitleStyle), clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(title) + 1;
        }

        if (_steps.Count == 0)
        {
            canvas.WriteText(x, y, ApplyStyle("(no steps)", PendingStepStyle.Merge(DisabledStepStyle)),
                clipped.Right - x);
            return;
        }

        for (var index = 0; index < _steps.Count && x < clipped.Right; index++)
        {
            if (index > 0)
            {
                canvas.WriteText(x, y, ApplyStyle(Connector, ResolveConnectorStyle()), clipped.Right - x);
                x += ControlTextLayout.MeasureDisplayWidth(Connector);
            }

            if (x >= clipped.Right)
            {
                break;
            }

            var label = FormatStepLabel(index);
            canvas.WriteText(x, y, ApplyStyle(label, ResolveStepStyle(index)), clipped.Right - x);
            x += ControlTextLayout.MeasureDisplayWidth(label);
        }
    }

    internal override LayoutMeasurement Measure(in Rect availableBounds)
    {
        var width = 0;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            width += ControlTextLayout.MeasureDisplayWidth(title) + (_steps.Count > 0 ? 1 : 0);
        }

        for (var index = 0; index < _steps.Count; index++)
        {
            if (index > 0)
            {
                width += ControlTextLayout.MeasureDisplayWidth(Connector);
            }

            width += ControlTextLayout.MeasureDisplayWidth(FormatStepLabel(index));
        }

        if (_steps.Count == 0)
        {
            width += ControlTextLayout.MeasureDisplayWidth("(no steps)");
        }

        return new LayoutMeasurement(
            Math.Clamp(width, 0, availableBounds.Width),
            Math.Clamp(width == 0 ? 0 : 1, 0, availableBounds.Height));
    }
}
