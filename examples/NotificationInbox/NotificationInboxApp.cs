using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
using TeaSharp.Styles;

internal sealed class NotificationInboxApp : TeaApp
{
    public static readonly TeaTheme DefaultTheme = TeaThemes.Catppuccin(CatppuccinVariant.Macchiato);

    private readonly NotificationInbox _inbox = new()
    {
        Title = "NotificationInbox",
        Padding = Thickness.All(1),
        PageSize = 8,
        FocusMarker = "◆",
    };

    private readonly StatusBar _status = new();

    private bool _isReadOnly;
    private bool _isDisabled;
    private bool _styleAlt;
    private int _selectionChanges;
    private int _addedCount;
    private string _statusText = "widget-only proof: select, read/pin/delete, api seed/add/select/clear";

    public NotificationInboxApp()
    {
        SeedInbox();
        ApplyTheme();
        _inbox.RequestFocus();
        _inbox.SelectionChanged += (_, args) =>
        {
            _selectionChanges++;
            var previous = args.PreviousItem?.Id ?? "-";
            var current = args.SelectedItem?.Id ?? "-";
            _statusText = $"selection {previous}->{current}";
        };
    }

    public override TeaEffect? Update(Message message)
    {
        if (message is not KeyPressed key)
        {
            return null;
        }

        if (key.IsCharacter('c', ModifierKeys.Ctrl))
        {
            return TeaEffects.Quit;
        }

        if (key.IsCharacter('r', ModifierKeys.Ctrl))
        {
            SeedInbox();
            _statusText = "api SetItems(seed): use arrows/page keys or click a row";
            return null;
        }

        if (key.IsCharacter('n', ModifierKeys.Ctrl))
        {
            _addedCount++;
            _inbox.Add(
                message: $"cpu alert {_addedCount}",
                level: _addedCount % 2 == 0 ? NotificationLevel.Error : NotificationLevel.Warning,
                source: "Worker");
            _statusText = $"api Add(cpu alert {_addedCount})";
            return null;
        }

        if (key.IsCharacter('g', ModifierKeys.Ctrl))
        {
            var changed = _inbox.Select(Math.Min(2, Math.Max(0, _inbox.Items.Count - 1)));
            _statusText = $"api Select(2)={changed}";
            return null;
        }

        if (key.IsCharacter('m', ModifierKeys.Ctrl))
        {
            _inbox.MarkAllRead();
            _statusText = "api MarkAllRead()";
            return null;
        }

        if (key.IsCharacter('e', ModifierKeys.Ctrl))
        {
            _inbox.Clear();
            _statusText = "api Clear(): empty state visible";
            return null;
        }

        if (key.IsCharacter('o', ModifierKeys.Ctrl))
        {
            _isReadOnly = !_isReadOnly;
            _inbox.IsReadOnly = _isReadOnly;
            _statusText = $"readonly={_isReadOnly}";
            return null;
        }

        if (key.IsCharacter('i', ModifierKeys.Ctrl))
        {
            _isDisabled = !_isDisabled;
            _inbox.IsDisabled = _isDisabled;
            _statusText = $"disabled={_isDisabled}";
            return null;
        }

        if (key.IsCharacter('t', ModifierKeys.Ctrl))
        {
            _styleAlt = !_styleAlt;
            ApplyTheme();
            _statusText = _styleAlt ? "style=alt" : "style=default";
            return null;
        }

        return null;
    }

    public override Screen Build(ScreenContext context)
    {
        UpdateFooter();

        return Screen.Build(window =>
        {
            window.Padding(1);
            window.Body(new CenterLayout
            {
                Content = _inbox,
                Width = Math.Min(88, Math.Max(56, context.Width - 4)),
                Height = Math.Min(14, Math.Max(10, context.Height - 4)),
            });
            window.Footer(1, _status);
        });
    }

    private void SeedInbox()
    {
        _inbox.SetItems(
        [
            new InboxItem("ci-ok", "Build finished on main", NotificationLevel.Success, new DateTimeOffset(2026, 3, 27, 8, 10, 0, TimeSpan.Zero), "CI", isRead: false, isPinned: true),
            new InboxItem("disk", "Disk usage above 80%", NotificationLevel.Warning, new DateTimeOffset(2026, 3, 27, 8, 14, 0, TimeSpan.Zero), "Host", isRead: false),
            new InboxItem("deploy", "Blue deploy completed", NotificationLevel.Info, new DateTimeOffset(2026, 3, 27, 8, 21, 0, TimeSpan.Zero), "Deploy", isRead: true),
            new InboxItem("auth", "Auth service restarted", NotificationLevel.Info, new DateTimeOffset(2026, 3, 27, 8, 25, 0, TimeSpan.Zero), "API", isRead: false),
            new InboxItem("queue", "Queue latency back to normal", NotificationLevel.Success, new DateTimeOffset(2026, 3, 27, 8, 31, 0, TimeSpan.Zero), "Worker", isRead: true),
            new InboxItem("db", "Replica lag exceeded threshold", NotificationLevel.Error, new DateTimeOffset(2026, 3, 27, 8, 34, 0, TimeSpan.Zero), "DB", isRead: false),
            new InboxItem("tls", "TLS cert rotation due tomorrow", NotificationLevel.Warning, new DateTimeOffset(2026, 3, 27, 8, 40, 0, TimeSpan.Zero), "Edge", isRead: false),
            new InboxItem("job", "Nightly cleanup skipped", NotificationLevel.Error, new DateTimeOffset(2026, 3, 27, 8, 43, 0, TimeSpan.Zero), "Ops", isRead: false),
            new InboxItem("pager", "Pager handoff acknowledged", NotificationLevel.Info, new DateTimeOffset(2026, 3, 27, 8, 45, 0, TimeSpan.Zero), "OnCall", isRead: true),
        ]);
    }

    private void ApplyTheme()
    {
        ThemeScope.Apply(DefaultTheme, _inbox, _status);

        var theme = DefaultTheme;
        _inbox.TitleStyle = theme.Text.Primary;
        _inbox.FocusedTitleStyle = theme.Focus.Ring.WithBold();
        _inbox.ItemStyle = theme.Text.Primary;
        _inbox.SelectedItemStyle = theme.Selection.Foreground.Merge(theme.Selection.Background).WithBold();
        _inbox.HoveredItemStyle = theme.Accent.Secondary.WithUnderline();
        _inbox.UnreadItemStyle = _styleAlt
            ? TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175))
            : TeaStyle.Empty.WithForeground(AnsiColor.Rgb(205, 214, 244));
        _inbox.MutedItemStyle = theme.Text.Muted.WithDim();
        _inbox.InfoItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(137, 180, 250));
        _inbox.SuccessItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(166, 227, 161));
        _inbox.WarningItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(249, 226, 175));
        _inbox.ErrorItemStyle = TeaStyle.Empty.WithForeground(AnsiColor.Rgb(243, 139, 168));
        _inbox.PinnedItemStyle = _styleAlt ? TeaStyle.Empty.WithBackground(AnsiColor.Rgb(59, 66, 97)) : TeaStyle.Empty.WithBold();
        _inbox.DisabledStyle = theme.Text.Muted.WithDim();
        _inbox.EmptyTextStyle = theme.Text.Muted.WithItalic();
        _inbox.SelectedMarker = _styleAlt ? "▶" : ">";
        _inbox.UnselectedMarker = _styleAlt ? "·" : " ";
    }

    private void UpdateFooter()
    {
        var selected = _inbox.SelectedItem?.Id ?? "-";
        var unread = _inbox.Items.Count(static item => !item.IsRead);
        var pinned = _inbox.Items.Count(static item => item.IsPinned);
        var status = _statusText;
        if (_isDisabled)
        {
            status = "disabled: keyboard and pointer inbox interaction blocked";
        }
        else if (_isReadOnly)
        {
            status = "read-only: navigation blocked by current control contract";
        }

        _status.LeftText =
            $"count={_inbox.Items.Count} unread={unread} pinned={pinned} sel={selected} ro={_isReadOnly} dis={_isDisabled} sch={_selectionChanges}";
        _status.RightText =
            $"{status} | Up/Down j/k Home/End PgUp/PgDn select Enter/Space read r toggle-read p pin d/Del remove a all-read c clear click/wheel rows ^R seed ^N add ^G select(2) ^M mark-all ^E clear ^T style ^O ro ^I dis ^C quit";
    }
}
