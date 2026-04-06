using System.ComponentModel;
using Tessera.Components.Styling.Internal;
using Tessera.Styles;

namespace Tessera.Components.Styling;

[EditorBrowsable(EditorBrowsableState.Advanced)]
internal sealed class WidgetStateAppearance
{
    public TesseraStyle TextStyle { get; set; } = TesseraStyle.Empty;

    public string Prefix { get; set; } = string.Empty;

    public string Suffix { get; set; } = string.Empty;

    public bool Uppercase { get; set; }
}
