using System.Globalization;
using TeaSharp.Controls;

internal sealed partial class WidgetCoverageWorkflowLabApp
{
    private void RouteFirstIssue()
    {
        if (_validationSummary.Issues.Count == 0)
        {
            _statusText = "no issues to route";
            return;
        }

        _validationSummary.SetSelectedIndex(0);
        var first = _validationSummary.Issues[0];
        var routed = RouteIssueSelection(first.Field ?? string.Empty);
        _statusText = routed ? $"routed first issue -> {first.Field}" : "could not route first issue";
    }

    private bool RouteIssueSelection(string field)
    {
        if (string.IsNullOrWhiteSpace(field))
        {
            return false;
        }

        if (_dataForm.SelectField(field))
        {
            _dataForm.RequestFocus();
            _issueRouteCount++;
            return true;
        }

        switch (field)
        {
            case "environment":
                _environmentChoice.RequestFocus();
                _issueRouteCount++;
                return true;
            case "template":
                _templateCombo.RequestFocus();
                _issueRouteCount++;
                return true;
            case "runbook":
                _runbookInput.RequestFocus();
                _issueRouteCount++;
                return true;
            case "ticket":
                _ticketInput.RequestFocus();
                _issueRouteCount++;
                return true;
            case "tags":
                _riskTags.RequestFocus();
                _issueRouteCount++;
                return true;
            case "reviewers":
                _reviewers.RequestFocus();
                _issueRouteCount++;
                return true;
            case "rolloutStart":
                _rolloutStart.RequestFocus();
                _issueRouteCount++;
                return true;
            case "rolloutEnd":
                _rolloutEnd.RequestFocus();
                _issueRouteCount++;
                return true;
            case "approvals":
                _approvalInbox.RequestFocus();
                if (_approvalInbox.Items.Count > 0)
                {
                    _approvalInbox.Select(_approvalInbox.Items.Count - 1);
                }

                _issueRouteCount++;
                return true;
            default:
                return false;
        }
    }

    private void RunValidation(string reason)
    {
        _validationRunCount++;
        var issues = BuildValidationIssues();
        _validationSummary.SetIssues(issues);

        var errors = CountErrors();
        var warnings = _validationSummary.Issues.Count(static issue => issue.Severity == ValidationSeverity.Warning);
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
    }

    private List<ValidationIssue> BuildValidationIssues()
    {
        var issues = new List<ValidationIssue>();

        if (_draft.Service.Trim().Length < 3)
        {
            issues.Add(new ValidationIssue("Service name must have at least 3 chars.", ValidationSeverity.Error, "service"));
        }

        if (!_draft.OwnerEmail.Contains('@', StringComparison.Ordinal))
        {
            issues.Add(new ValidationIssue("Owner email must contain '@'.", ValidationSeverity.Error, "ownerEmail"));
        }

        if (!LooksLikeWindow(_draft.ChangeWindow))
        {
            issues.Add(new ValidationIssue("Change window must follow HH:MM-HH:MM.", ValidationSeverity.Error, "window"));
        }

        if (!_draft.ChangeTicket.StartsWith("CHG-", StringComparison.OrdinalIgnoreCase))
        {
            issues.Add(new ValidationIssue("Ticket should start with CHG-.", ValidationSeverity.Error, "ticket"));
        }

        if (_draft.RolloutStart > _draft.RolloutEnd)
        {
            issues.Add(new ValidationIssue("Rollout start cannot exceed rollout end.", ValidationSeverity.Error, "rolloutStart"));
        }

        if (string.Equals(_draft.Environment, "Production", StringComparison.Ordinal) && _draft.RolloutEnd > 25)
        {
            issues.Add(new ValidationIssue("Production rollout above 25% requires explicit approval.", ValidationSeverity.Warning, "rolloutEnd"));
        }

        if (_reviewers.Tokens.Count == 0)
        {
            issues.Add(new ValidationIssue("At least one reviewer must be assigned.", ValidationSeverity.Error, "reviewers"));
        }

        if (_riskTags.Tags.Count == 0)
        {
            issues.Add(new ValidationIssue("Add at least one risk tag.", ValidationSeverity.Warning, "tags"));
        }

        if (string.IsNullOrWhiteSpace(_runbookInput.Text))
        {
            issues.Add(new ValidationIssue("Runbook should be selected for execution steps.", ValidationSeverity.Info, "runbook"));
        }

        if (_approvalInbox.Items.Count == 0)
        {
            issues.Add(new ValidationIssue("No approvals queued yet.", ValidationSeverity.Info, "approvals"));
        }

        return issues;
    }

    private void HandlePolicySearchQuery(string query)
    {
        var normalized = query.Trim();
        var matches = RunbookSuggestions
            .Where(suggestion => suggestion.Contains(normalized, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (normalized.Length == 0)
        {
            _policyMatchIndex = 0;
            _policySearch.ClearMatchState();
            _runbookInput.SetSuggestions(RunbookSuggestions);
            return;
        }

        if (matches.Length == 0)
        {
            _policySearch.SetMatchState(0);
            _runbookInput.SetSuggestions(RunbookSuggestions);
            _statusText = $"no policy match for '{normalized}'";
            return;
        }

        _policyMatchIndex = 0;
        _policySearch.SetMatchState(matches.Length, _policyMatchIndex);
        _runbookInput.SetSuggestions(matches);
        _statusText = $"policy matches: {matches.Length}";
    }

    private void HandlePolicyNavigation(SearchNavigationDirection direction)
    {
        if (!_policySearch.MatchCount.HasValue || _policySearch.MatchCount.Value <= 0)
        {
            return;
        }

        var matchCount = _policySearch.MatchCount.Value;
        _policyMatchIndex = direction == SearchNavigationDirection.Next
            ? (_policyMatchIndex + 1) % matchCount
            : (_policyMatchIndex - 1 + matchCount) % matchCount;

        _policySearch.SetMatchState(matchCount, _policyMatchIndex);
    }

    private void DetectEditorStateChanges()
    {
        var tagSnapshot = CreateTagSnapshot();
        if (!string.Equals(tagSnapshot, _lastTagSnapshot, StringComparison.Ordinal))
        {
            _lastTagSnapshot = tagSnapshot;
            _statusText = $"risk tags changed ({_riskTags.Tags.Count})";
            RunValidation("tag-input");
        }

        var reviewerSnapshot = CreateReviewerSnapshot();
        if (!string.Equals(reviewerSnapshot, _lastReviewerSnapshot, StringComparison.Ordinal))
        {
            _lastReviewerSnapshot = reviewerSnapshot;
            _statusText = $"reviewers changed ({_reviewers.Tokens.Count})";
            RunValidation("token-editor");
        }
    }

    private void RefreshDerivedViews()
    {
        _reviewForm.SetFields(
        [
            new FormField("service", "Service", _draft.Service, isRequired: true),
            new FormField("owner", "Owner", _draft.OwnerEmail, isRequired: true),
            new FormField("environment", "Environment", _draft.Environment, isRequired: true),
            new FormField("region", "Region", _draft.TargetRegion),
            new FormField("window", "Window", _draft.ChangeWindow),
            new FormField("rollout", "Rollout", $"{_draft.RolloutStart:0}% -> {_draft.RolloutEnd:0}%"),
            new FormField("ticket", "Ticket", _draft.ChangeTicket),
            new FormField("runbook", "Runbook", _runbookInput.Text),
            new FormField("reviewers", "Reviewers", string.Join(", ", _reviewers.Tokens.Select(static token => token.Value))),
            new FormField("tags", "Risk Tags", string.Join(", ", _riskTags.Tags)),
        ]);

        var errors = CountErrors();
        var warnings = _validationSummary.Issues.Count(static issue => issue.Severity == ValidationSeverity.Warning);
        _policyFieldSet.SetItems(
        [
            $"Current step: {CurrentStepLabel()}",
            $"Validation errors: {errors}",
            $"Validation warnings: {warnings}",
            $"Approvals queued: {_approvalInbox.Items.Count}",
            $"Issue routes handled: {_issueRouteCount}",
            $"Template: {(_templateCombo.SelectedItem.Length == 0 ? "(unset)" : _templateCombo.SelectedItem)}",
        ]);

        var workflowSection = new InspectorSection("Workflow", isExpanded: true);
        workflowSection.AddField("step", CurrentStepLabel());
        workflowSection.AddField("validationRuns", _validationRunCount.ToString(CultureInfo.InvariantCulture));
        workflowSection.AddField("focusRouteCount", _issueRouteCount.ToString(CultureInfo.InvariantCulture));

        var rolloutSection = new InspectorSection("Rollout", isExpanded: true);
        rolloutSection.AddField("start", _draft.RolloutStart.ToString("0", CultureInfo.InvariantCulture));
        rolloutSection.AddField("end", _draft.RolloutEnd.ToString("0", CultureInfo.InvariantCulture));
        rolloutSection.AddField("window", _draft.ChangeWindow);

        var editorSection = new InspectorSection("Editors", isExpanded: true);
        editorSection.AddField("tags", _riskTags.Tags.Count.ToString(CultureInfo.InvariantCulture));
        editorSection.AddField("reviewers", _reviewers.Tokens.Count.ToString(CultureInfo.InvariantCulture));
        editorSection.AddField("runbook", string.IsNullOrWhiteSpace(_runbookInput.Text) ? "(unset)" : _runbookInput.Text);
        editorSection.AddDetail("TagInput/TokenEditor are polled for change detection in this app.");

        _inspector.SetSections([workflowSection, rolloutSection, editorSection]);
    }

    private void AppendActivity(string message, NotificationLevel level)
    {
        _activity.Push(message, level);
    }

    private int CountErrors()
    {
        return _validationSummary.Issues.Count(static issue => issue.Severity == ValidationSeverity.Error);
    }

    private string CurrentStepLabel()
    {
        return _wizard.SelectedStep?.Title ?? "n/a";
    }

    private string CreateTagSnapshot()
    {
        return string.Join('|', _riskTags.Tags);
    }

    private string CreateReviewerSnapshot()
    {
        return string.Join('|', _reviewers.Tokens.Select(static token => token.Value));
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
}

internal sealed class ChangeDraft
{
    public string Service { get; set; } = "checkout-api";
    public string OwnerEmail { get; set; } = "platform-oncall@company.com";
    public string Environment { get; set; } = "Development";
    public string TargetRegion { get; set; } = "us-east-1";
    public string ChangeWindow { get; set; } = "09:00-11:00";
    public string ChangeTicket { get; set; } = "CHG-120045";
    public string Runbook { get; set; } = "API canary rollout";
    public string Notes { get; set; } = "Deploy behind feature flags with rollback checkpoints.";
    public double RolloutStart { get; set; } = 5;
    public double RolloutEnd { get; set; } = 15;

    public static ChangeDraft CreateDefault()
    {
        return new ChangeDraft();
    }
}

internal sealed record TemplatePreset(
    string Name,
    string Environment,
    string Region,
    string Window,
    string Runbook,
    double RolloutStart,
    double RolloutEnd);
