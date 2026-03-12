using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<ProductivityApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Productivity Example",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed class ProductivityApp : TeaApp
{
    private readonly MenuBar _menu = new();
    private readonly Tabs _tabs = new("Backlog", "Today", "Done");
    private readonly ListView<string> _tasks = new();
    private readonly Table _table = new("Metric", "Value");
    private readonly TextInput _command = new()
    {
        Title = "Quick Command",
        Placeholder = "type refresh or help",
        Border = TeaSharp.Components.Primitives.BorderStyle.SingleLine,
        Padding = TeaSharp.Components.Primitives.Thickness.All(1),
        ClearOnSubmit = true,
    };
    private readonly StatusBar _status = new();

    private readonly Dictionary<string, string[]> _taskSets = new(StringComparer.Ordinal)
    {
        ["Backlog"] = ["Review API names", "Replace legacy docs", "Add migration guide", "Audit examples"],
        ["Today"] = ["Finalize composition wrappers", "Verify integrations", "Write control tests"],
        ["Done"] = ["TeaApp foundation", "Showcase migration", "Legacy discoverability pass"],
    };

    public ProductivityApp()
    {
        _menu.SetItems(
        [
            new MenuItem("refresh", "Refresh", 'r'),
            new MenuItem("focus", "Focus Help", 'f'),
            new MenuItem("quit", "Quit", 'q'),
        ]);

        _menu.ItemActivated += (_, args) =>
        {
            if (args.ItemId == "refresh")
            {
                Refresh();
            }
            else if (args.ItemId == "focus")
            {
                _status.RightText = "Use Tab / Shift+Tab to move focus.";
            }
            else if (args.ItemId == "quit")
            {
                RequestEffect(TeaEffects.Quit);
            }
        };

        _tabs.SelectionChanged += (_, args) =>
        {
            LoadTasks(args.SelectedItem);
        };

        _command.Submitted += (_, args) =>
        {
            if (args.Value.Equals("refresh", StringComparison.OrdinalIgnoreCase))
            {
                Refresh();
            }
            else if (args.Value.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                _status.RightText = "Menu: r refresh   f focus help   q quit";
            }
            else
            {
                _status.RightText = $"Unknown command: {args.Value}";
            }
        };

        _tasks.Title = "Tasks";
        _tasks.Border = TeaSharp.Components.Primitives.BorderStyle.SingleLine;
        _tasks.Padding = TeaSharp.Components.Primitives.Thickness.All(1);

        _table.Title = "Summary";
        _table.Border = TeaSharp.Components.Primitives.BorderStyle.SingleLine;
        _table.Padding = TeaSharp.Components.Primitives.Thickness.All(1);
        _table.PageSize = 8;

        LoadTasks("Backlog");
        Refresh();
    }

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Tab={_tabs.Items[_tabs.SelectedIndex]}   Tasks={_tasks.Count}";

        var header = new ColumnLayout();
        header.AddFixed(_menu, 1);
        header.AddFixed(_tabs, 1);

        var details = new ColumnLayout
        {
            Gap = 1,
        };
        details.AddFill(_table);
        details.AddFixed(_command, 5);

        return Screen.From(new WindowLayout
        {
            Header = LayoutSlot.Fixed(header, 2),
            Footer = LayoutSlot.Fixed(_status, 1),
            Left = LayoutSlot.Fixed(_tasks, Math.Min(36, Math.Max(24, context.Width / 3))),
            Body = details,
            Gap = 1,
            Padding = TeaSharp.Components.Primitives.Thickness.All(1),
        });
    }

    private void LoadTasks(string tab)
    {
        if (!_taskSets.TryGetValue(tab, out var tasks))
        {
            tasks = [];
        }

        _tasks.SetItems(tasks);
        Refresh();
    }

    private void Refresh()
    {
        _table.SetRows(
        [
            ["Selected Tab", _tabs.Items[_tabs.SelectedIndex]],
            ["Visible Tasks", _tasks.Count.ToString(System.Globalization.CultureInfo.InvariantCulture)],
            ["Focused", Context.HasFocus ? "Yes" : "No"],
            ["Updated", DateTimeOffset.Now.ToString("HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)],
        ]);

        _status.RightText = "r refresh   q quit";
    }
}
