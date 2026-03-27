using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class SpinnerApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private static readonly RunPhase[] Phases =
    [
        new("Resolving workspace", ["|", "/", "-", "\\"], 10, AnsiColor.Rgb(137, 180, 250)),
        new("Downloading packages", [".  ", ".. ", "..."], 10, AnsiColor.Rgb(249, 226, 175)),
        new("Warming cache", [">  ", ">> ", ">>>"], 10, AnsiColor.Rgb(166, 227, 161)),
        new("Launching shell", ["o..", ".o.", "..o"], 10, AnsiColor.Rgb(243, 139, 168)),
    ];

    private readonly Spinner _spinner = new()
    {
        Title = "Spinner",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        IsFocused = true,
    };

    private readonly StatusBar _status = new();

    private int _phaseIndex;
    private int _phaseTicks;
    private bool _completed;

    public SpinnerApp()
    {
        RestartRun();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(90), static now => new SpinnerPulse(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key)
        {
            if (key.IsCharacter('c', ModifierKeys.Ctrl))
            {
                return TeaEffects.Quit;
            }

            if (key.IsCharacter('r', ModifierKeys.Ctrl))
            {
                RestartRun();
                return null;
            }
        }

        if (message is not SpinnerPulse)
        {
            return null;
        }

        if (_completed || !_spinner.Running)
        {
            return null;
        }

        _spinner.Advance();
        _phaseTicks++;

        if (_phaseTicks < Phases[_phaseIndex].Ticks)
        {
            return null;
        }

        if (_phaseIndex < Phases.Length - 1)
        {
            _phaseIndex++;
            _phaseTicks = 0;
            ApplyPhase();
            return null;
        }

        _completed = true;
        _spinner.SetRunning(false);
        _spinner.SetFrames(["*"]);
        _spinner.Label = "Ready";
        ApplyCompletePalette();
        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        UpdateFooter();

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Body(new CenterLayout
            {
                Content = _spinner,
                Width = Math.Min(54, Math.Max(38, context.Width - 4)),
                Height = 5,
            });
            window.Footer(1, _status);
        });
    }

    private void RestartRun()
    {
        _completed = false;
        _phaseIndex = 0;
        _phaseTicks = 0;
        _spinner.SetRunning(true);
        ApplyPhase();
    }

    private void ApplyPhase()
    {
        ThemeScope.Apply(DefaultTheme, _spinner, _status);

        var theme = DefaultTheme;
        var phase = Phases[_phaseIndex];
        var accent = TeaStyle.Empty.WithForeground(phase.Accent).WithBold();
        _spinner.SetFrames(phase.Frames);
        _spinner.Label = phase.Label;
        _spinner.TitleStyle = theme.Text.Primary;
        _spinner.FocusedTitleStyle = accent;
        _spinner.ValueStyle = theme.Text.Primary;
        _spinner.RunningValueStyle = accent;
        _spinner.StoppedValueStyle = theme.Text.Secondary;
        _spinner.DisabledValueStyle = theme.Text.Muted.WithDim();
        _spinner.BorderStyleText = theme.Border.Strong;
        _spinner.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border).Merge(accent);
    }

    private void ApplyCompletePalette()
    {
        ThemeScope.Apply(DefaultTheme, _spinner, _status);

        var theme = DefaultTheme;
        var accent = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(166, 227, 161)).WithBold();
        _spinner.TitleStyle = theme.Text.Primary;
        _spinner.FocusedTitleStyle = accent;
        _spinner.ValueStyle = theme.Text.Primary;
        _spinner.RunningValueStyle = accent;
        _spinner.StoppedValueStyle = accent;
        _spinner.DisabledValueStyle = theme.Text.Muted.WithDim();
        _spinner.BorderStyleText = theme.Border.Strong;
        _spinner.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border).Merge(accent);
    }

    private void UpdateFooter()
    {
        var phaseName = _completed ? "complete" : $"{_phaseIndex + 1}/{Phases.Length}";
        var status = _completed ? "complete" : _spinner.Running ? "running" : "paused";
        _status.LeftText =
            $"status={status} phase={phaseName} label={_spinner.Label} frames={_spinner.Frames.Count}";
        _status.RightText =
            $"auto-run frames+label swap | Enter/click pause Right/Space/wheel advance ^R restart ^C quit";
    }

    private sealed record SpinnerPulse(DateTimeOffset Timestamp) : Message;

    private sealed record RunPhase(string Label, string[] Frames, int Ticks, AnsiColor Accent);
}
