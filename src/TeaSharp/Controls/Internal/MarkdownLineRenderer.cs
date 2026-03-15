using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
namespace TeaSharp.Controls.Internal;

internal static class MarkdownLineRenderer
{
    public static List<string> Render(string markdown)
    {
        var lines = markdown
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n')
            .Split('\n');

        var output = new List<string>(lines.Length);
        var inCode = false;
        foreach (var raw in lines)
        {
            var line = raw ?? string.Empty;
            var trimmed = line.TrimStart();

            if (trimmed.StartsWith("```", StringComparison.Ordinal))
            {
                inCode = !inCode;
                output.Add(inCode ? "┌ code" : "└");
                continue;
            }

            if (inCode)
            {
                output.Add($"  {line}");
                continue;
            }

            if (trimmed.StartsWith("### ", StringComparison.Ordinal))
            {
                output.Add($"### {trimmed[4..]}");
                continue;
            }

            if (trimmed.StartsWith("## ", StringComparison.Ordinal))
            {
                output.Add($"## {trimmed[3..]}");
                continue;
            }

            if (trimmed.StartsWith("# ", StringComparison.Ordinal))
            {
                output.Add($"# {trimmed[2..].ToUpperInvariant()}");
                continue;
            }

            if (trimmed.StartsWith("- ", StringComparison.Ordinal) || trimmed.StartsWith("* ", StringComparison.Ordinal))
            {
                output.Add($"• {trimmed[2..]}");
                continue;
            }

            output.Add(line);
        }

        return output;
    }
}
