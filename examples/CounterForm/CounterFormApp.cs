using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Examples.CounterForm;

internal sealed class CounterFormApp : TeaApp
{
    private readonly Label _eyebrow = new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly Label _headline = new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly Label _countChip = new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly Label _labelChip = new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly Label _stepChip = new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly Label _paletteChip = new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly TextInput _labelInput = new() { Title = "Pulse Name", Placeholder = "Name this pulse", Padding = Thickness.Symmetric(1, 0) };
    private readonly NumberInput _stepInput = new() { Title = "Step", Min = 1, Max = 9, Precision = 0, Padding = Thickness.Symmetric(1, 0) };
    private readonly Choice _themeChoice = new() { Title = "Palette", Padding = Thickness.Symmetric(1, 0) };
    private readonly ProgressBar _meter = new() { Title = "Momentum", Padding = Thickness.Symmetric(1, 0) };
    private readonly Button _downButton = new() { Text = "Count -", Padding = Thickness.Symmetric(3, 0) };
    private readonly Button _upButton = new() { Text = "Count +", Padding = Thickness.Symmetric(3, 0) };
    private readonly Button _resetButton = new() { Text = "Reset", Padding = Thickness.Symmetric(3, 0) };
    private readonly Label _summary = new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };
    private readonly StatusBar _footer = new() { Fill = ' ' };

    private readonly Control[] _focusOrder;
    private CounterFormPalette _palette = CounterFormTheme.Default;
    private int _focusIndex;
    private int _count = 12;

    public CounterFormApp()
    {
        _focusOrder = [_labelInput, _stepInput, _themeChoice, _upButton, _downButton, _resetButton];
        _labelInput.SetValue("Launch pulse");
        _stepInput.SetValue(2);
        _themeChoice.SetItems(CounterFormTheme.All.Select(static palette => palette.Name));
        _themeChoice.TrySetSelectedItem(_palette.Name);
        ConfigureTheme();
        WireEvents();
        _labelInput.RequestFocus();
    }

    public override TeaEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key when key.IsCharacter('c', ModifierKeys.Ctrl):
                return TeaEffects.Quit;
            case KeyPressed key when key.Is(Key.Tab):
                FocusNext();
                return null;
            case KeyPressed key when key.IsCharacter('+'):
                _count += StepValue();
                return null;
            case KeyPressed key when key.IsCharacter('-'):
                _count -= StepValue();
                return null;
            case CounterNudgeMessage nudge:
                _count += nudge.Direction * StepValue();
                return null;
            case CounterResetMessage:
                _count = 0;
                return null;
            case CounterPaletteMessage palette:
                _palette = CounterFormTheme.Resolve(palette.Name);
                ConfigureTheme();
                return null;
            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshChrome();

        var cardWidth = Math.Max(74, Math.Min(94, context.Width - 4));
        var cardHeight = Math.Max(21, Math.Min(25, context.Height - 3));

        return Screen.Build(window =>
        {
            window.Body(body => body.Center(
                center => center.Column(column =>
                {
                    column.Gap(1);
                    column.Auto(content => content.Center(_eyebrow));
                    column.Auto(content => content.Center(_headline));
                    column.Auto(content => content.Center(_countChip));
                    column.Fixed(4, fields => fields.Center(row => row.Row(stack =>
                    {
                        stack.Gap(2);
                        stack.Fixed(30, _labelInput);
                        stack.Fixed(16, _stepInput);
                        stack.Fixed(18, _themeChoice);
                    })));
                    column.Fixed(1, ribbon => ribbon.Center(row => row.Row(chips =>
                    {
                        chips.Gap(2);
                        chips.Auto(_labelChip);
                        chips.Auto(_stepChip);
                        chips.Auto(_paletteChip);
                    })));
                    column.Fixed(4, meter => meter.Center(row => row.Row(stack => stack.Fixed(38, _meter))));
                    column.Fixed(3, actions => actions.Center(
                        row => row.Row(stack =>
                        {
                            stack.Gap(2);
                            stack.Fixed(14, _downButton);
                            stack.Fixed(14, _upButton);
                            stack.Fixed(14, _resetButton);
                        })));
                    column.Auto(content => content.Center(_summary));
                }),
                width: cardWidth,
                height: cardHeight));
            window.Footer(1, _footer);
        });
    }

    private void ConfigureTheme()
    {
        var theme = _palette.Theme;

        _labelInput.ApplyTheme(theme);
        _stepInput.ApplyTheme(theme);
        _themeChoice.ApplyTheme(theme);
        _meter.ApplyTheme(theme);
        _upButton.ApplyTheme(theme);
        _downButton.ApplyTheme(theme);
        _resetButton.ApplyTheme(theme);
        _footer.ApplyTheme(theme);

        _eyebrow.TextStyle = CounterFormTheme.Surface(0x090C16, 0xFFFFFF).WithBold();
        _headline.TextStyle = theme.Text.Primary.WithBold();
        _countChip.TextStyle = _palette.CountStyle;
        _labelChip.TextStyle = CounterFormTheme.Surface(0x090C16, 0xF8F7FF).WithBold();
        _stepChip.TextStyle = CounterFormTheme.Surface(0x090C16, 0xFFD37B).WithBold();
        _paletteChip.TextStyle = CounterFormTheme.Surface(0x091016, 0x96F2D7).WithBold();
        _summary.TextStyle = _palette.SummaryStyle;

        _upButton.LabelStyle = CounterFormTheme.Foreground(0x071018).WithBold();
        _upButton.SurfaceStyle = _palette.PositiveButtonStyle;
        _upButton.FocusedSurfaceStyle = _palette.PositiveButtonStyle;
        _upButton.PressedSurfaceStyle = CounterFormTheme.Background(0x5ED7B2);

        _downButton.LabelStyle = CounterFormTheme.Foreground(0x090C16).WithBold();
        _downButton.SurfaceStyle = _palette.NegativeButtonStyle;
        _downButton.FocusedSurfaceStyle = _palette.NegativeButtonStyle;
        _downButton.PressedSurfaceStyle = CounterFormTheme.Background(0xE57556);

        _resetButton.LabelStyle = CounterFormTheme.Foreground(0x090C16).WithBold();
        _resetButton.SurfaceStyle = _palette.NeutralButtonStyle;
        _resetButton.FocusedSurfaceStyle = _palette.NeutralButtonStyle;
        _resetButton.PressedSurfaceStyle = CounterFormTheme.Background(0xD8D8E5);

        _meter.FillStyle = theme.Accent.Primary;
        _meter.TrackStyle = theme.Text.Muted;
        _meter.LabelStyle = theme.Text.Primary.WithBold();

        _footer.LeftTextStyle = theme.Text.Secondary;
        _footer.RightTextStyle = theme.Accent.Primary;
        _footer.FillStyle = theme.Surface.Base;
    }

    private void WireEvents()
    {
        _themeChoice.SelectionChanged += (_, args) => Post(new CounterPaletteMessage(args.SelectedItem));
        _upButton.Activated += (_, _) => Post(new CounterNudgeMessage(+1));
        _downButton.Activated += (_, _) => Post(new CounterNudgeMessage(-1));
        _resetButton.Activated += (_, _) => Post(new CounterResetMessage());
    }

    private void FocusNext()
    {
        _focusIndex = (_focusIndex + 1) % _focusOrder.Length;
        _focusOrder[_focusIndex].RequestFocus();
    }

    private int StepValue()
    {
        var step = (int)Math.Round(_stepInput.Value, MidpointRounding.AwayFromZero);
        step = Math.Clamp(step, 1, 9);
        if (Math.Abs(_stepInput.Value - step) > double.Epsilon)
        {
            _stepInput.SetValue(step);
        }

        return step;
    }

    private void RefreshChrome()
    {
        var label = string.IsNullOrWhiteSpace(_labelInput.Value)
            ? "Launch pulse"
            : _labelInput.Value.Trim();
        var step = StepValue();
        var chipLabel = Shorten(label, 16);

        _eyebrow.Text = $"  COUNTER FORM // {_palette.Name.ToUpperInvariant()}  ";
        _headline.Text = "Tune a live counter, shift the palette, and watch the preview react in place.";
        _labelChip.Text = $"  label {chipLabel.ToLowerInvariant()}  ";
        _stepChip.Text = $"  step x{step}  ";
        _paletteChip.Text = $"  palette {_palette.Name.ToLowerInvariant()}  ";
        _countChip.Text = $"   {label.ToUpperInvariant()}  {_count:D2}   ";
        _meter.SetValue(Math.Clamp(Math.Abs(_count) / 32d, 0d, 1d));
        _summary.Text = $"{label} climbs by {step}  |  try Tab, arrows, Enter, and quick +/- nudges";
        _footer.LeftText = "CounterForm";
        _footer.RightText = "Palette swaps and live state in one centered card";
    }

    private static string Shorten(string value, int maxLength)
    {
        if (value.Length <= maxLength)
        {
            return value;
        }

        return $"{value[..Math.Max(0, maxLength - 3)]}...";
    }

    private sealed record CounterNudgeMessage(int Direction) : Message;

    private sealed record CounterResetMessage : Message;

    private sealed record CounterPaletteMessage(string Name) : Message;
}
