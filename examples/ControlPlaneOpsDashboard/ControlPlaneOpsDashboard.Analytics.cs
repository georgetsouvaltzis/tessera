using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed partial class ControlPlaneOpsDashboardApp
{
    private readonly StatsCard _latencyCard = new()
    {
        Title = "Latency",
        FocusMarker = "◆",
    };

    private readonly StatsCard _trafficCard = new()
    {
        Title = "Traffic",
        FocusMarker = "◆",
    };

    private readonly StatsCard _errorCard = new()
    {
        Title = "Errors",
        FocusMarker = "◆",
    };

    private readonly LineSeries _rpsSeries = new("Req/s")
    {
        Capacity = 160,
    };

    private readonly LineSeries _p95Series = new("P95 ms")
    {
        Capacity = 160,
        ScaleMode = LineSeriesScaleMode.Normalized,
    };

    private readonly LinePlot _trendPlot = new()
    {
        Title = "Traffic and Latency Trends",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly ScatterPlot _errorScatter = new()
    {
        Title = "Error Distribution",
        FocusMarker = "◆",
    };

    private readonly BoxPlot _distributionPlot = new()
    {
        Title = "Endpoint Five-Number Summary",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
    };

    private readonly Table _endpointTable = new("Endpoint", "P95", "Err%", "Req/s")
    {
        Title = "Endpoint Health",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        PageSize = 8,
        FocusMarker = "◆",
    };

    private void InitializeAnalyticsFeatures()
    {
        _trendPlot.SetSeries([_rpsSeries, _p95Series]);
        _trendPlot.ConfigureAxes(showAxes: true, xLabel: "ticks", sharedAxisLabel: "req/s", normalizedAxisLabel: "p95(norm)");
        _trendPlot.ConfigureGrid(true);
        _trendPlot.ConfigureLegend(true);
        _errorScatter.Options = new ScatterPlotOptions(
            ShowAxes: true,
            ShowLabels: false,
            Legend: "err-bps vs p95",
            XLabel: "error bps",
            YLabel: "p95 ms",
            PointGlyph: '●');
    }

    private Screen BuildAnalyticsScreen(ScreenContext context)
    {
        _endpointTable.SetRows(BuildEndpointRows());
        _selectionSummary.Text = BuildSelectionSummary(context);
        ConfigureStatus("analytics");

        var cardsRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _latencyCard, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _trafficCard, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _errorCard, Length = LayoutLength.Fill() },
            },
        };

        var plotRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _trendPlot, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _errorScatter, Length = Math.Min(56, Math.Max(40, context.Width / 3)) },
            },
        };

        var tableRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot { Content = _distributionPlot, Length = LayoutLength.Fill() },
                new LayoutSlot { Content = _endpointTable, Length = Math.Min(56, Math.Max(40, context.Width / 3)) },
            },
        };

        return BuildWithChrome(
            context,
            body =>
            {
                body.Column(column =>
                {
                    column.Gap(1);
                    column.Fixed(7, cardsRow);
                    column.Fixed(12, plotRow);
                    column.Fixed(12, tableRow);
                    column.Fill(_activity);
                });
            });
    }

    private void UpdateAnalyticsState()
    {
        var totalRps = 0;
        var totalP95 = 0;
        var totalErrorBps = 0;
        var degraded = 0;

        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            totalRps += service.RequestsPerSecond;
            totalP95 += service.P95Ms;
            totalErrorBps += service.ErrorBasisPoints;
            if (!string.Equals(service.State, "Healthy", StringComparison.Ordinal))
            {
                degraded++;
            }
        }

        var avgRps = _services.Count == 0 ? 0 : totalRps / _services.Count;
        var avgP95 = _services.Count == 0 ? 0 : totalP95 / _services.Count;
        var avgErrorBps = _services.Count == 0 ? 0 : totalErrorBps / _services.Count;

        _rpsSeries.Append(avgRps);
        _p95Series.Append(avgP95);

        _latencyCard.SetItems(
        [
            new StatItem("P95", $"{avgP95}ms"),
            new StatItem("Target", "<= 60ms"),
            new StatItem("State", avgP95 > 60 ? "At Risk" : "Healthy"),
        ]);

        _trafficCard.SetItems(
        [
            new StatItem("Avg Req/s", avgRps.ToString(CultureInfo.InvariantCulture)),
            new StatItem("Peak", _services.Max(static service => service.RequestsPerSecond).ToString(CultureInfo.InvariantCulture)),
            new StatItem("Samples", _rpsSeries.Samples.Count.ToString(CultureInfo.InvariantCulture)),
        ]);

        _errorCard.SetItems(
        [
            new StatItem("Err%", $"{avgErrorBps / 100d:0.00}%"),
            new StatItem("Degraded", degraded.ToString(CultureInfo.InvariantCulture)),
            new StatItem("Unread", _notifications.Items.Count(static item => !item.IsRead).ToString(CultureInfo.InvariantCulture)),
        ]);

        var points = new List<ScatterPlotPoint>(_services.Count);
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            points.Add(new ScatterPlotPoint(service.ErrorBasisPoints, service.P95Ms, service.Name));
        }

        _errorScatter.SetPoints(points);
        _errorScatter.Capacity = 128;

        _distributionPlot.SetSeries(
        [
            BuildFiveNumberSeries("P95 ms", _endpoints.Select(static endpoint => (double)endpoint.P95Ms)),
            BuildFiveNumberSeries("Err bps", _endpoints.Select(static endpoint => (double)endpoint.ErrorBasisPoints)),
            BuildFiveNumberSeries("Req/s", _endpoints.Select(static endpoint => (double)endpoint.RequestsPerSecond)),
        ]);
    }

    private List<IReadOnlyList<string>> BuildEndpointRows()
    {
        var rows = new List<IReadOnlyList<string>>(_endpoints.Count);
        for (var index = 0; index < _endpoints.Count; index++)
        {
            var endpoint = _endpoints[index];
            rows.Add(
            [
                endpoint.Path,
                $"{endpoint.P95Ms}ms",
                $"{endpoint.ErrorBasisPoints / 100d:0.00}%",
                endpoint.RequestsPerSecond.ToString(CultureInfo.InvariantCulture),
            ]);
        }

        return rows;
    }

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
            ComputePercentile(sorted, 0.50d),
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

    private void ApplyAnalyticsTheme(TeaTheme theme, TeaThemeOverrideBundle bundle)
    {
        _latencyCard.ApplyTheme(theme);
        _trafficCard.ApplyTheme(theme);
        _errorCard.ApplyTheme(theme);
        _latencyCard.TitleStyle = theme.Accent.Primary.WithBold();
        _trafficCard.TitleStyle = theme.Accent.Secondary.WithBold();
        _errorCard.TitleStyle = theme.State.Warning.WithBold();

        _trendPlot.ApplyTheme(theme);
        _trendPlot.FocusMarker = bundle.FocusMarker;
        _trendPlot.BorderStyleText = bundle.BorderStyleText;
        _trendPlot.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _rpsSeries.Style = theme.Accent.Primary.WithBold();
        _p95Series.Style = theme.Accent.Secondary.WithBold();

        _errorScatter.ApplyTheme(theme);
        _errorScatter.FocusMarker = bundle.FocusMarker;
        _errorScatter.PointStyle = theme.Accent.Secondary.WithBold();
        _errorScatter.LabelStyle = theme.Text.Secondary;
        _errorScatter.AxisStyle = theme.Border.Default;

        _distributionPlot.ApplyTheme(theme);
        _distributionPlot.FocusMarker = bundle.FocusMarker;
        _distributionPlot.BorderStyleText = bundle.BorderStyleText;
        _distributionPlot.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _distributionPlot.SelectedSeriesStyle = theme.Selection.Background.WithBold();

        _endpointTable.ApplyThemeAndDashboardOverrides(bundle);
    }
}
