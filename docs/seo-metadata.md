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
