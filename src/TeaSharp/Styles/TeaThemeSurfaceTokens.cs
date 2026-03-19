namespace TeaSharp.Styles;

/// <summary>
/// Defines semantic surface styles for a <see cref="TeaTheme"/>.
/// </summary>
public sealed class TeaThemeSurfaceTokens
{
    public TeaStyle Base { get; init; } = TeaStyle.Empty;

    public TeaStyle Panel { get; init; } = TeaStyle.Empty;

    public TeaStyle Overlay { get; init; } = TeaStyle.Empty;
}
