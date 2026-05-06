using Application.Abstractions.Storage;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace Infrastructure.Storage;

internal sealed class ImageSharpVariantGenerator : IImageVariantGenerator
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    public Task<ImageVariants> CreateWebpVariantsAsync(
        byte[] originalContent,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var format = Image.DetectFormat(originalContent);
        if (format is null)
            throw new InvalidDataException("Unable to detect image format.");

        var contentType = format.DefaultMimeType;
        if (string.IsNullOrWhiteSpace(contentType) || !AllowedContentTypes.Contains(contentType))
            throw new NotSupportedException($"Unsupported image content type '{contentType}'.");

        using var image = Image.Load(originalContent);

        var thumb = ResizeAndEncode(image, 300);
        var medium = ResizeAndEncode(image, 800);
        var large = ResizeAndEncode(image, 1600);

        return Task.FromResult(new ImageVariants(thumb, medium, large));
    }

    private static byte[] ResizeAndEncode(Image source, int maxSide)
    {
        using var clone = source.Clone(context =>
            context.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max,
                Size = new Size(maxSide, maxSide)
            }));

        using var stream = new MemoryStream();
        clone.SaveAsWebp(stream,
            new WebpEncoder
            {
                Quality = 80
            });

        return stream.ToArray();
    }
}