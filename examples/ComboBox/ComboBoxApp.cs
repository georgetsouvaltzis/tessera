using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class ComboBoxApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private static readonly string[] SeedItems =
    [
        "us-east-1",
        "us-west-2",
        "eu-west-1",
        "eu-central-1",
        "ap-south-1",
        "ap-southeast-2",
        "sa-east-1",
        "ca-central-1",
    ];

    private readonly ComboBox _combo = new()
    {
        Title = "ComboBox",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "type region, Enter commit",
        MaxVisibleItems = 5,
    };

    private readonly StatusBar _status = new();

    private bool _isReadOnly;
    private bool _isDisabled;
    private bool _styleAlt;
    private int _selectionChanges;
    private int _itemCount;
    private string _statusText = "widget-only proof: type/filter, open/select, pointer flow, api selection";

    public ComboBoxApp()
    {
        Seed();
        ApplyTheme();
        _combo.RequestFocus();
        _combo.SelectionChanged += (_, args) =>
        {
            _selectionChanges++;
            var previous = string.IsNullOrEmpty(args.PreviousItem) ? "-" : args.PreviousItem;
            var current = string.IsNullOrEmpty(args.SelectedItem) ? "-" : args.SelectedItem;
            _statusText = $"selection {previous}->{current}";
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
            _statusText = "api SetItems(seed): placeholder restored";
            return null;
        }

        if (key.IsCharacter('e', ModifierKeys.Ctrl))
        {
            _combo.SetItems(Array.Empty<string>());
            _combo.SetFilterText(string.Empty);
            _itemCount = 0;
            _statusText = "api SetItems(empty): field empty";
            return null;
        }

        if (key.IsCharacter('f', ModifierKeys.Ctrl))
        {
            _combo.SetFilterText("us");
            _statusText = "api SetFilterText(us): filtered list ready";
            return null;
        }

        if (key.IsCharacter('n', ModifierKeys.Ctrl))
        {
            _combo.SetFilterText("zzz");
            _statusText = "api SetFilterText(zzz): no matches";
            return null;
        }

        if (key.IsCharacter('u', ModifierKeys.Ctrl))
        {
            _combo.SetFilterText(string.Empty);
            _statusText = "api SetFilterText(empty): placeholder restored";
            return null;
        }

        if (key.IsCharacter('g', ModifierKeys.Ctrl))
        {
            var changed = _combo.SetSelectedIndex(99);
            _statusText = $"api SetSelectedIndex(99)={changed} => {_combo.SelectedItem}";
            return null;
        }

        if (key.IsCharacter('p', ModifierKeys.Ctrl))
        {
            var changed = _combo.TrySetSelectedItem("us-west-2");
            _statusText = $"api TrySetSelectedItem(us-west-2)={changed}";
            return null;
        }

        if (key.IsCharacter('o', ModifierKeys.Ctrl))
        {
            _isReadOnly = !_isReadOnly;
            _combo.IsReadOnly = _isReadOnly;
            _statusText = $"readonly={_isReadOnly}";
            return null;
        }

        if (key.IsCharacter('i', ModifierKeys.Ctrl))
        {
            _isDisabled = !_isDisabled;
            _combo.IsDisabled = _isDisabled;
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
                Content = _combo,
                Width = Math.Min(52, Math.Max(36, context.Width - 4)),
                Height = Math.Min(11, Math.Max(8, context.Height - 4)),
            });
            window.Footer(1, _status);
        });
    }

    private void Seed()
    {
        _combo.SetItems(Array.Empty<string>());
        _combo.SetItems(SeedItems);
        _combo.SetFilterText(string.Empty);
        _itemCount = SeedItems.Length;
    }

    private void ApplyTheme()
    {
        ThemeScope.Apply(DefaultTheme, _combo, _status);

        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        _combo.TitleStyle = theme.Text.Primary;
        _combo.FocusedTitleStyle = focusedBorder.WithBold();
        _combo.BorderStyleText = theme.Border.Strong;
        _combo.FocusedBorderStyleText = focusedBorder;
        _combo.ValueTextStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175)).WithBold()
            : theme.Text.Primary;
        _combo.PlaceholderTextStyle = theme.Text.Muted.WithItalic();
        _combo.HoveredValueStyle = theme.Accent.Secondary.WithUnderline();
        _combo.OptionStyle = theme.Text.Primary;
        _combo.SelectedOptionStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _combo.HoveredOptionStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(137, 220, 235)).WithUnderline()
            : theme.Accent.Secondary.WithUnderline();
        _combo.MutedStyle = theme.Text.Muted.WithItalic();
        _combo.DisabledStyle = theme.Text.Muted.WithDim();
        _combo.Glyphs = _styleAlt
            ? new DropdownGlyphSet("▸", "▾", "›", "●")
            : DropdownGlyphSet.Default;
    }

    private void UpdateFooter()
    {
        var selected = string.IsNullOrEmpty(_combo.SelectedItem) ? "-" : _combo.SelectedItem;
        var filter = string.IsNullOrEmpty(_combo.FilterText) ? "-" : _combo.FilterText;
        var status = _statusText;
        if (_isDisabled)
        {
            status = "disabled: typing, open, and pointer flow blocked";
        }
        else if (_isReadOnly)
        {
            status = "readonly: typing and selection flow blocked";
        }

        _status.LeftText =
            $"open={_combo.IsOpen} filter={filter} sel={selected} items={_itemCount} ro={_isReadOnly} dis={_isDisabled} sch={_selectionChanges}";
        _status.RightText =
            $"{status} | type to filter Down open j/k move Enter commit Esc close click/wheel list ^R seed ^E empty ^F us ^N no-match ^U clear ^G set-index(99) ^P set-west ^T style ^O ro ^I dis ^C quit";
    }
}
