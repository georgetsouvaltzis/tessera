using System.Reflection;
using System.Runtime.CompilerServices;
using Tessera.Components.Primitives;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static class BorderedControlParityPolicyTests
{
    private static readonly Type[] RegisteredBorderedControls =
    [
        typeof(ActivityFeed),
        typeof(AutocompleteInput),
        typeof(AreaPlot),
        typeof(BoxPlot),
        typeof(BulletChart),
        typeof(Choice),
        typeof(ComboBox),
        typeof(CommandOutput),
        typeof(CommandPalette),
        typeof(ContextMenu),
        typeof(DataGrid),
        typeof(DataForm<>),
        typeof(DashboardGrid),
        typeof(DatePicker),
        typeof(Dialog),
        typeof(DiffView),
        typeof(DockWorkspace),
        typeof(FieldSet),
        typeof(FileExplorer),
        typeof(Form),
        typeof(FuzzyFinder),
        typeof(GroupedListView<,>),
        typeof(Heatmap),
        typeof(InspectorPanel),
        typeof(JsonTreeView),
        typeof(JumpList),
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
        typeof(ResizablePaneGroup),
        typeof(RichTextView),
        typeof(SearchBox),
        typeof(SearchResultsView),
        typeof(SideNavRail),
        typeof(Slider),
        typeof(SplitView),
        typeof(Sparkline),
        typeof(TelemetryChart),
        typeof(StatsCard),
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
        typeof(TokenEditor),
        typeof(TraceViewer),
        typeof(TreeMapChart),
        typeof(TreeTable),
        typeof(TreeView),
        typeof(ValidationSummary),
        typeof(VirtualizedListView<>),
        typeof(Wizard),
        typeof(HealthBoard),
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
        yield return new TestCase(
            "VisualParityPolicy_BorderedControls_HaveThemeOverrideExtensions",
            BorderedControls_HaveThemeOverrideExtensions);
        yield return new TestCase(
            "VisualParityPolicy_BorderedControls_ExposePrimaryAndFocusVisualHooks",
            BorderedControls_ExposePrimaryAndFocusVisualHooks);
        yield return new TestCase(
            "VisualParityPolicy_BorderedControls_WithFocusMarker_ExposeShowToggleAndSetter",
            BorderedControls_WithFocusMarker_ExposeShowToggleAndSetter);
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
        var missingBorderStyleSetter = new List<string>();
        var missingFocusedBorderStyleSetter = new List<string>();

        foreach (var controlType in borderedControls)
        {
            var borderStyleProperty = controlType.GetProperty("BorderStyleText", BindingFlags.Instance | BindingFlags.Public);
            if (borderStyleProperty?.PropertyType != typeof(TesseraStyle))
            {
                missingBorderStyleText.Add(FormatTypeName(controlType));
            }
            else if (borderStyleProperty.SetMethod is null || !borderStyleProperty.SetMethod.IsPublic)
            {
                missingBorderStyleSetter.Add(FormatTypeName(controlType));
            }

            var focusedBorderStyleProperty = controlType.GetProperty("FocusedBorderStyleText", BindingFlags.Instance | BindingFlags.Public);
            if (focusedBorderStyleProperty?.PropertyType != typeof(TesseraStyle))
            {
                missingFocusedBorderStyleText.Add(FormatTypeName(controlType));
            }
            else if (focusedBorderStyleProperty.SetMethod is null || !focusedBorderStyleProperty.SetMethod.IsPublic)
            {
                missingFocusedBorderStyleSetter.Add(FormatTypeName(controlType));
            }
        }

        TestAssert.True(
            missingBorderStyleText.Count == 0
            && missingFocusedBorderStyleText.Count == 0
            && missingBorderStyleSetter.Count == 0
            && missingFocusedBorderStyleSetter.Count == 0,
            $"Bordered controls missing style hook parity. Missing BorderStyleText: {string.Join(", ", missingBorderStyleText)}. Missing FocusedBorderStyleText: {string.Join(", ", missingFocusedBorderStyleText)}. Non-settable BorderStyleText: {string.Join(", ", missingBorderStyleSetter)}. Non-settable FocusedBorderStyleText: {string.Join(", ", missingFocusedBorderStyleSetter)}.");

        return Task.CompletedTask;
    }

    private static Task BorderedControls_HaveThemeApplyAndDefaultsExtensions()
    {
        var borderedControls = DiscoverBorderedControls();
        var extensionMethods = typeof(TesseraThemeControlExtensions)
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
            $"Bordered controls missing TesseraTheme extension coverage. Missing ApplyTheme(this TControl, TesseraTheme): {string.Join(", ", missingApplyTheme)}. Missing ApplyThemeDefaults(this TControl, TesseraTheme): {string.Join(", ", missingApplyThemeDefaults)}.");

        return Task.CompletedTask;
    }

    private static Task BorderedControls_HaveThemeOverrideExtensions()
    {
        var borderedControls = DiscoverBorderedControls();
        var extensionMethods = typeof(TesseraThemeControlExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static);
        var missingApplyThemeOverride = new List<string>();
        var missingApplyThemeDefaultsOverride = new List<string>();

        foreach (var controlType in borderedControls)
        {
            if (!HasThemeOverrideMethod(extensionMethods, "ApplyTheme", controlType))
            {
                missingApplyThemeOverride.Add(FormatTypeName(controlType));
            }

            if (!HasThemeOverrideMethod(extensionMethods, "ApplyThemeDefaults", controlType))
            {
                missingApplyThemeDefaultsOverride.Add(FormatTypeName(controlType));
            }
        }

        TestAssert.True(
            missingApplyThemeOverride.Count == 0 && missingApplyThemeDefaultsOverride.Count == 0,
            $"Bordered controls missing override-aware TesseraTheme extension coverage. Missing ApplyTheme(this TControl, TesseraThemeOverrides, TesseraTheme, TesseraThemeVisualState): {string.Join(", ", missingApplyThemeOverride)}. Missing ApplyThemeDefaults(this TControl, TesseraThemeOverrides, TesseraTheme, TesseraThemeVisualState): {string.Join(", ", missingApplyThemeDefaultsOverride)}.");

        return Task.CompletedTask;
    }

    private static Task BorderedControls_ExposePrimaryAndFocusVisualHooks()
    {
        var borderedControls = DiscoverBorderedControls();
        var missingPrimaryVisualStyleHooks = new List<string>();
        var missingFocusedTitleCounterpart = new List<string>();
        var missingFocusVisualHooks = new List<string>();

        foreach (var controlType in borderedControls)
        {
            var teaStyleProperties = controlType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(static property => property.PropertyType == typeof(TesseraStyle))
                .ToArray();
            var nonBorderStyleProperties = teaStyleProperties
                .Where(static property =>
                    !string.Equals(property.Name, "BorderStyleText", StringComparison.Ordinal)
                    && !string.Equals(property.Name, "FocusedBorderStyleText", StringComparison.Ordinal))
                .ToArray();

            if (nonBorderStyleProperties.Length == 0)
            {
                missingPrimaryVisualStyleHooks.Add(FormatTypeName(controlType));
            }

            var titleStyleProperty = teaStyleProperties.FirstOrDefault(
                static property => string.Equals(property.Name, "TitleStyle", StringComparison.Ordinal));
            if (titleStyleProperty is not null
                && teaStyleProperties.All(static property => !string.Equals(property.Name, "FocusedTitleStyle", StringComparison.Ordinal)))
            {
                missingFocusedTitleCounterpart.Add(FormatTypeName(controlType));
            }

            var hasFocusedStyleHook = teaStyleProperties.Any(
                static property => property.Name.Contains("Focused", StringComparison.Ordinal));
            var hasFocusMarkerHook = controlType.GetProperty("FocusMarker", BindingFlags.Instance | BindingFlags.Public)?.PropertyType == typeof(string);
            if (!hasFocusedStyleHook && !hasFocusMarkerHook)
            {
                missingFocusVisualHooks.Add(FormatTypeName(controlType));
            }
        }

        TestAssert.True(
            missingPrimaryVisualStyleHooks.Count == 0
            && missingFocusedTitleCounterpart.Count == 0
            && missingFocusVisualHooks.Count == 0,
            $"Bordered controls missing visual hook parity. Missing primary style hooks (non-border TesseraStyle): {string.Join(", ", missingPrimaryVisualStyleHooks)}. Missing FocusedTitleStyle when TitleStyle is present: {string.Join(", ", missingFocusedTitleCounterpart)}. Missing focus visual hooks (Focused* TesseraStyle or FocusMarker): {string.Join(", ", missingFocusVisualHooks)}.");

        return Task.CompletedTask;
    }

    private static Task BorderedControls_WithFocusMarker_ExposeShowToggleAndSetter()
    {
        var borderedControls = DiscoverBorderedControls();
        var missingShowFocusMarker = new List<string>();
        var nonSettableFocusMarker = new List<string>();
        var nonSettableShowFocusMarker = new List<string>();

        foreach (var controlType in borderedControls)
        {
            var focusMarkerProperty = controlType.GetProperty("FocusMarker", BindingFlags.Instance | BindingFlags.Public);
            if (focusMarkerProperty?.PropertyType != typeof(string))
            {
                continue;
            }

            if (focusMarkerProperty.SetMethod is null || !focusMarkerProperty.SetMethod.IsPublic)
            {
                nonSettableFocusMarker.Add(FormatTypeName(controlType));
            }

            var showFocusMarkerProperty = controlType.GetProperty("ShowFocusMarker", BindingFlags.Instance | BindingFlags.Public);
            if (showFocusMarkerProperty?.PropertyType != typeof(bool))
            {
                missingShowFocusMarker.Add(FormatTypeName(controlType));
                continue;
            }

            if (showFocusMarkerProperty.SetMethod is null || !showFocusMarkerProperty.SetMethod.IsPublic)
            {
                nonSettableShowFocusMarker.Add(FormatTypeName(controlType));
            }
        }

        TestAssert.True(
            missingShowFocusMarker.Count == 0
            && nonSettableFocusMarker.Count == 0
            && nonSettableShowFocusMarker.Count == 0,
            $"Bordered controls with FocusMarker must expose configurable focus marker parity. Missing ShowFocusMarker: {string.Join(", ", missingShowFocusMarker)}. Non-settable FocusMarker: {string.Join(", ", nonSettableFocusMarker)}. Non-settable ShowFocusMarker: {string.Join(", ", nonSettableShowFocusMarker)}.");

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
            if (parameters.Length != 2 || parameters[1].ParameterType != typeof(TesseraTheme))
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

    private static bool HasThemeOverrideMethod(MethodInfo[] extensionMethods, string methodName, Type controlType)
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
            if (parameters.Length != 4
                || parameters[1].ParameterType != typeof(TesseraThemeOverrides)
                || parameters[2].ParameterType != typeof(TesseraTheme)
                || parameters[3].ParameterType != typeof(TesseraThemeVisualState))
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
