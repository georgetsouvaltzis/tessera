using TeaSharp.Components.Composition;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using System.ComponentModel;
using System.Reflection;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Hosting;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Input;
using TeaSharp.Core.Rendering;
using TeaSharp.Core.Terminal;

namespace TeaSharp.Tests;

internal static class RuntimeApiContractTests
{
    private static readonly (string Name, Type Type)[] AdvancedRuntimeTypes =
    [
        ("TeaHostingOptions", typeof(TeaHostingOptions)),
        ("BarChartOptions", typeof(TeaSharp.Controls.BarChartOptions)),
        ("LineChartOptions", typeof(TeaSharp.Controls.LineChartOptions)),
        ("IProgramRenderer", typeof(TeaSharp.Hosting.IProgramRenderer)),
        ("NullRenderer", typeof(TeaSharp.Hosting.NullRenderer)),
        ("AnsiDiffRenderer", typeof(TeaSharp.Hosting.AnsiDiffRenderer)),
        ("AnsiRendererOptions", typeof(AnsiRendererOptions)),
        ("ITerminalAdapter", typeof(TeaSharp.Hosting.ITerminalAdapter)),
        ("ConsoleTerminalAdapter", typeof(TeaSharp.Hosting.ConsoleTerminalAdapter)),
        ("IEventDecoder", typeof(TeaSharp.Hosting.IEventDecoder)),
        ("EventDecoder", typeof(TeaSharp.Hosting.EventDecoder)),
        ("TerminalReader", typeof(TerminalReader)),
        ("TerminalCapabilityDetector", typeof(TerminalCapabilityDetector)),
        ("TerminalColorProfileDetector", typeof(TerminalColorProfileDetector)),
        ("TerminalCapabilityProfile", typeof(TerminalCapabilityProfile)),
    ];

    public static IEnumerable<TestCase> Cases()
    {
        foreach (var (name, type) in AdvancedRuntimeTypes)
        {
            yield return new TestCase(
                $"RuntimeApi_{name}_IsMarkedAdvanced",
                () => AssertMarkedAdvanced(type));
        }

        yield return new TestCase(
            "RuntimeApi_TeaRuntimeOptions_RemainsDefaultSurface",
            TeaRuntimeOptions_RemainsDefaultSurface);
        yield return new TestCase(
            "RuntimeApi_TeaStartupSurface_RemainsDefaultDiscovery",
            TeaStartupSurface_RemainsDefaultDiscovery);
        yield return new TestCase(
            "RuntimeApi_TeaHostingOptions_UsePublicMessageContracts",
            TeaHostingOptions_UsePublicMessageContracts);
        yield return new TestCase(
            "RuntimeApi_TeaRuntimeOptions_DoNotExposeHostingOrInterceptionHooks",
            TeaRuntimeOptions_DoNotExposeHostingOrInterceptionHooks);
        yield return new TestCase(
            "RuntimeApi_TeaRuntimeOptions_DoNotOwnLegacyProgramTranslation",
            TeaRuntimeOptions_DoNotOwnLegacyProgramTranslation);
        yield return new TestCase(
            "RuntimeApi_TeaHostApplicationOverloads_AreMarkedAdvanced",
            TeaHostApplicationOverloads_AreMarkedAdvanced);
        yield return new TestCase(
            "RuntimeApi_DefaultSpacingAndBorderTypes_LiveAtRootNamespace",
            DefaultSpacingAndBorderTypes_LiveAtRootNamespace);
        yield return new TestCase(
            "RuntimeApi_LegacyChartingHelpers_AreRemoved",
            LegacyChartingHelpers_AreRemoved);
        yield return new TestCase(
            "RuntimeApi_LegacyDashboardHelpers_AreRemoved",
            LegacyDashboardHelpers_AreRemoved);
        yield return new TestCase(
            "RuntimeApi_LegacyProgramHostingSurface_IsInternalized",
            LegacyProgramHostingSurface_IsInternalized);
    }

    private static Task AssertMarkedAdvanced(Type type)
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            type,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is not null, $"{type.Name} should be explicitly marked as an advanced runtime seam.");
        TestAssert.True(
            attribute!.State == EditorBrowsableState.Advanced,
            $"{type.Name} should be hidden from default API discovery.");
        return Task.CompletedTask;
    }

    private static Task TeaRuntimeOptions_RemainsDefaultSurface()
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            typeof(TeaRuntimeOptions),
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is null, "TeaRuntimeOptions should remain the default discoverable host configuration surface.");
        return Task.CompletedTask;
    }

    private static Task TeaStartupSurface_RemainsDefaultDiscovery()
    {
        var createBuilder = typeof(Tea).GetMethod(nameof(Tea.CreateBuilder), BindingFlags.Public | BindingFlags.Static);
        var createApplication = typeof(Tea).GetMethod(nameof(Tea.CreateApplication), BindingFlags.Public | BindingFlags.Static, [typeof(TeaApp), typeof(TeaRuntimeOptions)]);
        var runAsync = typeof(Tea).GetMethod(nameof(Tea.RunAsync), BindingFlags.Public | BindingFlags.Static, [typeof(TeaApp), typeof(TeaRuntimeOptions), typeof(CancellationToken)]);

        TestAssert.True(createBuilder is not null, "Tea.CreateBuilder should exist.");
        TestAssert.True(createApplication is not null, "Tea.CreateApplication(app, options) should exist.");
        TestAssert.True(runAsync is not null, "Tea.RunAsync(app, options, token) should exist.");

        var builderAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(createBuilder!, typeof(EditorBrowsableAttribute));
        var applicationAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(createApplication!, typeof(EditorBrowsableAttribute));
        var runAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(runAsync!, typeof(EditorBrowsableAttribute));
        var legacyFactory = typeof(Tea).GetMethod("CreateProgram", BindingFlags.Public | BindingFlags.Static);

        TestAssert.True(builderAttribute is null, "Tea.CreateBuilder should remain discoverable.");
        TestAssert.True(applicationAttribute is null, "Tea.CreateApplication should remain discoverable.");
        TestAssert.True(runAttribute is null, "Tea.RunAsync should remain discoverable.");
        TestAssert.True(legacyFactory is null, "Tea should not expose advanced CreateProgram overloads on the root startup surface.");
        return Task.CompletedTask;
    }

    private static Task TeaHostFactoryOverloads_AreMarkedAdvanced()
    {
        var overloads = typeof(TeaHost).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(static method => method.Name == "CreateProgram")
            .ToArray();

        TestAssert.True(overloads.Length == 0, "TeaHost should no longer expose CreateProgram overloads.");
        return Task.CompletedTask;
    }

    private static Task TeaHostingOptions_UsePublicMessageContracts()
    {
        var messageFilter = typeof(TeaHostingOptions).GetProperty(nameof(TeaHostingOptions.MessageFilter));
        var mapEffectException = typeof(TeaHostingOptions).GetProperty(nameof(TeaHostingOptions.MapEffectException));
        var renderer = typeof(TeaHostingOptions).GetProperty(nameof(TeaHostingOptions.Renderer));
        var terminal = typeof(TeaHostingOptions).GetProperty(nameof(TeaHostingOptions.Terminal));
        var eventDecoder = typeof(TeaHostingOptions).GetProperty(nameof(TeaHostingOptions.EventDecoder));

        TestAssert.True(messageFilter is not null, "TeaHostingOptions.MessageFilter should exist.");
        TestAssert.True(mapEffectException is not null, "TeaHostingOptions.MapEffectException should exist.");
        TestAssert.True(renderer is not null, "TeaHostingOptions.Renderer should exist.");
        TestAssert.True(terminal is not null, "TeaHostingOptions.Terminal should exist.");
        TestAssert.True(eventDecoder is not null, "TeaHostingOptions.EventDecoder should exist.");
        TestAssert.True(messageFilter!.PropertyType == typeof(Func<TeaApp, Message, Message>), "TeaHostingOptions.MessageFilter should use TeaApp and Message, not core runtime types.");
        TestAssert.True(mapEffectException!.PropertyType == typeof(Func<Exception, Message>), "TeaHostingOptions.MapEffectException should use public Message contracts.");
        TestAssert.True(renderer!.PropertyType == typeof(TeaSharp.Hosting.IProgramRenderer), "TeaHostingOptions.Renderer should use TeaSharp.Hosting contracts.");
        TestAssert.True(terminal!.PropertyType == typeof(TeaSharp.Hosting.ITerminalAdapter), "TeaHostingOptions.Terminal should use TeaSharp.Hosting contracts.");
        TestAssert.True(eventDecoder!.PropertyType == typeof(TeaSharp.Hosting.IEventDecoder), "TeaHostingOptions.EventDecoder should use TeaSharp.Hosting contracts.");
        return Task.CompletedTask;
    }

    private static Task TeaRuntimeOptions_DoNotExposeHostingOrInterceptionHooks()
    {
        string[] removedProperties =
        [
            "MessageFilter",
            "MapEffectException",
            "Hosting",
        ];

        foreach (var propertyName in removedProperties)
        {
            var property = typeof(TeaRuntimeOptions).GetProperty(propertyName);
            TestAssert.True(property is null, $"TeaRuntimeOptions should no longer expose {propertyName} directly.");
        }

        return Task.CompletedTask;
    }

    private static Task TeaRuntimeOptions_DoNotOwnLegacyProgramTranslation()
    {
        var translationMethod = typeof(TeaRuntimeOptions).GetMethod("ToProgramOptions", BindingFlags.Instance | BindingFlags.NonPublic);

        TestAssert.True(
            translationMethod is null,
            "TeaRuntimeOptions should not own legacy TeaRuntimeLoopOptions translation once runtime bridging moves behind the internal runtime seam.");
        return Task.CompletedTask;
    }

    private static Task TeaHostApplicationOverloads_AreMarkedAdvanced()
    {
        var createApplication = typeof(TeaHost).GetMethod(
            nameof(TeaHost.CreateApplication),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(TeaApp), typeof(TeaRuntimeOptions), typeof(TeaHostingOptions)]);
        var runAsync = typeof(TeaHost).GetMethod(
            nameof(TeaHost.RunAsync),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(TeaApp), typeof(TeaRuntimeOptions), typeof(TeaHostingOptions), typeof(CancellationToken)]);

        TestAssert.True(createApplication is not null, "TeaHost.CreateApplication should exist for advanced hosting.");
        TestAssert.True(runAsync is not null, "TeaHost.RunAsync should exist for advanced hosting.");

        var createAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(createApplication!, typeof(EditorBrowsableAttribute));
        var runAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(runAsync!, typeof(EditorBrowsableAttribute));

        TestAssert.True(createAttribute is not null, "TeaHost.CreateApplication should be marked advanced.");
        TestAssert.True(createAttribute!.State == EditorBrowsableState.Advanced, "TeaHost.CreateApplication should stay out of default discovery.");
        TestAssert.True(runAttribute is not null, "TeaHost.RunAsync should be marked advanced.");
        TestAssert.True(runAttribute!.State == EditorBrowsableState.Advanced, "TeaHost.RunAsync should stay out of default discovery.");
        return Task.CompletedTask;
    }

    private static Task LegacyChartingHelpers_AreRemoved()
    {
        string[] typeNames =
        [
            "TeaSharp.Components.Charting.Charts",
            "TeaSharp.Components.Charting.BarChartComponent",
            "TeaSharp.Components.Charting.BarDatum",
            "TeaSharp.Components.Charting.LineChartComponent",
            "TeaSharp.Components.Primitives.Widgets",
        ];

        var assembly = typeof(Tea).Assembly;
        foreach (var typeName in typeNames)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            if (typeName == "TeaSharp.Components.Primitives.Widgets")
            {
                TestAssert.True(type is not null, $"{typeName} should continue to exist as an internal bridge.");
                TestAssert.True(type!.IsNotPublic, $"{typeName} should no longer be public once a root wrapper exists.");
                continue;
            }

            TestAssert.True(type is null, $"{typeName} should be removed once the root wrapper owns the implementation directly.");
        }

        return Task.CompletedTask;
    }

    private static Task LegacyDashboardHelpers_AreRemoved()
    {
        string[] typeNames =
        [
            "TeaSharp.Components.Dashboard.GaugeComponent",
            "TeaSharp.Components.Dashboard.MiniLogComponent",
            "TeaSharp.Components.Dashboard.StatsCardComponent",
            "TeaSharp.Components.Dashboard.StatsCardItem",
        ];

        var assembly = typeof(Tea).Assembly;
        foreach (var typeName in typeNames)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is null, $"{typeName} should be removed once the root wrapper owns the implementation directly.");
        }

        return Task.CompletedTask;
    }

    private static Task LegacyProgramHostingSurface_IsInternalized()
    {
        var teaSharpAssembly = typeof(Tea).Assembly;
        var teaProgramOptions = teaSharpAssembly.GetType("TeaSharp.Hosting.TeaProgramOptions", throwOnError: false);
        var iscreen = typeof(TeaRuntimeLoopOptions).Assembly.GetType("TeaSharp.Core.Abstractions.IScreen", throwOnError: false);

        TestAssert.True(teaProgramOptions is null, "TeaProgramOptions should no longer exist on the supported hosting surface.");
        TestAssert.True(iscreen is null, "IScreen should be removed once TeaRuntimeLoop owns the runtime delegates directly.");
        var coreAssembly = typeof(TeaRuntimeLoopOptions).Assembly;
        var teaProgram = coreAssembly.GetType("TeaSharp.Core.Application.TeaProgram", throwOnError: false);

        TestAssert.True(typeof(TeaRuntimeLoopOptions).IsNotPublic, "TeaRuntimeLoopOptions should be an internal runtime bridge.");
        TestAssert.True(teaProgram is null, "TeaProgram should be removed once TeaRuntimeLoop owns the runtime loop.");
        return Task.CompletedTask;
    }

    private static Task DefaultSpacingAndBorderTypes_LiveAtRootNamespace()
    {
        TestAssert.True(typeof(BorderStyle).Namespace == "TeaSharp", "BorderStyle should live at the TeaSharp root namespace for default app code.");
        TestAssert.True(typeof(Thickness).Namespace == "TeaSharp", "Thickness should live at the TeaSharp root namespace for default app code.");
        return Task.CompletedTask;
    }

}
