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
        if (InputHandled)
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

        var content = new ColumnLayout
        {
            Gap = 1,
        };
        content.AddFixed(_choice, 8);
        content.AddFixed(_details, 5);

        return Screen.From(new WindowLayout
        {
            Footer = LayoutSlot.Fixed(_status, 1),
            Body = new CenterLayout(
                new PanelLayout(
                    content,
                    title: "TeaSharp Choice",
                    border: TeaSharp.Components.Primitives.BorderStyle.Rounded,
                    padding: TeaSharp.Components.Primitives.Thickness.All(1)),
                width: Math.Min(54, Math.Max(32, context.Width - 4)),
                height: 16),
            Padding = TeaSharp.Components.Primitives.Thickness.All(1),
        });
    }
}
