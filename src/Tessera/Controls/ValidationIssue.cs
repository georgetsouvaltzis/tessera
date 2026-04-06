namespace Tessera.Controls;

/// <summary>
/// Represents a single validation issue entry.
/// </summary>
/// <param name="Message">The validation message text.</param>
/// <param name="Severity">The issue severity.</param>
/// <param name="Field">The optional field name associated with the issue.</param>
public sealed record ValidationIssue(
    string Message,
    ValidationSeverity Severity = ValidationSeverity.Error,
    string? Field = null);
