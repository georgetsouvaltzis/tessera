using NUnit.Framework;
using System.Reflection;
using System.Runtime.CompilerServices;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ThemeFocusMarkerParityPolicyTests
{
    [Test]
    public void ThemeFocusMarkerApplyThemeMapsMarkerForPolicyControls()
    {
        var theme = new TesseraTheme { Focus = new TesseraThemeFocusTokens { Marker = ">>" } };

        foreach (var policy in ResolvePolicies())
        {
            var control = CreateControl(policy.ControlType);
            policy.FocusMarker.SetValue(control, "initial");
            policy.ApplyTheme.Invoke(null, [control, theme]);
            var marker = (string?)policy.FocusMarker.GetValue(control) ?? string.Empty;
            TestAssert.Equal(
                theme.Focus.Marker,
                marker,
                $"{policy.ControlType.Name} ApplyTheme should map Focus.Marker.");
        }
    }

    [Test]
    public void ThemeFocusMarkerApplyThemeDefaultsFillsEmptyAndPreservesExplicitForPolicyControls()
    {
        var theme = new TesseraTheme { Focus = new TesseraThemeFocusTokens { Marker = "::" } };

        foreach (var policy in ResolvePolicies())
        {
            var emptyControl = CreateControl(policy.ControlType);
            policy.FocusMarker.SetValue(emptyControl, string.Empty);
            policy.ApplyThemeDefaults.Invoke(null, [emptyControl, theme]);
            var filled = (string?)policy.FocusMarker.GetValue(emptyControl) ?? string.Empty;

            var explicitControl = CreateControl(policy.ControlType);
            policy.FocusMarker.SetValue(explicitControl, "!");
            policy.ApplyThemeDefaults.Invoke(null, [explicitControl, theme]);
            var preserved = (string?)policy.FocusMarker.GetValue(explicitControl) ?? string.Empty;

            TestAssert.Equal(
                theme.Focus.Marker,
                filled,
                $"{policy.ControlType.Name} ApplyThemeDefaults should fill empty FocusMarker.");
            TestAssert.Equal(
                "!",
                preserved,
                $"{policy.ControlType.Name} ApplyThemeDefaults should preserve non-empty FocusMarker.");
        }
    }

    private static List<MarkerPolicy> ResolvePolicies()
    {
        var extensionMethods = typeof(TesseraThemeControlExtensions).Assembly
            .GetTypes()
            .Where(static type => type.IsSealed && type.IsAbstract)
            .SelectMany(static type => type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(static method => method.IsDefined(typeof(ExtensionAttribute), false))
            .ToArray();
        var policies = new List<MarkerPolicy>(PolicyControlTypes.Length);

        for (var index = 0; index < PolicyControlTypes.Length; index++)
        {
            var controlType = PolicyControlTypes[index];
            var focusMarker =
                controlType.GetProperty(nameof(Choice.FocusMarker), BindingFlags.Public | BindingFlags.Instance);
            if (focusMarker is null || focusMarker.PropertyType != typeof(string) || !focusMarker.CanWrite)
            {
                continue;
            }

            var applyTheme = FindExtensionMethod(extensionMethods, "ApplyTheme", controlType);
            var applyThemeDefaults = FindExtensionMethod(extensionMethods, "ApplyThemeDefaults", controlType);
            if (applyTheme is null || applyThemeDefaults is null)
            {
                continue;
            }

            policies.Add(
                new MarkerPolicy(
                    controlType,
                    focusMarker,
                    applyTheme,
                    applyThemeDefaults));
        }

        TestAssert.Equal(
            PolicyControlTypes.Length,
            policies.Count,
            "Policy controls must expose FocusMarker and both ApplyTheme + ApplyThemeDefaults extensions.");
        return policies;
    }

    private static MethodInfo? FindExtensionMethod(MethodInfo[] methods, string methodName, Type controlType)
    {
        for (var index = 0; index < methods.Length; index++)
        {
            var method = methods[index];
            if (!string.Equals(method.Name, methodName, StringComparison.Ordinal))
            {
                continue;
            }

            var parameters = method.GetParameters();
            if (parameters.Length == 2
                && parameters[0].ParameterType == controlType
                && parameters[1].ParameterType == typeof(TesseraTheme))
            {
                return method;
            }
        }

        return null;
    }

    private static object CreateControl(Type controlType)
    {
        var instance = Activator.CreateInstance(controlType);
        return instance ?? throw new InvalidOperationException($"Unable to create {controlType.Name}.");
    }

    private static readonly Type[] PolicyControlTypes =
    [
        typeof(Choice),
        typeof(ComboBox),
        typeof(TreeView)
    ];

    private sealed record MarkerPolicy(
        Type ControlType,
        PropertyInfo FocusMarker,
        MethodInfo ApplyTheme,
        MethodInfo ApplyThemeDefaults);
}
