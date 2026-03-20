using System.Reflection;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;

namespace TeaSharp.Tests;

internal static class BorderedControlParityPolicyTests
{
    private static readonly Type[] RegisteredBorderedControls =
    [
        typeof(Button),
        typeof(Choice),
        typeof(ComboBox),
        typeof(CommandPalette),
        typeof(ContextMenu),
        typeof(DataGrid),
        typeof(DatePicker),
        typeof(Dialog),
        typeof(DiffView),
        typeof(FileExplorer),
        typeof(FuzzyFinder),
        typeof(KeyValueList),
        typeof(Label),
        typeof(ListView<>),
        typeof(LogView),
        typeof(MarkdownView),
        typeof(MenuBar),
        typeof(Modal),
        typeof(Notifications),
        typeof(NumberInput),
        typeof(ProgressBar),
        typeof(PropertyGrid),
        typeof(SearchBox),
        typeof(Slider),
        typeof(Spinner),
        typeof(Table),
        typeof(TextArea),
        typeof(TextInput),
        typeof(TimePicker),
        typeof(Timeline),
        typeof(ToastCenter),
        typeof(Toggle),
        typeof(TreeTable),
        typeof(TreeView),
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "VisualParityPolicy_BorderedControlManifest_MatchesPublicBorderedControls",
            BorderedControlManifest_MatchesPublicBorderedControls);
    }

    private static Task BorderedControlManifest_MatchesPublicBorderedControls()
    {
        var discovered = DiscoverBorderedControlTypeNames();
        var registered = new HashSet<string>(RegisteredBorderedControls.Select(static type => type.FullName ?? type.Name), StringComparer.Ordinal);

        var missing = discovered.Where(name => !registered.Contains(name)).OrderBy(static name => name, StringComparer.Ordinal).ToArray();
        var stale = registered.Where(name => !discovered.Contains(name)).OrderBy(static name => name, StringComparer.Ordinal).ToArray();

        TestAssert.True(
            missing.Length == 0 && stale.Length == 0,
            $"Bordered control parity manifest drift detected. Missing registrations: {string.Join(", ", missing)}. Stale registrations: {string.Join(", ", stale)}.");

        return Task.CompletedTask;
    }

    private static HashSet<string> DiscoverBorderedControlTypeNames()
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        var assembly = typeof(Control).Assembly;

        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsPublic || type.IsAbstract || !typeof(Control).IsAssignableFrom(type))
            {
                continue;
            }

            var borderProperty = type.GetProperty(
                "Border",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            if (borderProperty?.PropertyType != typeof(BorderStyle))
            {
                continue;
            }

            result.Add(type.FullName ?? type.Name);
        }

        return result;
    }
}
