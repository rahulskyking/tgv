namespace TheGameVoice.Web.Areas.Admin.ViewModels.Media
{
    public class ArticleContentMigrationViewModel
    {
        public int ArticlesScanned { get; set; }
        public int ArticlesWithSupabaseUrls { get; set; }
        public int ImagesFound { get; set; }
        public int ImagesMatched { get; set; }
        public int ImagesUnmatched { get; set; }
        public int ArticlesReadyToMigrate { get; set; }

        public bool HasScanResult { get; set; }

        public List<string> UnmatchedFiles { get; set; } = new();

        //public List<MatchedImageViewModel> MatchedImages { get; set; } = new();
        public List<ReadyArticleViewModel> ReadyArticles { get; set; } = new();
    }

    //public class MatchedImageViewModel
    //{
    //    public Guid ArticleId { get; set; }
    //    public string ArticleTitle { get; set; } = string.Empty;

    //    public string OldUrl { get; set; } = string.Empty;
    //    public string OldFileName { get; set; } = string.Empty;

    //    public Guid MediaId { get; set; }
    //    public string R2Url { get; set; } = string.Empty;
    //}
    public class ReadyArticleViewModel
    {
        public Guid ArticleId { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ImageCount { get; set; }
    }
}
