namespace TeaSharp.Controls;

/// <summary>
/// Provides old/new query values for <see cref="SearchBox"/> query changes.
/// </summary>
public sealed class SearchBoxQueryChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new query-changed payload.
    /// </summary>
    /// <param name="previousQuery">The previous query text.</param>
    /// <param name="query">The current query text.</param>
    public SearchBoxQueryChangedEventArgs(string previousQuery, string query)
    {
        PreviousQuery = previousQuery ?? string.Empty;
        Query = query ?? string.Empty;
    }

    /// <summary>
    /// Gets the query value before the change.
    /// </summary>
    public string PreviousQuery { get; }

    /// <summary>
    /// Gets the current query value.
    /// </summary>
    public string Query { get; }
}
