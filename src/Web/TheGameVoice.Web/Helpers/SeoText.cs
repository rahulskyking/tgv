using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace TheGameVoice.Web.Helpers;

public static class SeoText
{
    public const string DefaultTitle =
        "TheGameVoice | Gaming News, Reviews and Features";

    public const string DefaultDescription =
        "TheGameVoice is your home for the latest gaming news, " +
        "in-depth reviews, and industry analysis.";

    private static readonly HashSet<string> BlockElements = new(
        StringComparer.OrdinalIgnoreCase)
    {
        "address", "article", "aside", "blockquote", "br", "dd", "div", "dl", "dt",
        "figcaption", "figure", "footer", "h1", "h2", "h3", "h4", "h5", "h6",
        "header", "hr", "li", "main", "nav", "ol", "p", "pre", "section",
        "table", "td", "th", "tr", "ul"
    };

    /// <summary>
    /// Preserves editorial SEO copy; otherwise tries a summary, then body text.
    /// Only generated excerpts are shortened to 155 UTF-16 characters.
    /// </summary>
    public static string ArticleDescription(
        string? seoDescription,
        string? summary,
        string? content)
    {
        if (!string.IsNullOrWhiteSpace(seoDescription))
        {
            return seoDescription.Trim();
        }

        var description = ToMetaDescription(summary);
        if (description.Length == 0)
        {
            description = ToMetaDescription(content);
        }

        return description.Length == 0 ? DefaultDescription : description;
    }

    /// <summary>
    /// Extracts readable text from HTML and truncates on a whole-word boundary.
    /// Returns empty when no complete word fits, allowing the caller to fall back.
    /// Never cuts a Unicode text element (such as an emoji) in half.
    /// </summary>
    public static string ToMetaDescription(string? value, int maxLength = 155)
    {
        if (string.IsNullOrWhiteSpace(value) || maxLength <= 0)
        {
            return string.Empty;
        }

        var document = new HtmlDocument();
        document.LoadHtml(value);

        var builder = new StringBuilder();
        AppendText(document.DocumentNode, builder);
        var text = Regex.Replace(builder.ToString(), @"\s+", " ").Trim();

        if (text.Length <= maxLength)
        {
            return text;
        }

        var cutoff = StringInfo.ParseCombiningCharacters(text)
            .Last(index => index <= maxLength);

        if (cutoff == 0)
        {
            return string.Empty;
        }

        // A word that ends exactly at the limit should not be discarded.
        if (char.IsWhiteSpace(text[cutoff]))
        {
            return text[..cutoff].TrimEnd();
        }

        var lastSpace = text.LastIndexOf(' ', cutoff - 1);
        return lastSpace > 0 ? text[..lastSpace].TrimEnd() : string.Empty;
    }

    private static void AppendText(HtmlNode node, StringBuilder builder)
    {
        if (node.NodeType == HtmlNodeType.Text)
        {
            builder.Append(HtmlEntity.DeEntitize(node.InnerText));
            return;
        }

        if (node.NodeType == HtmlNodeType.Comment
            || node.Name.Equals("script", StringComparison.OrdinalIgnoreCase)
            || node.Name.Equals("style", StringComparison.OrdinalIgnoreCase)
            || node.Name.Equals("noscript", StringComparison.OrdinalIgnoreCase)
            || node.Name.Equals("template", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var isBlock = BlockElements.Contains(node.Name);
        if (isBlock) builder.Append(' ');

        foreach (var child in node.ChildNodes)
        {
            AppendText(child, builder);
        }

        if (isBlock) builder.Append(' ');
    }
}
