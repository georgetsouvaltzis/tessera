using System.Globalization;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Examples.GitConsole;

internal sealed partial class GitConsoleApp : TeaApp
{
    private static readonly string[] PulseFrames = ["syncing", "scanning", "reviewing", "steady"];

    private readonly TeaTheme _theme = GitConsoleTheme.DefaultTheme;
    private readonly GitConsoleState _state = GitConsoleState.CreateSeed();

    private readonly GitRepoHeaderControl _repoHeader = new();
    private readonly StatsCard _flowCard = new() { Title = "Flow Pulse", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly StatsCard _queueCard = new() { Title = "Queue", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly StatsCard _syncCard = new() { Title = "Sync", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly SideNavRail _scopeRail = new() { Title = "Lanes", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆" };
    private readonly GitWorktreeControl _worktree = new() { Title = "Worktree Radar", FocusMarker = "◆" };
    private readonly PaneTabs _diffTabs = new() { Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", Title = "View" };
    private readonly DiffView _diff = new() { Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆" };
    private readonly TextInput _subjectInput = new() { Title = "Commit Subject", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", Placeholder = "ship: tighten flagship git shell" };
    private readonly TextArea _notesInput = new() { Title = "Operator Notes", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", ShowLineNumbers = false, Wrap = true };
    private readonly CommandOutput _history = new() { Title = "Command / Action History", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", AutoFollow = true, ShowTimestamp = true };
    private readonly StatusBar _footer = new() { Fill = ' ' };
    private readonly Button _stageButton = new() { Text = "Stage / Unstage", Description = "Toggle selected file", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _discardButton = new() { Text = "Discard", Description = "Drop selected patch", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _modeButton = new() { Text = "Toggle Diff", Description = "Inline vs patch radar", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _commitButton = new() { Text = "Commit", Description = "Ship staged queue", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _syncButton = new() { Text = "Sync", Description = "Push / fetch pulse", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Label _rightHeader = new() { Border = BorderStyle.None };

    public GitConsoleApp()
    {
        ConfigureTheme();
        WireEvents();
        SeedControls();

        _worktree.RequestFocus();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(900), _ => new PulseTickMessage());

    public override TeaEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key:
                return HandleKey(key);
            case AppActionMessage action:
                ExecuteAction(action.Kind);
                return null;
            case PulseTickMessage:
                _state.AdvancePulse();
                return null;
            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshChrome();
        RefreshControls();

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            ConfigureHeader(window, context);
            window.Body(body => ConfigureBody(body, context));
            window.Footer(1, _footer);
        });
    }

    private TeaEffect? HandleKey(KeyPressed key)
    {
        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.Is(Key.F1))
        {
            _worktree.RequestFocus();
            return null;
        }

        if (key.Is(Key.F2))
        {
            _diff.RequestFocus();
            return null;
        }

        if (key.Is(Key.F3))
        {
            _subjectInput.RequestFocus();
            return null;
        }

        if (key.Is(Key.F4))
        {
            _history.RequestFocus();
            return null;
        }

        if (IsEditingText())
        {
            return null;
        }

        if (key.IsCharacter('s'))
        {
            ExecuteAction(GitConsoleAction.ToggleStage);
            return null;
        }

        if (key.IsCharacter('x'))
        {
            ExecuteAction(GitConsoleAction.DiscardSelected);
            return null;
        }

        if (key.IsCharacter('d'))
        {
            ExecuteAction(GitConsoleAction.ToggleDiffMode);
            return null;
        }

        if (key.IsCharacter('u'))
        {
            ExecuteAction(GitConsoleAction.Sync);
            return null;
        }

        return null;
    }

    private void WireEvents()
    {
        _scopeRail.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null && _state.SetScope(args.SelectedItem.Id))
            {
                RefreshControls();
            }
        };

        _worktree.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null && _state.SelectFile(args.SelectedItem.Id))
            {
                RefreshDiff();
            }
        };

        _diffTabs.SelectionChanged += (_, _) => RefreshDiff();

        _stageButton.Activated += (_, _) => Post(new AppActionMessage(GitConsoleAction.ToggleStage));
        _discardButton.Activated += (_, _) => Post(new AppActionMessage(GitConsoleAction.DiscardSelected));
        _modeButton.Activated += (_, _) => Post(new AppActionMessage(GitConsoleAction.ToggleDiffMode));
        _commitButton.Activated += (_, _) => Post(new AppActionMessage(GitConsoleAction.CommitStaged));
        _syncButton.Activated += (_, _) => Post(new AppActionMessage(GitConsoleAction.Sync));
    }

    private void SeedControls()
    {
        _scopeRail.SetItems(_state.BuildNavItems());
        _scopeRail.SetSelectedIndex(0);
        _diffTabs.SetTabs(
        [
            new PaneTabItem("working", "Working Copy"),
            new PaneTabItem("staged", "Staged Snapshot"),
            new PaneTabItem("radar", "Patch Radar"),
        ]);
        _diffTabs.SetSelectedIndex(0);
        _history.SetLines(GitConsoleState.BuildSeedHistory());
        _notesInput.SetValue("- verify staged files\n- visually review diff rhythm\n");
        RefreshControls();
    }

    private void ConfigureTheme()
    {
        _scopeRail.ApplyTheme(_theme);
        _diffTabs.ApplyTheme(_theme);
        _diff.ApplyTheme(_theme);
        _subjectInput.ApplyTheme(_theme);
        _notesInput.ApplyTheme(_theme);
        _history.ApplyTheme(_theme);
        _flowCard.ApplyTheme(_theme);
        _queueCard.ApplyTheme(_theme);
        _syncCard.ApplyTheme(_theme);
        _stageButton.ApplyTheme(_theme);
        _discardButton.ApplyTheme(_theme);
        _modeButton.ApplyTheme(_theme);
        _commitButton.ApplyTheme(_theme);
        _syncButton.ApplyTheme(_theme);
        _footer.ApplyTheme(_theme);

        _scopeRail.TitleStyle = _theme.Text.Primary;
        _scopeRail.FocusedTitleStyle = _theme.Focus.Title;
        _scopeRail.BorderStyleText = _theme.Border.Strong;
        _scopeRail.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _scopeRail.ItemStyle = _theme.Text.Primary;
        _scopeRail.HoveredItemStyle = _theme.Accent.Secondary;
        _scopeRail.SelectedItemStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();
        _scopeRail.FocusedSelectedItemStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();

        _diff.TitleStyle = _theme.Text.Primary.WithBold();
        _diff.FocusedTitleStyle = _theme.Focus.Title;
        _diff.BorderStyleText = _theme.Border.Strong;
        _diff.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _diff.SelectedLineStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();

        _flowCard.TitleStyle = _theme.Text.Secondary;
        _flowCard.ValueStyle = _theme.Accent.Primary.WithBold();
        _flowCard.BorderStyleText = _theme.Border.Strong;
        _queueCard.ValueStyle = _theme.State.Warning.WithBold();
        _queueCard.BorderStyleText = _theme.Border.Strong;
        _syncCard.ValueStyle = _theme.State.Info.WithBold();
        _syncCard.BorderStyleText = _theme.Border.Strong;

        _subjectInput.ValueTextStyle = _theme.Text.Primary;
        _subjectInput.BorderStyleText = _theme.Border.Strong;
        _subjectInput.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _subjectInput.PlaceholderTextStyle = _theme.Text.Muted;

        _notesInput.ValueTextStyle = _theme.Text.Primary;
        _notesInput.DisabledValueTextStyle = _theme.Text.Muted;
        _notesInput.BorderStyleText = _theme.Border.Strong;
        _notesInput.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);

        _history.BorderStyleText = _theme.Border.Strong;
        _history.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _history.TitleStyle = _theme.Text.Primary.WithBold();

        _footer.LeftTextStyle = _theme.Text.Secondary;
        _footer.RightTextStyle = _theme.Text.Muted;
        _footer.FillStyle = _theme.Surface.Base;

        _rightHeader.TextStyle = _theme.Text.Secondary;
        _rightHeader.Text = string.Empty;

        _repoHeader.TitleStyle = _theme.Text.Secondary.WithBold();
        _repoHeader.BorderStyleText = _theme.Border.Strong;
        _repoHeader.NameStyle = _theme.Text.Primary.WithBold();
        _repoHeader.BranchStyle = GitConsoleTheme.Chip(0x091018, 0x7AE2CF, bold: true);
        _repoHeader.PathStyle = _theme.Text.Muted;
        _repoHeader.PulseStyle = GitConsoleTheme.Chip(0x091018, 0x67C6FF, bold: true);
        _repoHeader.ActionStyle = _theme.Accent.Secondary.WithBold();
        _repoHeader.DetailStyle = _theme.Text.Secondary;

        _worktree.TitleStyle = _theme.Text.Secondary.WithBold();
        _worktree.FocusedTitleStyle = _theme.Focus.Title;
        _worktree.BorderStyleText = _theme.Border.Strong;
        _worktree.FocusedBorderStyleText = _theme.Border.Focused.Merge(_theme.Focus.Border);
        _worktree.GroupStyle = _theme.Text.Muted.WithBold();
        _worktree.DefaultRowStyle = _theme.Text.Primary;
        _worktree.SelectedRowStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background);
        _worktree.FocusedSelectedRowStyle = _theme.Focus.Ring;
        _worktree.SecondaryStyle = _theme.Text.Secondary;
        _worktree.StagedStyle = GitConsoleTheme.Chip(0x091018, 0x61E294, bold: true);
        _worktree.ReviewStyle = GitConsoleTheme.Chip(0x091018, 0xF2C572, bold: true);
        _worktree.AddedStyle = _theme.State.Success;
        _worktree.RemovedStyle = _theme.State.Error;
        _worktree.EmptyStyle = _theme.Text.Muted;

        var buttonBorder = _theme.Border.Strong;
        var buttonFocus = _theme.Border.Focused.Merge(_theme.Focus.Border);
        foreach (var button in new[] { _stageButton, _discardButton, _modeButton, _commitButton, _syncButton })
        {
            button.LabelStyle = _theme.Text.Primary.WithBold();
            button.FocusedLabelStyle = _theme.Accent.Primary.WithBold();
            button.BorderStyleText = buttonBorder;
            button.FocusedBorderStyleText = buttonFocus;
            button.PressedLabelStyle = _theme.Selection.Foreground.Merge(_theme.Selection.Background).WithBold();
        }
    }

    private void RefreshChrome()
    {
        var metrics = _state.GetMetrics();
        _repoHeader.RepositoryName = _state.RepositoryName;
        _repoHeader.RepositoryPath = _state.RepositoryPath;
        _repoHeader.BranchName = _state.BranchName;
        _repoHeader.RemoteName = _state.RemoteName;
        _repoHeader.PulseText = PulseFrames[_state.PulseIndex];
        _repoHeader.LastAction = _state.LastAction;
        _repoHeader.LastActionDetail = _state.LastActionDetail;
        _repoHeader.Ahead = _state.Ahead;
        _repoHeader.Behind = _state.Behind;

        _flowCard.SetItems(
        [
            new StatItem("active", _state.LastAction.Split(' ', 2)[0].ToUpperInvariant()),
            new StatItem("focus", FocusLabel()),
        ]);
        _queueCard.SetItems(
        [
            new StatItem("staged", metrics.Staged.ToString("00", CultureInfo.InvariantCulture)),
            new StatItem("review", metrics.Review.ToString("00", CultureInfo.InvariantCulture)),
        ]);
        _syncCard.SetItems(
        [
            new StatItem("ahead", _state.Ahead.ToString("00", CultureInfo.InvariantCulture)),
            new StatItem("behind", _state.Behind.ToString("00", CultureInfo.InvariantCulture)),
        ]);

        _rightHeader.Text =
            "Commit lane visible. Use buttons or pointer.";
        _footer.LeftText =
            "F1 tree  F2 diff  F3 commit  F4 history  |  s stage  x discard  d mode  u sync  ctrl+c quit";
        _footer.RightText = $"{_state.Scope} · {_state.LastAction.Split(' ', 2)[0]}";
    }

    private void RefreshControls()
    {
        _scopeRail.SetItems(_state.BuildNavItems());
        _scopeRail.SetSelectedIndex((int)_state.Scope);

        _worktree.SetSections(_state.BuildSections());
        if (_state.SelectedFileId is not null)
        {
            _worktree.SelectById(_state.SelectedFileId);
        }

        RefreshDiff();
    }

    private void RefreshDiff()
    {
        var snapshot = _state.BuildDiffSnapshot(CurrentDiffTab());
        _diff.Title = snapshot.Title;
        _diff.Mode = snapshot.Mode;
        _diff.SetTexts(snapshot.OldText, snapshot.NewText);
    }

    private void ExecuteAction(GitConsoleAction action)
    {
        GitActionResult result;
        switch (action)
        {
            case GitConsoleAction.ToggleStage:
                result = _state.ToggleStageSelected();
                RefreshControls();
                break;
            case GitConsoleAction.DiscardSelected:
                result = _state.DiscardSelected();
                RefreshControls();
                break;
            case GitConsoleAction.CommitStaged:
                result = _state.CommitStaged(_subjectInput.Value, _notesInput.Value);
                if (result.Success)
                {
                    _subjectInput.Clear();
                    _notesInput.SetValue("- note follow-up reviewer\n");
                }
                RefreshControls();
                break;
            case GitConsoleAction.Sync:
                result = _state.Sync();
                RefreshChrome();
                break;
            default:
                CycleDiffTab();
                result = GitActionResult.InfoResult("View mode changed", _diffTabs.SelectedItem?.Title ?? "Diff view updated.");
                RefreshDiff();
                break;
        }

        AppendHistory(result);
    }

    private void AppendHistory(GitActionResult result)
    {
        var text = $"{result.Title} · {result.Detail}";
        switch (result.Channel)
        {
            case CommandOutputChannel.StdOut:
                _history.AppendStdOut(text);
                break;
            case CommandOutputChannel.StdErr:
                _history.AppendStdErr(text);
                break;
            default:
                _history.AppendSystem(text);
                break;
        }
    }

    private bool IsEditingText() => _subjectInput.IsFocused || _notesInput.IsFocused;

    private GitDiffTab CurrentDiffTab() => _diffTabs.SelectedItem?.Id switch
    {
        "staged" => GitDiffTab.StagedSnapshot,
        "radar" => GitDiffTab.PatchRadar,
        _ => GitDiffTab.WorkingCopy,
    };

    private void CycleDiffTab()
    {
        var next = (_diffTabs.SelectedIndex + 1) % Math.Max(1, _diffTabs.Tabs.Count);
        _diffTabs.SetSelectedIndex(next);
    }

    private string FocusLabel()
    {
        if (_worktree.IsFocused)
        {
            return "WORKTREE";
        }

        if (_diff.IsFocused)
        {
            return "DIFF";
        }

        if (_subjectInput.IsFocused || _notesInput.IsFocused)
        {
            return "COMMIT";
        }

        if (_history.IsFocused)
        {
            return "HISTORY";
        }

        return "SHELL";
    }

    private sealed record PulseTickMessage : Message;

    private sealed record AppActionMessage(GitConsoleAction Kind) : Message;

    private enum GitConsoleAction
    {
        ToggleStage,
        DiscardSelected,
        ToggleDiffMode,
        CommitStaged,
        Sync,
    }
}
