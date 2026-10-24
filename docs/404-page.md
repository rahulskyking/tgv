# Branded 404 page

The public 404 page uses the existing shared layout, purple design tokens,
Barlow Condensed/Inter typography, and theme preference. Its SVG controller
illustration is inline and uses theme tokens: no image downloads, icon library,
or additional runtime JavaScript are needed. Motion is disabled when the user
requests reduced motion.

## Routing

`Program.cs` registers `UseStatusCodePagesWithReExecute("/error/{0}")` before
static files and routing. Empty error responses, including unmatched URLs and
controller `NotFound()` results, are re-executed internally without redirecting
the browser. `ErrorController.HttpStatus` renders the page only for 404 and
explicitly preserves HTTP 404. Other valid error statuses stay unchanged rather
than being incorrectly labelled 404. Responses already containing a body, such
as JSON error results, are left to their original handlers by status-code pages.
The existing exception handler is not replaced by this feature.

The error endpoint allows anonymous access and has no HTTP verb constraint,
so error handling does not turn missing POSTs into 405 responses or require a
login. Error responses are marked non-cacheable. The original requested path
and query are not printed on the page.

View: `Views/Error/NotFound.cshtml`
Illustration: `Views/Shared/Components/_NotFoundIllustration.cshtml`
CSS source: the branded 404 section of `wwwroot/css/app.css`
Compiled CSS: `wwwroot/css/site.css` (`npm run css:build`)

## Local checks

No new database migration is needed for the 404 page. The application's existing
startup migrations still apply normally when starting the whole website.

With the .NET 8 SDK and your normal local configuration:

```sh
dotnet build TheGameVoice.sln
dotnet run --project src/Web/TheGameVoice.Web
```

Use the local HTTPS address printed by the application. Verify in a browser:

- A random missing URL renders the 404 page; the address bar does not redirect.
- Missing article, category, game, and event slugs render the same page.
- `/error/404` previews the 404 page directly and still returns status 404.
- Home, latest-article, and search actions navigate to their existing routes.
- The navbar/footer and theme toggle work through the shared layout.
- Mobile widths (including 320px), light/dark themes, keyboard focus, and reduced
  motion render correctly.
- Existing successful pages and JSON error responses retain their behavior.

Inspect the Network panel: the original missing document must return **404**,
not 200 or a redirect to the home page. You can also check headers using:

```sh
curl -I https://YOUR-LOCAL-HOST/a-page-that-does-not-exist
curl -I https://YOUR-LOCAL-HOST/article/a-slug-that-does-not-exist
```

During implementation, the CSS build and browser rendering of a static fixture
made from the actual view/illustration passed. The fixture covered light/dark
at 320, 375, 768, 1280, and 1920px, horizontal overflow, reduced motion, and
main action target sizes. It does NOT verify Razor compilation or HTTP routing.
Those require the SDK-enabled local checks above; this environment has no .NET SDK.
