using System.Reflection;
using System.Runtime.CompilerServices;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class BorderedControlParityPolicyTests
{
    private static readonly Type[] RegisteredBorderedControls =
    [
        typeof(ActivityFeed),
        typeof(AreaPlot),
        typeof(Button),
        typeof(Choice),
        typeof(ComboBox),
        typeof(CommandOutput),
        typeof(CommandPalette),
        typeof(ContextMenu),
        typeof(DataGrid),
        typeof(DatePicker),
        typeof(Dialog),
        typeof(DiffView),
        typeof(DockWorkspace),
        typeof(FileExplorer),
        typeof(FuzzyFinder),
        typeof(GroupedListView<,>),
        typeof(Heatmap),
        typeof(JsonTreeView),
        typeof(KanbanBoard),
        typeof(KeyValueList),
        typeof(Label),
        typeof(ListView<>),
        typeof(LinePlot),
        typeof(LogTailPanel),
        typeof(LogView),
        typeof(MarkdownView),
        typeof(MenuBar),
        typeof(Modal),
        typeof(Notifications),
        typeof(NumberInput),
        typeof(PaneTabs),
        typeof(PlotPanel),
        typeof(PivotTable),
        typeof(ProcessListView),
        typeof(ProgressBar),
        typeof(PropertyGrid),
        typeof(QueryBuilder),
        typeof(RichTextView),
        typeof(SearchBox),
        typeof(SearchResultsView),
        typeof(Slider),
        typeof(Sparkline),
        typeof(Spinner),
        typeof(TagInput),
        typeof(Table),
        typeof(TaskRunnerPanel),
        typeof(TextArea),
        typeof(TextInput),
        typeof(TimePicker),
        typeof(Timeline),
        typeof(ToastCenter),
        typeof(Toggle),
        typeof(TraceViewer),
        typeof(TreeMapChart),
        typeof(TreeTable),
        typeof(TreeView),
        typeof(ValidationSummary),
        typeof(VirtualizedListView<>),
    ];

    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "VisualParityPolicy_BorderedControlManifest_MatchesPublicBorderedControls",
            BorderedControlManifest_MatchesPublicBorderedControls);
        yield return new TestCase(
            "VisualParityPolicy_BorderedControls_ExposeBorderStyleHooks",
            BorderedControls_ExposeBorderStyleHooks);
        yield return new TestCase(
            "VisualParityPolicy_BorderedControls_HaveThemeApplyAndDefaultsExtensions",
            BorderedControls_HaveThemeApplyAndDefaultsExtensions);
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

    private static Task BorderedControls_ExposeBorderStyleHooks()
    {
        var borderedControls = DiscoverBorderedControls();
        var missingBorderStyleText = new List<string>();
        var missingFocusedBorderStyleText = new List<string>();

        foreach (var controlType in borderedControls)
        {
            var borderStyleProperty = controlType.GetProperty(nameof(Button.BorderStyleText), BindingFlags.Instance | BindingFlags.Public);
            if (borderStyleProperty?.PropertyType != typeof(TeaStyle))
            {
                missingBorderStyleText.Add(FormatTypeName(controlType));
            }

            var focusedBorderStyleProperty = controlType.GetProperty(nameof(Button.FocusedBorderStyleText), BindingFlags.Instance | BindingFlags.Public);
            if (focusedBorderStyleProperty?.PropertyType != typeof(TeaStyle))
            {
                missingFocusedBorderStyleText.Add(FormatTypeName(controlType));
            }
        }

        TestAssert.True(
            missingBorderStyleText.Count == 0 && missingFocusedBorderStyleText.Count == 0,
            $"Bordered controls missing style hook properties. Missing BorderStyleText: {string.Join(", ", missingBorderStyleText)}. Missing FocusedBorderStyleText: {string.Join(", ", missingFocusedBorderStyleText)}.");

        return Task.CompletedTask;
    }

    private static Task BorderedControls_HaveThemeApplyAndDefaultsExtensions()
    {
        var borderedControls = DiscoverBorderedControls();
        var extensionMethods = typeof(TeaThemeControlExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static);
        var missingApplyTheme = new List<string>();
        var missingApplyThemeDefaults = new List<string>();

        foreach (var controlType in borderedControls)
        {
            if (!HasThemeMethod(extensionMethods, "ApplyTheme", controlType))
            {
                missingApplyTheme.Add(FormatTypeName(controlType));
            }

            if (!HasThemeMethod(extensionMethods, "ApplyThemeDefaults", controlType))
            {
                missingApplyThemeDefaults.Add(FormatTypeName(controlType));
            }
        }

        TestAssert.True(
            missingApplyTheme.Count == 0 && missingApplyThemeDefaults.Count == 0,
            $"Bordered controls missing TeaTheme extension coverage. Missing ApplyTheme(this TControl, TeaTheme): {string.Join(", ", missingApplyTheme)}. Missing ApplyThemeDefaults(this TControl, TeaTheme): {string.Join(", ", missingApplyThemeDefaults)}.");

        return Task.CompletedTask;
    }

    private static HashSet<string> DiscoverBorderedControlTypeNames()
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (var type in DiscoverBorderedControls())
        {
            result.Add(type.FullName ?? type.Name);
        }

        return result;
    }

    private static Type[] DiscoverBorderedControls()
    {
        var result = new List<Type>();
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

            result.Add(type);
        }

        result.Sort(static (left, right) => StringComparer.Ordinal.Compare(FormatTypeName(left), FormatTypeName(right)));
        return [.. result];
    }

    private static bool HasThemeMethod(MethodInfo[] extensionMethods, string methodName, Type controlType)
    {
        foreach (var method in extensionMethods)
        {
            if (!string.Equals(method.Name, methodName, StringComparison.Ordinal))
            {
                continue;
            }

            if (!method.IsDefined(typeof(ExtensionAttribute), inherit: false))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length != 2 || parameters[1].ParameterType != typeof(TeaTheme))
            {
                continue;
            }

            if (!MatchesControlType(parameters[0].ParameterType, controlType))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    private static bool MatchesControlType(Type parameterType, Type controlType)
    {
        if (parameterType == controlType)
        {
            return true;
        }

        if (controlType.IsGenericTypeDefinition)
        {
            return parameterType.IsGenericType
                && parameterType.GetGenericTypeDefinition() == controlType;
        }

        return false;
    }

    private static string FormatTypeName(Type type)
    {
        if (!type.IsGenericTypeDefinition)
        {
            return type.FullName ?? type.Name;
        }

        var fullName = type.FullName ?? type.Name;
        var tickIndex = fullName.IndexOf('`', StringComparison.Ordinal);
        return tickIndex > 0
            ? $"{fullName[..tickIndex]}<>"
            : $"{fullName}<>";
    }
}
