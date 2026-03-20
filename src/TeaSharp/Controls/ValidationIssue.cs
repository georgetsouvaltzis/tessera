namespace TeaSharp.Controls;

/// <summary>
/// Represents a single validation issue displayed by <see cref="ValidationSummary" />.
/// </summary>
/// <param name="Message">The user-facing issue message.</param>
/// <param name="Severity">The issue severity.</param>
/// <param name="Field">Optional field or path associated with the issue.</param>
/// <param name="Code">Optional stable issue code.</param>
public sealed record ValidationIssue(
    string Message,
    ValidationSeverity Severity = ValidationSeverity.Error,
    string? Field = null,
    string? Code = null);
