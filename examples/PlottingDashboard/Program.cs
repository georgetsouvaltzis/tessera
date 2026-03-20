using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

var app = Tea.CreateBuilder()
    .UseApp<PlottingDashboardApp>()
    .ConfigureRuntime(static runtime =>
    {
        runtime.Screen = new ScreenOptions
        {
            AltScreen = true,
            WindowTitle = "TeaSharp Plotting Dashboard",
            EnableFocusReporting = true,
            MouseTracking = MouseTrackingMode.AllMotion,
            EnableBracketedPaste = true,
        };
    })
    .Build();

await app.RunAsync();

internal sealed record PlotTick(DateTimeOffset At) : Message;

internal enum DashboardThemeMode
{
    Catppuccin,
    RosePine,
}

internal enum DashboardDataMode
{
    Smooth,
    Bursty,
}

internal sealed class PlottingDashboardApp : TeaApp
{
    private readonly Random _random = new(9001);
    private readonly List<ScatterPlotPoint> _scatterWindow = [];
    private readonly LineSeries _latencyP50 = new("p50");
    private readonly LineSeries _latencyP95 = new("p95");
    private readonly LineSeries _latencyP99 = new("p99");

    private readonly Sparkline _cpuSpark = new(capacity: 180)
    {
        Title = "CPU %",
        Border = BorderStyle.SingleLine,
        MinValue = 0,
        MaxValue = 100,
        Options = new SparklineOptions(ShowStats: true, Legend: "usage"),
    };

    private readonly AreaPlot _memoryPlot = new(capacity: 180)
    {
        Title = "Memory (GB)",
        Border = BorderStyle.SingleLine,
        MinValue = 6,
        MaxValue = 64,
        Options = new AreaPlotOptions(
            FillGlyph: '░',
            LineGlyph: '▀',
            ShowBaseline: true,
            BaselineGlyph: '─',
            ShowStats: true,
            Legend: "rss"),
    };

    private readonly LinePlot _latencyLine = new()
    {
        Title = "Latency Timeline (ms)",
        Border = BorderStyle.SingleLine,
        MinValue = 0,
        MaxValue = 350,
        Options = new LinePlotOptions(
            ShowAxes: true,
            ShowGrid: true,
            ShowLegend: true,
            ShowStats: true,
            XLabel: "samples",
            YLabel: "ms"),
    };

    private readonly ScatterPlot _latencyScatter = new()
    {
        Title = "Latency Jitter",
        Options = new ScatterPlotOptions(
            ShowAxes: true,
            ShowLabels: false,
            Legend: "samples",
            XLabel: "time",
            YLabel: "ms",
            PointGlyph: '●'),
    };

    private readonly Histogram _errorHistogram = new()
    {
        Title = "Error Distribution (%)",
        Options = new HistogramOptions(
            ShowAxes: true,
            ShowBucketLabels: true,
            ShowScale: true,
            Legend: "rate",
            XLabel: "codes",
            YLabel: "%",
            BarGlyph: '█'),
    };

    private readonly PlotPanel _panel = new()
    {
        Title = "Runtime Telemetry",
        Border = BorderStyle.Rounded,
        Options = new PlotPanelOptions(Columns: 2, Spacing: 1),
        Padding = Thickness.All(1),
    };

    private readonly StatusBar _status = new()
    {
        Fill = ' ',
    };

    private DashboardThemeMode _themeMode = DashboardThemeMode.Catppuccin;
    private DashboardDataMode _dataMode = DashboardDataMode.Smooth;
    private bool _paused;
    private int _tick;
    private double _phase;

    public PlottingDashboardApp()
    {
        _panel.SetPlots(
        [
            _cpuSpark,
            _memoryPlot,
            _latencyLine,
            _latencyScatter,
            _errorHistogram,
        ]);
        _latencyLine.SetSeries([_latencyP50, _latencyP95, _latencyP99]);
        SeedHistogram();
        ApplyTheme(TeaThemes.Catppuccin(CatppuccinVariant.Macchiato));
    }

    public override TeaEffect? Initialize()
    {
        return TeaEffects.Tick(TimeSpan.FromMilliseconds(240), static now => new PlotTick(now));
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is PlotTick)
        {
            _tick++;
            if (!_paused)
            {
                AppendTelemetry();
            }

            return TeaEffects.Tick(TimeSpan.FromMilliseconds(240), static now => new PlotTick(now));
        }

        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.IsCharacter('q') || key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.IsCharacter('t'))
        {
            _themeMode = _themeMode == DashboardThemeMode.Catppuccin
                ? DashboardThemeMode.RosePine
                : DashboardThemeMode.Catppuccin;
            ApplyTheme(ResolveTheme());
            return null;
        }

        if (key.IsCharacter('m'))
        {
            _dataMode = _dataMode == DashboardDataMode.Smooth
                ? DashboardDataMode.Bursty
                : DashboardDataMode.Smooth;
            return null;
        }

        if (key.IsCharacter('p'))
        {
            _paused = !_paused;
            return null;
        }

        if (key.IsCharacter('r'))
        {
            ResetTelemetry();
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = "q quit  t theme  m mode  p pause  r reset";
        _status.RightText =
            $"theme:{ThemeLabel()} mode:{ModeLabel()} state:{(_paused ? "paused" : "live")} tick:{_tick:0000} size:{context.Width}x{context.Height}";

        _panel.Title = _paused
            ? $"Runtime Telemetry [{ThemeLabel()}] (paused)"
            : $"Runtime Telemetry [{ThemeLabel()}]";

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Body(_panel);
            window.Footer(1, _status);
        });
    }

    private void AppendTelemetry()
    {
        _phase += 1;
        var wave = Math.Sin(_phase / 9d) * 20;
        var burst = _dataMode == DashboardDataMode.Bursty && _tick % 24 is > 13 and < 20 ? 22 : 0;

        var cpu = Clamp(46 + wave + burst + Noise(9), 5, 100);
        var memory = Clamp(18 + (cpu * 0.34) + Math.Sin(_phase / 15d) * 4 + Noise(1.3), 6, 64);

        var p50 = Clamp(16 + (cpu * 0.48) + Noise(3.5), 6, 220);
        var p95 = Clamp(p50 + 18 + Math.Abs(Noise(15)), p50 + 1, 300);
        var p99 = Clamp(p95 + 11 + Math.Abs(Noise(19)), p95 + 1, 350);

        var errorRate = Clamp((_dataMode == DashboardDataMode.Bursty ? 2.4 : 1.1) + Math.Max(0, cpu - 68) * 0.15 + Math.Abs(Noise(0.8)), 0.1, 15);
        var softErrors = Clamp(errorRate * 0.58 + Math.Abs(Noise(0.5)), 0, 12);
        var hardErrors = Clamp(errorRate * 0.28 + Math.Abs(Noise(0.35)), 0, 8);
        var timeouts = Clamp(errorRate * 0.14 + Math.Abs(Noise(0.2)), 0, 5);

        _cpuSpark.Append(cpu);
        _memoryPlot.Append(memory);
        AppendSeriesSample(_latencyP50, p50);
        AppendSeriesSample(_latencyP95, p95);
        AppendSeriesSample(_latencyP99, p99);

        _scatterWindow.Add(
            new ScatterPlotPoint(
                x: _tick,
                y: p95,
                label: _tick % 20 == 0 ? $"{p95:0}" : null));
        if (_scatterWindow.Count > 80)
        {
            _scatterWindow.RemoveAt(0);
        }

        _latencyScatter.SetPoints(_scatterWindow);
        _errorHistogram.SetBuckets(
        [
            new HistogramBucket("4xx", softErrors),
            new HistogramBucket("5xx", hardErrors),
            new HistogramBucket("to", timeouts),
            new HistogramBucket("ok", Math.Max(0.5, 100 - ((softErrors + hardErrors + timeouts) * 4.2))),
        ]);
    }

    private void ResetTelemetry()
    {
        _cpuSpark.Clear();
        _memoryPlot.Clear();
        _latencyP50.Clear();
        _latencyP95.Clear();
        _latencyP99.Clear();
        _scatterWindow.Clear();
        _latencyScatter.Clear();
        SeedHistogram();
    }

    private void SeedHistogram()
    {
        _errorHistogram.SetBuckets(
        [
            new HistogramBucket("4xx", 1.2),
            new HistogramBucket("5xx", 0.6),
            new HistogramBucket("to", 0.2),
            new HistogramBucket("ok", 96),
        ]);
    }

    private void ApplyTheme(TeaTheme theme)
    {
        _cpuSpark.ApplyTheme(theme);
        _memoryPlot.ApplyTheme(theme);
        _latencyLine.ApplyTheme(theme);
        _latencyScatter.ApplyTheme(theme);
        _errorHistogram.ApplyTheme(theme);
        _panel.ApplyTheme(theme);

        _latencyP50.Style = theme.Accent.Primary;
        _latencyP95.Style = theme.Accent.Secondary;
        _latencyP99.Style = theme.State.Warning;
        _latencyP50.PointGlyph = '●';
        _latencyP95.PointGlyph = '◆';
        _latencyP99.PointGlyph = '▲';

        _errorHistogram.BarStyle = theme.State.Error;
        _status.LeftTextStyle = theme.Text.Secondary;
        _status.RightTextStyle = theme.Focus.Title;
        _status.FillStyle = theme.Surface.Panel;
    }

    private TeaTheme ResolveTheme()
    {
        return _themeMode == DashboardThemeMode.Catppuccin
            ? TeaThemes.Catppuccin(CatppuccinVariant.Macchiato)
            : TeaThemes.RosePine(RosePineVariant.Moon);
    }

    private string ThemeLabel()
    {
        return _themeMode == DashboardThemeMode.Catppuccin
            ? "catppuccin"
            : "rose-pine";
    }

    private string ModeLabel()
    {
        return _dataMode == DashboardDataMode.Smooth
            ? "smooth"
            : "bursty";
    }

    private static void AppendSeriesSample(LineSeries series, double sample)
    {
        series.Append(sample);
        if (series.Samples.Count <= 180)
        {
            return;
        }

        var keep = new double[180];
        var start = series.Samples.Count - keep.Length;
        for (var index = 0; index < keep.Length; index++)
        {
            keep[index] = series.Samples[start + index];
        }

        series.SetSamples(keep);
    }

    private double Noise(double amplitude)
    {
        return (_random.NextDouble() - 0.5) * 2d * amplitude;
    }

    private static double Clamp(double value, double min, double max)
    {
        return Math.Clamp(value, min, max);
    }
}
