namespace Tessera.Layout;

/// <summary>
///     Represents how a layout slot should consume space.
/// </summary>
public enum LayoutLengthKind
{
    /// <summary>
    ///     The auto value.
    /// </summary>
    Auto = 0,

    /// <summary>
    ///     The fixed value.
    /// </summary>
    Fixed = 1,

    /// <summary>
    ///     The fill value.
    /// </summary>
    Fill = 2,

    /// <summary>
    ///     The weighted value.
    /// </summary>
    Weighted = 3
}
