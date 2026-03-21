using System.Globalization;
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed partial class ExternalConsumerReviewApp
{
    private const int PlotRetentionWindow = 64;

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

    private readonly StatsCard _incidentCard = new()
    {
        Title = "Incidents",
        FocusMarker = "◆",
    };

    private readonly LineSeries _reqSeries = new("Req/s");
    private readonly LineSeries _p95Series = new("P95 ms");

    private readonly LinePlot _trafficPlot = new()
    {
        Title = "Traffic and Latency Trends",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        FocusMarker = "◆",
        Options = new LinePlotOptions(
            ShowAxes: true,
            ShowGrid: true,
            ShowLegend: true,
            ShowStats: true,
            XLabel: "ticks",
            YLabel: "value"),
    };

    private readonly Table _endpointTable = new("Endpoint", "P95", "Err%", "Req/s")
    {
        Title = "Endpoint Health",
        Border = BorderStyle.Rounded,
        Padding = Thickness.All(1),
        PageSize = 8,
        FocusMarker = "◆",
    };

    private readonly List<EndpointSnapshot> _endpoints =
    [
        new EndpointSnapshot("/v1/orders", 48, 34, 320),
        new EndpointSnapshot("/v1/checkout", 68, 97, 188),
        new EndpointSnapshot("/v1/refunds", 79, 142, 73),
        new EndpointSnapshot("/v1/catalog", 42, 22, 410),
        new EndpointSnapshot("/v1/search", 58, 55, 265),
        new EndpointSnapshot("/v1/payments", 91, 176, 101),
    ];

    private void InitializeWave2Dashboard()
    {
        _trafficPlot.SetSeries([_reqSeries, _p95Series]);
        UpdateWave2State();
    }

    private Screen BuildAnalyticsScreen(ScreenContext context)
    {
        _endpointTable.SetRows(BuildEndpointRows());
        _selectionSummary.Text = BuildSelectionSummary(context);
        _status.LeftText =
            $"{CurrentThemeName()}  analytics  tick={_tick:0000}  selected={GetSelectedService().Name}";
        _status.RightText =
            $"{_statusText}  1-4 tabs  t theme  d dialog  n note  Ctrl+C quit";

        var cardsRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _latencyCard,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _trafficCard,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _incidentCard,
                    Length = LayoutLength.Fill(),
                },
            },
        };

        var analyticsRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _trafficPlot,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _endpointTable,
                    Length = Math.Min(58, Math.Max(42, context.Width / 3)),
                },
            },
        };

        var lowerRow = new RowLayout
        {
            Gap = 1,
            Items =
            {
                new LayoutSlot
                {
                    Content = _activity,
                    Length = LayoutLength.Fill(),
                },
                new LayoutSlot
                {
                    Content = _notifications,
                    Length = Math.Min(48, Math.Max(34, context.Width / 4)),
                },
            },
        };

        return Screen.Build(window =>
        {
            window.Gap(1);
            window.Padding(1);
            window.Header(1, _navigation);
            window.Left(Math.Min(36, Math.Max(28, context.Width / 4)), _serviceList);
            window.Body(body => body.Column(column =>
            {
                column.Gap(1);
                column.Fixed(7, cardsRow);
                column.Fixed(14, analyticsRow);
                column.Fill(lowerRow);
            }));
            window.Footer(1, _status);
            window.Overlay(new CenterLayout
            {
                Content = _deployDialog,
                Width = Math.Min(66, Math.Max(48, context.Width - 8)),
                Height = 10,
            });
        });
    }

    private void UpdateWave2State()
    {
        var totalRps = 0;
        var totalP95 = 0;
        var degraded = 0;
        for (var index = 0; index < _services.Count; index++)
        {
            var service = _services[index];
            totalRps += service.RequestsPerSecond;
            totalP95 += service.P95Ms;
            if (!string.Equals(service.State, "Healthy", StringComparison.Ordinal))
            {
                degraded++;
            }
        }

        var avgP95 = _services.Count == 0 ? 0 : totalP95 / _services.Count;
        var avgRps = _services.Count == 0 ? 0 : totalRps / _services.Count;

        AppendSeriesSample(_reqSeries, avgRps, PlotRetentionWindow);
        AppendSeriesSample(_p95Series, avgP95, PlotRetentionWindow);

        for (var index = 0; index < _endpoints.Count; index++)
        {
            _endpoints[index].P95Ms = Math.Clamp(_endpoints[index].P95Ms + _random.Next(-7, 8), 14, 180);
            _endpoints[index].ErrorBasisPoints = Math.Clamp(_endpoints[index].ErrorBasisPoints + _random.Next(-25, 26), 0, 900);
            _endpoints[index].RequestsPerSecond = Math.Clamp(_endpoints[index].RequestsPerSecond + _random.Next(-35, 36), 20, 600);
        }

        _latencyCard.SetItems(
        [
            new StatItem("P95", avgP95.ToString(CultureInfo.InvariantCulture) + "ms"),
            new StatItem("SLO", "<= 60ms"),
            new StatItem("Status", avgP95 > 60 ? "At Risk" : "Healthy"),
        ]);

        _trafficCard.SetItems(
        [
            new StatItem("Req/s", avgRps.ToString(CultureInfo.InvariantCulture)),
            new StatItem("Peak", _services.Max(static service => service.RequestsPerSecond).ToString(CultureInfo.InvariantCulture)),
            new StatItem("Samples", _reqSeries.Samples.Count.ToString(CultureInfo.InvariantCulture)),
        ]);

        _incidentCard.SetItems(
        [
            new StatItem("Degraded", degraded.ToString(CultureInfo.InvariantCulture)),
            new StatItem("Alerts", _endpoints.Count(static endpoint => endpoint.ErrorBasisPoints >= 120).ToString(CultureInfo.InvariantCulture)),
            new StatItem("Unread", _notifications.Items.Count(static item => !item.IsRead).ToString(CultureInfo.InvariantCulture)),
        ]);
    }

    private List<IReadOnlyList<string>> BuildEndpointRows()
    {
        var rows = new List<IReadOnlyList<string>>();
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

    private static void AppendSeriesSample(LineSeries series, double sample, int retention)
    {
        series.Append(sample);
        if (series.Samples.Count <= retention)
        {
            return;
        }

        var overflow = series.Samples.Count - retention;
        var trimmed = new double[retention];
        for (var index = 0; index < retention; index++)
        {
            trimmed[index] = series.Samples[index + overflow];
        }

        series.SetSamples(trimmed);
    }

    private void ApplyWave2ThemeAndOverrides(TeaTheme theme, TeaThemeOverrideBundle bundle)
    {
        _latencyCard.ApplyTheme(theme);
        _trafficCard.ApplyTheme(theme);
        _incidentCard.ApplyTheme(theme);
        _latencyCard.TitleStyle = theme.Accent.Secondary.WithBold();
        _trafficCard.TitleStyle = theme.Accent.Primary.WithBold();
        _incidentCard.TitleStyle = theme.Text.Secondary.WithBold();

        _trafficPlot.ApplyTheme(theme);
        _trafficPlot.FocusMarker = bundle.FocusMarker;
        _trafficPlot.BorderStyleText = bundle.BorderStyleText;
        _trafficPlot.FocusedBorderStyleText = bundle.FocusedBorderStyleText;
        _reqSeries.Style = theme.Accent.Primary.WithBold();
        _p95Series.Style = theme.Accent.Secondary.WithBold();

        _endpointTable.ApplyThemeAndDashboardOverrides(bundle);
    }
}

internal sealed class EndpointSnapshot
{
    public EndpointSnapshot(string path, int p95Ms, int errorBasisPoints, int requestsPerSecond)
    {
        Path = path;
        P95Ms = p95Ms;
        ErrorBasisPoints = errorBasisPoints;
        RequestsPerSecond = requestsPerSecond;
    }

    public string Path { get; }

    public int P95Ms { get; set; }

    public int ErrorBasisPoints { get; set; }

    public int RequestsPerSecond { get; set; }
}
