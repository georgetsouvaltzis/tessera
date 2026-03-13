using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<OrdersApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.MaxFps = 30;
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Showcase",
            EnableFocusReporting = true,
            EnableBracketedPaste = true,
        };
    })
    .Build();

await app.RunAsync();

internal sealed class OrdersApp : TeaApp
{
    private readonly Dictionary<string, string> _details = new(StringComparer.Ordinal)
    {
        ["ORD-1024"] = "Pending shipment\nCustomer: Northwind\nPriority: High",
        ["ORD-1025"] = "Packed\nCustomer: Tailspin\nPriority: Normal",
        ["ORD-1026"] = "Delayed by carrier\nCustomer: Fabrikam\nPriority: High",
        ["ORD-1027"] = "Ready for invoicing\nCustomer: Contoso\nPriority: Low",
    };

    private readonly Choice _orders = new() { Title = "Orders" };
    private readonly Label _summary = new()
    {
        Title = "Summary",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };
    private readonly TextArea _detailsView = new()
    {
        Title = "Details",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Wrap = true,
    };
    private readonly TextInput _command = new()
    {
        Title = "Command",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Placeholder = "refresh, delete, or focus",
        ClearOnSubmit = true,
    };
    private readonly Button _refresh = new()
    {
        Text = "Refresh",
        Description = "Enter to reload",
        Border = BorderStyle.SingleLine,
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
        _orders.SetItems(_details.Keys.OrderBy(static value => value, StringComparer.Ordinal).ToArray());
        _orders.SelectionChanged += (_, args) => SelectOrder(args.SelectedItem);
        _refresh.Activated += (_, _) => Refresh();
        _command.Submitted += (_, args) => Execute(args.Value);
        _confirmDelete.Accepted += (_, _) => ApplyDialogResult(DialogResult.Accepted);
        _confirmDelete.Dismissed += (_, _) => ApplyDialogResult(DialogResult.Dismissed);
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

        var detailsColumn = new ColumnLayout
        {
            Gap = 1,
        };
        detailsColumn.AddFixed(_summary, 6);
        detailsColumn.AddFill(_detailsView);
        detailsColumn.AddFixed(_command, 5);
        var refreshPanel = new CenterLayout
        {
            Content = _refresh,
            Width = 26,
            Height = 5,
        };
        var ordersPanel = new PanelLayout
        {
            Content = _orders,
            Border = BorderStyle.SingleLine,
            Padding = Thickness.All(1),
        };
        var deleteOverlay = new CenterLayout
        {
            Content = _confirmDelete,
            Width = 42,
            Height = 8,
        };

        return Screen.From(new WindowLayout
        {
            Header = LayoutSlot.Fixed(refreshPanel, 5),
            Footer = LayoutSlot.Fixed(_status, 1),
            Left = LayoutSlot.Fixed(ordersPanel, 28),
            Body = detailsColumn,
            Overlay = deleteOverlay,
            Gap = 1,
            Padding = Thickness.All(1),
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
}
