using System.ComponentModel;
using TeaSharp;
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
}
