using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Examples.DataWorkbench;

internal sealed partial class DataWorkbenchApp
{
    private void ExecuteAction(DataWorkbenchAction action)
    {
        switch (action)
        {
            case DataWorkbenchAction.RunSlice:
                _output.AppendStdOut($"run slice / {_sourceId} / {_visibleRecords.Count} rows / query '{BuildQuerySummary()}'");
                _activity.SetItems(
                [
                    new ActivityFeedItem("query", "ran", _sourceId, $"{_visibleRecords.Count} rows / {BuildQuerySummary()}", ActivityFeedItemKind.Info),
                    .. _activity.Items,
                ]);
                break;
            case DataWorkbenchAction.PinCompare:
                _pinnedRecordId = SelectedRecord()?.Id;
                _page = DataWorkbenchPage.Compare;
                _pageTabs.SetSelectedIndex(1);
                _output.AppendStdOut($"compare pin / {(_pinnedRecordId ?? "none")}");
                break;
            case DataWorkbenchAction.SaveView:
                var saved = _state.SaveView(_sourceId, _search.QueryText, _query.Rules);
                _savedViews.SetItems(BuildSavedViewItems());
                _selectedSavedViewId = saved.Id;
                _output.AppendStdOut($"saved lens / {saved.Label}");
                break;
            case DataWorkbenchAction.ExportSlice:
                _output.AppendStdOut($"export slice / {_sourceId} / {_visibleRecords.Count} records / ndjson handoff queued");
                _activity.SetItems(
                [
                    new ActivityFeedItem("export", "queued", _sourceId, $"{_visibleRecords.Count} records pushed to analyst handoff", ActivityFeedItemKind.Success),
                    .. _activity.Items,
                ]);
                break;
            case DataWorkbenchAction.ClearSearch:
                _search.ClearQuery();
                break;
        }
    }

    private void ApplySourcePreset(string sourceId)
    {
        _search.ClearQuery();
        _query.SetRules(DefaultRulesForSource(sourceId));
        _results.SetColumns(DataWorkbenchState.BuildColumns());
        _sourceRail.SetItems(_state.BuildNavItems(sourceId));
        var selectedIndex = _state.Sources
            .Select((source, index) => new { source.Id, index })
            .First(entry => string.Equals(entry.Id, sourceId, StringComparison.Ordinal)).index;
        _sourceRail.SetSelectedIndex(selectedIndex);
    }

    private static IReadOnlyList<QueryRule> DefaultRulesForSource(string sourceId)
    {
        return sourceId switch
        {
            "fraud_signals" => [new QueryRule("score", QueryOperator.GreaterThanOrEqual, "70"), new QueryRule("region", QueryOperator.Contains, "eu")],
            "fulfillment_holds" => [new QueryRule("status", QueryOperator.NotEquals, "cleared"), new QueryRule("latency", QueryOperator.GreaterThan, "400")],
            "refund_journal" => [new QueryRule("status", QueryOperator.NotEquals, "posted"), new QueryRule("score", QueryOperator.GreaterThanOrEqual, "60")],
            "catalog_drift" => [new QueryRule("status", QueryOperator.NotEquals, "clear"), new QueryRule("score", QueryOperator.GreaterThanOrEqual, "55")],
            _ => [],
        };
    }

    private string BuildSavedViewPreview()
    {
        var selected = _state.SavedViews.FirstOrDefault(view => string.Equals(view.Id, _selectedSavedViewId, StringComparison.Ordinal))
            ?? (_state.SavedViews.Count > 0 ? _state.SavedViews[0] : null);
        if (selected is null)
        {
            return "No saved views yet.";
        }

        return string.Join(
            '\n',
            $"Lens       {selected.Label}",
            $"Source     {selected.SourceId}",
            $"Query      {(string.IsNullOrWhiteSpace(selected.Query) ? "(none)" : selected.Query)}",
            $"Rules      {selected.RuleSummary}",
            $"Why        {selected.Description}");
    }

    private static string BuildRunbookText()
    {
        return string.Join(
            '\n',
            "Saved-view runbook",
            "1. Open the lens from the left rail.",
            "2. Validate row pressure in Explore.",
            "3. Pin an outlier before moving to Compare.",
            "4. Use History to replay the last investigative handoff.");
    }

    private NavItem[] BuildSavedViewItems()
    {
        return _state.SavedViews
            .Select(view => new NavItem(view.Id, view.Label, "SV", view.SourceId[..Math.Min(2, view.SourceId.Length)].ToUpperInvariant()))
            .ToArray();
    }

    private static CommandOutputLine[] BuildTraceLines(WorkbenchRecord? record)
    {
        if (record is null)
        {
            return [new CommandOutputLine("trace unavailable", CommandOutputChannel.System, DateTimeOffset.UtcNow)];
        }

        return DataWorkbenchState.BuildTrace(record)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(line => new CommandOutputLine(line, CommandOutputChannel.StdOut, DateTimeOffset.UtcNow))
            .ToArray();
    }

    private WorkbenchRecord? SelectedRecord()
    {
        if (_visibleRecords.Count == 0)
        {
            return null;
        }

        var selectedIndex = _results.SelectedRowIndex;
        if (selectedIndex < 0 || selectedIndex >= _visibleRecords.Count)
        {
            return _visibleRecords[0];
        }

        return _visibleRecords[selectedIndex];
    }

    private WorkbenchRecord? PinnedRecord()
    {
        if (string.IsNullOrWhiteSpace(_pinnedRecordId))
        {
            return null;
        }

        return _state.FindRecord(_sourceId, _pinnedRecordId);
    }

    private string BuildQuerySummary()
    {
        if (_query.Rules.Count == 0 && string.IsNullOrWhiteSpace(_search.QueryText))
        {
            return "open slice";
        }

        return string.Join(" / ",
            string.IsNullOrWhiteSpace(_search.QueryText) ? "no search" : _search.QueryText.Trim(),
            _query.Rules.Count == 0 ? "no rules" : $"{_query.Rules.Count} rules");
    }

    private bool IsSearchEditing() => _search.IsFocused;

    private void FocusInspector()
    {
        switch (_inspectTabs.SelectedItem?.Id)
        {
            case "json":
                _jsonView.RequestFocus();
                break;
            case "trace":
                _traceView.RequestFocus();
                break;
            default:
                _profileView.RequestFocus();
                break;
        }
    }

    private void FocusBottomLane()
    {
        if (_page == DataWorkbenchPage.History)
        {
            _activity.RequestFocus();
            return;
        }

        _output.RequestFocus();
    }

    private void CycleInspector()
    {
        if (_inspectTabs.Tabs.Count == 0)
        {
            return;
        }

        _inspectTabs.SetSelectedIndex((_inspectTabs.SelectedIndex + 1) % _inspectTabs.Tabs.Count);
    }

    private void SelectPage(DataWorkbenchPage page)
    {
        _page = page;
        _pageTabs.SetSelectedIndex(page switch
        {
            DataWorkbenchPage.Compare => 1,
            DataWorkbenchPage.History => 2,
            DataWorkbenchPage.Saved => 3,
            _ => 0,
        });
    }

    private void ApplyTheme(DataWorkbenchPalette palette)
    {
        _palette = palette;
        var theme = palette.Theme;

        _sourceRail.ApplyTheme(theme);
        _search.ApplyTheme(theme);
        _query.ApplyTheme(theme);
        _results.ApplyTheme(theme);
        _profileView.ApplyTheme(theme);
        _jsonView.ApplyTheme(theme);
        _traceView.ApplyTheme(theme);
        _compareLeft.ApplyTheme(theme);
        _compareRight.ApplyTheme(theme);
        _compareSummary.ApplyTheme(theme);
        _activity.ApplyTheme(theme);
        _output.ApplyTheme(theme);
        _savedViews.ApplyTheme(theme);
        _savedPreview.ApplyTheme(theme);
        _savedRunbook.ApplyTheme(theme);
        _pageTabs.ApplyTheme(theme);
        _inspectTabs.ApplyTheme(theme);
        _slicePulse.ApplyTheme(theme);
        _velocityPulse.ApplyTheme(theme);
        _comparePulse.ApplyTheme(theme);
        _runButton.ApplyTheme(theme);
        _pinButton.ApplyTheme(theme);
        _saveButton.ApplyTheme(theme);
        _exportButton.ApplyTheme(theme);
        _clearButton.ApplyTheme(theme);
        _citrineButton.ApplyTheme(theme);
        _cobaltButton.ApplyTheme(theme);
        _emberButton.ApplyTheme(theme);
        _footer.ApplyTheme(theme);

        _header.TitleStyle = DataWorkbenchTheme.Foreground(palette.HeroTitle).WithBold();
        _header.ClockStyle = DataWorkbenchTheme.Foreground(palette.HeroClock).WithBold();
        _header.BadgeStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
        _header.SummaryStyle = theme.Text.Secondary;
        _header.PromptStyle = DataWorkbenchTheme.Foreground(palette.HeroAccent).WithBold();
        _header.BorderStyleText = theme.Border.Strong;
        _header.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);

        ConfigurePulse(_slicePulse, DataWorkbenchTheme.Foreground(palette.StatPrimary).WithBold());
        ConfigurePulse(_velocityPulse, DataWorkbenchTheme.Foreground(palette.StatSecondary).WithBold());
        ConfigurePulse(_comparePulse, DataWorkbenchTheme.Foreground(palette.StatTertiary).WithBold());

        _sourceRail.BorderStyleText = theme.Border.Strong;
        _sourceRail.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        _sourceRail.SelectedItemStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
        _sourceRail.FocusedSelectedItemStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.FooterChipBackground);
        _sourceRail.HoveredItemStyle = theme.Accent.Secondary;

        _pageTabs.BorderStyleText = theme.Border.Strong;
        _pageTabs.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        _pageTabs.SelectedTabStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
        _pageTabs.FocusedSelectedTabStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.FooterChipBackground);
        _pageTabs.HoveredTabStyle = theme.Accent.Secondary;
        _pageTabs.SeparatorStyle = theme.Text.Muted;

        _inspectTabs.BorderStyleText = theme.Border.Strong;
        _inspectTabs.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
        _inspectTabs.SelectedTabStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.HighlightB);
        _inspectTabs.FocusedSelectedTabStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.FooterChipBackground);
        _inspectTabs.HoveredTabStyle = theme.Accent.Secondary;

        _results.HeaderStyle = theme.Text.Secondary.WithBold();
        _results.RowStyle = theme.Text.Primary;
        _results.SelectedRowStyle = DataWorkbenchTheme.Background(palette.HeroBadgeBackground).Merge(DataWorkbenchTheme.Foreground(palette.HeroBadgeForeground));
        _results.SelectedCellStyle = theme.Text.Primary.WithBold();
        _results.HoveredRowStyle = theme.Accent.Secondary;
        _results.HoveredCellStyle = theme.Accent.Primary.WithBold();
        _results.BorderStyleText = theme.Border.Strong;
        _results.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);

        ConfigureInspector(_profileView, theme, palette);
        ConfigureInspector(_jsonView, theme, palette);
        ConfigureInspector(_traceView, theme, palette);
        ConfigureInspector(_compareLeft, theme, palette);
        ConfigureInspector(_compareRight, theme, palette);
        ConfigureInspector(_compareSummary, theme, palette);
        ConfigureInspector(_savedPreview, theme, palette);
        ConfigureInspector(_savedRunbook, theme, palette);
        ConfigureInspector(_activity, theme, palette);
        ConfigureInspector(_output, theme, palette);
        ConfigureInspector(_savedViews, theme, palette);

        ConfigureAction(_runButton, palette.HeroBadgeForeground, palette.HighlightA);
        ConfigureAction(_pinButton, palette.HeroBadgeForeground, palette.HighlightB);
        ConfigureAction(_saveButton, palette.HeroBadgeForeground, palette.HighlightC);
        ConfigureAction(_exportButton, palette.HeroBadgeForeground, palette.StatTertiary);
        ConfigureAction(_clearButton, palette.HeroBadgeForeground, palette.FrameStrong);
        ConfigureThemeButton(_citrineButton, DataWorkbenchThemeKind.Citrine);
        ConfigureThemeButton(_cobaltButton, DataWorkbenchThemeKind.Cobalt);
        ConfigureThemeButton(_emberButton, DataWorkbenchThemeKind.Ember);

        _footer.LeftTextStyle = DataWorkbenchTheme.Chip(palette.FooterChipForeground, palette.FooterChipBackground);
        _footer.RightTextStyle = theme.Text.Secondary;
        _footer.FillStyle = theme.Surface.Panel;
    }

    private static void ConfigurePulse(StatsCard card, TeaStyle valueStyle)
    {
        card.ValueStyle = valueStyle;
        card.BorderStyleText = valueStyle;
    }

    private static void ConfigureInspector(Control control, TeaTheme theme, DataWorkbenchPalette palette)
    {
        switch (control)
        {
            case RichTextView rich:
                rich.BorderStyleText = theme.Border.Strong;
                rich.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
                break;
            case JsonTreeView json:
                json.BorderStyleText = theme.Border.Strong;
                json.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
                break;
            case CommandOutput output:
                output.BorderStyleText = theme.Border.Strong;
                output.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
                output.SystemStyle = theme.Text.Muted;
                output.StdOutStyle = theme.Text.Primary;
                output.StdErrStyle = theme.State.Error;
                output.SelectedLineStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
                break;
            case ActivityFeed feed:
                feed.BorderStyleText = theme.Border.Strong;
                feed.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
                feed.SelectedItemStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
                feed.FocusedSelectedItemStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.FooterChipBackground);
                break;
            case SideNavRail rail:
                rail.BorderStyleText = theme.Border.Strong;
                rail.FocusedBorderStyleText = theme.Border.Focused.Merge(theme.Focus.Border);
                rail.SelectedItemStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.HeroBadgeBackground);
                rail.FocusedSelectedItemStyle = DataWorkbenchTheme.Chip(palette.HeroBadgeForeground, palette.FooterChipBackground);
                break;
        }
    }

    private void ConfigureAction(Button button, int foregroundRgb, int backgroundRgb)
    {
        var labelStyle = DataWorkbenchTheme.Foreground(foregroundRgb).WithBold();
        var surfaceStyle = DataWorkbenchTheme.Background(backgroundRgb);
        button.Border = BorderStyle.Heavy;
        button.LabelPrefix = string.Empty;
        button.LabelSuffix = string.Empty;
        button.Padding = Thickness.Symmetric(1, 0);
        button.LabelStyle = labelStyle.WithBold();
        button.FocusedLabelStyle = labelStyle.WithBold();
        button.PressedLabelStyle = labelStyle.WithBold();
        button.SurfaceStyle = surfaceStyle;
        button.FocusedSurfaceStyle = surfaceStyle;
        button.PressedSurfaceStyle = surfaceStyle;
        button.BorderStyleText = DataWorkbenchTheme.Foreground(_palette.FrameStrong);
        button.FocusedBorderStyleText = _palette.Theme.Focus.Border;
    }

    private void ConfigureThemeButton(Button button, DataWorkbenchThemeKind kind)
    {
        var isSelected = _palette.Kind == kind;
        button.Border = BorderStyle.Heavy;
        button.LabelPrefix = string.Empty;
        button.LabelSuffix = string.Empty;
        button.Padding = Thickness.Symmetric(1, 0);
        button.LabelStyle = isSelected
            ? DataWorkbenchTheme.Foreground(_palette.HeroBadgeForeground).WithBold()
            : _palette.Theme.Text.Secondary.WithBold();
        button.FocusedLabelStyle = isSelected
            ? DataWorkbenchTheme.Foreground(_palette.HeroBadgeForeground).WithBold()
            : _palette.Theme.Text.Secondary.WithBold();
        button.PressedLabelStyle = button.LabelStyle;
        button.SurfaceStyle = isSelected
            ? DataWorkbenchTheme.Background(_palette.HeroBadgeBackground)
            : _palette.Theme.Surface.Overlay;
        button.FocusedSurfaceStyle = button.SurfaceStyle;
        button.PressedSurfaceStyle = isSelected
            ? DataWorkbenchTheme.Background(_palette.FooterChipBackground)
            : _palette.Theme.Selection.Background;
        button.BorderStyleText = isSelected
            ? DataWorkbenchTheme.Foreground(_palette.FrameStrong).WithBold()
            : DataWorkbenchTheme.Foreground(_palette.FrameMuted);
        button.FocusedBorderStyleText = _palette.Theme.Focus.Border;
    }
}
