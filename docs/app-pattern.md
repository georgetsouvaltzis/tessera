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
4. `InputRouter`
5. `ViewTerminal`

## Default Shape

```csharp
using TeaSharp.Components.Composition;
using TeaSharp.Components.Prebuilt;
using TeaSharp.Components.Primitives;
using TeaSharp.Core.Abstractions;
using TeaSharp.Core.Messages;
using ModelView = TeaSharp.Core.Abstractions.View;

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
        Lines: ["Delete item?", "Enter/Space accept", "Esc cancel"]));

    private int _width = 100;
    private int _height = 30;

    public DemoModel()
    {
        InputRouter
            .AddScope("system", InputScopeKind.System, static () => true, HandleSystemKey)
            .AddScope("modal", InputScopeKind.Modal, () => _dialog.Visible, HandleDialogKey, InputScopeBehavior.CaptureWhileActive)
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

    public override Command? Init() => null;

    public override Command? Update(IMessage message)
    {
        return message switch
        {
            WindowSizeMsg ws => HandleResize(ws),
            MouseMsg mouse => RouteMouse(mouse) ? null : null,
            KeyPressMsg key => RouteKey(key),
            _ => null,
        };
    }

    public override ModelView View()
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
            Terminal = new ViewTerminal
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
        => _dialog.Visible ? DialogRegion : EditorRegion;

    protected override bool CanBuildScreen => _width >= 60 && _height >= 18;

    protected override void ComposeScreen(Rect bodyRect)
    {
        var frame = Frame(bodyRect, headerHeight: 1);
        Screen.AddComponent(TabsRegion, frame.Header, _tabs);
        Screen.AddRegion(EditorRegion, frame.Body, _editor.Render, _editor.Update, focusable: true);

        if (_dialog.Visible)
        {
            Screen.AddModalComponent(DialogRegion, bodyRect, _dialog);
        }
    }

    private Command? HandleResize(WindowSizeMsg ws)
    {
        _width = ws.Width;
        _height = ws.Height;
        return null;
    }

    private InputRouteResult HandleSystemKey(KeyPressMsg key)
        => key.Modifiers.HasFlag(KeyModifiers.Ctrl) && key.IsCharacter('c')
            ? InputRouteResult.FromCommand(Tea.Cmd.Quit)
            : InputRouteResult.NotHandled;

    private InputRouteResult HandleDialogKey(KeyPressMsg key)
    {
        if (!RouteFocusedMessage(key))
        {
            return InputRouteResult.NotHandled;
        }

        return InputRouteResult.HandledWithoutCommand;
    }

    private InputRouteResult HandleFocusedKey(KeyPressMsg key)
        => RouteFocusedMessage(key)
            ? InputRouteResult.HandledWithoutCommand
            : InputRouteResult.NotHandled;

    private InputRouteResult HandleGlobalKey(KeyPressMsg key)
    {
        if (key.Is(KeyCode.Tab))
        {
            FocusNext();
            return InputRouteResult.HandledWithoutCommand;
        }

        if (key.IsCharacter('d'))
        {
            _dialog.Visible = !_dialog.Visible;
            SetFocus(_dialog.Visible ? DialogRegion : EditorRegion);
            return InputRouteResult.HandledWithoutCommand;
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
        return InputRouteResult.HandledWithoutCommand;
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

## Scope Order

Use this order unless you have a clear reason not to:

1. `System`
2. `Modal`
3. `Palette`
4. `Command`
5. `FocusedRegion`
6. `Global`

Meaning:

- `System`: emergency keys like `Ctrl+C`
- `Modal`: delete/confirm dialogs
- `Palette`: command palette overlays
- `Command`: command bar / shell input
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
- Put terminal capability toggles in `ViewTerminal`, not in app-local routing code.

## When Not To Use This

Skip this pattern if the app is:

- a tiny single-widget demo
- render-only
- a small component experiment using only `ComponentComposer`

For everything else, this should be the default starting point.
