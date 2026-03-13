using System.Globalization;
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
            WindowTitle = "TeaSharp Productivity Widgets",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal sealed class ProductivityApp : TeaApp
{
    private readonly MenuBar _menu = new();
    private readonly MultiSelect _checklist = new()
    {
        Title = "Checklist",
    };
    private readonly RadioGroup _priority = new()
    {
        Title = "Priority",
    };
    private readonly NumberInput _estimate = new()
    {
        Title = "Estimate (hrs)",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Min = 0,
        Max = 40,
        Step = 1,
        Precision = 0,
    };
    private readonly DatePicker _dueDate = new()
    {
        Title = "Due Date",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };
    private readonly TimePicker _dueTime = new()
    {
        Title = "Due Time",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };
    private readonly MarkdownView _runbook = new()
    {
        Title = "Runbook",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Wrap = true,
    };
    private readonly StatusBar _status = new();
    private string _statusMessage = "Ready.";

    public ProductivityApp()
    {
        _menu.SetItems(
        [
            new MenuItem("refresh", "Refresh", 'r'),
            new MenuItem("help", "Help", 'h'),
            new MenuItem("quit", "Quit", 'q'),
        ]);

        _menu.ItemActivated += (_, args) =>
        {
            if (args.ItemId == "refresh")
            {
                RefreshStatus("Refreshed plan state.");
            }
            else if (args.ItemId == "help")
            {
                RefreshStatus("Tab through the widgets. Use arrows to edit date/time/priority.");
            }
            else if (args.ItemId == "quit")
            {
                RequestEffect(TeaEffects.Quit);
            }
        };

        _priority.SetItems(["Low", "Normal", "High"]);
        _priority.SelectionChanged += (_, _) => RefreshStatus("Priority updated.");

        _estimate.SetValue(6);
        _estimate.Submitted += (_, args) => RefreshStatus($"Estimate submitted: {args.Value.ToString(CultureInfo.InvariantCulture)}h");

        _dueDate.SetDate(new DateOnly(2026, 3, 20));
        _dueDate.DateChanged += (_, _) => RefreshStatus("Due date updated.");

        _dueTime.SetValue(new TimeOnly(14, 30, 0));
        _dueTime.ValueChanged += (_, _) => RefreshStatus("Due time updated.");

        _checklist.SetItems(
        [
            ("Review API naming", true),
            ("Verify docs/examples", false),
            ("Run test suite", true),
            ("Audit root controls", false),
        ]);

        _runbook.SetMarkdown(
            """
            ## Productivity Flow

            - Use the checklist to track rollout tasks.
            - Adjust estimate, due date, and due time from the main pane.
            - Switch priority with the radio group.
            - `r` refreshes the status line.
            - `q` quits the example.
            """);

        RefreshStatus("Ready.");
    }

    public override TeaEffect? Update(Message message) =>
        message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl)
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText =
            $"{_priority.SelectedItem}  {_checklist.CheckedItems.Count}/{_checklistCheckedCount} done  " +
            $"{_dueDate.SelectedDate:yyyy-MM-dd} {_dueTime.Value:HH\\:mm}";
        _status.RightText = $"{_statusMessage}  Size {context.Width}x{context.Height}  Ctrl+C Quit";

        var left = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _checklist,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _priority,
                    Length = 6,
                },
            },
        };

        var schedule = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _estimate,
                    Length = 24,
                },
                new LayoutSlot
                {
                    Content = _dueDate,
                    Length = 30,
                },
                new LayoutSlot
                {
                    Content = _dueTime,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var body = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = schedule,
                    Length = 10,
                },
                new LayoutSlot
                {
                    Content = _runbook,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        return Screen.From(new WindowLayout
        {
            Header = new LayoutSlot
            {
                Content = _menu,
                Length = 1,
            },
            Left = new LayoutSlot
            {
                Content = left,
                Length = Math.Min(34, Math.Max(28, context.Width / 3)),
            },
            Body = body,
            Footer = new LayoutSlot
            {
                Content = _status,
                Length = 1,
            },
            Gap = 1,
            Padding = Thickness.All(1),
        });
    }

    private int _checklistCheckedCount => _checklist.CheckedItems.Count;

    private void RefreshStatus(string message)
    {
        _statusMessage = message;
    }
}
