using TeaSharp.Controls;

internal sealed partial class ConsumerOpsStudioApp
{
    private void WireEvents()
    {
        _navigation.SelectionChanged += (_, args) =>
        {
            _activeNavigationId = args.SelectedItem?.Id ?? "overview";
            SelectServiceForNavigation();
            _statusText = $"navigation: {_activeNavigationId}";
            AppendLog($"Navigation changed -> {_activeNavigationId}");
        };

        _tabs.SelectionChanged += (_, args) =>
        {
            _statusText = $"tab: {args.SelectedItem}";
            _workTable.Title = $"{args.SelectedItem} Queue";
        };

        _serviceList.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _selectedServiceId = args.SelectedItem.Id;
            _statusText = $"service: {args.SelectedItem.Name}";
        };

        _workTable.SelectionChanged += (_, args) =>
        {
            var index = args.SelectedIndex;
            if (index < 0 || index >= _visibleWorkItemIds.Count)
            {
                return;
            }

            var selectedId = _visibleWorkItemIds[index];
            switch ((OpsPanelTab)_tabs.SelectedIndex)
            {
                case OpsPanelTab.Incidents:
                    _selectedIncidentId = selectedId;
                    _statusText = $"incident: {selectedId}";
                    break;
                case OpsPanelTab.Deployments:
                    _selectedDeploymentId = selectedId;
                    _statusText = $"deployment: {selectedId}";
                    break;
                case OpsPanelTab.Slo:
                    _selectedServiceId = selectedId;
                    _statusText = $"slo target: {selectedId}";
                    break;
            }
        };

        _commandBar.ItemActivated += (_, args) => PerformCommand(args.Item.Id);
        _ackButton.Activated += (_, _) => PerformCommand("ack");
        _rollbackButton.Activated += (_, _) => PerformCommand("rollback");
        _freezeButton.Activated += (_, _) => PerformCommand("freeze");

        _palette.ItemExecuted += (_, args) =>
        {
            PerformCommand(args.ItemId);
            _palette.Close();
        };

        _confirmDialog.Accepted += (_, _) => HandleDialogClose(accepted: true);
        _confirmDialog.Dismissed += (_, _) => HandleDialogClose(accepted: false);
    }

    private void PerformCommand(string commandId)
    {
        switch (commandId)
        {
            case "ack":
                AcknowledgeSelectedIncident();
                break;
            case "rollback":
                RequestRollback();
                break;
            case "scale":
                ScaleSelectedService();
                break;
            case "freeze":
                RequestToggleFreeze();
                break;
            case "palette":
                _palette.Open();
                _statusText = "palette open";
                break;
            case "theme":
                ToggleTheme();
                break;
            case "nav:overview":
                _navigation.SetSelectedIndex(0);
                break;
            case "nav:payments":
                _navigation.SetSelectedIndex(2);
                break;
            case "nav:capacity":
                _navigation.SetSelectedIndex(3);
                break;
        }
    }

    private void AcknowledgeSelectedIncident()
    {
        if ((OpsPanelTab)_tabs.SelectedIndex != OpsPanelTab.Incidents)
        {
            _notifications.Push("Switch to Incidents tab to acknowledge.", NotificationLevel.Warning);
            return;
        }

        var incident = FindIncident(_selectedIncidentId);
        if (incident is null || !incident.IsOpen)
        {
            _notifications.Push("No open incident selected.", NotificationLevel.Warning);
            return;
        }

        if (incident.IsAcknowledged)
        {
            _notifications.Push($"{incident.Id} already acknowledged.", NotificationLevel.Info);
            return;
        }

        incident.IsAcknowledged = true;
        _statusText = $"acknowledged {incident.Id}";
        _notifications.Push($"{incident.Id} acknowledged by lane-A", NotificationLevel.Success);
        AppendLog($"Incident {incident.Id} acknowledged.");
    }

    private void RequestRollback()
    {
        var service = FindService(_selectedServiceId);
        if (service is null)
        {
            return;
        }

        _pendingDialogAction = PendingDialogAction.Rollback;
        _pendingServiceId = service.Id;
        _confirmDialog.Show(
            "Rollback Selected Service",
            $"Service: {service.Name} ({service.Region})",
            "Enter confirms rollback deployment.",
            "Esc cancels.");
    }

    private void RequestToggleFreeze()
    {
        var service = FindService(_selectedServiceId);
        if (service is null)
        {
            return;
        }

        _pendingDialogAction = PendingDialogAction.ToggleFreeze;
        _pendingServiceId = service.Id;
        var verb = service.IsWriteFrozen ? "Unfreeze" : "Freeze";
        _confirmDialog.Show(
            $"{verb} Writes",
            $"Service: {service.Name} ({service.Region})",
            $"Enter confirms {verb.ToLowerInvariant()} action.",
            "Esc cancels.");
    }

    private void HandleDialogClose(bool accepted)
    {
        if (!accepted)
        {
            _statusText = "action cancelled";
            _pendingDialogAction = PendingDialogAction.None;
            _pendingServiceId = string.Empty;
            return;
        }

        var service = FindService(_pendingServiceId);
        if (service is null)
        {
            _pendingDialogAction = PendingDialogAction.None;
            _pendingServiceId = string.Empty;
            return;
        }

        if (_pendingDialogAction == PendingDialogAction.Rollback)
        {
            var nextId = $"DEP-{7700 + _deployments.Count + 1}";
            _deployments.Add(new DeploymentRun(nextId, service.Id, "rollback", "lane-A", ageMinutes: 0, progressPercent: 0, stage: "Rollback queued"));
            _notifications.Push($"Rollback scheduled for {service.Name}", NotificationLevel.Warning);
            AppendLog($"Rollback deployment queued for {service.Name}.");
            _statusText = $"rollback queued: {service.Name}";
        }
        else if (_pendingDialogAction == PendingDialogAction.ToggleFreeze)
        {
            service.IsWriteFrozen = !service.IsWriteFrozen;
            _notifications.Push(
                service.IsWriteFrozen
                    ? $"{service.Name} write freeze enabled"
                    : $"{service.Name} write freeze cleared",
                service.IsWriteFrozen ? NotificationLevel.Warning : NotificationLevel.Success);
            AppendLog($"{service.Name} write freeze -> {(service.IsWriteFrozen ? "enabled" : "disabled")}.");
            _statusText = service.IsWriteFrozen ? "write freeze enabled" : "write freeze disabled";
        }

        _pendingDialogAction = PendingDialogAction.None;
        _pendingServiceId = string.Empty;
    }

    private void ScaleSelectedService()
    {
        var service = FindService(_selectedServiceId);
        if (service is null)
        {
            return;
        }

        service.QueueDepth = Math.Max(0, service.QueueDepth - 220);
        service.ErrorRatePercent = Math.Max(0.01, service.ErrorRatePercent * 0.91);
        _notifications.Push($"Scaled +1 worker shard for {service.Name}", NotificationLevel.Info);
        AppendLog($"Scaling action applied to {service.Name}; queue pressure reduced.");
        _statusText = $"scaled: {service.Name}";
    }

    private void ToggleTheme()
    {
        _alertThemeEnabled = !_alertThemeEnabled;
        ApplyLocalOverrides();
        _notifications.Push(
            _alertThemeEnabled ? "Alert theme enabled (local overrides)." : "Default theme enabled.",
            NotificationLevel.Info);
        _statusText = _alertThemeEnabled ? "theme: alert" : "theme: default";
    }
}
