using Tessera.Components.Composition;
using Tessera.Components.Primitives;
using Tessera.Components.Styling;
using System.ComponentModel;
using System.Reflection;
using Tessera;
using Tessera.Controls;
using Tessera.Hosting;
using Tessera.Core.Abstractions;
using Tessera.Core.Application;
using Tessera.Core.Input;
using Tessera.Core.Terminal;
using CoreAnsiRendererOptions = Tessera.Core.Rendering.AnsiRendererOptions;

namespace Tessera.Tests;

internal static class RuntimeApiContractTests
{
    private static readonly (string Name, Type Type)[] AdvancedRuntimeTypes =
    [
        ("TesseraHostingOptions", typeof(TesseraHostingOptions)),
        ("BarChartOptions", typeof(Tessera.Controls.BarChartOptions)),
        ("LineChartOptions", typeof(Tessera.Controls.LineChartOptions)),
        ("IProgramRenderer", typeof(Tessera.Hosting.IProgramRenderer)),
        ("RenderOutput", typeof(Tessera.Hosting.RenderOutput)),
        ("NullRenderer", typeof(Tessera.Hosting.NullRenderer)),
        ("AnsiDiffRenderer", typeof(Tessera.Hosting.AnsiDiffRenderer)),
        ("AnsiRendererOptions", typeof(Tessera.Hosting.AnsiRendererOptions)),
        ("ITerminalAdapter", typeof(Tessera.Hosting.ITerminalAdapter)),
        ("TerminalSize", typeof(Tessera.Hosting.TerminalSize)),
        ("TerminalCapabilityProfile", typeof(Tessera.Hosting.TerminalCapabilityProfile)),
        ("TerminalColorProfile", typeof(Tessera.Hosting.TerminalColorProfile)),
        ("ConsoleTerminalAdapter", typeof(Tessera.Hosting.ConsoleTerminalAdapter)),
        ("IEventDecoder", typeof(Tessera.Hosting.IEventDecoder)),
        ("EventDecodeResult", typeof(Tessera.Hosting.EventDecodeResult)),
        ("EventDecoder", typeof(Tessera.Hosting.EventDecoder)),
        ("TerminalCursorStyle", typeof(Tessera.Hosting.TerminalCursorStyle)),
        ("TerminalCapabilityDetector", typeof(Tessera.Hosting.TerminalCapabilityDetector)),
        ("TerminalColorProfileDetector", typeof(Tessera.Hosting.TerminalColorProfileDetector)),
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
            TesseraRuntimeOptions_RemainsDefaultSurface);
        yield return new TestCase(
            "RuntimeApi_TeaStartupSurface_RemainsDefaultDiscovery",
            TesseraStartupSurface_RemainsDefaultDiscovery);
        yield return new TestCase(
            "RuntimeApi_TeaHostingOptions_UsePublicMessageContracts",
            TesseraHostingOptions_UsePublicMessageContracts);
        yield return new TestCase(
            "RuntimeApi_HostingInterfaces_DoNotInheritCoreContracts",
            HostingInterfaces_DoNotInheritCoreContracts);
        yield return new TestCase(
            "RuntimeApi_CoreInputDecoders_AreInternalized",
            CoreInputDecoders_AreInternalized);
        yield return new TestCase(
            "RuntimeApi_CoreTerminalDetectors_AreInternalized",
            CoreTerminalDetectors_AreInternalized);
        yield return new TestCase(
            "RuntimeApi_CoreRendererAndTerminalContracts_AreInternalized",
            CoreRendererAndTerminalContracts_AreInternalized);
        yield return new TestCase(
            "RuntimeApi_TeaRuntimeOptions_DoNotExposeHostingOrInterceptionHooks",
            TesseraRuntimeOptions_DoNotExposeHostingOrInterceptionHooks);
        yield return new TestCase(
            "RuntimeApi_TeaRuntimeOptions_DoNotOwnLegacyProgramTranslation",
            TesseraRuntimeOptions_DoNotOwnLegacyProgramTranslation);
        yield return new TestCase(
            "RuntimeApi_TeaHostApplicationOverloads_AreMarkedAdvanced",
            TesseraHostApplicationOverloads_AreMarkedAdvanced);
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

    private static Task TesseraRuntimeOptions_RemainsDefaultSurface()
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            typeof(TesseraRuntimeOptions),
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is null, "TesseraRuntimeOptions should remain the default discoverable host configuration surface.");
        return Task.CompletedTask;
    }

    private static Task TesseraStartupSurface_RemainsDefaultDiscovery()
    {
        var createBuilder = typeof(TesseraApplication).GetMethod(nameof(TesseraApplication.CreateBuilder), BindingFlags.Public | BindingFlags.Static);
        var createApplication = typeof(TesseraApplication).GetMethod(nameof(TesseraApplication.CreateApplication), BindingFlags.Public | BindingFlags.Static, [typeof(TesseraApp), typeof(TesseraRuntimeOptions)]);
        var runAsync = typeof(TesseraApplication).GetMethod(nameof(TesseraApplication.RunAsync), BindingFlags.Public | BindingFlags.Static, [typeof(TesseraApp), typeof(TesseraRuntimeOptions), typeof(CancellationToken)]);

        TestAssert.True(createBuilder is not null, "TesseraApplication.CreateBuilder should exist.");
        TestAssert.True(createApplication is not null, "TesseraApplication.CreateApplication(app, options) should exist.");
        TestAssert.True(runAsync is not null, "TesseraApplication.RunAsync(app, options, token) should exist.");

        var builderAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(createBuilder!, typeof(EditorBrowsableAttribute));
        var applicationAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(createApplication!, typeof(EditorBrowsableAttribute));
        var runAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(runAsync!, typeof(EditorBrowsableAttribute));
        var legacyFactory = typeof(TesseraApplication).GetMethod("CreateProgram", BindingFlags.Public | BindingFlags.Static);

        TestAssert.True(builderAttribute is null, "TesseraApplication.CreateBuilder should remain discoverable.");
        TestAssert.True(applicationAttribute is null, "TesseraApplication.CreateApplication should remain discoverable.");
        TestAssert.True(runAttribute is null, "TesseraApplication.RunAsync should remain discoverable.");
        TestAssert.True(legacyFactory is null, "TesseraApplication should not expose advanced CreateProgram overloads on the root startup surface.");
        return Task.CompletedTask;
    }

    private static Task TesseraHostFactoryOverloads_AreMarkedAdvanced()
    {
        var overloads = typeof(TesseraHost).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(static method => method.Name == "CreateProgram")
            .ToArray();

        TestAssert.True(overloads.Length == 0, "TesseraHost should no longer expose CreateProgram overloads.");
        return Task.CompletedTask;
    }

    private static Task TesseraHostingOptions_UsePublicMessageContracts()
    {
        var messageFilter = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.MessageFilter));
        var mapEffectException = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.MapEffectException));
        var renderer = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.Renderer));
        var rendererOptions = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.AnsiRendererOptions));
        var terminal = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.Terminal));
        var terminalCapabilities = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.TerminalCapabilities));
        var terminalCapabilityDetector = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.TerminalCapabilityDetector));
        var colorProfile = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.ColorProfile));
        var colorProfileDetector = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.ColorProfileDetector));
        var eventDecoder = typeof(TesseraHostingOptions).GetProperty(nameof(TesseraHostingOptions.EventDecoder));

        TestAssert.True(messageFilter is not null, "TesseraHostingOptions.MessageFilter should exist.");
        TestAssert.True(mapEffectException is not null, "TesseraHostingOptions.MapEffectException should exist.");
        TestAssert.True(renderer is not null, "TesseraHostingOptions.Renderer should exist.");
        TestAssert.True(rendererOptions is not null, "TesseraHostingOptions.AnsiRendererOptions should exist.");
        TestAssert.True(terminal is not null, "TesseraHostingOptions.Terminal should exist.");
        TestAssert.True(terminalCapabilities is not null, "TesseraHostingOptions.TerminalCapabilities should exist.");
        TestAssert.True(terminalCapabilityDetector is not null, "TesseraHostingOptions.TerminalCapabilityDetector should exist.");
        TestAssert.True(colorProfile is not null, "TesseraHostingOptions.ColorProfile should exist.");
        TestAssert.True(colorProfileDetector is not null, "TesseraHostingOptions.ColorProfileDetector should exist.");
        TestAssert.True(eventDecoder is not null, "TesseraHostingOptions.EventDecoder should exist.");
        TestAssert.True(messageFilter!.PropertyType == typeof(Func<TesseraApp, Message, Message>), "TesseraHostingOptions.MessageFilter should use TesseraApp and Message, not core runtime types.");
        TestAssert.True(mapEffectException!.PropertyType == typeof(Func<Exception, Message>), "TesseraHostingOptions.MapEffectException should use public Message contracts.");
        TestAssert.True(renderer!.PropertyType == typeof(Tessera.Hosting.IProgramRenderer), "TesseraHostingOptions.Renderer should use Tessera.Hosting contracts.");
        TestAssert.True(rendererOptions!.PropertyType == typeof(Tessera.Hosting.AnsiRendererOptions), "TesseraHostingOptions.AnsiRendererOptions should use Tessera.Hosting contracts.");
        TestAssert.True(terminal!.PropertyType == typeof(Tessera.Hosting.ITerminalAdapter), "TesseraHostingOptions.Terminal should use Tessera.Hosting contracts.");
        TestAssert.True(terminalCapabilities!.PropertyType == typeof(Tessera.Hosting.TerminalCapabilityProfile), "TesseraHostingOptions.TerminalCapabilities should use Tessera.Hosting capability contracts.");
        TestAssert.True(terminalCapabilityDetector!.PropertyType == typeof(Func<Tessera.Hosting.TerminalCapabilityProfile>), "TesseraHostingOptions.TerminalCapabilityDetector should use Tessera.Hosting capability contracts.");
        TestAssert.True(colorProfile!.PropertyType == typeof(Tessera.Hosting.TerminalColorProfile?), "TesseraHostingOptions.ColorProfile should use Tessera.Hosting color contracts.");
        TestAssert.True(colorProfileDetector!.PropertyType == typeof(Func<Tessera.Hosting.TerminalColorProfile>), "TesseraHostingOptions.ColorProfileDetector should use Tessera.Hosting color contracts.");
        TestAssert.True(eventDecoder!.PropertyType == typeof(Tessera.Hosting.IEventDecoder), "TesseraHostingOptions.EventDecoder should use Tessera.Hosting contracts.");
        return Task.CompletedTask;
    }

    private static Task HostingInterfaces_DoNotInheritCoreContracts()
    {
        TestAssert.True(
            !typeof(Tessera.Hosting.IProgramRenderer).IsAssignableTo(typeof(global::Tessera.Core.Rendering.IProgramRenderer)),
            "Tessera.Hosting.IProgramRenderer should be Tessera-owned, not a core interface alias.");
        TestAssert.True(
            !typeof(Tessera.Hosting.ITerminalAdapter).IsAssignableTo(typeof(global::Tessera.Core.Terminal.ITerminalAdapter)),
            "Tessera.Hosting.ITerminalAdapter should be Tessera-owned, not a core interface alias.");
        TestAssert.True(
            !typeof(Tessera.Hosting.IEventDecoder).IsAssignableTo(typeof(global::Tessera.Core.Input.Decoding.IEventDecoder)),
            "Tessera.Hosting.IEventDecoder should be Tessera-owned, not a core interface alias.");
        return Task.CompletedTask;
    }

    private static Task CoreInputDecoders_AreInternalized()
    {
        TestAssert.True(typeof(global::Tessera.Core.Input.Decoding.EventDecoder).IsNotPublic, "Tessera.Core.Input.Decoding.EventDecoder should be internal.");
        TestAssert.True(typeof(global::Tessera.Core.Input.Decoding.IEventDecoder).IsNotPublic, "Tessera.Core.Input.Decoding.IEventDecoder should be internal.");
        TestAssert.True(typeof(global::Tessera.Core.Input.Decoding.DecodeResult).IsNotPublic, "Tessera.Core.Input.Decoding.DecodeResult should be internal.");
        return Task.CompletedTask;
    }

    private static Task CoreTerminalDetectors_AreInternalized()
    {
        TestAssert.True(typeof(global::Tessera.Core.Terminal.Capabilities.TerminalCapabilityDetector).IsNotPublic, "Tessera.Core.Terminal.Capabilities.TerminalCapabilityDetector should be internal.");
        TestAssert.True(typeof(global::Tessera.Core.Terminal.Capabilities.TerminalColorProfileDetector).IsNotPublic, "Tessera.Core.Terminal.Capabilities.TerminalColorProfileDetector should be internal.");
        return Task.CompletedTask;
    }

    private static Task CoreRendererAndTerminalContracts_AreInternalized()
    {
        TestAssert.True(typeof(global::Tessera.Core.Rendering.IProgramRenderer).IsNotPublic, "Tessera.Core.Rendering.IProgramRenderer should be internal.");
        TestAssert.True(typeof(global::Tessera.Core.Rendering.AnsiDiffRenderer).IsNotPublic, "Tessera.Core.Rendering.AnsiDiffRenderer should be internal.");
        TestAssert.True(typeof(global::Tessera.Core.Rendering.NullRenderer).IsNotPublic, "Tessera.Core.Rendering.NullRenderer should be internal.");
        TestAssert.True(typeof(global::Tessera.Core.Rendering.AnsiRendererOptions).IsNotPublic, "Tessera.Core.Rendering.AnsiRendererOptions should be internal.");
        TestAssert.True(typeof(global::Tessera.Core.Terminal.ITerminalAdapter).IsNotPublic, "Tessera.Core.Terminal.ITerminalAdapter should be internal.");
        TestAssert.True(typeof(global::Tessera.Core.Terminal.Adapters.ConsoleTerminalAdapter).IsNotPublic, "Tessera.Core.Terminal.Adapters.ConsoleTerminalAdapter should be internal.");
        return Task.CompletedTask;
    }

    private static Task TesseraRuntimeOptions_DoNotExposeHostingOrInterceptionHooks()
    {
        string[] removedProperties =
        [
            "MessageFilter",
            "MapEffectException",
            "Hosting",
        ];

        foreach (var propertyName in removedProperties)
        {
            var property = typeof(TesseraRuntimeOptions).GetProperty(propertyName);
            TestAssert.True(property is null, $"TesseraRuntimeOptions should no longer expose {propertyName} directly.");
        }

        return Task.CompletedTask;
    }

    private static Task TesseraRuntimeOptions_DoNotOwnLegacyProgramTranslation()
    {
        var translationMethod = typeof(TesseraRuntimeOptions).GetMethod("ToProgramOptions", BindingFlags.Instance | BindingFlags.NonPublic);

        TestAssert.True(
            translationMethod is null,
            "TesseraRuntimeOptions should not own legacy TesseraRuntimeLoopOptions translation once runtime bridging moves behind the internal runtime seam.");
        return Task.CompletedTask;
    }

    private static Task TesseraHostApplicationOverloads_AreMarkedAdvanced()
    {
        var createApplication = typeof(TesseraHost).GetMethod(
            nameof(TesseraHost.CreateApplication),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(TesseraApp), typeof(TesseraRuntimeOptions), typeof(TesseraHostingOptions)]);
        var runAsync = typeof(TesseraHost).GetMethod(
            nameof(TesseraHost.RunAsync),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(TesseraApp), typeof(TesseraRuntimeOptions), typeof(TesseraHostingOptions), typeof(CancellationToken)]);

        TestAssert.True(createApplication is not null, "TesseraHost.CreateApplication should exist for advanced hosting.");
        TestAssert.True(runAsync is not null, "TesseraHost.RunAsync should exist for advanced hosting.");

        var createAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(createApplication!, typeof(EditorBrowsableAttribute));
        var runAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(runAsync!, typeof(EditorBrowsableAttribute));

        TestAssert.True(createAttribute is not null, "TesseraHost.CreateApplication should be marked advanced.");
        TestAssert.True(createAttribute!.State == EditorBrowsableState.Advanced, "TesseraHost.CreateApplication should stay out of default discovery.");
        TestAssert.True(runAttribute is not null, "TesseraHost.RunAsync should be marked advanced.");
        TestAssert.True(runAttribute!.State == EditorBrowsableState.Advanced, "TesseraHost.RunAsync should stay out of default discovery.");
        return Task.CompletedTask;
    }

    private static Task LegacyChartingHelpers_AreRemoved()
    {
        string[] typeNames =
        [
            "Tessera.Components.Charting.Charts",
            "Tessera.Components.Charting.BarChartComponent",
            "Tessera.Components.Charting.BarDatum",
            "Tessera.Components.Charting.LineChartComponent",
            "Tessera.Components.Primitives.Widgets",
        ];

        var assembly = typeof(TesseraApplication).Assembly;
        foreach (var typeName in typeNames)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            if (typeName == "Tessera.Components.Primitives.Widgets")
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
            "Tessera.Components.Dashboard.GaugeComponent",
            "Tessera.Components.Dashboard.MiniLogComponent",
            "Tessera.Components.Dashboard.StatsCardComponent",
            "Tessera.Components.Dashboard.StatsCardItem",
        ];

        var assembly = typeof(TesseraApplication).Assembly;
        foreach (var typeName in typeNames)
        {
            var type = assembly.GetType(typeName, throwOnError: false);
            TestAssert.True(type is null, $"{typeName} should be removed once the root wrapper owns the implementation directly.");
        }

        return Task.CompletedTask;
    }

    private static Task LegacyProgramHostingSurface_IsInternalized()
    {
        var teaSharpAssembly = typeof(TesseraApplication).Assembly;
        var teaProgramOptions = teaSharpAssembly.GetType("Tessera.Hosting.TesseraProgramOptions", throwOnError: false);
        var iscreen = typeof(TesseraRuntimeLoopOptions).Assembly.GetType("Tessera.Core.Abstractions.IScreen", throwOnError: false);

        TestAssert.True(teaProgramOptions is null, "TesseraProgramOptions should no longer exist on the supported hosting surface.");
        TestAssert.True(iscreen is null, "IScreen should be removed once TesseraRuntimeLoop owns the runtime delegates directly.");
        var coreAssembly = typeof(TesseraRuntimeLoopOptions).Assembly;
        var teaProgram = coreAssembly.GetType("Tessera.Core.Application.TesseraProgram", throwOnError: false);

        TestAssert.True(typeof(TesseraRuntimeLoopOptions).IsNotPublic, "TesseraRuntimeLoopOptions should be an internal runtime bridge.");
        TestAssert.True(teaProgram is null, "TesseraProgram should be removed once TesseraRuntimeLoop owns the runtime loop.");
        return Task.CompletedTask;
    }

    private static Task DefaultSpacingAndBorderTypes_LiveAtRootNamespace()
    {
        TestAssert.True(typeof(BorderStyle).Namespace == "Tessera", "BorderStyle should live at the Tessera root namespace for default app code.");
        TestAssert.True(typeof(Thickness).Namespace == "Tessera", "Thickness should live at the Tessera root namespace for default app code.");
        return Task.CompletedTask;
    }

}
