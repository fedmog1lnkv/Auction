namespace Application.Abstractions.Storage;

public interface IImageVariantGenerator
{
    Task<ImageVariants> CreateWebpVariantsAsync(
        byte[] originalContent,
        CancellationToken cancellationToken = default);
}