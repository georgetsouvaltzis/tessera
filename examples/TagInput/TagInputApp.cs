using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class TagInputApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly TagInput _tagInput = new()
    {
        Title = "TagInput",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        TagPadding = 1,
        InputPadding = 1,
        FocusMarker = "◆",
        CaretGlyph = "▏",
        Placeholder = "type a tag, press comma or Enter",
    };

    private readonly StatusBar _status = new();

    private bool _allowDuplicates;
    private int _maxTags = 5;
    private bool _isReadOnly;
    private bool _isDisabled;
    private bool _styleAlt;
    private int _changedCount;
    private string _statusText = "filled chips, overflow window, caret, duplicate/max validation";

    public TagInputApp()
    {
        ApplyTagRules();
        ApplyTheme();
        _tagInput.RequestFocus();
        _tagInput.TagsChanged += (_, args) =>
        {
            _changedCount++;
            _statusText = $"tags {args.PreviousTags.Count}->{args.Tags.Count}";
            UpdateErrorFlag();
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
            _tagInput.SetTags(["alpha", "beta", "gamma", "delta", "release"]);
            _statusText = "seeded overflow tags";
            UpdateErrorFlag();
            return null;
        }

        if (key.IsCharacter('e', ModifierKeys.Ctrl))
        {
            _tagInput.SetTags(Array.Empty<string>());
            _statusText = "cleared";
            UpdateErrorFlag();
            return null;
        }

        if (key.IsCharacter('d', ModifierKeys.Ctrl))
        {
            _allowDuplicates = !_allowDuplicates;
            ApplyTagRules();
            _statusText = $"duplicates={_allowDuplicates}";
            return null;
        }

        if (key.IsCharacter('m', ModifierKeys.Ctrl))
        {
            _maxTags = _maxTags == 5 ? 3 : 5;
            ApplyTagRules();
            _statusText = $"max={_maxTags}";
            return null;
        }

        if (key.IsCharacter('o', ModifierKeys.Ctrl))
        {
            _isReadOnly = !_isReadOnly;
            _tagInput.IsReadOnly = _isReadOnly;
            _statusText = $"readonly={_isReadOnly}";
            return null;
        }

        if (key.IsCharacter('i', ModifierKeys.Ctrl))
        {
            _isDisabled = !_isDisabled;
            _tagInput.IsDisabled = _isDisabled;
            _statusText = $"disabled={_isDisabled}";
            return null;
        }

        if (key.IsCharacter('t', ModifierKeys.Ctrl))
        {
            _styleAlt = !_styleAlt;
            ApplyTheme();
            _statusText = _styleAlt ? "style=alt" : "style=default";
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
                Content = _tagInput,
                Width = Math.Min(48, Math.Max(36, context.Width - 4)),
            });
            window.Footer(1, _status);
        });
    }

    private void ApplyTheme()
    {
        ThemeScope.Apply(DefaultTheme, _tagInput, _status);

        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        var baseChip = _styleAlt
            ? TeaStyle.Empty.WithBackground(AnsiColor.Rgb(35, 51, 72)).WithForeground(AnsiColor.Rgb(197, 214, 240))
            : TeaStyle.Empty.WithBackground(AnsiColor.Rgb(48, 52, 70)).WithForeground(AnsiColor.Rgb(166, 173, 200));
        var selected = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        var hovered = theme.Accent.Secondary.WithUnderline();
        var placeholder = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(245, 194, 231)).WithItalic()
            : TeaStyle.Empty.WithForeground(AnsiColor.Rgb(147, 153, 178)).WithItalic();
        var caret = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175)).WithBold()
            : theme.Focus.Ring.WithBold();

        _tagInput.BorderStyleText = theme.Border.Strong;
        _tagInput.FocusedBorderStyleText = focusedBorder;
        _tagInput.TagStyle = baseChip;
        _tagInput.SelectedTagStyle = selected;
        _tagInput.FocusedTagStyle = selected.WithUnderline();
        _tagInput.HoveredTagStyle = hovered;
        _tagInput.DisabledTagStyle = theme.Text.Muted.WithDim();
        _tagInput.ErrorTagStyle = theme.State.Error.WithBold();
        _tagInput.PlaceholderTextStyle = placeholder;
        _tagInput.CaretStyle = caret;
        _tagInput.CaretGlyph = _styleAlt ? "▌" : "▏";
    }

    private void ApplyTagRules()
    {
        _tagInput.Options = new TagInputOptions(
            Separator: ',',
            AllowDuplicates: _allowDuplicates,
            CaseSensitive: false,
            MaxTags: _maxTags,
            ShowTagCount: true,
            TagPrefix: string.Empty,
            TagSuffix: string.Empty);

        UpdateErrorFlag();
    }

    private void UpdateErrorFlag()
    {
        _tagInput.HasError = HasValidationIssue();
    }

    private bool HasValidationIssue()
    {
        return ResolveValidationReason().Length > 0;
    }

    private string ResolveValidationReason()
    {
        if (_maxTags > 0 && _tagInput.Tags.Count >= _maxTags)
        {
            return $"limit reached: max {_maxTags}; delete a chip to add more";
        }

        var pending = _tagInput.InputValue.Trim();
        if (_allowDuplicates || pending.Length == 0)
        {
            return string.Empty;
        }

        return _tagInput.Tags.Any(tag => string.Equals(tag, pending, StringComparison.OrdinalIgnoreCase))
            ? "duplicate pending: commit will be ignored while duplicates are off"
            : string.Empty;
    }

    private void UpdateFooter()
    {
        UpdateErrorFlag();

        var selected = _tagInput.SelectedTagIndex >= 0 ? _tagInput.SelectedTag : "-";
        var reason = ResolveValidationReason();
        if (_isDisabled)
        {
            reason = "disabled: input and chip selection are blocked";
        }
        else if (_isReadOnly)
        {
            reason = "read-only: input is visible but edits are blocked";
        }
        else if (string.IsNullOrEmpty(reason))
        {
            reason = _statusText;
        }

        _status.LeftText =
            $"count={_tagInput.Tags.Count} sel={selected} dup={_allowDuplicates} max={_maxTags} ro={_isReadOnly} dis={_isDisabled} chg={_changedCount}";
        _status.RightText =
            $"{reason} | ^R seed ^E clear ^D dup ^M max ^O ro ^I dis ^T style ^C quit";
    }
}
