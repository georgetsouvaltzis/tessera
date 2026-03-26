using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class DataFormApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly DataForm<ServiceProfile> _form = new()
    {
        Title = "DataForm",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        MaxLabelWidth = 14,
        NoModelText = "(no profile bound)",
    };

    private readonly StatusBar _status = new();

    private ServiceProfile? _model;
    private bool _isReadOnly;
    private bool _isDisabled;
    private bool _styleAlt;
    private int _selectionChanges;
    private int _commitCount;
    private string _statusText = "widget-only proof: select row, Enter to edit, commit/cancel, keyed select, validation";

    public DataFormApp()
    {
        ConfigureFields();
        ResetModel();
        ApplyTheme();
        _form.RequestFocus();
        _form.SelectionChanged += (_, args) =>
        {
            _selectionChanges++;
            var previous = args.PreviousField?.Key ?? "-";
            var current = args.SelectedField?.Key ?? "-";
            _statusText = $"selection {previous}->{current}";
        };
        _form.FieldCommitted += (_, args) =>
        {
            _commitCount++;
            _statusText = args.Success
                ? $"commit {args.Field.Key}={args.CommittedValue}"
                : $"commit {args.Field.Key} failed: {args.Error}";
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
            ResetModel();
            _statusText = "api SetModel(seed): select rows, press Enter to edit";
            return null;
        }

        if (key.IsCharacter('u', ModifierKeys.Ctrl))
        {
            _form.ClearModel();
            _model = null;
            _statusText = "api ClearModel(): no-model placeholders visible";
            return null;
        }

        if (key.IsCharacter('g', ModifierKeys.Ctrl))
        {
            var changed = _form.SelectField("team");
            _statusText = $"api SelectField(team)={changed}";
            return null;
        }

        if (key.IsCharacter('k', ModifierKeys.Ctrl))
        {
            var changed = _form.SelectField("owner");
            _statusText = $"api SelectField(owner)={changed}";
            return null;
        }

        if (key.IsCharacter('o', ModifierKeys.Ctrl))
        {
            _isReadOnly = !_isReadOnly;
            _form.IsReadOnly = _isReadOnly;
            _statusText = $"readonly={_isReadOnly}";
            return null;
        }

        if (key.IsCharacter('i', ModifierKeys.Ctrl))
        {
            _isDisabled = !_isDisabled;
            _form.IsDisabled = _isDisabled;
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
                Content = _form,
                Width = Math.Min(72, Math.Max(50, context.Width - 4)),
                Height = Math.Min(12, Math.Max(9, context.Height - 4)),
            });
            window.Footer(1, _status);
        });
    }

    private void ConfigureFields()
    {
        _form.SetFields(
        [
            new DataFormField<ServiceProfile>(
                "name",
                "Service",
                model => model.Name,
                (model, value) => model.Name = value,
                placeholder: "service name",
                validator: value => value.Trim().Length >= 3 ? null : "min 3 chars"),
            new DataFormField<ServiceProfile>(
                "email",
                "Email",
                model => model.Email,
                (model, value) => model.Email = value,
                placeholder: "owner@team.dev",
                validator: value => value.Contains('@', StringComparison.Ordinal) ? null : "email must contain @"),
            new DataFormField<ServiceProfile>(
                "team",
                "Team",
                model => model.Team,
                (model, value) => model.Team = value,
                placeholder: "team name"),
            new DataFormField<ServiceProfile>(
                "owner",
                "Owner",
                model => model.Owner,
                placeholder: "read-only",
                isReadOnly: true),
        ]);
    }

    private void ResetModel()
    {
        _model = new ServiceProfile
        {
            Name = "billing-api",
            Email = "ops@tea.dev",
            Team = "platform",
            Owner = "george",
        };
        _form.SetModel(_model);
    }

    private void ApplyTheme()
    {
        ThemeScope.Apply(DefaultTheme, _form, _status);

        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        _form.TitleStyle = theme.Text.Primary;
        _form.FocusedTitleStyle = focusedBorder.WithBold();
        _form.BorderStyleText = theme.Border.Strong;
        _form.FocusedBorderStyleText = focusedBorder;
        _form.LabelStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175)).WithBold()
            : theme.Text.Primary;
        _form.ValueStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(205, 214, 244))
            : theme.Text.Primary;
        _form.PlaceholderStyle = theme.Text.Muted.WithItalic();
        _form.SelectedFieldStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        _form.FocusedSelectedFieldStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _form.HoveredFieldStyle = theme.Accent.Secondary.WithUnderline();
        _form.ReadOnlyFieldStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(166, 173, 200)).WithDim();
        _form.DisabledStyle = theme.Text.Muted.WithDim();
        _form.ErrorStyle = theme.State.Error.WithBold();
    }

    private void UpdateFooter()
    {
        var selected = _form.SelectedField?.Key ?? "-";
        var modelState = _model is null ? "model=none" : $"svc={_model.Name} email={_model.Email} team={_model.Team}";
        var status = _statusText;
        if (_isDisabled)
        {
            status = "disabled: form input and selection blocked";
        }
        else if (_isReadOnly)
        {
            status = "readonly: selection works, edits blocked";
        }

        _status.LeftText =
            $"field={selected} buf={_form.EditBuffer} commits={_commitCount} sch={_selectionChanges}";
        _status.RightText =
            $"{status} | {modelState} | select row Up/Down j/k or click, Enter edit, type, Enter commit, Esc cancel | ^R set-model ^U clear-model ^G team ^K owner ^T style ^O ro ^I dis ^C quit";
    }

    private sealed class ServiceProfile
    {
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;

        public string Owner { get; set; } = string.Empty;
    }
}
