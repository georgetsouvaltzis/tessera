using NUnit.Framework;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Tests;

[TestFixture]
[NonParallelizable]
public sealed class ThemeOverridesRuntimeWiringTests
{
    [Test]
    public void ThemeOverridesRuntimeWiringRuntimeCreationPropagatesThemeAndOverridesToContext()
    {
        var theme = BuildThemeWithPrimary(AnsiColor.BrightGreen);
        var overrides = new TeaThemeOverrides();
        var app = new RuntimeProbeApp(new Button
        {
            Text = "Ship",
            Border = BorderStyle.None,
        });

        _ = Tea.CreateApplication(
            app,
            new TeaRuntimeOptions
            {
                Theme = theme,
                ThemeOverrides = overrides,
            });

        Assert.That(ReferenceEquals(theme, app.Context.Theme), Is.True, "Runtime should hydrate ScreenContext.Theme from TeaRuntimeOptions.");
        Assert.That(ReferenceEquals(overrides, app.Context.ThemeOverrides), Is.True, "Runtime should hydrate ScreenContext.ThemeOverrides from TeaRuntimeOptions.");
    }

    [Test]
    public void ThemeOverridesRuntimeWiringSceneCompilationAppliesOverrideHierarchyForFocusedState()
    {
        var button = new Button
        {
            Text = "Deploy",
            Border = BorderStyle.None,
            IsFocused = true,
        };

        var baseTheme = BuildThemeWithPrimary(AnsiColor.BrightGreen);
        var overrides = new TeaThemeOverrides
        {
            GlobalTheme = BuildThemeWithPrimary(AnsiColor.BrightBlue),
        };
        overrides.SetControlType<Button>(BuildThemeWithPrimary(AnsiColor.BrightYellow));
        overrides.SetControlInstance(button, BuildThemeWithPrimary(AnsiColor.BrightCyan));
        overrides.SetControlInstanceState(button, TeaThemeVisualState.Focused, BuildThemeWithPrimary(AnsiColor.BrightRed));

        Render(button, baseTheme, overrides);

        AssertForeground(
            button.LabelStyle,
            AnsiColor.BrightRed,
            "Focused instance-state override should win over global/type/instance/base themes.");
    }

    [Test]
    public void ThemeOverridesRuntimeWiringSceneCompilationPreservesExplicitControlStyles()
    {
        var explicitLabelStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightMagenta);
        var button = new Button
        {
            Text = "Run",
            Border = BorderStyle.None,
            IsFocused = true,
            LabelStyle = explicitLabelStyle,
        };

        var baseTheme = BuildThemeWithPrimary(AnsiColor.BrightGreen);
        var overrides = new TeaThemeOverrides();
        overrides.SetControlInstanceState(button, TeaThemeVisualState.Focused, BuildThemeWithPrimary(AnsiColor.BrightRed));

        Render(button, baseTheme, overrides);

        Assert.That(button.LabelStyle, Is.EqualTo(explicitLabelStyle), "Runtime theming should not overwrite explicit non-empty control style assignments.");
    }

    [Test]
    public void ThemeOverridesRuntimeWiringSceneCompilationRespectsRuntimeTerminalFocusWhenResolvingState()
    {
        var button = new Button
        {
            Text = "Focus",
            Border = BorderStyle.None,
            IsFocused = true,
        };

        var baseTheme = BuildThemeWithPrimary(AnsiColor.BrightGreen);
        var overrides = new TeaThemeOverrides();
        overrides.SetControlInstanceState(button, TeaThemeVisualState.Focused, BuildThemeWithPrimary(AnsiColor.BrightRed));

        var app = new RuntimeProbeApp(button);
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                Theme = baseTheme,
                ThemeOverrides = overrides,
            });
        _ = app.UpdateRuntime(new WindowResized(24, 2));
        _ = app.UpdateRuntime(new FocusChanged(false));
        _ = app.RenderRuntime();

        AssertForeground(
            button.LabelStyle,
            AnsiColor.BrightGreen,
            "Focused overrides should not apply while runtime terminal focus is lost.");
    }

    private static void Render(Button button, TeaTheme theme, TeaThemeOverrides overrides)
    {
        var app = new RuntimeProbeApp(button);
        app.ConfigureRuntimeOptions(
            new TeaRuntimeOptions
            {
                Theme = theme,
                ThemeOverrides = overrides,
            });
        _ = app.UpdateRuntime(new WindowResized(24, 2));
        _ = app.RenderRuntime();
    }

    private static TeaTheme BuildThemeWithPrimary(AnsiColor color)
    {
        return new TeaTheme
        {
            Text = new TeaThemeTextTokens
            {
                Primary = TeaStyle.Empty.WithForeground(color),
            },
        };
    }

    private static void AssertForeground(TeaStyle style, AnsiColor expected, string message)
    {
        Assert.That(style.IsEmpty, Is.False, $"{message} Style should not be empty.");
        Assert.That(style.Foreground.HasValue, Is.True, $"{message} Foreground should be set.");
        Assert.That(style.Foreground!.Value, Is.EqualTo(expected), message);
    }

    private sealed class RuntimeProbeApp : TeaApp
    {
        private readonly Control _control;

        public RuntimeProbeApp(Control control)
        {
            _control = control;
        }

        public override TeaEffect? Update(Message message) => null;

        public override Screen Build(ScreenContext context) => Screen.From(_control);
    }
}
