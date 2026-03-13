using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<ChoiceDemoApp>()
    .ConfigureRuntime(static runtime =>
    {
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
    private readonly Choice _choice = new()
    {
        Title = "Environment",
        MaxVisibleItems = 6,
    };

    private readonly Label _details = new()
    {
        Title = "Selection",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();

    public ChoiceDemoApp()
    {
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
            _details.Text = $"Current: {args.SelectedItem}";
            _status.RightText = $"selected={args.SelectedItem}";
        };

        _details.Text = $"Current: {_choice.SelectedItem}";
        _status.RightText = "ready";
    }

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = "Enter/Space open-select   Up/Down move   q quit";

        var content = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _choice,
                    Length = 8,
                },
                new LayoutSlot
                {
                    Content = _details,
                    Length = 5,
                },
            },
        };
        var panel = new PanelLayout
        {
            Content = content,
            Title = "TeaSharp Choice",
            Border = BorderStyle.Rounded,
            Padding = Thickness.All(1),
        };
        var body = new CenterLayout
        {
            Content = panel,
            Width = Math.Min(54, Math.Max(32, context.Width - 4)),
            Height = 16,
        };

        return Screen.From(new WindowLayout
        {
            Footer = new LayoutSlot
            {
                Content = _status,
                Length = 1,
            },
            Body = body,
            Padding = Thickness.All(1),
        });
    }
}
