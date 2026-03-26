using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class TokenEditorApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly TokenEditor _editor = new()
    {
        Title = "TokenEditor",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "type token, press Enter",
    };

    private readonly StatusBar _status = new();

    private bool _isReadOnly;
    private bool _isDisabled;
    private bool _styleAlt;
    private int _tokenChangeCount;
    private int _selectionChangeCount;
    private string _statusText = "widget-only proof: typing, keyboard/pointer selection, api mutation";

    public TokenEditorApp()
    {
        SeedTokens();
        ApplyTheme();
        _editor.RequestFocus();
        _editor.TokensChanged += (_, args) =>
        {
            _tokenChangeCount++;
            _statusText = $"tokens {args.PreviousTokens.Count}->{args.Tokens.Count}";
        };
        _editor.SelectionChanged += (_, args) =>
        {
            _selectionChangeCount++;
            var previous = args.PreviousToken?.Value ?? "-";
            var current = args.SelectedToken?.Value ?? "-";
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
            SeedTokens();
            _statusText = "api SetTokens(seed): click pager/locked or use arrows";
            return null;
        }

        if (key.IsCharacter('e', ModifierKeys.Ctrl))
        {
            _editor.SetTokens(Array.Empty<TokenItem>());
            _statusText = "api SetTokens(empty): placeholder visible";
            return null;
        }

        if (key.IsCharacter('a', ModifierKeys.Ctrl))
        {
            var added = _editor.AddToken("api-token");
            _statusText = $"api AddToken(api-token)={added}";
            return null;
        }

        if (key.IsCharacter('x', ModifierKeys.Ctrl))
        {
            var removed = _editor.RemoveSelectedToken();
            _statusText = $"api RemoveSelectedToken()={removed}";
            return null;
        }

        if (key.IsCharacter('o', ModifierKeys.Ctrl))
        {
            _isReadOnly = !_isReadOnly;
            _editor.IsReadOnly = _isReadOnly;
            _statusText = $"readonly={_isReadOnly}";
            return null;
        }

        if (key.IsCharacter('i', ModifierKeys.Ctrl))
        {
            _isDisabled = !_isDisabled;
            _editor.IsDisabled = _isDisabled;
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
                Content = _editor,
                Width = Math.Min(64, Math.Max(42, context.Width - 4)),
                Height = 5,
            });
            window.Footer(1, _status);
        });
    }

    private void SeedTokens()
    {
        _editor.SetTokens(
        [
            new TokenItem("ops"),
            new TokenItem("locked", isDisabled: true),
            new TokenItem("pager"),
        ]);
    }

    private void ApplyTheme()
    {
        ThemeScope.Apply(DefaultTheme, _editor, _status);

        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        var baseToken = _styleAlt
            ? TeaStyle.Empty.WithBackground(AnsiColor.Rgb(42, 58, 79)).WithForeground(AnsiColor.Rgb(205, 214, 244))
            : TeaStyle.Empty.WithBackground(AnsiColor.Rgb(48, 52, 70)).WithForeground(AnsiColor.Rgb(166, 173, 200));
        var selected = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        var hovered = theme.Accent.Secondary.WithUnderline();
        var disabled = theme.Text.Muted.WithDim();
        var placeholder = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175)).WithItalic()
            : TeaStyle.Empty.WithForeground(AnsiColor.Rgb(147, 153, 178)).WithItalic();

        _editor.BorderStyleText = theme.Border.Strong;
        _editor.FocusedBorderStyleText = focusedBorder;
        _editor.TokenStyle = baseToken;
        _editor.SelectedTokenStyle = selected;
        _editor.FocusedSelectedTokenStyle = selected.WithUnderline();
        _editor.HoveredTokenStyle = hovered;
        _editor.DisabledTokenStyle = disabled;
        _editor.PlaceholderTextStyle = placeholder;
        _editor.Glyphs = _styleAlt
            ? new TokenEditorGlyphSet(
                selectedMarker: "▶",
                unselectedMarker: "·",
                tokenPrefix: "{",
                tokenSuffix: "}",
                markerSeparator: " ",
                tokenSeparator: "  ")
            : TokenEditorGlyphSet.Default;
    }

    private void UpdateFooter()
    {
        var selected = _editor.SelectedToken?.Value ?? "-";
        var status = _statusText;
        if (_isDisabled)
        {
            status = "disabled: typing and pointer selection are blocked";
        }
        else if (_isReadOnly)
        {
            status = "read-only: input and delete are blocked";
        }

        _status.LeftText =
            $"count={_editor.Tokens.Count} sel={selected} ro={_isReadOnly} dis={_isDisabled} tch={_tokenChangeCount} sch={_selectionChangeCount}";
        _status.RightText =
            $"{status} | type + Enter add Left/Right select Del/Backspace remove click token ^R seed ^E clear ^A add ^X remove ^T style ^O ro ^I dis ^C quit";
    }
}
