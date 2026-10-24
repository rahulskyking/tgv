# Event hubs

Events are independent of primary categories. Each article keeps its existing
category and URL and may optionally reference one event. No existing article
is assigned automatically. Event coverage includes both PC/console and mobile
articles, but only articles with Published status appear publicly.

## Admin workflow

1. Sign in as Admin or SuperAdmin and open **Events** in the admin sidebar.
2. Create an event (for example IGDC 2026). The slug is suggested from the name
   and can be adjusted before saving; it becomes immutable after creation.
3. Add a description, optional dates, location (including Online), and official
   HTTP/HTTPS website. End dates require a start date and cannot precede it.
4. Upload the banner under **Media** if necessary, then select it from the
   event form's existing-media selector. Only images are accepted. Provide
   alternative text. A preview is shown; no banner uses a simple default header.
5. Enable **Active event** and **Show event name in navigation** to promote it.
   Navigation order sorts ascending. The first two promoted events appear by
   name in the main desktop row; overflow appears by name in the secondary row.
   All promoted events appear in mobile navigation.
6. Create/edit an article and choose its optional event in the Details tab.
   Its regular category remains unchanged. Publish through the existing workflow.

## Public pages

- `/events`: active events, with a link to the archive.
- `/events?archived=true`: archived events.
- `/events/{slug}`: banner, details, and paginated published coverage (12/page).
- Assigned article detail pages link back to the event hub.
- The sitemap includes event hubs, including archives.

Event pages, event selection, and event navigation query current data rather
than storing an additional event cache. Event edits/archive/reactivation also
invalidate assigned article-detail caches in both site modes. Editing an
article invalidates its detail caches so changed event assignments appear.

## Archiving

Archiving is NOT deletion, unpublishing, or privacy control. An archived event:

- Disappears from promoted navigation, even if Show in navigation stays checked.
- Moves to the public archive and keeps its original landing-page URL.
- Keeps all images/details and article associations.
- Does not affect article categories, publication status, or article URLs.
- Can be reactivated, restoring its saved navigation preference.

Article editors can retain an existing archived assignment or remove it. New
assignments to archived events are rejected server-side. Reactivate an event
before assigning new coverage. Dates do not automatically archive events.
There is deliberately no event-delete endpoint.

## Database and deployment

Migration: `20261024120000_AddEvents`, with a matching target model and snapshot.
It adds `events` and a nullable `articles.event_id`, plus indexes and foreign
keys. The category schema is not changed. Hard deletion of an event referenced
by articles is restricted; banner deletion sets only its media reference null.

Before deployment, back up the database and test the migration on staging.
The application already calls `Database.MigrateAsync()` at startup, so deploying
this feature will apply pending migrations using its configured database user.
No migration was run against a live database during implementation.

With the .NET 8 SDK installed:

```sh
dotnet build TheGameVoice.sln
dotnet ef migrations has-pending-model-changes \
  --project src/Infrastructure/TheGameVoice.Infrastructure \
  --startup-project src/Web/TheGameVoice.Web
```

If migration tooling is installed, you can apply explicitly on a staging DB:

```sh
dotnet ef database update \
  --project src/Infrastructure/TheGameVoice.Infrastructure \
  --startup-project src/Web/TheGameVoice.Web
```

Compile CSS after editing Razor views:

```sh
npm ci
npm run css:build
```

No test project was reintroduced. The implementation environment has no .NET
SDK, so C# build, Razor compilation, and database execution require verification
in an SDK-enabled environment before production deployment.

## Acceptance checks

- Create an event with image, dates, location, website, and nav enabled.
- Check its name links to its landing page in desktop AND mobile navigation.
- Active + nav disabled hides the link but leaves the hub accessible.
- Create a News article assigned to the event; publish it. It appears in News
  and event coverage with the same `/article/{slug}` URL.
- Draft/rejected/scheduled/archived articles do not appear in event coverage.
- Editing an article to another event or None updates both event listings and
  its detail-page event link after save (including a previously cached page).
- Archive the event. Navigation disappears, but the original hub/article URLs
  work and the hub appears in the archive. Existing assignments remain intact.
- Edit an article with an archived assignment without changing it; save succeeds.
  Forging a new archived/missing EventId is rejected.
- Reactivate the event and verify its selected navigation preference returns.
- Reject duplicate/malformed slugs, non-image banners, missing banner alt text,
  reversed dates, and non-HTTP(S) website URLs. A tampered edit slug is rejected.
- Author/Editor accounts cannot manage events; anonymous access is protected.
  Mutating event requests without a valid antiforgery token are rejected.
- Existing categories, unassigned articles, media uploads, article previews, and
  existing navigation continue to work. Check empty events and >12 articles.
