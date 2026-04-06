namespace TeaSharp.Controls;

/// <summary>
/// Controls how rounded surface-styled buttons compose their shell and body.
/// </summary>
public enum ButtonRoundedSurfaceMode
{
    /// <summary>
    /// Uses a unified rounded shell where border and fill read as one pill surface with inset cap and shoulder rows.
    /// Label-only pills reserve a taller silhouette than description-bearing action buttons.
    /// </summary>
    UnifiedShell = 0,

    /// <summary>
    /// Uses a distinct rounded outline with a separately filled inner body.
    /// This mode reserves enough height for a bordered shell plus centered inset fill, and it suppresses the
    /// default bracket label chrome when apps keep the built-in button label defaults.
    /// </summary>
    InsetBody = 1,
}
