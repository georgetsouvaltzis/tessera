using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class ChoiceApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private static readonly string[] SeedItems =
    [
        "dev",
        "stage",
        "prod",
        "perf",
        "demo",
        "dr",
        "local",
        "qa",
    ];

    private readonly Choice _choice = new()
    {
        Title = "Choice",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        MaxVisibleItems = 4,
    };

    private readonly StatusBar _status = new();

    private bool _isReadOnly;
    private bool _isDisabled;
    private bool _styleAlt;
    private int _selectionChanges;
    private string _statusText = "widget-only proof: open/select, pointer flow, api selection";

    public ChoiceApp()
    {
        Seed();
        ApplyTheme();
        _choice.RequestFocus();
        _choice.SelectionChanged += (_, args) =>
        {
            _selectionChanges++;
            _statusText = $"selection {args.PreviousItem}->{args.SelectedItem}";
        };
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.IsCharacter('r', ModifierKeys.Ctrl))
        {
            Seed();
            _statusText = "api SetItems(seed): open with Enter or click field";
            return null;
        }

        if (key.IsCharacter('e', ModifierKeys.Ctrl))
        {
            _choice.SetItems(Array.Empty<string>());
            _statusText = "api SetItems(empty): field shows (empty)";
            return null;
        }

        if (key.IsCharacter('g', ModifierKeys.Ctrl))
        {
            var changed = _choice.SetSelectedIndex(99);
            _statusText = $"api SetSelectedIndex(99)={changed} => {_choice.SelectedItem}";
            return null;
        }

        if (key.IsCharacter('p', ModifierKeys.Ctrl))
        {
            var changed = _choice.TrySetSelectedItem("prod");
            _statusText = $"api TrySetSelectedItem(prod)={changed}";
            return null;
        }

        if (key.IsCharacter('o', ModifierKeys.Ctrl))
        {
            _isReadOnly = !_isReadOnly;
            _choice.IsReadOnly = _isReadOnly;
            _statusText = $"readonly={_isReadOnly}";
            return null;
        }

        if (key.IsCharacter('i', ModifierKeys.Ctrl))
        {
            _isDisabled = !_isDisabled;
            _choice.IsDisabled = _isDisabled;
            _statusText = $"disabled={_isDisabled}";
            return null;
        }

        if (key.IsCharacter('t', ModifierKeys.Ctrl))
        {
            _styleAlt = !_styleAlt;
            ApplyTheme();
            _statusText = _styleAlt ? "style=alt glyphs" : "style=default glyphs";
            return null;
        }

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
                Content = _choice,
                Width = Math.Min(44, Math.Max(32, context.Width - 4)),
                Height = Math.Min(10, Math.Max(7, context.Height - 4)),
            });
            window.Footer(1, _status);
        });
    }

    private void Seed()
    {
        _choice.SetItems(SeedItems);
        _choice.SetSelectedIndex(1);
    }

    private void ApplyTheme()
    {
        ThemeScope.Apply(DefaultTheme, _choice, _status);

        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        _choice.TitleStyle = theme.Text.Primary;
        _choice.FocusedTitleStyle = focusedBorder.WithBold();
        _choice.BorderStyleText = theme.Border.Strong;
        _choice.FocusedBorderStyleText = focusedBorder;
        _choice.ValueStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175)).WithBold()
            : theme.Text.Primary;
        _choice.HoveredValueStyle = theme.Accent.Secondary.WithUnderline();
        _choice.OptionStyle = theme.Text.Primary;
        _choice.SelectedOptionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _choice.HoveredOptionStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(137, 220, 235)).WithUnderline()
            : theme.Accent.Secondary.WithUnderline();
        _choice.MutedStyle = theme.Text.Muted.WithItalic();
        _choice.DisabledStyle = theme.Text.Muted.WithDim();
        _choice.Glyphs = _styleAlt
            ? new DropdownGlyphSet(
                collapsedIndicator: "▸",
                expandedIndicator: "▾",
                highlightedOptionMarker: "›",
                selectedOptionMarker: "●")
            : DropdownGlyphSet.Default;
    }

    private void UpdateFooter()
    {
        var status = _statusText;
        if (_isDisabled)
        {
            status = "disabled: field and list interaction blocked";
        }
        else if (_isReadOnly)
        {
            status = "readonly: open/select interaction blocked";
        }

        _status.LeftText =
            $"open={_choice.IsOpen} idx={_choice.SelectedIndex} item={_choice.SelectedItem} ro={_isReadOnly} dis={_isDisabled} sch={_selectionChanges}";
        _status.RightText =
            $"{status} | Enter/Space/Down open Esc close Up/Down j/k move Enter/Space commit click field/item wheel open list ^R seed ^E empty ^G set-index(99) ^P set-prod ^T style ^O ro ^I dis ^C quit";
    }
}
