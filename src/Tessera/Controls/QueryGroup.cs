namespace Tessera.Controls;

/// <summary>
///     Represents a flat group of query rules combined by AND/OR.
/// </summary>
public sealed class QueryGroup
{
    private readonly List<QueryRule> _rules = [];

    /// <summary>
    ///     Gets or sets whether group rules are combined with OR.
    /// </summary>
    public bool UseOr { get; set; }

    /// <summary>
    ///     Gets rules in this group.
    /// </summary>
    public IReadOnlyList<QueryRule> Rules => _rules;

    /// <summary>
    ///     Gets number of rules in this group.
    /// </summary>
    public int Count => _rules.Count;

    /// <summary>
    ///     Replaces rules in this group.
    /// </summary>
    /// <param name="rules">Rules to set.</param>
    public void SetRules(IEnumerable<QueryRule> rules)
    {
        ArgumentNullException.ThrowIfNull(rules);
        _rules.Clear();
        foreach (var rule in rules.Where(static rule => rule is not null))
        {
            _rules.Add(rule);
        }
    }

    /// <summary>
    ///     Adds a rule to this group.
    /// </summary>
    /// <param name="rule">Rule to add.</param>
    public void AddRule(QueryRule rule)
    {
        ArgumentNullException.ThrowIfNull(rule);
        _rules.Add(rule);
    }

    /// <summary>
    ///     Removes a rule by index.
    /// </summary>
    /// <param name="index">Rule index.</param>
    /// <returns><see langword="true" /> when removed.</returns>
    public bool RemoveRuleAt(int index)
    {
        if ((uint)index >= (uint)_rules.Count)
        {
            return false;
        }

        _rules.RemoveAt(index);
        return true;
    }

    /// <summary>
    ///     Clears all rules.
    /// </summary>
    public void ClearRules()
    {
        _rules.Clear();
    }
}
