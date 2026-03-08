# TeaSharp Wizard

`dotnet run --project src/TeaSharp.Cli -- wizard` scaffolds a runnable TeaSharp app with selectable template, theme preset, and input bindings.

## Run

From repo root:

```bash
dotnet run --project src/TeaSharp.Cli -- wizard
```

## Wizard Inputs

- `App name`: used for output folder/project name.
- `Template`: `pomodoro` or `dashboard`.
- `Theme preset`: TeaSharp default, Catppuccin variants, Rosé Pine variants.
- `Command mode key`: single key (example `:`) or named key (`esc`, `enter`, `space`).
- `Toast key`: same validation rules.
- `Modal key`: same validation rules.
- `Output directory`: where files are generated.

## Generated Files

- `<AppName>.csproj`: references `src/TeaSharp` and `src/TeaSharp.Core`.
- `Program.cs`: template app with prewired widgets and bindings.
- `teasharp.json`: scaffold metadata (template/theme/keys/timestamp).

## Quick Validation

```bash
cd <generated-dir>
dotnet run --project <AppName>.csproj
```

Expected: app opens in alt-screen mode and responds to configured hotkeys.
