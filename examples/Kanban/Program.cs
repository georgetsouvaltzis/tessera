using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<KanbanApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Kanban",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
        };
    })
    .Build();

await app.RunAsync();

internal enum KanbanLane
{
    Todo = 0,
    Doing = 1,
    Done = 2,
}

internal enum CardPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
}

internal sealed record KanbanCard(int Id, string Title, CardPriority Priority, bool Blocked = false, string Assignee = "unassigned");

internal sealed class KanbanBoardState
{
    private int _nextId;

    public KanbanBoardState(string name, IEnumerable<KanbanCard> todo, IEnumerable<KanbanCard> doing, IEnumerable<KanbanCard> done)
    {
        Name = name;
        Todo = [.. todo];
        Doing = [.. doing];
        Done = [.. done];
        _nextId = Todo.Concat(Doing).Concat(Done).Select(static card => card.Id).DefaultIfEmpty(100).Max() + 1;
    }

    public string Name { get; }

    public List<KanbanCard> Todo { get; }

    public List<KanbanCard> Doing { get; }

    public List<KanbanCard> Done { get; }

    public KanbanCard CreateCard(string title, CardPriority priority) => new(_nextId++, title, priority);
}

internal sealed class KanbanApp : TeaApp
{
    private readonly Tabs _boards = new("Platform", "Mobile", "Docs");
    private readonly List<KanbanBoardState> _boardStates =
    [
        new(
            "Platform",
            [
                new KanbanCard(101, "Add plugin API", CardPriority.High),
                new KanbanCard(102, "Refactor keymap docs", CardPriority.Medium),
                new KanbanCard(103, "Design release notes", CardPriority.Low),
            ],
            [
                new KanbanCard(104, "Stabilize viewport sync", CardPriority.High, Blocked: true),
                new KanbanCard(105, "Stress test event decoder", CardPriority.Medium),
            ],
            [
                new KanbanCard(106, "Ship widget gallery", CardPriority.Medium),
            ]),
        new(
            "Mobile",
            [
                new KanbanCard(201, "Touch key fallback", CardPriority.High),
                new KanbanCard(202, "Compact layout mode", CardPriority.Medium),
            ],
            [
                new KanbanCard(203, "Session reconnect UI", CardPriority.High),
            ],
            [
                new KanbanCard(204, "Landscape split polish", CardPriority.Low),
                new KanbanCard(205, "Ship iOS demo", CardPriority.Medium),
            ]),
        new(
            "Docs",
            [
                new KanbanCard(301, "Write migration guide", CardPriority.High),
                new KanbanCard(302, "Update parity matrix", CardPriority.Medium),
            ],
            [
                new KanbanCard(303, "Review API cookbook", CardPriority.Medium),
            ],
            [
                new KanbanCard(304, "Publish getting started", CardPriority.Low),
            ]),
    ];

    private readonly ListView<KanbanCard> _todo = new(CardLabel)
    {
        Title = "Todo",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly ListView<KanbanCard> _doing = new(CardLabel)
    {
        Title = "In Progress",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly ListView<KanbanCard> _done = new(CardLabel)
    {
        Title = "Done",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly TextInput _composer = new()
    {
        Title = "New Card",
        Placeholder = "Enter title and press Enter. Prefix ! for high, ~ for low.",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        ClearOnSubmit = true,
    };

    private readonly Label _details = new()
    {
        Title = "Selected Card",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Notifications _activity = new()
    {
        Title = "Activity",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        MaxItems = 48,
    };

    private readonly Dialog _deleteDialog = new()
    {
        Title = "Delete Card",
        BodyLines =
        [
            "Delete selected card?",
            "Enter accepts",
            "Esc cancels",
        ],
    };

    private readonly StatusBar _status = new();

    private KanbanLane _activeLane = KanbanLane.Todo;
    private string _statusText = "ready";

    public KanbanApp()
    {
        _boards.SelectionChanged += (_, args) => LoadBoard(args.SelectedIndex);

        _todo.SelectionChanged += (_, args) => SelectCard(KanbanLane.Todo, args.SelectedItem);
        _doing.SelectionChanged += (_, args) => SelectCard(KanbanLane.Doing, args.SelectedItem);
        _done.SelectionChanged += (_, args) => SelectCard(KanbanLane.Done, args.SelectedItem);
        _composer.Submitted += (_, args) => CreateCard(args.Value);
        _deleteDialog.Accepted += (_, _) => ApplyDelete(DialogResult.Accepted);
        _deleteDialog.Dismissed += (_, _) => ApplyDelete(DialogResult.Dismissed);

        _activity.Push("Kanban ready", NotificationLevel.Success);
        _activity.Push("Use Tab to move focus", NotificationLevel.Info);
        _activity.Push("h/l move cards between lanes", NotificationLevel.Warning);

        LoadBoard(0);
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.IsCharacter('d'))
        {
            OpenDeleteDialog();
        }
        else if (key.IsCharacter('h') || key.Is(Key.Left))
        {
            MoveSelection(-1);
        }
        else if (key.IsCharacter('l') || key.Is(Key.Right))
        {
            MoveSelection(1);
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        var selected = GetSelectedCard();
        _details.Text = selected is null
            ? "No card selected."
            : $"""
              Id: {selected.Id.ToString(CultureInfo.InvariantCulture)}
              Title: {selected.Title}
              Priority: {selected.Priority}
              Blocked: {(selected.Blocked ? "yes" : "no")}
              Assignee: {selected.Assignee}
              Lane: {_activeLane}
              """;

        _status.LeftText =
            $"{_boardStates[_boards.SelectedIndex].Name}   Todo={_todo.Count} Doing={_doing.Count} Done={_done.Count}";
        _status.RightText = _statusText;

        var lanes = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _todo,
                    Length = LayoutLength.Weighted(1),
                },
                new LayoutSlot
                {
                    Content = _doing,
                    Length = LayoutLength.Weighted(1),
                },
                new LayoutSlot
                {
                    Content = _done,
                    Length = LayoutLength.Weighted(1),
                },
            },
        };

        var sidebar = new ColumnLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _details,
                    Length = 9,
                },
                new LayoutSlot
                {
                    Content = _composer,
                    Length = 5,
                },
                new LayoutSlot
                {
                    Content = _activity,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(1, _boards);
            window.Right(Math.Min(34, Math.Max(28, context.Width / 4)), sidebar);
            window.Body(lanes);
            window.Overlay(new CenterLayout
            {
                Content = _deleteDialog,
                Width = 42,
                Height = 8,
            });
            window.Footer(1, _status);
        });
    }

    private void LoadBoard(int index)
    {
        var board = _boardStates[index];
        _todo.SetItems(board.Todo);
        _doing.SetItems(board.Doing);
        _done.SetItems(board.Done);
        _activeLane = KanbanLane.Todo;
        SelectCard(KanbanLane.Todo, _todo.SelectedItem);
        _statusText = $"Loaded {board.Name}";
    }

    private void SelectCard(KanbanLane lane, KanbanCard? card)
    {
        _activeLane = lane;
        _statusText = card is null ? $"Focused {lane}" : $"Selected {card.Title}";
    }

    private void CreateCard(string raw)
    {
        var title = raw.Trim();
        if (title.Length == 0)
        {
            return;
        }

        var priority = CardPriority.Medium;
        if (title.StartsWith('!'))
        {
            priority = CardPriority.High;
            title = title[1..].Trim();
        }
        else if (title.StartsWith('~'))
        {
            priority = CardPriority.Low;
            title = title[1..].Trim();
        }

        if (title.Length == 0)
        {
            _statusText = "Card title is required.";
            return;
        }

        var board = _boardStates[_boards.SelectedIndex];
        var card = board.CreateCard(title, priority);
        board.Todo.Insert(0, card);
        _todo.SetItems(board.Todo);
        _activeLane = KanbanLane.Todo;
        _activity.Push($"created {card.Title}", NotificationLevel.Success);
        _statusText = $"Created {card.Title}";
    }

    private void OpenDeleteDialog()
    {
        var card = GetSelectedCard();
        if (card is null)
        {
            _statusText = "Nothing selected.";
            return;
        }

        _deleteDialog.Show("Delete Card", $"Delete {card.Title}?", "Enter accepts", "Esc cancels");
    }

    private void ApplyDelete(DialogResult result)
    {
        if (result != DialogResult.Accepted)
        {
            _statusText = "Delete cancelled.";
            return;
        }

        var board = _boardStates[_boards.SelectedIndex];
        var card = GetSelectedCard();
        if (card is null)
        {
            _statusText = "Nothing selected.";
            return;
        }

        GetLaneCollection(board, _activeLane).RemoveAll(existing => existing.Id == card.Id);
        LoadBoard(_boards.SelectedIndex);
        _activity.Push($"deleted {card.Title}", NotificationLevel.Warning);
        _statusText = $"Deleted {card.Title}";
    }

    private void MoveSelection(int delta)
    {
        var board = _boardStates[_boards.SelectedIndex];
        var card = GetSelectedCard();
        if (card is null)
        {
            _statusText = "Nothing selected.";
            return;
        }

        var nextLane = (int)_activeLane + delta;
        if (nextLane < (int)KanbanLane.Todo || nextLane > (int)KanbanLane.Done)
        {
            _statusText = "Card cannot move further.";
            return;
        }

        GetLaneCollection(board, _activeLane).RemoveAll(existing => existing.Id == card.Id);
        var targetLane = (KanbanLane)nextLane;
        GetLaneCollection(board, targetLane).Add(card);
        LoadBoard(_boards.SelectedIndex);
        _activeLane = targetLane;
        _activity.Push($"moved {card.Title} to {targetLane}", NotificationLevel.Info);
        _statusText = $"Moved {card.Title} to {targetLane}";
    }

    private KanbanCard? GetSelectedCard() => _activeLane switch
    {
        KanbanLane.Todo => _todo.SelectedItem,
        KanbanLane.Doing => _doing.SelectedItem,
        KanbanLane.Done => _done.SelectedItem,
        _ => null,
    };

    private static List<KanbanCard> GetLaneCollection(KanbanBoardState board, KanbanLane lane) => lane switch
    {
        KanbanLane.Todo => board.Todo,
        KanbanLane.Doing => board.Doing,
        KanbanLane.Done => board.Done,
        _ => board.Todo,
    };

    private static string CardLabel(KanbanCard card)
    {
        var priority = card.Priority switch
        {
            CardPriority.High => "!",
            CardPriority.Low => "~",
            _ => "-",
        };

        var blocked = card.Blocked ? " [blocked]" : string.Empty;
        return $"{priority} {card.Title}{blocked}";
    }
}
