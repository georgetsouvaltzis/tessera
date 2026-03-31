using System.Text.Json;
using TeaSharp.Controls;

namespace TeaSharp.Examples.DataWorkbench;

internal sealed partial class DataWorkbenchState
{
    private static readonly JsonSerializerOptions PayloadJsonOptions = new() { WriteIndented = true };

    public static DataWorkbenchState CreateSeed()
    {
        return new DataWorkbenchState(
        [
            BuildSource(
                "fraud_signals",
                "Fraud Signals",
                "FS",
                "chargeback_risk",
                "Atlas ring / live scoring",
                [
                    Record("risk_10442", "amaya cole", "watch", "eu-west-1", "atlas-risk", 86, 12450m, 381, "Card velocity divergence across two devices in twelve minutes.", "chargeback_risk", "session"),
                    Record("risk_10443", "luca ramos", "escalated", "eu-central-1", "atlas-risk", 94, 21890m, 441, "Coupon abuse pattern overlaps a recovered device fingerprint.", "chargeback_risk", "session"),
                    Record("risk_10444", "mia tang", "clear", "us-east-1", "atlas-risk", 28, 4200m, 219, "Risk dropped after device graph converged with prior trusted profile.", "chargeback_risk", "session"),
                    Record("risk_10445", "karim saleh", "watch", "eu-west-2", "manual-review", 71, 16500m, 315, "Issuer mismatch plus freight-forwarder destination requires manual review.", "chargeback_risk", "order"),
                    Record("risk_10446", "sofia lind", "priority", "ap-southeast-1", "atlas-risk", 90, 17880m, 389, "BIN cluster surge and reseller pattern detected across sister tenants.", "chargeback_risk", "order"),
                    Record("risk_10447", "jonas klein", "clear", "us-west-2", "atlas-risk", 35, 5980m, 204, "Device aged in without additional anomaly after challenge pass.", "chargeback_risk", "session"),
                    Record("risk_10448", "nina maric", "watch", "eu-north-1", "manual-review", 79, 12320m, 336, "High-value digital goods bundle still lacks trusted tenure history.", "chargeback_risk", "bundle"),
                ]),
            BuildSource(
                "fulfillment_holds",
                "Fulfillment Holds",
                "FH",
                "warehouse_dispatch",
                "Warehouse / dispatch exceptions",
                [
                    Record("hold_30012", "order 30012", "manual", "ams-2", "warehouse-a", 62, 9420m, 611, "Cold-chain parcel requires carrier override after route downgrade.", "warehouse_dispatch", "shipment"),
                    Record("hold_30019", "order 30019", "stalled", "ams-1", "warehouse-a", 77, 11480m, 708, "Inventory reservation split across bins and awaiting picker confirmation.", "warehouse_dispatch", "shipment"),
                    Record("hold_30024", "order 30024", "cleared", "fra-1", "warehouse-b", 18, 3220m, 195, "Replenishment caught up and label regenerated successfully.", "warehouse_dispatch", "shipment"),
                    Record("hold_30031", "order 30031", "priority", "fra-2", "control-floor", 88, 21540m, 812, "VIP cutoff window under 15 minutes with export paperwork still open.", "warehouse_dispatch", "shipment"),
                    Record("hold_30044", "order 30044", "manual", "iad-1", "warehouse-c", 59, 6030m, 430, "Packaging exception triggered after hazmat fee reclassification.", "warehouse_dispatch", "parcel"),
                    Record("hold_30052", "order 30052", "stalled", "sin-1", "warehouse-d", 80, 18900m, 754, "Carrier pickup missed after dock congestion and late handoff.", "warehouse_dispatch", "shipment"),
                    Record("hold_30057", "order 30057", "watch", "lhr-1", "warehouse-b", 53, 7310m, 506, "Manual spot-check requested after weight variance on carton merge.", "warehouse_dispatch", "parcel"),
                ]),
            BuildSource(
                "refund_journal",
                "Refund Journal",
                "RJ",
                "payments_refunds",
                "Payments / dispute and refund journal",
                [
                    Record("refund_8801", "refund 8801", "pending", "eu-west-1", "ledger-team", 68, 4870m, 256, "Partial refund queued while issuer callback window remains open.", "payments_refunds", "refund"),
                    Record("refund_8802", "refund 8802", "posted", "eu-west-1", "ledger-team", 26, 2290m, 188, "Refund posted and settlement trail reconciled in nightly batch.", "payments_refunds", "refund"),
                    Record("refund_8803", "refund 8803", "chargeback", "us-east-1", "disputes", 91, 9820m, 402, "Issuer escalated before merchant evidence packet was assembled.", "payments_refunds", "dispute"),
                    Record("refund_8804", "refund 8804", "pending", "ap-northeast-1", "ledger-team", 72, 6450m, 311, "Cross-currency adjustment requires manual sign-off after FX swing.", "payments_refunds", "refund"),
                    Record("refund_8805", "refund 8805", "review", "eu-central-1", "disputes", 84, 7330m, 364, "Multiple partial refunds against one authorization need fraud review.", "payments_refunds", "dispute"),
                    Record("refund_8806", "refund 8806", "posted", "us-west-2", "ledger-team", 24, 2110m, 171, "Settlement matched processor ledger and customer wallet reversal.", "payments_refunds", "refund"),
                    Record("refund_8807", "refund 8807", "pending", "sa-east-1", "ledger-team", 64, 5880m, 298, "Bank holiday queue keeps transfer in pending state beyond target.", "payments_refunds", "refund"),
                ]),
            BuildSource(
                "catalog_drift",
                "Catalog Drift",
                "CD",
                "inventory_sync",
                "Catalog / sync and pricing divergence",
                [
                    Record("drift_2041", "sku 2041", "drift", "eu-west-1", "catalog-bot", 81, 0m, 527, "Marketplace price undercut differs from source of truth by 14 percent.", "inventory_sync", "sku"),
                    Record("drift_2043", "sku 2043", "watch", "us-east-1", "catalog-bot", 63, 0m, 409, "Image asset set lagging behind inventory revision for two channels.", "inventory_sync", "sku"),
                    Record("drift_2046", "sku 2046", "clear", "ap-south-1", "catalog-bot", 19, 0m, 182, "All channels converged after overnight sync repair.", "inventory_sync", "sku"),
                    Record("drift_2051", "sku 2051", "drift", "eu-west-2", "pricing-desk", 87, 0m, 602, "Bundle component price changed but storefront aggregate did not.", "inventory_sync", "bundle"),
                    Record("drift_2052", "sku 2052", "priority", "us-west-1", "pricing-desk", 93, 0m, 734, "Holiday launch page still serving stale launch price to cache cohort.", "inventory_sync", "campaign"),
                    Record("drift_2058", "sku 2058", "watch", "ca-central-1", "catalog-bot", 57, 0m, 352, "Inventory feed delayed after supplier payload schema drift.", "inventory_sync", "sku"),
                    Record("drift_2060", "sku 2060", "clear", "eu-north-1", "catalog-bot", 21, 0m, 210, "Variant matrix fully reconciled after supplier reissue.", "inventory_sync", "sku"),
                ]),
        ]);
    }

    public IReadOnlyList<ActivityFeedItem> BuildSeedActivities()
    {
        return
        [
            new ActivityFeedItem("workbench", "hydrated", "atlas ring", "replayed saved investigation lenses", ActivityFeedItemKind.Success, DateTimeOffset.UtcNow.AddMinutes(-28)),
            new ActivityFeedItem("analyst", "flagged", "fraud_signals", "score surge on eu-west cohort", ActivityFeedItemKind.Warning, DateTimeOffset.UtcNow.AddMinutes(-21)),
            new ActivityFeedItem("query", "re-ran", "fulfillment_holds", "manual hold pocket collapsed from 14 to 7 rows", ActivityFeedItemKind.Info, DateTimeOffset.UtcNow.AddMinutes(-13)),
            new ActivityFeedItem("compare", "pinned", "refund_8805", "paired against refund_8803 for dispute drift", ActivityFeedItemKind.Info, DateTimeOffset.UtcNow.AddMinutes(-7)),
            new ActivityFeedItem("export", "queued", "catalog_drift", "handoff payload prepared for pricing desk", ActivityFeedItemKind.Success, DateTimeOffset.UtcNow.AddMinutes(-3)),
        ];
    }

    public IReadOnlyList<CommandOutputLine> BuildSeedOutput()
    {
        return
        [
            Sys("booted workbench / hydrated 4 sources / loaded 28 seeded records", -24),
            Sys("lens atlas burn watch restored / source fraud_signals / query score >= 70", -21),
            Out("query run / 7 rows returned / 381ms median", -14),
            Out("compare pinned / refund_8805 ↔ refund_8803", -8),
            Out("saved view drift pocket sealed to analyst handoff", -4),
        ];
    }

    private static WorkbenchSource BuildSource(
        string id,
        string label,
        string icon,
        string sourceTag,
        string description,
        IReadOnlyList<WorkbenchRecord> records)
    {
        return new WorkbenchSource(id, label, icon, sourceTag, description, records);
    }

    private static WorkbenchRecord Record(
        string id,
        string entity,
        string status,
        string region,
        string owner,
        int score,
        decimal amount,
        int latencyMs,
        string summary,
        string workflow,
        string compareKey)
    {
        var payload = new
        {
            id,
            entity,
            status,
            region,
            owner,
            score,
            amount,
            latencyMs,
            summary,
            workflow,
            compareKey,
            evidence = new[]
            {
                $"origin:{region}",
                $"owner:{owner}",
                $"pressure:{score}",
            },
            trace = new
            {
                stage = workflow,
                partitions = new[] { "alpha", "delta", "sigma" },
                freshness = "live",
            },
        };

        return new WorkbenchRecord(
            id,
            entity,
            status,
            region,
            owner,
            score,
            amount,
            latencyMs,
            DateTimeOffset.UtcNow.AddMinutes(-(score % 19 + 2)),
            summary,
            workflow,
            compareKey,
            JsonSerializer.Serialize(payload, PayloadJsonOptions));
    }

    private static CommandOutputLine Sys(string text, int minutesAgo) =>
        new(text, CommandOutputChannel.System, DateTimeOffset.UtcNow.AddMinutes(minutesAgo));

    private static CommandOutputLine Out(string text, int minutesAgo) =>
        new(text, CommandOutputChannel.StdOut, DateTimeOffset.UtcNow.AddMinutes(minutesAgo));
}
