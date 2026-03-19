namespace TeaSharp.Styles;

/// <summary>
/// Defines semantic state styles for a <see cref="TeaTheme"/>.
/// </summary>
public sealed class TeaThemeStateTokens
{
    public TeaStyle Success { get; init; } = TeaStyle.Empty;

    public TeaStyle Warning { get; init; } = TeaStyle.Empty;

    public TeaStyle Error { get; init; } = TeaStyle.Empty;

    public TeaStyle Info { get; init; } = TeaStyle.Empty;
}
