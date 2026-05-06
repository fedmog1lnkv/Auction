using System.Net;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Application.Abstractions.Storage;

namespace Infrastructure.Storage;

internal sealed class S3ObjectStorage : IObjectStorage
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucket;
    private readonly string _publicBaseUrl;

    public S3ObjectStorage(S3Options options)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = options.Endpoint,
            ForcePathStyle = true
        };

        if (!string.IsNullOrWhiteSpace(options.Region)) config.AuthenticationRegion = options.Region;

        var credentials = new BasicAWSCredentials(options.AccessKey, options.SecretKey);

        _s3Client = new AmazonS3Client(credentials, config);
        _bucket = options.Bucket;
        _publicBaseUrl = options.PublicBaseUrl.TrimEnd('/');
    }

    public async Task<string> GeneratePresignedUploadUrlAsync(
        string key,
        string contentType,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucket,
            Key = key,
            Verb = HttpVerb.PUT,
            ContentType = contentType,
            Expires = DateTime.UtcNow.Add(ttl)
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3Client.GetObjectMetadataAsync(_bucket, key, cancellationToken);
            return true;
        }
        catch (AmazonS3Exception exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }
    }

    public async Task<StorageObject> DownloadAsync(string key, CancellationToken cancellationToken = default)
    {
        using var response = await _s3Client.GetObjectAsync(_bucket, key, cancellationToken);

        await using var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

        return new StorageObject(
            memoryStream.ToArray(),
            response.Headers.ContentType,
            response.ContentLength);
    }

    public async Task UploadAsync(
        string key,
        byte[] content,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        await using var stream = new MemoryStream(content);

        var request = new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = stream,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(request, cancellationToken);
    }

    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            await _s3Client.DeleteObjectAsync(_bucket, key, cancellationToken);
        }
        catch (AmazonS3Exception exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
        }
    }

    public string GetPublicUrl(string key) => $"{_publicBaseUrl}/{key}";
}