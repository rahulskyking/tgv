using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameVoice.Application.Modules
{
    public class ArticleContentImageMapping
    {
        public Guid ArticleId { get; set; }
        public string ArticleTitle { get; set; } = string.Empty;

        public string OldUrl { get; set; } = string.Empty;
        public string OldFileName { get; set; } = string.Empty;

        public Guid MediaId { get; set; }
        public string R2Url { get; set; } = string.Empty;
    }
}
