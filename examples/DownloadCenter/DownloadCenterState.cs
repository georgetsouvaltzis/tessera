using System.Globalization;
using Tessera.Controls;

namespace Tessera.Examples.DownloadCenter;

internal enum DownloadCenterAction
{
    PauseResume,
    RetryNow,
    BoostLane,
    PurgeCompleted
}

internal enum DownloadJobPhase
{
    Queued,
    Active,
    Retrying,
    Verifying,
    Completed,
    Failed,
    Paused
}

internal sealed class DownloadCenterState
{
    private readonly List<ActivityFeedItem> _feed = [];
    private readonly List<DownloadJob> _jobs;
    private readonly Random _random = new(2744);
    private readonly List<double> _retryTrend = [];
    private readonly DateTimeOffset _simulatedStartUtc = DateTimeOffset.UtcNow;
    private readonly List<double> _throughputTrend = [];
    private string _selectedJobId;
    private int _tick;

    private DownloadCenterState(List<DownloadJob> jobs)
    {
        _jobs = jobs;
        _selectedJobId = jobs[0].Id;
        SeedFeed();
        SeedTrends();
    }

    public IReadOnlyList<ActivityFeedItem> FeedItems => _feed;
    public IReadOnlyList<double> ThroughputTrend => _throughputTrend;
    public IReadOnlyList<double> RetryTrend => _retryTrend;
    public static string ClockText => DateTimeOffset.UtcNow.ToString("HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);
    public string LastAction { get; private set; } = "holding orbital relay cadence";

    public int ActiveCount =>
        _jobs.Count(static job => job.Phase is DownloadJobPhase.Active or DownloadJobPhase.Verifying);

    public int RetryCount =>
        _jobs.Count(static job => job.Phase is DownloadJobPhase.Retrying or DownloadJobPhase.Failed);

    public int CompletedCount => _jobs.Count(static job => job.Phase == DownloadJobPhase.Completed);

    public double TotalThroughput => _jobs.Where(static job => job.Phase == DownloadJobPhase.Active)
        .Sum(static job => job.ThroughputMbps);

    public string SummaryBadge => $"{ActiveCount:00} live  {RetryCount:00} retry  {CompletedCount:00} done";
    public string ThroughputBadge => $"{TotalThroughput:0} MB/s sustained";
    public string PressureBadge => RetryCount > 0 ? $"{RetryCount:00} lanes unstable" : "all lanes aligned";
    public DownloadJob SelectedJob => _jobs.First(job => job.Id == _selectedJobId);

    public static DownloadCenterState CreateSeed()
    {
        return new DownloadCenterState(
        [
            new DownloadJob("orbital-sdk.pkg", "cdn-eu-1", 980, 610, 42, 1, DownloadJobPhase.Active),
            new DownloadJob("render-pipeline.tar", "edge-us-2", 620, 184, 31, 1, DownloadJobPhase.Active),
            new DownloadJob("nightly-assets.zip", "cache-ap-1", 410, 388, 18, 2, DownloadJobPhase.Verifying),
            new DownloadJob("mirror-snapshot.bin", "relay-dr-1", 1500, 0, 0, 0, DownloadJobPhase.Queued),
            new DownloadJob("drift-hotfix.pkg", "cdn-us-3", 340, 0, 0, 1, DownloadJobPhase.Retrying) { RetryTicks = 3 },
            new DownloadJob("shader-bundle.vpk", "cdn-eu-2", 290, 290, 0, 0, DownloadJobPhase.Completed),
            new DownloadJob("capture-index.json", "edge-eu-4", 90, 90, 0, 3, DownloadJobPhase.Failed)
        ]);
    }

    public IReadOnlyList<DownloadQueueSection> BuildSections()
    {
        return
        [
            new DownloadQueueSection("Live", ActiveCount,
                BuildItems(DownloadJobPhase.Active, DownloadJobPhase.Verifying, DownloadJobPhase.Paused)),
            new DownloadQueueSection("Retry", RetryCount,
                BuildItems(DownloadJobPhase.Retrying, DownloadJobPhase.Failed)),
            new DownloadQueueSection("Ready", _jobs.Count(static job => job.Phase == DownloadJobPhase.Queued),
                BuildItems(DownloadJobPhase.Queued)),
            new DownloadQueueSection("Done", CompletedCount, BuildItems(DownloadJobPhase.Completed))
        ];
    }

    public string BuildSelectionSummary()
    {
        var job = SelectedJob;
        return string.Join(
            '\n',
            $"Source        {job.Source}",
            $"Payload       {job.SizeMb:0} MB",
            $"Delivered     {job.CompletedMb:0} MB",
            $"Throughput    {job.ThroughputMbps:0} MB/s",
            $"ETA           {FormatEta(job)}",
            $"Retries       {job.Attempts:00}",
            $"State         {job.PhaseLabel}");
    }

    public IReadOnlyList<StatItem> BuildPulseItems(string mode)
    {
        return mode switch
        {
            "lanes" =>
            [
                new StatItem("Live", $"{ActiveCount:00}"),
                new StatItem("Retry", $"{RetryCount:00}"),
                new StatItem("Done", $"{CompletedCount:00}")
            ],
            "pipe" =>
            [
                new StatItem("Now", $"{TotalThroughput:0} MB/s"),
                new StatItem("Peak", $"{_throughputTrend.DefaultIfEmpty(0).Max():0} MB/s"),
                new StatItem("ETA", FormatEta(SelectedJob))
            ],
            _ =>
            [
                new StatItem("Hot", $"{RetryCount:00}"),
                new StatItem("Attempts", $"{_jobs.Sum(static job => job.Attempts):00}"),
                new StatItem("Queue", $"{_jobs.Count(static job => job.Phase == DownloadJobPhase.Queued):00}")
            ]
        };
    }

    public string BuildCommandText()
    {
        return RetryCount > 0
            ? "control hint: stabilize retry lanes first, then reopen queued mirrors."
            : "control hint: hold throughput near crest, keep verify lane ahead of queue inflow.";
    }

    public void MoveSelection(int delta)
    {
        var ordered = OrderedJobs();
        var currentIndex = ordered.FindIndex(job => job.Id == _selectedJobId);
        currentIndex = currentIndex < 0 ? 0 : Math.Clamp(currentIndex + delta, 0, ordered.Count - 1);
        _selectedJobId = ordered[currentIndex].Id;
    }

    public void Advance()
    {
        _tick++;
        var liveBudget = 3;
        foreach (var job in _jobs)
        {
            switch (job.Phase)
            {
                case DownloadJobPhase.Active:
                    job.ThroughputMbps = Math.Clamp(job.ThroughputMbps + _random.Next(-6, 9), 14, 78);
                    job.CompletedMb = Math.Min(job.SizeMb, job.CompletedMb + job.ThroughputMbps);
                    if (_random.NextDouble() < 0.04)
                    {
                        job.Phase = DownloadJobPhase.Retrying;
                        job.Attempts++;
                        job.RetryTicks = _random.Next(2, 5);
                        job.ThroughputMbps = 0;
                        PushFeed("relay", "retry", job.Name, "checksum drift detected, lane rewinding",
                            ActivityFeedItemKind.Warning);
                    }
                    else if (job.CompletedMb >= job.SizeMb)
                    {
                        job.Phase = DownloadJobPhase.Verifying;
                        job.VerifyTicks = 2;
                        job.ThroughputMbps = 0;
                        PushFeed("verifier", "seal", job.Name, "payload landed, digest verification armed",
                            ActivityFeedItemKind.Info);
                    }

                    liveBudget--;
                    break;
                case DownloadJobPhase.Verifying:
                    job.VerifyTicks--;
                    if (job.VerifyTicks <= 0)
                    {
                        job.Phase = DownloadJobPhase.Completed;
                        PushFeed("relay", "complete", job.Name, "transfer sealed and mirrored to cache",
                            ActivityFeedItemKind.Success);
                    }

                    liveBudget--;
                    break;
                case DownloadJobPhase.Retrying:
                    job.RetryTicks--;
                    if (job.RetryTicks <= 0)
                    {
                        job.Phase = DownloadJobPhase.Active;
                        job.ThroughputMbps = _random.Next(18, 42);
                        PushFeed("relay", "resume", job.Name, "retry lane reopened with warmed socket pool",
                            ActivityFeedItemKind.Info);
                        liveBudget--;
                    }

                    break;
                case DownloadJobPhase.Paused:
                case DownloadJobPhase.Completed:
                case DownloadJobPhase.Failed:
                case DownloadJobPhase.Queued:
                    job.ThroughputMbps = 0;
                    break;
            }
        }

        while (liveBudget > 0)
        {
            var queued = _jobs.FirstOrDefault(static job => job.Phase == DownloadJobPhase.Queued);
            if (queued is null)
            {
                break;
            }

            queued.Phase = DownloadJobPhase.Active;
            queued.ThroughputMbps = _random.Next(16, 38);
            PushFeed("scheduler", "launch", queued.Name, "lane reserved and transfer primed",
                ActivityFeedItemKind.Info);
            liveBudget--;
        }

        AppendTrend(_throughputTrend, TotalThroughput);
        AppendTrend(_retryTrend, RetryCount * 18);
    }

    public void Execute(DownloadCenterAction action)
    {
        var job = SelectedJob;
        switch (action)
        {
            case DownloadCenterAction.PauseResume:
                if (job.Phase == DownloadJobPhase.Active)
                {
                    job.Phase = DownloadJobPhase.Paused;
                    job.ThroughputMbps = 0;
                    LastAction = $"paused {job.Name}";
                    PushFeed("ops", "paused", job.Name, "lane frozen, sockets retained warm",
                        ActivityFeedItemKind.Info);
                }
                else if (job.Phase == DownloadJobPhase.Paused)
                {
                    job.Phase = DownloadJobPhase.Active;
                    job.ThroughputMbps = _random.Next(18, 44);
                    LastAction = $"resumed {job.Name}";
                    PushFeed("ops", "resumed", job.Name, "lane reopened with priority slice",
                        ActivityFeedItemKind.Success);
                }

                break;
            case DownloadCenterAction.RetryNow:
                if (job.Phase is DownloadJobPhase.Retrying or DownloadJobPhase.Failed)
                {
                    job.Phase = DownloadJobPhase.Active;
                    job.ThroughputMbps = _random.Next(20, 46);
                    job.RetryTicks = 0;
                    LastAction = $"forced retry for {job.Name}";
                    PushFeed("ops", "retried", job.Name, "manual retry cut ahead of backoff window",
                        ActivityFeedItemKind.Warning);
                }

                break;
            case DownloadCenterAction.BoostLane:
                if (job.Phase == DownloadJobPhase.Active)
                {
                    job.ThroughputMbps = Math.Min(88, job.ThroughputMbps + 18);
                    LastAction = $"boosted lane for {job.Name}";
                    PushFeed("scheduler", "boosted", job.Name, "reserved fast lane and larger socket pool",
                        ActivityFeedItemKind.Success);
                }

                break;
            case DownloadCenterAction.PurgeCompleted:
                var removed = _jobs.RemoveAll(static job => job.Phase == DownloadJobPhase.Completed);
                if (removed > 0)
                {
                    _selectedJobId = _jobs[0].Id;
                    LastAction = $"purged {removed:00} completed transfers";
                    PushFeed("ops", "purged", "completed set", $"{removed:00} sealed jobs archived",
                        ActivityFeedItemKind.Info);
                }

                break;
        }
    }

    private List<DownloadJob> OrderedJobs()
    {
        return _jobs
            .OrderBy(static job => job.Phase switch
            {
                DownloadJobPhase.Active => 0,
                DownloadJobPhase.Verifying => 1,
                DownloadJobPhase.Paused => 2,
                DownloadJobPhase.Retrying => 3,
                DownloadJobPhase.Failed => 4,
                DownloadJobPhase.Queued => 5,
                _ => 6
            })
            .ThenBy(static job => job.Name, StringComparer.Ordinal)
            .ToList();
    }

    private DownloadQueueItem[] BuildItems(params DownloadJobPhase[] phases)
    {
        return OrderedJobs()
            .Where(job => phases.Contains(job.Phase))
            .Select(job => new DownloadQueueItem(
                job.Id,
                job.Name,
                job.Source,
                $"{job.ProgressPercent:0}%  {job.ThroughputMbps:0} MB/s",
                FormatEta(job),
                job.PhaseLabel,
                job.Phase))
            .ToArray();
    }

    private void SeedFeed()
    {
        PushFeed("relay", "sealed", "shader-bundle.vpk", "cache shadow already hot", ActivityFeedItemKind.Success);
        PushFeed("scheduler", "queued", "mirror-snapshot.bin", "orbit lane reserved behind active crest",
            ActivityFeedItemKind.Info);
        PushFeed("relay", "retry", "drift-hotfix.pkg", "integrity mismatch after segment 12",
            ActivityFeedItemKind.Warning);
    }

    private void SeedTrends()
    {
        for (var i = 0; i < 24; i++)
        {
            AppendTrend(_throughputTrend, 160 + i * 6);
            AppendTrend(_retryTrend, i % 8 == 0 ? 34 : 18);
        }
    }

    private void PushFeed(string actor, string action, string target, string details, ActivityFeedItemKind kind)
    {
        _feed.Insert(0,
            new ActivityFeedItem(actor, action, target, details, kind, _simulatedStartUtc.AddSeconds(_tick))
            {
                IsUnread = kind is ActivityFeedItemKind.Warning or ActivityFeedItemKind.Error
            });
        if (_feed.Count > 48)
        {
            _feed.RemoveRange(48, _feed.Count - 48);
        }
    }

    private static string FormatEta(DownloadJob job)
    {
        if (job.Phase == DownloadJobPhase.Completed)
        {
            return "sealed";
        }

        if (job.Phase == DownloadJobPhase.Verifying)
        {
            return "verify";
        }

        if (job.Phase == DownloadJobPhase.Retrying)
        {
            return $"retry {job.RetryTicks:0}";
        }

        if (job.Phase == DownloadJobPhase.Failed)
        {
            return "manual";
        }

        if (job.Phase == DownloadJobPhase.Paused)
        {
            return "paused";
        }

        if (job.ThroughputMbps <= 0)
        {
            return "queued";
        }

        var remainingSeconds = Math.Max(0, (job.SizeMb - job.CompletedMb) / Math.Max(1, job.ThroughputMbps));
        return TimeSpan.FromSeconds(remainingSeconds).ToString(@"mm\:ss", CultureInfo.InvariantCulture);
    }

    private static void AppendTrend(List<double> trend, double value)
    {
        trend.Add(value);
        if (trend.Count > 48)
        {
            trend.RemoveAt(0);
        }
    }
}

internal sealed class DownloadJob
{
    public DownloadJob(string name, string source, double sizeMb, double completedMb, double throughputMbps,
        int attempts, DownloadJobPhase phase)
    {
        Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
        Name = name;
        Source = source;
        SizeMb = sizeMb;
        CompletedMb = completedMb;
        ThroughputMbps = throughputMbps;
        Attempts = attempts;
        Phase = phase;
    }

    public string Id { get; }
    public string Name { get; }
    public string Source { get; }
    public double SizeMb { get; }
    public double CompletedMb { get; set; }
    public double ThroughputMbps { get; set; }
    public int Attempts { get; set; }
    public int RetryTicks { get; set; }
    public int VerifyTicks { get; set; }
    public DownloadJobPhase Phase { get; set; }
    public double ProgressPercent => SizeMb <= 0 ? 0 : Math.Clamp(CompletedMb / SizeMb * 100, 0, 100);

    public string PhaseLabel => Phase switch
    {
        DownloadJobPhase.Active => "transferring",
        DownloadJobPhase.Verifying => "verifying",
        DownloadJobPhase.Retrying => "retry lane",
        DownloadJobPhase.Completed => "complete",
        DownloadJobPhase.Failed => "failed",
        DownloadJobPhase.Paused => "paused",
        _ => "queued"
    };
}
