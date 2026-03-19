namespace TeaSharp.Styles;

/// <summary>
/// Defines semantic border styles for a <see cref="TeaTheme"/>.
/// </summary>
public sealed class TeaThemeBorderTokens
{
    public TeaStyle Default { get; init; } = TeaStyle.Empty;

    public TeaStyle Strong { get; init; } = TeaStyle.Empty;

    public TeaStyle Focused { get; init; } = TeaStyle.Empty;

    public TeaStyle Error { get; init; } = TeaStyle.Empty;
}
