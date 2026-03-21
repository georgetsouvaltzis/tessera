using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<OrdersApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.MaxFps = 30;
        runtime.Theme = OrdersApp.DemoTheme;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Showcase",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record OrderSelected(string OrderId) : Message;
internal sealed record RefreshRequested : Message;
internal sealed record CommandSubmitted(string Value) : Message;
internal sealed record DeleteDialogResponded(DialogResult Result) : Message;

internal sealed class OrdersApp : TeaApp
{
    internal static readonly TeaTheme DemoTheme = TeaThemes.RosePine(RosePineVariant.Moon);

    private readonly Dictionary<string, string> _details = new(StringComparer.Ordinal)
    {
        ["ORD-1024"] = "Pending shipment\nCustomer: Northwind\nPriority: High",
        ["ORD-1025"] = "Packed\nCustomer: Tailspin\nPriority: Normal",
        ["ORD-1026"] = "Delayed by carrier\nCustomer: Fabrikam\nPriority: High",
        ["ORD-1027"] = "Ready for invoicing\nCustomer: Contoso\nPriority: Low",
    };

    private readonly Choice _orders = new()
    {
        Title = "Orders",
        Border = BorderStyle.Rounded,
        FocusMarker = "◆",
    };
    private readonly Label _summary = new()
    {
        Title = "Summary",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };
    private readonly TextArea _detailsView = new()
    {
        Title = "Details",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Wrap = true,
        FocusMarker = "◆",
    };
    private readonly TextInput _command = new()
    {
        Title = "Command",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        Placeholder = "refresh, delete, or focus",
        ClearOnSubmit = true,
        FocusMarker = "◆",
    };
    private readonly Button _refresh = new()
    {
        Text = "Refresh",
        Description = "Enter to reload",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
    };
    private readonly Dialog _confirmDelete = new()
    {
        Title = "Confirm Delete",
        BodyLines =
        [
            "Delete selected order?",
            "Enter accepts",
            "Esc cancels",
        ],
    };
    private readonly StatusBar _status = new();

    private string _statusText = "Ready";

    public OrdersApp()
    {
        ApplyVisuals();
        _orders.SetItems(_details.Keys.OrderBy(static value => value, StringComparer.Ordinal).ToArray());
        _orders.SelectionChanged += (_, args) => Post(new OrderSelected(args.SelectedItem));
        _refresh.Activated += (_, _) => Post(new RefreshRequested());
        _command.Submitted += (_, args) => Post(new CommandSubmitted(args.Value));
        _confirmDelete.Accepted += (_, _) => Post(new DeleteDialogResponded(DialogResult.Accepted));
        _confirmDelete.Dismissed += (_, _) => Post(new DeleteDialogResponded(DialogResult.Dismissed));
        SelectOrder(_orders.SelectedItem);
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (message is KeyPressed focusKey && focusKey.IsCharacter('d'))
        {
            OpenDeleteDialog();
            return null;
        }

        switch (message)
        {
            case OrderSelected selected:
                SelectOrder(selected.OrderId);
                break;
            case RefreshRequested:
                Refresh();
                break;
            case CommandSubmitted submitted:
                Execute(submitted.Value);
                break;
            case DeleteDialogResponded responded:
                ApplyDialogResult(responded.Result);
                break;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _summary.Text =
            $"""
            Selected: {_orders.SelectedItem}
            Terminal: {context.Width} x {context.Height}
            Focused: {context.HasFocus}
            """;

        _status.LeftText = $"Order {_orders.SelectedItem}    Tab move focus    d delete";
        _status.RightText = _statusText;

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            window.Header(5, header => header.Center(_refresh, width: 26, height: 5));
            window.Footer(1, _status);
            window.Left(28, panel => panel.Border().Padding(1).Content(_orders));
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(6, _summary);
                column.Fill(_detailsView);
                column.Fixed(5, _command);
            }));
            window.Overlay(overlay => overlay.Center(_confirmDelete, width: 42, height: 8));
        });
    }

    private void SelectOrder(string orderId)
    {
        if (string.IsNullOrWhiteSpace(orderId) || !_details.TryGetValue(orderId, out var details))
        {
            _detailsView.SetValue("No order selected.");
            return;
        }

        _detailsView.SetValue(details);
        _statusText = $"Selected {orderId}";
    }

    private void Refresh()
    {
        _statusText = $"Refreshed at {DateTimeOffset.Now:HH:mm:ss}";
    }

    private void Execute(string command)
    {
        var normalized = command.Trim();
        if (normalized.Length == 0)
        {
            return;
        }

        if (normalized.Equals("refresh", StringComparison.OrdinalIgnoreCase))
        {
            Refresh();
            return;
        }

        if (normalized.Equals("delete", StringComparison.OrdinalIgnoreCase))
        {
            OpenDeleteDialog();
            return;
        }

        if (normalized.Equals("focus", StringComparison.OrdinalIgnoreCase))
        {
            _statusText = "Use Tab / Shift+Tab to move focus.";
            return;
        }

        _statusText = $"Unknown command: {normalized}";
    }

    private void OpenDeleteDialog()
    {
        if (string.IsNullOrWhiteSpace(_orders.SelectedItem))
        {
            _statusText = "Nothing to delete.";
            return;
        }

        _confirmDelete.Show(
            $"Delete {_orders.SelectedItem}?",
            "Press Enter to remove it.",
            "Press Esc to keep it.");
    }

    private void ApplyDialogResult(DialogResult result)
    {
        if (result != DialogResult.Accepted)
        {
            _statusText = "Delete cancelled.";
            return;
        }

        var selected = _orders.SelectedItem;
        if (!_details.Remove(selected))
        {
            _statusText = "Nothing to delete.";
            return;
        }

        _orders.SetItems(_details.Keys.OrderBy(static value => value, StringComparer.Ordinal).ToArray());
        SelectOrder(_orders.SelectedItem);
        _statusText = $"Deleted {selected}";
    }

    private void ApplyVisuals()
    {
        var selectedStyle = DemoTheme.Selection.Background.Merge(DemoTheme.Selection.Foreground);

        _orders.Glyphs = new DropdownGlyphSet("⌄", "⌃", "▸", "◆");
        _orders.TitleStyle = DemoTheme.Accent.Primary.WithBold();
        _orders.FocusedTitleStyle = DemoTheme.Focus.Title.WithBold();
        _orders.BorderStyleText = DemoTheme.Border.Strong;
        _orders.FocusedBorderStyleText = DemoTheme.Border.Focused.Merge(DemoTheme.Focus.Border);
        _orders.ValueStyle = DemoTheme.Text.Primary.WithBold();
        _orders.OptionStyle = DemoTheme.Text.Secondary;
        _orders.SelectedOptionStyle = selectedStyle.WithBold();
        _orders.HoveredOptionStyle = DemoTheme.Accent.Secondary.WithUnderline();
        _orders.HoveredValueStyle = DemoTheme.Accent.Primary.WithUnderline();
        _orders.MutedStyle = DemoTheme.Text.Muted;

        _summary.TitleStyle = DemoTheme.Text.Secondary;
        _summary.FocusedTitleStyle = DemoTheme.Focus.Title;
        _summary.BorderStyleText = DemoTheme.Border.Default;
        _summary.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _summary.TextStyle = DemoTheme.Text.Primary;

        _detailsView.TitleStyle = DemoTheme.Text.Secondary;
        _detailsView.FocusedTitleStyle = DemoTheme.Focus.Title;
        _detailsView.BorderStyleText = DemoTheme.Border.Default;
        _detailsView.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _detailsView.ValueTextStyle = DemoTheme.Text.Primary;

        _command.TitleStyle = DemoTheme.Text.Secondary;
        _command.FocusedTitleStyle = DemoTheme.Focus.Title;
        _command.BorderStyleText = DemoTheme.Border.Default;
        _command.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _command.ValueTextStyle = DemoTheme.Text.Primary;
        _command.PlaceholderTextStyle = DemoTheme.Text.Muted.WithItalic();

        _refresh.LabelStyle = DemoTheme.Text.Primary.WithBold();
        _refresh.FocusedLabelStyle = DemoTheme.Accent.Primary.WithBold();
        _refresh.PressedLabelStyle = selectedStyle.WithBold();
        _refresh.BorderStyleText = DemoTheme.Border.Default;
        _refresh.FocusedBorderStyleText = DemoTheme.Focus.Border;

        _confirmDelete.TitleStyle = DemoTheme.Text.Secondary;
        _confirmDelete.FocusedTitleStyle = DemoTheme.Focus.Title;
        _confirmDelete.BorderStyleText = DemoTheme.Border.Strong;
        _confirmDelete.FocusedBorderStyleText = DemoTheme.Focus.Border;
        _confirmDelete.BodyTextStyle = DemoTheme.Text.Primary;
        _confirmDelete.FocusMarker = "◆";

        _status.Fill = '·';
        _status.LeftTextStyle = DemoTheme.Text.Muted;
        _status.RightTextStyle = DemoTheme.Accent.Primary.WithBold();
        _status.FillStyle = DemoTheme.Surface.Panel;
    }
}
