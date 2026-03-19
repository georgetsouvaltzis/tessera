using TeaSharp.Components.Composition;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using NUnit.Framework;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class NUnitCaseAdapters
{
    public static IEnumerable<TestCaseData> Cases()
    {
        foreach (var testCase in RuntimeLoopTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in EventDecoderGoldenTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in TerminalReaderBehaviorTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in RendererBehaviorTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in RendererSnapshotTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in StyleRenderingTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in WidgetStateTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in ChartComponentTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in DashboardComponentTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in ProtocolFixtureTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in TerminalCapabilityDetectorTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in ComponentRenderingTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in CompositionApiContractTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in RuntimeApiContractTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in PublicApiXmlDocsTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in PublicApiBoundaryTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in ThemeFoundationTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in TeaControlCatalogTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in TeaAppCompositionTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in TeaAppFoundationTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in TeaApplicationBuilderContractTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in WidgetApiContractTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in UiKitComponentTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in PrebuiltWidgetTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in WidgetStatePaletteTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in AdvancedPrebuiltWidgetTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in KeyBindingTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in ProductivityPrebuiltWidgetTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in ApiErgonomicsTests.Cases()) yield return ToCaseData(testCase);
        foreach (var testCase in ConsoleTerminalAdapterHelperTests.Cases()) yield return ToCaseData(testCase);
    }

    [TestCaseSource(nameof(Cases))]
    public Task Execute(global::TeaSharp.Tests.TestCase testCase)
    {
        return testCase.Execute();
    }

    private static TestCaseData ToCaseData(global::TeaSharp.Tests.TestCase testCase)
    {
        return new TestCaseData(testCase).SetName(testCase.Name);
    }
}
