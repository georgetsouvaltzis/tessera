# TeaSharp Components

TeaSharp now has two composition layers:

- root app layer for normal apps
- advanced component layer for custom or legacy-heavy screens

The root layer is the default path.

## Recommended Imports

Most apps should start with:

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;
```

Add `TeaSharp` for `BorderStyle` and `Thickness`. Add `TeaSharp.Components.Primitives` only when you need lower-level drawing types like `Canvas` or `Rect`.

## Root Model

Normal apps are built from:

- `TeaApp`
- `Screen`
- `ScreenContext`
- `TeaEffects`
- `TeaSharp.Controls.*`
- `TeaSharp.Layout.*`

Recommended flow:

1. `Update(Message)` handles typed app/runtime input.
2. built-in controls receive routed input automatically before `Update(Message)`.
3. `Build(ScreenContext)` returns a screen assembled from named layout objects.

That replaces the older default reliance on `InteractiveScreenModel`, `ScreenComposer`, and `InputRouter`.

## Layout

Primary default layout nouns:

- `WindowLayout`
- `RowLayout`
- `ColumnLayout`
- `PanelLayout`
- `CenterLayout`
- `LayoutSlot`
- `LayoutLength`

Advanced tree-oriented layout primitives still exist:

- `StackLayout`
- `SplitLayout`
- `DockLayout`
- `OverlayLayout`

The default path should read like shallow screen assembly, not nested tree construction.

## Root Controls

Current default catalog:

- `Label`
- `Button`
- `TextInput`
- `TextArea`
- `Choice`
- `ComboBox`
- `Dialog`
- `ProgressBar`
- `LogView`
- `Notifications`
- `Toggle`
- `Slider`
- `Spinner`
- `StatusBar`
- `Tabs`
- `ListView<T>`
- `Table`
- `TreeItem`
- `TreeView`
- `MenuBar`

These names are the preferred public nouns.

## Advanced Components

The older `*Component` catalog still exists for:

- controls that do not have root wrappers yet
- advanced customization
- migration support
- custom-control composition

Important advanced namespaces:

- `TeaSharp.Components.Prebuilt`
- `TeaSharp.Components.Productivity`
- `TeaSharp.Components.Advanced`
- `TeaSharp.Components.Primitives`
- `TeaSharp.Components.Composition`

Examples:

- `ComboboxComponent`
- `SortableTableComponent`
- `ToastCenterComponent`
- `ViewportClass`
- `UiWidgets`
- `Canvas`

These can still be placed directly inside the new layout model through `PanelLayout`, `CenterLayout`, or `LayoutSlot`.

## Legacy Composition APIs

The following remain public but are no longer the default story:

- `ScreenComposer`
- `ComponentComposer`
- `InteractiveScreenModel`
- `InputRouter`
- `ScreenRegionKey`

Use them only when you need lower-level orchestration than the root model provides.

## Example

```csharp
using TeaSharp;
using TeaSharp.Controls;
using TeaSharp.Layout;

internal sealed class SearchApp : TeaApp
{
    private readonly TextInput _query = new()
    {
        Title = "Search",
        Border = BorderStyle.SingleLine,
        Padding = Thickness.All(1),
        Placeholder = "type and press Enter",
    };

    private readonly StatusBar _status = new();

    public override TeaEffect? Update(Message message)
        => message is KeyPressed key && key.IsCharacter('q')
            ? TeaEffects.Quit
            : null;

    public override Screen Build(ScreenContext context)
    {
        _status.LeftText = $"Size {context.Width}x{context.Height}";
        _status.RightText = "q quit";

        return Screen.From(new WindowLayout
        {
            Footer = LayoutSlot.Fixed(_status, 1),
            Body = new CenterLayout(_query, width: 48, height: 5),
            Padding = Thickness.All(1),
        });
    }
}
```

See also:

- `README.md`
- `docs/app-pattern.md`
- `docs/custom-components.md`
- `docs/migration-map.md`
