using Tessera.Controls;
using Tessera.Styles;

namespace Tessera.Examples.TransitBoard;

internal sealed partial class TransitBoardApp : TesseraApp
{
    private TransitBoardPalette _palette = TransitBoardTheme.Default;
    private readonly TransitBoardState _state = TransitBoardState.CreateSeed();

    private readonly TransitHeroControl _hero = new();
    private readonly TransitChipStripControl _modeStrip = new() { Title = string.Empty };
    private readonly TransitChipStripControl _routeStrip = new() { Title = string.Empty };
    private readonly TransitChipStripControl _themeStrip = new() { Title = string.Empty };
    private readonly TransitDepartureBoardControl _board = new();
    private readonly TransitNoticeControl _notices = new();
    private readonly TransitJourneyControl _journey = new();
    private readonly StatusBar _footer = new() { Fill = ' ' };

    private TransitBoardMode _mode = TransitBoardMode.Departures;
    private string _routeId = "all";
    private string? _selectedServiceId;
    private int _focusLane;
    private IReadOnlyList<TransitService> _visibleServices = [];

    public TransitBoardApp()
    {
        ApplyTheme(_palette);
        WireEvents();
        SeedControls();
        _board.RequestFocus();
    }

    public override TesseraEffect? Initialize() =>
        TesseraEffects.Periodic(TimeSpan.FromSeconds(1), _ => new TransitBoardTickMessage());

    public override TesseraEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key:
                return HandleKey(key);
            case TransitBoardTickMessage:
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

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Gap(0);
            ConfigureHeader(window, context);
            window.Body(body => ConfigureBody(body, context));
            window.Footer(1, _footer);
        });
    }

    private TesseraEffect? HandleKey(KeyPressed key)
    {
        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TesseraEffects.Quit;
        }

        if (key.Is(Key.Tab))
        {
            _focusLane = (_focusLane + 1) % 4;
            FocusCurrentLane();
            return null;
        }

        if (key.IsCharacter('1'))
        {
            SetTheme(TransitBoardThemeKind.Meridian);
            return null;
        }

        if (key.IsCharacter('2'))
        {
            SetTheme(TransitBoardThemeKind.Harbor);
            return null;
        }

        if (key.IsCharacter('3'))
        {
            SetTheme(TransitBoardThemeKind.Afterglow);
            return null;
        }

        if (key.IsCharacter('d'))
        {
            SelectMode(TransitBoardMode.Departures);
            return null;
        }

        if (key.IsCharacter('a'))
        {
            SelectMode(TransitBoardMode.Arrivals);
            return null;
        }

        if (key.IsCharacter('m'))
        {
            SelectMode(TransitBoardMode.AllBoard);
            return null;
        }

        if (key.Is(Key.F1))
        {
            _focusLane = 0;
            FocusCurrentLane();
            return null;
        }

        if (key.Is(Key.F2))
        {
            _focusLane = 1;
            FocusCurrentLane();
            return null;
        }

        if (key.Is(Key.F3))
        {
            _focusLane = 2;
            FocusCurrentLane();
            return null;
        }

        if (key.Is(Key.F4))
        {
            _focusLane = 3;
            FocusCurrentLane();
            return null;
        }

        return null;
    }

    private void WireEvents()
    {
        _modeStrip.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            _mode = args.SelectedItem.Id switch
            {
                "arrivals" => TransitBoardMode.Arrivals,
                "all" => TransitBoardMode.AllBoard,
                _ => TransitBoardMode.Departures,
            };
        };

        _routeStrip.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is not null)
            {
                _routeId = args.SelectedItem.Id;
            }
        };

        _themeStrip.SelectionChanged += (_, args) =>
        {
            if (args.SelectedItem is null)
            {
                return;
            }

            SetTheme(args.SelectedItem.Id switch
            {
                "harbor" => TransitBoardThemeKind.Harbor,
                "afterglow" => TransitBoardThemeKind.Afterglow,
                _ => TransitBoardThemeKind.Meridian,
            });
        };

        _board.SelectionChanged += (_, args) => _selectedServiceId = args.Selected?.Id;
    }

    private void SeedControls()
    {
        _modeStrip.SetItems(TransitBoardState.BuildModeItems(_mode, _palette));
        _modeStrip.SelectById("departures");
        _routeStrip.SetItems(_state.BuildRouteItems(_routeId, _palette));
        _routeStrip.SelectById("all");
        _themeStrip.SetItems(TransitBoardState.BuildThemeItems(_palette.Kind, _palette));
        _themeStrip.SelectById(_palette.Kind.ToString().ToLowerInvariant());
        _notices.SetItems(_state.Notices);
    }

    private void RefreshData()
    {
        _visibleServices = _state.FilterServices(_routeId, _mode);
        _board.SetServices(_visibleServices);
        if (!_board.SelectService(_selectedServiceId))
        {
            _selectedServiceId = _board.SelectedService?.Id ?? (_visibleServices.Count > 0 ? _visibleServices[0].Id : null);
            _board.SelectService(_selectedServiceId);
        }

        _journey.Service = _board.SelectedService;
    }

    private void RefreshChrome()
    {
        _hero.Title = "Central Exchange Transit";
        _hero.ClockText = $"{_state.ClockText} UTC";
        _hero.SummaryText = _state.BuildHeroSummary(_routeId, _mode, _visibleServices);
        _hero.AdvisoryText = TransitBoardState.BuildAdvisory(_visibleServices);
        _hero.NoticeText = "Concourse rhythm steady  ·  airport screening adds light dwell  ·  night ferry under soft gate shift";

        _modeStrip.SetItems(TransitBoardState.BuildModeItems(_mode, _palette));
        _modeStrip.SelectById(_mode switch
        {
            TransitBoardMode.Arrivals => "arrivals",
            TransitBoardMode.AllBoard => "all",
            _ => "departures",
        });

        _routeStrip.SetItems(_state.BuildRouteItems(_routeId, _palette));
        _routeStrip.SelectById(_routeId);

        _themeStrip.SetItems(TransitBoardState.BuildThemeItems(_palette.Kind, _palette));
        _themeStrip.SelectById(_palette.Kind.ToString().ToLowerInvariant());

        _board.Title = _mode switch
        {
            TransitBoardMode.Arrivals => "Arrivals Board · F3",
            TransitBoardMode.AllBoard => "Network Board · F3",
            _ => "Departures Board · F3",
        };

        _footer.LeftText = $"transitboard  {_palette.Label.ToLowerInvariant()}  route {_routeId}  live {_visibleServices.Count:00}";
        _footer.RightText = "Tab focus  F1 mode  F2 lines  F3 board  F4 palette  d/a/m board  1/2/3 themes  Ctrl+C quit";
    }

    private void SelectMode(TransitBoardMode mode)
    {
        _mode = mode;
        _modeStrip.SelectById(mode switch
        {
            TransitBoardMode.Arrivals => "arrivals",
            TransitBoardMode.AllBoard => "all",
            _ => "departures",
        });
    }

    private void SetTheme(TransitBoardThemeKind kind)
    {
        ApplyTheme(TransitBoardTheme.Resolve(kind));
    }

    private void FocusCurrentLane()
    {
        switch (_focusLane)
        {
            case 0:
                _modeStrip.RequestFocus();
                break;
            case 1:
                _routeStrip.RequestFocus();
                break;
            case 2:
                _board.RequestFocus();
                break;
            case 3:
                _themeStrip.RequestFocus();
                break;
        }
    }

    private void ApplyTheme(TransitBoardPalette palette)
    {
        _palette = palette;
        var theme = palette.Theme;

        _footer.ApplyTheme(theme);
        _hero.TitleStyle = TransitBoardTheme.Foreground(palette.HeroTitle).WithBold();
        _hero.ClockStyle = TransitBoardTheme.Foreground(palette.HeroClock).WithBold();
        _hero.SummaryStyle = theme.Text.Secondary.WithBold();
        _hero.AdvisoryStyle = TransitBoardTheme.Foreground(palette.HeroAccent);
        _hero.NoticeStyle = theme.Text.Muted;
        _hero.DividerStyle = TransitBoardTheme.Foreground(palette.Divider);

        ConfigureChipStrip(_modeStrip, theme);
        ConfigureChipStrip(_routeStrip, theme);
        ConfigureChipStrip(_themeStrip, theme);

        _board.TitleStyle = theme.Text.Secondary.WithBold();
        _board.FocusedTitleStyle = theme.Focus.Title;
        _board.DividerStyle = TransitBoardTheme.Foreground(palette.Divider);
        _board.EmptyStyle = theme.Text.Muted;
        _board.PrimaryTextStyle = theme.Text.Primary.WithBold();
        _board.SecondaryTextStyle = theme.Text.Secondary;
        _board.SelectedRowStyle = TransitBoardTheme.Chip(palette.SelectionForeground, palette.SelectionBackground);
        _board.SelectedSecondaryStyle = TransitBoardTheme.Chip(palette.SelectionForeground, palette.SelectionBackground, false);
        _board.DelayStyle = TransitBoardTheme.Foreground(palette.Delay).WithBold();
        _board.WarningStyle = TransitBoardTheme.Foreground(palette.Warning).WithBold();
        _board.SuccessStyle = TransitBoardTheme.Foreground(palette.Success).WithBold();
        _board.PlatformStyle = TransitBoardTheme.Chip(palette.PlatformForeground, palette.PlatformBackground);
        _board.RouteStyle = TransitBoardTheme.Chip(palette.RouteForeground, palette.RouteBackground);

        _notices.TitleStyle = theme.Text.Secondary.WithBold();
        _notices.DividerStyle = TransitBoardTheme.Foreground(palette.Divider);
        _notices.PrimaryStyle = theme.Text.Primary;
        _notices.SecondaryStyle = theme.Text.Secondary;
        _notices.WarningStyle = TransitBoardTheme.Foreground(palette.Warning).WithBold();
        _notices.DelayStyle = TransitBoardTheme.Foreground(palette.Delay).WithBold();
        _notices.SuccessStyle = TransitBoardTheme.Foreground(palette.Success).WithBold();

        _journey.TitleStyle = theme.Text.Secondary.WithBold();
        _journey.DividerStyle = TransitBoardTheme.Foreground(palette.Divider);
        _journey.PrimaryStyle = theme.Text.Primary;
        _journey.SecondaryStyle = theme.Text.Secondary;
        _journey.AccentStyle = TransitBoardTheme.Foreground(palette.HeroAccent).WithBold();
        _journey.MutedStyle = TransitBoardTheme.Foreground(palette.NoticeMuted);

        _footer.LeftTextStyle = TransitBoardTheme.Chip(palette.FooterForeground, palette.FooterBackground);
        _footer.RightTextStyle = theme.Text.Secondary;
        _footer.FillStyle = theme.Surface.Panel;
    }

    private static void ConfigureChipStrip(TransitChipStripControl control, TesseraTheme theme)
    {
        control.TitleStyle = theme.Text.Muted;
        control.FocusedTitleStyle = theme.Focus.Title;
        control.DividerStyle = theme.Border.Strong;
        control.EmptyStyle = theme.Text.Muted;
    }
}

internal sealed record TransitBoardTickMessage() : Message;
