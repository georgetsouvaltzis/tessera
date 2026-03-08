using TeaSharp;
using TeaSharp.Components;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Core.Terminal;
using TeaSharp.Styles;
using ModelView = TeaSharp.Core.Abstractions.View;

var terminal = new ConsoleTerminalAdapter();
var program = Tea.NewProgram(new KanbanModel(), new ProgramOptions
{
    Terminal = terminal,
    UseConsoleKeyEvents = false,
});

try
{
    await program.RunAsync();
    return 0;
}
catch (TeaProgramInterruptedException)
{
    return 130;
}

internal enum KanbanFocus
{
    Tabs = 0,
    Todo = 1,
    Doing = 2,
    Done = 3,
    Composer = 4,
    Activity = 5,
}

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

        var maxId = 0;
        foreach (var id in Todo.Concat(Doing).Concat(Done).Select(card => card.Id))
        {
            if (id > maxId)
            {
                maxId = id;
            }
        }

        _nextId = maxId + 1;
    }

    public string Name { get; }

    public List<KanbanCard> Todo { get; }

    public List<KanbanCard> Doing { get; }

    public List<KanbanCard> Done { get; }

    public KanbanCard CreateCard(string title, CardPriority priority)
    {
        return new KanbanCard(_nextId++, title, priority);
    }
}

internal sealed record PendingDelete(KanbanLane Lane, int CardId, string Title);

internal sealed class KanbanModel : IModel
{
    private readonly TabsComponent _boardTabs = new(["Platform", "Mobile", "Docs"]);

    private readonly List<KanbanBoardState> _boards =
    [
        new(
            "Platform",
            [
                new KanbanCard(101, "Add plugin API", CardPriority.High),
                new KanbanCard(102, "Refactor keymap docs", CardPriority.Medium),
                new KanbanCard(103, "Design release notes layout", CardPriority.Low),
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

    private readonly ListComponent<KanbanCard> _todo = new([], CardLabel)
    {
        Title = "Todo",
    };

    private readonly ListComponent<KanbanCard> _doing = new([], CardLabel)
    {
        Title = "In Progress",
    };

    private readonly ListComponent<KanbanCard> _done = new([], CardLabel)
    {
        Title = "Done",
    };

    private readonly TextInputComponent _composer = new()
    {
        Title = "New Card",
        ClearOnSubmit = true,
    };

    private readonly LabelComponent _details = new()
    {
        Title = "Selected Card",
        DrawBorder = true,
    };

    private readonly NotificationCenterComponent _activity = new()
    {
        Title = "Activity",
        MaxEntries = 64,
    };

    private readonly ProgressBarComponent _completion = new()
    {
        Title = "Completion",
    };

    private readonly DialogComponent _deleteDialog = new()
    {
        Title = "Delete Card",
    };

    private readonly StatusBarComponent _status = new()
    {
        Theme = new UiTheme(StatusFill: '·'),
    };

    private KanbanFocus _focus = KanbanFocus.Todo;
    private int _width = 128;
    private int _height = 40;
    private string _lastEvent = "ready";
    private PendingDelete? _pendingDelete;

    public KanbanModel()
    {
        _composer.Input.Placeholder = "Enter title and press Enter (prefix ! high, ~ low)";
        _composer.Input.MaxLength = 120;

        ApplyListPalettes();
        RefreshColumns();
        PushInfo("Kanban ready. Tab focus, h/l move cards, n add, x delete.");
    }

    public Command? Init() => null;

    public UpdateResult Update(IMessage message)
    {
        if (message is WindowSizeMsg ws)
        {
            _width = ws.Width;
            _height = ws.Height;
            _lastEvent = $"resize:{_width}x{_height}";
            return new UpdateResult(this, null);
        }

        if (message is not KeyPressMsg key)
        {
            return new UpdateResult(this, null);
        }

        if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c'))
            || key.IsCharacter('q', KeyModifiers.None))
        {
            return new UpdateResult(this, Tea.Cmd.Quit);
        }

        if (_deleteDialog.Visible)
        {
            if (_deleteDialog.Update(key))
            {
                if (_deleteDialog.LastResult == DialogResult.Accepted)
                {
                    CommitDelete();
                }
                else
                {
                    _pendingDelete = null;
                    _lastEvent = "delete:cancelled";
                }
            }

            return new UpdateResult(this, null);
        }

        if (key.TryGetDigit(out var oneBased)
            && oneBased >= 1
            && oneBased <= _boardTabs.Tabs.Count)
        {
            var before = _boardTabs.SelectedIndex;
            _boardTabs.Select(oneBased - 1);
            if (before != _boardTabs.SelectedIndex)
            {
                RefreshColumns();
                PushInfo($"board:{ActiveBoard.Name.ToLowerInvariant()}");
            }

            _lastEvent = $"board:{_boardTabs.SelectedIndex + 1}";
            return new UpdateResult(this, null);
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.None))
        {
            CycleFocus(1);
            _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
            return new UpdateResult(this, null);
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.Shift))
        {
            CycleFocus(-1);
            _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
            return new UpdateResult(this, null);
        }

        if (key.IsCharacter('n', KeyModifiers.None))
        {
            _focus = KanbanFocus.Composer;
            _lastEvent = "focus:composer";
            return new UpdateResult(this, null);
        }

        if (IsLaneFocus() && key.IsCharacter('x', KeyModifiers.None))
        {
            OpenDeleteDialog();
            return new UpdateResult(this, null);
        }

        if (IsLaneFocus() && key.IsCharacter('b', KeyModifiers.None))
        {
            if (ToggleBlocked())
            {
                _lastEvent = "card:block";
            }

            return new UpdateResult(this, null);
        }

        if (IsLaneFocus() && key.IsCharacter('p', KeyModifiers.None))
        {
            if (CyclePriority())
            {
                _lastEvent = "card:priority";
            }

            return new UpdateResult(this, null);
        }

        if (IsLaneFocus() && key.IsCharacter('u', KeyModifiers.None))
        {
            if (AssignToYou())
            {
                _lastEvent = "card:assign";
            }

            return new UpdateResult(this, null);
        }

        if (IsLaneFocus() && (key.Is(KeyCode.Right, KeyModifiers.None) || key.IsCharacter('l', KeyModifiers.None)))
        {
            MoveSelectedCard(1);
            return new UpdateResult(this, null);
        }

        if (IsLaneFocus() && (key.Is(KeyCode.Left, KeyModifiers.None) || key.IsCharacter('h', KeyModifiers.None)))
        {
            MoveSelectedCard(-1);
            return new UpdateResult(this, null);
        }

        var changed = RouteFocusedInput(key, out var eventOverride);

        if (changed)
        {
            _lastEvent = eventOverride ?? key.Keystroke();
        }

        return new UpdateResult(this, null);
    }

    public ModelView View()
    {
        var width = Math.Max(92, _width);
        var height = Math.Max(26, _height);
        if (_width < 92 || _height < 26)
        {
            return ModelView.From("TeaSharp Kanban Example\n\nTerminal too small. Expand to at least 92x26.");
        }

        ApplyFocusFlags();
        UpdateDetails();
        UpdateCompletion();

        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        var frame = new Rect(0, 0, width, height);
        canvas.DrawBox(frame, "TeaSharp Kanban Board", BorderStyle.Rounded);

        var body = frame.Inset(1, 1);
        _boardTabs.Render(canvas, new Rect(body.X, body.Y, body.Width, 1));
        canvas.WriteText(body.X, body.Y + 1, "tab focus | h/l move | b block | p priority | u assign | n new | x delete | q quit", body.Width);

        var content = new Rect(body.X, body.Y + 2, body.Width, body.Height - 3);
        var (boardRect, sideRect) = Layout.SplitVertical(content, Math.Max(56, content.Width * 2 / 3), minFirst: 54, minSecond: 28);

        RenderBoardColumns(canvas, boardRect);
        RenderSidebar(canvas, sideRect);

        _deleteDialog.Render(canvas, content);

        _status.LeftText = $"board={ActiveBoard.Name.ToLowerInvariant()} focus={_focus.ToString().ToLowerInvariant()} todo={ActiveBoard.Todo.Count} doing={ActiveBoard.Doing.Count} done={ActiveBoard.Done.Count}";
        _status.RightText = $"event={_lastEvent}";
        _status.Render(canvas, new Rect(0, height - 1, width, 1));

        return ModelView.From(canvas.Render()) with
        {
            AltScreen = true,
            EnableBracketedPaste = true,
            EnableFocusReporting = true,
            MouseMode = MouseMode.AllMotion,
            ForegroundColor = "#CDD6F4",
            BackgroundColor = "#1E1E2E",
            CursorColor = "#F5C2E7",
            WindowTitle = "TeaSharp Kanban Example",
        };
    }

    private KanbanBoardState ActiveBoard => _boards[_boardTabs.SelectedIndex];

    private bool RouteFocusedInput(KeyPressMsg key, out string? eventOverride)
    {
        eventOverride = null;

        if (_focus == KanbanFocus.Tabs)
        {
            var before = _boardTabs.SelectedIndex;
            var changed = _boardTabs.Update(key);
            if (before != _boardTabs.SelectedIndex)
            {
                RefreshColumns();
                PushInfo($"board:{ActiveBoard.Name.ToLowerInvariant()}");
            }

            if (changed)
            {
                eventOverride = $"board:{_boardTabs.SelectedIndex + 1}";
            }

            return changed;
        }

        if (_focus == KanbanFocus.Composer)
        {
            var beforeSubmit = _composer.SubmitCount;
            var changed = _composer.Update(key);
            if (_composer.SubmitCount != beforeSubmit)
            {
                CreateCardFromComposer();
                eventOverride = _lastEvent;
                return true;
            }

            return changed;
        }

        return _focus switch
        {
            KanbanFocus.Todo => _todo.Update(key),
            KanbanFocus.Doing => _doing.Update(key),
            KanbanFocus.Done => _done.Update(key),
            KanbanFocus.Activity => UpdateActivity(key, out eventOverride),
            _ => false,
        };
    }

    private bool UpdateActivity(KeyPressMsg key, out string? eventOverride)
    {
        eventOverride = null;
        var changed = _activity.Update(key);
        if (!changed)
        {
            return false;
        }

        if (_activity.MarkReadKey.Matches(key))
        {
            eventOverride = "activity:mark-read";
        }
        else if (_activity.DismissKey.Matches(key))
        {
            eventOverride = "activity:dismiss";
        }
        else if (_activity.ClearAllKey.Matches(key))
        {
            eventOverride = "activity:clear";
        }
        else if (_activity.NextItemKey.Matches(key) || _activity.PreviousItemKey.Matches(key))
        {
            eventOverride = "activity:navigate";
        }

        return true;
    }

    private void RenderBoardColumns(Canvas canvas, Rect rect)
    {
        var firstWidth = Math.Max(18, rect.Width / 3);
        var (todoRect, rest) = Layout.SplitVertical(rect, firstWidth, minFirst: 18, minSecond: 36);
        var (doingRect, doneRect) = Layout.SplitVertical(rest, Math.Max(18, rest.Width / 2), minFirst: 18, minSecond: 18);

        _todo.Render(canvas, todoRect);
        _doing.Render(canvas, doingRect);
        _done.Render(canvas, doneRect);
    }

    private void RenderSidebar(Canvas canvas, Rect rect)
    {
        var (progressRect, remainAfterProgress) = Layout.SplitHorizontal(rect, 3, minFirst: 3, minSecond: 8);
        _completion.Render(canvas, progressRect);

        var (detailsRect, remainAfterDetails) = Layout.SplitHorizontal(remainAfterProgress, 8, minFirst: 6, minSecond: 8);
        _details.Render(canvas, detailsRect);

        var (composerRect, activityRect) = Layout.SplitHorizontal(remainAfterDetails, 4, minFirst: 4, minSecond: 4);
        _composer.Render(canvas, composerRect);
        _activity.Render(canvas, activityRect);
    }

    private void RefreshColumns()
    {
        _todo.Model.SetItems(ActiveBoard.Todo);
        _doing.Model.SetItems(ActiveBoard.Doing);
        _done.Model.SetItems(ActiveBoard.Done);
    }

    private void ApplyFocusFlags()
    {
        _todo.Focused = _focus == KanbanFocus.Todo;
        _doing.Focused = _focus == KanbanFocus.Doing;
        _done.Focused = _focus == KanbanFocus.Done;
        _composer.Focused = _focus == KanbanFocus.Composer;
        _activity.Focused = _focus == KanbanFocus.Activity;
        _completion.Focused = false;
        _deleteDialog.Focused = _deleteDialog.Visible;
    }

    private void ApplyListPalettes()
    {
        var basePalette = WidgetStatePalette.CreateDefault();
        basePalette[WidgetVisualState.Completed] = new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithStrikethrough().WithForeground(AnsiColor.BrightGreen),
            Prefix = "[x] ",
        };
        basePalette[WidgetVisualState.Error] = new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightRed),
            Prefix = "! ",
        };
        basePalette[WidgetVisualState.Warning] = new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightYellow),
            Prefix = "▲ ",
        };
        basePalette[WidgetVisualState.New] = new WidgetStateAppearance
        {
            TextStyle = TeaStyle.Empty.WithForeground(AnsiColor.BrightCyan),
        };

        _todo.ItemStatePalette.InheritFrom(basePalette);
        _doing.ItemStatePalette.InheritFrom(basePalette);
        _done.ItemStatePalette.InheritFrom(basePalette);

        _todo.ItemStateResolver = card => ResolveCardStates(card, KanbanLane.Todo);
        _doing.ItemStateResolver = card => ResolveCardStates(card, KanbanLane.Doing);
        _done.ItemStateResolver = card => ResolveCardStates(card, KanbanLane.Done);
    }

    private static IReadOnlyCollection<WidgetVisualState> ResolveCardStates(KanbanCard card, KanbanLane lane)
    {
        var states = new List<WidgetVisualState>(5);
        if (lane == KanbanLane.Done)
        {
            states.Add(WidgetVisualState.Completed);
        }

        if (card.Blocked)
        {
            states.Add(WidgetVisualState.Error);
        }

        if (card.Priority == CardPriority.High)
        {
            states.Add(WidgetVisualState.Warning);
        }
        else if (card.Priority == CardPriority.Low)
        {
            states.Add(WidgetVisualState.New);
        }

        return states;
    }

    private static string CardLabel(KanbanCard card)
    {
        var priority = card.Priority switch
        {
            CardPriority.High => "H",
            CardPriority.Low => "L",
            _ => "M",
        };
        var blocked = card.Blocked ? "[blocked] " : string.Empty;
        return $"#{card.Id} [{priority}] {blocked}{card.Title}";
    }

    private void CreateCardFromComposer()
    {
        var raw = _composer.LastSubmittedValue.Trim();
        if (string.IsNullOrWhiteSpace(raw))
        {
            return;
        }

        var priority = CardPriority.Medium;
        if (raw.StartsWith('!'))
        {
            priority = CardPriority.High;
            raw = raw[1..].Trim();
        }
        else if (raw.StartsWith('~'))
        {
            priority = CardPriority.Low;
            raw = raw[1..].Trim();
        }

        if (string.IsNullOrWhiteSpace(raw))
        {
            PushWarning("card title is empty");
            return;
        }

        var card = ActiveBoard.CreateCard(raw, priority);
        ActiveBoard.Todo.Insert(0, card);
        RefreshColumns();
        _focus = KanbanFocus.Composer;
        _lastEvent = $"card:new:{card.Id}";
        PushSuccess($"added #{card.Id} to todo");
    }

    private bool ToggleBlocked()
    {
        if (!TryGetSelected(out var lane, out var selected))
        {
            PushWarning("no card selected");
            return false;
        }

        var updated = selected with { Blocked = !selected.Blocked };
        ReplaceCard(lane, updated);
        RefreshColumns();
        PushInfo(updated.Blocked
            ? $"#{updated.Id} marked blocked"
            : $"#{updated.Id} unblocked");
        return true;
    }

    private bool CyclePriority()
    {
        if (!TryGetSelected(out var lane, out var selected))
        {
            PushWarning("no card selected");
            return false;
        }

        var next = selected.Priority switch
        {
            CardPriority.Low => CardPriority.Medium,
            CardPriority.Medium => CardPriority.High,
            _ => CardPriority.Low,
        };

        var updated = selected with { Priority = next };
        ReplaceCard(lane, updated);
        RefreshColumns();
        PushInfo($"#{updated.Id} priority:{next.ToString().ToLowerInvariant()}");
        return true;
    }

    private bool AssignToYou()
    {
        if (!TryGetSelected(out var lane, out var selected))
        {
            PushWarning("no card selected");
            return false;
        }

        var updated = selected with
        {
            Assignee = string.Equals(selected.Assignee, "you", StringComparison.Ordinal)
                ? "unassigned"
                : "you",
        };
        ReplaceCard(lane, updated);
        RefreshColumns();
        PushInfo($"#{updated.Id} assignee:{updated.Assignee}");
        return true;
    }

    private void MoveSelectedCard(int direction)
    {
        if (!TryGetSelected(out var lane, out var selected))
        {
            PushWarning("no card selected");
            return;
        }

        var target = direction switch
        {
            < 0 when lane == KanbanLane.Doing => KanbanLane.Todo,
            < 0 when lane == KanbanLane.Done => KanbanLane.Doing,
            > 0 when lane == KanbanLane.Todo => KanbanLane.Doing,
            > 0 when lane == KanbanLane.Doing => KanbanLane.Done,
            _ => lane,
        };

        if (target == lane)
        {
            PushWarning("cannot move beyond board edge");
            return;
        }

        RemoveCard(lane, selected.Id);
        LaneItems(target).Add(selected);
        RefreshColumns();

        _focus = target switch
        {
            KanbanLane.Todo => KanbanFocus.Todo,
            KanbanLane.Doing => KanbanFocus.Doing,
            _ => KanbanFocus.Done,
        };

        _lastEvent = $"move:{selected.Id}:{lane.ToString().ToLowerInvariant()}->{target.ToString().ToLowerInvariant()}";
        PushSuccess($"moved #{selected.Id} to {target.ToString().ToLowerInvariant()}");
    }

    private void OpenDeleteDialog()
    {
        if (!TryGetSelected(out var lane, out var selected))
        {
            PushWarning("no card selected");
            return;
        }

        _pendingDelete = new PendingDelete(lane, selected.Id, selected.Title);
        _deleteDialog.Lines =
        [
            $"Delete card #{selected.Id}?",
            selected.Title,
            "Enter/Space to confirm",
            "Esc to cancel",
        ];
        _deleteDialog.Visible = true;
        _deleteDialog.Focused = true;
        _lastEvent = "delete:confirm";
    }

    private void CommitDelete()
    {
        if (_pendingDelete is null)
        {
            _deleteDialog.Visible = false;
            return;
        }

        var pending = _pendingDelete;
        RemoveCard(pending.Lane, pending.CardId);
        RefreshColumns();
        _deleteDialog.Visible = false;
        _pendingDelete = null;
        _lastEvent = $"delete:{pending.CardId}";
        PushInfo($"deleted #{pending.CardId}");
    }

    private bool TryGetSelected(out KanbanLane lane, out KanbanCard card)
    {
        lane = KanbanLane.Todo;
        card = null!;

        if (_focus == KanbanFocus.Todo)
        {
            var selected = _todo.Model.SelectedItem;
            if (selected is null)
            {
                return false;
            }

            lane = KanbanLane.Todo;
            card = selected;
            return true;
        }

        if (_focus == KanbanFocus.Doing)
        {
            var selected = _doing.Model.SelectedItem;
            if (selected is null)
            {
                return false;
            }

            lane = KanbanLane.Doing;
            card = selected;
            return true;
        }

        if (_focus == KanbanFocus.Done)
        {
            var selected = _done.Model.SelectedItem;
            if (selected is null)
            {
                return false;
            }

            lane = KanbanLane.Done;
            card = selected;
            return true;
        }

        return false;
    }

    private void ReplaceCard(KanbanLane lane, KanbanCard updated)
    {
        var items = LaneItems(lane);
        var index = items.FindIndex(card => card.Id == updated.Id);
        if (index >= 0)
        {
            items[index] = updated;
        }
    }

    private void RemoveCard(KanbanLane lane, int cardId)
    {
        var items = LaneItems(lane);
        var index = items.FindIndex(card => card.Id == cardId);
        if (index >= 0)
        {
            items.RemoveAt(index);
        }
    }

    private List<KanbanCard> LaneItems(KanbanLane lane)
    {
        return lane switch
        {
            KanbanLane.Todo => ActiveBoard.Todo,
            KanbanLane.Doing => ActiveBoard.Doing,
            _ => ActiveBoard.Done,
        };
    }

    private void CycleFocus(int delta)
    {
        var values = (KanbanFocus[])Enum.GetValues(typeof(KanbanFocus));
        var index = Array.IndexOf(values, _focus);
        if (index < 0)
        {
            _focus = KanbanFocus.Todo;
            return;
        }

        var next = (index + delta) % values.Length;
        if (next < 0)
        {
            next += values.Length;
        }

        _focus = values[next];
    }

    private bool IsLaneFocus()
    {
        return _focus is KanbanFocus.Todo or KanbanFocus.Doing or KanbanFocus.Done;
    }

    private void UpdateDetails()
    {
        var selected = ResolveSelectedForDetails();
        if (selected is null)
        {
            _details.Text = "No card selected\n\nUse Up/Down in a lane to select.";
            return;
        }

        var priority = selected.Priority.ToString().ToLowerInvariant();
        var blocked = selected.Blocked ? "yes" : "no";
        _details.Text =
            $"id: #{selected.Id}\n" +
            $"title: {selected.Title}\n" +
            $"priority: {priority}\n" +
            $"blocked: {blocked}\n" +
            $"assignee: {selected.Assignee}\n";
    }

    private KanbanCard? ResolveSelectedForDetails()
    {
        if (TryGetSelected(out _, out var selected))
        {
            return selected;
        }

        return _todo.Model.SelectedItem
            ?? _doing.Model.SelectedItem
            ?? _done.Model.SelectedItem;
    }

    private void UpdateCompletion()
    {
        var total = ActiveBoard.Todo.Count + ActiveBoard.Doing.Count + ActiveBoard.Done.Count;
        if (total == 0)
        {
            _completion.SetValue(0);
            return;
        }

        _completion.SetValue((double)ActiveBoard.Done.Count / total);
    }

    private void PushInfo(string message)
    {
        _activity.Push(message, NotificationSeverity.Info);
    }

    private void PushSuccess(string message)
    {
        _activity.Push(message, NotificationSeverity.Success);
    }

    private void PushWarning(string message)
    {
        _activity.Push(message, NotificationSeverity.Warning);
    }
}
