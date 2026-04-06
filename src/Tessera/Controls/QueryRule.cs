namespace Tessera.Controls;

/// <summary>
/// Represents one field/operator/value rule in a query builder.
/// </summary>
public sealed class QueryRule
{
    /// <summary>
    /// Initializes a new rule.
    /// </summary>
    /// <param name="field">Field name.</param>
    /// <param name="operator">Rule operator.</param>
    /// <param name="value">Optional rule value.</param>
    public QueryRule(string field, QueryOperator @operator, string? value = null)
    {
        Field = field ?? string.Empty;
        Operator = @operator;
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets field name.
    /// </summary>
    public string Field
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets rule operator.
    /// </summary>
    public QueryOperator Operator { get; set; }

    /// <summary>
    /// Gets or sets rule value.
    /// </summary>
    public string Value
    {
        get;
        set => field = value ?? string.Empty;
    }

    /// <summary>
    /// Gets or sets whether this rule is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    /// Gets or sets whether this rule is in error state.
    /// </summary>
    public bool HasError { get; set; }

    internal bool RequiresValue =>
        Operator is not QueryOperator.IsEmpty and not QueryOperator.IsNotEmpty;
}
