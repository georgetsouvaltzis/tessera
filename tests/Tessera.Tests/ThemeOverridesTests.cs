using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

internal static partial class ThemeOverridesTests
{
    public static IEnumerable<TestCase> Cases()
    {
        foreach (var testCase in FoundationCases())
        {
            yield return testCase;
        }

        foreach (var testCase in NavigationCases())
        {
            yield return testCase;
        }

        foreach (var testCase in NavigationOverlayCases())
        {
            yield return testCase;
        }

        foreach (var testCase in RenderingCases())
        {
            yield return testCase;
        }

        foreach (var testCase in DataCases())
        {
            yield return testCase;
        }

        foreach (var testCase in FlowCases())
        {
            yield return testCase;
        }

        foreach (var testCase in InputValueCases())
        {
            yield return testCase;
        }
    }

    private static TesseraTheme BuildThemeWithPrimary(byte red, byte green, byte blue)
    {
        return new TesseraTheme
        {
            Text = new TesseraThemeTextTokens
            {
                Primary = TesseraStyle.Empty.WithForeground(AnsiColor.Rgb(red, green, blue)),
            },
        };
    }
}
