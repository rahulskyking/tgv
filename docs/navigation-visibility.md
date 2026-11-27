# Category navigation visibility and event row

## Public layout

Desktop (1024px and wider):

- Logo, all visible categories, search, and theme controls remain within the
  main header block. There is no primary/secondary category split anymore.
- Category links start at .8125rem with balanced spacing. When the available
  width cannot hold one line, `navigation.js` selects a .75rem compact size
  and tighter spacing. Text never keeps shrinking to force a single line.
- If needed, links wrap within the same category block. Long/unbroken names
  can also wrap, without overlapping the logo, search, or theme controls.
- A dedicated, softly purple-tinted Events row sits below categories. It shows
  each eligible event's name as a link, with active-page styling. Events are
  not duplicated in the main category links. No empty event row is rendered.

Mobile:

- Categories remain inside the hamburger overlay, not in a scrolling header row.
- The overlay has a separate Events section below the category links.
- Labeled search, open/close controls, active-link states, focus trapping,
  Escape-to-close, focus restoration, and body scroll locking are provided.
- Switching to desktop while the menu is open closes it and releases scroll.

Both themes use existing site tokens. Anchor scroll offset follows the actual
header height (including wrapping and the Events row). The site-mode toggle
remains hidden as before. Public category/event routes are unchanged.

Events continue to use the existing `IsActive && ShowInNavigation` query and
DisplayOrder/Name ordering. Inactive events do not appear in the header, but
their existing direct event URLs still work. No public event directory, archive
browser, or “All events” link was added.

## Admin hide/show

Admin → Categories now shows a navigation visibility status and a per-category
**Hide from navigation / Show in navigation** button. It uses the category
manager's existing authenticated access, with anti-forgery validation on the
new POST action. Each POST submits an explicit target state, so repeated
submission does not inadvertently toggle back.

The flag affects **only** categories passed to the public Navigation component.
It does not change:

- Article publication status, category assignment, or optional event assignment.
- Category/article URLs or access to their pages.
- Homepage, search, category hubs, article editor options, sitemap, or feed.
- Audience flags or display order. When re-shown, the category returns at its
  saved position within its selected audience sections.

The category repository is intentionally not globally filtered by the flag.
Both desktop and mobile consume the same filtered navigation model, using the
existing segment checks and sort order. Category navigation is read on each
render; there is no extra navigation cache to wait for. Existing homepage cache
keys are invalidated after a visibility update as a precaution.

The list also keeps its existing audience selector and saved drag order. Drag
starts only from the handle, so interacting with forms does not start dragging.
Save a changed drag order before submitting other row actions (those reload
the page, as the audience selector already did).

## Migration

`20261024140000_AddCategoryNavigationVisibility` adds the non-null boolean
`categories.show_in_navigation`, default **true**. Existing categories therefore
remain shown, and new categories also default to shown. Down removes only that
column and loses saved visibility preferences.

The migration has not been run in the implementation environment. The Web app's
existing startup `Database.MigrateAsync()` will apply pending migrations when
started against a database. Use the normal backup/staging/deployment procedure.
There are no new npm or NuGet dependencies.

## Verification

CSS compilation, the existing `npm run css:test` builds, JavaScript syntax checks,
C# syntax/source-contract checks, and static browser fixtures were run. Public
navigation fixtures use the real view markup, compiled CSS, JavaScript, and
Inter fonts. Seventy layout scenarios covered 0/6/12/24 categories, long/unbroken
names, 320/375/768/1024/1280/1440/1920px, and light/dark themes. Checks covered
horizontal overflow, category/brand/search overlap, font minimum, event-row
position, IDs/search labels, mobile focus/escape/scroll locking, and resize.

Ten additional static admin layouts covered shown/hidden category states,
explicit true/false form targets, labeled audience selectors, long names/slugs,
44px visibility controls, and mobile/desktop widths in both themes.

These fixtures do not execute Razor or EF. The environment has no .NET SDK or
application database; compilation, HTTP authorization/anti-forgery behavior,
and migration/database behavior still require local or staging verification.

Local/staging checks:

1. Build `TheGameVoice.sln`, review/apply the migration through the normal process,
   and confirm all existing category flags are true.
2. Hide a category. Reload desktop navigation and the mobile hamburger: it must
   be absent in both, while its direct URL, assigned published articles,
   homepage/search results, and article-editor selection remain available.
3. Show it again and confirm saved audience flags and order are respected.
4. Check PC-only, Mobile-only, and both-audience categories; hidden categories
   must not leak into either mode's menu.
5. Submit missing/invalid visibility values and anti-forgery tokens to the action;
   verify no state is changed. Verify unauthenticated access is blocked.
6. Add enough visible categories to trigger compact text and wrapping; check
   long names at desktop widths, zoomed text, and narrow hamburger menus.
7. Check zero/one/multiple eligible events, event order, and active-page styling.
   Toggle active/show-in-navigation in admin and confirm both menus update.
   Archived event direct URLs must remain accessible.
8. Check keyboard focus, Escape, menu scrolling, search, theme control, resizing
   with an open menu, and normal article link navigation in both themes.
