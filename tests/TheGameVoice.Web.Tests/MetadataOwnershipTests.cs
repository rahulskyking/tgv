using System.Text.RegularExpressions;
using Xunit;

namespace TheGameVoice.Web.Tests;

// Source-level regression checks, not a substitute for rendered-HTML checks.
public class MetadataOwnershipTests
{
    private static string ViewsPath
    {
        get
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null && !File.Exists(
                Path.Combine(directory.FullName, "TheGameVoice.sln")))
            {
                directory = directory.Parent;
            }

            return Path.Combine(
                directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."),
                "src", "Web", "TheGameVoice.Web", "Views");
        }
    }

    [Theory]
    [InlineData(@"<title\b")]
    [InlineData("<meta\\s+name=\"description\"")]
    [InlineData("<link\\s+rel=\"canonical\"")]
    [InlineData("<meta\\s+property=\"og:title\"")]
    [InlineData("<meta\\s+property=\"og:description\"")]
    [InlineData("<meta\\s+property=\"og:type\"")]
    public void SharedTagsAreOnlyEmittedOnceInThePublicLayout(string pattern)
    {
        var owners = Directory.GetFiles(ViewsPath, "*.cshtml", SearchOption.AllDirectories)
            .SelectMany(path => Regex.Matches(File.ReadAllText(path), pattern)
                .Select(_ => Path.GetRelativePath(ViewsPath, path).Replace('\\', '/')))
            .ToArray();

        Assert.Equal(new[] { "Shared/_Layout.cshtml" }, owners);
    }

    [Theory]
    [InlineData("Home/Index.cshtml")]
    [InlineData("Home/Privacy.cshtml")]
    [InlineData("News/Index.cshtml")]
    [InlineData("News/Details.cshtml")]
    [InlineData("Pages/About.cshtml")]
    [InlineData("Pages/Contact.cshtml")]
    [InlineData("Pages/ReviewPolicy.cshtml")]
    [InlineData("Categories/Details.cshtml")]
    [InlineData("Tags/Details.cshtml")]
    [InlineData("Games/Details.cshtml")]
    [InlineData("Authors/Details.cshtml")]
    [InlineData("Search/Index.cshtml")]
    [InlineData("Shared/Error.cshtml")]
    public void EveryPublicPageSuppliesTitleAndDescription(string view)
    {
        var source = File.ReadAllText(Path.Combine(ViewsPath, view));
        Assert.Contains("ViewData[\"Title\"] =", source);
        Assert.Contains("ViewData[\"MetaDescription\"] =", source);
    }
}
