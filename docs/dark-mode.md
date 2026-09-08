# Dark / Light Mode

Crisp, additive dark theme. **The existing light design is untouched** — not a
single colour class was changed in the views, and `site.css` (the Tailwind
build output) was not regenerated.

## How it works

| Layer | File | Job |
|---|---|---|
| State | `<html data-theme="light\|dark">` | Single source of truth |
| No-flash boot | inline `<script>` in `_Layout.cshtml` + `_AdminLayout.cshtml` | Sets `data-theme` **before first paint** from `localStorage` → OS preference |
| Styles | `wwwroot/css/theme-dark.css` | Everything dark, scoped under `html[data-theme="dark"]` |
| Behaviour | `wwwroot/js/theme.js` | Delegated click on `[data-theme-toggle]`, flips the attribute, saves to `localStorage["tgv-theme"]` |
| UI | `Views/Shared/Components/_ThemeToggle.cshtml` | Sun/moon button (public header ×3 spots, admin sidebar) |

`theme-dark.css` has two parts:

1. **Token overrides** — re-points the existing `--tgv-*` variables
   (`--tgv-primary`, `--tgv-background`, `--tgv-surface`, `--tgv-text`,
   `--tgv-border`, `--tgv-shadow`, …) at a dark palette. Everything already
   written against tokens (`theme.css`, `theme-utilities.css`,
   `.article-content`, `.tgv-*`) flips for free.
2. **Bridge layer** — the views also use hardcoded Tailwind colours
   (`bg-white` ×129, `text-slate-900` ×91, `border-slate-200` ×80 …). Rather
   than adding `dark:` variants to ~40 `.cshtml` files, those utilities are
   re-mapped inside the dark scope only. Specificity `0-2-0`
   (`html[data-theme="dark"] .bg-white`) always beats Tailwind's `0-1-0`, so
   load order doesn't matter — the dark sheet is still linked last.

Classes that are **already dark by design** are deliberately left alone:
`bg-black`, `bg-slate-900`, `bg-gray-900/800`, `text-white`,
`text-slate-300/200`, `border-gray-700/800`, `from-black` image overlays.
`text-black` is also untouched — it is always paired with `bg-primary`
buttons.

## Notes

* **No `npm run css:build` needed.** `theme-dark.css` is plain hand-written
  CSS, not Tailwind-generated. Tailwind's `darkMode` option is intentionally
  *not* enabled (it would force a rebuild + mass markup churn).
* **No server state.** Unlike `tgv_mode` (PC/Mobile), the theme is pure
  client-side — no cookie, no `Vary`, no cache-key impact.
* OS preference is followed only until the reader clicks the toggle once.
* Admin bonus fix: `_AdminLayout.cshtml` was never linking `theme.css` /
  `theme-utilities.css`, so every admin `bg-primary` button was rendering
  with **no background**. Both links are now added.

## Manual test checklist

1. Load the public site → click the moon in the header → page goes dark with
   a 220 ms fade; icon becomes a sun.
2. Hard-refresh → still dark, **no white flash** at any point.
3. Open a new tab / another page (article, category, search) → dark persists.
4. Mobile width: toggle appears in the mobile header and inside the menu
   overlay; the overlay itself is dark.
5. PC ⇄ Mobile section switch still works and stays in the chosen theme.
6. `/Admin` → sidebar has a "Dark mode / Light mode" row; tables, cards,
   forms, TomSelect inputs and badges are all readable.
7. Article body: headings, blockquote, tables, `hr` and code all follow the
   dark tokens; images are dimmed to 92 % brightness.
8. Switch the OS to dark with `localStorage` cleared → site opens dark.
9. Set light explicitly → OS change no longer overrides it.
