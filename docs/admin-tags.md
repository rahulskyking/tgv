# Admin tag library UX

The public tag template and `/tag/{slug}` route are unchanged. No migration or
new dependency is required. Existing authenticated admin access is preserved;
this change does not introduce new role permissions.

## Library

Admin → Tags now offers:

- All / In use / Unused summary links. Counts cover the whole tag library;
  clicking a summary retains the current search/sort and resets pagination.
- Case-insensitive literal search over tag names and slugs (up to 100 characters).
- A–Z, Z–A, newest-first, and most-used sorting with stable ID tie-breakers.
- Database-side pagination, 20 tags per page; page numbers are clamped after
  filtering or deletion so an out-of-range URL does not produce a blank page.
- Assignment counts, creation dates, and Edit / View actions.
- Distinct initial-empty and no-matches states with recovery links.
- Mobile-stacked rows, labelled controls, 44px action targets, and theme tokens.

Assignment counts include all `article_tags` links, including those attached to
drafts, archived articles, or soft-deleted articles. This matches the existing
server-side deletion guard rather than promising a tag is safe to delete based
only on published articles. There is no per-row database request for counts.

Delete is disabled for in-use tags. Unused tags retain a confirmation step with
correct wording: deletion does not detach articles. The POST action independently
rechecks usage. Search, usage, sort, and page are retained after deletion, with
pagination clamped if the last item on a page disappears. As before, the usage
check is not an atomic lock against simultaneous assignment by another editor.

All browsing controls use GET forms and links; search, filtering, sorting, and
pagination do not require JavaScript. The deletion confirmation uses the site's
existing shared confirmation UI.

## Create and edit

Create and edit forms include labels, inline name errors, client/server validation,
help text, Cancel, and a link back to the library. Names are trimmed, must contain
1–100 characters, and are checked case-insensitively for existing live tag names.
The name check is an application validation rule, not a new database unique index.

Create generates a slug using the existing slug service and additionally checks
**tag** slug collisions (including soft-deleted tags). Empty generated slugs get a
`tag` fallback; collisions get numeric suffixes. A database slug-uniqueness race
returns validation feedback instead of an unhandled unique-index exception.
The article editor's QuickCreate JSON endpoint reuses these checks and preserves
its existing response shape and authorization behavior.

Edit changes only the tag name. It never updates the stored slug, article
assignments, article content, or categories. Name changes invalidate the existing
public tag-page, affected article-detail, and homepage cache keys for both site
modes. Cache invalidation remains process-local under the site's existing cache
architecture; other instances may serve old content until expiry.

Create, edit, and delete form POSTs validate anti-forgery tokens. The existing
QuickCreate JSON client's anti-forgery behavior is unchanged by this UX update.

## Verification

The implementation environment has no .NET SDK or database connection. C# syntax
parsing, source-contract checks, `npm run css:build`, and `git diff --check` were
run. Static browser fixtures using the actual view markup and compiled stylesheet
were checked at 320, 375, 768, 1024, and 1440px in light/dark mode, including long
names and 150-character slugs, 20 rows, counts, label associations, required name
input, conditional delete actions, and 44px touch targets. These do not execute
Razor, EF queries, authentication, or real CRUD.

Local/staging checklist:

1. Build `TheGameVoice.sln` and sign into the admin panel.
2. Check the initial empty state, a one-tag library, and more than 20 tags.
3. Search by name/slug with mixed case and punctuation; verify no-match reset.
4. Combine search, usage, and every sort option. Follow Next/Previous; filters
   must persist and sorting must remain stable across pages.
5. Request negative/huge page numbers, delete the last tag on a page, and verify
   the library displays the nearest valid page rather than a blank result.
6. Verify all/in-use/unused counts and assignment totals with draft, published,
   archived, and soft-deleted articles. In-use deletion must be unavailable in
   the UI and rejected through a direct forged POST too.
7. Create tags with whitespace-only, duplicate/mixed-case, 100-character, and
   overlong names. Test two distinct names that generate the same slug, a
   non-Latin name that generates an empty base, and a soft-deleted slug collision.
8. Rename a tag, including one in use. Check its public URL and assignments
   stay intact and its new name appears in relevant cached pages.
9. Confirm the article editor's quick-add tag modal still works and handles
   duplicate/invalid names without saving bad tags.
10. Submit create/edit/delete without anti-forgery tokens and verify rejection.
    Confirm unauthenticated users cannot access the admin controller.
11. Check empty/error/success states and keyboard/mobile use in both themes.
