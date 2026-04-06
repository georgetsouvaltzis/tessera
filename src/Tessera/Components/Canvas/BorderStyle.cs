using Tessera.Components.Primitives.Internal;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Tessera;

/// <summary>
/// Defines the frame style used when a component renders a border.
/// </summary>
public enum BorderStyle
{
    /// <summary>
    /// No border is rendered.
    /// </summary>
    None = -1,
    /// <summary>
    /// Compatibility alias for <see cref="SingleLine"/>.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Compatibility alias retained for existing consumers. Prefer SingleLine for new code.")]
    Single = 0,
    /// <summary>
    /// Single-line box drawing border.
    /// </summary>
    SingleLine = Single,
    /// <summary>
    /// Rounded box drawing border.
    /// </summary>
    Rounded = 1,
    /// <summary>
    /// Heavy box drawing border.
    /// </summary>
    Heavy = 2,
    /// <summary>
    /// ASCII fallback border.
    /// </summary>
    Ascii = 3,
}
