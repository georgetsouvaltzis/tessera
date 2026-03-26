using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class TagInputLabApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly Label _instructions = new()
    {
        Title = "TagInput Lab",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly TagInput _tagInput = new()
    {
        Title = "Tags",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "type tag, press comma or Enter",
    };

    private readonly EmptyState _emptyState = new()
    {
        Title = "No tags yet",
        Description = "Use keyboard or pointer to add tags.",
        Hint = "Try typing in TagInput then Enter",
        ActionText = "Seed Sample Tags",
        FocusMarker = "◆",
    };

    private readonly FieldSet _stateView = new()
    {
        Title = "TagInput State",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        SelectedMarker = "▶",
        UnselectedMarker = "·",
    };

    private readonly Notifications _events = new()
    {
        Title = "Changed Event Feed",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        MaxItems = 80,
    };

    private readonly Button _addApiTagButton = new() { Text = "Add API Tag", Description = "a", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _seedButton = new() { Text = "Seed Tags", Description = "Ctrl+R", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _clearButton = new() { Text = "Clear", Description = "Ctrl+E", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _rulesButton = new() { Text = "Toggle Rules", Description = "Ctrl+D/M", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _styleButton = new() { Text = "Toggle Style", Description = "Ctrl+T", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };

    private readonly StatusBar _status = new();

    private bool _allowDuplicates;
    private int _maxTags = 6;
    private bool _styleAlt;
    private int _changedCount;
    private int _apiTagCounter;
    private string _statusText = "Ready";

    public TagInputLabApp()
    {
        ApplyTagRules();
        WireEvents();
        ApplyTheme();
        _instructions.Text = BuildInstructionsText();
        _tagInput.RequestFocus();
        _events.Push("TagInput lab ready", NotificationLevel.Info);
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
            SeedTags();
            return null;
        }

        if (key.IsCharacter('e', ModifierKeys.Ctrl))
        {
            ClearTags();
            return null;
        }

        if (key.IsCharacter('d', ModifierKeys.Ctrl))
        {
            _allowDuplicates = !_allowDuplicates;
            ApplyTagRules();
            _statusText = $"allow duplicates -> {_allowDuplicates}";
            _events.Push(_statusText, NotificationLevel.Info);
            return null;
        }

        if (key.IsCharacter('m', ModifierKeys.Ctrl))
        {
            _maxTags = _maxTags == 6 ? 3 : 6;
            ApplyTagRules();
            _statusText = $"max tags -> {_maxTags}";
            _events.Push(_statusText, NotificationLevel.Info);
            return null;
        }

        if (key.IsCharacter('t', ModifierKeys.Ctrl))
        {
            _styleAlt = !_styleAlt;
            ApplyTheme();
            _statusText = _styleAlt ? "style -> alternate" : "style -> default";
            _events.Push(_statusText, NotificationLevel.Info);
            return null;
        }

        if (key.IsCharacter('a'))
        {
            AddApiTag();
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _instructions.Text = BuildInstructionsText();
        RefreshStateView();

        _status.LeftText =
            $"tags={_tagInput.Tags.Count} selected={_tagInput.SelectedTagIndex} changes={_changedCount} dup={_allowDuplicates} max={_maxTags}";
        _status.RightText = $"{_statusText}  Ctrl+R seed  Ctrl+E clear  Ctrl+D/M rules  Ctrl+T style  Ctrl+C quit";

        Control statePanel = _tagInput.Tags.Count == 0 ? _emptyState : _stateView;

        var actions = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _addApiTagButton, Length = 14 },
                new LayoutSlot { Content = _seedButton, Length = 13 },
                new LayoutSlot { Content = _clearButton, Length = 10 },
                new LayoutSlot { Content = _rulesButton, Length = 15 },
                new LayoutSlot { Content = _styleButton, Length = 14 },
            },
        };

        var top = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _instructions, Length = 8 },
                new LayoutSlot { Content = _tagInput, Length = 5 },
                new LayoutSlot { Content = actions, Length = 5 },
            },
        };

        var bottom = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = statePanel, Length = Math.Min(48, Math.Max(34, context.Width / 3)) },
                new LayoutSlot { Content = _events, Length = LayoutLength.Fill() },
            },
        };

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(19, top);
                column.Fill(bottom);
            }));
            window.Footer(1, _status);
        });
    }

    private void WireEvents()
    {
        _tagInput.TagsChanged += (_, args) =>
        {
            _changedCount++;
            var before = string.Join(", ", args.PreviousTags);
            var after = string.Join(", ", args.Tags);
            _statusText = $"TagsChanged #{_changedCount}: {args.PreviousTags.Count} -> {args.Tags.Count}";
            _events.Push($"{_statusText} | [{before}] => [{after}]", NotificationLevel.Success);
        };

        _emptyState.ActionInvoked += (_, _) => SeedTags();

        _addApiTagButton.Activated += (_, _) => AddApiTag();
        _seedButton.Activated += (_, _) => SeedTags();
        _clearButton.Activated += (_, _) => ClearTags();

        _rulesButton.Activated += (_, _) =>
        {
            _allowDuplicates = !_allowDuplicates;
            ApplyTagRules();
            _statusText = $"allow duplicates -> {_allowDuplicates}";
            _events.Push(_statusText, NotificationLevel.Info);
        };

        _styleButton.Activated += (_, _) =>
        {
            _styleAlt = !_styleAlt;
            ApplyTheme();
            _statusText = _styleAlt ? "style -> alternate" : "style -> default";
            _events.Push(_statusText, NotificationLevel.Info);
        };
    }

    private void ApplyTheme()
    {
        ThemeScope.Apply(
            DefaultTheme,
            _instructions,
            _tagInput,
            _emptyState,
            _stateView,
            _events,
            _addApiTagButton,
            _seedButton,
            _clearButton,
            _rulesButton,
            _styleButton,
            _status);

        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        var selected = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        var accent = theme.Accent.Secondary.WithUnderline();

        _tagInput.BorderStyleText = theme.Border.Strong;
        _tagInput.FocusedBorderStyleText = focusedBorder;
        _tagInput.TagStyle = _styleAlt ? theme.State.Info : theme.Accent.Primary;
        _tagInput.SelectedTagStyle = selected;
        _tagInput.FocusedTagStyle = selected;
        _tagInput.HoveredTagStyle = accent;
        _tagInput.ErrorTagStyle = theme.State.Error.WithBold();
        _tagInput.HasError = _tagInput.Tags.Count >= _maxTags;

        _emptyState.DefaultStyle = theme.Text.Secondary;
        _emptyState.FocusedStyle = selected;
        _emptyState.ActionStyle = theme.Accent.Primary.WithBold();
        _emptyState.FocusedActionStyle = selected;

        _stateView.BorderStyleText = theme.Border.Strong;
        _stateView.FocusedBorderStyleText = focusedBorder;
        _stateView.SelectedItemStyle = selected;
        _stateView.HoveredItemStyle = accent;

        _events.BorderStyleText = theme.Border.Strong;
        _events.FocusedBorderStyleText = focusedBorder;
        _events.SelectedItemStyle = selected;
        _events.HoveredItemStyle = accent;
        _events.WarningItemStyle = theme.State.Warning.WithBold();

        _instructions.BorderStyleText = theme.Border.Strong;
        _instructions.TitleStyle = theme.Accent.Primary.WithBold();

        var buttonStyle = _styleAlt ? theme.State.Info.WithBold() : theme.Accent.Primary.WithBold();
        _addApiTagButton.LabelStyle = buttonStyle;
        _seedButton.LabelStyle = buttonStyle;
        _clearButton.LabelStyle = buttonStyle;
        _rulesButton.LabelStyle = buttonStyle;
        _styleButton.LabelStyle = buttonStyle;
    }

    private void ApplyTagRules()
    {
        _tagInput.Options = new TagInputOptions(
            Separator: ',',
            AllowDuplicates: _allowDuplicates,
            CaseSensitive: false,
            MaxTags: _maxTags,
            ShowTagCount: true,
            TagPrefix: "#",
            TagSuffix: string.Empty);

        _tagInput.HasError = _tagInput.Tags.Count >= _maxTags;
    }

    private void AddApiTag()
    {
        _apiTagCounter++;
        var tag = $"api-{_apiTagCounter:00}";
        var changed = _tagInput.AddTag(tag);
        _statusText = changed ? $"AddTag('{tag}') applied" : $"AddTag('{tag}') rejected by rules";
        _events.Push(_statusText, changed ? NotificationLevel.Success : NotificationLevel.Warning);
        _tagInput.HasError = _tagInput.Tags.Count >= _maxTags;
    }

    private void SeedTags()
    {
        _tagInput.SetTags(["alpha", "beta", "beta", "  ", "prod-ready", "alpha"]);
        _statusText = "SeedTags applied with duplicate/blank edge inputs";
        _events.Push(_statusText, NotificationLevel.Info);
        _tagInput.HasError = _tagInput.Tags.Count >= _maxTags;
    }

    private void ClearTags()
    {
        _tagInput.SetTags(Array.Empty<string>());
        _statusText = "All tags cleared";
        _events.Push(_statusText, NotificationLevel.Info);
        _tagInput.HasError = false;
    }

    private void RefreshStateView()
    {
        var selectedTag = string.IsNullOrWhiteSpace(_tagInput.SelectedTag) ? "(none)" : _tagInput.SelectedTag;
        _stateView.SetItems(
        [
            $"Tag count: {_tagInput.Tags.Count}",
            $"Selected index: {_tagInput.SelectedTagIndex}",
            $"Selected tag: {selectedTag}",
            $"Input value: {_tagInput.InputValue}",
            $"Allow duplicates: {_allowDuplicates}",
            $"Max tags: {_maxTags}",
            $"Has error flag: {_tagInput.HasError}",
            "Pointer: click tag chips to select",
            "Keyboard: Enter/comma add, arrows select, backspace/delete remove",
        ]);
    }

    private string BuildInstructionsText()
    {
        return
            "Focus TagInput and try these:\n"
            + "1) Type text + Enter/comma to add tags\n"
            + "2) Left/Right to move selection, Backspace/Delete to remove\n"
            + "3) Click chips with pointer to test selection/focus\n"
            + "4) Ctrl+R seed edge values (duplicates + blanks)\n"
            + "5) Ctrl+D toggle duplicate rule, Ctrl+M toggle max tags\n"
            + "6) Watch footer + event feed for TagsChanged payload";
    }
}
