using System.ComponentModel;

namespace TeaSharp.Components.Composition;

/// <summary>
/// Defines an ordered set of focusable regions for app-level navigation helpers.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class ScreenFocusChain
{
    private readonly ScreenRegionKey[] _regionKeys;

    /// <summary>
    /// Initializes a new ordered focus chain.
    /// </summary>
    /// <param name="regionKeys">The region keys in preferred focus order.</param>
    public ScreenFocusChain(IEnumerable<ScreenRegionKey> regionKeys)
    {
        _regionKeys = (regionKeys ?? throw new ArgumentNullException(nameof(regionKeys))).ToArray();
    }

    internal IReadOnlyList<ScreenRegionKey> RegionKeys => _regionKeys;
}
