using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class ConsumerWorkflowLabApp : TeaApp
{
    internal static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private static readonly TeaTheme AlternateTheme = TeaThemes.RosePine(RosePineVariant.Moon);

    private static readonly string[] WorkflowSteps = ["Intake", "Validation", "Approval", "Execute"];

    private static readonly string[] EnvironmentOptions = ["Development", "Staging", "Production", "Canary"];

    private static readonly string[] TemplateOptions = ["Blank", "Blue/Green API", "Batch Worker", "Emergency Rollback"];

    private readonly Dictionary<string, WorkflowTemplate> _templates = new(StringComparer.Ordinal)
    {
        ["Blank"] = new WorkflowTemplate("Blank", "Development", "us-east-1", "09:00-11:00", 20, "Use previous stable image.", "CHG-000000"),
        ["Blue/Green API"] = new WorkflowTemplate("Blue/Green API", "Production", "us-east-1", "20:00-22:00", 15, "Switch traffic to green pool; keep blue warm.", "CHG-184512"),
        ["Batch Worker"] = new WorkflowTemplate("Batch Worker", "Staging", "eu-west-1", "10:30-12:00", 50, "Pause queue; resume after health checks.", "CHG-219004"),
        ["Emergency Rollback"] = new WorkflowTemplate("Emergency Rollback", "Canary", "us-west-2", "00:15-01:00", 5, "Pin previous artifact and freeze deploys.", "CHG-911000"),
    };

    private readonly WorkflowDraft _draft = WorkflowDraft.CreateDefault();

    private readonly Stepper _stepper = new()
    {
        Title = "Release Workflow",
        FocusMarker = "◆",
        Connector = " -> ",
    };

    private readonly Wizard _wizard = new()
    {
        Title = "Step Detail",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        ActiveMarker = "▶",
        CompletedMarker = "✓",
        PendingMarker = "·",
    };

    private readonly DataForm<WorkflowDraft> _dataForm = new()
    {
        Title = "Change Request",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        SelectedMarker = "▶",
        UnselectedMarker = "·",
        FieldSeparatorText = " : ",
        MaxLabelWidth = 18,
    };

    private readonly ValidationSummary _validationSummary = new()
    {
        Title = "Validation Summary",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Form _reviewForm = new()
    {
        Title = "Review Snapshot",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        SelectedMarker = "▶",
        UnselectedMarker = "·",
    };

    private readonly FieldSet _policyFieldSet = new()
    {
        Title = "Policy Gates",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        SelectedMarker = "▶",
        UnselectedMarker = "·",
    };

    private readonly Choice _environmentChoice = new()
    {
        Title = "Environment",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly ComboBox _templateCombo = new()
    {
        Title = "Template",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "Type to filter templates",
    };

    private readonly TextInput _ticketInput = new()
    {
        Title = "Change Ticket",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "CHG-123456",
    };

    private readonly TextInput _approverInput = new()
    {
        Title = "Approver",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "on-call approver",
    };

    private readonly Button _validateButton = new()
    {
        Text = "Validate",
        Description = "Ctrl+V",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Button _previousButton = new()
    {
        Text = "Back",
        Description = "Ctrl+B",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Button _nextButton = new()
    {
        Text = "Next",
        Description = "Ctrl+N",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Button _submitButton = new()
    {
        Text = "Submit",
        Description = "d",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Button _stressButton = new()
    {
        Text = "Stress Selection",
        Description = "Ctrl+S",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly Label _summary = new()
    {
        Title = "Current Draft",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly ListView<string> _selectionTrace = new(static line => line)
    {
        Title = "Selection Trace",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly LogView _activityLog = new()
    {
        Title = "Activity",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Dialog _submitDialog = new()
    {
        Title = "Submit Workflow",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();
    private readonly Dictionary<string, int> _fieldIndexByKey = new(StringComparer.Ordinal);
    private readonly List<string> _selectionTraceEntries = [];

    private bool _useAlternateTheme;
    private bool _syncingStepSelection;
    private int _validationRunCount;
    private int _choiceStressIndex;
    private int _comboStressIndex;
    private int _dataFormStressIndex;
    private string _approver = "platform-oncall";
    private string _statusText = "Ready";

    public ConsumerWorkflowLabApp()
    {
        ConfigureWorkflowSteps();
        ConfigureDataFormFields();
        WireInteractions();
        InitializeDefaults();
        ApplyThemeAndOverrides();
        RunValidation("startup");
        AppendActivity("Consumer workflow lab initialized.");
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

        if (key.IsCharacter('t', ModifierKeys.Ctrl))
        {
            _useAlternateTheme = !_useAlternateTheme;
            ApplyThemeAndOverrides();
            _statusText = $"theme switched -> {ThemeLabel()}";
            AppendActivity(_statusText);
            return null;
        }

        if (key.IsCharacter('v', ModifierKeys.Ctrl))
        {
            RunValidation("Ctrl+V");
            return null;
        }

        if (key.IsCharacter('s', ModifierKeys.Ctrl))
        {
            RunSelectionStressPass("Ctrl+S");
            return null;
        }

        if (key.IsCharacter('n', ModifierKeys.Ctrl))
        {
            AdvanceStep();
            return null;
        }

        if (key.IsCharacter('b', ModifierKeys.Ctrl))
        {
            RetreatStep();
            return null;
        }

        if (key.IsCharacter('1', ModifierKeys.Ctrl))
        {
            CycleChoiceSelection("Ctrl+1");
            return null;
        }

        if (key.IsCharacter('2', ModifierKeys.Ctrl))
        {
            CycleComboSelection("Ctrl+2");
            return null;
        }

        if (key.IsCharacter('3', ModifierKeys.Ctrl))
        {
            CycleDataFormSelection("Ctrl+3");
            return null;
        }

        if (key.IsCharacter('d'))
        {
            AttemptSubmit();
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshDerivedViews();

        _status.LeftText =
            $"step={CurrentStepLabel()}  env={_draft.Environment}  template={TemplateOrUnset()}  validation_runs={_validationRunCount}";
        _status.RightText =
            $"{_statusText}  Ctrl+1 choice  Ctrl+2 combo  Ctrl+3 dataform  Ctrl+S stress  Ctrl+T theme  Ctrl+C quit";

        var selectorRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _environmentChoice,
                    Length = 30,
                },
                new LayoutSlot
                {
                    Content = _templateCombo,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var metadataRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _ticketInput,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _approverInput,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var actionRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _validateButton, Length = 15 },
                new LayoutSlot { Content = _previousButton, Length = 13 },
                new LayoutSlot { Content = _nextButton, Length = 13 },
                new LayoutSlot { Content = _submitButton, Length = 13 },
                new LayoutSlot { Content = _stressButton, Length = 23 },
            },
        };

        var topPanel = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _summary, Length = 6 },
                new LayoutSlot { Content = selectorRow, Length = 6 },
                new LayoutSlot { Content = metadataRow, Length = 5 },
                new LayoutSlot { Content = actionRow, Length = 5 },
            },
        };

        var editAndValidationRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _dataForm, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _validationSummary, Length = Math.Min(42, Math.Max(34, context.Width / 3)) },
            },
        };

        var reviewAndPoliciesRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _reviewForm, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _policyFieldSet, Length = Math.Min(44, Math.Max(36, context.Width / 3)) },
            },
        };

        var activityRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _selectionTrace, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _activityLog, Length = LayoutLength.Fill() },
            },
        };

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(2, _stepper);
            window.Left(Math.Min(36, Math.Max(29, context.Width / 4)), _wizard);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(25, topPanel);
                column.Fixed(14, editAndValidationRow);
                column.Fixed(12, reviewAndPoliciesRow);
                column.Fill(activityRow);
            }));
            window.Footer(1, _status);

            if (_submitDialog.IsVisible)
            {
                window.Overlay(new CenterLayout
                {
                    Content = _submitDialog,
                    Width = Math.Min(76, Math.Max(52, context.Width - 8)),
                    Height = 10,
                });
            }
        });
    }

    private void ConfigureWorkflowSteps()
    {
        _stepper.SetSteps(WorkflowSteps.Select(static step => new StepperStep(step.ToLowerInvariant(), step)));
        _wizard.SetSteps(WorkflowSteps.Select(static step => new WizardStep(step.ToLowerInvariant(), step, $"{step} checks")));

        if (_stepper.Steps.Count > 0)
        {
            _stepper.SetCurrentStep(0);
        }

        if (_wizard.Steps.Count > 0)
        {
            _wizard.SelectStep(0);
        }
    }

    private void ConfigureDataFormFields()
    {
        RegisterDataField(
            "service",
            "Service",
            static model => model.Service,
            static (model, value) => model.Service = value,
            placeholder: "checkout-api",
            validator: static value => value.Trim().Length >= 3 ? null : "Service name must be at least 3 chars.");

        RegisterDataField(
            "ownerEmail",
            "Owner Email",
            static model => model.OwnerEmail,
            static (model, value) => model.OwnerEmail = value,
            placeholder: "owner@company.com",
            validator: static value => value.Contains('@') ? null : "Owner email must contain '@'.");

        RegisterDataField(
            "region",
            "Target Region",
            static model => model.TargetRegion,
            static (model, value) => model.TargetRegion = value,
            placeholder: "us-east-1",
            validator: static value => value.Contains('-', StringComparison.Ordinal) ? null : "Region format should include '-'.");

        RegisterDataField(
            "window",
            "Change Window",
            static model => model.ChangeWindow,
            static (model, value) => model.ChangeWindow = value,
            placeholder: "20:00-22:00",
            validator: static value => LooksLikeWindow(value) ? null : "Expected HH:MM-HH:MM.");

        RegisterDataField(
            "rollout",
            "Rollout (%)",
            static model => model.RolloutPercent.ToString(CultureInfo.InvariantCulture),
            static (model, value) => model.RolloutPercent = ParseRollout(value),
            placeholder: "25",
            validator: static value =>
                int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed) && parsed is >= 1 and <= 100
                    ? null
                    : "Rollout must be an integer 1..100.");

        RegisterDataField(
            "rollback",
            "Rollback Plan",
            static model => model.RollbackPlan,
            static (model, value) => model.RollbackPlan = value,
            placeholder: "Describe rollback sequence",
            validator: static value => value.Trim().Length >= 12 ? null : "Rollback plan must be descriptive.");

        RegisterDataField(
            "ticket",
            "Ticket",
            static model => model.ChangeTicket,
            static (model, value) => model.ChangeTicket = value,
            placeholder: "CHG-123456",
            validator: static value => value.StartsWith("CHG-", StringComparison.OrdinalIgnoreCase) ? null : "Ticket must start with CHG-.");

        _dataForm.SetModel(_draft);
    }

    private void RegisterDataField(
        string key,
        string label,
        Func<WorkflowDraft, string> readValue,
        Action<WorkflowDraft, string> writeValue,
        string placeholder,
        Func<string, string?> validator)
    {
        _fieldIndexByKey[key] = _fieldIndexByKey.Count;
        _dataForm.RegisterField(key, label, readValue, writeValue, placeholder: placeholder, validator: validator);
    }

    private void WireInteractions()
    {
        _stepper.SelectionChanged += (_, args) => SynchronizeStepSelection(args.SelectedIndex, "stepper");
        _wizard.SelectionChanged += (_, args) => SynchronizeStepSelection(args.SelectedIndex, "wizard");

        _dataForm.SelectionChanged += (_, args) =>
        {
            if (args.SelectedField is null)
            {
                return;
            }

            _statusText = $"data form selected -> {args.SelectedField.Label}";
            AppendSelectionTrace(_statusText);
        };

        _dataForm.FieldCommitted += (_, args) =>
        {
            if (args.Success)
            {
                _statusText = $"committed {args.Field.Label}";
                AppendActivity($"DataForm commit succeeded for {args.Field.Key}.");
            }
            else
            {
                _statusText = $"commit failed -> {args.Field.Label}";
                AppendActivity($"DataForm commit failed for {args.Field.Key}: {args.Error}");
            }

            RunValidation("data-form-commit");
        };

        _environmentChoice.SelectionChanged += (_, args) =>
        {
            _draft.Environment = args.SelectedItem;
            AppendSelectionTrace($"choice selected -> {args.SelectedItem}");
            _statusText = $"environment -> {args.SelectedItem}";
            RunValidation("environment-changed");
        };

        _templateCombo.SelectionChanged += (_, args) =>
        {
            if (string.IsNullOrWhiteSpace(args.SelectedItem))
            {
                return;
            }

            ApplyTemplate(args.SelectedItem, reason: "combo-selection");
        };

        _ticketInput.Submitted += (_, args) =>
        {
            _draft.ChangeTicket = args.Value.Trim();
            _dataForm.SetModel(_draft);
            _statusText = $"ticket updated -> {_draft.ChangeTicket}";
            AppendActivity(_statusText);
            RunValidation("ticket-submitted");
        };

        _approverInput.Submitted += (_, args) =>
        {
            _approver = args.Value.Trim();
            _statusText = $"approver updated -> {_approver}";
            AppendActivity(_statusText);
            RunValidation("approver-submitted");
        };

        _validationSummary.SelectionChanged += (_, args) =>
        {
            if (args.SelectedIssue is null || string.IsNullOrWhiteSpace(args.SelectedIssue.Field))
            {
                return;
            }

            var focused = RouteIssueSelection(args.SelectedIssue.Field);
            AppendSelectionTrace(
                focused
                    ? $"validation routed -> {args.SelectedIssue.Field}"
                    : $"validation route missing -> {args.SelectedIssue.Field}");
        };

        _validateButton.Activated += (_, _) => RunValidation("button-validate");
        _previousButton.Activated += (_, _) => RetreatStep();
        _nextButton.Activated += (_, _) => AdvanceStep();
        _submitButton.Activated += (_, _) => AttemptSubmit();
        _stressButton.Activated += (_, _) => RunSelectionStressPass("button-stress");

        _submitDialog.Closed += (_, args) => HandleSubmitDialogClosed(args.Result);
    }

    private void InitializeDefaults()
    {
        _environmentChoice.SetItems(EnvironmentOptions);
        _templateCombo.SetItems(TemplateOptions);
        _ticketInput.SetValue(_draft.ChangeTicket);
        _approverInput.SetValue(_approver);

        _selectionTrace.SetItems(Array.Empty<string>());
        _wizard.RequestFocus();

        _ = ForceChoiceSelection(_draft.Environment);
        _ = ForceComboSelection("Blank");
    }

    private void RefreshDerivedViews()
    {
        _summary.Text =
            $"""
             Service: {_draft.Service}
             Owner: {_draft.OwnerEmail}
             Environment: {_draft.Environment}
             Ticket: {_draft.ChangeTicket}
             Approver: {_approver}
             """;

        _reviewForm.SetFields(
        [
            new FormField("service", "Service", _draft.Service, isRequired: true),
            new FormField("owner", "Owner", _draft.OwnerEmail, isRequired: true),
            new FormField("env", "Environment", _draft.Environment, isRequired: true),
            new FormField("region", "Target Region", _draft.TargetRegion),
            new FormField("window", "Window", _draft.ChangeWindow),
            new FormField("rollout", "Rollout", $"{_draft.RolloutPercent}%"),
            new FormField("ticket", "Ticket", _draft.ChangeTicket, helperText: "submitted via TextInput and DataForm"),
            new FormField("approver", "Approver", _approver, isRequired: true),
        ]);

        var errorCount = _validationSummary.Issues.Count(static issue => issue.Severity == ValidationSeverity.Error);
        var warningCount = _validationSummary.Issues.Count(static issue => issue.Severity == ValidationSeverity.Warning);
        _policyFieldSet.SetItems(
        [
            $"Current step: {CurrentStepLabel()}",
            $"Validation errors: {errorCount}",
            $"Validation warnings: {warningCount}",
            $"Approver set: {(!string.IsNullOrWhiteSpace(_approver) ? "yes" : "no")}",
            $"Ticket format ok: {(_draft.ChangeTicket.StartsWith("CHG-", StringComparison.OrdinalIgnoreCase) ? "yes" : "no")}",
            $"Choice selection API runs: {_choiceStressIndex}",
            $"ComboBox selection API runs: {_comboStressIndex}",
            $"DataForm key-map jumps: {_dataFormStressIndex}",
        ]);

        _selectionTrace.SetItems(_selectionTraceEntries);
    }

    private void ApplyThemeAndOverrides()
    {
        var theme = _useAlternateTheme ? AlternateTheme : DefaultTheme;
        ThemeScope.Apply(
            theme,
            _stepper,
            _wizard,
            _dataForm,
            _validationSummary,
            _reviewForm,
            _policyFieldSet,
            _environmentChoice,
            _templateCombo,
            _ticketInput,
            _approverInput,
            _validateButton,
            _previousButton,
            _nextButton,
            _submitButton,
            _stressButton,
            _summary,
            _selectionTrace,
            _activityLog,
            _submitDialog,
            _status);

        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        var selected = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();

        _stepper.FocusMarker = "◆";
        _stepper.TitleStyle = theme.Accent.Primary.WithBold();
        _stepper.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _stepper.ActiveStepStyle = selected;
        _stepper.CompletedStepStyle = theme.State.Success.WithBold();
        _stepper.PendingStepStyle = theme.Text.Secondary;

        _wizard.FocusMarker = "◆";
        _wizard.TitleStyle = theme.Accent.Primary.WithBold();
        _wizard.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _wizard.BorderStyleText = theme.Border.Strong;
        _wizard.FocusedBorderStyleText = focusedBorder;
        _wizard.ActiveStepStyle = selected;
        _wizard.FocusedActiveStepStyle = selected;
        _wizard.CompletedStepStyle = theme.State.Success.WithBold();
        _wizard.PendingStepStyle = theme.Text.Secondary;
        _wizard.HoveredStepStyle = theme.Accent.Secondary.WithUnderline();

        _dataForm.FocusMarker = "◆";
        _dataForm.BorderStyleText = theme.Border.Strong;
        _dataForm.FocusedBorderStyleText = focusedBorder;
        _dataForm.TitleStyle = theme.Accent.Primary.WithBold();
        _dataForm.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _dataForm.LabelStyle = theme.Text.Secondary;
        _dataForm.ValueStyle = theme.Text.Primary;
        _dataForm.PlaceholderStyle = theme.Text.Muted.WithItalic();
        _dataForm.SelectedFieldStyle = selected;
        _dataForm.FocusedSelectedFieldStyle = selected;
        _dataForm.HoveredFieldStyle = theme.Accent.Secondary.WithUnderline();
        _dataForm.ErrorStyle = theme.State.Error.WithBold();

        _validationSummary.FocusMarker = "◆";
        _validationSummary.BorderStyleText = theme.Border.Strong;
        _validationSummary.FocusedBorderStyleText = focusedBorder;
        _validationSummary.TitleStyle = theme.Accent.Primary.WithBold();
        _validationSummary.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _validationSummary.SelectedIssueStyle = selected;
        _validationSummary.FocusedIssueStyle = selected;
        _validationSummary.HoveredIssueStyle = theme.Accent.Secondary.WithUnderline();
        _validationSummary.ErrorSeverityStyle = theme.State.Error.WithBold();
        _validationSummary.WarningSeverityStyle = theme.State.Warning.WithBold();
        _validationSummary.InfoSeverityStyle = theme.State.Info;

        _reviewForm.FocusMarker = "◆";
        _reviewForm.BorderStyleText = theme.Border.Strong;
        _reviewForm.FocusedBorderStyleText = focusedBorder;
        _reviewForm.TitleStyle = theme.Accent.Primary.WithBold();
        _reviewForm.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _reviewForm.SelectedRowStyle = selected;
        _reviewForm.FocusedSelectedRowStyle = selected;
        _reviewForm.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();
        _reviewForm.RequiredMarkerStyle = theme.State.Warning.WithBold();

        _policyFieldSet.FocusMarker = "◆";
        _policyFieldSet.BorderStyleText = theme.Border.Strong;
        _policyFieldSet.FocusedBorderStyleText = focusedBorder;
        _policyFieldSet.TitleStyle = theme.Accent.Primary.WithBold();
        _policyFieldSet.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _policyFieldSet.SelectedItemStyle = selected;
        _policyFieldSet.FocusedSelectedItemStyle = selected;
        _policyFieldSet.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();

        _environmentChoice.FocusMarker = "◆";
        _environmentChoice.Glyphs = new DropdownGlyphSet("⌄", "⌃", "▸", "◆");
        _environmentChoice.BorderStyleText = theme.Border.Strong;
        _environmentChoice.FocusedBorderStyleText = focusedBorder;
        _environmentChoice.TitleStyle = theme.Accent.Primary.WithBold();
        _environmentChoice.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _environmentChoice.ValueStyle = theme.Text.Primary.WithBold();
        _environmentChoice.SelectedOptionStyle = selected;
        _environmentChoice.HoveredOptionStyle = theme.Accent.Secondary.WithUnderline();

        _templateCombo.FocusMarker = "◆";
        _templateCombo.Glyphs = new DropdownGlyphSet("⌄", "⌃", "▸", "◆");
        _templateCombo.BorderStyleText = theme.Border.Strong;
        _templateCombo.FocusedBorderStyleText = focusedBorder;
        _templateCombo.TitleStyle = theme.Accent.Primary.WithBold();
        _templateCombo.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _templateCombo.ValueTextStyle = theme.Text.Primary.WithBold();
        _templateCombo.PlaceholderTextStyle = theme.Text.Muted.WithItalic();
        _templateCombo.SelectedOptionStyle = selected;
        _templateCombo.HoveredOptionStyle = theme.Accent.Secondary.WithUnderline();

        _summary.TextStyle = theme.Text.Primary;
        _summary.TitleStyle = theme.Accent.Primary.WithBold();
        _summary.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _summary.BorderStyleText = theme.Border.Strong;

        _selectionTrace.FocusMarker = "◆";
        _selectionTrace.BorderStyleText = theme.Border.Strong;
        _selectionTrace.FocusedBorderStyleText = focusedBorder;
        _selectionTrace.TitleStyle = theme.Accent.Primary.WithBold();
        _selectionTrace.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _selectionTrace.SelectedRowStyle = selected;
        _selectionTrace.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();

        _activityLog.FocusMarker = "◆";
        _activityLog.BorderStyleText = theme.Border.Strong;
        _activityLog.FocusedBorderStyleText = focusedBorder;
        _activityLog.TitleStyle = theme.Accent.Primary.WithBold();
        _activityLog.FocusedTitleStyle = theme.Focus.Title.WithBold();
        _activityLog.EntryStyle = theme.Text.Secondary;

        _status.LeftTextStyle = theme.Text.Secondary.WithBold();
        _status.RightTextStyle = theme.Accent.Secondary;
        _status.FillStyle = theme.Surface.Panel.Merge(theme.Text.Muted);
    }

    private void RunValidation(string reason)
    {
        _validationRunCount++;
        var issues = BuildValidationIssues();
        _validationSummary.SetIssues(issues);

        var errors = issues.Count(static issue => issue.Severity == ValidationSeverity.Error);
        var warnings = issues.Count(static issue => issue.Severity == ValidationSeverity.Warning);
        _statusText = errors == 0
            ? $"validation pass ({warnings} warning(s)) [{reason}]"
            : $"validation failed ({errors} error(s)) [{reason}]";

        if (errors == 0)
        {
            var current = _wizard.SelectedIndex;
            if (current >= 0)
            {
                _wizard.SetStepCompleted(current, isCompleted: true);
                _stepper.SetStepCompleted(current, isCompleted: true);
            }
        }

        AppendActivity(_statusText);
    }

    private List<ValidationIssue> BuildValidationIssues()
    {
        var issues = new List<ValidationIssue>();

        if (_draft.Service.Trim().Length < 3)
        {
            issues.Add(new ValidationIssue("Service name must have at least 3 characters.", ValidationSeverity.Error, "service"));
        }

        if (!_draft.OwnerEmail.Contains('@', StringComparison.Ordinal))
        {
            issues.Add(new ValidationIssue("Owner email must contain '@'.", ValidationSeverity.Error, "ownerEmail"));
        }

        if (!LooksLikeWindow(_draft.ChangeWindow))
        {
            issues.Add(new ValidationIssue("Change window must follow HH:MM-HH:MM.", ValidationSeverity.Error, "window"));
        }

        if (_draft.RolloutPercent is < 1 or > 100)
        {
            issues.Add(new ValidationIssue("Rollout must be between 1 and 100.", ValidationSeverity.Error, "rollout"));
        }

        if (_draft.RollbackPlan.Trim().Length < 12)
        {
            issues.Add(new ValidationIssue("Rollback plan should be at least 12 characters.", ValidationSeverity.Error, "rollback"));
        }

        if (!_draft.ChangeTicket.StartsWith("CHG-", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ValidationIssue("Ticket should start with CHG-.", ValidationSeverity.Error, "ticket"));
        }

        if (string.IsNullOrWhiteSpace(_approver))
        {
            issues.Add(new ValidationIssue("Approver is required before submission.", ValidationSeverity.Error, "approverInput"));
        }

        if (string.IsNullOrWhiteSpace(_environmentChoice.SelectedItem))
        {
            issues.Add(new ValidationIssue("Environment selection is required.", ValidationSeverity.Error, "environment"));
        }

        if (string.Equals(_environmentChoice.SelectedItem, "Production", StringComparison.Ordinal)
            && _draft.RolloutPercent > 25)
        {
            issues.Add(new ValidationIssue("Production rollout above 25% should have explicit approval.", ValidationSeverity.Warning, "rollout"));
        }

        if (string.IsNullOrWhiteSpace(_templateCombo.SelectedItem))
        {
            issues.Add(new ValidationIssue("No template selected; flow will use manual defaults.", ValidationSeverity.Info, "template"));
        }

        return issues;
    }

    private void AdvanceStep()
    {
        var current = _wizard.SelectedIndex;
        if (current < 0)
        {
            return;
        }

        _wizard.SetStepCompleted(current, isCompleted: true);
        _stepper.SetStepCompleted(current, isCompleted: true);
        SetWorkflowStep(current + 1, "next");
    }

    private void RetreatStep()
    {
        var current = _wizard.SelectedIndex;
        if (current < 0)
        {
            return;
        }

        SetWorkflowStep(current - 1, "back");
    }

    private void SetWorkflowStep(int requestedIndex, string source)
    {
        var bounded = Math.Clamp(requestedIndex, 0, Math.Max(0, _wizard.Steps.Count - 1));
        _syncingStepSelection = true;
        _stepper.SetCurrentStep(bounded);
        _wizard.SelectStep(bounded);
        _syncingStepSelection = false;
        _statusText = $"step -> {CurrentStepLabel()} ({source})";
        AppendSelectionTrace(_statusText);
    }

    private void SynchronizeStepSelection(int selectedIndex, string source)
    {
        if (_syncingStepSelection || selectedIndex < 0)
        {
            return;
        }

        _syncingStepSelection = true;
        _stepper.SetCurrentStep(selectedIndex);
        _wizard.SelectStep(selectedIndex);
        _syncingStepSelection = false;

        _statusText = $"step sync from {source} -> {CurrentStepLabel()}";
    }

    private void AttemptSubmit()
    {
        RunValidation("submit-attempt");
        var errors = _validationSummary.Issues.Count(static issue => issue.Severity == ValidationSeverity.Error);
        if (errors > 0)
        {
            _statusText = "cannot submit until validation errors are fixed";
            AppendActivity(_statusText);
            return;
        }

        if (_wizard.SelectedIndex != _wizard.Steps.Count - 1)
        {
            _statusText = "navigate to final step before submitting";
            AppendActivity(_statusText);
            return;
        }

        _submitDialog.Show(
            "Submit release package?",
            $"Service: {_draft.Service}",
            $"Environment: {_draft.Environment}",
            $"Approver: {_approver}",
            "Enter confirms, Esc cancels.");
    }

    private void HandleSubmitDialogClosed(DialogResult result)
    {
        if (result == DialogResult.Accepted)
        {
            _statusText = $"submission accepted for {_draft.Service}";
            AppendActivity($"Submitted workflow for {_draft.Service} ({_draft.ChangeTicket}).");
            return;
        }

        if (result == DialogResult.Dismissed)
        {
            _statusText = "submission canceled";
            AppendActivity("Submission canceled by user.");
        }
    }

    private void RunSelectionStressPass(string source)
    {
        for (var i = 0; i < 3; i++)
        {
            CycleChoiceSelection(source);
            CycleComboSelection(source);
            CycleDataFormSelection(source);
        }

        RunValidation($"{source}-post-selection");

        for (var index = 0; index < _validationSummary.Issues.Count; index++)
        {
            _validationSummary.SetSelectedIndex(index);
        }

        AppendActivity($"selection stress pass finished via {source}");
    }

    private void CycleChoiceSelection(string source)
    {
        var target = EnvironmentOptions[_choiceStressIndex % EnvironmentOptions.Length];
        _choiceStressIndex++;
        var success = ForceChoiceSelection(target);
        AppendSelectionTrace($"choice selection API ({source}) -> {target} ({(success ? "ok" : "failed")})");
    }

    private void CycleComboSelection(string source)
    {
        var target = TemplateOptions[_comboStressIndex % TemplateOptions.Length];
        _comboStressIndex++;
        var success = ForceComboSelection(target);
        AppendSelectionTrace($"combo selection API ({source}) -> {target} ({(success ? "ok" : "failed")})");
    }

    private void CycleDataFormSelection(string source)
    {
        var keys = _fieldIndexByKey.Keys.OrderBy(static value => value, StringComparer.Ordinal).ToArray();
        if (keys.Length == 0)
        {
            return;
        }

        var targetKey = keys[_dataFormStressIndex % keys.Length];
        _dataFormStressIndex++;
        var success = SelectDataFormFieldByKey(targetKey);
        AppendSelectionTrace($"dataform key-map ({source}) -> {targetKey} ({(success ? "ok" : "failed")})");
    }

    private bool ForceChoiceSelection(string target)
    {
        if (!EnvironmentOptions.Contains(target, StringComparer.Ordinal))
        {
            return false;
        }

        if (string.Equals(_environmentChoice.SelectedItem, target, StringComparison.Ordinal))
        {
            return true;
        }

        return _environmentChoice.TrySetSelectedItem(target)
            || string.Equals(_environmentChoice.SelectedItem, target, StringComparison.Ordinal);
    }

    private bool ForceComboSelection(string target)
    {
        if (!TemplateOptions.Contains(target, StringComparer.Ordinal))
        {
            return false;
        }

        if (string.Equals(_templateCombo.SelectedItem, target, StringComparison.Ordinal))
        {
            return true;
        }

        return _templateCombo.TrySetSelectedItem(target)
            || string.Equals(_templateCombo.SelectedItem, target, StringComparison.Ordinal);
    }

    private bool SelectDataFormFieldByKey(string key)
    {
        if (!_fieldIndexByKey.TryGetValue(key, out var index))
        {
            return false;
        }

        _dataForm.RequestFocus();
        var changed = _dataForm.SelectField(index);
        return changed || string.Equals(_dataForm.SelectedField?.Key, key, StringComparison.Ordinal);
    }

    private bool RouteIssueSelection(string field)
    {
        if (SelectDataFormFieldByKey(field))
        {
            return true;
        }

        if (string.Equals(field, "environment", StringComparison.Ordinal))
        {
            _environmentChoice.RequestFocus();
            return true;
        }

        if (string.Equals(field, "template", StringComparison.Ordinal))
        {
            _templateCombo.RequestFocus();
            return true;
        }

        if (string.Equals(field, "approverInput", StringComparison.Ordinal))
        {
            _approverInput.RequestFocus();
            return true;
        }

        return false;
    }

    private void ApplyTemplate(string templateName, string reason)
    {
        if (!_templates.TryGetValue(templateName, out var template))
        {
            return;
        }

        _draft.Environment = template.Environment;
        _draft.TargetRegion = template.TargetRegion;
        _draft.ChangeWindow = template.ChangeWindow;
        _draft.RolloutPercent = template.RolloutPercent;
        _draft.RollbackPlan = template.RollbackPlan;
        if (string.IsNullOrWhiteSpace(_draft.Service) || string.Equals(template.Name, "Blank", StringComparison.Ordinal))
        {
            _draft.Service = template.Name == "Blank" ? _draft.Service : template.Name.Replace(' ', '-').ToLowerInvariant();
        }

        _draft.ChangeTicket = template.ChangeTicket;

        _ticketInput.SetValue(_draft.ChangeTicket);
        _dataForm.SetModel(_draft);
        _ = ForceChoiceSelection(_draft.Environment);

        _statusText = $"template applied -> {templateName}";
        AppendActivity($"Applied template ({reason}): {templateName}");
        RunValidation("template-applied");
    }

    private static int ParseRollout(string raw)
    {
        return int.TryParse(raw.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? Math.Clamp(parsed, 1, 100)
            : 1;
    }

    private static bool LooksLikeWindow(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var parts = value.Split('-', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 && parts[0].Contains(':', StringComparison.Ordinal) && parts[1].Contains(':', StringComparison.Ordinal);
    }

    private string CurrentStepLabel()
    {
        return _wizard.SelectedStep?.Title ?? "n/a";
    }

    private string TemplateOrUnset()
    {
        return string.IsNullOrWhiteSpace(_templateCombo.SelectedItem) ? "(unset)" : _templateCombo.SelectedItem;
    }

    private string ThemeLabel()
    {
        return _useAlternateTheme ? "Rose Pine" : "Catppuccin";
    }

    private void AppendSelectionTrace(string line)
    {
        _selectionTraceEntries.Add($"{DateTime.Now:HH:mm:ss} {line}");
        if (_selectionTraceEntries.Count > 120)
        {
            _selectionTraceEntries.RemoveRange(0, _selectionTraceEntries.Count - 120);
        }
    }

    private void AppendActivity(string line)
    {
        _activityLog.Append($"{DateTime.Now:HH:mm:ss} {line}");
    }
}
