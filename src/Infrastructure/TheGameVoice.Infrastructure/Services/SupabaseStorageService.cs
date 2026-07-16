using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using TheGameVoice.Application.Interfaces.Services;
using TheGameVoice.Infrastructure.Configuration;

namespace TheGameVoice.Infrastructure.Services;

public class SupabaseStorageService
    : IStorageService
{
    private readonly HttpClient _httpClient;

    private readonly SupabaseStorageOptions
        _options;

    public SupabaseStorageService(
        HttpClient httpClient,
        IOptions<SupabaseStorageOptions> options)
    {
        _httpClient = httpClient;

        _options = options.Value;
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

        var uploadUrl =
            $"{_options.ProjectUrl}" +
            $"/storage/v1/object/" +
            $"{_options.BucketName}/" +
            $"{objectPath}";

        using var content =
            new StreamContent(stream);

        content.Headers.ContentType =
            new MediaTypeHeaderValue(contentType);

        var request =
            new HttpRequestMessage(
                HttpMethod.Post,
                uploadUrl);

        request.Headers.Add(
            "apikey",
            _options.ServiceRoleKey);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _options.ServiceRoleKey);

        request.Content = content;

        var response =
            await _httpClient.SendAsync(
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();

        return
            $"{_options.ProjectUrl}" +
            $"/storage/v1/object/public/" +
            $"{_options.BucketName}/" +
            $"{objectPath}";
    }

    public async Task DeleteAsync(
     string filePath,
     CancellationToken cancellationToken = default)
    {
        var uri = new Uri(filePath);

        var marker =
            $"/storage/v1/object/public/{_options.BucketName}/";

        var objectPath =
            uri.AbsolutePath
                .Replace(marker, "");

        var deleteUrl =
            $"{_options.ProjectUrl}" +
            $"/storage/v1/object/" +
            $"{_options.BucketName}/" +
            $"{objectPath}";

        var request =
            new HttpRequestMessage(
                HttpMethod.Delete,
                deleteUrl);

        request.Headers.Add(
            "apikey",
            _options.ServiceRoleKey);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                _options.ServiceRoleKey);

        var response =
            await _httpClient.SendAsync(
                request,
                cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}