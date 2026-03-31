using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

namespace TeaSharp.Examples.DataWorkbench;

internal sealed partial class DataWorkbenchApp : TeaApp
{
    private DataWorkbenchPalette _palette = DataWorkbenchTheme.Default;
    private readonly DataWorkbenchState _state = DataWorkbenchState.CreateSeed();

    private readonly DataWorkbenchHeaderControl _header = new() { Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _slicePulse = new() { Title = "Slice Pulse", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _velocityPulse = new() { Title = "Query Velocity", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };
    private readonly StatsCard _comparePulse = new() { Title = "Compare Pocket", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0) };

    private readonly PaneTabs _pageTabs = new() { Title = "Workspace", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0), FocusMarker = "◆" };
    private readonly SideNavRail _sourceRail = new() { Title = "Sources · F1", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆" };
    private readonly SearchBox _search = new() { Title = "Search Slice", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", Placeholder = "search entity, owner, region, or narrative" };
    private readonly QueryBuilder _query = new() { Title = "Rule Composer · Q", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆" };
    private readonly DataGrid _results = new() { Title = "Result Grid · F2", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", PageSize = 14 };
    private readonly PaneTabs _inspectTabs = new() { Title = "Inspector", Border = BorderStyle.Rounded, Padding = Thickness.Symmetric(1, 0), FocusMarker = "◆" };
    private readonly RichTextView _profileView = new() { Title = "Record Profile · F3", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆" };
    private readonly JsonTreeView _jsonView = new() { Title = "JSON Payload · F3", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆" };
    private readonly CommandOutput _traceView = new() { Title = "Trace Lens · F3", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", AutoFollow = true, ShowTimestamp = false };

    private readonly RichTextView _compareLeft = new() { Title = "Pinned Record", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly RichTextView _compareRight = new() { Title = "Current Record", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly RichTextView _compareSummary = new() { Title = "Compare Briefing", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };

    private readonly ActivityFeed _activity = new() { Title = "Investigation Feed · F4", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", ShowTimestamp = true, AutoFollow = true };
    private readonly CommandOutput _output = new() { Title = "Execution Lane · F4", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆", AutoFollow = true, ShowTimestamp = true };

    private readonly SideNavRail _savedViews = new() { Title = "Saved Views", Border = BorderStyle.Rounded, Padding = Thickness.All(1), FocusMarker = "◆" };
    private readonly RichTextView _savedPreview = new() { Title = "View Briefing", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly RichTextView _savedRunbook = new() { Title = "Re-entry Runbook", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };

    private readonly Button _runButton = new() { Text = "Run Slice", Description = "r", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _pinButton = new() { Text = "Pin Compare", Description = "p", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _saveButton = new() { Text = "Save View", Description = "b", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _exportButton = new() { Text = "Export Slice", Description = "e", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _clearButton = new() { Text = "Clear Search", Description = "/", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _citrineButton = new() { Text = "Citrine", Description = "7", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _cobaltButton = new() { Text = "Cobalt", Description = "8", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };
    private readonly Button _emberButton = new() { Text = "Ember", Description = "9", Border = BorderStyle.Rounded, Padding = Thickness.All(1) };

    private readonly StatusBar _footer = new() { Fill = ' ' };

    private string _sourceId = "fraud_signals";
    private DataWorkbenchPage _page = DataWorkbenchPage.Explore;
    private string? _pinnedRecordId;
    private string? _selectedSavedViewId;
    private string? _inspectedRecordId;
    private IReadOnlyList<WorkbenchRecord> _visibleRecords = [];

    public DataWorkbenchApp()
    {
        ApplyTheme(_palette);
        WireEvents();
        SeedControls();
        ApplySourcePreset(_sourceId);
        _results.RequestFocus();
    }

    public override TeaEffect? Initialize() =>
        TeaEffects.Periodic(TimeSpan.FromMilliseconds(1400), _ => new DataWorkbenchTickMessage());

    public override TeaEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key:
                return HandleKey(key);
            case DataWorkbenchActionMessage action:
                ExecuteAction(action.Action);
                return null;
            case DataWorkbenchThemeMessage theme:
                ApplyTheme(DataWorkbenchTheme.Resolve(theme.Kind));
                return null;
            case DataWorkbenchTickMessage:
                _state.Advance();
                return null;
            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshData();
        RefreshChrome();
        RefreshViews();

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
            _sourceRail.RequestFocus();
            return null;
        }

        if (key.Is(Key.F2))
        {
            _results.RequestFocus();
            return null;
        }

        if (key.Is(Key.F3))
        {
            FocusInspector();
            return null;
        }

        if (key.Is(Key.F4))
        {
            FocusBottomLane();
            return null;
        }

        if (key.IsCharacter('/'))
        {
            _search.RequestFocus();
            return null;
        }

        if (key.IsCharacter('q'))
        {
            _query.RequestFocus();
            return null;
        }

        if (key.IsCharacter('1'))
        {
            SelectPage(DataWorkbenchPage.Explore);
            return null;
        }

        if (key.IsCharacter('2'))
        {
            SelectPage(DataWorkbenchPage.Compare);
            return null;
        }

        if (key.IsCharacter('3'))
        {
            SelectPage(DataWorkbenchPage.History);
            return null;
        }

        if (key.IsCharacter('4'))
        {
            SelectPage(DataWorkbenchPage.Saved);
            return null;
        }

        if (key.IsCharacter('7'))
        {
            Post(new DataWorkbenchThemeMessage(DataWorkbenchThemeKind.Citrine));
            return null;
        }

        if (key.IsCharacter('8'))
        {
            Post(new DataWorkbenchThemeMessage(DataWorkbenchThemeKind.Cobalt));
            return null;
        }

        if (key.IsCharacter('9'))
        {
            Post(new DataWorkbenchThemeMessage(DataWorkbenchThemeKind.Ember));
            return null;
        }

        if (IsSearchEditing())
        {
            return null;
        }

        if (key.IsCharacter('r'))
        {
            ExecuteAction(DataWorkbenchAction.RunSlice);
            return null;
        }

        if (key.IsCharacter('p'))
        {
            ExecuteAction(DataWorkbenchAction.PinCompare);
            return null;
        }

        if (key.IsCharacter('b'))
        {
            ExecuteAction(DataWorkbenchAction.SaveView);
            return null;
        }

        if (key.IsCharacter('e'))
        {
            ExecuteAction(DataWorkbenchAction.ExportSlice);
            return null;
        }

        if (key.Is(Key.Tab))
        {
            CycleInspector();
            return null;
        }

        return null;
    }

    private void WireEvents()
    {
        _pageTabs.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _page = args.SelectedIndex switch
            {
                1 => DataWorkbenchPage.Compare,
                2 => DataWorkbenchPage.History,
                3 => DataWorkbenchPage.Saved,
                _ => DataWorkbenchPage.Explore,
            };
        };

        _sourceRail.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _sourceId = args.SelectedItem.Id;
            ApplySourcePreset(_sourceId);
        };

        _search.QueryChanged += (_, _) => { };
        _runButton.Activated += (_, _) => Post(new DataWorkbenchActionMessage(DataWorkbenchAction.RunSlice));
        _pinButton.Activated += (_, _) => Post(new DataWorkbenchActionMessage(DataWorkbenchAction.PinCompare));
        _saveButton.Activated += (_, _) => Post(new DataWorkbenchActionMessage(DataWorkbenchAction.SaveView));
        _exportButton.Activated += (_, _) => Post(new DataWorkbenchActionMessage(DataWorkbenchAction.ExportSlice));
        _clearButton.Activated += (_, _) => Post(new DataWorkbenchActionMessage(DataWorkbenchAction.ClearSearch));
        _citrineButton.Activated += (_, _) => Post(new DataWorkbenchThemeMessage(DataWorkbenchThemeKind.Citrine));
        _cobaltButton.Activated += (_, _) => Post(new DataWorkbenchThemeMessage(DataWorkbenchThemeKind.Cobalt));
        _emberButton.Activated += (_, _) => Post(new DataWorkbenchThemeMessage(DataWorkbenchThemeKind.Ember));
        _savedViews.SelectionChanged += (_, args) => _selectedSavedViewId = args.SelectedItem?.Id;
    }

    private void SeedControls()
    {
        _pageTabs.SetTabs(
        [
            new PaneTabItem("explore", "Explore"),
            new PaneTabItem("compare", "Compare"),
            new PaneTabItem("history", "History"),
            new PaneTabItem("saved", "Saved Views"),
        ]);
        _pageTabs.SetSelectedIndex(0);

        _inspectTabs.SetTabs(
        [
            new PaneTabItem("profile", "Profile"),
            new PaneTabItem("json", "JSON"),
            new PaneTabItem("trace", "Trace"),
        ]);
        _inspectTabs.SetSelectedIndex(0);

        _sourceRail.SetItems(_state.BuildNavItems(_sourceId));
        _sourceRail.SetSelectedIndex(0);
        _savedViews.SetItems(BuildSavedViewItems());
        if (_savedViews.Items.Count > 0)
        {
            _savedViews.SetSelectedIndex(0);
            _selectedSavedViewId = _savedViews.SelectedItem?.Id;
        }

        _query.ShowQueryPreview = true;
        _query.SelectedMarker = "◆";
        _query.UnselectedMarker = "·";
        _results.SetColumns(DataWorkbenchState.BuildColumns());
        _activity.MaxItems = 96;
        _activity.SetItems(DataWorkbenchState.BuildSeedActivities());
        _output.SetLines(DataWorkbenchState.BuildSeedOutput());
    }

    private void RefreshData()
    {
        _visibleRecords = _state.FilterRecords(_sourceId, _search.QueryText, _query.Rules);
        _results.SetRows(DataWorkbenchState.BuildRows(_visibleRecords));
        _search.SetMatchState(_visibleRecords.Count, _visibleRecords.Count == 0 ? null : 0);
    }

    private void RefreshChrome()
    {
        var current = SelectedRecord();
        var source = _state.GetSource(_sourceId);

        _header.Title = "DataWorkbench // Investigation Console";
        _header.ClockText = _state.ClockText;
        _header.WorkspaceText = "atlas warehouse";
        _header.SourceText = source.Label;
        _header.ViewText = _page.ToString().ToLowerInvariant();
        _header.SummaryText = _state.BuildWorkspaceSummary(_sourceId, _visibleRecords);
        _header.PromptText = DataWorkbenchState.BuildPrompt(current);

        _slicePulse.SetItems(DataWorkbenchState.BuildPulseItems(_visibleRecords));
        _velocityPulse.SetItems(DataWorkbenchState.BuildVelocityItems(_visibleRecords));
        _comparePulse.SetItems(DataWorkbenchState.BuildCompareItems(PinnedRecord(), current));

        _footer.LeftText = $"dataworkbench  {_palette.Label.ToLowerInvariant()}  {source.SourceTag}  rows {_visibleRecords.Count:00}  pinned {(PinnedRecord()?.Id ?? "none")}";
        _footer.RightText = "1-4 pages  7/8/9 themes  / search  q rules  r run  p pin  b save  e export  F1/F2/F3/F4 focus";
    }

    private void RefreshViews()
    {
        var current = SelectedRecord();
        var pinned = PinnedRecord();

        _profileView.SetPlainText(DataWorkbenchState.BuildSummary(current));
        _traceView.SetLines(BuildTraceLines(current));
        RefreshJson(current);

        _compareLeft.SetPlainText(DataWorkbenchState.BuildSummary(pinned));
        _compareRight.SetPlainText(DataWorkbenchState.BuildSummary(current));
        _compareSummary.SetPlainText(DataWorkbenchState.BuildCompareSummary(pinned, current));

        _savedPreview.SetPlainText(BuildSavedViewPreview());
        _savedRunbook.SetPlainText(BuildRunbookText());
    }

    private void RefreshJson(WorkbenchRecord? current)
    {
        if (current is null)
        {
            _jsonView.SetRoots([]);
            _inspectedRecordId = null;
            return;
        }

        if (string.Equals(_inspectedRecordId, current.Id, StringComparison.Ordinal))
        {
            return;
        }

        _jsonView.SetJson(current.Json);
        _inspectedRecordId = current.Id;
    }
}

internal enum DataWorkbenchAction
{
    RunSlice,
    PinCompare,
    SaveView,
    ExportSlice,
    ClearSearch,
}

internal sealed record DataWorkbenchActionMessage(DataWorkbenchAction Action) : Message;
internal sealed record DataWorkbenchThemeMessage(DataWorkbenchThemeKind Kind) : Message;
internal sealed record DataWorkbenchTickMessage() : Message;
