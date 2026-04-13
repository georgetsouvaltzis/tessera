namespace Tessera.Controls;

/// <summary>
///     Provides query text and metadata when <see cref="QueryBuilder" /> changes.
/// </summary>
public sealed class QueryChangedEventArgs : EventArgs
{
    /// <summary>
    ///     Initializes a query-changed payload.
    /// </summary>
    /// <param name="previousQuery">Previous query text.</param>
    /// <param name="query">Current query text.</param>
    /// <param name="ruleCount">Current rule count.</param>
    /// <param name="useOr"><see langword="true" /> when OR combinator is active.</param>
    public QueryChangedEventArgs(string previousQuery, string query, int ruleCount, bool useOr)
    {
        PreviousQuery = previousQuery;
        Query = query;
        RuleCount = ruleCount;
        UseOr = useOr;
    }

    /// <summary>
    ///     Gets previous query text.
    /// </summary>
    public string PreviousQuery { get; }

    /// <summary>
    ///     Gets current query text.
    /// </summary>
    public string Query { get; }

    /// <summary>
    ///     Gets current rule count.
    /// </summary>
    public int RuleCount { get; }

    /// <summary>
    ///     Gets whether OR combinator is active.
    /// </summary>
    public bool UseOr { get; }
}
