namespace Tessera.Styles;

/// <summary>
///     Applies semantic theme tokens to style-enabled controls.
/// </summary>
public static class TesseraThemeControlExtensions
{
    internal static TesseraStyle ApplyDefault(TesseraStyle current, TesseraStyle fallback)
    {
        return current.IsEmpty ? fallback : current;
    }

    internal static string ApplyDefault(string current, string fallback)
    {
        return string.IsNullOrEmpty(current) ? fallback : current;
    }
}
