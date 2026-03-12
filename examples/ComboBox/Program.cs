using TeaSharp;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Controls;
using TeaSharp.Layout;

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
    private readonly ComboboxComponent _combobox = new(new ComboboxOptions(
        Items:
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
        ],
        Title: "Region",
        Placeholder: "type to filter regions",
        IsFocused: true,
        MaxVisibleItems: 7,
        Border: BorderStyle.SingleLine,
        Padding: Thickness.All(1)));

    private readonly Label _details = new()
    {
        Title = "Selection",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();

    public ComboBoxDemoApp()
    {
        _combobox.SelectionChanged += (_, args) =>
        {
            _details.Text = $"Selected: {args.SelectedItem}";
            _status.RightText = $"selected={args.SelectedItem}";
        };

        _details.Text = $"Selected: {_combobox.SelectedItem}";
        _status.RightText = "ready";
    }

    public override TeaEffect? Update(Message message)
    {
        if (HandleScreenInput(message))
        {
            _details.Text =
                $"""
                Selected: {_combobox.SelectedItem}
                Filter: {_combobox.FilterText}
                Open: {(_combobox.IsOpen ? "yes" : "no")}
                """;

            return null;
        }

        return message is KeyPressed key && (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            ? TeaEffects.Quit
            : null;
    }

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = "Type to filter   Enter select   Esc close   q quit";

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
                                    new LayoutSlot(_combobox, LayoutLength.Fixed(9)),
                                    new LayoutSlot(_details, LayoutLength.Fixed(6)),
                                ]),
                            title: "ComboBox",
                            border: BorderStyle.Rounded,
                            padding: Thickness.All(1)),
                        width: Math.Min(60, Math.Max(38, context.Width - 4)),
                        height: 20),
                    LayoutLength.Fill()),
                padding: Thickness.All(1)));
    }
}
