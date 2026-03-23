internal sealed class ServiceNode
{
    public ServiceNode(
        string id,
        string name,
        string region,
        string state,
        int p95Ms,
        int cpuPercent,
        int requestsPerSecond,
        int errorBasisPoints)
    {
        Id = id;
        Name = name;
        Region = region;
        State = state;
        P95Ms = p95Ms;
        CpuPercent = cpuPercent;
        RequestsPerSecond = requestsPerSecond;
        ErrorBasisPoints = errorBasisPoints;
    }

    public string Id { get; }

    public string Name { get; }

    public string Region { get; }

    public string State { get; set; }

    public int P95Ms { get; set; }

    public int CpuPercent { get; set; }

    public int RequestsPerSecond { get; set; }

    public int ErrorBasisPoints { get; set; }
}

internal sealed class EndpointNode
{
    public EndpointNode(string path, int p95Ms, int errorBasisPoints, int requestsPerSecond)
    {
        Path = path;
        P95Ms = p95Ms;
        ErrorBasisPoints = errorBasisPoints;
        RequestsPerSecond = requestsPerSecond;
    }

    public string Path { get; }

    public int P95Ms { get; set; }

    public int ErrorBasisPoints { get; set; }

    public int RequestsPerSecond { get; set; }
}
