using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class GitConsoleApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly ListView<RepoFile> _files = new(static file => file.RenderLabel())
    {
        Title = "Changed Files",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        PageSize = 14,
    };

    private readonly DiffView _diff = new()
    {
        Title = "Diff",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly ActivityFeed _output = new()
    {
        Title = "Ops Log",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        AutoFollow = true,
        ShowTimestamp = true,
    };

    private readonly StatusBar _headline = new();
    private readonly StatusBar _status = new();
    private readonly List<RepoFile> _repoFiles = CreateRepoFiles();
    private readonly Control[] _focusOrder;
    private string _lastAction = "workspace ready";

    public GitConsoleApp()
    {
        _focusOrder = [_files, _diff, _output];

        ThemeScope.Apply(DefaultTheme, _files, _diff, _output, _headline, _status);
        ConfigureTheme();

        _files.SelectionChanged += (_, _) => SyncDiffToSelection();

        ResetFileList();
        SeedOutput();
        _files.RequestFocus();
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.Is(Key.F1) || key.IsCharacter('1', ModifierKeys.Ctrl))
        {
            FocusPane(_files);
            return null;
        }

        if (key.Is(Key.F2) || key.IsCharacter('2', ModifierKeys.Ctrl))
        {
            FocusPane(_diff);
            return null;
        }

        if (key.Is(Key.F3) || key.IsCharacter('3', ModifierKeys.Ctrl))
        {
            FocusPane(_output);
            return null;
        }

        if (key.IsCharacter('s', ModifierKeys.Ctrl))
        {
            ToggleStageSelected();
            return null;
        }

        if (key.IsCharacter('r', ModifierKeys.Ctrl))
        {
            RevertSelected();
            return null;
        }

        if (key.IsCharacter('k', ModifierKeys.Ctrl))
        {
            CommitStaged();
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        UpdateChrome();

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(1);
            window.HeaderRow(1, row => row.Fill(_headline));
            window.Body(body => body.Row(row =>
            {
                row.Fixed(Math.Min(42, Math.Max(34, context.Width / 3)), _files);
                row.Fill(content => content.Column(column =>
                {
                    column.Weighted(3, _diff);
                    column.Weighted(2, _output);
                }));
            }));
            window.Footer(1, _status);
        });
    }

    private void ConfigureTheme()
    {
        var theme = DefaultTheme;
        var focusedBorder = theme.Border.Focused.Merge(theme.Focus.Border);

        _files.TitleStyle = theme.Text.Primary;
        _files.FocusedTitleStyle = focusedBorder.WithBold();
        _files.BorderStyleText = theme.Border.Strong;
        _files.FocusedBorderStyleText = focusedBorder;
        _files.DefaultRowStyle = theme.Text.Primary;
        _files.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();
        _files.SelectedRowStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _files.RowMarkers = new ListViewMarkerSet("·", "▶", "▸");

        _diff.TitleStyle = theme.Text.Primary;
        _diff.FocusedTitleStyle = focusedBorder.WithBold();
        _diff.BorderStyleText = theme.Border.Strong;
        _diff.FocusedBorderStyleText = focusedBorder;
        _diff.HeaderStyle = theme.Text.Secondary.WithBold();
        _diff.AddedLineStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(166, 227, 161));
        _diff.RemovedLineStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(243, 139, 168));
        _diff.UnchangedLineStyle = theme.Text.Primary;
        _diff.SelectedLineStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);

        _output.TitleStyle = theme.Text.Primary;
        _output.FocusedTitleStyle = focusedBorder.WithBold();
        _output.BorderStyleText = theme.Border.Strong;
        _output.FocusedBorderStyleText = focusedBorder;
        _output.InfoItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(137, 180, 250));
        _output.SuccessItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(166, 227, 161));
        _output.WarningItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175));
        _output.ErrorItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(243, 139, 168));
        _output.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();
        _output.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background);
        _output.FocusedSelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _output.UnreadItemStyle = theme.Accent.Primary.WithBold();
        _output.MutedItemStyle = theme.Text.Muted.WithDim();
        _output.DisabledItemStyle = theme.Text.Muted.WithDim();
        _output.TimestampStyle = theme.Text.Secondary;

        _headline.LeftTextStyle = theme.Text.Primary.WithBold();
        _headline.RightTextStyle = theme.Text.Secondary;
        _headline.FillStyle = theme.Surface.Base.Merge(theme.Border.Default);
        _headline.Fill = ' ';

        _status.LeftTextStyle = theme.Text.Secondary;
        _status.RightTextStyle = theme.Text.Muted;
        _status.FillStyle = theme.Surface.Base;
        _status.Fill = ' ';
    }

    private void ResetFileList()
    {
        _files.SetItems(_repoFiles.Where(static file => file.IsChanged).ToArray());
        if (_files.Count > 0)
        {
            _files.SetSelectedIndex(0);
        }

        SyncDiffToSelection();
    }

    private void SyncDiffToSelection()
    {
        var selected = _files.SelectedItem;
        if (selected is null)
        {
            _diff.Title = "Patch View | clean tree";
            _diff.SetTexts(string.Empty, string.Empty);
            return;
        }

        _diff.Title =
            $"{selected.StatusLabel} {selected.Path} | {(selected.IsStaged ? "index" : "worktree")} | +{selected.AddedLines}/-{selected.RemovedLines}";
        _diff.SetTexts(selected.OldText, selected.NewText);
    }

    private void ToggleStageSelected()
    {
        var selected = _files.SelectedItem;
        if (selected is null)
        {
            AppendOutput("git", "stage skipped", "no file selected", kind: ActivityFeedItemKind.Warning);
            _lastAction = "stage skipped";
            return;
        }

        selected.IsStaged = !selected.IsStaged;
        _lastAction = selected.IsStaged ? $"staged {selected.Path}" : $"unstaged {selected.Path}";
        AppendOutput(
            "git",
            selected.IsStaged ? "staged" : "unstaged",
            selected.Path,
            selected.StatusLabel,
            kind: selected.IsStaged ? ActivityFeedItemKind.Success : ActivityFeedItemKind.Info);
        RefreshFilePane(selected.Path);
    }

    private void RevertSelected()
    {
        var selected = _files.SelectedItem;
        if (selected is null)
        {
            AppendOutput("git", "revert skipped", "no file selected", kind: ActivityFeedItemKind.Warning);
            _lastAction = "revert skipped";
            return;
        }

        selected.IsChanged = false;
        selected.IsStaged = false;
        _lastAction = $"reverted {selected.Path}";
        AppendOutput("git", "reverted", selected.Path, selected.StatusLabel, kind: ActivityFeedItemKind.Warning);
        RefreshFilePane(selected.Path);
    }

    private void CommitStaged()
    {
        var staged = _repoFiles.Where(static file => file.IsChanged && file.IsStaged).ToArray();
        if (staged.Length == 0)
        {
            AppendOutput("git", "commit skipped", "nothing staged", kind: ActivityFeedItemKind.Warning);
            _lastAction = "commit skipped";
            return;
        }

        foreach (var file in staged)
        {
            file.IsChanged = false;
            file.IsStaged = false;
        }

        _lastAction = $"committed {staged.Length} files";
        AppendOutput("git", "commit", $"{staged.Length} files", "feat: tighten public-v1 examples", ActivityFeedItemKind.Success);
        ResetFileList();
    }

    private void RefreshFilePane(string? preferredPath)
    {
        var visible = _repoFiles.Where(static file => file.IsChanged).ToArray();
        _files.SetItems(visible);

        if (visible.Length == 0)
        {
            SyncDiffToSelection();
            return;
        }

        var preferredIndex = preferredPath is null
            ? -1
            : Array.FindIndex(visible, file => string.Equals(file.Path, preferredPath, StringComparison.Ordinal));
        _files.SetSelectedIndex(preferredIndex >= 0 ? preferredIndex : Math.Min(_files.SelectedIndex, visible.Length - 1));
        SyncDiffToSelection();
    }

    private void SeedOutput()
    {
        _output.SetItems(
        [
            new ActivityFeedItem("git", "status", "feature/public-v1", "6 files changed, 2 staged", ActivityFeedItemKind.Info, new DateTimeOffset(2026, 3, 27, 8, 10, 0, TimeSpan.Zero))
            {
                IsUnread = false,
            },
            new ActivityFeedItem("git", "fetch", "origin/main", "up to date, 1 review thread still open", ActivityFeedItemKind.Success, new DateTimeOffset(2026, 3, 27, 8, 12, 0, TimeSpan.Zero))
            {
                IsUnread = false,
            },
            new ActivityFeedItem("ci", "warned", "widget-labs", "snapshot drift pending for TagInput", ActivityFeedItemKind.Warning, new DateTimeOffset(2026, 3, 27, 8, 14, 0, TimeSpan.Zero)),
            new ActivityFeedItem("review", "requested", "GitConsole", "focus keys and pane chrome need polish", ActivityFeedItemKind.Info, new DateTimeOffset(2026, 3, 27, 8, 16, 0, TimeSpan.Zero)),
            new ActivityFeedItem("merge", "blocked", "feature/public-v1", "follow-up visual review still pending", ActivityFeedItemKind.Warning, new DateTimeOffset(2026, 3, 27, 8, 18, 0, TimeSpan.Zero)),
        ]);
    }

    private void AppendOutput(
        string actor,
        string action,
        string? target = null,
        string? details = null,
        ActivityFeedItemKind kind = ActivityFeedItemKind.Info)
    {
        _output.Append(actor, action, target, details, kind, DateTimeOffset.UtcNow);
    }

    private void UpdateChrome()
    {
        var focus = ResolveFocusName();
        var changed = _repoFiles.Count(static file => file.IsChanged);
        var staged = _repoFiles.Count(static file => file.IsChanged && file.IsStaged);
        var selected = _files.SelectedItem;
        var file = selected?.Path ?? "-";
        var mode = selected?.IsStaged == true ? "index" : "worktree";

        _headline.LeftText =
            $"repo=teasharp branch=feature/public-v1 head=edff77f changed={changed} staged={staged}";
        _headline.RightText =
            $"ci=warning review=2 pending last={_lastAction}";

        _files.Title = $"Working Tree | {changed} changed | {staged} staged";
        _output.Title = $"Ops Log | {_output.Items.Count} events";

        _status.LeftText =
            $"focus={focus} file={file} mode={mode}";
        _status.RightText =
            $"F1 files F2 diff F3 output | Ctrl+S stage/unstage Ctrl+R revert Ctrl+K commit | diff Tab mode | Ctrl+C quit";
    }

    private string ResolveFocusName()
    {
        if (_files.IsFocused)
        {
            return "files";
        }

        if (_diff.IsFocused)
        {
            return "diff";
        }

        if (_output.IsFocused)
        {
            return "output";
        }

        return "-";
    }

    private static void FocusPane(Control target)
    {
        target.RequestFocus();
    }

    private static List<RepoFile> CreateRepoFiles() =>
    [
        new("src/TeaSharp/Controls/TagInput.cs", "M", 3, 0,
            """
            public bool AllowDuplicates => _options.AllowDuplicates;
            public int? MaxTags => _options.MaxTags;
            """,
            """
            public bool AllowDuplicates => _options.AllowDuplicates;
            public int? MaxTags => _options.MaxTags;
            public bool PreserveViewportOnCommit => _options.PreserveViewportOnCommit;
            """)
        {
            IsStaged = true,
        },
        new("examples/DeployConsole/DeployConsoleApp.cs", "A", 18, 0,
            string.Empty,
            """
            private void StartDeployment()
            {
                _deployTarget = _services.SelectedItem;
                _deploy.SetRunning(true);
            }
            """),
        new("tests/TeaSharp.Tests/SpinnerControlTests.cs", "M", 1, 0,
            """
            Assert.That(spinner.Running, Is.True);
            """,
            """
            Assert.That(spinner.Running, Is.True);
            Assert.That(spinner.Frames.Count, Is.EqualTo(3));
            """)
        {
            IsStaged = true,
        },
        new("docs/spec.md", "M", 2, 1,
            """
            - spinner exposes Frames
            """,
            """
            - spinner exposes Frames
            - spinner supports runtime SetFrames(...) swaps
            """),
        new("examples/GitConsole/GitConsoleApp.cs", "M", 12, 3,
            """
            _status.RightText = "Ctrl+1 files Ctrl+2 diff Ctrl+3 output";
            """,
            """
            _headline.RightText = "ci=warning review=2 pending last=workspace ready";
            _status.RightText = "F1 files F2 diff F3 output | Ctrl+S stage/unstage";
            """),
        new("src/TeaSharp/Controls/Choice.cs", "D", 0, 6,
            """
            public bool LegacySelection { get; set; }
            """,
            string.Empty),
    ];

    private sealed class RepoFile(string path, string statusLabel, int addedLines, int removedLines, string oldText, string newText)
    {
        public string Path { get; } = path;

        public string StatusLabel { get; } = statusLabel;

        public int AddedLines { get; } = Math.Max(0, addedLines);

        public int RemovedLines { get; } = Math.Max(0, removedLines);

        public string OldText { get; } = oldText;

        public string NewText { get; } = newText;

        public bool IsChanged { get; set; } = true;

        public bool IsStaged { get; set; }

        public string RenderLabel()
        {
            var stage = IsStaged ? "●" : " ";
            return $"{stage} {StatusLabel,-2} +{AddedLines,-2}/-{RemovedLines,-2} {Path}";
        }
    }
}
