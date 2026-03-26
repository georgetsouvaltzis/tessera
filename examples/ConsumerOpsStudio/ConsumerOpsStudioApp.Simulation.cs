using TeaSharp;
using TeaSharp.Controls;

internal sealed partial class ConsumerOpsStudioApp
{
    private void AdvanceServiceSignals()
    {
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            var wave = Math.Sin((_tick + (index * 3)) / 8d) * 11d;
            var jitter = Noise(-8, 8);
            var pressure = service.QueueDepth / 420d;

            service.LatencyP95 = Clamp(service.BaselineLatency + wave + jitter + pressure, 32, 260);
            service.LatencyP99 = Clamp(service.LatencyP95 + 20 + Math.Abs(Noise(-4, 12)), 45, 320);
            service.ErrorRatePercent = Clamp(service.BaselineErrorRate + Math.Abs(Noise(-0.02, 0.18)) + (service.LatencyP95 > 150 ? 0.22 : 0), 0.01, 8.5);
            service.QueueDepth = (int)Math.Round(Clamp(service.QueueDepth + Noise(-65, 90) + (service.IsWriteFrozen ? -120 : 0), 0, 3900));
            service.ErrorBudgetRemaining = Clamp(service.ErrorBudgetRemaining - (service.ErrorRatePercent * 0.012), 3, 100);

            var previousStatus = service.Status;
            service.Status = service.IsWriteFrozen
                ? "Frozen"
                : service.ErrorRatePercent > 1.8 || service.LatencyP95 > 170
                    ? "Degraded"
                    : service.ErrorRatePercent > 0.8 || service.LatencyP95 > 130
                        ? "Warning"
                        : "Healthy";

            if (!string.Equals(previousStatus, service.Status, StringComparison.Ordinal))
            {
                _notifications.Push($"{service.Name} status -> {service.Status}", NotificationLevel.Info);
                AppendLog($"Service state changed: {service.Name} {previousStatus} -> {service.Status}.");
            }
        }
    }

    private void AdvanceIncidents()
    {
        for (var index = 0; index < _incidents.Count; index++)
        {
            var incident = _incidents[index];
            if (!incident.IsOpen)
            {
                continue;
            }

            incident.AgeMinutes++;
            if (incident.IsAcknowledged && incident.AgeMinutes > 40 && _tick % 16 == 0 && _random.NextDouble() > 0.55)
            {
                incident.IsOpen = false;
                _notifications.Push($"{incident.Id} resolved", NotificationLevel.Success);
                AppendLog($"Incident {incident.Id} resolved after mitigation.");
            }
        }

        if (_tick % 30 != 0)
        {
            return;
        }

        var candidate = ServiceWithHighestRisk();
        if (candidate is null || HasOpenIncident(candidate.Id))
        {
            return;
        }

        var incidentId = $"INC-{2800 + _incidents.Count + 1}";
        var severity = candidate.ErrorRatePercent > 3.0 ? "Sev1" : "Sev2";
        var summary = candidate.ErrorRatePercent > 3.0
            ? "Elevated error rate across checkout path"
            : "Sustained latency and queue pressure";

        _incidents.Insert(0, new IncidentTicket(incidentId, candidate.Id, summary, severity, candidate.Owner, ageMinutes: 0));
        _selectedIncidentId = incidentId;
        _notifications.Push($"New incident {incidentId} for {candidate.Name}", NotificationLevel.Warning);
        AppendLog($"Auto-generated {incidentId} for {candidate.Name} due to threshold breach.");
    }

    private void AdvanceDeployments()
    {
        for (var index = 0; index < _deployments.Count; index++)
        {
            var deployment = _deployments[index];
            deployment.AgeMinutes++;

            if (deployment.ProgressPercent >= 100)
            {
                deployment.Stage = "Completed";
                continue;
            }

            deployment.ProgressPercent = Math.Clamp(deployment.ProgressPercent + _random.Next(3, 14), 0, 100);
            deployment.Stage = deployment.ProgressPercent switch
            {
                < 30 => "Canary 10%",
                < 60 => "Canary 30%",
                < 85 => "Regional rollout",
                < 100 => "Final bake",
                _ => "Completed",
            };
        }
    }

    private void AppendPlotSample()
    {
        var service = FindService(_selectedServiceId) ?? _services[0];
        _p95Series.Append(service.LatencyP95);
        _p99Series.Append(service.LatencyP99);
        _errorSeries.Append(service.ErrorRatePercent * 40);
    }

    private void SyncServiceListSelection()
    {
        var previousServiceId = _selectedServiceId;
        var sorted = new List<ServiceSnapshot>(_services);
        sorted.Sort(static (left, right) =>
        {
            var queueCompare = right.QueueDepth.CompareTo(left.QueueDepth);
            return queueCompare != 0
                ? queueCompare
                : string.Compare(left.Name, right.Name, StringComparison.Ordinal);
        });

        _serviceList.SetItems(sorted);

        var selectedIndex = 0;
        for (var index = 0; index < sorted.Count; index++)
        {
            if (!string.Equals(sorted[index].Id, previousServiceId, StringComparison.Ordinal))
            {
                continue;
            }

            selectedIndex = index;
            break;
        }

        _serviceList.SetSelectedIndex(selectedIndex);
        _selectedServiceId = sorted[selectedIndex].Id;
    }

    private void RefreshWorkRows()
    {
        _visibleWorkItemIds.Clear();

        var rows = (OpsPanelTab)_tabs.SelectedIndex switch
        {
            OpsPanelTab.Incidents => BuildIncidentRows(),
            OpsPanelTab.Deployments => BuildDeploymentRows(),
            _ => BuildSloRows(),
        };

        _workTable.SetRows(rows);
    }

    private List<IReadOnlyList<string>> BuildIncidentRows()
    {
        var rows = new List<IReadOnlyList<string>>();
        for (var index = 0; index < _incidents.Count; index++)
        {
            var incident = _incidents[index];
            if (!IsVisibleForNavigation(incident.ServiceId))
            {
                continue;
            }

            var service = FindService(incident.ServiceId);
            if (service is null)
            {
                continue;
            }

            _visibleWorkItemIds.Add(incident.Id);
            rows.Add(
            [
                incident.Id,
                service.Name,
                incident.Severity,
                incident.Owner,
                incident.State,
                $"{incident.AgeMinutes,3}m  {incident.Summary}",
            ]);
        }

        if (rows.Count == 0)
        {
            rows.Add(["-", "-", "-", "-", "Empty", "No incidents in current navigation scope."]);
        }

        return rows;
    }

    private List<IReadOnlyList<string>> BuildDeploymentRows()
    {
        var rows = new List<IReadOnlyList<string>>();
        for (var index = 0; index < _deployments.Count; index++)
        {
            var deployment = _deployments[index];
            if (!IsVisibleForNavigation(deployment.ServiceId))
            {
                continue;
            }

            var service = FindService(deployment.ServiceId);
            if (service is null)
            {
                continue;
            }

            _visibleWorkItemIds.Add(deployment.Id);
            rows.Add(
            [
                deployment.Id,
                service.Name,
                deployment.Version,
                deployment.Owner,
                deployment.Stage,
                $"{deployment.ProgressPercent,3}%  age {deployment.AgeMinutes,3}m",
            ]);
        }

        if (rows.Count == 0)
        {
            rows.Add(["-", "-", "-", "-", "Empty", "No deployments in current navigation scope."]);
        }

        return rows;
    }

    private List<IReadOnlyList<string>> BuildSloRows()
    {
        var rows = new List<IReadOnlyList<string>>();
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            if (!IsVisibleForNavigation(service.Id))
            {
                continue;
            }

            _visibleWorkItemIds.Add(service.Id);
            var burnState = service.ErrorBudgetRemaining < 40
                ? "At Risk"
                : service.ErrorBudgetRemaining < 65
                    ? "Watch"
                    : "Stable";
            rows.Add(
            [
                $"SLO-{service.Id[4..]}",
                service.Name,
                $"{service.ErrorBudgetRemaining,5:0.0}%",
                "Auto",
                burnState,
                $"p95 {service.LatencyP95,5:0}ms  err {service.ErrorRatePercent,4:0.00}%",
            ]);
        }

        if (rows.Count == 0)
        {
            rows.Add(["-", "-", "-", "-", "Empty", "No services in current navigation scope."]);
        }

        return rows;
    }

    private void SelectServiceForNavigation()
    {
        string? serviceId = _activeNavigationId switch
        {
            "fulfillment" => "svc-fulfillment",
            "payments" => "svc-payments",
            "capacity" => ServiceWithHighestRisk()?.Id,
            _ => null,
        };

        if (string.IsNullOrEmpty(serviceId))
        {
            return;
        }

        _selectedServiceId = serviceId;
        SyncServiceListSelection();
    }

    private ServiceSnapshot? ServiceWithHighestRisk()
    {
        ServiceSnapshot? current = null;
        var currentScore = double.MinValue;
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            var score = (service.ErrorRatePercent * 100) + service.LatencyP95 + (service.QueueDepth / 8d);
            if (score <= currentScore)
            {
                continue;
            }

            current = service;
            currentScore = score;
        }

        return current;
    }

    private bool IsVisibleForNavigation(string serviceId)
    {
        return _activeNavigationId switch
        {
            "fulfillment" => string.Equals(serviceId, "svc-fulfillment", StringComparison.Ordinal),
            "payments" => string.Equals(serviceId, "svc-payments", StringComparison.Ordinal),
            _ => true,
        };
    }
}
