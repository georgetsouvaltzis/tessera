using Tessera.Controls;
using Tessera.Layout;
using Tessera.Styles;

namespace Tessera.Examples.WorkspaceApp;

internal sealed class WorkspaceApp : TesseraApp
{
    private readonly Label _actionChip =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly TextArea _editor = new() { Title = "Launch Narrative", Padding = Thickness.All(1), Wrap = true };

    private readonly Label _eyebrow =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly Control[] _focusOrder;
    private readonly StatusBar _footer = new() { Fill = ' ' };

    private readonly Label _headline =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly Label _insight = new()
    {
        Title = "Director Notes",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1)
    };

    private readonly Label _preview =
        new() { Title = "Live Preview", Border = BorderStyle.SingleLine, Padding = Thickness.All(1) };

    private readonly ProgressBar _readiness = new() { Title = "Readiness", Padding = Thickness.Symmetric(1) };
    private readonly Button _reviewButton = new() { Text = "Send To Review", Padding = Thickness.Symmetric(2) };
    private readonly Button _snapshotButton = new() { Text = "Save Snapshot", Padding = Thickness.Symmetric(2) };

    private readonly Label _snapshotChip =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly TesseraTheme _theme = WorkspaceTheme.Default;

    private readonly Label _viewChip =
        new() { Border = BorderStyle.None, HorizontalAlignment = HorizontalAlignment.Center };

    private readonly ListView<string> _views = new(static view => view)
    {
        Title = "Scenes",
        Padding = Thickness.All(1)
    };

    private int _focusIndex;
    private string _lastAction = "Draft opened";
    private double _readinessValue = 0.42;
    private string _selectedView = "Overview";
    private int _snapshotCount = 3;

    public WorkspaceApp()
    {
        _focusOrder = [_views, _editor, _snapshotButton, _reviewButton];
        _views.SetItems(["Overview", "Narrative", "Review", "Launch"]);
        _views.SetSelectedIndex(0);
        _editor.SetValue(
            "Northstar Console\n\n" +
            "Shape a calmer launch story. Give the first screen one obvious center of gravity and one obvious next action.\n\n" +
            "Use color to signal hierarchy, not noise.");
        ConfigureTheme();
        WireEvents();
        _views.RequestFocus();
    }

    public override TesseraEffect? Update(Message message)
    {
        switch (message)
        {
            case KeyPressed key when key.IsCharacter('c', ModifierKeys.Ctrl):
                return TesseraEffects.Quit;
            case KeyPressed key when key.Is(Key.Tab):
                FocusNext();
                return null;
            case WorkspaceActionMessage action:
                ApplyAction(action.Kind);
                return null;
            default:
                return null;
        }
    }

    public override Screen Build(ScreenContext context)
    {
        RefreshChrome();

        var shellWidth = Math.Max(88, Math.Min(120, context.Width - 4));
        var shellHeight = Math.Max(20, Math.Min(28, context.Height - 3));
        var leftWidth = Math.Clamp(shellWidth / 5, 18, 22);
        var rightWidth = Math.Clamp(shellWidth / 4, 24, 28);

        return Screen.Build(window =>
        {
            window.Body(body => body.Center(
                center => center.Column(column =>
                {
                    column.Gap(1);
                    column.Auto(content => content.Center(_eyebrow));
                    column.Auto(content => content.Center(_headline));
                    column.Fixed(1, ribbon => ribbon.Center(row => row.Row(chips =>
                    {
                        chips.Gap(2);
                        chips.Auto(_viewChip);
                        chips.Auto(_snapshotChip);
                        chips.Auto(_actionChip);
                    })));
                    column.Fill(main => main.Row(row =>
                    {
                        row.Fixed(leftWidth, _views, new Thickness(0, 0, 1, 0));
                        row.Weighted(1, centerPane => centerPane.Column(stack =>
                        {
                            stack.Fill(_editor);
                            stack.Fixed(3, actions => actions.Row(buttons =>
                            {
                                buttons.Gap(2);
                                buttons.Fixed(16, _snapshotButton);
                                buttons.Fixed(18, _reviewButton);
                            }));
                        }));
                        row.Fixed(rightWidth, right => right.Column(stack =>
                        {
                            stack.Fixed(7, _preview);
                            stack.Fixed(4, _readiness);
                            stack.Fill(_insight);
                        }), new Thickness(1, 0, 0, 0));
                    }));
                }),
                shellWidth,
                shellHeight));
            window.Footer(1, _footer);
        });
    }

    private void ConfigureTheme()
    {
        _views.ApplyTheme(_theme);
        _editor.ApplyTheme(_theme);
        _preview.ApplyTheme(_theme);
        _readiness.ApplyTheme(_theme);
        _insight.ApplyTheme(_theme);
        _snapshotButton.ApplyTheme(_theme);
        _reviewButton.ApplyTheme(_theme);
        _footer.ApplyTheme(_theme);

        _eyebrow.TextStyle = WorkspaceTheme.Surface(0x0A1320, 0x7DE3FF).WithBold();
        _headline.TextStyle = _theme.Text.Primary.WithBold();
        _viewChip.TextStyle = WorkspaceTheme.Surface(0x09131B, 0x7DE3FF).WithBold();
        _snapshotChip.TextStyle = WorkspaceTheme.Surface(0x09131B, 0xFFD46B).WithBold();
        _actionChip.TextStyle = WorkspaceTheme.Surface(0x09131B, 0x86F4B5).WithBold();

        _preview.TitleStyle = _theme.Text.Secondary.WithBold();
        _preview.BorderStyleText = _theme.Border.Strong;
        _preview.TextStyle = _theme.Text.Primary;

        _insight.TitleStyle = _theme.Text.Secondary.WithBold();
        _insight.BorderStyleText = _theme.Border.Strong;
        _insight.TextStyle = _theme.Text.Secondary;

        _readiness.TitleStyle = _theme.Text.Secondary.WithBold();
        _readiness.BorderStyleText = _theme.Border.Strong;
        _readiness.FillStyle = WorkspaceTheme.Foreground(0x86F4B5).WithBold();
        _readiness.TrackStyle = WorkspaceTheme.Foreground(0x31506F);
        _readiness.LabelStyle = _theme.Text.Primary.WithBold();

        _snapshotButton.LabelStyle = WorkspaceTheme.Foreground(0x09131B).WithBold();
        _snapshotButton.SurfaceStyle = WorkspaceTheme.Background(0xFFD46B);
        _snapshotButton.FocusedSurfaceStyle = WorkspaceTheme.Background(0xFFE091);
        _snapshotButton.PressedSurfaceStyle = WorkspaceTheme.Background(0xD8B24A);

        _reviewButton.LabelStyle = WorkspaceTheme.Foreground(0x09131B).WithBold();
        _reviewButton.SurfaceStyle = WorkspaceTheme.Background(0x7DE3FF);
        _reviewButton.FocusedSurfaceStyle = WorkspaceTheme.Background(0xA0ECFF);
        _reviewButton.PressedSurfaceStyle = WorkspaceTheme.Background(0x54B9D9);

        _footer.LeftTextStyle = _theme.Text.Secondary;
        _footer.RightTextStyle = _theme.Accent.Primary;
        _footer.FillStyle = _theme.Surface.Base;
    }

    private void WireEvents()
    {
        _views.SelectionChanged += (_, args) =>
        {
            _selectedView = args.SelectedItem ?? _selectedView;
            _lastAction = $"Focused {_selectedView}";
        };
        _snapshotButton.Activated += (_, _) => Post(new WorkspaceActionMessage(WorkspaceAction.Snapshot));
        _reviewButton.Activated += (_, _) => Post(new WorkspaceActionMessage(WorkspaceAction.SendToReview));
    }

    private void FocusNext()
    {
        _focusIndex = (_focusIndex + 1) % _focusOrder.Length;
        _focusOrder[_focusIndex].RequestFocus();
    }

    private void ApplyAction(WorkspaceAction action)
    {
        switch (action)
        {
            case WorkspaceAction.Snapshot:
                _snapshotCount++;
                _readinessValue = Math.Min(1.0, _readinessValue + 0.08);
                _lastAction = $"Saved snapshot {_snapshotCount} for {_selectedView}";
                break;
            case WorkspaceAction.SendToReview:
                _readinessValue = Math.Min(1.0, _readinessValue + 0.17);
                _lastAction = $"Sent {_selectedView} to review";
                break;
        }
    }

    private void RefreshChrome()
    {
        var actionChip = Shorten(_lastAction, 24);

        _eyebrow.Text = "  WORKSPACE APP // MULTI-PANE STARTER  ";
        _headline.Text =
            "A centered workstation shell with navigation, editing, preview, and one clear promotion path.";
        _viewChip.Text = $"  view {_selectedView.ToLowerInvariant()}  ";
        _snapshotChip.Text = $"  snapshots {_snapshotCount:D2}  ";
        _actionChip.Text = $"  {actionChip.ToLowerInvariant()}  ";

        var note = _editor.Value.Replace('\n', ' ').Trim();
        if (note.Length > 120)
        {
            note = $"{note[..117]}...";
        }

        var previewAction = Shorten(_lastAction, 16);
        var insightNote = Shorten(note, 38);

        _preview.Text = string.Join(
            '\n',
            $"view     {_selectedView}",
            "draft    northstar shell",
            $"snaps    {_snapshotCount:D2}",
            $"action   {previewAction}");

        _readiness.SetValue(_readinessValue);
        _insight.Text = string.Join(
            '\n',
            "current intent",
            insightNote,
            string.Empty,
            "next move",
            "lead with one action,",
            "keep the shell calm.");

        _footer.LeftText = "WorkspaceApp";
        _footer.RightText = "Center the shell. Then scale out to the flagship apps.";
    }

    private static string Shorten(string value, int maxLength)
    {
        if (value.Length <= maxLength)
        {
            return value;
        }

        return $"{value[..Math.Max(0, maxLength - 3)]}...";
    }

    private enum WorkspaceAction
    {
        Snapshot,
        SendToReview
    }

    private sealed record WorkspaceActionMessage(WorkspaceAction Kind) : Message;
}
