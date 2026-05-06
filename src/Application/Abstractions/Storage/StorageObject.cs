namespace Application.Abstractions.Storage;

public sealed record StorageObject(byte[] Content, string? ContentType, long SizeBytes);