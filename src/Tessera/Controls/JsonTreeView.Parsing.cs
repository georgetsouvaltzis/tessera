using System.Text.Json;

namespace Tessera.Controls;

public sealed partial class JsonTreeView
{
    private static List<JsonTreeNode> ParseJson(string json)
    {
        using var document = JsonDocument.Parse(json);
        var nodes = new List<JsonTreeNode>();
        var root = document.RootElement;
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                nodes.Add(ParseElement(property.Name, property.Value));
            }

            return nodes;
        }

        if (root.ValueKind == JsonValueKind.Array)
        {
            var index = 0;
            foreach (var item in root.EnumerateArray())
            {
                nodes.Add(ParseElement($"[{index}]", item));
                index++;
            }

            return nodes;
        }

        nodes.Add(ParseElement("$", root));
        return nodes;
    }

    private static JsonTreeNode ParseElement(string key, JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var children = new List<JsonTreeNode>();
            foreach (var property in element.EnumerateObject())
            {
                children.Add(ParseElement(property.Name, property.Value));
            }

            return new JsonTreeNode(key, "{...}", JsonTreeNodeKind.ObjectNode, children) { Expanded = true };
        }

        if (element.ValueKind == JsonValueKind.Array)
        {
            var children = new List<JsonTreeNode>();
            var index = 0;
            foreach (var child in element.EnumerateArray())
            {
                children.Add(ParseElement($"[{index}]", child));
                index++;
            }

            return new JsonTreeNode(key, $"[{children.Count}]", JsonTreeNodeKind.Array, children) { Expanded = true };
        }

        return new JsonTreeNode(key, FormatScalar(element), JsonTreeNodeKind.Value);
    }

    private static string FormatScalar(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => $"\"{element.GetString() ?? string.Empty}\"",
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "null",
            _ => element.GetRawText()
        };
    }
}
