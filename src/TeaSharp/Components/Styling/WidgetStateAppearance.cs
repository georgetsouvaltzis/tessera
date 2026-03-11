using TeaSharp.Styles;

namespace TeaSharp.Components.Styling;

public sealed class WidgetStateAppearance
{
    public TeaStyle TextStyle { get; set; } = TeaStyle.Empty;

    public string Prefix { get; set; } = string.Empty;

    public string Suffix { get; set; } = string.Empty;

    public bool Uppercase { get; set; }
}

