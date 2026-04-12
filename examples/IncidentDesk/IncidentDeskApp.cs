using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;
using System.Text;

namespace Tessera.Examples.IncidentDesk;

internal sealed partial class IncidentDeskApp : TesseraApp
{
    private readonly TesseraTheme _theme = IncidentDeskTheme.DefaultTheme;
    private readonly IncidentDeskState _state = IncidentDeskState.CreateSeed();

    private readonly IncidentHeroControl _hero = new() { Title = "Incident Command Deck", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly Label _queuePulse = new() { Title = "Open Queue", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly Label _escalationPulse = new() { Title = "Escalation Pressure", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly Label _crewPulse = new() { Title = "Crew Footprint", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly NotificationInbox _queue = new() { Title = "Incident Queue · F1", Padding = Thickness.Symmetric(1, 0), FocusMarker = "◈", ShowSource = true, ShowTimestamp = true };
    private readonly Label _briefing = new() { Title = "Incident Briefing", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly ActivityFeed _timeline = new() { Title = "Event Narrative · F2", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◈", ShowTimestamp = true };
    private readonly Label _responderCard = new() { Title = "Responder Lane", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly TextArea _notes = new() { Title = "Operator Notes · F3", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◈", Wrap = true, ShowLineNumbers = false };
    private readonly Button _ackButton = new() { Text = "Acknowledge", Description = "a · take command", Padding = Thickness.All(1) };
    private readonly Button _assignButton = new() { Text = "Assign", Description = "g · cycle owner", Padding = Thickness.All(1) };
    private readonly Button _escalateButton = new() { Text = "Escalate", Description = "e · raise severity", Padding = Thickness.All(1) };
    private readonly Button _resolveButton = new() { Text = "Resolve", Description = "v · verify recovery", Padding = Thickness.All(1) };
    private readonly Button _reopenButton = new() { Text = "Reopen", Description = "o · restore active mode", Padding = Thickness.All(1) };
    private readonly Button _syncButton = new() { Text = "Sync", Description = "s · refresh telemetry", Padding = Thickness.All(1) };
    private readonly LogTailPanel _logs = new() { Title = "Live Telemetry · F4", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◈", ShowTimestamp = true, ShowSource = true, ShowLevel = true, AutoFollow = true };
    private readonly StatusBar _footer = new() { Fill = ' ' };

    private bool _syncingQueueSelection;

    public IncidentDeskApp()
    {
        ConfigureTheme();
        WireEvents();
        SeedControls();
        _queue.RequestFocus();
    }

    public override TesseraEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key:
                return HandleKey(key);
            case IncidentDeskActionMessage action:
                ExecuteAction(action.Action);
                return null;
            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshChrome(context);
        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            ConfigureHeader(window, context);
            window.Body(body => ConfigureBody(body, context));
            window.Footer(1, _footer);
        });
    }

    private TesseraEffect? HandleKey(KeyPressed key)
    {
        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TesseraEffects.Quit;
        }

        if (key.Is(Key.F1))
        {
            _queue.RequestFocus();
            return null;
        }

        if (key.Is(Key.F2))
        {
            _timeline.RequestFocus();
            return null;
        }

        if (key.Is(Key.F3))
        {
            _notes.RequestFocus();
            return null;
        }

        if (key.Is(Key.F4))
        {
            _logs.RequestFocus();
            return null;
        }

        if (IsEditingNotes())
        {
            return null;
        }

        if (key.IsCharacter('a'))
        {
            ExecuteAction(IncidentDeskAction.Acknowledge);
            return null;
        }

        if (key.IsCharacter('g'))
        {
            ExecuteAction(IncidentDeskAction.Assign);
            return null;
        }

        if (key.IsCharacter('e'))
        {
            ExecuteAction(IncidentDeskAction.Escalate);
            return null;
        }

        if (key.IsCharacter('v'))
        {
            ExecuteAction(IncidentDeskAction.Resolve);
            return null;
        }

        if (key.IsCharacter('o'))
        {
            ExecuteAction(IncidentDeskAction.Reopen);
            return null;
        }

        if (key.IsCharacter('s'))
        {
            ExecuteAction(IncidentDeskAction.Sync);
            return null;
        }

        return null;
    }

    private void WireEvents()
    {
        _queue.SelectionChanged += (_, args) =>
        {
            if (_syncingQueueSelection || args.SelectedItem is null)
            {
                return;
            }

            _state.CaptureDraft(_notes.Value);
            if (_state.SelectIncident(args.SelectedItem.Id))
            {
                RefreshSelectionState();
            }
        };

        _ackButton.Activated += (_, _) => Post(new IncidentDeskActionMessage(IncidentDeskAction.Acknowledge));
        _assignButton.Activated += (_, _) => Post(new IncidentDeskActionMessage(IncidentDeskAction.Assign));
        _escalateButton.Activated += (_, _) => Post(new IncidentDeskActionMessage(IncidentDeskAction.Escalate));
        _resolveButton.Activated += (_, _) => Post(new IncidentDeskActionMessage(IncidentDeskAction.Resolve));
        _reopenButton.Activated += (_, _) => Post(new IncidentDeskActionMessage(IncidentDeskAction.Reopen));
        _syncButton.Activated += (_, _) => Post(new IncidentDeskActionMessage(IncidentDeskAction.Sync));
    }

    private void SeedControls()
    {
        _timeline.MaxItems = 64;
        _timeline.SelectedMarker = "◆";
        _timeline.UnselectedMarker = "·";
        _timeline.UnreadMarker = "●";
        _logs.MaxEntries = 128;
        _logs.SelectedMarker = "◆";
        RefreshSelectionState();
    }

    private void ConfigureTheme()
    {
        _queue.ApplyTheme(_theme);
        _timeline.ApplyTheme(_theme);
        _logs.ApplyTheme(_theme);
        _briefing.ApplyTheme(_theme);
        _responderCard.ApplyTheme(_theme);
        _notes.ApplyTheme(_theme);
        _queuePulse.ApplyTheme(_theme);
        _escalationPulse.ApplyTheme(_theme);
        _crewPulse.ApplyTheme(_theme);
        _ackButton.ApplyTheme(_theme);
        _assignButton.ApplyTheme(_theme);
        _escalateButton.ApplyTheme(_theme);
        _resolveButton.ApplyTheme(_theme);
        _reopenButton.ApplyTheme(_theme);
        _syncButton.ApplyTheme(_theme);
        _footer.ApplyTheme(_theme);

        _hero.TitleStyle = _theme.Text.Secondary.WithBold();
        _hero.BorderStyleText = _theme.Border.Strong;
        _hero.SummaryStyle = _theme.Text.Primary.WithBold();
        _hero.MetaStyle = _theme.Text.Secondary;
        _hero.DetailStyle = _theme.Text.Muted;
        _hero.HighlightStyle = _theme.Accent.Primary.WithBold();
        _hero.SeverityStyle = IncidentDeskTheme.Chip(0xFFF4E8, 0x8D3228);
        _hero.StatusStyle = IncidentDeskTheme.Chip(0xFEE9D0, 0x5A4334);
        _hero.PhaseStyle = _theme.Accent.Secondary.WithBold();

        ConfigurePulseCard(_queuePulse, _theme.Accent.Primary.WithBold());
        ConfigurePulseCard(_escalationPulse, _theme.State.Warning.WithBold());
        ConfigurePulseCard(_crewPulse, _theme.State.Info.WithBold());

        _queue.TitleStyle = _theme.Text.Secondary.WithBold();
        _queue.FocusedTitleStyle = _theme.Focus.Title;
        _queue.ItemStyle = _theme.Text.Primary;
        _queue.UnreadItemStyle = _theme.Text.Primary.WithBold();
        _queue.MutedItemStyle = _theme.Text.Muted;
        _queue.InfoItemStyle = _theme.State.Info;
        _queue.SuccessItemStyle = _theme.State.Success;
        _queue.WarningItemStyle = _theme.State.Warning;
        _queue.ErrorItemStyle = _theme.State.Error;
        _queue.SelectedItemStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();
        _queue.HoveredItemStyle = _theme.Accent.Primary;
        _queue.PinnedItemStyle = IncidentDeskTheme.Foreground(0xF3B276).WithBold();
        _queue.EmptyTextStyle = _theme.Text.Muted;
        _queue.SelectedMarker = "◆";
        _queue.UnselectedMarker = "·";

        _briefing.TitleStyle = _theme.Text.Secondary.WithBold();
        _briefing.FocusedTitleStyle = _theme.Focus.Title;
        _briefing.BorderStyleText = _theme.Border.Strong;
        _briefing.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _briefing.TextStyle = _theme.Text.Primary;

        _timeline.TitleStyle = _theme.Text.Secondary.WithBold();
        _timeline.FocusedTitleStyle = _theme.Focus.Title;
        _timeline.BorderStyleText = _theme.Border.Strong;
        _timeline.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _timeline.InfoItemStyle = _theme.Text.Secondary;
        _timeline.SuccessItemStyle = _theme.State.Success;
        _timeline.WarningItemStyle = _theme.State.Warning;
        _timeline.ErrorItemStyle = _theme.State.Error;
        _timeline.SelectedItemStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();
        _timeline.FocusedSelectedItemStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();
        _timeline.HoveredItemStyle = _theme.Accent.Primary;
        _timeline.TimestampStyle = _theme.Text.Muted;

        _responderCard.TitleStyle = _theme.Text.Secondary.WithBold();
        _responderCard.FocusedTitleStyle = _theme.Focus.Title;
        _responderCard.BorderStyleText = _theme.Border.Strong;
        _responderCard.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _responderCard.TextStyle = _theme.Text.Primary;

        _notes.ValueTextStyle = _theme.Text.Primary;
        _notes.TitleStyle = _theme.Text.Secondary.WithBold();
        _notes.FocusedTitleStyle = _theme.Focus.Title;
        _notes.BorderStyleText = _theme.Border.Strong;
        _notes.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);

        ConfigureActionButton(_ackButton, 0xF4F7E9, 0x35533A);
        ConfigureActionButton(_assignButton, 0xFFF4E8, 0x604236);
        ConfigureActionButton(_escalateButton, 0xFFF4E8, 0x8D3228);
        ConfigureActionButton(_resolveButton, 0xF4F7E9, 0x2C604B);
        ConfigureActionButton(_reopenButton, 0xFFF4E8, 0x6F4A33);
        ConfigureActionButton(_syncButton, 0xF3F7FF, 0x3F5165);

        _logs.TitleStyle = _theme.Text.Secondary.WithBold();
        _logs.FocusedTitleStyle = _theme.Focus.Title;
        _logs.BorderStyleText = _theme.Border.Strong;
        _logs.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _logs.InfoEntryStyle = _theme.State.Info;
        _logs.WarningEntryStyle = _theme.State.Warning;
        _logs.ErrorEntryStyle = _theme.State.Error;
        _logs.CriticalEntryStyle = IncidentDeskTheme.Foreground(0xFF6D66).WithBold();
        _logs.SelectedEntryStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();
        _logs.FocusedSelectedEntryStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();

        _footer.LeftTextStyle = _theme.Text.Primary;
        _footer.RightTextStyle = _theme.Text.Secondary;
        _footer.FillStyle = _theme.Surface.Overlay;
    }

    private void RefreshChrome(ScreenContext context)
    {
        var incident = _state.SelectedIncident;
        var summary = _state.BuildSummary();
        int detailWidth;
        if (context.Width >= 140)
        {
            detailWidth = 56;
        }
        else if (context.Width >= 110)
        {
            detailWidth = 46;
        }
        else
        {
            detailWidth = 38;
        }

        var sideWidth = context.Width >= 140 ? 34 : 28;

        _hero.IncidentId = incident.Id;
        _hero.Summary = ToSingleLine(incident.Summary, Math.Max(24, context.Width - 36));
        _hero.Severity = IncidentDeskState.SeverityText(incident.Severity);
        _hero.Status = IncidentDeskState.StatusText(incident.Status);
        _hero.Service = incident.Service;
        _hero.Environment = incident.Environment.ToUpperInvariant();
        _hero.Region = incident.Region;
        _hero.Owner = incident.PrimaryOwner;
        _hero.Sla = IncidentDeskState.SlaText(incident);
        _hero.Channel = $"#{incident.Channel}";
        _hero.Phase = ToSingleLine(incident.CurrentPhase, Math.Max(28, context.Width - 52));
        _hero.Impact = ToSingleLine(incident.CustomerImpact, Math.Max(24, context.Width - 56));
        _hero.SeverityStyle = SeverityChipStyle(incident.Severity);
        _hero.StatusStyle = StatusChipStyle(incident.Status);

        _queuePulse.Text = $"Open\n{summary.OpenCount:00} active incidents";
        _escalationPulse.Text = $"Critical / escalated\n{summary.CriticalCount:00} sev1  ·  {summary.EscalatedCount:00} escalated";
        _crewPulse.Text = $"Responders online\n{summary.ActiveResponders:00} crew  ·  {incident.PrimaryOwner} driving";

        RefreshQueue();
        RefreshBriefing(incident, detailWidth);
        RefreshResponderLane(incident, sideWidth);
        RefreshTimeline(incident);
        RefreshLogs(incident);
        RefreshActionStates(incident);
        RefreshFooter(incident);
    }

    private void RefreshSelectionState()
    {
        var incident = _state.SelectedIncident;
        _notes.SetValue(incident.DraftNotes);
        RefreshQueue();
    }

    private void RefreshQueue()
    {
        var items = _state.Incidents
            .Select(incident => new InboxItem(
                incident.Id,
                $"{IncidentDeskState.SeverityText(incident.Severity)} · {incident.Summary}",
                IncidentDeskState.NotificationLevel(incident.Severity),
                incident.OpenedAt,
                $"{IncidentDeskState.StatusText(incident.Status)} · {incident.PrimaryOwner}",
                isRead: !incident.HasUnreadUpdate,
                isPinned: incident.IsPinned))
            .ToList();

        _syncingQueueSelection = true;
        _queue.SetItems(items);
        _queue.Select(_state.SelectedIndex);
        _syncingQueueSelection = false;
    }

    private void RefreshBriefing(IncidentRecord incident, int width)
    {
        var lines = new List<string>();
        lines.AddRange(ComposeBlock("Customer impact", incident.CustomerImpact, width));
        lines.AddRange(ComposeBlock("Working theory", incident.Hypothesis, width));
        lines.AddRange(ComposeBlock("Runbook", incident.Runbook, width));
        lines.AddRange(ComposeBlock("Command phase", incident.CurrentPhase, width));
        _briefing.Text = string.Join('\n', lines);
    }

    private void RefreshResponderLane(IncidentRecord incident, int width)
    {
        var responders = string.Join(", ", incident.Responders);
        var lines = new List<string>();
        lines.AddRange(ComposeBlock("Primary owner", incident.PrimaryOwner, width));
        lines.AddRange(ComposeBlock("Commander", incident.Commander, width));
        lines.AddRange(ComposeBlock("Bridge", $"#{incident.Channel}", width));
        lines.AddRange(ComposeBlock("Responder set", responders, width));
        lines.AddRange(ComposeBlock("Draft note", FirstLine(incident.DraftNotes), width));
        _responderCard.Text = string.Join('\n', lines);
    }

    private void RefreshTimeline(IncidentRecord incident)
    {
        _timeline.SetItems(incident.Timeline);
        if (_timeline.Items.Count > 0)
        {
            _timeline.SetSelectedIndex(0);
        }
    }

    private void RefreshLogs(IncidentRecord incident)
    {
        _logs.SetEntries(incident.Logs);
        if (incident.Logs.Count > 0)
        {
            _logs.SetSelectedIndex(0);
        }
    }

    private void RefreshActionStates(IncidentRecord incident)
    {
        _ackButton.IsDisabled = incident.Status == IncidentStatus.Resolved;
        _assignButton.IsDisabled = false;
        _escalateButton.IsDisabled = incident.Status == IncidentStatus.Resolved && incident.Severity == IncidentSeverity.Critical;
        _resolveButton.IsDisabled = incident.Status == IncidentStatus.Resolved;
        _reopenButton.IsDisabled = incident.Status != IncidentStatus.Resolved && incident.Status != IncidentStatus.Monitoring;
        _syncButton.IsDisabled = false;
    }

    private void RefreshFooter(IncidentRecord incident)
    {
        _footer.LeftText = $"{incident.Id}  {IncidentDeskState.SeverityText(incident.Severity)}  {IncidentDeskState.StatusText(incident.Status)}  owner {incident.PrimaryOwner}  {IncidentDeskState.SlaText(incident)}";
        _footer.RightText = $"F1 queue  F2 timeline  F3 notes  F4 logs  a g e v o s  ·  {_state.LastCommand}";
    }

    private void ExecuteAction(IncidentDeskAction action)
    {
        _state.CaptureDraft(_notes.Value);
        switch (action)
        {
            case IncidentDeskAction.Acknowledge:
                _state.AcknowledgeSelected();
                break;
            case IncidentDeskAction.Assign:
                _state.AssignSelected();
                break;
            case IncidentDeskAction.Escalate:
                _state.EscalateSelected();
                break;
            case IncidentDeskAction.Resolve:
                _state.ResolveSelected();
                break;
            case IncidentDeskAction.Reopen:
                _state.ReopenSelected();
                break;
            case IncidentDeskAction.Sync:
                _state.SyncSelected();
                break;
        }

        RefreshSelectionState();
    }

    private bool IsEditingNotes() => _notes.IsFocused;

    private static void ConfigurePulseCard(Label card, TesseraStyle textStyle)
    {
        card.TextStyle = textStyle;
        card.TitleStyle = IncidentDeskTheme.Foreground(0xD6BFAF).WithBold();
        card.FocusedTitleStyle = IncidentDeskTheme.Foreground(0xF3B276).WithBold();
        card.BorderStyleText = IncidentDeskTheme.Foreground(0x8B5A46);
        card.FocusedBorderStyleText = IncidentDeskTheme.Foreground(0xF3B276).WithBold();
    }

    private static void ConfigureActionButton(Button button, int foregroundRgb, int backgroundRgb)
    {
        var labelStyle = IncidentDeskTheme.Foreground(foregroundRgb).WithBold();
        var surfaceStyle = IncidentDeskTheme.Background(backgroundRgb);
        button.LabelStyle = labelStyle;
        button.FocusedLabelStyle = labelStyle;
        button.PressedLabelStyle = labelStyle.WithUnderline();
        button.SurfaceStyle = surfaceStyle;
        button.FocusedSurfaceStyle = surfaceStyle;
        button.PressedSurfaceStyle = surfaceStyle;
        button.DisabledLabelStyle = IncidentDeskTheme.Foreground(0x8E7A74);
    }

    private static TesseraStyle SeverityChipStyle(IncidentSeverity severity) => severity switch
    {
        IncidentSeverity.Critical => IncidentDeskTheme.Chip(0xFFF4E8, 0x8D3228),
        IncidentSeverity.High => IncidentDeskTheme.Chip(0xFFF4E8, 0x7B4A28),
        IncidentSeverity.Medium => IncidentDeskTheme.Chip(0xFFF4E8, 0x5C5447),
        _ => IncidentDeskTheme.Chip(0xEAF7EF, 0x345043),
    };

    private static TesseraStyle StatusChipStyle(IncidentStatus status) => status switch
    {
        IncidentStatus.Escalated => IncidentDeskTheme.Chip(0xFFF4E8, 0x6D231E),
        IncidentStatus.Investigating => IncidentDeskTheme.Chip(0xFFF4E8, 0x5D4335),
        IncidentStatus.Acknowledged => IncidentDeskTheme.Chip(0xFFF4E8, 0x4B5640),
        IncidentStatus.Monitoring => IncidentDeskTheme.Chip(0xF4F7E9, 0x385548),
        _ => IncidentDeskTheme.Chip(0xEAF7EF, 0x345043),
    };

    private static IEnumerable<string> ComposeBlock(string label, string value, int width)
    {
        var labelWidth = label.Length + 2;
        var lines = WrapText(value, Math.Max(12, width - labelWidth));
        if (lines.Count == 0)
        {
            yield return $"{label}:";
            yield break;
        }

        yield return $"{label}: {lines[0]}";
        for (var index = 1; index < lines.Count; index++)
        {
            yield return $"{new string(' ', labelWidth)}{lines[index]}";
        }
    }

    private static List<string> WrapText(string text, int width)
    {
        var words = (text ?? string.Empty)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (words.Length == 0)
        {
            return [string.Empty];
        }

        var lines = new List<string>();
        var current = new StringBuilder(words[0]);
        for (var index = 1; index < words.Length; index++)
        {
            var next = words[index];
            if (current.Length + 1 + next.Length <= width)
            {
                current.Append(' ').Append(next);
                continue;
            }

            lines.Add(current.ToString());
            current.Clear();
            current.Append(next);
        }

        lines.Add(current.ToString());
        return lines;
    }

    private static string FirstLine(string value)
    {
        var split = (value ?? string.Empty)
            .Split('\n', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        return split.Length == 0 ? "No working note captured." : split[0];
    }

    private static string ToSingleLine(string text, int width)
    {
        var normalized = (text ?? string.Empty).Replace('\n', ' ').Trim();
        if (normalized.Length <= width)
        {
            return normalized;
        }

        return normalized[..Math.Max(0, width - 1)].TrimEnd() + "…";
    }
}

internal enum IncidentDeskAction
{
    Acknowledge,
    Assign,
    Escalate,
    Resolve,
    Reopen,
    Sync,
}

internal sealed record IncidentDeskActionMessage(IncidentDeskAction Action) : Message;
