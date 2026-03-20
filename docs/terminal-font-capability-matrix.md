# Terminal Font Capability Matrix (TeaSharp V1)

This matrix covers whether a TUI app can change terminal font family/size through output sequences.
Scope is runtime behavior from app output, not manual user settings.

## Matrix

| Terminal | Family from TUI output | Size from TUI output | Mechanism | Notes |
| --- | --- | --- | --- | --- |
| xterm | Partial | Partial | `OSC 50` ("Set Font to Pt"), subject to `allowFontOps` | Legacy/xterm-specific; may be disabled by terminal policy. |
| iTerm2 | Indirect | Indirect | `OSC 1337;SetProfile=...` (switch profile) | Font changes come from selected profile settings; iTerm2 docs note legacy `OSC 50` moved to `1337`. |
| Kitty | Not first-class | Yes | kitty remote control `kitten @ set-font-size` | Kitty-specific control path; requires kitty protocol/remote-control usage. |
| WezTerm | No (via generic VT output) | No (via generic VT output) | Config/Lua (`font`, `font_size`, `window:set_config_overrides`) | Use config/runtime API, not portable OSC font control. |
| Ghostty | No | No (via generic VT output) | Config (`font-family`, `font-size`) and local keybind actions | VT reference does not define OSC font-family/font-size control for app output. |
| Windows Terminal | No (VT) | No (VT) | `settings.json` profile (`font.face`, `font.size`) | Profile/config driven; not exposed as VT font sequence. |
| Windows Console (conhost) | No (VT) | No (VT) | Host settings / Win32 APIs | Console VT sequence docs do not define font-family/font-size OSC control. |

## TeaSharp Recommendations

- Keep `ScreenOptions.FontSpec` explicitly best-effort and optional.
- Do not treat font requests as cross-terminal contract; treat them as advisory.
- Emit font request only on change/startup, never per frame.
- Keep sanitization and no forced restore behavior (current TeaSharp policy).
- For predictable typography, recommend profile/config setup in app docs instead of runtime font mutation.

## Official Sources

- xterm control sequences: https://invisible-island.net/xterm/ctlseqs/ctlseqs.html
- iTerm2 proprietary escape codes: https://iterm2.com/documentation-escape-codes.html
- Kitty remote control (`set-font-size`): https://sw.kovidgoyal.net/kitty/remote-control/
- Kitty font config (`font_family`, `font_size`): https://sw.kovidgoyal.net/kitty/conf/
- WezTerm escape sequences: https://wezterm.org/escape-sequences.html
- WezTerm `font_size`: https://wezterm.org/config/lua/config/font_size.html
- WezTerm runtime overrides: https://wezterm.org/config/lua/window/set_config_overrides.html
- Ghostty VT reference: https://ghostty.org/docs/vt/reference
- Ghostty config reference: https://ghostty.org/docs/config/reference
- Ghostty keybind actions: https://ghostty.org/docs/config/keybind/reference
- Windows Console VT sequences: https://learn.microsoft.com/windows/console/console-virtual-terminal-sequences
- Windows Terminal profile appearance (`font.face`, `font.size`): https://learn.microsoft.com/windows/terminal/customize-settings/profile-appearance
