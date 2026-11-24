# Review thumbnail score badges

The shared partial `Views/Shared/Components/_ReviewScoreBadge.cshtml` renders a
non-interactive bottom-right rating overlay using the article's existing editorial
review data. No new rating field, user-rating system, or migration is involved.

## Eligibility

A badge appears only when the article has a loaded featured image, `IsReview`
is true, and `ReviewScore` is between 1 and 10 inclusive (the existing editor's
valid range). Null/missing/out-of-range scores and non-review articles do not
show a badge. Scores use invariant formatting with exactly one decimal place,
for example `8.0`, `8.5`, or `10.0`. Only the number is visible; no `/10` denominator
is shown. No score is invented.

## Coverage and styling

The partial is used in the homepage hero mosaic and Hot Picks, reusable hero,
list, category/event, related/author, and sidebar cards, and the news listing.
Search, tags, and game hubs inherit it through their shared card partials.
The large article-body cover and the existing detailed review-summary panel
are intentionally unchanged: this feature concerns thumbnail overlays.

Thumbnail wrappers use `tgv-review-thumbnail`. Add
`tgv-review-thumbnail--compact` for small horizontal cards or
`tgv-review-thumbnail--hero` for hero images. CSS is maintained in `app.css`
and compiled into `site.css` with `npm run css:build`.

The overlay uses white score text over a soft, transparent purple corner fade
in both themes, without a solid badge box or border. Homepage mosaic captions
reserve room for the score on eligible review cards. It sits above image
gradients and stays stationary when the image zooms. It contains no links or
buttons and does not intercept clicks on the article card. Its accessible
label announces the review score out of 10.

## Verification

The CSS build and browser layout checks passed using the actual badge markup
and compiled CSS in a static fixture: compact, normal, and hero sizes at 320,
375, 768, and 1280px, light/dark themes, scores 1/8/8.5/10. Checks covered badge
containment, score-text overflow, stacking, accessible labels, and non-blocking
pointer events. These are design checks, not execution of Razor eligibility.

With the .NET 8 SDK, run `dotnet build TheGameVoice.sln`, then check the running
site with real data:

- A published review with an image and score shows the same numeric rating on
  every applicable thumbnail, including the event hub and search results.
- A news article with a stale score does not show a badge (`IsReview` is false).
- A review with no score, no featured image, or an invalid score shows no badge.
- Scores 1, 8.5, and 10 fit the smallest cards in both themes.
- Card clicks still open the article; hover zoom does not move the badge.
- The existing article review panel and original uploaded image are unchanged.

The implementation environment has no .NET SDK, so compilation and rendered
Razor behavior remain to be verified locally. No test project was introduced.
