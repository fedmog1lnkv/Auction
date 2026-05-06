namespace Infrastructure.Storage;

internal sealed class S3Options
{
    public string Endpoint { get; init; } = string.Empty;
    public string Region { get; init; } = string.Empty;
    public string Bucket { get; init; } = string.Empty;
    public string AccessKey { get; init; } = string.Empty;
    public string SecretKey { get; init; } = string.Empty;
    public string PublicBaseUrl { get; init; } = string.Empty;
}