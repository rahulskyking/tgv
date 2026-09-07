# Mobile vs PC / Console sections (`GameSegment`)

One editorial module, two audience sections. An article is tagged with the
audience it is written for and the public site renders one audience at a
time; readers flip between them with the toggle under the logo.

## Concept

`GameSegment` is a `[Flags]` enum on `Article`, `Category` and `Game`:

| Value | Meaning |
|-------|---------|
| `1` `PcConsole` | PC / console gaming content |
| `2` `Mobile` | Mobile gaming content |
| `3` `All` | Both sections |

It is **not** a publishing channel (web vs app) and it is **not**
`Game.Platforms` (the free text "PS5, Xbox, PC" field).

## Admin

* **Articles → Create / Edit** — a *Section* card in the sidebar with two
  checkboxes: *PC / Console games* and *Mobile games*. At least one is
  required (validated in `ArticleFormViewModel.Validate`). Ticking both
  publishes the article into both sections.
* **Articles → list** — a *Section* column with badges and a *Section*
  filter in the filter bar (`ArticleFilter.Segment`), preserved across
  pagination.
* **Categories → list** — a dropdown per category (PC / Console, Mobile,
  Both) posting to `CategoriesController.SetSegment`. Categories drive the
  header navigation, so this is what makes the navigation flip.
* **Games → Create / Edit** — the same two checkboxes; drives the trending
  games widget.

Nothing was duplicated: the same controller, the same views and the same
repository serve both sections.

## Public site

* Mode is resolved per request by `ISiteSegmentAccessor`
  (`src/Web/TheGameVoice.Web/Services/ISiteSegmentAccessor.cs`):
  `?mode=` query → `tgv_mode` cookie → `PcConsole` default.
* `SiteModeController` (`/switch-mode/{mode}?returnUrl=`) writes the cookie
  and returns the reader to the page they were on.
* The toggle lives in `Views/Shared/Components/_SiteModeToggle.cshtml` and is
  rendered under the logo (desktop), in the mobile header and in the mobile
  overlay menu.
* Segment-filtered: homepage, news index, category, tag, author, search,
  related articles, trending articles, trending games and the header
  navigation.
* **Not** filtered on purpose: `/sitemap.xml` and `/feed` (they should list
  everything), and article detail pages — a shared link always opens, and if
  the article belongs to the other section the site flips mode and shows a
  notice instead of a 404.
* Cache keys are scoped per mode (`CacheKeys.ForSegment`) and admin actions
  invalidate every variant (`CacheKeys.AllHomePageKeys()`). Public responses
  send `Vary: Cookie`.

## Database

Migration `20260907120000_AddGameSegment`:

```
articles.segment    integer not null default 1   -- existing content = PC / Console
games.segment       integer not null default 1
categories.segment  integer not null default 3   -- stay visible in both sections
+ ix_articles_segment_status_published_at
+ ix_games_segment
```

The migration and its designer/snapshot were written by hand (no .NET SDK in
the authoring environment). Before deploying:

```bash
dotnet build
dotnet ef migrations list -p src/Infrastructure/TheGameVoice.Infrastructure -s src/Web/TheGameVoice.Web
dotnet ef database update -p src/Infrastructure/TheGameVoice.Infrastructure -s src/Web/TheGameVoice.Web
```

If `dotnet ef migrations add Verify` produces a non-empty migration, the
hand-written snapshot drifted from the model — inspect it and delete it.

## Manual test checklist

1. Admin → create an article, tick **Mobile games** only, publish it.
2. Public site in PC / Console mode: the article must not appear anywhere.
3. Click **Mobile** in the header: homepage, navigation and listings switch.
4. Open the mobile article's URL while in PC mode → it opens with the
   "switched section" notice.
5. Untick both checkboxes on the article form → validation error.

## Possible next steps

* `/mobile/...` URL prefix instead of a cookie, for per-section SEO.
* Per-section RSS feeds and sitemaps.
* Section split on the admin dashboard KPIs.
