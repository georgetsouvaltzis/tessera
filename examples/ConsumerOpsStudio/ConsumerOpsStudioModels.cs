using TeaSharp;
using TeaSharp.Controls;

internal sealed record OpsTick(DateTimeOffset At) : Message;

internal enum OpsPanelTab
{
    Incidents = 0,
    Deployments = 1,
    Slo = 2,
}

internal enum PendingDialogAction
{
    None = 0,
    Rollback = 1,
    ToggleFreeze = 2,
}

internal sealed class ServiceSnapshot
{
    public ServiceSnapshot(string id, string name, string region, string owner, double baselineLatency, double baselineErrorRate, int queueDepth, double errorBudgetRemaining)
    {
        Id = id;
        Name = name;
        Region = region;
        Owner = owner;
        BaselineLatency = baselineLatency;
        BaselineErrorRate = baselineErrorRate;
        QueueDepth = queueDepth;
        ErrorBudgetRemaining = errorBudgetRemaining;
        LatencyP95 = baselineLatency;
        LatencyP99 = baselineLatency * 1.3;
        ErrorRatePercent = baselineErrorRate;
        Status = "Healthy";
    }

    public string Id { get; }

    public string Name { get; }

    public string Region { get; }

    public string Owner { get; }

    public double BaselineLatency { get; }

    public double BaselineErrorRate { get; }

    public double LatencyP95 { get; set; }

    public double LatencyP99 { get; set; }

    public double ErrorRatePercent { get; set; }

    public int QueueDepth { get; set; }

    public double ErrorBudgetRemaining { get; set; }

    public bool IsWriteFrozen { get; set; }

    public string Status { get; set; }
}

internal sealed class IncidentTicket
{
    public IncidentTicket(string id, string serviceId, string summary, string severity, string owner, int ageMinutes)
    {
        Id = id;
        ServiceId = serviceId;
        Summary = summary;
        Severity = severity;
        Owner = owner;
        AgeMinutes = ageMinutes;
    }

    public string Id { get; }

    public string ServiceId { get; }

    public string Summary { get; set; }

    public string Severity { get; set; }

    public string Owner { get; set; }

    public int AgeMinutes { get; set; }

    public bool IsAcknowledged { get; set; }

    public bool IsOpen { get; set; } = true;

    public string State => !IsOpen
        ? "Resolved"
        : IsAcknowledged
            ? "Mitigating"
            : "Investigating";
}

internal sealed class DeploymentRun
{
    public DeploymentRun(string id, string serviceId, string version, string owner, int ageMinutes, int progressPercent, string stage)
    {
        Id = id;
        ServiceId = serviceId;
        Version = version;
        Owner = owner;
        AgeMinutes = ageMinutes;
        ProgressPercent = progressPercent;
        Stage = stage;
    }

    public string Id { get; }

    public string ServiceId { get; }

    public string Version { get; set; }

    public string Owner { get; set; }

    public int AgeMinutes { get; set; }

    public int ProgressPercent { get; set; }

    public string Stage { get; set; }
}

internal static class ConsumerOpsSeedData
{
    public static List<ServiceSnapshot> CreateServices()
    {
        return
        [
            new ServiceSnapshot("svc-checkout", "Checkout API", "us-east", "Iris", 78, 0.18, 960, 82),
            new ServiceSnapshot("svc-payments", "Payments Ledger", "us-east", "Mina", 62, 0.11, 540, 91),
            new ServiceSnapshot("svc-fulfillment", "Fulfillment Worker", "us-west", "Soren", 93, 0.24, 1480, 74),
            new ServiceSnapshot("svc-catalog", "Catalog Search", "eu-west", "Niko", 56, 0.09, 380, 95),
            new ServiceSnapshot("svc-notify", "Notification Hub", "us-central", "Ari", 71, 0.15, 620, 88),
        ];
    }

    public static List<IncidentTicket> CreateIncidents()
    {
        return
        [
            new IncidentTicket("INC-2841", "svc-checkout", "Spike in checkout timeout rate", "Sev1", "Iris", 19),
            new IncidentTicket("INC-2838", "svc-fulfillment", "Queue depth growth after partner import", "Sev2", "Soren", 34)
            {
                IsAcknowledged = true,
            },
            new IncidentTicket("INC-2831", "svc-notify", "Delayed SMS send acknowledgments", "Sev3", "Ari", 57),
        ];
    }

    public static List<DeploymentRun> CreateDeployments()
    {
        return
        [
            new DeploymentRun("DEP-7712", "svc-checkout", "2026.03.26.4", "Mina", 8, 35, "Canary 20%"),
            new DeploymentRun("DEP-7708", "svc-payments", "2026.03.26.2", "Iris", 21, 100, "Completed"),
            new DeploymentRun("DEP-7701", "svc-catalog", "2026.03.25.9", "Niko", 63, 100, "Completed"),
        ];
    }

    public static IReadOnlyList<NavItem> CreateNavigation()
    {
        return
        [
            new NavItem("overview", "Overview", icon: "◉"),
            new NavItem("fulfillment", "Fulfillment", icon: "⇄"),
            new NavItem("payments", "Payments", icon: "$"),
            new NavItem("capacity", "Capacity", icon: "▦"),
            new NavItem("audit", "Audit", icon: "✎"),
        ];
    }

    public static IReadOnlyList<CommandBarItem> CreateCommandBarItems()
    {
        return
        [
            new CommandBarItem("ack", "Acknowledge", 'a'),
            new CommandBarItem("rollback", "Rollback", 'r'),
            new CommandBarItem("scale", "Scale +1", 's'),
            new CommandBarItem("freeze", "Freeze/Unfreeze", 'f'),
            new CommandBarItem("palette", "Palette", 'p'),
            new CommandBarItem("theme", "Theme", 't'),
        ];
    }

    public static IReadOnlyList<CommandPaletteItem> CreatePaletteItems()
    {
        return
        [
            new CommandPaletteItem("ack", "Acknowledge selected incident", "Marks selected incident as acknowledged."),
            new CommandPaletteItem("rollback", "Rollback selected service", "Schedules immediate rollback deployment."),
            new CommandPaletteItem("scale", "Scale selected service", "Adds one worker shard and drains queue."),
            new CommandPaletteItem("freeze", "Toggle write freeze", "Freeze or unfreeze writes for selected service."),
            new CommandPaletteItem("nav:overview", "Go to Overview", "Switch rail navigation to Overview."),
            new CommandPaletteItem("nav:payments", "Go to Payments", "Switch rail navigation to Payments."),
            new CommandPaletteItem("nav:capacity", "Go to Capacity", "Switch rail navigation to Capacity."),
            new CommandPaletteItem("theme", "Toggle alert theme", "Switch between normal and alert visual mode."),
        ];
    }
}
