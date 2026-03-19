using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<ChoiceDemoApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = ChoiceDemoApp.DemoTheme;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Choice Example",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed class ChoiceDemoApp : TeaApp
{
    internal static readonly TeaTheme DemoTheme = TeaThemes.RosePine(RosePineVariant.Moon);

    private readonly Choice _choice = new()
    {
        Title = "Deployment Target",
        MaxVisibleItems = 6,
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Label _details = new()
    {
        Title = "Live Selection",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();

    public ChoiceDemoApp()
    {
        _choice.ApplyThemeDefaults(DemoTheme);
        _details.ApplyThemeDefaults(DemoTheme);
        _status.ApplyThemeDefaults(DemoTheme);

        _choice.FocusedTitleStyle = DemoTheme.Focus.Title.WithUnderline();
        _details.TextStyle = DemoTheme.Text.Secondary;
        _status.Fill = '·';
        _status.LeftTextStyle = DemoTheme.Text.Muted;
        _status.RightTextStyle = DemoTheme.Accent.Primary.WithBold();

        _choice.SetItems(
        [
            "Development",
            "Staging",
            "Production",
            "Canary",
            "Disaster-Recovery",
            "Benchmark",
            "QA",
            "Sandbox",
        ]);

        _choice.SelectionChanged += (_, args) =>
        {
            RefreshSelection(args.SelectedItem);
        };

        RefreshSelection(_choice.SelectedItem);
    }

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = "↑/↓ navigate   Enter/Space pick   Mouse hover/click   q quit";

        var content = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _choice,
                    Length = 9,
                },
                new LayoutSlot
                {
                    Content = _details,
                    Length = 6,
                },
            },
        };
        var panel = new PanelLayout
        {
            Content = content,
            Title = "Rosé Pine • Dropdown",
            Border = BorderStyle.Rounded,
            Padding = Thickness.All(1),
        };
        var body = new CenterLayout
        {
            Content = panel,
            Width = Math.Min(66, Math.Max(40, context.Width - 6)),
            Height = 18,
        };

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Body(body);
            window.Footer(1, _status);
        });
    }

    private void RefreshSelection(string value)
    {
        _details.Text =
            $"""
            Active profile
            {DemoTheme.Accent.Primary.WithBold().Render(value)}
            Rollout hint: this target will be used by the next deploy command.
            """;
        _status.RightText = $"● {value}";
    }
}
