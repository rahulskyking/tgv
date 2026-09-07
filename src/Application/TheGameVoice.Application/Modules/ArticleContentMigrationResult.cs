using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameVoice.Application.Modules
{
    public class ArticleContentMigrationResult
    {
        public int ArticlesScanned { get; set; }
        public int ArticlesWithSupabaseUrls { get; set; }
        public int ImagesFound { get; set; }
        public int ImagesMatched { get; set; }
        public int ImagesUnmatched { get; set; }
        public int ArticlesReadyToMigrate { get; set; }
        public int ArticlesMigrated { get; set; }
        public int Failed { get; set; }

        public List<string> UnmatchedFiles { get; set; } = new();
        public List<string> Errors { get; set; } = new();
        public List<ArticleContentImageMapping> MatchedImages { get; set; } = new();
    }
}
