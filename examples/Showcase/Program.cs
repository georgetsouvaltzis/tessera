using TeaSharp;
using TeaSharp.Components.Primitives;
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
        SelectOrder(_orders.SelectedItem);
    }

    public override TeaEffect? Update(Message message)
    {
        if (HandleScreenInput(message))
        {
            if (_refresh.TryConsumeActivation())
            {
                Refresh();
            }

            if (_command.TryConsumeSubmission(out var command))
            {
                return Execute(command);
            }

            if (_confirmDelete.TryConsumeResult(out var result))
            {
                return ApplyDialogResult(result);
            }

            return null;
        }

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

        var content = new DockLayout(
            top: new LayoutSlot(new CenterLayout(_refresh, width: 26, height: 5), LayoutLength.Fixed(5)),
            bottom: new LayoutSlot(_status, LayoutLength.Fixed(1)),
            fill: new LayoutSlot(
                new SplitLayout(
                    LayoutOrientation.Horizontal,
                    new LayoutSlot(new PanelLayout(_orders, border: BorderStyle.SingleLine, padding: Thickness.All(1)), LayoutLength.Fixed(28)),
                    new LayoutSlot(
                        new StackLayout(
                            LayoutOrientation.Vertical,
                            gap: 1,
                            children:
                            [
                                new LayoutSlot(_summary, LayoutLength.Fixed(6)),
                                new LayoutSlot(_detailsView, LayoutLength.Fill()),
                                new LayoutSlot(_command, LayoutLength.Fixed(5)),
                            ]),
                        LayoutLength.Fill()),
                    gap: 1),
                LayoutLength.Fill()),
            gap: 1,
            padding: Thickness.All(1));

        return Screen.From(new OverlayLayout(
            content,
            new CenterLayout(_confirmDelete, width: 42, height: 8)));
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

    private TeaEffect? Execute(string command)
    {
        var normalized = command.Trim();
        if (normalized.Length == 0)
        {
            return null;
        }

        if (normalized.Equals("refresh", StringComparison.OrdinalIgnoreCase))
        {
            Refresh();
            return null;
        }

        if (normalized.Equals("delete", StringComparison.OrdinalIgnoreCase))
        {
            OpenDeleteDialog();
            return null;
        }

        if (normalized.Equals("focus", StringComparison.OrdinalIgnoreCase))
        {
            _statusText = "Use Tab / Shift+Tab to move focus.";
            return null;
        }

        _statusText = $"Unknown command: {normalized}";
        return null;
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

    private TeaEffect? ApplyDialogResult(DialogResult result)
    {
        if (result != DialogResult.Accepted)
        {
            _statusText = "Delete cancelled.";
            return null;
        }

        var selected = _orders.SelectedItem;
        if (!_details.Remove(selected))
        {
            _statusText = "Nothing to delete.";
            return null;
        }

        _orders.SetItems(_details.Keys.OrderBy(static value => value, StringComparer.Ordinal).ToArray());
        SelectOrder(_orders.SelectedItem);
        _statusText = $"Deleted {selected}";
        return null;
    }
}
