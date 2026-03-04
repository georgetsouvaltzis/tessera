# TeaSharp Stateful Widgets

TeaSharp now ships a stateful widget layer in `TeaSharp.Widgets` modeled after Bubble Tea's bubbles-style approach.

## Widgets

- `ViewportModel`
  - vertical and horizontal scrolling
  - optional soft-wrap mode
  - key and mouse-wheel updates via `ViewportKeyMap`
- `TextInputModel`
  - cursor movement (char + word)
  - selection basics (`ctrl+a`, shift-extend)
  - delete variants (char/word forward/backward)
  - submit event and placeholder/mask support
- `ListModel<T>`
  - selection and paging
  - filtering (`SetFilter`)
  - key and mouse-wheel updates via `ListKeyMap`

## Keymaps + Help

- `KeyBinding`: normalized chord matcher and help label.
- `ViewportKeyMap`, `TextInputKeyMap`, `ListKeyMap`: default bindings per widget.
- `HelpView.RenderCompact(...)`: deterministic compact/multi-line help rendering with width wrapping.

## Example Integration

`TeaSharp.Examples` workspace page (`2`) now composes:

- actions table backed by `ListModel<ActionItem>`
- scrollable log panel backed by `ViewportModel`
- command input backed by `TextInputModel`
- focus routing (`tab`) and dynamic help (`?`) driven by keymaps
