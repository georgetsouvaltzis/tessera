namespace TeaSharp.Components.Composition;

/// <summary>
/// Captures the currently focused region for later restoration.
/// </summary>
/// <param name="RegionKey">The focused region key at capture time.</param>
public readonly record struct ScreenFocusSnapshot(ScreenRegionKey? RegionKey);
