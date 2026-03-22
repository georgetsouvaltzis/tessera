using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Styles;

internal sealed partial class ExternalConsumerReviewApp
{
    private static BoxPlotSeries BuildFiveNumberSeries(string name, IEnumerable<double> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        var sorted = values.OrderBy(static value => value).ToArray();
        if (sorted.Length == 0)
        {
            return new BoxPlotSeries(name, 0, 0, 0, 0, 0);
        }

        return new BoxPlotSeries(
            name,
            sorted[0],
            ComputePercentile(sorted, 0.25d),
            ComputePercentile(sorted, 0.5d),
            ComputePercentile(sorted, 0.75d),
            sorted[^1]);
    }

    private static double ComputePercentile(double[] sortedValues, double percentile)
    {
        if (sortedValues.Length == 0)
        {
            return 0;
        }

        if (sortedValues.Length == 1)
        {
            return sortedValues[0];
        }

        var clamped = Math.Clamp(percentile, 0d, 1d);
        var position = (sortedValues.Length - 1) * clamped;
        var lower = (int)Math.Floor(position);
        var upper = (int)Math.Ceiling(position);
        if (lower == upper)
        {
            return sortedValues[lower];
        }

        var fraction = position - lower;
        return sortedValues[lower] + ((sortedValues[upper] - sortedValues[lower]) * fraction);
    }

    private List<QuickOpenItem> BuildQuickOpenItems()
    {
        var items = new List<QuickOpenItem>
        {
            new("nav:overview", "Go to Overview tab", "Sets top tab selection to Overview"),
            new("nav:analytics", "Go to Analytics tab", "Sets top tab selection to Analytics"),
            new("action:mark-all-read", "Mark all notifications read", "Calls Notifications.MarkAllRead()"),
            new("action:add-token", "Add token incident:open", "Calls TokenEditor.AddToken(...)"),
        };

        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            items.Add(
                new QuickOpenItem(
                    $"service:{service.Id}",
                    $"Service: {service.Name}",
                    $"{service.State}  p95 {service.P95Ms}ms  {service.Environment}"));
        }

        for (var index = 0; index < _endpoints.Count; index++)
        {
            var endpoint = _endpoints[index];
            items.Add(
                new QuickOpenItem(
                    $"endpoint:{endpoint.Path}",
                    $"Endpoint: {endpoint.Path}",
                    $"err {endpoint.ErrorBasisPoints / 100d:0.00}%  req/s {endpoint.RequestsPerSecond}"));
        }

        return items;
    }

    private void HandleQuickOpenSubmitted(QuickOpenOverlaySubmittedEventArgs args)
    {
        var itemId = args.ItemId;
        if (itemId.StartsWith("service:", StringComparison.Ordinal))
        {
            var serviceId = itemId["service:".Length..];
            SelectServiceById(serviceId);
            _statusText = $"quick-open selected service {serviceId}";
            _notifications.Push($"Quick-open -> service {serviceId}", NotificationLevel.Info);
            return;
        }

        if (string.Equals(itemId, "action:mark-all-read", StringComparison.Ordinal))
        {
            _notifications.MarkAllRead();
            _statusText = "quick-open marked all notifications read";
            return;
        }

        if (string.Equals(itemId, "action:add-token", StringComparison.Ordinal))
        {
            _tokenEditor.AddToken("incident:open");
            _statusText = "quick-open appended token incident:open";
            return;
        }

        if (string.Equals(itemId, "nav:overview", StringComparison.Ordinal))
        {
            _navigation.SetSelectedIndex(0);
            _statusText = "quick-open switched to Overview";
            return;
        }

        if (string.Equals(itemId, "nav:analytics", StringComparison.Ordinal))
        {
            _navigation.SetSelectedIndex(3);
            _statusText = "quick-open switched to Analytics";
            return;
        }

        _statusText = $"quick-open submitted {args.Item.Label}";
        AppendActivity($"Quick-open submitted -> {args.ItemId} (query: {args.Query})");
    }

    private void SelectServiceById(string serviceId)
    {
        for (var index = 0; index < _services.Count; index++)
        {
            if (!string.Equals(_services[index].Id, serviceId, StringComparison.Ordinal))
            {
                continue;
            }

            _selectedServiceId = serviceId;
            _healthBoard.SetSelectedIndex(index);
            return;
        }
    }

    private string BuildDashboardApiSummary(ScreenContext context)
    {
        var rail = _dashboardRail.SelectedItem?.Label ?? "(none)";
        var tile = _dashboardGrid.SelectedItem?.Title ?? "(none)";
        var health = _healthBoard.SelectedItem?.Name ?? "(none)";
        var pane = _workflowPanes.SelectedPane?.Id ?? "(none)";
        var jump = _jumpList.SelectedItem?.Label ?? "(none)";
        var command = string.IsNullOrWhiteSpace(_commandInput.Text) ? "(empty)" : _commandInput.Text;
        return
            $"""
             Rail: {rail}  Pane: {pane}
             Tile: {tile}  Health: {health}
             Jump: {jump}
             Command: {command}
             Viewport: {context.Width}x{context.Height}
             """;
    }

    private bool HandleDashboardApiHotKeys(KeyPressed key)
    {
        if (_navigation.SelectedIndex != 4)
        {
            return false;
        }

        if (key.IsCharacter('p', ModifierKeys.Ctrl))
        {
            if (_quickOpenOverlay.IsOpen)
            {
                _quickOpenOverlay.Close();
                _statusText = "quick-open closed";
            }
            else
            {
                _quickOpenOverlay.Open();
                _statusText = "quick-open opened";
            }

            return true;
        }

        if (key.IsCharacter('a'))
        {
            var selected = _healthBoard.SelectedItem;
            if (selected is null)
            {
                _statusText = "no service selected to acknowledge";
                return true;
            }

            if (!_acknowledgedHealthServices.Add(selected.Id))
            {
                _statusText = $"{selected.Name} already acknowledged";
                return true;
            }

            _healthBoard.Acknowledge(selected.Id);
            _notifications.Push($"Acknowledged {selected.Name}", NotificationLevel.Warning);
            AppendActivity($"Acknowledged health row -> {selected.Name}");
            _statusText = $"acknowledged {selected.Name}";
            return true;
        }

        return false;
    }

    private void ApplyDashboardApiThemeAndOverrides(TeaTheme theme, TeaThemeOverrideBundle bundle)
    {
        _dashboardRail.ApplyTheme(theme);
        _dashboardRail.FocusMarker = bundle.FocusMarker;
        _dashboardRail.BorderStyleText = bundle.BorderStyleText;
        _dashboardRail.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _dashboardGrid.ApplyTheme(theme);
        _dashboardGrid.FocusMarker = bundle.FocusMarker;
        _dashboardGrid.BorderStyleText = bundle.BorderStyleText;
        _dashboardGrid.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _latencyBudgetChart.ApplyTheme(theme);
        _latencyBudgetChart.FocusMarker = bundle.FocusMarker;
        _latencyBudgetChart.BorderStyleText = bundle.BorderStyleText;
        _latencyBudgetChart.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _healthBoard.ApplyTheme(theme);
        _healthBoard.FocusMarker = bundle.FocusMarker;
        _healthBoard.BorderStyleText = bundle.BorderStyleText;
        _healthBoard.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _distributionPlot.ApplyTheme(theme);
        _distributionPlot.FocusMarker = bundle.FocusMarker;
        _distributionPlot.BorderStyleText = bundle.BorderStyleText;
        _distributionPlot.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _jumpList.ApplyThemeDefaults(theme);
        _jumpList.FocusMarker = bundle.FocusMarker;
        _jumpList.BorderStyleText = bundle.BorderStyleText;
        _jumpList.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _tokenEditor.ApplyThemeDefaults(theme);
        _tokenEditor.FocusMarker = bundle.FocusMarker;
        _tokenEditor.BorderStyleText = bundle.BorderStyleText;
        _tokenEditor.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _commandInput.ApplyThemeDefaults(theme);
        _commandInput.FocusMarker = bundle.FocusMarker;
        _commandInput.BorderStyleText = bundle.BorderStyleText;
        _commandInput.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _commandInput.CommitMarkerStyle = theme.Accent.Primary.WithBold();

        _workflowPanes.ApplyTheme(theme);
        _workflowPanes.FocusMarker = bundle.FocusMarker;
        _workflowPanes.BorderStyleText = bundle.BorderStyleText;
        _workflowPanes.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _quickOpenOverlay.ApplyTheme(theme);
        _quickOpenOverlay.FocusMarker = bundle.FocusMarker;
        _quickOpenOverlay.BorderStyleText = bundle.BorderStyleText;
        _quickOpenOverlay.FocusedBorderStyleText = bundle.FocusedBorderStyleText;

        _dashboardApiSummary.ApplyTheme(theme);
        _dashboardApiSummary.TextStyle = theme.Text.Primary;
        _dashboardApiSummary.BorderStyleText = bundle.BorderStyleText;
        _dashboardApiSummary.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
    }
}
