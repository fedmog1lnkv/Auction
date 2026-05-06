namespace Application.Abstractions.Storage;

public sealed record ImageVariants(byte[] ThumbWebp, byte[] MediumWebp, byte[] LargeWebp);