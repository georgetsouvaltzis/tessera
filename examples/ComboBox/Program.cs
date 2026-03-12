using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Components.Primitives;

var app = Tea.CreateBuilder()
    .UseApp<ComboBoxDemoApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp ComboBox Example",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
            EnableBracketedPaste = true,
        };
    })
    .Build();

await app.RunAsync();

internal sealed class ComboBoxDemoApp : TeaApp
{
    private readonly ComboBox _combobox = new()
    {
        Title = "Region",
        Placeholder = "type to filter regions",
        MaxVisibleItems = 7,
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        IsFocused = true,
    };

    private readonly Label _details = new()
    {
        Title = "Selection",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();

    public ComboBoxDemoApp()
    {
        _combobox.SetItems(
        [
            "us-east-1",
            "us-east-2",
            "us-west-1",
            "us-west-2",
            "eu-central-1",
            "eu-west-1",
            "eu-west-2",
            "ap-southeast-1",
            "ap-southeast-2",
            "ap-northeast-1",
            "sa-east-1",
        ]);

        _combobox.SelectionChanged += (_, args) =>
        {
            _details.Text = $"Selected: {args.SelectedItem}";
            _status.RightText = $"selected={args.SelectedItem}";
        };

        _details.Text = $"Selected: {_combobox.SelectedItem}";
        _status.RightText = "ready";
    }

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = "Type to filter   Enter select   Esc close   q quit";
        _details.Text =
            $"""
            Selected: {_combobox.SelectedItem}
            Filter: {_combobox.FilterText}
            Open: {(_combobox.IsOpen ? "yes" : "no")}
            """;

        var content = new ColumnLayout
        {
            Gap = 1,
        };
        content.AddFixed(_combobox, 9);
        content.AddFixed(_details, 6);

        return Screen.From(new WindowLayout
        {
            Footer = LayoutSlot.Fixed(_status, 1),
            Body = new CenterLayout(
                new PanelLayout(
                    content,
                    title: "ComboBox",
                    border: BorderStyle.Rounded,
                    padding: Thickness.All(1)),
                width: Math.Min(60, Math.Max(38, context.Width - 4)),
                height: 20),
            Padding = Thickness.All(1),
        });
    }
}
