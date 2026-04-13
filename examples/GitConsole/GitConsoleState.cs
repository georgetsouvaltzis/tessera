using System.Globalization;
using Tessera.Controls;

namespace Tessera.Examples.GitConsole;

internal enum GitScope
{
    Overview,
    Changes,
    ReviewQueue,
    Shiproom
}

internal enum GitDiffTab
{
    WorkingCopy,
    StagedSnapshot,
    PatchRadar
}

internal enum GitChangeKind
{
    Modified,
    Added,
    Deleted,
    Renamed
}

internal sealed class GitConsoleState
{
    private readonly List<GitFileEntry> _files;

    private GitConsoleState(List<GitFileEntry> files)
    {
        _files = files;
        Scope = GitScope.Overview;
        SelectedFileId = files.FirstOrDefault()?.Id;
    }

    public string RepositoryName { get; } = "tessera";

    public string RepositoryPath { get; } = "~/work/tea/public-v1/tessera";

    public string BranchName { get; } = "release/public-v1";

    public string RemoteName { get; } = "origin";

    public string LastAction { get; private set; } = "Queue primed for release handoff.";

    public string LastActionDetail { get; private set; } =
        "Two staged paths are ready; remote is one commit behind local intent.";

    public int Ahead { get; private set; } = 2;

    public int Behind { get; private set; } = 1;

    public int PulseIndex { get; private set; }

    public int CommitSequence { get; private set; } = 18;

    public GitScope Scope { get; private set; }

    public string? SelectedFileId { get; private set; }

    public GitFileEntry? SelectedFile =>
        _files.FirstOrDefault(file => string.Equals(file.Id, SelectedFileId, StringComparison.Ordinal));

    public static GitConsoleState CreateSeed()
    {
        return new GitConsoleState(CreateSeedFiles());
    }

    public GitRepoMetrics GetMetrics()
    {
        var staged = _files.Count(static file => file.IsStaged);
        var changed = _files.Count(static file => !file.IsStaged);
        var review = _files.Count(static file => file.IsReviewCritical);
        return new GitRepoMetrics(staged, changed, review, _files.Count);
    }

    public IReadOnlyList<NavItem> BuildNavItems()
    {
        var staged = _files.Count(static file => file.IsStaged);
        var changed = _files.Count(static file => !file.IsStaged);
        var review = _files.Count(static file => file.IsReviewCritical);

        return
        [
            new NavItem("overview", "Overview", "◆", _files.Count.ToString("00", CultureInfo.InvariantCulture)),
            new NavItem("changes", "Changes", "Δ", changed.ToString("00", CultureInfo.InvariantCulture)),
            new NavItem("review", "Review Queue", "!", review.ToString("00", CultureInfo.InvariantCulture)),
            new NavItem("shiproom", "Shiproom", "↑", staged.ToString("00", CultureInfo.InvariantCulture))
        ];
    }

    public bool SetScope(string id)
    {
        var next = id switch
        {
            "changes" => GitScope.Changes,
            "review" => GitScope.ReviewQueue,
            "shiproom" => GitScope.Shiproom,
            _ => GitScope.Overview
        };

        if (Scope == next)
        {
            return false;
        }

        Scope = next;
        NormalizeSelection();
        return true;
    }

    public bool SelectFile(string id)
    {
        if (!GetVisibleFiles().Any(file => string.Equals(file.Id, id, StringComparison.Ordinal)))
        {
            return false;
        }

        if (string.Equals(SelectedFileId, id, StringComparison.Ordinal))
        {
            return false;
        }

        SelectedFileId = id;
        return true;
    }

    public IReadOnlyList<GitWorktreeSection> BuildSections()
    {
        var visible = GetVisibleFiles().ToArray();
        if (visible.Length == 0)
        {
            return [];
        }

        if (Scope == GitScope.ReviewQueue)
        {
            return new[]
            {
                BuildSection("Review Hotspots", visible.Where(static file => file.IsReviewCritical).ToArray()),
                BuildSection("Ready To Land", visible.Where(static file => file.IsStaged).ToArray())
            }.Where(static section => section.Items.Count > 0).ToArray();
        }

        return new[]
        {
            BuildSection("Staged", visible.Where(static file => file.IsStaged).ToArray()), BuildSection("Modified",
                visible.Where(static file => !file.IsStaged && file.Kind != GitChangeKind.Added).ToArray()),
            BuildSection("Untracked",
                visible.Where(static file => !file.IsStaged && file.Kind == GitChangeKind.Added).ToArray())
        }.Where(static section => section.Items.Count > 0).ToArray();
    }

    public GitDiffSnapshot BuildDiffSnapshot(GitDiffTab tab)
    {
        var selected = SelectedFile;
        if (selected is null)
        {
            return new GitDiffSnapshot(
                "Patch View | clean tree",
                string.Empty,
                string.Empty,
                DiffViewMode.Inline);
        }

        return tab switch
        {
            GitDiffTab.StagedSnapshot => new GitDiffSnapshot(
                $"Index Snapshot | {selected.Path}",
                selected.HeadText,
                selected.IsStaged ? selected.IndexText : selected.HeadText,
                DiffViewMode.Inline),
            GitDiffTab.PatchRadar => new GitDiffSnapshot(
                $"Patch Radar | {selected.Path}",
                selected.HeadText,
                selected.WorktreeText,
                DiffViewMode.SideBySide),
            _ => new GitDiffSnapshot(
                $"Working Copy | {selected.Path}",
                selected.IsStaged ? selected.IndexText : selected.HeadText,
                selected.WorktreeText,
                DiffViewMode.Inline)
        };
    }

    public static IReadOnlyList<CommandOutputLine> BuildSeedHistory()
    {
        var now = DateTimeOffset.UtcNow;
        return
        [
            new CommandOutputLine("scan workspace --hydrate diff cache", CommandOutputChannel.System,
                now.AddMinutes(-8)),
            new CommandOutputLine("review lane warmed; 2 high-signal patches detected", CommandOutputChannel.StdOut,
                now.AddMinutes(-7)),
            new CommandOutputLine("queue docs/public-api-inventory.md for the release train",
                CommandOutputChannel.StdOut, now.AddMinutes(-5)),
            new CommandOutputLine("rebuild flagship shell after examples reset", CommandOutputChannel.System,
                now.AddMinutes(-4)),
            new CommandOutputLine("origin/public-v1 moved +1 commit ahead of the local base",
                CommandOutputChannel.StdErr, now.AddMinutes(-2)),
            new CommandOutputLine("commit deck armed; waiting for intent", CommandOutputChannel.System,
                now.AddSeconds(-40))
        ];
    }

    public GitActionResult ToggleStageSelected()
    {
        var selected = SelectedFile;
        if (selected is null)
        {
            return GitActionResult.WarningResult("Stage skipped", "No file selected.");
        }

        selected.IsStaged = !selected.IsStaged;
        LastAction = selected.IsStaged ? $"Staged {selected.Path}" : $"Moved {selected.Path} back to working tree";
        LastActionDetail = selected.IsStaged
            ? $"Queued {selected.Summary.ToLowerInvariant()} for the next commit."
            : "Keeping edits live in the worktree for another pass.";
        NormalizeSelection();
        return selected.IsStaged
            ? GitActionResult.SuccessResult("Staged file", $"{selected.Path} is now in the shiproom queue.")
            : GitActionResult.InfoResult("Returned to worktree", $"{selected.Path} left the staged queue.");
    }

    public GitActionResult DiscardSelected()
    {
        var selected = SelectedFile;
        if (selected is null)
        {
            return GitActionResult.WarningResult("Discard skipped", "No file selected.");
        }

        _files.Remove(selected);
        LastAction = $"Discarded {selected.Path}";
        LastActionDetail = "Removed the patch from both worktree and queue.";
        NormalizeSelection();
        return GitActionResult.WarningResult("Discarded patch", $"{selected.Path} is clean again.");
    }

    public GitActionResult CommitStaged(string subject, string body)
    {
        var staged = _files.Where(static file => file.IsStaged).ToArray();
        if (staged.Length == 0)
        {
            return GitActionResult.WarningResult("Commit skipped", "Stage at least one file first.");
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            return GitActionResult.ErrorResult("Commit blocked", "Subject cannot be empty.");
        }

        foreach (var file in staged)
        {
            _files.Remove(file);
        }

        CommitSequence++;
        Ahead++;
        Behind = Math.Max(0, Behind - 1);
        LastAction = $"Committed {staged.Length} file{(staged.Length == 1 ? string.Empty : "s")}";
        LastActionDetail = $"[{BranchName} {CommitSequence:X6}] {subject.Trim()}" +
                           (string.IsNullOrWhiteSpace(body) ? string.Empty : " + notes");
        NormalizeSelection();
        return GitActionResult.SuccessResult("Commit created",
            $"{subject.Trim()} ({staged.Length} file{(staged.Length == 1 ? string.Empty : "s")}).");
    }

    public GitActionResult Sync()
    {
        var hadTraffic = Ahead > 0 || Behind > 0;
        Ahead = 0;
        Behind = 0;
        LastAction = hadTraffic ? $"Synced {RemoteName}/{BranchName}" : $"Checked {RemoteName}/{BranchName}";
        LastActionDetail = hadTraffic
            ? "Push and fetch lanes are now clean."
            : "Remote already matched local state.";
        return GitActionResult.InfoResult(hadTraffic ? "Sync complete" : "Remote steady", LastActionDetail);
    }

    public void AdvancePulse()
    {
        PulseIndex = (PulseIndex + 1) % 4;
    }

    private IEnumerable<GitFileEntry> GetVisibleFiles()
    {
        return Scope switch
        {
            GitScope.Changes => _files,
            GitScope.ReviewQueue => _files.Where(static file => file.IsReviewCritical || file.IsStaged),
            GitScope.Shiproom => _files.Where(static file => file.IsStaged),
            _ => _files
        };
    }

    private void NormalizeSelection()
    {
        if (SelectedFileId is not null && GetVisibleFiles()
                .Any(file => string.Equals(file.Id, SelectedFileId, StringComparison.Ordinal)))
        {
            return;
        }

        SelectedFileId = GetVisibleFiles().FirstOrDefault()?.Id;
    }

    private static GitWorktreeSection BuildSection(string title, GitFileEntry[] files)
    {
        return new GitWorktreeSection(title, files);
    }

    private static List<GitFileEntry> CreateSeedFiles()
    {
        return
        [
            new GitFileEntry(
                "runtime-loop",
                "src/Tessera/Internal/TesseraRuntimeBridge.cs",
                GitChangeKind.Modified,
                "Throttle idle redraws under mouse-motion load",
                18,
                4,
                "runtime",
                true,
                true,
                """
                private async Task RenderLoopAsync(CancellationToken cancellationToken)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await RenderFrameAsync(cancellationToken).ConfigureAwait(false);
                        await Task.Delay(_options.FrameDelay, cancellationToken).ConfigureAwait(false);
                    }
                }
                """,
                """
                private async Task RenderLoopAsync(CancellationToken cancellationToken)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await RenderFrameAsync(cancellationToken).ConfigureAwait(false);
                        await DelayForFrameBudgetAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                """,
                """
                private async Task RenderLoopAsync(CancellationToken cancellationToken)
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await RenderFrameAsync(cancellationToken).ConfigureAwait(false);
                        await DelayForFrameBudgetAsync(cancellationToken).ConfigureAwait(false);
                    }
                }
                """),
            new GitFileEntry(
                "public-api-docs",
                "docs/public-api-inventory.md",
                GitChangeKind.Modified,
                "Tighten onboarding and flagship-example wording",
                27,
                9,
                "docs",
                true,
                false,
                """
                ## Examples
                - HelloWorld
                - CounterForm
                - WorkspaceApp
                """,
                """
                ## Examples
                - DataWorkbench
                - OpsWatch
                - GitConsole
                """,
                """
                ## Examples
                - DataWorkbench
                - OpsWatch
                - GitConsole
                """),
            new GitFileEntry(
                "gitconsole-app",
                "examples/GitConsole/GitConsoleApp.cs",
                GitChangeKind.Added,
                "Flagship app shell, orchestration, seeded repo state",
                196,
                0,
                "examples",
                false,
                true,
                string.Empty,
                string.Empty,
                """
                internal sealed class GitConsoleApp : TesseraApp
                {
                    // flagship shell wiring here
                }
                """),
            new GitFileEntry(
                "worktree-control",
                "examples/GitConsole/GitWorktreeControl.cs",
                GitChangeKind.Added,
                "Grouped worktree list with badges and review heat",
                143,
                0,
                "examples",
                false,
                false,
                string.Empty,
                string.Empty,
                """
                internal sealed class GitWorktreeControl : Control
                {
                    // grouped selection surface
                }
                """),
            new GitFileEntry(
                "perf-gate-runner",
                "benchmarks/Tessera.Benchmarks/PerfGateRunner.cs",
                GitChangeKind.Modified,
                "Direct perf gate runner; baseline-backed startup and latency checks",
                48,
                11,
                "perf",
                false,
                false,
                "dotnet run --project benchmarks/Tessera.Benchmarks -- --perf-gate\n",
                "dotnet run --project benchmarks/Tessera.Benchmarks -- --perf-gate\n",
                "dotnet run --project benchmarks/Tessera.Benchmarks --configuration Release -- --perf-gate --baseline docs/perf-baselines/v1-slo-gate-baseline.json --output docs/perf-baselines/latest-slo-gate-result.json\n"),
            new GitFileEntry(
                "release-notes",
                "docs/alpha-release-checklist.md",
                GitChangeKind.Modified,
                "Capture flagship-example Alpha gate follow-up",
                12,
                3,
                "release",
                false,
                true,
                "- M4 pending manual signoff\n",
                "- M4 pending manual signoff\n",
                "- M4 pending manual signoff\n- flagship GitConsole visual review scheduled\n")
        ];
    }
}

internal sealed class GitFileEntry(
    string id,
    string path,
    GitChangeKind kind,
    string summary,
    int addedLines,
    int removedLines,
    string owner,
    bool isStaged,
    bool isReviewCritical,
    string headText,
    string indexText,
    string worktreeText)
{
    public string Id { get; } = id;
    public string Path { get; } = path;
    public GitChangeKind Kind { get; } = kind;
    public string Summary { get; } = summary;
    public int AddedLines { get; } = addedLines;
    public int RemovedLines { get; } = removedLines;
    public string Owner { get; } = owner;
    public bool IsStaged { get; set; } = isStaged;
    public bool IsReviewCritical { get; } = isReviewCritical;
    public string HeadText { get; } = headText;
    public string IndexText { get; } = indexText;
    public string WorktreeText { get; } = worktreeText;
}

internal sealed record GitRepoMetrics(int Staged, int Changed, int Review, int Total);

internal sealed record GitWorktreeSection(string Title, IReadOnlyList<GitFileEntry> Items);

internal sealed record GitDiffSnapshot(string Title, string OldText, string NewText, DiffViewMode Mode);

internal sealed record GitActionResult(bool Success, string Title, string Detail, CommandOutputChannel Channel)
{
    public static GitActionResult SuccessResult(string title, string detail)
    {
        return new GitActionResult(true, title, detail, CommandOutputChannel.StdOut);
    }

    public static GitActionResult InfoResult(string title, string detail)
    {
        return new GitActionResult(true, title, detail, CommandOutputChannel.System);
    }

    public static GitActionResult WarningResult(string title, string detail)
    {
        return new GitActionResult(false, title, detail, CommandOutputChannel.StdErr);
    }

    public static GitActionResult ErrorResult(string title, string detail)
    {
        return new GitActionResult(false, title, detail, CommandOutputChannel.StdErr);
    }
}
