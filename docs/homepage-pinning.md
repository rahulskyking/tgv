# Homepage article pinning

## Using the editor

As an Editor, Admin, or SuperAdmin, open **Admin → Articles → Create/Edit →
Details → Homepage placement**:

1. Check **Pin to homepage**.
2. Set **Homepage display order** (1–9999). Lower numbers appear first.
3. Save the article. A new article is still a draft until published through
   the existing editorial workflow.

Multiple articles may be pinned. The first eligible story becomes the main
hero; the next eight fill the smaller hero-grid positions. Ties use newest
publication, then creation time, then ID for deterministic ordering. Remaining
positions are filled newest-first with unpinned articles, without duplicates
inside the hero grid. If more than nine eligible articles are pinned, only the
first nine fit. With no pins the hero grid uses the newest published articles.

Uncheck the pin and save to restore normal date-based placement. The Articles
list labels pinned stories with their priority and indicates pins that are
inactive until published. You can prepare pins on drafts or scheduled stories;
they do not appear publicly until the existing workflow publishes them.

Pins respect the article's audience flags. A Mobile-only story never appears
on the PC/Console homepage; a story assigned to both uses the same pin priority
in each homepage. Archived, rejected, draft, scheduled, and soft-deleted
articles are excluded by the existing published-article query. Pinning never
publishes an article or changes its category, event, URL, or publication date.

The Latest Articles feed remains newest-first, even when an older story leads
the hero. Category/event/search/tag/game/author pages, review lists, trending,
and RSS are unchanged. Pins have no automatic expiration.

## Dedicated Homepage Pins tab

Open **Admin → Homepage Pins** in the sidebar, or use the Homepage Pins link
on the Articles page. Editor/Admin/SuperAdmin users see every saved pin in
priority order, including unpublished and archived pins. Soft-deleted articles
remain excluded. This screen reads the database directly, not the homepage cache.

Each card shows:

- Title, saved order, editorial status, and audience.
- Its independent PC/Console and Mobile placement: Main hero, Grid position
  2–9, Outside top 9, Not assigned, or Not live (unpublished).
- An inline numeric order field and Save order button.
- Edit article and Unpin actions.

Saving changes only the selected article's order, not its editorial content,
publication status, or other pins. Unpinning leaves the article intact and
eligible for ordinary newest-first placement. Both operations clear homepage
cache keys after saving. All writes require authorized roles and anti-forgery
tokens. The submitted prior order is checked against the current saved pin so
an already changed/unpinned item prompts a refresh rather than silently re-pinning.
This is a stale-page safeguard, not a database-wide concurrency lock.

No additional migration is needed for this screen beyond AddHomepagePins.

## Authorization and caching

Pin controls are shown only to Editor/Admin/SuperAdmin users. Create and Edit
also enforce these roles server-side: forged author submissions cannot set pins
on creation or overwrite saved pin settings during edits. Existing article
ownership checks and anti-forgery validation are retained.

Existing save, publish, archive, delete, and bulk-action paths invalidate the
homepage caches for both audience modes. The existing five-minute homepage
cache remains in place. Cache invalidation is local to the application process,
as it was before this feature; multiple running instances may briefly disagree
until their caches expire.

## Database and deployment

Migration: `20261024130000_AddHomepagePins` (after the existing Events migration).

It adds two non-null columns to `articles`:

- `is_homepage_pinned` (boolean, default false)
- `homepage_pin_order` (integer, default 1)

Existing articles remain unpinned. There is no data rewrite, category change,
or additional table. The migration has a Down operation that removes only
these two columns (and therefore loses pin settings).

**The Web application's existing startup calls `Database.MigrateAsync()`.**
Starting this version against a database will apply pending migrations. Use
your normal backup/review/deployment procedure; migration execution was not
performed in the implementation environment.

## Verification

The implementation environment does not have the .NET SDK or a test database.
CSS compilation, C# syntax parsing, source-contract checks, and static admin
layout checks were run; they are not a substitute for Razor compilation or EF
integration testing. No test project was added.

On a local/staging database:

1. Build `TheGameVoice.sln`; verify/apply pending migrations through your normal
   workflow. Confirm existing articles have false/1 defaults.
2. With no pins, check the existing nine-story mosaic and newest-first feed.
3. Pin an older published article at priority 1. Verify it leads the hero even
   if it is older than the newest nine. The Latest Articles feed must not move.
4. Pin two more stories at priorities 2 and 3. Verify they fill the next two
   positions. Tie their priorities and verify newest publication wins.
5. Test zero, one, fewer than nine, and more than nine eligible stories; no
   duplicate cards should occur inside the mosaic.
6. Unpin the hero, save, and verify automatic fallback without waiting for cache
   expiry in a single instance. Publish/archive/delete pinned stories and check
   the same fallback behavior.
7. Check PC-only, Mobile-only, and both-audience pins in both site modes. Draft,
   scheduled, archived, rejected, and soft-deleted pins must remain hidden.
8. As an author, verify the pin controls are absent. Submit forged pin fields
   while creating/editing an owned article and confirm they cannot change pins.
9. Check order validation at 0, 1, 9999, 10000, and non-numeric input; errors
   should retain the rest of the article form and not save invalid settings.
10. Check the Details panel at 320px and desktop widths in both themes, and
    confirm review-score overlays and the homepage brand schema are unchanged.

### Dedicated manager checks

- As an editor/admin, open Homepage Pins from both sidebar and Articles link;
  confirm the active navigation state, empty state, and all saved pins.
- Verify PC/Mobile placement labels against each homepage with mixed audiences,
  unpublished pins, equal priorities, missing dates, and more than nine live pins.
- Change one priority and save. Confirm list/placements resort, the change
  persists on reload, and no article content or status changes.
- Unpin a story and confirm it disappears from this list but is not deleted.
- Open two tabs, change/unpin an item in one, then submit the old order in the
  other. Confirm a stale-page message appears and the newer settings remain.
- Verify invalid/missing orders cannot save, authors cannot GET or POST to the
  controller, and POST without an anti-forgery token is rejected.
- Check the manager at 320px and desktop widths in both themes.
