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

The older `*Component` catalog still exists only for:

- a small advanced band where we still intentionally expose raw seams
- advanced customization
- custom-control composition

Important cutoff:

- `TextBlockComponent`, `ButtonComponent`, and `TextInputComponent` are removed. Use `Label`, `Button`, and `TextInput`.

Important advanced namespaces:

- `TeaSharp.Components.Prebuilt`
- `TeaSharp.Components.Productivity`
- `TeaSharp.Components.Advanced`
- `TeaSharp.Components.Primitives`
- `TeaSharp.Components.Composition`

Examples:

- `Canvas`
- `TeaSharp.Controls.LineChartOptions` when you need advanced chart rendering knobs through root `LineChart.Options`
- `TeaSharp.Controls.BarChartOptions` when you need advanced chart rendering knobs through root `BarChart.Options`

These advanced types now exist primarily as option records used through the root chart controls. Raw canvas-component interop still exists separately through `ICanvasComponent`.

## Legacy Composition APIs

The old composition engine is now mostly internal-only.

Explicit region-key composition is now internal-only.

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
            Footer = new LayoutSlot
            {
                Content = _status,
                Length = 1,
            },
            Body = new CenterLayout
            {
                Content = _query,
                Width = 48,
                Height = 5,
            },
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
