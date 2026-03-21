# M4 Theme Extension Consistency Audit

Date: 2026-03-21  
Scope: `src/TeaSharp/Styles/TeaThemeControlExtensions.*`

## P0

- No P0 blockers found in overload shape.
- `ApplyTheme`/`ApplyThemeDefaults` overload families are present and signature-compatible across current controls (base theme + overrides overload).

## P1

- Public XML docs coverage is incomplete for several extension surfaces.
  - Missing on all methods in:
    - `src/TeaSharp/Styles/TeaThemeControlExtensions.FormsAndShell.cs:7`
    - `src/TeaSharp/Styles/TeaThemeControlExtensions.InputValue.cs:7`
    - `src/TeaSharp/Styles/TeaThemeControlExtensions.ModalAndCharts.cs:7`
    - `src/TeaSharp/Styles/TeaThemeControlExtensions.NavigationOverlay.cs:7`
    - `src/TeaSharp/Styles/TeaThemeControlExtensions.Plotting.cs:7`
    - `src/TeaSharp/Styles/TeaThemeControlExtensions.RenderingTextUtilities.cs:7`
    - `src/TeaSharp/Styles/TeaThemeControlExtensions.Workspace.cs:7`
- Partial coverage inside a single file causes IntelliSense inconsistency by control type.
  - `src/TeaSharp/Styles/TeaThemeControlExtensions.ExplorerAndFeedback.cs:7` (undocumented block) vs `:121` (documented block starts).
  - `src/TeaSharp/Styles/TeaThemeControlExtensions.Navigation.cs:135` (undocumented block starts) vs `:299` (documented block resumes).
- Documentation depth/wording is uneven across domains.
  - Full `<param>/<returns>/<remarks>` style exists in `src/TeaSharp/Styles/TeaThemeControlExtensions.DevOpsAndWorkflows.cs:7`.
  - Summary-only style in `src/TeaSharp/Styles/TeaThemeControlExtensions.Basic.cs:7` and `src/TeaSharp/Styles/TeaThemeControlExtensions.DataAndFlow.cs:7`.
  - No docs in files listed above.

## P2

- Method ordering is not fully uniform.
  - Example: `Button` places defaults before direct apply in `src/TeaSharp/Styles/TeaThemeControlExtensions.Basic.cs:10`, while most controls use `ApplyTheme` first.
- Override overload body style is mixed (`var resolved = ...` vs inline `return control.ApplyTheme(overrides.Resolve(...))`).
  - Compare `src/TeaSharp/Styles/TeaThemeControlExtensions.Basic.cs:36` and `src/TeaSharp/Styles/TeaThemeControlExtensions.ExplorerAndFeedback.cs:32`.
- Domain-level file discoverability can improve (no domain summary comments at top of partial files such as `Workspace`, `Plotting`, `NavigationOverlay`).

## Non-Breaking V1 Recommendations

1. Adopt a single XML-doc template for every public `ApplyTheme*` method:
   - Summary + `control/theme/overrides/baseTheme/state` params + returns.
   - Add default-only `<remarks>`: "Existing non-empty style values are preserved."
2. Normalize summary wording to one glossary:
   - `ApplyTheme`: "Applies resolved theme tokens..."
   - `ApplyThemeDefaults`: "Applies theme tokens to unset style members..."
   - Override overloads: "Resolves overrides and applies..."
3. Normalize method order per control group:
   - `ApplyTheme(base)` -> `ApplyTheme(overrides)` -> `ApplyThemeDefaults(base)` -> `ApplyThemeDefaults(overrides)`.
4. Add a lightweight CI guard (reflection + source scan) to fail when new public `ApplyTheme*` methods miss XML docs or break canonical overload ordering/parameter naming.
