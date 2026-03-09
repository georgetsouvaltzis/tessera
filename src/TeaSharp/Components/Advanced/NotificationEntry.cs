using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using TeaSharp.Widgets;

namespace TeaSharp.Components;

public sealed record NotificationEntry(
    string Id,
    string Message,
    NotificationSeverity Severity,
    DateTimeOffset CreatedAt,
    bool IsRead = false);

