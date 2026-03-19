# TeaSharp Widget Roadmap V1

This roadmap defines the widget-heavy Public V1 path targeting **40-50 built-in widgets** with consistent APIs and theming.

## Target Inventory

### Current Built-ins (baseline)
- `Label`, `Button`, `TextInput`, `TextArea`, `StatusBar`
- `Choice`, `ComboBox`, `ListView<T>`, `Table`, `TreeView`, `TreeItem`, `Tabs`, `MenuBar`
- `Dialog`, `Modal`, `Notifications`, `ContextMenu`, `CommandPalette`
- `Toggle`, `Slider`, `Spinner`, `ProgressBar`, `LogView`
- `Badge`, `Accordion`, `NumberInput`, `DatePicker`, `TimePicker`
- `MultiSelect`, `RadioGroup`, `Gauge`, `MiniLog`, `StatsCard`
- `BarChart`, `LineChart`, `MarkdownView`

### Planned Expansion (to reach 40-50 total)
- `DataGrid` (virtualized rows/columns)
- `TreeTable`
- `FileExplorer`
- `FuzzyFinder`
- `Breadcrumb`
- `CommandBar`
- `Toolbar`
- `Paginator`
- `PropertyGrid`
- `DiffView`
- `Timeline`
- `Stepper` (wizard flow)
- `KeyValueList` (inspector panel)
- `SearchBox` (highlight + result nav)
- `ToastCenter`

## Prioritized V1 Tranche (10-15)
1. `DataGrid`
2. `FuzzyFinder`
3. `Breadcrumb`
4. `CommandBar`
5. `PropertyGrid`
6. `FileExplorer`
7. `TreeTable`
8. `DiffView`
9. `Paginator`
10. `Toolbar`
11. `SearchBox`
12. `ToastCenter`

Acceptance for V1 tranche:
- implemented in `TeaSharp.Controls` as first-class controls
- covered by unit tests + example usage
- documented in control catalog/docs
- theme-aware and overrideable

## V1 Tranche Progress
- `[x]` `Breadcrumb` (shipped)
- `[x]` `Paginator` (shipped)
- `[ ]` `DataGrid`
- `[ ]` `FuzzyFinder`
- `[ ]` `CommandBar`
- `[ ]` `PropertyGrid`
- `[ ]` `FileExplorer`
- `[ ]` `TreeTable`
- `[ ]` `DiffView`
- `[ ]` `Toolbar`
- `[ ]` `SearchBox`
- `[ ]` `ToastCenter`

## API Consistency Rules
- All widgets derive from `Control`.
- Input routing follows normal control `Handle(...)` semantics.
- Public notifications use `EventHandler` / `EventHandler<TEventArgs>`.
- Mutable configuration via explicit properties (no implicit hidden globals).
- Names must align with existing control vocabulary.
- No `TeaSharp.Core.*` types in default widget authoring APIs.

## Theming and Customization Requirements
Each new widget must support:
- global theme application through `TeaRuntimeOptions.Theme`
- per-widget style overrides for default/focused/selected/disabled/error states
- focus rendering override (not hardcoded marker-only behavior)
- readable output in both color-capable and monochrome terminals

Minimum style hooks per widget:
- text/content style
- border/title style
- focus style
- selection style (if selectable)
- disabled/error style (if state applies)

## Coordination Notes
- `docs/v1-master-plan.md` remains milestone source of truth.
- This roadmap is the widget scope contract for M3.
- Image-centric controls remain V1.1 scope.
