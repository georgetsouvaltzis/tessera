using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<ComboBoxDemoApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = ComboBoxDemoApp.DemoTheme;
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
    internal static readonly TeaTheme DemoTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly ComboBox _combobox = new()
    {
        Title = "Cloud Region",
        Placeholder = "start typing to narrow regions",
        MaxVisibleItems = 7,
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Label _details = new()
    {
        Title = "Selection Snapshot",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();

    public ComboBoxDemoApp()
    {
        _combobox.ApplyThemeDefaults(DemoTheme);
        _details.ApplyThemeDefaults(DemoTheme);
        _status.ApplyThemeDefaults(DemoTheme);

        _combobox.FocusedTitleStyle = DemoTheme.Focus.Title.WithUnderline();
        _combobox.PlaceholderTextStyle = DemoTheme.Text.Muted.WithItalic();
        _details.TextStyle = DemoTheme.Text.Secondary;
        _status.Fill = '·';
        _status.LeftTextStyle = DemoTheme.Text.Muted;
        _status.RightTextStyle = DemoTheme.Accent.Primary.WithBold();

        _combobox.RequestFocus();
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
            RefreshSelection(args.SelectedItem);
        };

        RefreshSelection(_combobox.SelectedItem);
    }

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = "Type to filter   Enter pick   Esc close list   q quit";
        _details.Text =
            $"""
            Selected region
            {DemoTheme.Accent.Primary.WithBold().Render(_combobox.SelectedItem)}
            Query: {DemoTheme.Text.Muted.Render(string.IsNullOrEmpty(_combobox.FilterText) ? "none" : _combobox.FilterText)}
            State: {(_combobox.IsOpen ? "expanded" : "collapsed")}
            """;

        var content = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _combobox,
                    Length = 10,
                },
                new LayoutSlot
                {
                    Content = _details,
                    Length = 7,
                },
            },
        };
        var panel = new PanelLayout
        {
            Content = content,
            Title = "Catppuccin • ComboBox",
            Border = BorderStyle.Rounded,
            Padding = Thickness.All(1),
        };
        var body = new CenterLayout
        {
            Content = panel,
            Width = Math.Min(72, Math.Max(46, context.Width - 6)),
            Height = 21,
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
        _status.RightText = $"● {value}";
    }
}
