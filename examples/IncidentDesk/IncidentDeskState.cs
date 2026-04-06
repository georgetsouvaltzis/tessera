using Tessera.Controls;

namespace Tessera.Examples.IncidentDesk;

internal enum IncidentSeverity
{
    Critical,
    High,
    Medium,
    Low,
}

internal enum IncidentStatus
{
    Investigating,
    Acknowledged,
    Escalated,
    Monitoring,
    Resolved,
}

internal readonly record struct IncidentDeckSummary(int OpenCount, int CriticalCount, int EscalatedCount, int ActiveResponders);

internal sealed class IncidentRecord
{
    public required string Id { get; init; }
    public required string Summary { get; set; }
    public required string Service { get; init; }
    public required string Environment { get; init; }
    public required string Region { get; init; }
    public required string Commander { get; set; }
    public required string Channel { get; init; }
    public required string CustomerImpact { get; set; }
    public required string Hypothesis { get; set; }
    public required string Runbook { get; init; }
    public required string CurrentPhase { get; set; }
    public required DateTimeOffset OpenedAt { get; init; }
    public required DateTimeOffset SlaDeadline { get; set; }
    public required IncidentSeverity Severity { get; set; }
    public required IncidentStatus Status { get; set; }
    public required string PrimaryOwner { get; set; }
    public required string DraftNotes { get; set; }
    public required List<string> Responders { get; init; }
    public required List<ActivityFeedItem> Timeline { get; init; }
    public required List<LogEntry> Logs { get; init; }
    public bool HasUnreadUpdate { get; set; } = true;
    public bool IsPinned { get; set; }
}

internal sealed class IncidentDeskState
{
    private static readonly string[] OwnerRotation = ["Mira", "Ishan", "Nika", "Sora", "Wei", "Dana"];
    private readonly List<IncidentRecord> _incidents;
    private int _selectedIndex;
    private int _assignCursor = 1;

    private IncidentDeskState(List<IncidentRecord> incidents)
    {
        _incidents = incidents;
        SortIncidents();
    }

    public IReadOnlyList<IncidentRecord> Incidents => _incidents;

    public IncidentRecord SelectedIncident => _incidents[_selectedIndex];

    public int SelectedIndex => _selectedIndex;

    public string LastCommand { get; private set; } = "Queue synchronized with seeded ops pressure.";

    public static IncidentDeskState CreateSeed()
    {
        var now = DateTimeOffset.UtcNow;
        return new IncidentDeskState(
        [
            CreateIncident(
                now,
                "INC-1042",
                "payments-api latency spike",
                IncidentSeverity.Critical,
                IncidentStatus.Investigating,
                "Mira",
                "Checkout retries crossing 18% in prod; card authorizations timing out for EU shoppers.",
                "Redis shard saturation after last rules rollout is starving the card-risk cache.",
                "prod",
                "us-east-1",
                "payments-api",
                "war-room-payments",
                "Runbook /payments/latency",
                "Stabilize p95 under 950ms before regional failover is triggered.",
                14,
                ["Mira", "Nika", "Arun"],
                [
                    Timeline("auto-triage", "opened", "latency guard", "Checkout p95 breached 4.2s in 3 regions.", ActivityFeedItemKind.Error, now.AddMinutes(-18)),
                    Timeline("Mira", "paged", "payments-oncall", "Primary on-call acknowledged within 48 seconds.", ActivityFeedItemKind.Warning, now.AddMinutes(-17)),
                    Timeline("Nika", "isolated", "redis shard 04", "Connection pool utilization holding at 97%.", ActivityFeedItemKind.Warning, now.AddMinutes(-12)),
                    Timeline("Arun", "started", "regional replay", "Synthetic orders reproducing timeout curve.", ActivityFeedItemKind.Info, now.AddMinutes(-8)),
                ],
                [
                    Log("Timeouts crossing 3,100 req/min on /charge", LogLevel.Critical, "edge-api", now.AddMinutes(-7)),
                    Log("Cache miss storm detected on risk-profile reads", LogLevel.Error, "payments-api", now.AddMinutes(-6)),
                    Log("Fallback hedging enabled for vip-checkout cohort", LogLevel.Warning, "traffic-router", now.AddMinutes(-4)),
                ],
                """
                - Confirm redis shard pressure with data infra.
                - Keep customer comms in draft-only mode until retries stabilize.
                - Prepare read-only checkout fallback if p95 stays above 4s.
                """,
                isPinned: true),
            CreateIncident(
                now,
                "INC-1038",
                "auth token issuer rollback drift",
                IncidentSeverity.High,
                IncidentStatus.Escalated,
                "Ishan",
                "Mobile sessions are failing silent refresh after the issuer rollback; sign-ins degraded for premium users.",
                "Old issuer metadata is still cached on two edge clusters and breaking kid resolution.",
                "prod",
                "eu-west-1",
                "identity-broker",
                "war-room-auth",
                "Runbook /identity/issuer-rollback",
                "Purge stale metadata and verify refresh-token path on iOS and Android.",
                28,
                ["Ishan", "Lia", "Jon"],
                [
                    Timeline("auto-triage", "opened", "refresh token alarms", "Silent refresh failure rate crossed 22%.", ActivityFeedItemKind.Error, now.AddMinutes(-46)),
                    Timeline("Ishan", "escalated", "identity lead", "Requested issuer metadata purge on EU edge.", ActivityFeedItemKind.Warning, now.AddMinutes(-34)),
                    Timeline("Lia", "verified", "android client", "Manual sign-in succeeds; refresh path still fails.", ActivityFeedItemKind.Info, now.AddMinutes(-27)),
                ],
                [
                    Log("issuer kid mismatch on cached jwks bundle", LogLevel.Error, "identity-broker", now.AddMinutes(-29)),
                    Log("edge metadata purge job queued for 2 clusters", LogLevel.Warning, "deploy-runner", now.AddMinutes(-21)),
                    Log("ios refresh probes recovering in dublin", LogLevel.Info, "synthetic-monitor", now.AddMinutes(-8)),
                ],
                """
                - Keep app-release managers in the loop before forcing logout.
                - Watch silent refresh in Dublin and Frankfurt separately.
                """),
            CreateIncident(
                now,
                "INC-1029",
                "broker disk pressure on event spine",
                IncidentSeverity.High,
                IncidentStatus.Acknowledged,
                "Wei",
                "Backlog pressure building on the analytics ingest spine; downstream dashboards 15m behind.",
                "Topic retention compaction lag plus a failed rebalance is pinning broker 7 at 91% disk.",
                "prod",
                "us-central-1",
                "event-spine",
                "war-room-streams",
                "Runbook /kafka/disk-pressure",
                "Drain hot partitions before disk watermark blocks producers.",
                37,
                ["Wei", "Daria", "Sam"],
                [
                    Timeline("auto-triage", "opened", "disk watermark", "Broker 7 crossed 90% usage.", ActivityFeedItemKind.Warning, now.AddMinutes(-59)),
                    Timeline("Wei", "acknowledged", "streams-oncall", "Partition reassignment prepared.", ActivityFeedItemKind.Success, now.AddMinutes(-53)),
                    Timeline("Daria", "paused", "cold analytics jobs", "Reduced non-critical ingest by 30%.", ActivityFeedItemKind.Info, now.AddMinutes(-41)),
                ],
                [
                    Log("rebalance skipped for partition analytics-44", LogLevel.Warning, "kafka-admin", now.AddMinutes(-39)),
                    Log("disk watermark relief target 84% after migration", LogLevel.Info, "ops-bot", now.AddMinutes(-15)),
                ],
                """
                - Hold BI stakeholders on yellow status.
                - If compaction lag grows, pause replay consumers first.
                """),
            CreateIncident(
                now,
                "INC-1022",
                "edge cache purge drift",
                IncidentSeverity.Medium,
                IncidentStatus.Monitoring,
                "Dana",
                "Product catalog serving stale price tiles for a subset of LATAM traffic after purge wave.",
                "Catalog purge fan-out hit a stale route map and skipped 3 PoPs.",
                "prod",
                "sa-east-1",
                "catalog-edge",
                "war-room-catalog",
                "Runbook /catalog/cache-purge",
                "Verify fresh prices and keep drift below 2% of requests.",
                46,
                ["Dana", "Mina"],
                [
                    Timeline("auto-triage", "opened", "catalog drift", "Price mismatch reports from Sao Paulo edge.", ActivityFeedItemKind.Warning, now.AddMinutes(-84)),
                    Timeline("Dana", "redirected", "purge map", "Forced regional invalidation replay.", ActivityFeedItemKind.Success, now.AddMinutes(-68)),
                    Timeline("Mina", "confirmed", "latam probes", "Freshness recovered to 98.7%.", ActivityFeedItemKind.Success, now.AddMinutes(-25)),
                ],
                [
                    Log("catalog warmers caught up in gru/scl/lim", LogLevel.Info, "catalog-edge", now.AddMinutes(-22)),
                    Log("stale price sightings now under threshold", LogLevel.Info, "synthetic-monitor", now.AddMinutes(-9)),
                ],
                """
                - Keep merchandising informed until freshness stays green for 30m.
                - Capture stale route map regression in postmortem.
                """),
            CreateIncident(
                now,
                "INC-1016",
                "push worker backlog on mobile fanout",
                IncidentSeverity.Medium,
                IncidentStatus.Investigating,
                "Sora",
                "Transactional push delivery delayed up to 12 minutes for APAC users.",
                "One notification shard is hot after a traffic promotion and worker concurrency is capped.",
                "prod",
                "ap-southeast-1",
                "push-orchestrator",
                "war-room-push",
                "Runbook /push/backlog",
                "Restore p90 delivery under 90 seconds before merchant sunset campaigns start.",
                51,
                ["Sora", "Ben"],
                [
                    Timeline("auto-triage", "opened", "fanout delay", "Delivery latency crossed 11m.", ActivityFeedItemKind.Warning, now.AddMinutes(-31)),
                    Timeline("Sora", "sampled", "hot shard", "Shard 3 holding 74% of in-flight work.", ActivityFeedItemKind.Info, now.AddMinutes(-18)),
                ],
                [
                    Log("worker concurrency cap still 24 on shard 3", LogLevel.Warning, "push-orchestrator", now.AddMinutes(-17)),
                    Log("merchant promo segment generating burst writes", LogLevel.Info, "audience-service", now.AddMinutes(-14)),
                ],
                """
                - Consider temporary shard split if queue depth stays above 80k.
                - Alert merchant ops before sunset wave starts.
                """),
            CreateIncident(
                now,
                "INC-1008",
                "reporting warehouse failover audit",
                IncidentSeverity.Low,
                IncidentStatus.Resolved,
                "Nika",
                "Warehouse follower promoted cleanly; audit trail still missing 3 low-risk sync markers.",
                "Background auditor skipped three checkpoints during the managed failover.",
                "staging",
                "us-west-2",
                "reporting-warehouse",
                "war-room-warehouse",
                "Runbook /warehouse/failover",
                "Backfill audit markers and close comms without reopening customer notice.",
                68,
                ["Nika", "Paul"],
                [
                    Timeline("auto-triage", "opened", "audit mismatch", "Follower promotion completed with minor audit drift.", ActivityFeedItemKind.Info, now.AddMinutes(-123)),
                    Timeline("Nika", "resolved", "backfill job", "Audit markers replayed successfully.", ActivityFeedItemKind.Success, now.AddMinutes(-73)),
                ],
                [
                    Log("audit replay complete for sync windows 02:18-02:24", LogLevel.Info, "warehouse-auditor", now.AddMinutes(-72)),
                    Log("customer-facing impact: none", LogLevel.Debug, "status-bot", now.AddMinutes(-70)),
                ],
                """
                - Resolved. Keep notes for release-readiness evidence.
                """),
        ]);
    }

    public bool SelectIncident(string incidentId)
    {
        var index = _incidents.FindIndex(incident => string.Equals(incident.Id, incidentId, StringComparison.Ordinal));
        if (index < 0 || index == _selectedIndex)
        {
            return false;
        }

        _selectedIndex = index;
        _incidents[_selectedIndex].HasUnreadUpdate = false;
        return true;
    }

    public void CaptureDraft(string draft) => SelectedIncident.DraftNotes = draft ?? string.Empty;

    public string AcknowledgeSelected()
    {
        var incident = SelectedIncident;
        if (incident.Status == IncidentStatus.Resolved)
        {
            return LastCommand = $"{incident.Id} is resolved; reopen before acknowledging.";
        }

        incident.Status = IncidentStatus.Acknowledged;
        incident.HasUnreadUpdate = false;
        incident.CurrentPhase = "Responder roles locked and mitigation underway.";
        AddEvent(incident, "desk", "acknowledged", "incident", "Command deck acknowledged the page and locked response roles.", ActivityFeedItemKind.Success, LogLevel.Info, "incident-desk");
        SortIncidents();
        return LastCommand = $"{incident.Id} acknowledged and pinned to active response.";
    }

    public string AssignSelected()
    {
        var incident = SelectedIncident;
        incident.PrimaryOwner = OwnerRotation[_assignCursor % OwnerRotation.Length];
        _assignCursor++;
        if (!incident.Responders.Contains(incident.PrimaryOwner, StringComparer.Ordinal))
        {
            incident.Responders.Insert(0, incident.PrimaryOwner);
        }

        incident.HasUnreadUpdate = true;
        AddEvent(incident, "desk", "assigned", incident.PrimaryOwner, $"Ownership moved to {incident.PrimaryOwner} for the next command loop.", ActivityFeedItemKind.Info, LogLevel.Info, "incident-desk");
        SortIncidents();
        return LastCommand = $"{incident.Id} reassigned to {incident.PrimaryOwner}.";
    }

    public string EscalateSelected()
    {
        var incident = SelectedIncident;
        incident.Status = IncidentStatus.Escalated;
        incident.IsPinned = true;
        incident.HasUnreadUpdate = true;
        if (incident.Severity != IncidentSeverity.Critical)
        {
            incident.Severity--;
        }

        incident.CurrentPhase = "Incident command escalated; cross-functional leads requested.";
        incident.SlaDeadline = incident.SlaDeadline.AddMinutes(-6);
        AddEvent(incident, "desk", "escalated", "incident command", "Cross-functional bridge opened and SLA window tightened.", ActivityFeedItemKind.Error, LogLevel.Critical, "incident-desk");
        SortIncidents();
        return LastCommand = $"{incident.Id} escalated to {SeverityText(incident.Severity)} response.";
    }

    public string ResolveSelected()
    {
        var incident = SelectedIncident;
        incident.Status = IncidentStatus.Resolved;
        incident.HasUnreadUpdate = false;
        incident.CurrentPhase = "Recovery verified; watch window held for clean exit.";
        AddEvent(incident, "desk", "resolved", "watch window", "Primary metrics recovered and customer impact cleared.", ActivityFeedItemKind.Success, LogLevel.Info, "incident-desk");
        SortIncidents();
        return LastCommand = $"{incident.Id} moved to resolved.";
    }

    public string ReopenSelected()
    {
        var incident = SelectedIncident;
        incident.Status = IncidentStatus.Investigating;
        if (incident.Severity == IncidentSeverity.Low)
        {
            incident.Severity = IncidentSeverity.Medium;
        }

        incident.HasUnreadUpdate = true;
        incident.CurrentPhase = "Signal regressed after watch window; mitigation reopened.";
        AddEvent(incident, "desk", "reopened", "response bridge", "Monitoring signal slipped and active investigation resumed.", ActivityFeedItemKind.Warning, LogLevel.Warning, "incident-desk");
        SortIncidents();
        return LastCommand = $"{incident.Id} reopened for active investigation.";
    }

    public string SyncSelected()
    {
        var incident = SelectedIncident;
        incident.HasUnreadUpdate = true;
        AddEvent(
            incident,
            "ops-bot",
            "synced",
            incident.Service,
            $"{incident.Service} telemetry refreshed; {incident.CurrentPhase}",
            incident.Status == IncidentStatus.Resolved ? ActivityFeedItemKind.Success : ActivityFeedItemKind.Info,
            incident.Severity == IncidentSeverity.Critical ? LogLevel.Error : LogLevel.Info,
            "ops-bot");
        return LastCommand = $"{incident.Id} synchronized with latest simulated telemetry.";
    }

    public IncidentDeckSummary BuildSummary()
    {
        var open = _incidents.Count(incident => incident.Status != IncidentStatus.Resolved);
        var critical = _incidents.Count(incident => incident.Severity == IncidentSeverity.Critical && incident.Status != IncidentStatus.Resolved);
        var escalated = _incidents.Count(incident => incident.Status == IncidentStatus.Escalated);
        var responders = _incidents
            .SelectMany(incident => incident.Responders)
            .Distinct(StringComparer.Ordinal)
            .Count();
        return new IncidentDeckSummary(open, critical, escalated, responders);
    }

    public static string SeverityText(IncidentSeverity severity) => severity switch
    {
        IncidentSeverity.Critical => "SEV1",
        IncidentSeverity.High => "SEV2",
        IncidentSeverity.Medium => "SEV3",
        _ => "SEV4",
    };

    public static string StatusText(IncidentStatus status) => status switch
    {
        IncidentStatus.Investigating => "Investigating",
        IncidentStatus.Acknowledged => "Acknowledged",
        IncidentStatus.Escalated => "Escalated",
        IncidentStatus.Monitoring => "Monitoring",
        _ => "Resolved",
    };

    public static NotificationLevel NotificationLevel(IncidentSeverity severity) => severity switch
    {
        IncidentSeverity.Critical => Controls.NotificationLevel.Error,
        IncidentSeverity.High => Controls.NotificationLevel.Warning,
        IncidentSeverity.Medium => Controls.NotificationLevel.Info,
        _ => Controls.NotificationLevel.Success,
    };

    public static string SlaText(IncidentRecord incident)
    {
        var remaining = incident.SlaDeadline - DateTimeOffset.UtcNow;
        var minutes = Math.Max(0, (int)Math.Round(remaining.TotalMinutes, MidpointRounding.AwayFromZero));
        return $"SLA {minutes:00}m";
    }

    private static IncidentRecord CreateIncident(
        DateTimeOffset now,
        string id,
        string summary,
        IncidentSeverity severity,
        IncidentStatus status,
        string owner,
        string impact,
        string hypothesis,
        string environment,
        string region,
        string service,
        string channel,
        string runbook,
        string phase,
        int openedMinutesAgo,
        string[] responders,
        List<ActivityFeedItem> timeline,
        List<LogEntry> logs,
        string draftNotes,
        bool isPinned = false)
    {
        return new IncidentRecord
        {
            Id = id,
            Summary = summary,
            Severity = severity,
            Status = status,
            PrimaryOwner = owner,
            Commander = "Ops Desk",
            CustomerImpact = impact,
            Hypothesis = hypothesis,
            Environment = environment,
            Region = region,
            Service = service,
            Channel = channel,
            Runbook = runbook,
            CurrentPhase = phase,
            OpenedAt = now.AddMinutes(-openedMinutesAgo),
            SlaDeadline = now.AddMinutes(Math.Max(12, 75 - openedMinutesAgo)),
            Responders = responders.ToList(),
            Timeline = timeline,
            Logs = logs,
            DraftNotes = draftNotes.TrimEnd(),
            IsPinned = isPinned,
        };
    }

    private static ActivityFeedItem Timeline(string actor, string action, string target, string details, ActivityFeedItemKind kind, DateTimeOffset timestamp)
    {
        return new ActivityFeedItem(actor, action, target, details, kind, timestamp) { IsUnread = kind is ActivityFeedItemKind.Warning or ActivityFeedItemKind.Error };
    }

    private static LogEntry Log(string message, LogLevel level, string source, DateTimeOffset timestamp)
    {
        return new LogEntry(message, level, timestamp, source) { HasError = level >= LogLevel.Error };
    }

    private static void AddEvent(
        IncidentRecord incident,
        string actor,
        string action,
        string target,
        string details,
        ActivityFeedItemKind kind,
        LogLevel logLevel,
        string source)
    {
        incident.Timeline.Insert(0, Timeline(actor, action, target, details, kind, DateTimeOffset.UtcNow));
        incident.Logs.Insert(0, Log(details, logLevel, source, DateTimeOffset.UtcNow));
    }

    private void SortIncidents()
    {
        var selectedId = _incidents.Count == 0 ? string.Empty : SelectedIncident.Id;
        _incidents.Sort(static (left, right) =>
        {
            var severity = left.Severity.CompareTo(right.Severity);
            if (severity != 0)
            {
                return severity;
            }

            var status = StatusRank(left.Status).CompareTo(StatusRank(right.Status));
            if (status != 0)
            {
                return status;
            }

            return right.OpenedAt.CompareTo(left.OpenedAt);
        });

        _selectedIndex = _incidents.FindIndex(incident => string.Equals(incident.Id, selectedId, StringComparison.Ordinal));
        if (_selectedIndex < 0)
        {
            _selectedIndex = 0;
        }

        _incidents[_selectedIndex].HasUnreadUpdate = false;
    }

    private static int StatusRank(IncidentStatus status) => status switch
    {
        IncidentStatus.Escalated => 0,
        IncidentStatus.Investigating => 1,
        IncidentStatus.Acknowledged => 2,
        IncidentStatus.Monitoring => 3,
        _ => 4,
    };
}
