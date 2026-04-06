namespace Tessera.Controls;

/// <summary>
/// Identifies a requested search navigation direction.
/// </summary>
public enum SearchNavigationDirection
{
    /// <summary>
    /// Navigate to the next match.
    /// </summary>
    Next = 0,

    /// <summary>
    /// Navigate to the previous match.
    /// </summary>
    Previous = 1,
}

/// <summary>
/// Provides details when a search navigation command is requested.
/// </summary>
public sealed class SearchBoxNavigationRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new navigation request payload.
    /// </summary>
    /// <param name="direction">The navigation direction that was requested.</param>
    /// <param name="previousMatchIndex">The match index before navigation.</param>
    /// <param name="currentMatchIndex">The match index after navigation.</param>
    /// <param name="matchCount">The total known match count, when available.</param>
    public SearchBoxNavigationRequestedEventArgs(
        SearchNavigationDirection direction,
        int? previousMatchIndex,
        int? currentMatchIndex,
        int? matchCount)
    {
        Direction = direction;
        PreviousMatchIndex = previousMatchIndex;
        CurrentMatchIndex = currentMatchIndex;
        MatchCount = matchCount;
    }

    /// <summary>
    /// Gets the requested navigation direction.
    /// </summary>
    public SearchNavigationDirection Direction { get; }

    /// <summary>
    /// Gets the match index before navigation.
    /// </summary>
    public int? PreviousMatchIndex { get; }

    /// <summary>
    /// Gets the match index after navigation.
    /// </summary>
    public int? CurrentMatchIndex { get; }

    /// <summary>
    /// Gets the known total match count, when available.
    /// </summary>
    public int? MatchCount { get; }
}
