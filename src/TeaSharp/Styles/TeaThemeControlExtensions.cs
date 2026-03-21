namespace TeaSharp.Styles;

/// <summary>
/// Applies semantic theme tokens to style-enabled controls.
/// </summary>
public static partial class TeaThemeControlExtensions
{
    private static TeaStyle ApplyDefault(TeaStyle current, TeaStyle fallback)
    {
        return current.IsEmpty ? fallback : current;
    }

    private static string ApplyDefault(string current, string fallback)
    {
        return string.IsNullOrEmpty(current) ? fallback : current;
    }
}
