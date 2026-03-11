using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace TeaSharp.Components.Primitives;

public enum BorderStyle
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Compatibility alias retained for existing consumers. Prefer SingleLine for new code.")]
    Single = 0,
    SingleLine = Single,
    Rounded = 1,
    Heavy = 2,
    Ascii = 3,
}
