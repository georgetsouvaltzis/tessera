# TeaSharp App Pattern

Use this pattern for screen-oriented apps with:

- multiple panes
- a command/input area
- overlays or dialogs
- global shortcuts
- mouse + keyboard routing

The intended shell is:

1. `InteractiveScreenModel`
2. `ScreenRegionKey`
3. `ScreenComposer`
4. `TeaSharp.Layout`
5. `InputRouter`
6. `TerminalOutput`

## Default Shape

```csharp
using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.ScreenOutput;

internal sealed class DemoModel : InteractiveScreenModel
{
    private static readonly ScreenRegionKey TabsRegion = new("demo.tabs");
    private static readonly ScreenRegionKey EditorRegion = new("demo.editor");
    private static readonly ScreenRegionKey DialogRegion = new("demo.dialog");

    private readonly TabsComponent _tabs = new(new TabsOptions(["Home", "Data"]));
    private readonly TextInputComponent _editor = new(new TextInputOptions(
        Title: "Command",
        Placeholder: "type and press enter",
        ClearOnSubmit: true));
    private readonly DialogComponent _dialog = new(new DialogOptions(
        Title: "Confirm",
        BodyLines: ["Delete item?", "Enter/Space accept", "Esc cancel"]));

    private int _width = 100;
    private int _height = 30;

    public DemoModel()
    {
        InputRouter
            .AddScope("system", InputScopeKind.System, static () => true, HandleSystemKey)
            .AddScope("modal", InputScopeKind.Modal, () => _dialog.IsVisible, HandleDialogKey, InputScopeBehavior.CaptureWhileActive)
            .AddScope(
                "focused",
                InputScopeKind.FocusedRegion,
                () => FocusedRegionKey is not null,
                HandleFocusedKey,
                blocksGlobalShortcuts: key => FocusedRegionKey == EditorRegion
                    && key.Modifiers == KeyModifiers.None
                    && key.Code == KeyCode.Character)
            .AddScope("global", InputScopeKind.Global, static () => true, HandleGlobalKey);
    }

    public override Effect? Init() => null;

    public override Effect? Update(IMessage message)
    {
        return message switch
        {
            WindowSizeMsg ws => HandleResize(ws),
            MouseMsg mouse => RouteMouse(mouse) ? null : null,
            KeyPressMsg key => RouteKey(key),
            _ => null,
        };
    }

    public override ModelView Render()
    {
        if (_width < 60 || _height < 18)
        {
            return ModelView.From("Terminal too small.");
        }

        var canvas = new Canvas(_width, _height, CanvasTextMode.GraphemeAware);
        canvas.Clear();
        RenderScreen(canvas);

        return ModelView.From(canvas.Render()) with
        {
            Terminal = new TerminalOutput
            {
                AltScreen = true,
                EnableBracketedPaste = true,
                EnableFocusReporting = true,
                MouseMode = MouseMode.AllMotion,
                WindowTitle = "TeaSharp Demo",
            },
        };
    }

    protected override Rect GetBodyRect() => new(0, 0, _width, _height);

    protected override ScreenRegionKey? PreferredFocusRegionKey
        => _dialog.IsVisible ? DialogRegion : EditorRegion;

    protected override bool CanBuildScreen => _width >= 60 && _height >= 18;

    protected override void ComposeScreen(Rect bodyRect)
    {
        var frame = Frame(bodyRect, headerHeight: 1);
        Screen.AddComponent(TabsRegion, frame.Header, _tabs);
        Screen.AddRegion(EditorRegion, frame.Body, _editor.Render, _editor.Update, focusable: true);

        if (_dialog.IsVisible)
        {
            Screen.AddModalComponent(DialogRegion, bodyRect, _dialog);
        }
    }

    private Effect? HandleResize(WindowSizeMsg ws)
    {
        _width = ws.Width;
        _height = ws.Height;
        return null;
    }

    private InputRouteResult HandleSystemKey(KeyPressMsg key)
        => key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c')
            ? InputRouteResult.FromEffect(Tea.Effects.Quit)
            : InputRouteResult.NotHandled;

    private InputRouteResult HandleDialogKey(KeyPressMsg key)
    {
        if (!RouteFocusedMessage(key))
        {
            return InputRouteResult.NotHandled;
        }

        return InputRouteResult.HandledWithoutEffect;
    }

    private InputRouteResult HandleFocusedKey(KeyPressMsg key)
        => RouteFocusedMessage(key)
            ? InputRouteResult.HandledWithoutEffect
            : InputRouteResult.NotHandled;

    private InputRouteResult HandleGlobalKey(KeyPressMsg key)
    {
        if (key.Is(KeyCode.Tab))
        {
            FocusNext();
            return InputRouteResult.HandledWithoutEffect;
        }

        if (key.IsCharacter('d'))
        {
            _dialog.IsVisible = !_dialog.IsVisible;
            SetFocus(_dialog.IsVisible ? DialogRegion : EditorRegion);
            return InputRouteResult.HandledWithoutEffect;
        }

        return InputRouteResult.NotHandled;
    }
}
```

Wire modal decisions and submit-style actions through component events in the model constructor, for example `_dialog.Accepted += ...`, `_dialog.Dismissed += ...`, and `_input.Submitted += ...`, so the update loop stays focused on routing instead of action polling.

For focus-heavy screens, create an ordered focus chain once and reuse the built-in helpers:

```csharp
private readonly ScreenFocusChain _focusChain;

public WorkspaceModel()
{
    _focusChain = CreateFocusChain(TabsRegion, EditorRegion, DetailsRegion);
}

private InputRouteResult HandleGlobalKey(KeyPressMsg key)
{
    if (HandleTabNavigation(key, _focusChain))
    {
        return InputRouteResult.HandledWithoutEffect;
    }

    return InputRouteResult.NotHandled;
}
```

Use `CaptureFocus()` before opening a modal or palette and `RestoreFocus(...)` when it closes if you want automatic return-to-previous-region behavior.

For the common two-pane app shape, prefer the built-in master-detail scaffold over manual body splitting:

```csharp
protected override void ComposeScreen(Rect bodyRect)
{
    var shell = MasterDetail(bodyRect, masterWidth: 28, headerHeight: 1, footerHeight: 1);

    shell.AddHeader(HeaderRegion, _menuBar);
    shell.AddMaster(ListRegion, _items);
    shell.AddDetail(DetailsRegion, _details);
    shell.AddFooter(StatusRegion, _statusBar);

    if (FocusFirst(shell.CreateFocusChain()))
    {
        return;
    }
}
```

This keeps shell layout, region registration, and focus order in one place.

For dashboard-style apps, use the sidebar scaffold instead of manually splitting the body every time:

```csharp
protected override void ComposeScreen(Rect bodyRect)
{
    var shell = Dashboard(bodyRect, sidebarWidth: 24, headerHeight: 1, footerHeight: 1);

    shell.AddHeader(HeaderRegion, _tabs);
    shell.AddSidebar(FiltersRegion, _filters);
    shell.AddMain(MainRegion, _table);
    shell.AddFooter(StatusRegion, _statusBar);
}
```

This keeps the common `header + sidebar + main + footer` shell as a first-class API.

For form-style apps, use the built-in form scaffold so action bars stop being ad-hoc row math:

```csharp
protected override void ComposeScreen(Rect bodyRect)
{
    var shell = Form(bodyRect, actionsHeight: 2, headerHeight: 1, footerHeight: 1);

    shell.AddHeader(HeaderRegion, _titleBar);
    shell.AddBody(FormRegion, _editor);
    shell.AddActions(ActionsRegion, _actionBar);
    shell.AddFooter(StatusRegion, _statusBar);
}
```

When the screen is mostly structure rather than named shell regions, prefer the layout facade over manual `Rect` math:

```csharp
using TeaSharp.Layout;
using TeaSharp.Styles;

protected override void ComposeScreen(Rect bodyRect)
{
    Compose(
        Dock.Layout(
            top: Slot.Auto(_tabs, HeaderRegion, preferredHeight: 1),
            fill: Slot.Fill(
                Split.Columns(
                    left: Slot.Fixed(24, _filters, FiltersRegion),
                    right: Slot.Fill(
                        Panel.Column(
                            [
                                Slot.Auto(_summary, SummaryRegion, preferredHeight: 3),
                                Slot.Fill(_table, MainRegion),
                            ],
                            gap: 1,
                            title: "Main",
                            border: BorderStyle.Rounded,
                            padding: Thickness.All(1)))))),
        bodyRect);
}
```

For centered splash screens or empty states, the one-line path is:

```csharp
var layout = Center.Text("Hello World", style: TeaStyle.Empty.WithBold());
Compose(layout, bodyRect);
```

This keeps the common `header + body + actions + footer` workflow shell as a first-class API.

For confirm flows, prefer a dialog workflow over manual `IsVisible` toggles plus `CaptureFocus()` / `RestoreFocus(...)` plumbing:

```csharp
private readonly DialogComponent _deleteDialog = new(new DialogOptions(Title: "Delete"));
private readonly DialogWorkflow _deleteWorkflow;

public WorkspaceModel()
{
    _deleteWorkflow = CreateDialogWorkflow(_deleteDialog, DeleteDialogRegion, _focusChain);
    _deleteDialog.Accepted += (_, _) => DeleteItem();
}

protected override void ComposeScreen(Rect bodyRect)
{
    // other regions
    _deleteWorkflow.Compose(bodyRect);
}

private InputRouteResult HandleGlobalKey(KeyPressMsg key)
{
    if (key.IsCharacter('x'))
    {
        _deleteWorkflow.Show("Delete item", ["Delete the selected item?"]);
        return InputRouteResult.HandledWithoutEffect;
    }

    return InputRouteResult.NotHandled;
}
```

This keeps modal registration, open/close state, and focus restoration in one place.

## Scope Order

Use this order unless you have a clear reason not to:

1. `System`
2. `Modal`
3. `Palette`
4. `Effect`
5. `FocusedRegion`
6. `Global`

Meaning:

- `System`: emergency keys like `Ctrl+C`
- `Modal`: delete/confirm dialogs
- `Palette`: command palette overlays
- `CommandBar`: command bar / shell input
- `FocusedRegion`: active pane or widget
- `Global`: app-wide shortcuts

## Practical Rules

- Keep region keys as `static readonly ScreenRegionKey` fields.
- Let `ScreenComposer` own region focus and pointer hit-testing.
- Use `Frame(...)` for the common header/body/footer shell before dropping into custom rect math.
- Let `InputRouter` own key precedence.
- Let `InteractiveScreenModel` own screen rebuild timing.
- Prefer component action events for discrete user actions; keep `TryConsume...` helpers for pull-style update loops that want explicit consumption.
- Prefer `CreateFocusChain(...)`, `HandleTabNavigation(...)`, `CaptureFocus()`, and `RestoreFocus(...)` over per-app focus enums for standard screen navigation.
- Use `blocksGlobalShortcuts` for plain character suppression while text input is active.
- Prefer `SetFocus(...)`, `FocusNext()`, and `FocusPrevious()` from `InteractiveScreenModel` instead of reaching into `Screen` directly.
- Put terminal capability toggles in `TerminalOutput`, not in app-local routing code.

## When Not To Use This

Skip this pattern if the app is:

- a tiny single-widget demo
- render-only
- a small component experiment using only `ComponentComposer`

For everything else, this should be the default starting point.

## Starter Guidance

If you want a real runnable reference for this pattern, start with `examples/WidgetGallery`.

It already demonstrates:

- `InteractiveScreenModel`
- `Dashboard(...)`
- `Form(...)`
- `CreateDialogWorkflow(...)`
- event-driven widget integration

Other example apps may still use more manual `IScreen`-level composition and should be treated as advanced examples, not the default starting point.
