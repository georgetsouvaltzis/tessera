using System.Globalization;
using TeaSharp.Controls;
using TeaSharp.Styles;

namespace TeaSharp.Examples.TransitBoard;

internal enum TransitBoardMode
{
    Departures,
    Arrivals,
    AllBoard,
}

internal sealed class TransitBoardState
{
    private readonly List<TransitRoute> _routes;
    private readonly List<TransitService> _services;
    private readonly List<TransitNotice> _notices;
    private readonly Random _random = new(7413);
    private int _tick;

    private TransitBoardState(List<TransitRoute> routes, List<TransitService> services, List<TransitNotice> notices)
    {
        _routes = routes;
        _services = services;
        _notices = notices;
    }

    public static TransitBoardState CreateSeed()
    {
        var now = DateTimeOffset.UtcNow;

        List<TransitRoute> routes =
        [
            new TransitRoute("all", "All", "network"),
            new TransitRoute("aero", "Aero", "airport"),
            new TransitRoute("coast", "Coast", "harbor"),
            new TransitRoute("civic", "Civic", "loop"),
            new TransitRoute("inter", "Inter", "fast"),
            new TransitRoute("night", "Night", "pier"),
        ];

        List<TransitService> services =
        [
            Service("svc_170", "A1", "AeroLink", "North Terminal", "via Skybridge", "4", now.AddMinutes(3), false, 0, "boarding", "All doors open", "Gate 4 stable", "Concourse B", Calls("Central Exchange", "Museum Dock", "Skybridge", "North Terminal", now.AddMinutes(3))),
            Service("svc_182", "C7", "Coastline", "Harbor Point", "via Customs Hall", "7", now.AddMinutes(5), false, 4, "delayed", "+04 weather check", "Gate 7 -> 9", "Concourse A", Calls("Central Exchange", "Canal Yard", "Customs Hall", "Harbor Point", now.AddMinutes(5))),
            Service("svc_191", "L2", "Civic Loop", "Old Market", "via River Walk", "2", now.AddMinutes(7), false, 0, "final call", "Doors closing", "Gate 2 stable", "Concourse C", Calls("Central Exchange", "River Walk", "Civic Square", "Old Market", now.AddMinutes(7))),
            Service("svc_205", "I4", "InterCity", "Northgate", "via Meridian", "11", now.AddMinutes(12), false, 0, "on time", "Express formation", "Gate 11 stable", "Platform Hall", Calls("Central Exchange", "Meridian", "West Spur", "Northgate", now.AddMinutes(12))),
            Service("svc_218", "N8", "Night Ferry", "Dock Eight", "via Glass Pier", "15", now.AddMinutes(15), false, 2, "gate change", "+02 boarding hold", "Gate 15 -> 12", "Pier Floor", Calls("Central Exchange", "Glass Pier", "Low Water", "Dock Eight", now.AddMinutes(15))),
            Service("svc_231", "A3", "AeroLink", "South Terminal", "via Runway South", "5", now.AddMinutes(18), false, 0, "on time", "Heavy luggage flow", "Gate 5 stable", "Concourse B", Calls("Central Exchange", "Runway South", "Cargo Annex", "South Terminal", now.AddMinutes(18))),
            Service("svc_246", "C1", "Coastline", "Lighthouse", "via Salt Yard", "8", now.AddMinutes(22), false, 0, "on time", "Rain sweep active", "Gate 8 stable", "Concourse A", Calls("Central Exchange", "Salt Yard", "Marina South", "Lighthouse", now.AddMinutes(22))),
            Service("svc_259", "I2", "InterCity", "Stonebridge", "via Northgate", "10", now.AddMinutes(27), false, 9, "delayed", "+09 upstream hold", "Gate 10 stable", "Platform Hall", Calls("Central Exchange", "Northgate", "Red Vale", "Stonebridge", now.AddMinutes(27))),
            Service("svc_301", "A1", "AeroLink", "Central Exchange", "from North Terminal", "4", now.AddMinutes(2), true, 0, "landing", "Arriving platform 4", "Gate 4 stable", "Concourse B", Calls("North Terminal", "Skybridge", "Museum Dock", "Central Exchange", now.AddMinutes(2))),
            Service("svc_309", "C7", "Coastline", "Central Exchange", "from Harbor Point", "9", now.AddMinutes(6), true, 3, "approaching", "+03 tidal buffer", "Gate 9 stable", "Concourse A", Calls("Harbor Point", "Customs Hall", "Canal Yard", "Central Exchange", now.AddMinutes(6))),
            Service("svc_316", "L2", "Civic Loop", "Central Exchange", "from Old Market", "2", now.AddMinutes(9), true, 0, "arriving", "Loop frequency restored", "Gate 2 stable", "Concourse C", Calls("Old Market", "Civic Square", "River Walk", "Central Exchange", now.AddMinutes(9))),
            Service("svc_324", "I4", "InterCity", "Central Exchange", "from Northgate", "11", now.AddMinutes(14), true, 0, "arriving", "Fast consist", "Gate 11 stable", "Platform Hall", Calls("Northgate", "West Spur", "Meridian", "Central Exchange", now.AddMinutes(14))),
        ];

        List<TransitNotice> notices =
        [
            new TransitNotice("amber", "Platform 9", "Harbor point boarding moved from 7 to 9 due to wind corridor."),
            new TransitNotice("blue", "Skybridge", "Airport baggage screening is adding two minutes to AeroLink dwell time."),
            new TransitNotice("mint", "Civic Loop", "Inner-city headways normalized after crowd release from museum district."),
            new TransitNotice("rose", "Dock Eight", "Night Ferry footbridge under one-way movement after maintenance sweep."),
        ];

        return new TransitBoardState(routes, services, notices);
    }

    public IReadOnlyList<TransitRoute> Routes => _routes;
    public string ClockText => DateTimeOffset.UtcNow.AddSeconds(_tick).ToString("HH:mm:ss", CultureInfo.InvariantCulture);

    public IReadOnlyList<TransitService> FilterServices(string routeId, TransitBoardMode mode)
    {
        IEnumerable<TransitService> services = _services;

        if (!string.Equals(routeId, "all", StringComparison.Ordinal))
        {
            services = services.Where(service => string.Equals(service.RouteId, routeId, StringComparison.Ordinal));
        }

        services = mode switch
        {
            TransitBoardMode.Departures => services.Where(static service => !service.IsArrival),
            TransitBoardMode.Arrivals => services.Where(static service => service.IsArrival),
            _ => services,
        };

        return services.OrderBy(service => service.DepartsAt).ToArray();
    }

    public IReadOnlyList<TransitNotice> Notices => _notices;

    public TransitService? FindService(string id) =>
        _services.FirstOrDefault(service => string.Equals(service.Id, id, StringComparison.Ordinal));

    public string BuildHeroSummary(string routeId, TransitBoardMode mode, IReadOnlyList<TransitService> visible)
    {
        var route = _routes.First(route => string.Equals(route.Id, routeId, StringComparison.Ordinal));
        var label = mode switch
        {
            TransitBoardMode.Arrivals => "Arrivals board",
            TransitBoardMode.AllBoard => "Mixed board",
            _ => "Departures board",
        };
        var delayed = visible.Count(static service => service.DelayMinutes > 0);
        return $"{route.Label}  ·  {label}  ·  {visible.Count:00} live services  ·  {delayed:00} marked";
    }

    public static string BuildAdvisory(IReadOnlyList<TransitService> visible)
    {
        var next = visible.Count > 0 ? visible[0] : null;
        if (next is null)
        {
            return "No live services in the active slice.";
        }

        var marker = next.DelayMinutes > 0 ? $"+{next.DelayMinutes:00}" : "on time";
        return $"Next movement: {next.RouteCode} to {next.Destination} from platform {next.Platform}  ·  {marker}  ·  {next.Concourse}";
    }

    public TransitChipItem[] BuildRouteItems(string selectedRouteId, TransitBoardPalette palette)
    {
        return _routes.Select(route =>
        {
            var isSelected = string.Equals(route.Id, selectedRouteId, StringComparison.Ordinal);
            return new TransitChipItem(
                route.Id,
                route.Label,
                route.Subtitle,
                isSelected
                    ? TransitBoardTheme.Chip(palette.SelectionForeground, palette.SelectionBackground)
                    : TransitBoardTheme.Foreground(palette.RouteForeground).WithBold(),
                isSelected
                    ? TransitBoardTheme.Chip(palette.SelectionForeground, palette.SelectionBackground)
                    : palette.Theme.Text.Secondary);
        }).ToArray();
    }

    public static TransitChipItem[] BuildModeItems(TransitBoardMode selectedMode, TransitBoardPalette palette)
    {
        return
        [
            ModeItem("departures", "Departures", "next out", selectedMode == TransitBoardMode.Departures, palette),
            ModeItem("arrivals", "Arrivals", "coming in", selectedMode == TransitBoardMode.Arrivals, palette),
            ModeItem("all", "All Board", "network", selectedMode == TransitBoardMode.AllBoard, palette),
        ];
    }

    public static TransitChipItem[] BuildThemeItems(TransitBoardThemeKind selectedTheme, TransitBoardPalette palette)
    {
        return
        [
            ThemeItem(TransitBoardThemeKind.Meridian, selectedTheme, palette),
            ThemeItem(TransitBoardThemeKind.Harbor, selectedTheme, palette),
            ThemeItem(TransitBoardThemeKind.Afterglow, selectedTheme, palette),
        ];
    }

    public void Advance()
    {
        _tick++;
        if (_tick % 5 != 0)
        {
            return;
        }

        foreach (var service in _services)
        {
            service.DelayMinutes = Math.Clamp(service.DelayMinutes + _random.Next(-1, 2), 0, 11);
            service.Platform = service.DelayMinutes >= 8 && service.Platform != "12" ? "12" : service.Platform;
            service.Status = service.DelayMinutes switch
            {
                >= 8 => "delayed",
                >= 3 => "watch",
                _ when service.IsArrival => "approaching",
                _ => "on time",
            };

            service.MarkerText = service.DelayMinutes switch
            {
                0 when service.Status == "final call" => "Doors closing",
                0 => "Right on signal",
                _ => $"+{service.DelayMinutes:00} regulation",
            };
        }
    }

    private static TransitService Service(
        string id,
        string routeCode,
        string routeLabel,
        string destination,
        string via,
        string platform,
        DateTimeOffset departsAt,
        bool isArrival,
        int delayMinutes,
        string status,
        string markerText,
        string gateChange,
        string concourse,
        IReadOnlyList<TransitCall> calls)
    {
        return new TransitService(id, routeLabel.ToLowerInvariant()[..Math.Min(5, routeLabel.Length)], routeCode, routeLabel, destination, via, platform, departsAt, isArrival, delayMinutes, status, markerText, gateChange, concourse, calls);
    }

    private static List<TransitCall> Calls(string first, string second, string third, string fourth, DateTimeOffset arrival)
    {
        return
        [
            new TransitCall(first, arrival.AddMinutes(-18)),
            new TransitCall(second, arrival.AddMinutes(-12)),
            new TransitCall(third, arrival.AddMinutes(-6)),
            new TransitCall(fourth, arrival),
        ];
    }

    private static TransitChipItem ModeItem(string id, string label, string subtitle, bool selected, TransitBoardPalette palette)
    {
        var primary = selected
            ? TransitBoardTheme.Chip(palette.SelectionForeground, palette.SelectionBackground)
            : TransitBoardTheme.Foreground(palette.HeroAccent).WithBold();
        var secondary = selected
            ? TransitBoardTheme.Chip(palette.SelectionForeground, palette.SelectionBackground)
            : palette.Theme.Text.Secondary;
        return new TransitChipItem(id, label, subtitle, primary, secondary);
    }

    private static TransitChipItem ThemeItem(TransitBoardThemeKind kind, TransitBoardThemeKind selectedTheme, TransitBoardPalette palette)
    {
        var selected = kind == selectedTheme;
        return new TransitChipItem(
            kind.ToString().ToLowerInvariant(),
            kind.ToString(),
            selected ? "live" : "theme",
            selected
                ? TransitBoardTheme.Chip(palette.FooterForeground, palette.FooterBackground)
                : palette.Theme.Text.Secondary.WithBold(),
            selected
                ? TransitBoardTheme.Chip(palette.FooterForeground, palette.FooterBackground)
                : palette.Theme.Text.Muted);
    }
}

internal sealed record TransitRoute(string Id, string Label, string Subtitle);

internal sealed class TransitService(
    string id,
    string routeId,
    string routeCode,
    string routeLabel,
    string destination,
    string via,
    string platform,
    DateTimeOffset departsAt,
    bool isArrival,
    int delayMinutes,
    string status,
    string markerText,
    string gateChange,
    string concourse,
    IReadOnlyList<TransitCall> calls)
{
    public string Id { get; } = id;
    public string RouteId { get; } = routeId;
    public string RouteCode { get; } = routeCode;
    public string RouteLabel { get; } = routeLabel;
    public string Destination { get; } = destination;
    public string Via { get; } = via;
    public string Platform { get; set; } = platform;
    public DateTimeOffset DepartsAt { get; } = departsAt;
    public bool IsArrival { get; } = isArrival;
    public int DelayMinutes { get; set; } = delayMinutes;
    public string Status { get; set; } = status;
    public string MarkerText { get; set; } = markerText;
    public string GateChange { get; } = gateChange;
    public string Concourse { get; } = concourse;
    public IReadOnlyList<TransitCall> Calls { get; } = calls;

    public string DisplayTime => DepartsAt.AddMinutes(DelayMinutes).ToString("HH:mm", CultureInfo.InvariantCulture);
}

internal sealed record TransitNotice(string Tone, string Label, string Message);
internal sealed record TransitCall(string Stop, DateTimeOffset Time);
internal sealed record TransitChipItem(string Id, string Label, string Subtitle, TeaStyle PrimaryStyle, TeaStyle SecondaryStyle);
