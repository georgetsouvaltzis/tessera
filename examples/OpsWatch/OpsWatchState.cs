using System.Globalization;
using Tessera.Controls;

namespace Tessera.Examples.OpsWatch;

internal enum OpsWatchAction
{
    Restart,
    Drain,
    MuteAlerts,
    Scale,
    Inspect,
    Failover,
    Acknowledge,
}

internal sealed class OpsWatchState
{
    private readonly Random _random = new(1447);
    private readonly DateTimeOffset _simulatedStartUtc = DateTimeOffset.UtcNow;
    private readonly List<OpsCluster> _clusters;
    private readonly List<ActivityFeedItem> _feed = [];
    private readonly List<double> _cpuTrend = [];
    private readonly List<double> _memoryTrend = [];
    private readonly List<double> _networkTrend = [];
    private readonly List<double> _diskTrend = [];
    private int _tick;
    private string _selectedClusterId;
    private string _selectedNodeId;
    private string _activeRoute = "north-atlantic -> dr-equinox";
    private string _automationMode = "steady hand";
    private string _commandText = "Watch captain: hold traffic on atlas-west, rehearse failover but keep canary live.";

    private OpsWatchState(List<OpsCluster> clusters)
    {
        _clusters = clusters;
        _selectedClusterId = clusters[0].Id;
        _selectedNodeId = clusters[0].Nodes[0].Id;

        SeedTrends();
        SeedFeed();
        SyncSelection();
    }

    public static OpsWatchState CreateSeed()
    {
        return new OpsWatchState(
        [
            new OpsCluster("atl", "Atlas Ring", "AT", 14, "north-atlantic", 7,
            [
                new OpsNode("atl-api-01", "api-gw-01", "api-gw", "eu-1", 48, 52, 61, 45),
                new OpsNode("atl-api-02", "api-gw-02", "api-gw", "eu-2", 56, 58, 63, 48),
                new OpsNode("atl-stream-01", "stream-01", "stream", "eu-1", 67, 62, 74, 56),
                new OpsNode("atl-queue-01", "queue-01", "queue", "eu-3", 43, 54, 41, 64),
                new OpsNode("atl-cache-01", "cache-01", "cache", "eu-2", 29, 71, 26, 38),
            ]),
            new OpsCluster("boreal", "Boreal Edge", "BO", 9, "coastal-edge", 5,
            [
                new OpsNode("bor-edge-01", "edge-01", "edge", "us-1", 62, 47, 82, 39),
                new OpsNode("bor-edge-02", "edge-02", "edge", "us-2", 58, 44, 77, 36),
                new OpsNode("bor-api-01", "api-01", "api-gw", "us-1", 49, 55, 58, 41),
                new OpsNode("bor-job-01", "jobs-01", "batch", "us-3", 38, 49, 22, 61),
                new OpsNode("bor-cache-01", "cache-01", "cache", "us-2", 27, 68, 19, 35),
            ]),
            new OpsCluster("cinder", "Cinder Relay", "CI", 11, "relay-fabric", 3,
            [
                new OpsNode("cin-relay-01", "relay-01", "relay", "edge-a", 72, 63, 69, 58),
                new OpsNode("cin-relay-02", "relay-02", "relay", "edge-b", 64, 59, 71, 54),
                new OpsNode("cin-dns-01", "dns-01", "dns", "edge-a", 41, 38, 65, 29),
                new OpsNode("cin-auth-01", "auth-01", "auth", "edge-c", 52, 61, 47, 43),
                new OpsNode("cin-store-01", "store-01", "store", "edge-b", 47, 66, 31, 72),
            ]),
            new OpsCluster("drift", "Drift Reserve", "DR", 6, "failover-reserve", 1,
            [
                new OpsNode("drf-api-01", "reserve-api-01", "api-gw", "dr-1", 22, 37, 19, 24),
                new OpsNode("drf-api-02", "reserve-api-02", "api-gw", "dr-2", 19, 34, 21, 25),
                new OpsNode("drf-cache-01", "reserve-cache-01", "cache", "dr-1", 17, 49, 11, 22),
                new OpsNode("drf-queue-01", "reserve-queue-01", "queue", "dr-2", 14, 31, 9, 18),
            ]),
        ]);
    }

    public string SelectedClusterName => SelectedCluster.Name;

    public OpsNode SelectedNode => CurrentNodes.First(static node => node.IsSelected);

    public IReadOnlyList<double> CpuTrend => _cpuTrend;
    public IReadOnlyList<double> MemoryTrend => _memoryTrend;
    public IReadOnlyList<double> NetworkTrend => _networkTrend;
    public IReadOnlyList<double> DiskTrend => _diskTrend;
    public IReadOnlyList<ActivityFeedItem> FeedItems => _feed;

    public string ClockText => DateTimeOffset.UtcNow.ToString("HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);
    public string FleetBadge => $"{SelectedClusterName} / {CurrentNodes.Count} live";
    public string ModeBadge => $"autonomy {AutomationMode}";
    public string RouteBadge => $"route {ActiveRoute}";
    public string PressureText => $"alert pressure {AlertPressureLabel}  {ActiveAlertCount:00} hot";
    public string CrewText => $"operators {SelectedCluster.OperatorCount:00}  canaries {SelectedCluster.CanaryCount:00}  muted {MutedCount:00}";
    public string CommandText => _commandText;
    public string AutomationMode => _automationMode;
    public string ActiveRoute => _activeRoute;

    public double CpuAverage => Average(CurrentNodes, static node => node.Cpu);
    public double MemoryAverage => Average(CurrentNodes, static node => node.Memory);
    public double NetworkAverage => Average(CurrentNodes, static node => node.Network);
    public double DiskAverage => Average(CurrentNodes, static node => node.Disk);

    public int HealthyCount => CurrentNodes.Count(static node => node.Severity == HealthServiceSeverity.Healthy);
    public int DegradedCount => CurrentNodes.Count(static node => node.Severity == HealthServiceSeverity.Degraded);
    public int OutageCount => CurrentNodes.Count(static node => node.Severity == HealthServiceSeverity.Outage);
    public int MutedCount => CurrentNodes.Count(static node => node.IsMuted);
    public int DrainingCount => CurrentNodes.Count(static node => node.IsDraining);
    public int ActiveAlertCount => CurrentNodes.Count(static node => node.Severity != HealthServiceSeverity.Healthy);
    public int AckCount => CurrentNodes.Count(static node => node.IsAcknowledged);
    public string AlertPressureLabel => ActiveAlertCount >= 4 ? "severe" : ActiveAlertCount >= 2 ? "elevated" : "controlled";

    public IReadOnlyList<NavItem> BuildNavItems()
    {
        return _clusters
            .Select(cluster => new NavItem(
                cluster.Id,
                cluster.Name,
                icon: cluster.Code,
                badge: cluster.Nodes.Count(node => node.Severity != HealthServiceSeverity.Healthy).ToString("00", CultureInfo.InvariantCulture)))
            .ToArray();
    }

    public IReadOnlyList<HealthService> BuildServices()
    {
        return CurrentNodes
            .Select(node => new HealthService(node.Id, $"{node.Name}  {node.Role}", node.Severity, BuildNodeSummary(node))
            {
                IsAcknowledged = node.IsAcknowledged,
                IsMuted = node.IsMuted,
                ObservedAt = SimulatedUtcNow.AddSeconds(-node.AgeSeconds),
            })
            .ToArray();
    }

    public IReadOnlyList<StatItem> BuildFleetPulseItems()
    {
        return
        [
            new StatItem("Nodes", CurrentNodes.Count.ToString("00", CultureInfo.InvariantCulture)),
            new StatItem("Health", $"{HealthyCount:00}/{CurrentNodes.Count:00}"),
            new StatItem("Ack", $"{AckCount:00}/{ActiveAlertCount:00}"),
        ];
    }

    public IReadOnlyList<StatItem> BuildTrafficPulseItems()
    {
        return
        [
            new StatItem("RX/TX", $"{NetworkAverage:0}%"),
            new StatItem("Drain", $"{DrainingCount:00}"),
            new StatItem("Scale", $"+{SelectedCluster.ScaleTarget - SelectedCluster.BaseScaleTarget:0}"),
        ];
    }

    public IReadOnlyList<StatItem> BuildRoutePulseItems()
    {
        return
        [
            new StatItem("Route", SelectedCluster.RouteCode),
            new StatItem("Mode", AutomationMode),
            new StatItem("Feed", $"{_feed.Count:000}"),
        ];
    }

    public static IReadOnlyList<StatItem> BuildMetricCardItems(string label, double value, string delta)
    {
        return
        [
            new StatItem("Now", $"{value:0}%"),
            new StatItem("Drift", delta),
            new StatItem("Ceil", value >= 85 ? "hot" : value >= 65 ? "watch" : "green"),
        ];
    }

    public IReadOnlyList<StatItem> BuildSelectedNodeItems()
    {
        var node = SelectedNode;
        return
        [
            new StatItem("Role", node.Role),
            new StatItem("Zone", node.Zone),
            new StatItem("CPU", $"{node.Cpu:0}%"),
            new StatItem("MEM", $"{node.Memory:0}%"),
            new StatItem("NET", $"{node.Network:0}%"),
            new StatItem("DISK", $"{node.Disk:0}%"),
            new StatItem("Flags", BuildFlags(node)),
        ];
    }

    public string BuildFocusText()
    {
        var node = SelectedNode;
        return string.Join(
            '\n',
            $"Node      {node.Name}",
            $"Route     {SelectedCluster.RouteCode}  /  {node.Zone}",
            $"State     {BuildFlags(node)}",
            $"Summary   {BuildNodeSummary(node)}",
            $"Last op   {node.LastOperatorAction}");
    }

    public string BuildRunbookText()
    {
        var node = SelectedNode;
        return string.Join(
            '\n',
            "Runbook lane",
            $"1. hold traffic on {node.Zone}",
            $"2. sample {node.Role} queue depth",
            $"3. keep failover path {ActiveRoute}",
            $"4. ack or drain before restart");
    }

    public void SelectCluster(string clusterId)
    {
        if (!_clusters.Any(cluster => string.Equals(cluster.Id, clusterId, StringComparison.Ordinal)))
        {
            return;
        }

        _selectedClusterId = clusterId;
        var current = CurrentNodes;
        if (!current.Any(node => string.Equals(node.Id, _selectedNodeId, StringComparison.Ordinal)))
        {
            _selectedNodeId = current[0].Id;
        }

        SyncSelection();
        _commandText = $"Watch captain: shift focus to {SelectedCluster.Name}, preserve headroom and keep route {SelectedCluster.RouteCode}.";
    }

    public void SelectNode(string nodeId)
    {
        if (!CurrentNodes.Any(node => string.Equals(node.Id, nodeId, StringComparison.Ordinal)))
        {
            return;
        }

        _selectedNodeId = nodeId;
        SyncSelection();
    }

    public void Advance()
    {
        _tick++;
        foreach (var cluster in _clusters)
        {
            for (var index = 0; index < cluster.Nodes.Count; index++)
            {
                UpdateNode(cluster.Nodes[index]);
            }
        }

        if (_tick % 5 == 0)
        {
            EmitSystemFeed();
        }

        _automationMode = ActiveAlertCount >= 4 ? "containment" : ActiveAlertCount >= 2 ? "assisted" : "steady hand";
        CaptureTrends();
        SyncSelection();
    }

    public string Execute(OpsWatchAction action)
    {
        var node = SelectedNode;
        string message;
        switch (action)
        {
            case OpsWatchAction.Restart:
                node.RestartTicks = 3;
                node.Severity = HealthServiceSeverity.Degraded;
                node.IsAcknowledged = false;
                node.LastOperatorAction = "restart queued";
                message = $"restart queued on {node.Name}";
                PushFeed("ops", "restarted", node.Name, "rolling restart staged", ActivityFeedItemKind.Warning);
                break;
            case OpsWatchAction.Drain:
                node.IsDraining = !node.IsDraining;
                node.LastOperatorAction = node.IsDraining ? "drain armed" : "drain released";
                message = node.IsDraining ? $"traffic draining from {node.Name}" : $"drain released for {node.Name}";
                PushFeed("ops", node.IsDraining ? "draining" : "resumed", node.Name, node.IsDraining ? "sessions evacuating" : "traffic restored", ActivityFeedItemKind.Info);
                break;
            case OpsWatchAction.MuteAlerts:
                node.IsMuted = !node.IsMuted;
                node.LastOperatorAction = node.IsMuted ? "alerts muted" : "alerts audible";
                message = node.IsMuted ? $"alerts muted for {node.Name}" : $"alerts restored for {node.Name}";
                PushFeed("ops", node.IsMuted ? "muted" : "unmuted", node.Name, "operator preference updated", ActivityFeedItemKind.Info);
                break;
            case OpsWatchAction.Scale:
                SelectedCluster.ScaleTarget++;
                message = $"scale target raised for {SelectedCluster.Name}";
                PushFeed("autoscaler", "scaled", SelectedCluster.Name, $"+1 replica to {SelectedCluster.ScaleTarget}", ActivityFeedItemKind.Success);
                break;
            case OpsWatchAction.Inspect:
                node.LastOperatorAction = "deep inspect";
                message = $"inspection lane pinned to {node.Name}";
                PushFeed("ops", "inspected", node.Name, "profiling and queue trace armed", ActivityFeedItemKind.Info);
                break;
            case OpsWatchAction.Failover:
                _activeRoute = _activeRoute == "north-atlantic -> dr-equinox"
                    ? "north-atlantic -> boreal-edge"
                    : "north-atlantic -> dr-equinox";
                message = $"failover path switched to {_activeRoute}";
                PushFeed("control", "rerouted", SelectedCluster.Name, _activeRoute, ActivityFeedItemKind.Warning);
                break;
            default:
                node.IsAcknowledged = true;
                node.LastOperatorAction = "alert acknowledged";
                message = $"acknowledged {node.Name}";
                PushFeed("ops", "acknowledged", node.Name, "operator absorbed current alert", ActivityFeedItemKind.Success);
                break;
        }

        SyncSelection();
        return message;
    }

    private OpsCluster SelectedCluster => _clusters.First(cluster => string.Equals(cluster.Id, _selectedClusterId, StringComparison.Ordinal));

    private List<OpsNode> CurrentNodes => SelectedCluster.Nodes;

    private DateTimeOffset SimulatedUtcNow => _simulatedStartUtc.AddSeconds(_tick);

    private void SeedTrends()
    {
        for (var index = 0; index < 32; index++)
        {
            CaptureTrends();
        }
    }

    private void SeedFeed()
    {
        PushFeed("router", "stabilized", "atlas ring", "latency band back under 21ms", ActivityFeedItemKind.Success);
        PushFeed("autoscaler", "prewarmed", "boreal edge", "2 reserve pods hot", ActivityFeedItemKind.Info);
        PushFeed("sentinel", "flagged", "relay-01", "jitter burst in lane edge-a", ActivityFeedItemKind.Warning);
        PushFeed("ops", "verified", "drift reserve", "failover heartbeat clean", ActivityFeedItemKind.Success);
        PushFeed("sentinel", "detected", "store-01", "write queue pressure rising", ActivityFeedItemKind.Warning);
    }

    private void CaptureTrends()
    {
        AppendSample(_cpuTrend, CpuAverage);
        AppendSample(_memoryTrend, MemoryAverage);
        AppendSample(_networkTrend, NetworkAverage);
        AppendSample(_diskTrend, DiskAverage);
    }

    private void UpdateNode(OpsNode node)
    {
        if (node.RestartTicks > 0)
        {
            node.RestartTicks--;
            node.Cpu = Damp(node.Cpu, 34);
            node.Memory = Damp(node.Memory, 41);
            node.Network = Damp(node.Network, 28);
            node.Disk = Damp(node.Disk, 37);
            if (node.RestartTicks == 0)
            {
                node.Severity = HealthServiceSeverity.Healthy;
            }
        }
        else
        {
            node.Cpu = Nudge(node.Cpu, -6, 6, node.IsDraining ? -4 : 2);
            node.Memory = Nudge(node.Memory, -4, 4, 1);
            node.Network = Nudge(node.Network, -8, 8, node.IsDraining ? -10 : 3);
            node.Disk = Nudge(node.Disk, -5, 5, 2);
        }

        if (_random.NextDouble() < 0.06)
        {
            node.Cpu = Math.Min(99, node.Cpu + _random.Next(10, 18));
            node.Network = Math.Min(99, node.Network + _random.Next(7, 14));
        }

        var priorSeverity = node.Severity;
        node.Severity = ResolveSeverity(node);
        if (node.Severity > priorSeverity)
        {
            node.IsAcknowledged = false;
            PushFeed("sentinel", "escalated", node.Name, BuildNodeSummary(node), node.Severity == HealthServiceSeverity.Outage ? ActivityFeedItemKind.Error : ActivityFeedItemKind.Warning);
        }

        node.AgeSeconds = Math.Min(999, node.AgeSeconds + _random.Next(4, 9));
    }

    private void EmitSystemFeed()
    {
        if (OutageCount > 0)
        {
            PushFeed("control", "contain", SelectedCluster.Name, "failover rehearsed, manual hold remains active", ActivityFeedItemKind.Warning);
            return;
        }

        if (DegradedCount > 0)
        {
            PushFeed("sentinel", "observe", SelectedCluster.Name, "degraded lane holding under operator watch", ActivityFeedItemKind.Info);
            return;
        }

        PushFeed("telemetry", "steady", SelectedCluster.Name, "load curve stable and canaries green", ActivityFeedItemKind.Success);
    }

    private void PushFeed(string actor, string action, string target, string details, ActivityFeedItemKind kind)
    {
        _feed.Insert(0, new ActivityFeedItem(actor, action, target, details, kind, SimulatedUtcNow)
        {
            IsUnread = kind is ActivityFeedItemKind.Warning or ActivityFeedItemKind.Error,
        });
        if (_feed.Count > 48)
        {
            _feed.RemoveRange(48, _feed.Count - 48);
        }
    }

    private void SyncSelection()
    {
        foreach (var cluster in _clusters)
        {
            foreach (var node in cluster.Nodes)
            {
                node.IsSelected = string.Equals(node.Id, _selectedNodeId, StringComparison.Ordinal);
            }
        }
    }

    private static void AppendSample(List<double> samples, double value)
    {
        samples.Add(value);
        if (samples.Count > 48)
        {
            samples.RemoveAt(0);
        }
    }

    private static double Average(IEnumerable<OpsNode> nodes, Func<OpsNode, double> selector)
    {
        var total = 0d;
        var count = 0;
        foreach (var node in nodes)
        {
            total += selector(node);
            count++;
        }

        return count == 0 ? 0 : total / count;
    }

    private static string BuildFlags(OpsNode node)
    {
        var flags = new List<string>();
        if (node.IsDraining)
        {
            flags.Add("drain");
        }

        if (node.IsMuted)
        {
            flags.Add("mute");
        }

        if (node.IsAcknowledged)
        {
            flags.Add("ack");
        }

        if (node.RestartTicks > 0)
        {
            flags.Add("restart");
        }

        return flags.Count == 0 ? "watch" : string.Join('/', flags);
    }

    private static string BuildNodeSummary(OpsNode node)
    {
        return $"cpu {node.Cpu:0}%  mem {node.Memory:0}%  net {node.Network:0}%  disk {node.Disk:0}%";
    }

    private static HealthServiceSeverity ResolveSeverity(OpsNode node)
    {
        if (node.RestartTicks > 0)
        {
            return HealthServiceSeverity.Degraded;
        }

        if (node.Cpu >= 92 || node.Network >= 95 || node.Disk >= 94)
        {
            return HealthServiceSeverity.Outage;
        }

        if (node.Cpu >= 76 || node.Memory >= 83 || node.Network >= 82 || node.Disk >= 80)
        {
            return HealthServiceSeverity.Degraded;
        }

        return HealthServiceSeverity.Healthy;
    }

    private double Nudge(double value, int minStep, int maxStep, int bias)
    {
        var next = value + _random.Next(minStep, maxStep + 1) + bias;
        return Math.Clamp(next, 4, 99);
    }

    private static double Damp(double value, double toward)
    {
        return value + ((toward - value) * 0.55);
    }

    private sealed class OpsCluster(
        string id,
        string name,
        string code,
        int operatorCount,
        string routeCode,
        int canaryCount,
        List<OpsNode> nodes)
    {
        public string Id { get; } = id;
        public string Name { get; } = name;
        public string Code { get; } = code;
        public int OperatorCount { get; } = operatorCount;
        public string RouteCode { get; } = routeCode;
        public int CanaryCount { get; } = canaryCount;
        public List<OpsNode> Nodes { get; } = nodes;
        public int BaseScaleTarget => Nodes.Count;
        public int ScaleTarget { get; set; } = nodes.Count;
    }

    internal sealed class OpsNode(
        string id,
        string name,
        string role,
        string zone,
        double cpu,
        double memory,
        double network,
        double disk)
    {
        public string Id { get; } = id;
        public string Name { get; } = name;
        public string Role { get; } = role;
        public string Zone { get; } = zone;
        public double Cpu { get; set; } = cpu;
        public double Memory { get; set; } = memory;
        public double Network { get; set; } = network;
        public double Disk { get; set; } = disk;
        public bool IsMuted { get; set; }
        public bool IsDraining { get; set; }
        public bool IsAcknowledged { get; set; }
        public bool IsSelected { get; set; }
        public int RestartTicks { get; set; }
        public int AgeSeconds { get; set; }
        public string LastOperatorAction { get; set; } = "steady watch";
        public HealthServiceSeverity Severity { get; set; }
    }
}
