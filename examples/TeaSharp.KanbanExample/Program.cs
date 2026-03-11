using TeaSharp.Components.Advanced;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Components.Styling;
using TeaSharp.Components.UiKit;
using TeaSharp;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Application;
using TeaSharp.Core.Messages;
using TeaSharp.Styles;
using ModelView = TeaSharp.Core.Abstractions.View;

var program = Tea.NewProgram(new KanbanModel(), new TeaProgramOptions
{
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
    private const int MinimumWidth = 92;
    private const int MinimumHeight = 26;
    private const int MinimumBoardWidth = 54;
    private const int MinimumSidebarWidth = 28;

    private readonly TabsComponent _boardTabs = new(new TabsOptions(["Platform", "Mobile", "Docs"]));

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

    private readonly ListComponent<KanbanCard> _todo = new(new ListOptions<KanbanCard>([], CardLabel, Title: "Todo"));

    private readonly ListComponent<KanbanCard> _doing = new(new ListOptions<KanbanCard>([], CardLabel, Title: "In Progress"));

    private readonly ListComponent<KanbanCard> _done = new(new ListOptions<KanbanCard>([], CardLabel, Title: "Done"));

    private readonly TextInputComponent _composer = new(new TextInputOptions(
        Title: "New Card",
        Placeholder: "Enter title and press Enter (prefix ! high, ~ low)",
        ClearOnSubmit: true,
        ClearOnCancel: true,
        MaxLength: 120));

    private readonly LabelComponent _details = new(new LabelOptions(
        Title: "Selected Card",
        ShowBorder: true));

    private readonly NotificationCenterComponent _activity = new()
    {
        Title = "Activity",
        MaxEntries = 64,
    };

    private readonly ProgressBarComponent _completion = new(new ProgressBarOptions(Title: "Completion"));

    private readonly DialogComponent _deleteDialog = new(new DialogOptions(Title: "Delete Card"));

    private readonly StatusBarComponent _status = new(new StatusBarOptions(
        Theme: new UiTheme(StatusFill: '·')));
    private KanbanFocus _focus = KanbanFocus.Todo;
    private KanbanFocus _focusBeforeComposer = KanbanFocus.Todo;
    private int _width = 128;
    private int _height = 40;
    private string _lastEvent = "ready";
    private PendingDelete? _pendingDelete;
    private int? _boardSplitWidth;
    private bool _draggingBoardSplit;

    public KanbanModel()
    {
        ApplyListPalettes();
        RefreshColumns();
        PushInfo("Kanban ready. Tab focus, h/l move cards, n add, x delete.");

        SetFocus(KanbanFocus.Todo);
    }

    public Command? Init() => null;

    public Command? Update(IMessage message)
    {
        if (message is WindowSizeMsg ws)
        {
            _width = ws.Width;
            _height = ws.Height;
            _lastEvent = $"resize:{_width}x{_height}";
            return null;
        }

        if (message is MouseMsg mouse)
        {
            HandleMouse(mouse);
            return null;
        }

        if (message is not KeyPressMsg key)
        {
            return null;
        }

        if ((key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c'))
            || key.IsCharacter('q', KeyModifiers.None))
        {
            return Tea.Cmd.Quit;
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
                    if (!_deleteDialog.Visible)
                    {
                        _deleteDialog.Focused = false;
                    }
                }
            }

            return null;
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.None))
        {
            CycleFocus(1);
            _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
            return null;
        }

        if (key.Is(KeyCode.Tab, KeyModifiers.Shift))
        {
            CycleFocus(-1);
            _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
            return null;
        }

        // When in composer focus, keep key handling local (including digits, letters,
        // and escape cancel) so global board/lane hotkeys do not fire while typing.
        if (_focus == KanbanFocus.Composer)
        {
            var composerChanged = RouteFocusedInput(key, out var composerEvent);
            if (composerChanged)
            {
                _lastEvent = composerEvent ?? key.Keystroke();
            }

            return null;
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
            return null;
        }

        if (key.IsCharacter('n', KeyModifiers.None))
        {
            EnterComposerFocus();
            _lastEvent = "focus:composer";
            return null;
        }

        if (IsLaneFocus() && key.IsCharacter('x', KeyModifiers.None))
        {
            OpenDeleteDialog();
            return null;
        }

        if (IsLaneFocus() && key.IsCharacter('b', KeyModifiers.None))
        {
            if (ToggleBlocked())
            {
                _lastEvent = "card:block";
            }

            return null;
        }

        if (IsLaneFocus() && key.IsCharacter('p', KeyModifiers.None))
        {
            if (CyclePriority())
            {
                _lastEvent = "card:priority";
            }

            return null;
        }

        if (IsLaneFocus() && key.IsCharacter('u', KeyModifiers.None))
        {
            if (AssignToYou())
            {
                _lastEvent = "card:assign";
            }

            return null;
        }

        if (IsLaneFocus() && key.Is(KeyCode.Right, KeyModifiers.None))
        {
            FocusLane(1);
            _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
            return null;
        }

        if (IsLaneFocus() && key.Is(KeyCode.Left, KeyModifiers.None))
        {
            FocusLane(-1);
            _lastEvent = $"focus:{_focus.ToString().ToLowerInvariant()}";
            return null;
        }

        if (IsLaneFocus() && key.IsCharacter('l', KeyModifiers.None))
        {
            MoveSelectedCard(1);
            return null;
        }

        if (IsLaneFocus() && key.IsCharacter('h', KeyModifiers.None))
        {
            MoveSelectedCard(-1);
            return null;
        }

        var changed = RouteFocusedInput(key, out var eventOverride);

        if (changed)
        {
            _lastEvent = eventOverride ?? key.Keystroke();
        }

        return null;
    }

    public ModelView View()
    {
        var width = Math.Max(MinimumWidth, _width);
        var height = Math.Max(MinimumHeight, _height);
        if (_width < MinimumWidth || _height < MinimumHeight)
        {
            return ModelView.From("TeaSharp Kanban Example\n\nTerminal too small. Expand to at least 92x26.");
        }

        UpdateDetails();
        UpdateCompletion();
        var layout = ComputeLayout(width, height);

        var canvas = new Canvas(width, height, CanvasTextMode.GraphemeAware);
        canvas.Clear();

        canvas.DrawBox(layout.Frame, "TeaSharp Kanban Board", BorderStyle.Rounded);
        _boardTabs.Render(canvas, layout.TabsRect);
        canvas.WriteText(
            layout.HelpRect.X,
            layout.HelpRect.Y,
            "click select | wheel scroll | drag split | ←/→ lane focus | h/l move | b block | p priority | u assign | n new | x delete | q quit",
            layout.HelpRect.Width);

        RenderBoardColumns(canvas, layout);
        RenderSidebar(canvas, layout);

        _deleteDialog.Render(canvas, layout.ContentRect);

        _status.LeftText = $"board={ActiveBoard.Name.ToLowerInvariant()} focus={_focus.ToString().ToLowerInvariant()} todo={ActiveBoard.Todo.Count} doing={ActiveBoard.Doing.Count} done={ActiveBoard.Done.Count}";
        _status.RightText = $"event={_lastEvent}";
        _status.Render(canvas, new Rect(0, height - 1, width, 1));

        return ModelView.From(canvas.Render()) with
        {
            Terminal = new ViewTerminal
            {
                AltScreen = true,
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                MouseMode = MouseMode.AllMotion,
                ForegroundColor = "#CDD6F4",
                BackgroundColor = "#1E1E2E",
                CursorColor = "#F5C2E7",
                WindowTitle = "TeaSharp Kanban Example",
            },
        };
    }

    private KanbanBoardState ActiveBoard => _boards[_boardTabs.SelectedIndex];

    private readonly record struct KanbanLayout(
        Rect Frame,
        Rect TabsRect,
        Rect HelpRect,
        Rect ContentRect,
        Rect BoardRect,
        Rect SideRect,
        Rect TodoRect,
        Rect DoingRect,
        Rect DoneRect,
        Rect ProgressRect,
        Rect DetailsRect,
        Rect ComposerRect,
        Rect ActivityRect);

    private KanbanLayout ComputeLayout(int width, int height)
    {
        var frame = new Rect(0, 0, width, height);
        var body = frame.Inset(1, 1);
        var tabsRect = new Rect(body.X, body.Y, body.Width, 1);
        var helpRect = new Rect(body.X, body.Y + 1, body.Width, 1);
        var contentRect = new Rect(body.X, body.Y + 2, body.Width, body.Height - 3);

        var splitWidth = _boardSplitWidth ?? Math.Max(56, contentRect.Width * 2 / 3);
        var (boardRect, sideRect) = Layout.SplitVertical(contentRect, splitWidth, minFirst: MinimumBoardWidth, minSecond: MinimumSidebarWidth);

        var firstWidth = Math.Max(18, boardRect.Width / 3);
        var (todoRect, laneRest) = Layout.SplitVertical(boardRect, firstWidth, minFirst: 18, minSecond: 36);
        var (doingRect, doneRect) = Layout.SplitVertical(laneRest, Math.Max(18, laneRest.Width / 2), minFirst: 18, minSecond: 18);

        var (progressRect, remainAfterProgress) = Layout.SplitHorizontal(sideRect, 3, minFirst: 3, minSecond: 8);
        var (detailsRect, remainAfterDetails) = Layout.SplitHorizontal(remainAfterProgress, 8, minFirst: 6, minSecond: 8);
        var (composerRect, activityRect) = Layout.SplitHorizontal(remainAfterDetails, 4, minFirst: 4, minSecond: 4);

        return new KanbanLayout(
            Frame: frame,
            TabsRect: tabsRect,
            HelpRect: helpRect,
            ContentRect: contentRect,
            BoardRect: boardRect,
            SideRect: sideRect,
            TodoRect: todoRect,
            DoingRect: doingRect,
            DoneRect: doneRect,
            ProgressRect: progressRect,
            DetailsRect: detailsRect,
            ComposerRect: composerRect,
            ActivityRect: activityRect);
    }

    private bool HandleMouse(MouseMsg mouse)
    {
        if (_deleteDialog.Visible)
        {
            if (mouse is MouseReleaseMsg { Button: MouseButton.Left })
            {
                _draggingBoardSplit = false;
            }

            return false;
        }

        if (_width < MinimumWidth || _height < MinimumHeight)
        {
            return false;
        }

        var layout = ComputeLayout(Math.Max(MinimumWidth, _width), Math.Max(MinimumHeight, _height));
        if (HandleBoardSplitterMouse(mouse, layout))
        {
            return true;
        }

        if (mouse is MouseMotionMsg motion)
        {
            var changed = false;
            changed |= _todo.UpdateMouse(motion, layout.TodoRect);
            changed |= _doing.UpdateMouse(motion, layout.DoingRect);
            changed |= _done.UpdateMouse(motion, layout.DoneRect);
            return changed;
        }

        if (layout.TodoRect.Contains(mouse.X, mouse.Y))
        {
            return HandleLaneMouse(mouse, _todo, layout.TodoRect, KanbanFocus.Todo, "todo");
        }

        if (layout.DoingRect.Contains(mouse.X, mouse.Y))
        {
            return HandleLaneMouse(mouse, _doing, layout.DoingRect, KanbanFocus.Doing, "doing");
        }

        if (layout.DoneRect.Contains(mouse.X, mouse.Y))
        {
            return HandleLaneMouse(mouse, _done, layout.DoneRect, KanbanFocus.Done, "done");
        }

        if (mouse is MouseClickMsg { Button: MouseButton.Left } click)
        {
            if (layout.TabsRect.Contains(click.X, click.Y))
            {
                return TrySelectBoardTabFromMouse(click.X, layout);
            }

            if (layout.ComposerRect.Contains(click.X, click.Y))
            {
                EnterComposerFocus();
                _lastEvent = "focus:composer";
                return true;
            }

            if (layout.ActivityRect.Contains(click.X, click.Y))
            {
                var changed = _focus != KanbanFocus.Activity;
                SetFocus(KanbanFocus.Activity);
                if (changed)
                {
                    _lastEvent = "focus:activity";
                }

                return changed;
            }
        }

        if (mouse is MouseWheelMsg wheel && IsLaneFocus())
        {
            return RouteWheelToFocusedLane(wheel, layout);
        }

        return false;
    }

    private bool HandleBoardSplitterMouse(MouseMsg mouse, in KanbanLayout layout)
    {
        if (mouse is MouseReleaseMsg { Button: MouseButton.Left } && _draggingBoardSplit)
        {
            _draggingBoardSplit = false;
            _lastEvent = $"split:{_boardSplitWidth ?? layout.BoardRect.Width}";
            return true;
        }

        if (mouse is MouseClickMsg { Button: MouseButton.Left } click && IsSplitterHit(click.X, click.Y, layout))
        {
            _draggingBoardSplit = true;
            _lastEvent = "split:drag";
            return true;
        }

        if (mouse is not MouseMotionMsg motion || !_draggingBoardSplit)
        {
            return false;
        }

        var requested = motion.X - layout.ContentRect.X;
        var (boardRect, _) = Layout.SplitVertical(layout.ContentRect, requested, minFirst: MinimumBoardWidth, minSecond: MinimumSidebarWidth);
        var changed = _boardSplitWidth != boardRect.Width;
        _boardSplitWidth = boardRect.Width;
        if (changed)
        {
            _lastEvent = $"split:{boardRect.Width}";
        }

        return true;
    }

    private bool TrySelectBoardTabFromMouse(int x, in KanbanLayout layout)
    {
        var cursor = layout.TabsRect.X;
        for (var i = 0; i < _boardTabs.Tabs.Count && cursor < layout.TabsRect.Right; i++)
        {
            var active = i == _boardTabs.SelectedIndex;
            var label = active
                ? $"[{i + 1}:{_boardTabs.Tabs[i]}]"
                : $" {i + 1}:{_boardTabs.Tabs[i]} ";
            var end = cursor + label.Length;
            if (x >= cursor && x < end)
            {
                var changed = _focus != KanbanFocus.Tabs;
                SetFocus(KanbanFocus.Tabs);

                var before = _boardTabs.SelectedIndex;
                _boardTabs.Select(i);
                if (before != _boardTabs.SelectedIndex)
                {
                    RefreshColumns();
                    PushInfo($"board:{ActiveBoard.Name.ToLowerInvariant()}");
                    changed = true;
                }

                if (changed)
                {
                    _lastEvent = $"board:{_boardTabs.SelectedIndex + 1}";
                }

                return changed;
            }

            cursor = end + 1;
        }

        var focusChanged = _focus != KanbanFocus.Tabs;
        SetFocus(KanbanFocus.Tabs);
        if (focusChanged)
        {
            _lastEvent = "focus:tabs";
        }

        return focusChanged;
    }

    private bool HandleLaneMouse(
        MouseMsg mouse,
        ListComponent<KanbanCard> lane,
        Rect laneRect,
        KanbanFocus laneFocus,
        string laneName)
    {
        if (mouse is MouseWheelMsg)
        {
            lane.UpdateMouse(mouse, laneRect);
            _lastEvent = $"mouse:scroll:{laneName}";
            return true;
        }

        var changed = false;
        if (mouse is MouseClickMsg { Button: MouseButton.Left })
        {
            changed = _focus != laneFocus;
            SetFocus(laneFocus);
        }

        var laneChanged = lane.UpdateMouse(mouse, laneRect);
        changed |= laneChanged;

        if (mouse is MouseClickMsg { Button: MouseButton.Left })
        {
            _lastEvent = mouse switch
            {
                MouseClickMsg => $"mouse:select:{laneName}",
                _ => $"mouse:{laneName}",
            };
        }

        return changed;
    }

    private bool RouteWheelToFocusedLane(MouseWheelMsg wheel, in KanbanLayout layout)
    {
        var changed = _focus switch
        {
            KanbanFocus.Todo => _todo.UpdateMouse(wheel, layout.TodoRect),
            KanbanFocus.Doing => _doing.UpdateMouse(wheel, layout.DoingRect),
            KanbanFocus.Done => _done.UpdateMouse(wheel, layout.DoneRect),
            _ => false,
        };
        if (changed)
        {
            _lastEvent = $"mouse:scroll:{_focus.ToString().ToLowerInvariant()}";
        }

        return changed;
    }

    private static bool IsSplitterHit(int x, int y, in KanbanLayout layout)
    {
        return layout.ContentRect.Contains(x, y) && x == layout.SideRect.X;
    }

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
            var beforeCancel = _composer.CancelCount;
            var changed = _composer.Update(key);
            if (_composer.CancelCount != beforeCancel)
            {
                SetFocus(_focusBeforeComposer);
                eventOverride = $"focus:{_focus.ToString().ToLowerInvariant()}";
                return true;
            }

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

    private void RenderBoardColumns(Canvas canvas, in KanbanLayout layout)
    {
        _todo.Render(canvas, layout.TodoRect);
        _doing.Render(canvas, layout.DoingRect);
        _done.Render(canvas, layout.DoneRect);
    }

    private void RenderSidebar(Canvas canvas, in KanbanLayout layout)
    {
        _completion.Render(canvas, layout.ProgressRect);
        _details.Render(canvas, layout.DetailsRect);
        _composer.Render(canvas, layout.ComposerRect);
        _activity.Render(canvas, layout.ActivityRect);
    }

    private void RefreshColumns()
    {
        _todo.SetItems(ActiveBoard.Todo);
        _doing.SetItems(ActiveBoard.Doing);
        _done.SetItems(ActiveBoard.Done);
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
        SetFocus(KanbanFocus.Composer);
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
            _deleteDialog.Focused = false;
            return;
        }

        var pending = _pendingDelete;
        RemoveCard(pending.Lane, pending.CardId);
        RefreshColumns();
        _deleteDialog.Visible = false;
        _deleteDialog.Focused = false;
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
            var selected = _todo.SelectedItem;
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
            var selected = _doing.SelectedItem;
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
            var selected = _done.SelectedItem;
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
            SetFocus(KanbanFocus.Todo);
            return;
        }

        var next = (index + delta) % values.Length;
        if (next < 0)
        {
            next += values.Length;
        }

        SetFocus(values[next]);
    }

    private void EnterComposerFocus()
    {
        if (_focus != KanbanFocus.Composer)
        {
            _focusBeforeComposer = _focus;
        }

        SetFocus(KanbanFocus.Composer);
    }

    private bool IsLaneFocus()
    {
        return _focus is KanbanFocus.Todo or KanbanFocus.Doing or KanbanFocus.Done;
    }

    private void FocusLane(int delta)
    {
        if (!IsLaneFocus())
        {
            return;
        }

        SetFocus((delta, _focus) switch
        {
            (< 0, KanbanFocus.Doing) => KanbanFocus.Todo,
            (< 0, KanbanFocus.Done) => KanbanFocus.Doing,
            (> 0, KanbanFocus.Todo) => KanbanFocus.Doing,
            (> 0, KanbanFocus.Doing) => KanbanFocus.Done,
            _ => _focus,
        });
    }

    private void SetFocus(KanbanFocus focus)
    {
        _focus = focus;
        _todo.Focused = focus == KanbanFocus.Todo;
        _doing.Focused = focus == KanbanFocus.Doing;
        _done.Focused = focus == KanbanFocus.Done;
        _composer.Focused = focus == KanbanFocus.Composer;
        _activity.Focused = focus == KanbanFocus.Activity;
        _completion.Focused = false;
        if (!_deleteDialog.Visible)
        {
            _deleteDialog.Focused = false;
        }
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

        return _todo.SelectedItem
            ?? _doing.SelectedItem
            ?? _done.SelectedItem;
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
