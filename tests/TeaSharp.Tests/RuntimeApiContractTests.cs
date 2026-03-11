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
            "RuntimeApi_TeaProgramOptions_RemainsDefaultSurface",
            TeaProgramOptions_RemainsDefaultSurface);
        yield return new TestCase(
            "RuntimeApi_TeaProgramFactoryOverloads_FavorStableSurface",
            TeaProgramFactoryOverloads_FavorStableSurface);
        yield return new TestCase(
            "RuntimeApi_TeaProgramFactory_DefaultOverload_RemainsStableSurface",
            TeaProgramFactory_DefaultOverload_RemainsStableSurface);
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
        var defaultOverload = typeof(Tea).GetMethod(
            nameof(Tea.NewProgram),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(IModel)]);
        var advancedOverload = typeof(Tea).GetMethod(
            nameof(Tea.NewProgram),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(IModel), typeof(ProgramOptions)]);
        var stableOverload = typeof(Tea).GetMethod(
            nameof(Tea.NewProgram),
            BindingFlags.Public | BindingFlags.Static,
            [typeof(IModel), typeof(TeaProgramOptions)]);

        TestAssert.True(defaultOverload is not null, "The zero-config Tea.NewProgram overload should exist.");
        TestAssert.True(advancedOverload is not null, "The advanced Tea.NewProgram overload should exist.");
        TestAssert.True(stableOverload is not null, "The stable Tea.NewProgram overload should exist.");

        var defaultAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            defaultOverload!,
            typeof(EditorBrowsableAttribute));
        var advancedAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            advancedOverload!,
            typeof(EditorBrowsableAttribute));
        var stableAttribute = (EditorBrowsableAttribute?)Attribute.GetCustomAttribute(
            stableOverload!,
            typeof(EditorBrowsableAttribute));

        TestAssert.True(defaultAttribute is null, "The zero-config Tea.NewProgram overload should remain the default discoverable factory.");
        TestAssert.True(advancedAttribute is not null, "The ProgramOptions overload should be explicitly marked as advanced.");
        TestAssert.True(
            advancedAttribute!.State == EditorBrowsableState.Advanced,
            "The ProgramOptions overload should be hidden from default discovery.");
        TestAssert.True(stableAttribute is null, "The TeaProgramOptions overload should remain the default discoverable factory.");
        return Task.CompletedTask;
    }

    private static Task TeaProgramFactory_DefaultOverload_RemainsStableSurface()
    {
        var program = Tea.NewProgram(new NoOpModel());

        TestAssert.True(program is not null, "Tea.NewProgram(model) should create a program using stable host defaults.");
        return Task.CompletedTask;
    }

    private static Task TeaProgramConstructor_IsMarkedAdvanced()
    {
        var constructor = typeof(TeaProgram).GetConstructor([typeof(IModel), typeof(ProgramOptions)]);

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

    private sealed class NoOpModel : IModel
    {
        public Command? Init() => null;

        public Command? Update(IMessage message) => null;

        public View View() => TeaSharp.Core.Abstractions.View.From(string.Empty);
    }
}
