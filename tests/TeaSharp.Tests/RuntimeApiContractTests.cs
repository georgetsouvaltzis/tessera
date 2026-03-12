using TeaSharp.Components.Advanced;
using TeaSharp.Components.Charting;
using TeaSharp.Components.Composition;
using TeaSharp.Components.Dashboard;
using TeaSharp.Components.Interaction;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Productivity;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using System.ComponentModel;
using System.Reflection;
using TeaSharp;
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
        ("IProgramRenderer", typeof(IProgramRenderer)),
        ("NullRenderer", typeof(NullRenderer)),
        ("AnsiDiffRenderer", typeof(AnsiDiffRenderer)),
        ("AnsiRendererOptions", typeof(AnsiRendererOptions)),
        ("ITerminalAdapter", typeof(ITerminalAdapter)),
        ("ConsoleTerminalAdapter", typeof(ConsoleTerminalAdapter)),
        ("IEventDecoder", typeof(IEventDecoder)),
        ("EventDecoder", typeof(EventDecoder)),
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
            "RuntimeApi_TeaProgramOptions_IsMarkedAdvanced",
            TeaProgramOptions_IsMarkedAdvanced);
        yield return new TestCase(
            "RuntimeApi_TeaStartupSurface_RemainsDefaultDiscovery",
            TeaStartupSurface_RemainsDefaultDiscovery);
        yield return new TestCase(
            "RuntimeApi_TeaProgramFactoryOverloads_AreMarkedAdvanced",
            TeaProgramFactoryOverloads_AreMarkedAdvanced);
        yield return new TestCase(
            "RuntimeApi_TeaProgramConstructor_IsMarkedAdvanced",
            TeaProgramConstructor_IsMarkedAdvanced);
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

    private static Task TeaProgramOptions_IsMarkedAdvanced()
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            typeof(TeaProgramOptions),
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is not null, "TeaProgramOptions should be explicitly marked as legacy program-hosting options.");
        TestAssert.True(
            attribute!.State == EditorBrowsableState.Advanced,
            "TeaProgramOptions should be hidden from default discovery.");
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

        TestAssert.True(builderAttribute is null, "Tea.CreateBuilder should remain discoverable.");
        TestAssert.True(applicationAttribute is null, "Tea.CreateApplication should remain discoverable.");
        TestAssert.True(runAttribute is null, "Tea.RunAsync should remain discoverable.");
        return Task.CompletedTask;
    }

    private static Task TeaProgramFactoryOverloads_AreMarkedAdvanced()
    {
        var defaultOverload = typeof(Tea).GetMethod(
            nameof(Tea.CreateProgram),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(IScreen)]);
        var advancedOverload = typeof(Tea).GetMethod(
            nameof(Tea.CreateProgram),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(IScreen), typeof(ProgramOptions)]);
        var stableOverload = typeof(Tea).GetMethod(
            nameof(Tea.CreateProgram),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(IScreen), typeof(TeaProgramOptions)]);

        TestAssert.True(defaultOverload is not null, "The zero-config Tea.CreateProgram overload should exist.");
        TestAssert.True(advancedOverload is not null, "The advanced Tea.CreateProgram overload should exist.");
        TestAssert.True(stableOverload is not null, "The stable Tea.CreateProgram overload should exist.");

        var defaultAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            defaultOverload!,
            typeof(EditorBrowsableAttribute));
        var advancedAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            advancedOverload!,
            typeof(EditorBrowsableAttribute));
        var stableAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            stableOverload!,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(defaultAttribute is not null, "The zero-config Tea.CreateProgram overload should be explicitly marked as advanced.");
        TestAssert.True(
            defaultAttribute!.State == EditorBrowsableState.Advanced,
            "The zero-config Tea.CreateProgram overload should be hidden from default discovery.");
        TestAssert.True(advancedAttribute is not null, "The ProgramOptions overload should be explicitly marked as advanced.");
        TestAssert.True(
            advancedAttribute!.State == EditorBrowsableState.Advanced,
            "The ProgramOptions overload should be hidden from default discovery.");
        TestAssert.True(stableAttribute is not null, "The TeaProgramOptions overload should be explicitly marked as advanced.");
        TestAssert.True(
            stableAttribute!.State == EditorBrowsableState.Advanced,
            "The TeaProgramOptions overload should be hidden from default discovery.");
        return Task.CompletedTask;
    }

    private static Task TeaProgramConstructor_IsMarkedAdvanced()
    {
        var constructor = typeof(TeaProgram).GetConstructor([typeof(IScreen), typeof(ProgramOptions)]);

        TestAssert.True(constructor is not null, "TeaProgram advanced constructor should exist.");

        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            constructor!,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is not null, "TeaProgram constructor should be marked as an advanced host seam.");
        TestAssert.True(
            attribute!.State == EditorBrowsableState.Advanced,
            "TeaProgram constructor should be hidden from default discovery.");
        return Task.CompletedTask;
    }

    private sealed class NoOpModel : IScreen
    {
        public Effect? Init() => null;

        public Effect? Update(IMessage message) => null;

        public ScreenOutput Render() => TeaSharp.Core.Abstractions.ScreenOutput.From(string.Empty);
    }
}
