using NUnit.Framework;
using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ThemeOverridesRuntimeWiringTests
{
    [Test]
    public void ThemeOverridesRuntimeWiringRuntimeCreationPropagatesThemeAndOverridesToContext()
    {
        var theme = BuildThemeWithPrimary(AnsiColor.BrightGreen);
        var overrides = new TesseraThemeOverrides();
        var app = new RuntimeProbeApp(new Button { Text = "Ship" });

        _ = TesseraApplication.CreateApplication(
            app,
            new TesseraRuntimeOptions { Theme = theme, ThemeOverrides = overrides });

        Assert.That(ReferenceEquals(theme, app.Context.Theme), Is.True,
            "Runtime should hydrate ScreenContext.Theme from TesseraRuntimeOptions.");
        Assert.That(ReferenceEquals(overrides, app.Context.ThemeOverrides), Is.True,
            "Runtime should hydrate ScreenContext.ThemeOverrides from TesseraRuntimeOptions.");
    }

    [Test]
    public void ThemeOverridesRuntimeWiringSceneCompilationAppliesOverrideHierarchyForFocusedState()
    {
        var button = new Button { Text = "Deploy", IsFocused = true };

        var baseTheme = BuildThemeWithPrimary(AnsiColor.BrightGreen);
        var overrides = new TesseraThemeOverrides { GlobalTheme = BuildThemeWithPrimary(AnsiColor.BrightBlue) };
        overrides.SetControlType<Button>(BuildThemeWithPrimary(AnsiColor.BrightYellow));
        overrides.SetControlInstance(button, BuildThemeWithPrimary(AnsiColor.BrightCyan));
        overrides.SetControlInstanceState(button, TesseraThemeVisualState.Focused,
            BuildThemeWithPrimary(AnsiColor.BrightRed));

        Render(button, baseTheme, overrides);

        AssertForeground(
            button.LabelStyle,
            AnsiColor.BrightRed,
            "Focused instance-state override should win over global/type/instance/base themes.");
    }

    [Test]
    public void ThemeOverridesRuntimeWiringSceneCompilationPreservesExplicitControlStyles()
    {
        var explicitLabelStyle = TesseraStyle.Empty.WithForeground(AnsiColor.BrightMagenta);
        var button = new Button { Text = "Run", IsFocused = true, LabelStyle = explicitLabelStyle };

        var baseTheme = BuildThemeWithPrimary(AnsiColor.BrightGreen);
        var overrides = new TesseraThemeOverrides();
        overrides.SetControlInstanceState(button, TesseraThemeVisualState.Focused,
            BuildThemeWithPrimary(AnsiColor.BrightRed));

        Render(button, baseTheme, overrides);

        Assert.That(button.LabelStyle, Is.EqualTo(explicitLabelStyle),
            "Runtime theming should not overwrite explicit non-empty control style assignments.");
    }

    [Test]
    public void ThemeOverridesRuntimeWiringSceneCompilationRespectsRuntimeTerminalFocusWhenResolvingState()
    {
        var button = new Button { Text = "Focus", IsFocused = true };

        var baseTheme = BuildThemeWithPrimary(AnsiColor.BrightGreen);
        var overrides = new TesseraThemeOverrides();
        overrides.SetControlInstanceState(button, TesseraThemeVisualState.Focused,
            BuildThemeWithPrimary(AnsiColor.BrightRed));

        var app = new RuntimeProbeApp(button);
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions { Theme = baseTheme, ThemeOverrides = overrides });
        _ = app.UpdateRuntime(new WindowResized(24, 2));
        _ = app.UpdateRuntime(new FocusChanged(false));
        _ = app.RenderRuntime();

        AssertForeground(
            button.LabelStyle,
            AnsiColor.BrightGreen,
            "Focused overrides should not apply while runtime terminal focus is lost.");
    }

    private static void Render(Button button, TesseraTheme theme, TesseraThemeOverrides overrides)
    {
        var app = new RuntimeProbeApp(button);
        app.ConfigureRuntimeOptions(
            new TesseraRuntimeOptions { Theme = theme, ThemeOverrides = overrides });
        _ = app.UpdateRuntime(new WindowResized(24, 2));
        _ = app.RenderRuntime();
    }

    private static TesseraTheme BuildThemeWithPrimary(AnsiColor color)
    {
        return new TesseraTheme
        {
            Text = new TesseraThemeTextTokens { Primary = TesseraStyle.Empty.WithForeground(color) }
        };
    }

    private static void AssertForeground(TesseraStyle style, AnsiColor expected, string message)
    {
        Assert.That(style.IsEmpty, Is.False, $"{message} Style should not be empty.");
        Assert.That(style.Foreground.HasValue, Is.True, $"{message} Foreground should be set.");
        Assert.That(style.Foreground!.Value, Is.EqualTo(expected), message);
    }

    private sealed class RuntimeProbeApp : TesseraApp
    {
        private readonly Control _control;

        public RuntimeProbeApp(Control control)
        {
            _control = control;
        }

        public override TesseraEffect? Update(Message message)
        {
            return null;
        }

        public override Screen Build(ScreenContext context)
        {
            return Screen.From(_control);
        }
    }
}
