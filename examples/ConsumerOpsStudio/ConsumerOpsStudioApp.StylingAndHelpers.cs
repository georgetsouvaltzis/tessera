using TeaSharp;
using TeaSharp.Styles;

internal sealed partial class ConsumerOpsStudioApp
{
    private void UpdateSummaryAndGauges(ScreenContext context)
    {
        var service = FindService(_selectedServiceId) ?? _services[0];
        var incident = FindIncident(_selectedIncidentId);
        var deployment = FindDeployment(_selectedDeploymentId);

        _selectionSummary.Text =
            $"""
            Service: {service.Name} ({service.Region})
            Status: {service.Status}
            Queue: {service.QueueDepth} msgs
            Incident: {(incident is null ? "n/a" : $"{incident.Id} [{incident.State}]")}
            Deploy: {(deployment is null ? "n/a" : $"{deployment.Id} {deployment.Stage}")}
            Viewport: {context.Width}x{context.Height}
            """;

        _errorBudgetGauge.Value = service.ErrorBudgetRemaining;
        _errorBudgetGauge.Label = $"{service.ErrorBudgetRemaining:0.0}%";

        _queueDepthGauge.Value = service.QueueDepth;
        _queueDepthGauge.Label = $"{service.QueueDepth} msgs";

        _latencyPlot.Title = $"{service.Name} Trend";
    }

    private void UpdateButtonState()
    {
        _ackButton.IsDisabled = (OpsPanelTab)_tabs.SelectedIndex != OpsPanelTab.Incidents;
        _rollbackButton.IsDisabled = false;

        var service = FindService(_selectedServiceId) ?? _services[0];
        _freezeButton.Text = service.IsWriteFrozen ? "Unfreeze Writes" : "Freeze Writes";
        _freezeButton.Description = service.IsWriteFrozen ? "f unfreezes writes" : "f freezes writes";
    }

    private bool HasOpenIncident(string serviceId)
    {
        for (var index = 0; index < _incidents.Count; index++)
        {
            if (_incidents[index].IsOpen && string.Equals(_incidents[index].ServiceId, serviceId, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    private ServiceSnapshot? FindService(string serviceId)
    {
        for (var index = 0; index < _services.Count; index++)
        {
            if (string.Equals(_services[index].Id, serviceId, StringComparison.Ordinal))
            {
                return _services[index];
            }
        }

        return null;
    }

    private IncidentTicket? FindIncident(string incidentId)
    {
        if (string.IsNullOrEmpty(incidentId))
        {
            return null;
        }

        for (var index = 0; index < _incidents.Count; index++)
        {
            if (string.Equals(_incidents[index].Id, incidentId, StringComparison.Ordinal))
            {
                return _incidents[index];
            }
        }

        return null;
    }

    private DeploymentRun? FindDeployment(string deploymentId)
    {
        if (string.IsNullOrEmpty(deploymentId))
        {
            return null;
        }

        for (var index = 0; index < _deployments.Count; index++)
        {
            if (string.Equals(_deployments[index].Id, deploymentId, StringComparison.Ordinal))
            {
                return _deployments[index];
            }
        }

        return null;
    }

    private void AppendLog(string message)
    {
        _activityLog.Append($"{DateTimeOffset.Now:HH:mm:ss}  {message}");
    }

    private void ApplyLocalOverrides()
    {
        var theme = _alertThemeEnabled ? ConsumerOpsStudioTheme.Alert : ConsumerOpsStudioTheme.Default;
        var selected = theme.Selection.Background.Merge(theme.Selection.Foreground);

        _navigation.TitleStyle = theme.Text.Secondary;
        _navigation.FocusedTitleStyle = theme.Focus.Title;
        _navigation.BorderStyleText = theme.Border.Default;
        _navigation.FocusedBorderStyleText = theme.Focus.Border;
        _navigation.ItemStyle = theme.Text.Secondary;
        _navigation.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();
        _navigation.SelectedItemStyle = selected.WithBold();
        _navigation.FocusedSelectedItemStyle = selected.WithBold().WithUnderline();

        _commandBar.TitleStyle = theme.Text.Secondary;
        _commandBar.FocusedTitleStyle = theme.Focus.Title;
        _commandBar.ItemStyle = theme.Text.Primary;
        _commandBar.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();
        _commandBar.SelectedItemStyle = selected.WithBold();
        _commandBar.SeparatorStyle = theme.Text.Muted;

        _tabs.TitleStyle = theme.Text.Muted;
        _tabs.FocusedTitleStyle = theme.Focus.Title;

        _serviceList.TitleStyle = theme.Text.Secondary;
        _serviceList.FocusedTitleStyle = theme.Focus.Title;
        _serviceList.BorderStyleText = theme.Border.Default;
        _serviceList.FocusedBorderStyleText = theme.Focus.Border;
        _serviceList.DefaultRowStyle = theme.Text.Primary;
        _serviceList.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();
        _serviceList.SelectedRowStyle = selected.WithBold();

        _workTable.TitleStyle = theme.Text.Secondary;
        _workTable.FocusedTitleStyle = theme.Focus.Title;
        _workTable.BorderStyleText = theme.Border.Default;
        _workTable.FocusedBorderStyleText = theme.Focus.Border;
        _workTable.HeaderStyle = theme.Accent.Primary.WithBold();
        _workTable.RowStyle = theme.Text.Primary;
        _workTable.HoveredRowStyle = theme.Accent.Secondary.WithUnderline();
        _workTable.SelectedRowStyle = selected.WithBold();

        _p95Series.Style = theme.Accent.Primary.WithBold();
        _p99Series.Style = theme.Accent.Secondary.WithBold();
        _errorSeries.Style = theme.State.Warning.WithBold();
        _p95Series.PointGlyph = '●';
        _p99Series.PointGlyph = '◆';
        _errorSeries.PointGlyph = '▪';

        _latencyPlot.TitleStyle = theme.Text.Secondary;
        _latencyPlot.FocusedTitleStyle = theme.Focus.Title;
        _latencyPlot.BorderStyleText = theme.Border.Strong;
        _latencyPlot.FocusedBorderStyleText = theme.Focus.Border;
        _latencyPlot.LegendStyle = theme.Text.Muted;
        _latencyPlot.StatsStyle = theme.Text.Secondary;
        _latencyPlot.AxisStyle = theme.Text.Muted;
        _latencyPlot.GridStyle = theme.Border.Default;

        _errorBudgetGauge.TitleStyle = theme.Text.Secondary;
        _errorBudgetGauge.FocusedTitleStyle = theme.Focus.Title;
        _errorBudgetGauge.ValueLabelStyle = theme.State.Warning.WithBold();

        _queueDepthGauge.TitleStyle = theme.Text.Secondary;
        _queueDepthGauge.FocusedTitleStyle = theme.Focus.Title;
        _queueDepthGauge.ValueLabelStyle = theme.Accent.Secondary.WithBold();

        _ackButton.LabelStyle = theme.State.Success.WithBold();
        _ackButton.FocusedLabelStyle = theme.Focus.Title;
        _ackButton.BorderStyleText = theme.Border.Default;
        _ackButton.FocusedBorderStyleText = theme.Focus.Border;

        _rollbackButton.LabelStyle = theme.State.Warning.WithBold();
        _rollbackButton.FocusedLabelStyle = theme.Focus.Title;
        _rollbackButton.BorderStyleText = theme.Border.Default;
        _rollbackButton.FocusedBorderStyleText = theme.Focus.Border;

        _freezeButton.LabelStyle = theme.State.Error.WithBold();
        _freezeButton.FocusedLabelStyle = theme.Focus.Title;
        _freezeButton.BorderStyleText = theme.Border.Error;
        _freezeButton.FocusedBorderStyleText = theme.Focus.Border;

        _selectionSummary.TitleStyle = theme.Text.Secondary;
        _selectionSummary.FocusedTitleStyle = theme.Focus.Title;
        _selectionSummary.BorderStyleText = theme.Border.Default;
        _selectionSummary.FocusedBorderStyleText = theme.Focus.Border;
        _selectionSummary.TextStyle = theme.Text.Primary;

        _notifications.TitleStyle = theme.Text.Secondary;
        _notifications.FocusedTitleStyle = theme.Focus.Title;
        _notifications.BorderStyleText = theme.Border.Default;
        _notifications.FocusedBorderStyleText = theme.Focus.Border;
        _notifications.ItemStyle = theme.Text.Secondary;
        _notifications.SelectedItemStyle = selected;
        _notifications.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();
        _notifications.UnreadItemStyle = theme.Text.Primary.WithBold();
        _notifications.InfoItemStyle = theme.State.Info;
        _notifications.SuccessItemStyle = theme.State.Success;
        _notifications.WarningItemStyle = theme.State.Warning;
        _notifications.ErrorItemStyle = theme.State.Error;

        _activityLog.TitleStyle = theme.Text.Secondary;
        _activityLog.FocusedTitleStyle = theme.Focus.Title;
        _activityLog.BorderStyleText = theme.Border.Default;
        _activityLog.FocusedBorderStyleText = theme.Focus.Border;
        _activityLog.EntryStyle = theme.Text.Muted;

        _palette.TitleStyle = theme.Text.Secondary;
        _palette.FocusedTitleStyle = theme.Focus.Title;
        _palette.BorderStyleText = theme.Border.Strong;
        _palette.FocusedBorderStyleText = theme.Focus.Border;
        _palette.QueryTextStyle = theme.Text.Primary;
        _palette.PlaceholderTextStyle = theme.Text.Muted.WithItalic();
        _palette.ItemStyle = theme.Text.Secondary;
        _palette.SelectedItemStyle = selected.WithBold();
        _palette.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();

        _confirmDialog.TitleStyle = theme.Text.Secondary;
        _confirmDialog.FocusedTitleStyle = theme.Focus.Title;
        _confirmDialog.BorderStyleText = theme.Border.Strong;
        _confirmDialog.FocusedBorderStyleText = theme.Focus.Border;
        _confirmDialog.BodyTextStyle = theme.Text.Primary;

        _status.Fill = '·';
        _status.LeftTextStyle = theme.Text.Muted;
        _status.RightTextStyle = theme.Accent.Primary.WithBold();
        _status.FillStyle = theme.Surface.Panel;
    }

    private double Noise(double min, double max)
    {
        return min + (_random.NextDouble() * (max - min));
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Clamp(value, min, max);
    }
}
