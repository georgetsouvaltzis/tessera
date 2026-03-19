using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

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
    }

    private static TeaTheme BuildThemeWithPrimary(byte red, byte green, byte blue)
    {
        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(red, green, blue)),
            },
        };
    }
}
