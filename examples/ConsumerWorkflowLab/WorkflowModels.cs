internal sealed class WorkflowDraft
{
    public string Service { get; set; } = "checkout-api";

    public string OwnerEmail { get; set; } = "owner@company.com";

    public string Environment { get; set; } = "Development";

    public string TargetRegion { get; set; } = "us-east-1";

    public string ChangeWindow { get; set; } = "09:00-11:00";

    public int RolloutPercent { get; set; } = 20;

    public string RollbackPlan { get; set; } = "Scale down new pool and restore previous image.";

    public string ChangeTicket { get; set; } = "CHG-100001";

    public static WorkflowDraft CreateDefault()
    {
        return new WorkflowDraft();
    }
}

internal sealed record WorkflowTemplate(
    string Name,
    string Environment,
    string TargetRegion,
    string ChangeWindow,
    int RolloutPercent,
    string RollbackPlan,
    string ChangeTicket);
