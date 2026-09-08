# Responsive layer

`wwwroot/css/theme-responsive.css` makes every page work from 320px phones to
4K displays. It is **plain CSS, additive, and desktop-safe** — the ≥1024px
rendering is unchanged.

## Why a CSS layer and not `dark:`-style utility edits

Tailwind cannot be rebuilt in this environment (`site.css` is a committed
build artifact and there is no npm install), so **no new Tailwind class can be
added to markup** — an unknown class would simply do nothing. Everything is
therefore done in hand-written CSS that overrides the compiled utilities.

Load order: `site.css → theme.css → theme-utilities.css → **theme-responsive.css** → theme-dark.css`

## What it fixes

### Public site

| Problem found | Fix |
|---|---|
| `.tgv-container` had a flat 24px padding, ignored notches | `padding: max(clamp(16px, 4vw, 24px), env(safe-area-inset-*))` — identical from ~600px up |
| `.tgv-heading` jumped 2rem → 3rem at exactly 1024px | `clamp(1.6rem, 1.05rem + 2.4vw, 3rem)`, same 3rem on desktop |
| `.article-content` fixed 1.15rem / line-height 2 | Fluid font size, line-height 1.75 and tighter paragraph rhythm below 768px |
| `h2` 2.25rem with a 4rem top margin on a 360px screen | Fluid size + fluid margins |
| 48× `text-3xl`, 8× `text-4xl`, 3× `text-6xl`, 2× `lg:text-7xl` with **no** responsive prefix | Base utilities scaled down under 640px and again under 380px. `sm:`/`md:`/`lg:` variants only apply from 640px up, so desktop is untouched |
| Desktop rhythm (`py-24`, `p-16`, `gap-16`, `mt-24`) applied on phones | Roughly halved below 640px |
| Wide article tables / pasted 560×315 iframes / long slugs pushed the page sideways | `overflow-x:auto` tables, `aspect-ratio:16/9` iframes, `overflow-wrap`, `body { overflow-x: clip }` (clip, not hidden, so the sticky header still sticks) |
| Mobile menu is `fixed inset-0` — content below the fold was unreachable | `overflow-y:auto` + `overscroll-behavior:contain` |
| Logo + section toggle + theme + burger collided under 400px | The section toggle is wrapped in `.tgv-hide-xs` (it is still in the menu) |
| Sticky header ate a landscape phone's viewport | Header goes static under 480px height in landscape |
| iOS zoomed in whenever a form field was focused | 16px form text and 44px minimum targets below 1024px |

### Admin panel

The admin had **zero** responsive handling.

| Problem | Fix |
|---|---|
| **No `viewport` meta tag at all** → phones rendered it at a 980px virtual width, zoomed out | Added (with `viewport-fit=cover`) |
| Hard `w-64` sidebar in a flex row → ~100px content column on a phone | Off-canvas drawer below 1024px: `.tgv-admin-topbar` burger, `.tgv-admin-backdrop`, `body.tgv-admin-open`; driven by `wwwroot/js/admin-nav.js` (Esc, backdrop tap, link tap and resize all close it) |
| 4 index tables (Articles, Games, Tags, Users) had no scroll container | `.tgv-admin-main table` scrolls itself below 1280px; add `.tgv-wrap` to a cell that should wrap |
| `flex-1` main without `min-width:0` — a wide table blew the layout out | `.tgv-admin-main { min-width: 0 }` + fluid padding |
| `grid-cols-2` stat tiles unreadable at 360px | Single column below 480px (thumbnail pickers keep 2 columns) |
| `min-w-[300px]` filter forced sideways scroll | Released below 768px |

Plus `prefers-reduced-motion` support and a wider container (1600 / 1760px) on
1920px+ displays.

## Checking it without running the app

There is a static harness (not part of the app, not committed):

```
python3 -m http.server 8080 --bind 0.0.0.0 --directory /home/user/preview
```

It renders the real stylesheets at 320 / 360 / 390 / 430 / 768 / 1024 / 1440 /
1920px side by side, with public/admin and light/dark switches.

## Manual test checklist

1. Home page at 360px — no horizontal scrollbar anywhere, hero headline fits.
2. Article page at 360px — body text comfortable, wide table scrolls inside
   its own box, YouTube embed is 16:9 full width.
3. Rotate to landscape on a short phone — header stops sticking, page scrolls.
4. iPhone with a notch, landscape — content clears the notch.
5. `/Admin` on a phone — top bar with a burger, drawer slides in, backdrop
   dims, tapping a link closes it; Escape closes it.
6. `/Admin/Articles` at 768px — table scrolls horizontally, the page does not.
7. Focus a form field on iOS — the page must not zoom.
8. 1920px+ — the container is wider (1600px) but the layout is unchanged.
9. Desktop at 1440px — everything pixel-identical to before this change.
