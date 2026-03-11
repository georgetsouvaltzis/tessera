namespace TeaSharp.Components.Prebuilt.Internal;

internal static class LogViewerContent
{
    public static IEnumerable<string> Filter(IReadOnlyList<string> entries, string filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return entries;
        }

        return entries.Where(line => line.Contains(filter, StringComparison.OrdinalIgnoreCase));
    }
}
