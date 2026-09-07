using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheGameVoice.Infrastructure.Persistence.Configurations
{

    public class CloudflareR2StorageOptions
    {
        public string AccountId { get; set; } = default!;

        public string AccessKeyId { get; set; } = default!;

        public string SecretAccessKey { get; set; } = default!;

        public string BucketName { get; set; } = default!;

        public string Endpoint { get; set; } = default!;

        public string PublicUrl { get; set; } = default!;
    }
}
