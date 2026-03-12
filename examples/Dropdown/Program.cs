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
        Border = TeaSharp.Components.Primitives.BorderStyle.SingleLine,
        Padding = TeaSharp.Components.Primitives.Thickness.All(1),
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
    {
        if (HandleScreenInput(message))
        {
            return null;
        }

        return message is KeyPressed key && (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            ? TeaEffects.Quit
            : null;
    }

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = "Enter/Space open-select   Up/Down move   q quit";

        return Screen.From(
            new DockLayout(
                bottom: new LayoutSlot(_status, LayoutLength.Fixed(1)),
                fill: new LayoutSlot(
                    new CenterLayout(
                        new PanelLayout(
                            new StackLayout(
                                LayoutOrientation.Vertical,
                                gap: 1,
                                children:
                                [
                                    new LayoutSlot(_choice, LayoutLength.Fixed(8)),
                                    new LayoutSlot(_details, LayoutLength.Fixed(5)),
                                ]),
                            title: "TeaSharp Choice",
                            border: TeaSharp.Components.Primitives.BorderStyle.Rounded,
                            padding: TeaSharp.Components.Primitives.Thickness.All(1)),
                        width: Math.Min(54, Math.Max(32, context.Width - 4)),
                        height: 16),
                    LayoutLength.Fill()),
                padding: TeaSharp.Components.Primitives.Thickness.All(1)));
    }
}
