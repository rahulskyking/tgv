# Design system, dark mode and responsive layout

Everything visual now comes from **Tailwind** — one config, one source
stylesheet, one compiled output. There are no hand-written stylesheets loaded
next to the build any more.

```
tailwind.config.js                      ← tokens, screens, fluid scales, dark variant
src/.../wwwroot/css/app.css             ← the only CSS source you edit
        ↓  npm run css:build   (or npm run css to watch)
src/.../wwwroot/css/site.css            ← the only stylesheet the layouts link
```

Deleted (folded into the pipeline): `theme.css`, `theme-utilities.css`,
`theme-responsive.css`, `theme-dark.css`, and the stray `<link>` to the raw
`app.css` source.

## 1. Colour tokens

Colours live in CSS variables as `R G B` channel triplets and are exposed to
Tailwind through `theme.extend.colors`, so **opacity modifiers work**
(`bg-primary/10`, `text-ink/70`) and **dark mode is a variable swap**.

| Utility | Token | Light | Dark |
|---|---|---|---|
| `bg-primary`, `text-primary`, `border-primary` | `--tgv-primary` | `#7c3aed` | `#8b5cf6` |
| `bg-canvas` | `--tgv-background` | `#f8fafc` | `#0d1117` |
| `bg-surface` | `--tgv-surface` | `#ffffff` | `#161b22` |
| `bg-surface-muted` | `--tgv-surface-2` | `#f8fafc` | `#1c2330` |
| `bg-surface-raised` | `--tgv-surface-3` | `#f1f5f9` | `#232c3b` |
| `text-ink` | `--tgv-text` | `#0f172a` | `#e8eef7` |
| `text-ink-soft` / `-mid` / `-muted` / `-faint` | slate 700 / 600 / 500 / 400 | exact | lightened |
| `border-line`, `border-default` | `--tgv-border` | `#e2e8f0` | `#232b36` |
| `border-line-subtle` / `-strong` | slate 100 / 300 | exact | dark equivalents |

The views were migrated to these semantic utilities — **777 replacements**
(`bg-white` → `bg-surface`, `text-slate-900` → `text-ink`,
`border-slate-200` → `border-line`, …). Every light value is byte-identical to
what was there before, so **the light theme did not change**, and dark mode
now needs **no override layer at all**.

Left as literal palette colours on purpose: `bg-black`, `bg-slate-900`,
`bg-gray-800/900`, `text-white`, `text-slate-300`, `from-black` image
overlays, and `text-black` (always on a `bg-primary` button).

Status chips (`bg-green-100 text-green-800` …) are re-mixed for dark mode at
the bottom of `app.css` using `color-mix()` + `theme()` for 14 hues.

## 2. Dark mode

* Mechanism: `data-theme="dark"` on `<html>`; Tailwind is configured with
  `darkMode: ['selector', '[data-theme="dark"]']`, so a `dark:` variant is
  available if a one-off ever needs it.
* An inline script in both layout `<head>`s applies the saved/OS theme
  **before first paint** — no flash.
* `wwwroot/js/theme.js` toggles it and persists `localStorage["tgv-theme"]`.
* Toggle UI: `Views/Shared/Components/_ThemeToggle.cshtml`, rendered in the
  desktop header, mobile header, mobile menu and admin sidebar.
* No cookie, no server state, no cache-key impact.

## 3. Responsive

### Fluid scales (config), not breakpoint stacks

| Utility | Range | Replaces |
|---|---|---|
| `text-fluid-3xl … -7xl` | shrinks on phones, **same maximum** as `text-3xl … text-7xl` | 49 unprefixed oversized headings |
| `text-fluid-display / -h1 / -h2 / -h3 / -lead / -body` | editorial scale | ad-hoc sizes |
| `p/py/mt/gap-fluid-8 … -28` | e.g. `py-fluid-24` = 40px → 96px | 40 unprefixed desktop paddings |
| `px-gutter` | 16px → 24px | fixed container padding |
| `py-section`, `py-section-sm` | fluid section rhythm | — |

Screens gained `xs: 400px` (the size where the mobile header used to collide —
now `max-xs:hidden`) and `3xl: 1920px`.

### Structural fixes

| Problem | Fix |
|---|---|
| Admin had **no `viewport` meta tag** — phones rendered it at 980px, zoomed out | added, with `viewport-fit=cover` |
| Admin sidebar hard `w-64` in a flex row | off-canvas drawer below `lg` (`admin-nav.js`: burger, backdrop, Esc, link-tap, resize) |
| Admin tables (Articles, Games, Tags, Users) with no scroll container | `.tgv-admin-main table` scrolls itself below `xl`; `.tgv-wrap` opts a cell back into wrapping |
| `flex-1` main without `min-width:0` — a wide table blew the layout out | `.tgv-admin-main { min-width: 0 }` |
| Container ignored notch / had flat padding | `max(px-gutter, env(safe-area-inset-*))` |
| Article prose fixed at 1.15rem / line-height 2 | fluid size, 1.75 line-height under `md`, fluid heading margins |
| Wide prose tables, pasted 560×315 iframes, long slugs | scrollable tables, `aspect-ratio: 16/9` iframes, `overflow-wrap`, `body { overflow-x: clip }` |
| Mobile menu (`fixed inset-0`) unscrollable | `overflow-y:auto` + `overscroll-behavior: contain` |
| Sticky header ate a landscape phone | header goes `static` under 480px height |
| iOS zoomed on input focus | 16px form text + 44px targets below `lg` |
| 1440px cap on a 4K panel | container grows to 1600 / 1760px |

Plus `prefers-reduced-motion` support.

## 4. Bugs found and fixed along the way

* `_AdminLayout` never linked the theme stylesheets → every admin
  `bg-primary` button had **no background**.
* `.tgv-button-primary` (5 admin buttons) was **never defined anywhere** —
  now a real component class.
* `<body class="bg-slate-50 color: var(--tgv-text);">` and
  `class="tgv-display text-4xl color: var(--tgv-text); lg:text-6xl"` — CSS
  declarations pasted inside `class` attributes.
* `text-danger` (Bootstrap leftover) on the error page resolved to nothing.
* Tailwind purge also dropped 7 design-system classes that no view uses
  (`tgv-image-16x9`, `tgv-sidebar`, `tgv-body`, `tgv-meta`, `tgv-muted`,
  `tgv-nav-link`, `article-hero-title`) — they stay in the source and will
  reappear the moment they are used.

## 5. Workflow

```bash
npm install          # once
npm run css          # watch while developing
npm run css:build    # minified build — commit the resulting site.css
```

**`site.css` is a build artifact: never edit it by hand.** After changing any
`.cshtml`, `.js` or `app.css`, rerun the build so new utility classes exist.

### Verification done

A script extracts every `class="…"` token from all 60 views (710 distinct
classes) and checks each one against the compiled CSS. The only unresolved
names are JS hooks (`media-item`, `remove-row`, …), which are selectors, not
styles.

There is also a static harness (not part of the app):

```bash
python3 -m http.server 8080 --bind 0.0.0.0 --directory /home/user/preview
```

It renders the compiled stylesheet at 320 / 360 / 390 / 430 / 768 / 1024 /
1440 / 1920px side by side, with public/admin and light/dark switches.

## 6. Manual test checklist

1. Home at 360px — no horizontal scrollbar, hero headline fits.
2. Article at 360px — comfortable body text, wide table scrolls in its box.
3. Toggle dark — instant, persists across pages, no flash on reload.
4. `/Admin` on a phone — burger, drawer, backdrop, Esc all work.
5. `/Admin/Articles` at 768px — the table scrolls, the page does not.
6. Admin "New article" / Games / Tags buttons now actually look like buttons.
7. Focus a form field on iOS — no zoom.
8. Desktop 1440px, light mode — identical to before this change.
