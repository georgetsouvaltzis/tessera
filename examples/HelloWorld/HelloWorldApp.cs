using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Examples.HelloWorld;

internal sealed class HelloWorldApp : TesseraApp
{
    private readonly Label _centerChip =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly Label _colorChip =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly Label _countChip =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly Label _eyebrow =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly Control[] _focusOrder;
    private readonly StatusBar _footer = new() { Fill = ' ' };

    private readonly Label _headline =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly Label _hint = new()
    {
        Border = BorderStyle.None,
        HorizontalAlignment = HorizontalAlignment.Center
    };

    private readonly Button _incrementButton = new() { Text = "Spark +1", Padding = Thickness.Symmetric(3) };
    private readonly Button _resetButton = new() { Text = "Reset", Padding = Thickness.Symmetric(3) };

    private readonly Label _runtimeChip =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly Label _subhead =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private int _count = 3;
    private int _focusIndex;

    public HelloWorldApp()
    {
        _focusOrder = [_incrementButton, _resetButton];
        ConfigureTheme();
        WireEvents();
        _incrementButton.RequestFocus();
    }

    public override TesseraEffect? Update(Message message)
    {
        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TesseraEffects.Quit;
        }

        if (key.Is(Key.Tab))
        {
            FocusNext();
            return null;
        }

        if (key.IsCharacter('+'))
        {
            _count++;
            return null;
        }

        if (key.IsCharacter('0') || key.IsCharacter('r'))
        {
            _count = 0;
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshChrome();

        var cardWidth = Math.Max(56, Math.Min(84, context.Width - 4));
        var cardHeight = Math.Max(15, Math.Min(19, context.Height - 3));

        return Screen.Build(window =>
        {
            window.Body(body => body.Center(
                center => center.Column(column =>
                {
                    column.Gap(1);
                    column.Auto(content => content.Center(_eyebrow));
                    column.Auto(content => content.Center(_headline));
                    column.Auto(content => content.Center(_subhead));
                    column.Fixed(1, ribbon => ribbon.Center(row => row.Row(chips =>
                    {
                        chips.Gap(2);
                        chips.Auto(_centerChip);
                        chips.Auto(_colorChip);
                        chips.Auto(_runtimeChip);
                    })));
                    column.Auto(content => content.Center(_countChip));
                    column.Fixed(3, actions => actions.Center(row => row.Row(buttons =>
                    {
                        buttons.Gap(2);
                        buttons.Fixed(16, _incrementButton);
                        buttons.Fixed(14, _resetButton);
                    })));
                    column.Auto(content => content.Center(_hint));
                }),
                cardWidth,
                cardHeight));
            window.Footer(1, _footer);
        });
    }

    private void ConfigureTheme()
    {
        _incrementButton.ApplyTheme(HelloWorldTheme.Default);
        _resetButton.ApplyTheme(HelloWorldTheme.Default);
        _footer.ApplyTheme(HelloWorldTheme.Default);

        _eyebrow.TextStyle = HelloWorldTheme.Surface(0x0A1020, 0x67F7C7).WithBold();
        _headline.TextStyle = HelloWorldTheme.Foreground(0xF6F8FF).WithBold();
        _subhead.TextStyle = HelloWorldTheme.Foreground(0xA8B2D8);
        _centerChip.TextStyle = HelloWorldTheme.Surface(0x071018, 0x6CEFD0).WithBold();
        _colorChip.TextStyle = HelloWorldTheme.Surface(0x11172C, 0xFFC86B).WithBold();
        _runtimeChip.TextStyle = HelloWorldTheme.Surface(0x0F1324, 0x93D8FF).WithBold();
        _countChip.TextStyle = HelloWorldTheme.Surface(0x071018, 0xFFD166).WithBold();
        _hint.TextStyle = HelloWorldTheme.Foreground(0x8AD8FF);

        _incrementButton.LabelStyle = HelloWorldTheme.Foreground(0x071018).WithBold();
        _incrementButton.SurfaceStyle = HelloWorldTheme.Background(0x67F7C7);
        _incrementButton.FocusedSurfaceStyle = HelloWorldTheme.Background(0x83FFDA);
        _incrementButton.PressedSurfaceStyle = HelloWorldTheme.Background(0x42D7A8);

        _resetButton.LabelStyle = HelloWorldTheme.Foreground(0x0F1424).WithBold();
        _resetButton.SurfaceStyle = HelloWorldTheme.Background(0xFFB86B);
        _resetButton.FocusedSurfaceStyle = HelloWorldTheme.Background(0xFFC98E);
        _resetButton.PressedSurfaceStyle = HelloWorldTheme.Background(0xE89C43);

        _footer.LeftTextStyle = HelloWorldTheme.Foreground(0xA8B2D8);
        _footer.RightTextStyle = HelloWorldTheme.Foreground(0x67F7C7).WithBold();
        _footer.FillStyle = HelloWorldTheme.Background(0x090D1A);
    }

    private void WireEvents()
    {
        _incrementButton.Activated += (_, _) => _count++;
        _resetButton.Activated += (_, _) => _count = 0;
    }

    private void FocusNext()
    {
        _focusIndex = (_focusIndex + 1) % _focusOrder.Length;
        _focusOrder[_focusIndex].RequestFocus();
    }

    private void RefreshChrome()
    {
        _eyebrow.Text = "  HELLO WORLD // PUBLIC ALPHA  ";
        _headline.Text = "Hello world, but already dressed like a product";
        _subhead.Text = "A centered launch card with loud color, live state, and obvious actions.";
        _centerChip.Text = "  centered shell  ";
        _colorChip.Text = "  mint + amber  ";
        _runtimeChip.Text = "  click ready  ";
        _countChip.Text = $"   {_count:D2} sparks in orbit   ";
        _hint.Text = "Tab moves focus  |  Enter activates  |  + increments  |  0 resets";
        _footer.LeftText = "Tessera HelloWorld";
        _footer.RightText = "Start here, then open CounterForm";
    }
}
