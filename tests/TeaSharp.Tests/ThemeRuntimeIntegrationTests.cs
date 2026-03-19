using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

internal static class ThemeRuntimeIntegrationTests
{
    public static IEnumerable<TestCase> Cases()
    {
        yield return new TestCase(
            "ThemeRuntime_RuntimeOptionsTheme_PropagatesToContext",
            RuntimeOptionsTheme_PropagatesToContext);
        yield return new TestCase(
            "ThemeRuntime_SceneCompilation_AppliesThemeDefaultsForStyleableControls",
            SceneCompilation_AppliesThemeDefaultsForStyleableControls);
        yield return new TestCase(
            "ThemeRuntime_SceneCompilation_PreservesExplicitControlStyleOverrides",
            SceneCompilation_PreservesExplicitControlStyleOverrides);
    }

    private static Task RuntimeOptionsTheme_PropagatesToContext()
    {
        var theme = TeaThemes.RosePine(RosePineVariant.Main);
        var app = new ContextThemeProbeApp();

        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                Theme = theme,
            });

        _ = app.UpdateRuntime(new WindowResized(120, 40));
        _ = app.UpdateRuntime(new FocusChanged(false));
        _ = app.RenderRuntime();

        TestAssert.ReferenceSame(theme, app.Context.Theme!, "Runtime options theme should propagate to ScreenContext.");
        TestAssert.ReferenceSame(theme, app.LastBuildTheme!, "Build should receive the runtime theme through ScreenContext.");
        TestAssert.True(app.Context.Width == 120 && app.Context.Height == 40, "Context size should still track resize updates.");
        TestAssert.True(!app.Context.HasFocus, "Context focus should still track focus updates.");
        return Task.CompletedTask;
    }

    private static Task SceneCompilation_AppliesThemeDefaultsForStyleableControls()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            },
        };

        var listView = new ListView<string>(static value => value)
        {
            Border = BorderStyle.None,
        };
        listView.SetItems(["alpha", "beta"]);

        var output = Render(new ControlThemeProbeApp(listView), theme, width: 28, height: 3);
        AssertContains(output, "\u001b[38;5;10m");
        return Task.CompletedTask;
    }

    private static Task SceneCompilation_PreservesExplicitControlStyleOverrides()
    {
        var theme = new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(AnsiColor.BrightGreen),
            },
        };

        var button = new Button
        {
            Text = "Ship",
            Border = BorderStyle.None,
            LabelStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightMagenta),
        };

        var output = Render(new ControlThemeProbeApp(button), theme, width: 28, height: 2);
        AssertContains(output, "\u001b[38;5;13m");
        AssertNotContains(output, "\u001b[38;5;10m");
        return Task.CompletedTask;
    }

    private static string Render(TeaApp app, TeaTheme theme, int width, int height)
    {
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                Theme = theme,
            });
        _ = app.UpdateRuntime(new WindowResized(width, height));
        return app.RenderRuntime().Output.Frame.Content;
    }

    private static void AssertContains(string actual, string expectedFragment)
    {
        if (!actual.Contains(expectedFragment, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Expected output to contain '{Escape(expectedFragment)}'.");
        }
    }

    private static void AssertNotContains(string actual, string unexpectedFragment)
    {
        if (actual.Contains(unexpectedFragment, StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"Expected output to not contain '{Escape(unexpectedFragment)}'.");
        }
    }

    private static string Escape(string text)
    {
        return text
            .Replace("\u001b", "\\u001b", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal);
    }

    private sealed class ContextThemeProbeApp : TeaApp
    {
        public TeaTheme? LastBuildTheme { get; private set; }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context)
        {
            LastBuildTheme = context.Theme;
            return Screen.From("theme-context");
        }
    }

    private sealed class ControlThemeProbeApp : TeaApp
    {
        private readonly Control _control;

        public ControlThemeProbeApp(Control control)
        {
            _control = control;
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From(_control);
    }
}
