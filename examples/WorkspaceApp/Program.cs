using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

var app = Tea.CreateBuilder()
    .UseApp<WorkspaceApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Workspace App",
            EnableFocusReporting = true,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record WorkspaceLoadRequested(string Reason) : Message;
internal sealed record WorkspaceLoaded(IReadOnlyList<WorkspaceProject> Projects, DateTimeOffset LoadedAt) : Message;
internal sealed record WorkspaceSelected(string ProjectId) : Message;

internal sealed record WorkspaceProject(string Id, string Name, string Status, string Owner, string Summary);

internal sealed class WorkspaceApp : TeaApp
{
    private readonly ListView<WorkspaceProject> _projects = new(project => $"{project.Name} [{project.Status}]")
    {
        Title = "Projects",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Button _reload = new()
    {
        Text = "Reload",
        Description = "r reloads asynchronously",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Label _details = new()
    {
        Title = "Details",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly Label _activity = new()
    {
        Title = "Activity",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new();
    private readonly List<string> _activityEntries = [];
    private readonly Dictionary<string, WorkspaceProject> _projectsById = new(StringComparer.Ordinal);

    private bool _isLoading;
    private DateTimeOffset _lastLoadedAt;

    public WorkspaceApp()
    {
        _projects.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null)
            {
                Post(new WorkspaceSelected(args.SelectedItem.Id));
            }
        };

        _reload.Activated += (_, _) => Post(new WorkspaceLoadRequested("reload button"));
        AddActivity("Workspace started. Loading projects...");
    }

    public override TeaEffect? Initialize()
    {
        _isLoading = true;
        return CreateLoadEffect("startup");
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is KeyPressed key && key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (message is KeyPressed reloadKey && reloadKey.IsCharacter('r'))
        {
            _isLoading = true;
            AddActivity("Reload requested from keyboard.");
            return CreateLoadEffect("keyboard");
        }

        switch (message)
        {
            case WorkspaceLoadRequested requested:
                _isLoading = true;
                AddActivity($"Reload requested from {requested.Reason}.");
                return CreateLoadEffect(requested.Reason);
            case WorkspaceLoaded loaded:
                ApplyLoadedProjects(loaded);
                break;
            case WorkspaceSelected selected:
                ShowProject(selected.ProjectId);
                break;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _activity.Text = string.Join(Environment.NewLine, _activityEntries);
        _status.LeftText = _isLoading
            ? "Loading project data..."
            : $"Loaded {_projects.Count} projects";
        _status.RightText = $"Last sync {_lastLoadedAt:HH:mm:ss}  Ctrl+C quits";

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(5, header => header.Center(_reload, width: 30, height: 5));
            window.Left(Math.Min(42, Math.Max(32, context.Width / 3)), _projects);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(8, _details);
                column.Fill(_activity);
            }));
            window.Footer(1, _status);
        });
    }

    private static TeaEffect CreateLoadEffect(string reason) => async cancellationToken =>
    {
        await Task.Delay(TimeSpan.FromMilliseconds(450), cancellationToken).ConfigureAwait(false);
        var projects = CreateSnapshot(reason);
        return new WorkspaceLoaded(projects, DateTimeOffset.UtcNow);
    };

    private void ApplyLoadedProjects(WorkspaceLoaded loaded)
    {
        _projectsById.Clear();
        for (var index = 0; index < loaded.Projects.Count; index++)
        {
            _projectsById[loaded.Projects[index].Id] = loaded.Projects[index];
        }

        _projects.SetItems(loaded.Projects);
        _isLoading = false;
        _lastLoadedAt = loaded.LoadedAt.ToLocalTime();
        AddActivity($"Loaded {_projectsById.Count} projects.");

        if (_projects.SelectedItem is not null)
        {
            ShowProject(_projects.SelectedItem.Id);
        }
    }

    private void ShowProject(string projectId)
    {
        if (!_projectsById.TryGetValue(projectId, out var project))
        {
            _details.Text = "No project selected.";
            return;
        }

        _details.Text =
            $"""
             {project.Name}
             Owner: {project.Owner}
             Status: {project.Status}
             
             {project.Summary}
             """;

        AddActivity($"Selected {project.Name}.");
    }

    private void AddActivity(string message)
    {
        _activityEntries.Insert(0, $"{DateTimeOffset.Now:HH:mm:ss}  {message}");
        while (_activityEntries.Count > 8)
        {
            _activityEntries.RemoveAt(_activityEntries.Count - 1);
        }
    }

    private static IReadOnlyList<WorkspaceProject> CreateSnapshot(string reason)
    {
        var status = reason.Equals("startup", StringComparison.Ordinal)
            ? "Bootstrapping"
            : "Active";

        return
        [
            new WorkspaceProject("proj-1", "Shell Host", status, "Ava", "Stabilize terminal resize and focus transitions."),
            new WorkspaceProject("proj-2", "Task Board", "Review", "Noah", "Ship keyboard shortcuts for list and table workflows."),
            new WorkspaceProject("proj-3", "Docs Refresh", "Ready", "Mia", "Publish migration notes and onboarding walkthrough."),
        ];
    }
}
