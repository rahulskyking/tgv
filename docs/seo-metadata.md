# Public page SEO metadata

## Ownership

`Views/Shared/_Layout.cshtml` renders the public title, meta description,
canonical link (when supplied), and Open Graph title, description, and type.
Do not emit those tags again in a view's `Head` section or a partial.
The separate admin layout is unchanged.

Each public page sets these values in its initial Razor block:

```cshtml
@{
    ViewData["Title"] = "Page purpose | TheGameVoice";
    ViewData["MetaDescription"] = "An accurate, page-specific summary.";
}
```

The layout handles null, empty, and whitespace-only values with the site
fallbacks in `Helpers/SeoText.cs`. Razor encodes attribute and title values;
do not wrap metadata in `Html.Raw`.

## Articles

`Views/News/Details.cshtml` uses `SeoTitle`, then the article title.
Descriptions use editorial `SeoDescription`, then an automatically generated
excerpt from `Summary`, then `Content`, then the site default.

Editorial descriptions are trimmed but not forcibly shortened. Automatic
excerpts use HtmlAgilityPack to extract text, decode entities, omit
script/style/noscript/template content, and preserve word boundaries within
155 UTF-16 characters without splitting Unicode text elements. An overlong
first word yields an empty excerpt, so another fallback can be tried.
Inline formatting does not introduce spaces inside a word; block boundaries
do introduce spaces. Generated excerpts are a fallback, not a replacement
for well-written editorial summaries.

Articles set `OgType` to `article`. Their `Head` section retains social images,
Twitter metadata, and structured data. Canonical URLs still come from the
existing controller/view logic; this change does not alter canonical policy.

## Validation

With the .NET 8 SDK and NuGet access:

```sh
dotnet build TheGameVoice.sln
dotnet test tests/TheGameVoice.Web.Tests/TheGameVoice.Web.Tests.csproj
```

Tests cover text extraction, word/Unicode boundaries, fallback order, editorial
copy preservation, and source-level metadata ownership. Source checks do not
replace rendered-page verification.

After starting the site with its normal database/configuration, inspect **View
Page Source** on the home, news listing, article, category, tag, game, author,
search, and static pages:

- Exactly one title and meta description, with relevant page-specific values.
- At most one canonical link, matching the intended preferred URL.
- One Open Graph title/description/type; articles use `article`.
- Article Twitter descriptions and structured-data descriptions match the
  selected article description.
- Descriptions remain correct with quotes, ampersands, HTML summaries, blank
  SEO fields, and long content.

The privacy view currently contains placeholder policy text. Its metadata is
intentionally modest; replace the placeholder with a real policy before
claiming detailed privacy practices.

Robots rules, sitemap coverage, indexing directives, redirect policy, schema
eligibility, and performance are separate audit work. Metadata alone does not
guarantee rankings, and search engines may rewrite displayed snippets.

## Discovery links and legacy URLs

The preferred article path remains `/article/{slug}`. Sitemap and RSS article
links now use `Url.Action("Details", "News", ...)`, so they follow the actual
MVC route. The sitemap uses `XElement` to escape XML safely and still includes
published articles across both gaming segments. Old `/news/{slug}` links
permanently redirect to the matching article; the lower-priority legacy route
leaves existing conventional routes such as `/news/index` available.

`wwwroot/robots.txt` advertises `https://thegamevoice.com/sitemap.xml` instead
of localhost. This uses the non-www domain already referenced by the project;
if production's preferred hostname is different, update the robots directive
to match. Sitemap/RSS absolute URLs continue to use the request's scheme and
host, like the current article canonicals. Verify those behind the production
proxy; preferred-origin configuration and proxy trust are separate follow-up
work, not changed here.

After deployment, check `/robots.txt`, `/sitemap.xml`, and `/feed`; sitemap and
feed article links must resolve to published articles. Confirm a known old
`/news/{slug}` returns 301 to `/article/{slug}`, `/news` and `/news/index` still
show the listing, and nonexistent article slugs end in 404 rather than a home
page redirect. The sitemap helps discovery but does not guarantee indexing.

## Public URL service registration (controller migration pending)

`Services/PublicSiteUrl.cs` generates absolute URLs with an explicitly configured
HTTPS origin while leaving path generation to MVC. It is registered as a
singleton in `DependencyInjection/WebServiceRegistration.cs`, alongside the
existing MVC, HTTP context accessor, and scoped site-segment services.
`Program.cs` now calls `AddWebServices()`; Infrastructure registrations remain
unchanged and do not depend on Web.

`appsettings.json` defaults `Site:BaseUrl` to `https://thegamevoice.com`.
In Railway, set `Site__BaseUrl=https://thegamevoice.com` to override it.
Only a root HTTPS origin is accepted (no credentials, subpath, query, or
fragment). The configured origin is validated when the service is resolved.

Registration alone does **not** change existing sitemap/canonical/RSS URLs.
The controller and view migration is still to be applied: inject `PublicSiteUrl`
and replace request-derived absolute links with, for example:

```csharp
_publicSiteUrl.Action(Url, "Details", "News", new { slug = article.Slug })
```

For Razor, inject the service with
`@inject TheGameVoice.Web.Services.PublicSiteUrl PublicUrls` and use the same
`Action` method. Keep relative navigation links unchanged. Test the migration
before deployment and inspect generated URLs behind Railway/Cloudflare.
