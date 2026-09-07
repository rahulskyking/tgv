using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Infrastructure.Configuration;
using TheGameVoice.Infrastructure.Persistence.Configurations;

namespace TheGameVoice.Infrastructure.Services;

public class CloudflareR2StorageService
    : IStorageService, IDisposable
{
    private readonly IAmazonS3 _s3Client;
    private readonly CloudflareR2StorageOptions _options;

    public CloudflareR2StorageService(
        IOptions<CloudflareR2StorageOptions> options)
    {
        _options = options.Value;

        var credentials = new BasicAWSCredentials(
            _options.AccessKeyId,
            _options.SecretAccessKey);

        _s3Client = new AmazonS3Client(
            credentials,
            new AmazonS3Config
            {
                ServiceURL = _options.Endpoint,
                ForcePathStyle = true
            });
    }

    public async Task<string> UploadAsync(
       Stream stream,
       string fileName,
       string contentType,
       CancellationToken cancellationToken = default)
    {
        var uniqueFileName =
            $"{Guid.NewGuid()}_{fileName}";

        var objectPath =
            $"uploads/{uniqueFileName}";

        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(
            memoryStream,
            cancellationToken);

        memoryStream.Position = 0;

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectPath,
            InputStream = memoryStream,
            ContentType = contentType,

            // Important for Cloudflare R2
            UseChunkEncoding = false
        };

        await _s3Client.PutObjectAsync(
            request,
            cancellationToken);

        return
            $"{_options.PublicUrl.TrimEnd('/')}/{objectPath}";
    }

    public async Task DeleteAsync(
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var publicUrl =
            _options.PublicUrl.TrimEnd('/');

        var objectPath =
            filePath.Replace(
                $"{publicUrl}/",
                "");

        var request = new DeleteObjectRequest
        {
            BucketName = _options.BucketName,
            Key = objectPath
        };

        await _s3Client.DeleteObjectAsync(
            request,
            cancellationToken);
    }

    public void Dispose()
    {
        _s3Client.Dispose();
    }
}