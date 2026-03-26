using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed partial class WidgetCoverageWorkflowLabApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private static readonly string[] WorkflowSteps = ["Intake", "Validate", "Approve", "Schedule", "Execute"];
    private static readonly string[] EnvironmentOptions = ["Development", "Staging", "Production", "Canary"];
    private static readonly string[] TemplateOptions = ["Blank", "Blue/Green API", "Batch Worker", "Emergency Rollback"];
    private static readonly string[] RunbookSuggestions =
    [
        "API canary rollout",
        "Batch worker restart",
        "DB migration with fallback",
        "Service rollback checklist",
        "High-risk weekend runbook",
    ];

    private readonly ChangeDraft _draft = ChangeDraft.CreateDefault();

    private readonly Dictionary<string, TemplatePreset> _templates = new(StringComparer.Ordinal)
    {
        ["Blank"] = new TemplatePreset("Blank", "Development", "us-east-1", "09:00-11:00", "API canary rollout", 5, 15),
        ["Blue/Green API"] = new TemplatePreset("Blue/Green API", "Production", "us-east-1", "20:00-22:00", "API canary rollout", 10, 25),
        ["Batch Worker"] = new TemplatePreset("Batch Worker", "Staging", "eu-west-1", "11:00-12:30", "Batch worker restart", 25, 60),
        ["Emergency Rollback"] = new TemplatePreset("Emergency Rollback", "Canary", "us-west-2", "00:15-01:00", "Service rollback checklist", 1, 5),
    };

    private readonly Stepper _stepper = new()
    {
        Title = "Release Intake Workflow",
        FocusMarker = "◆",
        Connector = " -> ",
    };

    private readonly Wizard _wizard = new()
    {
        Title = "Step Guide",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        ActiveMarker = "▶",
        CompletedMarker = "✓",
        PendingMarker = "·",
    };

    private readonly DataForm<ChangeDraft> _dataForm = new()
    {
        Title = "Change Intake Form",
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
        Title = "Validation Issues",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly EmptyState _noIssuesState = new()
    {
        Title = "No validation issues",
        Description = "Run validation after edits to refresh gates.",
        Hint = "Ctrl+V or Validate button",
        ActionText = "Run Validation",
        FocusMarker = "◆",
    };

    private readonly Form _reviewForm = new()
    {
        Title = "Submission Snapshot",
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

    private readonly InspectorPanel _inspector = new()
    {
        Title = "Inspector",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
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
        Placeholder = "Select a change template",
    };

    private readonly SearchBox _policySearch = new()
    {
        Title = "Policy Search",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "Find policy rule or runbook",
    };

    private readonly TextInput _ticketInput = new()
    {
        Title = "Change Ticket",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "CHG-123456",
    };

    private readonly AutocompleteInput _runbookInput = new()
    {
        Title = "Runbook",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Placeholder = "Pick or type a runbook",
        MaxVisibleSuggestions = 5,
    };

    private readonly TagInput _riskTags = new()
    {
        Title = "Risk Tags",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly TokenEditor _reviewers = new()
    {
        Title = "Reviewers",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly NumberInput _rolloutStart = new()
    {
        Title = "Rollout Start %",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Min = 0,
        Max = 100,
        Step = 1,
        Precision = 0,
    };

    private readonly NumberInput _rolloutEnd = new()
    {
        Title = "Rollout End %",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Min = 0,
        Max = 100,
        Step = 1,
        Precision = 0,
    };

    private readonly Notifications _activity = new()
    {
        Title = "Activity Feed",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        MaxItems = 160,
    };

    private readonly NotificationInbox _approvalInbox = new()
    {
        Title = "Approval Inbox",
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        PageSize = 8,
        MaxItems = 160,
    };

    private readonly Button _validateButton = new() { Text = "Validate", Description = "Ctrl+V", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _nextButton = new() { Text = "Next Step", Description = "Ctrl+N", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _backButton = new() { Text = "Back", Description = "Ctrl+B", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _queueApprovalButton = new() { Text = "Queue Approval", Description = "Ctrl+A", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };

    private readonly StatusBar _status = new();

    private bool _syncingStepSelection;
    private int _validationRunCount;
    private int _policyMatchIndex;
    private int _issueRouteCount;
    private string _statusText = "Ready";
    private string _lastTagSnapshot = string.Empty;
    private string _lastReviewerSnapshot = string.Empty;

    public WidgetCoverageWorkflowLabApp()
    {
        ConfigureWorkflowControls();
        ConfigureDataForm();
        ConfigureInputSources();
        WireEvents();
        ApplyThemeAndOverrides();
        InitializeDefaults();
        RunValidation("startup");
        AppendActivity("Workflow lab initialized.", NotificationLevel.Info);
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key)
        {
            if (key.IsCharacter('c', ModifierKeys.Ctrl))
            {
                return TeaEffects.Quit;
            }

            if (key.IsCharacter('v', ModifierKeys.Ctrl))
            {
                RunValidation("Ctrl+V");
            }
            else if (key.IsCharacter('n', ModifierKeys.Ctrl))
            {
                AdvanceStep();
            }
            else if (key.IsCharacter('b', ModifierKeys.Ctrl))
            {
                RetreatStep();
            }
            else if (key.IsCharacter('a', ModifierKeys.Ctrl))
            {
                QueueApprovalRequest("shortcut");
            }
            else if (key.IsCharacter('1', ModifierKeys.Ctrl))
            {
                CycleEnvironment();
            }
            else if (key.IsCharacter('2', ModifierKeys.Ctrl))
            {
                CycleTemplate();
            }
            else if (key.IsCharacter('r', ModifierKeys.Ctrl))
            {
                RouteFirstIssue();
            }
        }

        DetectEditorStateChanges();
        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshDerivedViews();

        _status.LeftText =
            $"step={CurrentStepLabel()} env={_draft.Environment} rollout={_draft.RolloutStart:0}-{_draft.RolloutEnd:0}% runs={_validationRunCount}";
        _status.RightText =
            $"{_statusText}  Ctrl+V validate  Ctrl+N/B step  Ctrl+A approval  Ctrl+R route  Ctrl+C quit";

        var selectorRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _environmentChoice, Length = 24 },
                new LayoutSlot { Content = _templateCombo, Length = Math.Min(36, Math.Max(28, context.Width / 4)) },
                new LayoutSlot { Content = _policySearch, Length = LayoutLength.Fill() },
            },
        };

        var inputRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _ticketInput, Length = Math.Min(28, Math.Max(22, context.Width / 4)) },
                new LayoutSlot { Content = _runbookInput, Length = LayoutLength.Fill() },
            },
        };

        var rolloutRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _rolloutStart, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _rolloutEnd, Length = LayoutLength.Fill() },
            },
        };

        var editorPanel = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _riskTags, Length = 4 },
                new LayoutSlot { Content = _reviewers, Length = 4 },
                new LayoutSlot { Content = rolloutRow, Length = 6 },
            },
        };

        var actions = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _validateButton, Length = 14 },
                new LayoutSlot { Content = _backButton, Length = 12 },
                new LayoutSlot { Content = _nextButton, Length = 14 },
                new LayoutSlot { Content = _queueApprovalButton, Length = 20 },
            },
        };

        var topPanel = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = selectorRow, Length = 6 },
                new LayoutSlot { Content = inputRow, Length = 5 },
                new LayoutSlot { Content = actions, Length = 5 },
            },
        };

        Control issuesPanel = _validationSummary.Issues.Count == 0 ? _noIssuesState : _validationSummary;

        var formAndIssues = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _dataForm, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = issuesPanel, Length = Math.Min(48, Math.Max(38, context.Width / 3)) },
            },
        };

        var reviewAndInspector = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _reviewForm, Length = LayoutLength.Fill() },
                new LayoutSlot
                {
                    Content = new ColumnLayout
                    {
                        Gap = 1,
                        Items =
                        {
                            new LayoutSlot { Content = _policyFieldSet, Length = 7 },
                            new LayoutSlot { Content = _inspector, Length = LayoutLength.Fill() },
                        },
                    },
                    Length = Math.Min(50, Math.Max(42, context.Width / 3)),
                },
            },
        };

        var notificationsRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _activity, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _approvalInbox, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = editorPanel, Length = Math.Min(46, Math.Max(38, context.Width / 3)) },
            },
        };

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(2, _stepper);
            window.Left(Math.Min(38, Math.Max(31, context.Width / 4)), _wizard);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(18, topPanel);
                column.Fixed(14, formAndIssues);
                column.Fixed(12, reviewAndInspector);
                column.Fill(notificationsRow);
            }));
            window.Footer(1, _status);
        });
    }
}
