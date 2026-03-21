using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<AdvancedWidgetsApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Theme = AdvancedWidgetsApp.DemoTheme;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Advanced Widgets",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record AdvancedTick(DateTimeOffset At) : Message;

internal sealed class AdvancedWidgetsApp : TeaApp
{
    internal static readonly TeaTheme DemoTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly Badge _modeBadge = new()
    {
        Text = "stable",
        Tone = BadgeTone.Success,
    };

    private readonly Toggle _toggle = new()
    {
        Title = "Feature Flag",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Slider _slider = new()
    {
        Title = "Concurrency",
        Min = 1,
        Max = 32,
        Step = 1,
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Spinner _spinner = new()
    {
        Title = "Indexer",
        Label = "running",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly TreeView _tree = new()
    {
        Title = "Workspace",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Notifications _notifications = new()
    {
        Title = "Notifications",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        MaxItems = 48,
        FocusMarker = "◆",
    };

    private readonly CommandPalette _palette = new()
    {
        Title = "Workspace Actions",
        FocusMarker = "◆",
        ShowFocusMarker = true,
    };

    private readonly ContextMenu _contextMenu = new()
    {
        Title = "Quick Actions",
        FocusMarker = "◆",
        ShowFocusMarker = true,
    };

    private readonly Label _summary = new()
    {
        Title = "Summary",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();

    private int _tick;

    public AdvancedWidgetsApp()
    {
        ApplyVisuals();
        _slider.SetValue(8);
        _toggle.SetValue(true);
        _tree.SetItems(
        [
            new TreeItem("root", "TeaSharp")
            {
                Expanded = true,
            },
            new TreeItem("runtime", "Runtime",
            [
                new TreeItem("input", "Input pipeline"),
                new TreeItem("render", "Renderer"),
                new TreeItem("screen", "Screen compiler"),
            ])
            {
                Expanded = true,
            },
            new TreeItem("controls", "Controls",
            [
                new TreeItem("core", "Root catalog"),
                new TreeItem("advanced", "Advanced catalog"),
            ])
            {
                Expanded = true,
            },
        ]);

        _notifications.Push("advanced demo booted", NotificationLevel.Success);
        _notifications.Push("tab cycles focus", NotificationLevel.Info);
        _notifications.Push("n pushes a notification", NotificationLevel.Warning);
        _notifications.Push("p opens command palette", NotificationLevel.Info);
        _notifications.Push("x opens context menu", NotificationLevel.Info);

        _palette.SetItems(
        [
            new CommandPaletteItem("refresh", "Refresh workspace", "poll files and redraw"),
            new CommandPaletteItem("notify", "Push notification", "emit a manual notification"),
            new CommandPaletteItem("pause", "Pause spinner", "toggle indexer activity"),
        ]);
        _palette.ItemExecuted += (_, args) =>
        {
            _notifications.Push($"palette: {args.Item.Title}", NotificationLevel.Success);
            if (args.ItemId == "notify")
            {
                _notifications.Push($"manual event {_tick:000}", NotificationLevel.Warning);
            }
            else if (args.ItemId == "pause")
            {
                _spinner.SetRunning(!_spinner.Running);
            }
        };

        _contextMenu.SetItems(
        [
            new ContextMenuItem("copy", "Copy summary"),
            new ContextMenuItem("clear", "Clear notifications"),
        ]);
        _contextMenu.ItemExecuted += (_, args) =>
        {
            _notifications.Push($"menu: {args.Item.Title}", NotificationLevel.Info);
            if (args.ItemId == "clear")
            {
                _notifications.Clear();
                _notifications.Push("notifications cleared", NotificationLevel.Warning);
            }
        };
    }

    public override TeaEffect? Initialize() => TeaEffects.Tick(TimeSpan.FromMilliseconds(250), static now => new AdvancedTick(now));

    public override TeaEffect? Update(Message message)
    {
        if (message is AdvancedTick tick)
        {
            _tick++;
            if (_spinner.Running)
            {
                _spinner.Advance();
            }

            if (_tick % 12 == 0)
            {
                _notifications.Push($"heartbeat {tick.At:HH:mm:ss}", NotificationLevel.Info);
            }

            return TeaEffects.Tick(TimeSpan.FromMilliseconds(250), static now => new AdvancedTick(now));
        }

        if (message is KeyPressed key)
        {
            if (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
            {
                return TeaEffects.Quit;
            }

            if (key.IsCharacter('n'))
            {
                _notifications.Push($"manual event {_tick:000}", NotificationLevel.Warning);
                _status.RightText = "notification pushed";
                return null;
            }

            if (key.IsCharacter('p'))
            {
                _palette.Open();
                _status.RightText = "palette open";
                return null;
            }

            if (key.IsCharacter('x'))
            {
                _contextMenu.OpenAt(2, 2);
                _status.RightText = "context menu open";
                return null;
            }
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _summary.Text =
            $"""
            Mode: {_modeBadge.Text}
            Toggle: {(_toggle.Value ? "ON" : "OFF")}
            Concurrency: {_slider.Value:0}
            Spinner: {(_spinner.Running ? "running" : "paused")}
            Selected node: {_tree.SelectedId ?? "none"}
            Notifications: {_notifications.Count}
            Palette: {(_palette.IsVisible ? "open" : "closed")}
            Context menu: {(_contextMenu.IsVisible ? "open" : "closed")}
            Size: {context.Width}x{context.Height}
            """;

        _modeBadge.Text = _toggle.Value ? "live" : "stable";
        _modeBadge.Tone = _toggle.Value ? BadgeTone.Warning : BadgeTone.Success;

        _status.LeftText = "Tab focus   Enter/Space activate   n notify   p palette   x menu   q quit";
        _status.RightText = $"tick={_tick:0000}";

        var left = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _modeBadge,
                    Length = 1,
                },
                new LayoutSlot
                {
                    Content = _toggle,
                    Length = 5,
                },
                new LayoutSlot
                {
                    Content = _slider,
                    Length = 6,
                },
                new LayoutSlot
                {
                    Content = _spinner,
                    Length = 5,
                },
                new LayoutSlot
                {
                    Content = _summary,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var right = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _tree,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _notifications,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        LayoutNode? overlay = null;
        if (_palette.IsVisible)
        {
            overlay = new CenterLayout
            {
                Content = _palette,
                Width = Math.Min(72, Math.Max(48, context.Width - 6)),
                Height = Math.Min(14, Math.Max(8, context.Height - 4)),
            };
        }
        else if (_contextMenu.IsVisible)
        {
            overlay = new CenterLayout
            {
                Content = _contextMenu,
                Width = 32,
                Height = 8,
            };
        }

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Left(Math.Min(38, Math.Max(30, context.Width / 3)), left);
            window.Body(right);

            if (overlay is not null)
            {
                window.Overlay(overlay);
            }

            window.Footer(1, _status);
        });
    }

    private void ApplyVisuals()
    {
        var selectedStyle = DemoTheme.Selection.Background.Merge(DemoTheme.Selection.Foreground);

        _modeBadge.TextStyle = DemoTheme.Text.Secondary;
        _modeBadge.SuccessTextStyle = DemoTheme.State.Success.WithBold();
        _modeBadge.WarningTextStyle = DemoTheme.State.Warning.WithBold();

        _toggle.TitleStyle = DemoTheme.Text.Secondary;
        _toggle.FocusedTitleStyle = DemoTheme.Focus.Title;
        _toggle.BorderStyleText = DemoTheme.Border.Default;
        _toggle.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _toggle.ValueStyle = DemoTheme.Text.Primary;
        _toggle.OnValueStyle = DemoTheme.State.Success.WithBold();
        _toggle.OffValueStyle = DemoTheme.State.Warning.WithBold();

        _slider.TitleStyle = DemoTheme.Text.Secondary;
        _slider.FocusedTitleStyle = DemoTheme.Focus.Title;
        _slider.BorderStyleText = DemoTheme.Border.Default;
        _slider.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _slider.ValueLabelStyle = DemoTheme.Accent.Secondary.WithBold();
        _slider.FillStyle = DemoTheme.Accent.Primary.WithBold();
        _slider.TrackStyle = DemoTheme.Text.Muted;

        _spinner.TitleStyle = DemoTheme.Text.Secondary;
        _spinner.FocusedTitleStyle = DemoTheme.Focus.Title;
        _spinner.BorderStyleText = DemoTheme.Border.Default;
        _spinner.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _spinner.ValueStyle = DemoTheme.Text.Primary;
        _spinner.RunningValueStyle = DemoTheme.State.Success.WithBold();
        _spinner.StoppedValueStyle = DemoTheme.State.Warning.WithBold();

        _tree.Glyphs = new TreeViewGlyphSet("▾", "▸", "◦");
        _tree.TitleStyle = DemoTheme.Accent.Primary.WithBold();
        _tree.FocusedTitleStyle = DemoTheme.Focus.Title.WithBold();
        _tree.BorderStyleText = DemoTheme.Border.Strong;
        _tree.FocusedBorderStyleText = DemoTheme.Border.Focused.Merge(DemoTheme.Focus.Border);
        _tree.BranchStyle = DemoTheme.Text.Secondary;
        _tree.LeafStyle = DemoTheme.Text.Primary;
        _tree.SelectedItemStyle = selectedStyle.WithBold();
        _tree.HoveredItemStyle = DemoTheme.Accent.Secondary.WithUnderline();

        _notifications.TitleStyle = DemoTheme.Text.Secondary;
        _notifications.FocusedTitleStyle = DemoTheme.Focus.Title;
        _notifications.BorderStyleText = DemoTheme.Border.Default;
        _notifications.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _notifications.ItemStyle = DemoTheme.Text.Secondary;
        _notifications.SelectedItemStyle = selectedStyle;
        _notifications.HoveredItemStyle = DemoTheme.Accent.Primary.WithUnderline();
        _notifications.UnreadItemStyle = DemoTheme.Text.Primary.WithBold();
        _notifications.InfoItemStyle = DemoTheme.State.Info;
        _notifications.SuccessItemStyle = DemoTheme.State.Success;
        _notifications.WarningItemStyle = DemoTheme.State.Warning;
        _notifications.ErrorItemStyle = DemoTheme.State.Error;

        _palette.TitleStyle = DemoTheme.Text.Secondary;
        _palette.FocusedTitleStyle = DemoTheme.Focus.Title;
        _palette.BorderStyleText = DemoTheme.Border.Strong;
        _palette.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _palette.QueryTextStyle = DemoTheme.Text.Primary;
        _palette.PlaceholderTextStyle = DemoTheme.Text.Muted.WithItalic();
        _palette.ItemStyle = DemoTheme.Text.Secondary;
        _palette.SelectedItemStyle = selectedStyle;
        _palette.HoveredItemStyle = DemoTheme.Accent.Secondary.WithUnderline();

        _contextMenu.TitleStyle = DemoTheme.Text.Secondary;
        _contextMenu.FocusedTitleStyle = DemoTheme.Focus.Title;
        _contextMenu.BorderStyleText = DemoTheme.Border.Strong;
        _contextMenu.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _contextMenu.ItemStyle = DemoTheme.Text.Secondary;
        _contextMenu.SelectedItemStyle = selectedStyle;
        _contextMenu.HoveredItemStyle = DemoTheme.Accent.Secondary.WithUnderline();

        _summary.TitleStyle = DemoTheme.Text.Secondary;
        _summary.FocusedTitleStyle = DemoTheme.Focus.Title;
        _summary.BorderStyleText = DemoTheme.Border.Default;
        _summary.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _summary.TextStyle = DemoTheme.Text.Primary;

        _status.Fill = '·';
        _status.LeftTextStyle = DemoTheme.Text.Muted;
        _status.RightTextStyle = DemoTheme.Accent.Primary.WithBold();
        _status.FillStyle = DemoTheme.Surface.Panel;
    }
}
