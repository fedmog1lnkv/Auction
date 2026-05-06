namespace Application.Abstractions.Storage;

public interface IObjectStorage
{
    Task<string> GeneratePresignedUploadUrlAsync(
        string key,
        string contentType,
        TimeSpan ttl,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task<StorageObject> DownloadAsync(string key, CancellationToken cancellationToken = default);
    Task UploadAsync(string key, byte[] content, string contentType, CancellationToken cancellationToken = default);
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);
    string GetPublicUrl(string key);
}