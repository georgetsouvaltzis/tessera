using TeaSharp.Controls;
using TeaSharp.Styles;

internal sealed partial class WidgetCoverageWorkflowLabApp
{
    private void ConfigureWorkflowControls()
    {
        _stepper.SetSteps(WorkflowSteps.Select(static step => new StepperStep(step.ToLowerInvariant(), step)));
        _wizard.SetSteps(WorkflowSteps.Select(static step => new WizardStep(step.ToLowerInvariant(), step, $"{step} tasks")));
        _stepper.SetCurrentStep(0);
        _wizard.SelectStep(0);
    }

    private void ConfigureDataForm()
    {
        _dataForm.RegisterField(
            "service",
            "Service",
            static draft => draft.Service,
            static (draft, value) => draft.Service = value,
            placeholder: "checkout-api",
            validator: static value => value.Trim().Length >= 3 ? null : "Service name must be at least 3 chars.");

        _dataForm.RegisterField(
            "ownerEmail",
            "Owner Email",
            static draft => draft.OwnerEmail,
            static (draft, value) => draft.OwnerEmail = value,
            placeholder: "owner@company.com",
            validator: static value => value.Contains('@') ? null : "Owner email must contain '@'.");

        _dataForm.RegisterField(
            "region",
            "Target Region",
            static draft => draft.TargetRegion,
            static (draft, value) => draft.TargetRegion = value,
            placeholder: "us-east-1",
            validator: static value => value.Contains('-', StringComparison.Ordinal) ? null : "Region should include '-' separator.");

        _dataForm.RegisterField(
            "window",
            "Change Window",
            static draft => draft.ChangeWindow,
            static (draft, value) => draft.ChangeWindow = value,
            placeholder: "20:00-22:00",
            validator: static value => LooksLikeWindow(value) ? null : "Expected HH:MM-HH:MM.");

        _dataForm.RegisterField(
            "ticket",
            "Ticket",
            static draft => draft.ChangeTicket,
            static (draft, value) => draft.ChangeTicket = value,
            placeholder: "CHG-123456",
            validator: static value => value.StartsWith("CHG-", StringComparison.OrdinalIgnoreCase) ? null : "Ticket must start with CHG-.");

        _dataForm.RegisterField(
            "notes",
            "Notes",
            static draft => draft.Notes,
            static (draft, value) => draft.Notes = value,
            placeholder: "deployment notes",
            validator: static value => value.Trim().Length >= 8 ? null : "Notes must be at least 8 chars.");

        _dataForm.SetModel(_draft);
    }

    private void ConfigureInputSources()
    {
        _environmentChoice.SetItems(EnvironmentOptions);
        _templateCombo.SetItems(TemplateOptions);
        _runbookInput.SetSuggestions(RunbookSuggestions);
        _riskTags.Options = new TagInputOptions(Separator: ',', AllowDuplicates: false, MaxTags: 8, ShowTagCount: true, TagPrefix: "#", TagSuffix: string.Empty);
    }

    private void InitializeDefaults()
    {
        _riskTags.SetTags(["low-risk", "weekday"]);
        _reviewers.SetTokens([new TokenItem("platform-oncall"), new TokenItem("release-manager")]);
        _ticketInput.SetValue(_draft.ChangeTicket);
        _runbookInput.SetText(_draft.Runbook);
        _rolloutStart.SetValue(_draft.RolloutStart);
        _rolloutEnd.SetValue(_draft.RolloutEnd);

        _ = _environmentChoice.TrySetSelectedItem(_draft.Environment);
        _ = _templateCombo.TrySetSelectedItem("Blank");

        _approvalInbox.SetItems(
        [
            new InboxItem("seed-1", "Awaiting initial reviewer acknowledgement", NotificationLevel.Info, DateTimeOffset.UtcNow, source: "workflow"),
        ]);
        _approvalInbox.Select(0);
        _wizard.RequestFocus();

        _lastTagSnapshot = CreateTagSnapshot();
        _lastReviewerSnapshot = CreateReviewerSnapshot();
    }

    private void WireEvents()
    {
        _stepper.SelectionChanged += (_, args) => SynchronizeStepSelection(args.SelectedIndex, "stepper");
        _wizard.SelectionChanged += (_, args) => SynchronizeStepSelection(args.SelectedIndex, "wizard");

        _dataForm.SelectionChanged += (_, args) =>
        {
            if (args.SelectedField is null)
            {
                return;
            }

            _statusText = $"form field -> {args.SelectedField.Label}";
        };

        _dataForm.FieldCommitted += (_, args) =>
        {
            _draft.ChangeTicket = _draft.ChangeTicket.Trim();
            _ticketInput.SetValue(_draft.ChangeTicket);
            _statusText = args.Success ? $"Committed {args.Field.Key}" : $"Commit failed: {args.Error}";
            AppendActivity(_statusText, args.Success ? NotificationLevel.Success : NotificationLevel.Warning);
            RunValidation("data-form-commit");
        };

        _environmentChoice.SelectionChanged += (_, args) =>
        {
            _draft.Environment = args.SelectedItem;
            _statusText = $"environment -> {args.SelectedItem}";
            RunValidation("environment-selection");
        };

        _templateCombo.SelectionChanged += (_, args) =>
        {
            if (!string.IsNullOrWhiteSpace(args.SelectedItem))
            {
                _ = ApplyTemplate(args.SelectedItem, "template-combo");
            }
        };

        _ticketInput.Submitted += (_, args) =>
        {
            _draft.ChangeTicket = args.Value.Trim();
            _dataForm.SetModel(_draft);
            _statusText = $"ticket updated -> {_draft.ChangeTicket}";
            RunValidation("ticket-input");
        };

        _runbookInput.SuggestionCommitted += (_, args) =>
        {
            _draft.Runbook = args.Text;
            _statusText = $"runbook committed -> {args.Text}";
            RunValidation("runbook-commit");
        };

        _policySearch.QueryChanged += (_, args) => HandlePolicySearchQuery(args.Query);
        _policySearch.NavigationRequested += (_, args) => HandlePolicyNavigation(args.Direction);

        _rolloutStart.Submitted += (_, args) =>
        {
            _draft.RolloutStart = args.Value;
            _statusText = $"rollout start -> {_draft.RolloutStart:0}%";
            RunValidation("rollout-start");
        };

        _rolloutEnd.Submitted += (_, args) =>
        {
            _draft.RolloutEnd = args.Value;
            _statusText = $"rollout end -> {_draft.RolloutEnd:0}%";
            RunValidation("rollout-end");
        };

        _validationSummary.SelectionChanged += (_, args) =>
        {
            if (args.SelectedIssue is null)
            {
                return;
            }

            var routed = RouteIssueSelection(args.SelectedIssue.Field ?? string.Empty);
            _statusText = routed
                ? $"routed issue -> {args.SelectedIssue.Field}"
                : $"route unavailable -> {args.SelectedIssue.Field}";
        };

        _noIssuesState.ActionInvoked += (_, _) => RunValidation("no-issues-action");
        _validateButton.Activated += (_, _) => RunValidation("validate-button");
        _nextButton.Activated += (_, _) => AdvanceStep();
        _backButton.Activated += (_, _) => RetreatStep();
        _queueApprovalButton.Activated += (_, _) => QueueApprovalRequest("button");

        _activity.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _statusText = $"activity selected -> {args.SelectedItem.Message}";
        };
    }

    private void ApplyThemeAndOverrides()
    {
        ThemeScope.Apply(
            DefaultTheme,
            _stepper,
            _wizard,
            _dataForm,
            _validationSummary,
            _noIssuesState,
            _reviewForm,
            _policyFieldSet,
            _inspector,
            _environmentChoice,
            _templateCombo,
            _policySearch,
            _ticketInput,
            _runbookInput,
            _riskTags,
            _reviewers,
            _rolloutStart,
            _rolloutEnd,
            _activity,
            _approvalInbox,
            _validateButton,
            _nextButton,
            _backButton,
            _queueApprovalButton,
            _status);

        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);
        var selected = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();

        _stepper.ActiveStepStyle = selected;
        _stepper.CompletedStepStyle = theme.State.Success.WithBold();
        _stepper.PendingStepStyle = theme.Text.Secondary;

        _wizard.BorderStyleText = theme.Border.Strong;
        _wizard.FocusedBorderStyleText = focusedBorder;
        _wizard.ActiveStepStyle = selected;
        _wizard.FocusedActiveStepStyle = selected;
        _wizard.HoveredStepStyle = theme.Accent.Secondary.WithUnderline();

        _dataForm.BorderStyleText = theme.Border.Strong;
        _dataForm.FocusedBorderStyleText = focusedBorder;
        _dataForm.SelectedFieldStyle = selected;
        _dataForm.FocusedSelectedFieldStyle = selected;
        _dataForm.HoveredFieldStyle = theme.Accent.Secondary.WithUnderline();
        _dataForm.ErrorStyle = theme.State.Error.WithBold();

        _validationSummary.BorderStyleText = theme.Border.Strong;
        _validationSummary.FocusedBorderStyleText = focusedBorder;
        _validationSummary.SelectedIssueStyle = selected;
        _validationSummary.FocusedIssueStyle = selected;
        _validationSummary.ErrorSeverityStyle = theme.State.Error.WithBold();
        _validationSummary.WarningSeverityStyle = theme.State.Warning.WithBold();

        _reviewForm.BorderStyleText = theme.Border.Strong;
        _reviewForm.FocusedBorderStyleText = focusedBorder;
        _reviewForm.SelectedRowStyle = selected;
        _reviewForm.FocusedSelectedRowStyle = selected;

        _policyFieldSet.BorderStyleText = theme.Border.Strong;
        _policyFieldSet.FocusedBorderStyleText = focusedBorder;
        _policyFieldSet.SelectedItemStyle = selected;
        _policyFieldSet.FocusedSelectedItemStyle = selected;

        _inspector.BorderStyleText = theme.Border.Strong;
        _inspector.FocusedBorderStyleText = focusedBorder;
        _inspector.SelectedRowStyle = selected;
        _inspector.FocusedSelectedRowStyle = selected;

        _environmentChoice.Glyphs = new DropdownGlyphSet("⌄", "⌃", "▸", "◆");
        _environmentChoice.BorderStyleText = theme.Border.Strong;
        _environmentChoice.FocusedBorderStyleText = focusedBorder;
        _environmentChoice.SelectedOptionStyle = selected;

        _templateCombo.Glyphs = new DropdownGlyphSet("⌄", "⌃", "▸", "◆");
        _templateCombo.BorderStyleText = theme.Border.Strong;
        _templateCombo.FocusedBorderStyleText = focusedBorder;
        _templateCombo.SelectedOptionStyle = selected;

        _policySearch.BorderStyleText = theme.Border.Strong;
        _policySearch.FocusedBorderStyleText = focusedBorder;
        _policySearch.MatchHighlightStyle = theme.Accent.Secondary.WithBold();

        _ticketInput.BorderStyleText = theme.Border.Strong;
        _ticketInput.FocusedBorderStyleText = focusedBorder;

        _runbookInput.BorderStyleText = theme.Border.Strong;
        _runbookInput.FocusedBorderStyleText = focusedBorder;
        _runbookInput.SelectedSuggestionStyle = selected;
        _runbookInput.FocusedSelectedSuggestionStyle = selected;

        _riskTags.BorderStyleText = theme.Border.Strong;
        _riskTags.FocusedBorderStyleText = focusedBorder;
        _riskTags.SelectedTagStyle = selected;
        _riskTags.FocusedTagStyle = selected;

        _reviewers.BorderStyleText = theme.Border.Strong;
        _reviewers.FocusedBorderStyleText = focusedBorder;
        _reviewers.SelectedTokenStyle = selected;
        _reviewers.FocusedSelectedTokenStyle = selected;

        _rolloutStart.BorderStyleText = theme.Border.Strong;
        _rolloutStart.FocusedBorderStyleText = focusedBorder;
        _rolloutStart.SummaryTextStyle = theme.Text.Secondary;

        _rolloutEnd.BorderStyleText = theme.Border.Strong;
        _rolloutEnd.FocusedBorderStyleText = focusedBorder;
        _rolloutEnd.SummaryTextStyle = theme.Text.Secondary;

        _activity.BorderStyleText = theme.Border.Strong;
        _activity.FocusedBorderStyleText = focusedBorder;
        _activity.SelectedItemStyle = selected;

        _approvalInbox.SelectedItemStyle = selected;
        _approvalInbox.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();
        _approvalInbox.WarningItemStyle = theme.State.Warning.WithBold();
        _approvalInbox.ErrorItemStyle = theme.State.Error.WithBold();
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

    private void SetWorkflowStep(int requestedIndex, string source)
    {
        var bounded = Math.Clamp(requestedIndex, 0, Math.Max(0, _wizard.Steps.Count - 1));
        _syncingStepSelection = true;
        _stepper.SetCurrentStep(bounded);
        _wizard.SelectStep(bounded);
        _syncingStepSelection = false;
        _statusText = $"step -> {CurrentStepLabel()} ({source})";
    }

    private void AdvanceStep()
    {
        RunValidation("step-advance");
        var errors = CountErrors();
        if (errors > 0)
        {
            _statusText = "fix validation errors before advancing";
            AppendActivity(_statusText, NotificationLevel.Warning);
            return;
        }

        var current = _wizard.SelectedIndex;
        if (current < 0)
        {
            return;
        }

        _stepper.SetStepCompleted(current, isCompleted: true);
        _wizard.SetStepCompleted(current, isCompleted: true);
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

    private void CycleEnvironment()
    {
        _ = _environmentChoice.SetSelectedIndex(_environmentChoice.SelectedIndex + 1);
    }

    private void CycleTemplate()
    {
        var applied = _templateCombo.SetSelectedIndex(_templateCombo.SelectedItem.Length == 0 ? 0 : _templateCombo.SelectedItem == TemplateOptions[^1]
            ? 0
            : Array.IndexOf(TemplateOptions, _templateCombo.SelectedItem) + 1);
        if (!applied)
        {
            _ = _templateCombo.SetSelectedIndex(0);
        }
    }

    private bool ApplyTemplate(string templateName, string source)
    {
        if (!_templates.TryGetValue(templateName, out var template))
        {
            return false;
        }

        _draft.Environment = template.Environment;
        _draft.TargetRegion = template.Region;
        _draft.ChangeWindow = template.Window;
        _draft.Runbook = template.Runbook;
        _draft.RolloutStart = template.RolloutStart;
        _draft.RolloutEnd = template.RolloutEnd;

        _rolloutStart.SetValue(_draft.RolloutStart);
        _rolloutEnd.SetValue(_draft.RolloutEnd);
        _runbookInput.SetText(_draft.Runbook);
        _dataForm.SetModel(_draft);

        _ = _environmentChoice.TrySetSelectedItem(_draft.Environment);
        _ = _templateCombo.TrySetSelectedItem(templateName);

        _statusText = $"template applied -> {templateName}";
        AppendActivity($"Applied template from {source}: {templateName}", NotificationLevel.Info);
        RunValidation("template-applied");
        return true;
    }

    private void QueueApprovalRequest(string source)
    {
        var message = $"Approval needed for {_draft.Service} ({_draft.ChangeTicket})";
        _approvalInbox.Add(message, NotificationLevel.Warning, source: "approval");
        _approvalInbox.Select(_approvalInbox.Items.Count - 1);
        AppendActivity($"Queued approval ({source}).", NotificationLevel.Warning);
        RunValidation("approval-queued");
    }

}
