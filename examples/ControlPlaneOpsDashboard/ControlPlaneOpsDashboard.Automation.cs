using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed partial class ControlPlaneOpsDashboardApp
{
    private bool _quickOpenItemsDirty;

    private readonly JumpList _jumpList = new()
    {
        Title = "Runbook Jump List",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly TokenEditor _tokenEditor = new()
    {
        Title = "Incident Tags",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Placeholder = "Add tag and press Enter...",
        FocusMarker = "◆",
    };

    private readonly AutocompleteInput _commandInput = new()
    {
        Title = "Command Input",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Placeholder = "Type command...",
        MaxVisibleSuggestions = 6,
        FocusMarker = "◆",
    };

    private readonly ResizablePaneGroup _automationPanes = new()
    {
        Title = "Automation Workspace",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly QuickOpenOverlay _quickOpenOverlay = new()
    {
        Title = "Quick Open",
        BorderStyle = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Placeholder = "Search service, endpoint, command, or navigation...",
        MaxVisibleItems = 10,
        FocusMarker = "◆",
    };

    private readonly Label _automationSummary = new()
    {
        Title = "Automation Summary",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly List<TaskRunItem> _pipelineRuns =
    [
        new TaskRunItem("build", "Build", TaskRunStatus.Succeeded, "compiled"),
        new TaskRunItem("tests", "Tests", TaskRunStatus.Running, "integration"),
        new TaskRunItem("security", "Security Scan", TaskRunStatus.Queued, "pending"),
        new TaskRunItem("deploy", "Deploy", TaskRunStatus.Queued, "waiting"),
    ];

    private void InitializeAutomationFeatures()
    {
        _jumpList.SetItems(
        [
            new JumpListItem("queue-deploy", "Queue deployment", isPinned: true),
            new JumpListItem("rollback", "Run rollback", isPinned: true, isDisabled: true),
            new JumpListItem("promote", "Promote canary", isRecent: true),
            new JumpListItem("drain", "Drain region traffic"),
            new JumpListItem("report", "Generate incident report", isRecent: true),
        ]);
        _jumpList.Activated += (_, args) => RunJumpAction(args.SelectedItem.Id, "jump-list");

        _tokenEditor.SetTokens(
        [
            new TokenItem("tier:critical"),
            new TokenItem("owner:platform"),
            new TokenItem("region:global"),
        ]);
        _tokenEditor.SelectionChanged += (_, args) =>
        {
            _statusText = args.SelectedToken is null
                ? "token selection cleared"
                : $"token selected -> {args.SelectedToken.Value}";
        };

        _commandInput.SetSuggestions(
        [
            "deploy checkout --region us-east-1",
            "rollback checkout --to previous",
            "promote search --from canary",
            "scale edge --replicas 12",
            "mute billing --duration 10m",
            "tail logs --service checkout --errors",
            "acknowledge mailer --incident INC-9021",
        ]);
        _commandInput.SuggestionCommitted += (_, args) =>
        {
            _statusText = $"command committed ({args.SuggestionIndex})";
            _notifications.Push($"Command committed: {args.Text}", NotificationLevel.Info);
            AppendActivity($"Command committed from autocomplete -> {args.Text}");
        };

        _quickOpenOverlay.Submitted += (_, args) => HandleQuickOpenSubmitted(args);
        _quickOpenOverlay.Cancelled += (_, _) =>
        {
            _statusText = "quick-open canceled";
            RefreshQuickOpenItemsIfIdle();
        };

        _automationPanes.SetPanes(
        [
            new PaneSpec("jump", _jumpList, minSize: 20),
            new PaneSpec("tags", _tokenEditor, minSize: 22),
            new PaneSpec("command", _commandInput, minSize: 24),
        ]);
        _automationPanes.SetSplitRatio(0, 0.34d);
        _automationPanes.SetSplitRatio(1, 0.72d);

        _quickOpenOverlay.SetItems(BuildQuickOpenItems());
        _quickOpenItemsDirty = false;
    }

    private Screen BuildAutomationScreen(ScreenContext context)
    {
        ConfigureStatus("automation");
        _automationSummary.Text = BuildAutomationSummary(context);

        var topRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _automationPanes, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _pipelinePanel, Length = Math.Min(58, Math.Max(42, context.Width / 3)) },
            },
        };

        var bottomRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _automationSummary, Length = Math.Min(58, Math.Max(42, context.Width / 3)) },
                new LayoutSlot { Content = _activity, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _notifications, Length = Math.Min(44, Math.Max(32, context.Width / 4)) },
            },
        };

        return BuildWithChrome(
            context,
            body =>
            {
                body.Column(column =>
                {
                    column.Gap(1);
                    column.Fixed(12, topRow);
                    column.Fill(bottomRow);
                });
            });
    }

    private void UpdateAutomationState()
    {
        if (_pipelineRuns.Count > 0)
        {
            var index = _tick % _pipelineRuns.Count;
            var run = _pipelineRuns[index];
            run.Status = run.Status switch
            {
                TaskRunStatus.Queued => TaskRunStatus.Running,
                TaskRunStatus.Running when _tick % 3 == 0 => TaskRunStatus.Failed,
                TaskRunStatus.Running => TaskRunStatus.Succeeded,
                TaskRunStatus.Failed => TaskRunStatus.Running,
                _ => TaskRunStatus.Queued,
            };
            run.Description = run.Status switch
            {
                TaskRunStatus.Queued => "queued",
                TaskRunStatus.Running => "in-progress",
                TaskRunStatus.Succeeded => "completed",
                TaskRunStatus.Failed => "retry required",
                _ => run.Description,
            };
            run.UpdatedAt = DateTimeOffset.UtcNow;
        }

        _pipelinePanel.SetItems(_pipelineRuns);

        var hasOutage = _services.Any(static service => string.Equals(service.State, "Degraded", StringComparison.Ordinal));
        _jumpList.SetItems(
        [
            new JumpListItem("queue-deploy", "Queue deployment", isPinned: true),
            new JumpListItem("rollback", "Run rollback", isPinned: true, isDisabled: !hasOutage),
            new JumpListItem("promote", "Promote canary", isRecent: true),
            new JumpListItem("drain", "Drain region traffic"),
            new JumpListItem("report", "Generate incident report", isRecent: true),
        ]);
    }

    private List<QuickOpenItem> BuildQuickOpenItems()
    {
        var items = new List<QuickOpenItem>
        {
            new("nav:overview", "Go to Overview", "Top tab navigation"),
            new("nav:fleet", "Go to Fleet", "Top tab navigation"),
            new("nav:incidents", "Go to Incidents", "Top tab navigation"),
            new("nav:analytics", "Go to Analytics", "Top tab navigation"),
            new("nav:automation", "Go to Automation", "Top tab navigation"),
            new("action:mark-all-read", "Mark all notifications read", "Calls Notifications.MarkAllRead()"),
            new("action:add-token", "Add token incident:open", "Calls TokenEditor.AddToken(...)"),
            new("action:queue-deploy", "Queue deployment", "Opens deployment confirmation dialog"),
            new("action:ack", "Acknowledge selected incident", "Runs HealthBoard.Acknowledge(...)"),
        };

        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            items.Add(
                new QuickOpenItem(
                    $"service:{service.Id}",
                    $"Service: {service.Name}",
                    $"{service.State}  p95 {service.P95Ms}ms  {service.Region}"));
        }

        for (var index = 0; index < _endpoints.Count; index++)
        {
            var endpoint = _endpoints[index];
            items.Add(
                new QuickOpenItem(
                    $"endpoint:{endpoint.Path}",
                    $"Endpoint: {endpoint.Path}",
                    $"err {endpoint.ErrorBasisPoints / 100d:0.00}%  req/s {endpoint.RequestsPerSecond}"));
        }

        return items;
    }

    private void HandleQuickOpenSubmitted(QuickOpenOverlaySubmittedEventArgs args)
    {
        if (_quickOpenOverlay.IsOpen)
        {
            _quickOpenOverlay.Close();
        }

        var itemId = args.ItemId;
        if (itemId.StartsWith("service:", StringComparison.Ordinal))
        {
            var serviceId = itemId["service:".Length..];
            SelectServiceById(serviceId);
            _statusText = $"quick-open selected service {serviceId}";
            _notifications.Push($"Quick-open -> service {serviceId}", NotificationLevel.Info);
            MarkQuickOpenItemsDirty();
            RefreshQuickOpenItemsIfIdle();
            return;
        }

        if (itemId.StartsWith("endpoint:", StringComparison.Ordinal))
        {
            _tabs.SetSelectedIndex(3);
            _statusText = $"quick-open opened analytics for {itemId["endpoint:".Length..]}";
            MarkQuickOpenItemsDirty();
            RefreshQuickOpenItemsIfIdle();
            return;
        }

        if (itemId.StartsWith("nav:", StringComparison.Ordinal))
        {
            var tab = itemId["nav:".Length..] switch
            {
                "overview" => 0,
                "fleet" => 1,
                "incidents" => 2,
                "analytics" => 3,
                "automation" => 4,
                _ => _tabs.SelectedIndex,
            };
            _tabs.SetSelectedIndex(tab);
            _statusText = $"quick-open switched to {_tabs.Items[tab]}";
            MarkQuickOpenItemsDirty();
            RefreshQuickOpenItemsIfIdle();
            return;
        }

        if (string.Equals(itemId, "action:mark-all-read", StringComparison.Ordinal))
        {
            _notifications.MarkAllRead();
            _statusText = "quick-open marked all notifications read";
            MarkQuickOpenItemsDirty();
            RefreshQuickOpenItemsIfIdle();
            return;
        }

        if (string.Equals(itemId, "action:add-token", StringComparison.Ordinal))
        {
            _tokenEditor.AddToken("incident:open");
            _statusText = "quick-open appended token incident:open";
            MarkQuickOpenItemsDirty();
            RefreshQuickOpenItemsIfIdle();
            return;
        }

        if (string.Equals(itemId, "action:queue-deploy", StringComparison.Ordinal))
        {
            OpenDeployDialog();
            MarkQuickOpenItemsDirty();
            RefreshQuickOpenItemsIfIdle();
            return;
        }

        if (string.Equals(itemId, "action:ack", StringComparison.Ordinal))
        {
            AcknowledgeSelectedIncident();
            MarkQuickOpenItemsDirty();
            RefreshQuickOpenItemsIfIdle();
            return;
        }

        _statusText = $"quick-open submitted {args.Item.Label}";
        AppendActivity($"Quick-open submitted -> {args.ItemId} (query: {args.Query})");
        MarkQuickOpenItemsDirty();
        RefreshQuickOpenItemsIfIdle();
    }

    private void SelectServiceById(string serviceId)
    {
        for (var index = 0; index < _services.Count; index++)
        {
            if (!string.Equals(_services[index].Id, serviceId, StringComparison.Ordinal))
            {
                continue;
            }

            _selectedServiceId = serviceId;
            _serviceList.SetSelectedIndex(index);
            SelectHealthService(serviceId);
            return;
        }
    }

    private void RunJumpAction(string actionId, string source)
    {
        switch (actionId)
        {
            case "queue-deploy":
                OpenDeployDialog();
                break;
            case "rollback":
                _notifications.Push("Rollback started in simulation mode", NotificationLevel.Warning);
                AppendActivity("Rollback runbook triggered.");
                break;
            case "promote":
                _notifications.Push("Canary promotion started", NotificationLevel.Info);
                AppendActivity("Canary promotion started.");
                break;
            case "drain":
                _notifications.Push("Traffic drain sequence started", NotificationLevel.Warning);
                AppendActivity("Drain sequence started.");
                break;
            case "report":
                _notifications.Push("Incident report generated", NotificationLevel.Success);
                AppendActivity("Status report generated.");
                break;
            default:
                _notifications.Push($"Unhandled jump action: {actionId}", NotificationLevel.Error);
                break;
        }

        _statusText = $"{source} action -> {actionId}";
    }

    private string BuildAutomationSummary(ScreenContext context)
    {
        var selectedJump = _jumpList.SelectedItem?.Label ?? "(none)";
        var selectedPane = _automationPanes.SelectedPane?.Id ?? "(none)";
        var command = string.IsNullOrWhiteSpace(_commandInput.Text) ? "(empty)" : _commandInput.Text;
        return
            $"""
             Pane: {selectedPane}
             Jump: {selectedJump}
             Command: {command}
             Tokens: {_tokenEditor.Tokens.Count}
             Viewport: {context.Width}x{context.Height}
             """;
    }

    private bool HandleAutomationHotKeys(KeyPressed key)
    {
        if (_tabs.SelectedIndex != 4)
        {
            return false;
        }

        if (key.IsCharacter('r', ModifierKeys.Ctrl))
        {
            if (_jumpList.SelectedItem is null || _jumpList.SelectedItem.IsDisabled)
            {
                _statusText = "selected jump action is disabled";
                return true;
            }

            RunJumpAction(_jumpList.SelectedItem.Id, "keyboard");
            return true;
        }

        return false;
    }

    private void MarkQuickOpenItemsDirty()
    {
        _quickOpenItemsDirty = true;
        RefreshQuickOpenItemsIfIdle();
    }

    private void RefreshQuickOpenItemsIfIdle()
    {
        if (!_quickOpenItemsDirty || _quickOpenOverlay.IsOpen)
        {
            return;
        }

        _quickOpenOverlay.SetItems(BuildQuickOpenItems());
        _quickOpenItemsDirty = false;
    }

    private void ApplyAutomationTheme(TeaTheme theme, TeaThemeOverrideBundle bundle)
    {
        _jumpList.ApplyTheme(theme);
        _jumpList.FocusMarker = bundle.FocusMarker;
        _jumpList.BorderStyleText = bundle.BorderStyleText;
        _jumpList.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _jumpList.SelectedItemStyle = theme.Selection.Background.WithBold();
        _jumpList.FocusedSelectedItemStyle = theme.Selection.Foreground.WithBold();
        _jumpList.HoveredItemStyle = theme.State.Info;
        _jumpList.DisabledItemStyle = theme.Text.Muted.WithDim();

        _tokenEditor.ApplyTheme(theme);
        _tokenEditor.FocusMarker = bundle.FocusMarker;
        _tokenEditor.BorderStyleText = bundle.BorderStyleText;
        _tokenEditor.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _tokenEditor.SelectedTokenStyle = theme.Selection.Background.WithBold();
        _tokenEditor.HoveredTokenStyle = theme.State.Info;

        _commandInput.ApplyTheme(theme);
        _commandInput.FocusMarker = bundle.FocusMarker;
        _commandInput.BorderStyleText = bundle.BorderStyleText;
        _commandInput.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _commandInput.SelectedSuggestionStyle = theme.Selection.Background.WithBold();
        _commandInput.FocusedSelectedSuggestionStyle = theme.Selection.Foreground.WithBold();
        _commandInput.PopupStyle = theme.Text.Secondary;
        _commandInput.CommitMarkerStyle = theme.Accent.Primary.WithBold();

        _automationPanes.ApplyTheme(theme);
        _automationPanes.FocusMarker = bundle.FocusMarker;
        _automationPanes.BorderStyleText = bundle.BorderStyleText;
        _automationPanes.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _quickOpenOverlay.ApplyTheme(theme);
        _quickOpenOverlay.FocusMarker = bundle.FocusMarker;
        _quickOpenOverlay.BorderStyleText = bundle.BorderStyleText;
        _quickOpenOverlay.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _quickOpenOverlay.SelectedItemStyle = theme.Selection.Background.WithBold();

        _automationSummary.ApplyTheme(theme);
        _automationSummary.BorderStyleText = bundle.BorderStyleText;
        _automationSummary.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
    }
}
