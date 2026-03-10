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
            "RuntimeApi_TeaProgramOptions_RemainsDefaultSurface",
            TeaProgramOptions_RemainsDefaultSurface);
        yield return new TestCase(
            "RuntimeApi_TeaProgramFactoryOverloads_FavorStableSurface",
            TeaProgramFactoryOverloads_FavorStableSurface);
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

    private static Task TeaProgramOptions_RemainsDefaultSurface()
    {
        var attribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            typeof(TeaProgramOptions),
            typeof(EditorBrowsableAttribute));

        TestAssert.True(attribute is null, "TeaProgramOptions should remain the default discoverable host configuration surface.");
        return Task.CompletedTask;
    }

    private static Task TeaProgramFactoryOverloads_FavorStableSurface()
    {
        var advancedOverload = typeof(Tea).GetMethod(
            nameof(Tea.NewProgram),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(IModel), typeof(ProgramOptions)]);
        var stableOverload = typeof(Tea).GetMethod(
            nameof(Tea.NewProgram),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(IModel), typeof(TeaProgramOptions)]);

        TestAssert.True(advancedOverload is not null, "The advanced Tea.NewProgram overload should exist.");
        TestAssert.True(stableOverload is not null, "The stable Tea.NewProgram overload should exist.");

        var advancedAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            advancedOverload!,
            typeof(EditorBrowsableAttribute));
        var stableAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            stableOverload!,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(advancedAttribute is not null, "The ProgramOptions overload should be explicitly marked as advanced.");
        TestAssert.True(
            advancedAttribute!.State == EditorBrowsableState.Advanced,
            "The ProgramOptions overload should be hidden from default discovery.");
        TestAssert.True(stableAttribute is null, "The TeaProgramOptions overload should remain the default discoverable factory.");
        return Task.CompletedTask;
    }
}
