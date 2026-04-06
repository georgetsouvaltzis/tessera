namespace Tessera.Styles;

/// <summary>
/// Applies semantic theme tokens to style-enabled controls.
/// </summary>
public static partial class TesseraThemeControlExtensions
{
    private static TesseraStyle ApplyDefault(TesseraStyle current, TesseraStyle fallback)
    {
        return current.IsEmpty ? fallback : current;
    }

    private static string ApplyDefault(string current, string fallback)
    {
        return string.IsNullOrEmpty(current) ? fallback : current;
    }
}
