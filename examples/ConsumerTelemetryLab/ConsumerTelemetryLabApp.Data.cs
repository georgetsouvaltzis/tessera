using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;

internal sealed partial class ConsumerTelemetryLabApp
{
    private void SimulateTick(DateTimeOffset now)
    {
        var profileMultiplier = _profile == LoadProfile.Incident ? 1.35 : 1.0;

        foreach (var service in _services)
        {
            var serviceBias = service.Id switch
            {
                "api" => 1.0,
                "billing" => 1.15,
                "search" => 1.2,
                "cache" => 0.8,
                _ => 0.95,
            };

            var wave = 0.5 + (Math.Sin((_tick + service.Name.Length) / 8d) * 0.5);
            var burst = _profile == LoadProfile.Incident && _tick % 16 is > 7 and < 13 ? 18 : 0;

            service.Cpu = Clamp(20 + (wave * 55 * serviceBias * profileMultiplier) + burst + Noise(5), 3, 100);
            service.MemoryGb = Clamp(5 + (service.Cpu * 0.29) + Noise(1.1), 2, 64);
            service.P95Ms = Clamp(12 + (service.Cpu * 1.7 * serviceBias) + burst + Noise(8), 6, 420);
            service.ErrorRatePct = Clamp((service.P95Ms / 180d) + (service.Cpu / 220d) + Noise(0.2), 0, 18);
            service.ReqPerSec = Clamp(80 + (service.Cpu * 9 * serviceBias) + Noise(30), 20, 2200);

            service.Health = service.ErrorRatePct switch
            {
                > 4.5 => "Degraded",
                > 1.8 => "Warning",
                _ => "Healthy",
            };

            if (service.P95Ms > 190 && service.ErrorRatePct > 2.5 && _random.NextDouble() < 0.04)
            {
                OpenIncident(service, now);
            }
        }

        if (_incidents.Count > 0 && _random.NextDouble() < 0.05)
        {
            ResolveOldestOpenIncident(now);
        }

        var selected = SelectedService();
        if (selected is not null)
        {
            AppendPlots(selected);
        }

        PruneIncidents(now);
    }

    private void RefreshListsAndTables()
    {
        var filteredServices = FilteredServices().ToList();
        _serviceList.SetItems(filteredServices);
        SyncServiceListSelection();

        _serviceTable.SetRows(filteredServices.Select(static service =>
            (IReadOnlyList<string>)
            [
                service.Name,
                service.Health,
                service.Cpu.ToString("0.0", CultureInfo.InvariantCulture),
                service.MemoryGb.ToString("0.0", CultureInfo.InvariantCulture),
                service.P95Ms.ToString("0", CultureInfo.InvariantCulture),
                service.ErrorRatePct.ToString("0.00", CultureInfo.InvariantCulture),
                service.ReqPerSec.ToString("0", CultureInfo.InvariantCulture),
            ]));

        var incidentRows = SortedIncidentsForTable();
        _incidentTable.SetRows(incidentRows.Select(static incident =>
            (IReadOnlyList<string>)
            [
                incident.Id,
                incident.Severity,
                incident.ServiceName,
                incident.State,
                $"{incident.MinutesOpen}m",
                incident.Summary,
            ]));

        if (!string.IsNullOrWhiteSpace(_selectedIncidentId))
        {
            var selectedIndex = incidentRows.FindIndex(incident =>
                string.Equals(incident.Id, _selectedIncidentId, StringComparison.Ordinal));
            if (selectedIndex >= 0)
            {
                _incidentTable.SetSelectedIndex(selectedIndex);
            }
        }
    }

    private IEnumerable<ServiceState> FilteredServices()
    {
        return _activeCluster == "all"
            ? _services
            : _services.Where(service => string.Equals(service.Cluster, _activeCluster, StringComparison.Ordinal));
    }

    private List<IncidentState> SortedIncidentsForTable()
    {
        return _incidents
            .OrderByDescending(static incident => incident.State == "Open")
            .ThenByDescending(static incident => incident.SeverityRank)
            .ThenByDescending(static incident => incident.MinutesOpen)
            .ToList();
    }

    private void SyncServiceListSelection()
    {
        var filtered = FilteredServices().ToList();
        var index = filtered.FindIndex(service => string.Equals(service.Id, _selectedServiceId, StringComparison.Ordinal));
        if (index >= 0)
        {
            _serviceList.SetSelectedIndex(index);
            return;
        }

        if (filtered.Count == 0)
        {
            return;
        }

        _selectedServiceId = filtered[0].Id;
        _serviceList.SetSelectedIndex(0);
    }

    private void SelectNextService()
    {
        var filtered = FilteredServices().ToList();
        if (filtered.Count == 0)
        {
            return;
        }

        var current = filtered.FindIndex(service => string.Equals(service.Id, _selectedServiceId, StringComparison.Ordinal));
        var next = current < 0 ? 0 : (current + 1) % filtered.Count;
        _selectedServiceId = filtered[next].Id;
        _serviceList.SetSelectedIndex(next);
        TryRequestIncidentDrilldownForService(_selectedServiceId, "next-service");
    }

    private ServiceState? SelectedService()
    {
        return _services.FirstOrDefault(service => string.Equals(service.Id, _selectedServiceId, StringComparison.Ordinal));
    }

    private void TryRequestIncidentDrilldownForService(string serviceId, string reason)
    {
        var incident = MostRecentIncidentForService(serviceId);
        if (incident is null)
        {
            return;
        }

        RequestIncidentDrilldown(incident.Id, reason);
    }

    private IncidentState? MostRecentIncidentForService(string serviceId)
    {
        return _incidents
            .Where(incident => string.Equals(incident.ServiceId, serviceId, StringComparison.Ordinal))
            .OrderByDescending(static incident => incident.State == "Open")
            .ThenByDescending(static incident => incident.MinutesOpen)
            .FirstOrDefault();
    }

    private void RequestIncidentDrilldown(string incidentId, string source)
    {
        _selectedIncidentId = incidentId;
        RefreshListsAndTables();
        _tableSyncNote = $"table sync: SetSelectedIndex({incidentId}) via {source}";
    }

    private string BuildIncidentDetail(ScreenContext context)
    {
        var incident = _incidents.FirstOrDefault(item => string.Equals(item.Id, _selectedIncidentId, StringComparison.Ordinal));
        if (incident is null)
        {
            return "No incident selected.\n\nSelect an alert or click a row in Incident Queue.";
        }

        return string.Join('\n',
            $"{incident.Id}  {incident.Severity}  {incident.State}",
            $"Service: {incident.ServiceName} ({incident.ServiceId})",
            $"Owner: {incident.Owner}",
            $"Opened: {incident.StartedAt:HH:mm:ss}  Age: {incident.MinutesOpen}m",
            $"Summary: {incident.Summary}",
            $"Runbook: {incident.Runbook}",
            $"View: {_tabs.Items[_tabs.SelectedIndex]}");
    }

    private void SeedInitialMetrics()
    {
        foreach (var service in _services)
        {
            service.Cpu = 24 + Noise(3);
            service.MemoryGb = 7 + Noise(1);
            service.P95Ms = 26 + Noise(3);
            service.ErrorRatePct = 0.4 + Noise(0.1);
            service.ReqPerSec = 180 + Noise(25);
            service.Health = "Healthy";
        }
    }

    private void SeedInitialIncidents()
    {
        var billing = _services.First(static service => service.Id == "billing");
        var scheduler = _services.First(static service => service.Id == "scheduler");

        _incidents.Add(new IncidentState($"I{_nextIncident++:0000}", billing.Id, billing.Name, "High", "Open", "retry storm in invoice processor", "oncall-billing", "billing-retry-runbook", 11, DateTimeOffset.UtcNow.AddMinutes(-11)));
        _incidents.Add(new IncidentState($"I{_nextIncident++:0000}", scheduler.Id, scheduler.Name, "Medium", "Investigating", "cron backlog past SLO", "oncall-platform", "scheduler-backlog-runbook", 7, DateTimeOffset.UtcNow.AddMinutes(-7)));
        _selectedIncidentId = _incidents[0].Id;
    }

    private void OpenIncident(ServiceState service, DateTimeOffset now)
    {
        if (_incidents.Any(item => item.State == "Open" && string.Equals(item.ServiceId, service.Id, StringComparison.Ordinal)))
        {
            return;
        }

        var severity = service.ErrorRatePct > 6 ? "Critical" : service.ErrorRatePct > 3 ? "High" : "Medium";
        var summary = $"p95 {service.P95Ms:0}ms / err {service.ErrorRatePct:0.0}% exceeds budget";
        var owner = service.Cluster switch
        {
            "prod-eu" => "oncall-eu",
            "edge" => "oncall-edge",
            _ => "oncall-us",
        };

        var incident = new IncidentState(
            $"I{_nextIncident++:0000}",
            service.Id,
            service.Name,
            severity,
            "Open",
            summary,
            owner,
            $"runbook-{service.Id}",
            0,
            now);

        _incidents.Add(incident);
        _alerts.Add(new InboxItem($"inc:{incident.Id}", $"{incident.Id} {incident.ServiceName}: {incident.Summary}", NotificationLevel.Error, now, source: "detector"));
        _activity.Append($"incident opened -> {incident.Id} {incident.ServiceName}");
    }

    private void ResolveOldestOpenIncident(DateTimeOffset now)
    {
        var incident = _incidents
            .Where(static item => item.State == "Open")
            .OrderByDescending(static item => item.MinutesOpen)
            .FirstOrDefault();
        if (incident is null)
        {
            return;
        }

        incident.State = "Resolved";
        _alerts.Add(new InboxItem($"inc:{incident.Id}", $"{incident.Id} resolved ({incident.ServiceName})", NotificationLevel.Success, now, source: "auto-heal", isRead: false));
        _activity.Append($"incident resolved -> {incident.Id}");
    }

    private void PruneIncidents(DateTimeOffset now)
    {
        foreach (var incident in _incidents)
        {
            incident.MinutesOpen = Math.Max(0, (int)Math.Round((now - incident.StartedAt).TotalMinutes, MidpointRounding.AwayFromZero));
        }

        _incidents.RemoveAll(static incident => incident.State == "Resolved" && incident.MinutesOpen > 60);
    }

    private void AppendPlots(ServiceState selected)
    {
        _cpuTrend.Append(selected.Cpu);
        _memoryTrend.Append(selected.MemoryGb);

        var p50 = Clamp(selected.P95Ms * 0.42 + Noise(2.5), 4, selected.P95Ms);
        var p99 = Clamp(selected.P95Ms + Math.Abs(Noise(20)), selected.P95Ms, 500);

        _latP50.Append(p50);
        _latP95.Append(selected.P95Ms);
        _latP99.Append(p99);

        _jitterPlot.Append(new ScatterPlotPoint(_tick, selected.P95Ms, _tick % 20 == 0 ? $"{selected.P95Ms:0}" : null));

        var warning = Clamp(selected.ErrorRatePct * 3.2, 0, 100);
        var critical = Clamp(selected.ErrorRatePct * 1.7, 0, 100);
        var ok = Clamp(100 - warning - critical, 0, 100);
        _errorMix.SetBuckets([
            new HistogramBucket("ok", ok),
            new HistogramBucket("warn", warning),
            new HistogramBucket("crit", critical),
        ]);
    }

    private void ResetTelemetryAndIncidents()
    {
        _incidents.Clear();
        _alerts.Clear();
        _activity.Clear();

        _cpuTrend.Clear();
        _memoryTrend.Clear();
        _latP50.Clear();
        _latP95.Clear();
        _latP99.Clear();
        _jitterPlot.Clear();
        _errorMix.Clear();

        SeedInitialMetrics();
        SeedInitialIncidents();
        RefreshListsAndTables();

        _alerts.Push("state reset complete", NotificationLevel.Info, $"sys:reset:{_tick}");
    }

    private double Noise(double amplitude)
    {
        return (_random.NextDouble() - 0.5d) * amplitude * 2d;
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Clamp(value, min, max);
    }
}
