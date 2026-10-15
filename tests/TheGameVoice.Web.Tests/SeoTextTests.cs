using System.Globalization;
using TheGameVoice.Web.Helpers;
using Xunit;

namespace TheGameVoice.Web.Tests;

public class SeoTextTests
{
    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" \t\n", "")]
    [InlineData("Short gaming summary.", "Short gaming summary.")]
    [InlineData("<p>News &amp; reviews&nbsp; today.</p>", "News & reviews today.")]
    [InlineData("<p>First</p><p>Second<br>Third</p>", "First Second Third")]
    [InlineData("co<strong>op</strong> gaming", "coop gaming")]
    [InlineData("<p>Visible</p><!-- comment --><script>bad()</script><style>.bad{}</style><noscript>hidden</noscript><template>hidden</template>", "Visible")]
    [InlineData("<p>Unclosed <strong>HTML", "Unclosed HTML")]
    public void ExtractsReadableText(string? input, string expected)
    {
        Assert.Equal(expected, SeoText.ToMetaDescription(input));
    }

    [Theory]
    [InlineData("one two three", 7, "one two")]
    [InlineData("one two three", 6, "one")]
    [InlineData("one two", 7, "one two")]
    [InlineData("unbrokenword", 5, "")]
    [InlineData("hello", 0, "")]
    [InlineData("hello", -1, "")]
    [InlineData("go 🎮 now", 4, "go")]
    [InlineData("go e\u0301 now", 4, "go")]
    public void TruncatesWithoutPartialWordsOrUnicodeTextElements(
        string input, int limit, string expected)
    {
        Assert.Equal(expected, SeoText.ToMetaDescription(input, limit));
    }

    [Fact]
    public void UsesDefault155CharacterLimit()
    {
        var input = string.Join(" ", Enumerable.Repeat("gaming", 30));
        var result = SeoText.ToMetaDescription(input);

        Assert.True(result.Length <= 155);
        Assert.Equal(string.Join(" ", Enumerable.Repeat("gaming", 22)), result);
    }

    [Fact]
    public void KeepsAWordEndingExactlyAt155Characters()
    {
        var prefix = new string('a', 150) + " game";
        Assert.Equal(prefix, SeoText.ToMetaDescription(prefix + " news"));
    }

    [Fact]
    public void DoesNotSplitALargeGraphemeCluster()
    {
        var grapheme = "e" + new string('\u0301', 160);
        Assert.Single(StringInfo.ParseCombiningCharacters(grapheme));
        Assert.Empty(SeoText.ToMetaDescription(grapheme));
    }

    [Fact]
    public void PreservesManualDescriptionEvenWhenLongerThan155Characters()
    {
        var editorialCopy = new string('a', 180);
        Assert.Equal(editorialCopy,
            SeoText.ArticleDescription($"  {editorialCopy}  ", "summary", "body"));
    }

    [Theory]
    [InlineData(null, "<p>Summary &amp; news</p>", "Body", "Summary & news")]
    [InlineData("  ", " ", "<p>Body excerpt</p>", "Body excerpt")]
    [InlineData(null, "<script>ignored</script>", "Body", "Body")]
    public void UsesSummaryBeforeContent(
        string? editorialCopy, string? summary, string? body, string expected)
    {
        Assert.Equal(expected, SeoText.ArticleDescription(editorialCopy, summary, body));
    }

    [Fact]
    public void SkipsSummaryWhenItsFirstWordCannotFit()
    {
        Assert.Equal("Body excerpt", SeoText.ArticleDescription(
            null, new string('x', 160), "<p>Body excerpt</p>"));
    }

    [Theory]
    [InlineData(null, null, null)]
    [InlineData(" ", "<p></p>", "<script>ignored</script>")]
    public void UsesSiteFallbackWhenNoDescriptionIsUsable(
        string? editorialCopy, string? summary, string? body)
    {
        Assert.Equal(SeoText.DefaultDescription,
            SeoText.ArticleDescription(editorialCopy, summary, body));
    }
}
