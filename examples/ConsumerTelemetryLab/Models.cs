internal sealed class ServiceState(string id, string name, string cluster, string region)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public string Cluster { get; } = cluster;
    public string Region { get; } = region;

    public string Health { get; set; } = "Healthy";
    public double Cpu { get; set; }
    public double MemoryGb { get; set; }
    public double P95Ms { get; set; }
    public double ErrorRatePct { get; set; }
    public double ReqPerSec { get; set; }
}

internal sealed class IncidentState(
    string id,
    string serviceId,
    string serviceName,
    string severity,
    string state,
    string summary,
    string owner,
    string runbook,
    int minutesOpen,
    DateTimeOffset startedAt)
{
    public string Id { get; } = id;
    public string ServiceId { get; } = serviceId;
    public string ServiceName { get; } = serviceName;

    public string Severity { get; set; } = severity;
    public string State { get; set; } = state;
    public string Summary { get; set; } = summary;
    public string Owner { get; set; } = owner;
    public string Runbook { get; set; } = runbook;
    public int MinutesOpen { get; set; } = minutesOpen;
    public DateTimeOffset StartedAt { get; } = startedAt;

    public int SeverityRank => Severity switch
    {
        "Critical" => 4,
        "High" => 3,
        "Medium" => 2,
        "Low" => 1,
        _ => 0,
    };
}
