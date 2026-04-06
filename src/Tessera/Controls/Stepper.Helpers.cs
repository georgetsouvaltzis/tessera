using Tessera.Components.Primitives;
using Tessera.Controls.Internal;
using Tessera.Styles;

namespace Tessera.Controls;

public sealed partial class Stepper
{
    private int ResolveInitialCurrentIndex(string? previousId)
    {
        if (_steps.Count == 0)
        {
            return -1;
        }

        if (!string.IsNullOrEmpty(previousId))
        {
            for (var index = 0; index < _steps.Count; index++)
            {
                if (!_steps[index].IsDisabled && string.Equals(_steps[index].Id, previousId, StringComparison.Ordinal))
                {
                    return index;
                }
            }
        }

        return TryFindEnabledFrom(0, +1, out var firstEnabled) ? firstEnabled : -1;
    }

    private bool MoveCurrent(int direction)
    {
        if (_steps.Count == 0 || _currentIndex < 0)
        {
            return false;
        }

        return TryFindEnabledFrom(_currentIndex + direction, direction, out var index) && SetCurrentStep(index);
    }

    private bool SelectEdge(bool selectLast)
    {
        if (_steps.Count == 0)
        {
            return false;
        }

        var start = selectLast ? _steps.Count - 1 : 0;
        var direction = selectLast ? -1 : +1;
        return TryFindEnabledFrom(start, direction, out var index) && SetCurrentStep(index);
    }

    private bool TryFindEnabledFrom(int startIndex, int direction, out int foundIndex)
    {
        foundIndex = -1;
        if (_steps.Count == 0)
        {
            return false;
        }

        var index = startIndex;
        while (index >= 0 && index < _steps.Count)
        {
            if (!_steps[index].IsDisabled)
            {
                foundIndex = index;
                return true;
            }

            index += direction;
        }

        return false;
    }

    private int HitTestStepIndex(int x, Rect bounds)
    {
        var cursor = bounds.X;
        var title = FormatTitleText();
        if (!string.IsNullOrEmpty(title))
        {
            cursor += ControlTextLayout.MeasureDisplayWidth(title) + 1;
        }

        for (var index = 0; index < _steps.Count; index++)
        {
            if (index > 0)
            {
                cursor += ControlTextLayout.MeasureDisplayWidth(Connector);
            }

            var label = FormatStepLabel(index);
            var width = ControlTextLayout.MeasureDisplayWidth(label);
            if (x >= cursor && x < cursor + width)
            {
                return _steps[index].IsDisabled ? -1 : index;
            }

            cursor += width;
        }

        return -1;
    }

    private string FormatTitleText()
    {
        if (string.IsNullOrEmpty(Title))
        {
            return string.Empty;
        }

        return IsFocused && ShowFocusMarker && !string.IsNullOrWhiteSpace(FocusMarker)
            ? $"{Title} {FocusMarker}"
            : Title;
    }

    private string FormatStepLabel(int index)
    {
        var step = _steps[index];
        var prefix = step.IsDisabled
            ? "[-]"
            : step.IsCompleted
                ? "[x]"
                : index == _currentIndex
                    ? "[>]"
                    : "[ ]";
        return $"{prefix} {step.Label}";
    }

    private TesseraStyle ResolveStepStyle(int index)
    {
        var step = _steps[index];
        var style = StepTextStyle;
        if (step.IsDisabled)
        {
            style = style.Merge(DisabledStepStyle);
        }
        else if (index == _currentIndex)
        {
            style = style.Merge(ActiveStepStyle);
        }
        else if (step.IsCompleted)
        {
            style = style.Merge(CompletedStepStyle);
        }
        else
        {
            style = style.Merge(PendingStepStyle);
        }

        if (IsDisabled)
        {
            style = style.Merge(DisabledStepStyle);
        }

        return style;
    }

    private TesseraStyle ResolveConnectorStyle()
    {
        var style = ConnectorStyle;
        if (IsDisabled)
        {
            style = style.Merge(DisabledStepStyle);
        }

        return style;
    }

    private void RaiseCurrentStepChangedIfNeeded(int previousIndex, StepperStep? previousStep)
    {
        if (previousIndex == _currentIndex && ReferenceEquals(previousStep, CurrentStep))
        {
            return;
        }

        var args = new StepperCurrentStepChangedEventArgs(previousIndex, _currentIndex, previousStep, CurrentStep);
        SelectionChanged?.Invoke(this, args);
        CurrentStepChanged?.Invoke(this, args);
    }

    private static string ApplyStyle(string text, TesseraStyle style)
    {
        return string.IsNullOrEmpty(text) || style.IsEmpty
            ? text
            : style.Render(text);
    }
}
