namespace TeaSharp.Styles;

/// <summary>
/// Defines semantic text styles for a <see cref="TeaTheme"/>.
/// </summary>
public sealed class TeaThemeTextTokens
{
    public TeaStyle Primary { get; init; } = TeaStyle.Empty;

    public TeaStyle Secondary { get; init; } = TeaStyle.Empty;

    public TeaStyle Muted { get; init; } = TeaStyle.Empty;

    public TeaStyle Inverse { get; init; } = TeaStyle.Empty;
}
