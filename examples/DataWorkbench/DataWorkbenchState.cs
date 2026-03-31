using System.Globalization;
using TeaSharp.Controls;

namespace TeaSharp.Examples.DataWorkbench;

internal enum DataWorkbenchPage
{
    Explore,
    Compare,
    History,
    Saved,
}

internal sealed partial class DataWorkbenchState
{
    private readonly List<WorkbenchSource> _sources;
    private readonly List<SavedWorkbenchView> _savedViews;
    private readonly Random _random = new(1427);
    private int _tick;

    private DataWorkbenchState(List<WorkbenchSource> sources)
    {
        _sources = sources;
        _savedViews =
        [
            new SavedWorkbenchView("sv_atlas", "Atlas burn watch", "fraud_signals", "eu", "score >= 70", "burn-up lens for eu fraud spikes"),
            new SavedWorkbenchView("sv_disputes", "Dispute drift", "refund_journal", "chargeback", "status contains pending", "watch refunds likely to convert into disputes"),
            new SavedWorkbenchView("sv_ship", "Shipment gap", "fulfillment_holds", "manual", "status != shipped", "investigate manual hold pockets before cut-off"),
        ];
    }

    public IReadOnlyList<WorkbenchSource> Sources => _sources;
    public IReadOnlyList<SavedWorkbenchView> SavedViews => _savedViews;
    public string ClockText => DateTimeOffset.UtcNow.AddMinutes(_tick).ToString("HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);

    public IReadOnlyList<NavItem> BuildNavItems(string selectedSourceId)
    {
        return _sources
            .Select(source => new NavItem(source.Id, source.Label, source.Icon, source.Records.Count(record => record.IsHot).ToString("00", CultureInfo.InvariantCulture)))
            .ToArray();
    }

    public WorkbenchSource GetSource(string sourceId)
    {
        return _sources.First(source => string.Equals(source.Id, sourceId, StringComparison.Ordinal));
    }

    public IReadOnlyList<WorkbenchRecord> FilterRecords(string sourceId, string searchQuery, IReadOnlyList<QueryRule> rules)
    {
        var source = GetSource(sourceId);
        IEnumerable<WorkbenchRecord> query = source.Records;

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var term = searchQuery.Trim();
            query = query.Where(record =>
                record.Id.Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Entity.Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Status.Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Region.Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Owner.Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Summary.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var rule in rules.Where(static rule => !rule.IsDisabled && !rule.HasError))
        {
            query = query.Where(record => Matches(record, rule));
        }

        return query.OrderByDescending(record => record.Score).ThenByDescending(record => record.UpdatedAt).ToArray();
    }

    public IReadOnlyList<DataGridColumn> BuildColumns()
    {
        return
        [
            new DataGridColumn("id", "Record") { Width = 14, IsSortable = true, SortComparer = string.CompareOrdinal },
            new DataGridColumn("entity", "Entity") { Width = 16, IsSortable = true, SortComparer = StringComparer.OrdinalIgnoreCase.Compare },
            new DataGridColumn("status", "Status") { Width = 10, IsSortable = true, SortComparer = StringComparer.OrdinalIgnoreCase.Compare },
            new DataGridColumn("region", "Region") { Width = 12, IsSortable = true, SortComparer = StringComparer.OrdinalIgnoreCase.Compare },
            new DataGridColumn("owner", "Owner") { Width = 14, IsSortable = true, SortComparer = StringComparer.OrdinalIgnoreCase.Compare },
            new DataGridColumn("score", "Score") { Width = 7, IsSortable = true, SortComparer = CompareInt },
            new DataGridColumn("latency", "Latency") { Width = 10, IsSortable = true, SortComparer = CompareInt },
            new DataGridColumn("updated", "Updated") { Width = 8, IsSortable = true, SortComparer = StringComparer.Ordinal.Compare },
        ];
    }

    public IReadOnlyList<IReadOnlyList<string>> BuildRows(IReadOnlyList<WorkbenchRecord> records)
    {
        return records
            .Select(record => (IReadOnlyList<string>)
            [
                record.Id,
                record.Entity,
                record.Status,
                record.Region,
                record.Owner,
                record.Score.ToString(CultureInfo.InvariantCulture),
                $"{record.LatencyMs}ms",
                record.UpdatedAt.ToString("HH:mm", CultureInfo.InvariantCulture),
            ])
            .ToArray();
    }

    public WorkbenchRecord? FindRecord(string sourceId, string recordId)
    {
        return GetSource(sourceId).Records.FirstOrDefault(record => string.Equals(record.Id, recordId, StringComparison.Ordinal));
    }

    public string BuildSummary(WorkbenchRecord? record)
    {
        if (record is null)
        {
            return "Select a record to inspect evidence, impacted entities, and execution trace.";
        }

        return string.Join(
            '\n',
            $"Entity      {record.Entity}",
            $"State       {record.Status}  /  score {record.Score:00}",
            $"Region      {record.Region}  /  owner {record.Owner}",
            $"Latency     {record.LatencyMs}ms  /  updated {record.UpdatedAt:HH:mm 'UTC'}",
            $"Narrative   {record.Summary}");
    }

    public string BuildTrace(WorkbenchRecord? record)
    {
        if (record is null)
        {
            return "trace unavailable";
        }

        return string.Join(
            '\n',
            $"{record.Id} :: capture loaded from {record.Workflow}",
            $"{record.Id} :: query fan-out resolved in {record.LatencyMs}ms",
            $"{record.Id} :: enrichment stitched profile, device, and ledger context",
            $"{record.Id} :: compare key {record.CompareKey} now eligible for side-by-side diff");
    }

    public string BuildCompareSummary(WorkbenchRecord? left, WorkbenchRecord? right)
    {
        if (left is null || right is null)
        {
            return "Pin one record and select another to compare anomalies, owners, and payload drift.";
        }

        return string.Join(
            '\n',
            $"Compare key   {left.CompareKey} ↔ {right.CompareKey}",
            $"Severity      {left.Score:00} vs {right.Score:00}",
            $"Latency       {left.LatencyMs}ms vs {right.LatencyMs}ms",
            $"Ownership     {left.Owner} ↔ {right.Owner}",
            $"Observation   {left.Entity} and {right.Entity} diverge on {FirstDifference(left, right)}");
    }

    public string BuildWorkspaceSummary(string sourceId, IReadOnlyList<WorkbenchRecord> records)
    {
        var source = GetSource(sourceId);
        var hottest = records.Count > 0 ? records[0] : null;
        var hotText = hottest is null ? "no hot record selected" : $"{hottest.Id} at {hottest.Score:00}";
        return $"{source.SourceTag}  {source.Description}  ·  {records.Count:00} visible rows  ·  top pressure {hotText}";
    }

    public string BuildPrompt(WorkbenchRecord? record)
    {
        return record is null
            ? "Operator prompt: choose a source, sculpt the slice, then pin a record for compare."
            : $"Operator prompt: follow {record.Id}, validate {record.Workflow}, and decide if {record.Entity} deserves a saved view.";
    }

    public IReadOnlyList<StatItem> BuildPulseItems(IReadOnlyList<WorkbenchRecord> visible)
    {
        return
        [
            new StatItem("Rows", visible.Count.ToString("00", CultureInfo.InvariantCulture)),
            new StatItem("Hot", visible.Count(static record => record.IsHot).ToString("00", CultureInfo.InvariantCulture)),
            new StatItem("Regions", visible.Select(static record => record.Region).Distinct(StringComparer.Ordinal).Count().ToString("00", CultureInfo.InvariantCulture)),
        ];
    }

    public IReadOnlyList<StatItem> BuildVelocityItems(IReadOnlyList<WorkbenchRecord> visible)
    {
        var medianLatency = visible.Count == 0 ? 0 : (int)visible.OrderBy(static record => record.LatencyMs).ElementAt(visible.Count / 2).LatencyMs;
        return
        [
            new StatItem("Median", $"{medianLatency}ms"),
            new StatItem("P95", $"{(visible.Count == 0 ? 0 : visible.Max(static record => record.LatencyMs))}ms"),
            new StatItem("Spike", visible.Count(static record => record.Score >= 85).ToString("00", CultureInfo.InvariantCulture)),
        ];
    }

    public IReadOnlyList<StatItem> BuildCompareItems(WorkbenchRecord? pinned, WorkbenchRecord? current)
    {
        return
        [
            new StatItem("Pinned", pinned?.Id ?? "none"),
            new StatItem("Current", current?.Id ?? "none"),
            new StatItem("Gap", pinned is null || current is null ? "--" : Math.Abs(pinned.Score - current.Score).ToString("00", CultureInfo.InvariantCulture)),
        ];
    }

    public void Advance()
    {
        _tick++;
        foreach (var source in _sources)
        {
            foreach (var record in source.Records)
            {
                var jitter = _random.Next(-3, 4);
                record.Score = Math.Clamp(record.Score + jitter, 4, 98);
                record.LatencyMs = Math.Clamp(record.LatencyMs + _random.Next(-18, 28), 110, 980);
                record.UpdatedAt = record.UpdatedAt.AddSeconds(35);
            }
        }
    }

    public SavedWorkbenchView SaveView(string sourceId, string query, IReadOnlyList<QueryRule> rules)
    {
        var source = GetSource(sourceId);
        var description = string.Join(
            " / ",
            string.IsNullOrWhiteSpace(query) ? "wide-open slice" : query.Trim(),
            rules.Count == 0 ? "no explicit rules" : $"{rules.Count:00} rule lens");
        var view = new SavedWorkbenchView(
            $"sv_user_{_savedViews.Count + 1:00}",
            $"{source.Label} lens {_savedViews.Count + 1:00}",
            sourceId,
            query,
            BuildRuleSummary(rules),
            description);
        _savedViews.Insert(0, view);
        return view;
    }

    private static bool Matches(WorkbenchRecord record, QueryRule rule)
    {
        var fieldValue = ResolveField(record, rule.Field);
        if (fieldValue is null)
        {
            return false;
        }

        return rule.Operator switch
        {
            QueryOperator.Equals => fieldValue.Equals(rule.Value, StringComparison.OrdinalIgnoreCase),
            QueryOperator.NotEquals => !fieldValue.Equals(rule.Value, StringComparison.OrdinalIgnoreCase),
            QueryOperator.Contains => fieldValue.Contains(rule.Value, StringComparison.OrdinalIgnoreCase),
            QueryOperator.StartsWith => fieldValue.StartsWith(rule.Value, StringComparison.OrdinalIgnoreCase),
            QueryOperator.EndsWith => fieldValue.EndsWith(rule.Value, StringComparison.OrdinalIgnoreCase),
            QueryOperator.GreaterThan => CompareDecimal(fieldValue, rule.Value) > 0,
            QueryOperator.GreaterThanOrEqual => CompareDecimal(fieldValue, rule.Value) >= 0,
            QueryOperator.LessThan => CompareDecimal(fieldValue, rule.Value) < 0,
            QueryOperator.LessThanOrEqual => CompareDecimal(fieldValue, rule.Value) <= 0,
            QueryOperator.In => rule.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Any(candidate => fieldValue.Equals(candidate, StringComparison.OrdinalIgnoreCase)),
            QueryOperator.NotIn => rule.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).All(candidate => !fieldValue.Equals(candidate, StringComparison.OrdinalIgnoreCase)),
            QueryOperator.IsEmpty => string.IsNullOrWhiteSpace(fieldValue),
            QueryOperator.IsNotEmpty => !string.IsNullOrWhiteSpace(fieldValue),
            _ => false,
        };
    }

    private static string? ResolveField(WorkbenchRecord record, string field)
    {
        return field.Trim().ToLowerInvariant() switch
        {
            "record" or "id" => record.Id,
            "entity" => record.Entity,
            "status" => record.Status,
            "region" => record.Region,
            "owner" => record.Owner,
            "score" => record.Score.ToString(CultureInfo.InvariantCulture),
            "latency" => record.LatencyMs.ToString(CultureInfo.InvariantCulture),
            "amount" => record.Amount.ToString(CultureInfo.InvariantCulture),
            "workflow" => record.Workflow,
            "kind" => record.CompareKey,
            _ => null,
        };
    }

    private static int CompareDecimal(string left, string right)
    {
        _ = decimal.TryParse(left, NumberStyles.Any, CultureInfo.InvariantCulture, out var leftValue);
        _ = decimal.TryParse(right, NumberStyles.Any, CultureInfo.InvariantCulture, out var rightValue);
        return leftValue.CompareTo(rightValue);
    }

    private static int CompareInt(string left, string right)
    {
        _ = int.TryParse(left.TrimEnd('m', 's'), NumberStyles.Any, CultureInfo.InvariantCulture, out var leftValue);
        _ = int.TryParse(right.TrimEnd('m', 's'), NumberStyles.Any, CultureInfo.InvariantCulture, out var rightValue);
        return leftValue.CompareTo(rightValue);
    }

    private static string BuildRuleSummary(IReadOnlyList<QueryRule> rules)
    {
        if (rules.Count == 0)
        {
            return "open";
        }

        return string.Join(" and ", rules.Select(static rule => $"{rule.Field} {rule.Operator} {rule.Value}".Trim()));
    }

    private static string FirstDifference(WorkbenchRecord left, WorkbenchRecord right)
    {
        if (!string.Equals(left.Status, right.Status, StringComparison.Ordinal))
        {
            return $"status ({left.Status} vs {right.Status})";
        }

        if (!string.Equals(left.Owner, right.Owner, StringComparison.Ordinal))
        {
            return $"owner ({left.Owner} vs {right.Owner})";
        }

        if (!string.Equals(left.Region, right.Region, StringComparison.Ordinal))
        {
            return $"region ({left.Region} vs {right.Region})";
        }

        return $"severity ({left.Score:00} vs {right.Score:00})";
    }

}

internal sealed record WorkbenchSource(
    string Id,
    string Label,
    string Icon,
    string SourceTag,
    string Description,
    IReadOnlyList<WorkbenchRecord> Records);

internal sealed class WorkbenchRecord(
    string id,
    string entity,
    string status,
    string region,
    string owner,
    int score,
    decimal amount,
    int latencyMs,
    DateTimeOffset updatedAt,
    string summary,
    string workflow,
    string compareKey,
    string json)
{
    public string Id { get; } = id;
    public string Entity { get; } = entity;
    public string Status { get; set; } = status;
    public string Region { get; } = region;
    public string Owner { get; } = owner;
    public int Score { get; set; } = score;
    public decimal Amount { get; } = amount;
    public int LatencyMs { get; set; } = latencyMs;
    public DateTimeOffset UpdatedAt { get; set; } = updatedAt;
    public string Summary { get; } = summary;
    public string Workflow { get; } = workflow;
    public string CompareKey { get; } = compareKey;
    public string Json { get; } = json;
    public bool IsHot => Score >= 80 || string.Equals(Status, "priority", StringComparison.OrdinalIgnoreCase) || string.Equals(Status, "escalated", StringComparison.OrdinalIgnoreCase);
}

internal sealed record SavedWorkbenchView(
    string Id,
    string Label,
    string SourceId,
    string Query,
    string RuleSummary,
    string Description);
