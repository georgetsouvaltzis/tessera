namespace TeaSharp.Styles;

/// <summary>
/// Defines semantic selection styles for a <see cref="TeaTheme"/>.
/// </summary>
public sealed class TeaThemeSelectionTokens
{
    public TeaStyle Foreground { get; init; } = TeaStyle.Empty;

    public TeaStyle Background { get; init; } = TeaStyle.Empty;
}
